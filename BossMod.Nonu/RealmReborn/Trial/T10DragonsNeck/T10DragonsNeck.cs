namespace BossMod.RealmReborn.Trial.T10DragonsNeck;

public enum OID : uint
{
    Boss = 0xD27, // R5.100, x1
    Typhon = 0xD26, // R4.500, x1
    Helper = 0xD28, // R0.500, x12, Helper type
    UltrosHelper = 0xD29, // R0.500, x4, Helper type
    WeightHelper = 0xD2D, // R1.000, x4, Helper type
    Tentacle = 0xD2A, // R7.200, x4
    TyphonSnortsault = 0xD2B, // R1.000, x1, spawn during fight
    TyphonSnortsault2 = 0xD2C, // R1.500, x1, spawn during fight
    SummaRudis = 0xD2F, // R0.500, x4
}

public enum AID : uint
{
    AutoAttackTyphon = 870, // Typhon->player, no cast, single-target
    AutoAttackUltros = 872, // Boss->player, no cast, single-target
    SevereSnort = 3117, // Typhon->self, 10.0s cast, range 40 circle, knocks everyone off the platform
    SnortsaultVisual = 3119, // Typhon->self, 5.0s cast, single-target
    SnortsaultCone = 3120, // Helper->self, no cast, range 20+R 60-degree cone, 2 opposite cones rotating
    SnortsaultCenter = 3121, // Helper->self, no cast, range 5 circle
    Fungah = 3125, // Typhon->self, no cast, range 8+R 90-degree cone, tank knockback
    Fungahhh = 3126, // Typhon->self, 2.5s cast, range 8+R 90-degree cone
    Fireball = 3127, // Typhon->location, 3.5s cast, range 8 circle
    Grind = 3128, // Typhon->player, no cast, single-target
    BosomBuddies = 3129, // Typhon->Boss, no cast, single-target
    FourTonzeWeightVisual = 3130, // Boss->self, 2.5s cast, single-target
    FourTonzeWeight = 3131, // WeightHelper->self, 2.5s cast, range 4 circle
    ImpSong = 3132, // Boss->self, no cast, range 40 circle
    AquaBreath = 3133, // Boss->self, no cast, range 8+R 90-degree cone
    Megavolt = 3134, // Boss->self, 3.5s cast, range 6+R circle
    TentacleVisual = 3135, // Boss->self, no cast, single-target
    Wallop = 3136, // Tentacle->self, 3.0s cast, range 19+R width 8 rect
    ImpPunch = 3139, // player->Typhon/Boss, no cast, single-target, the only action imps have
}

public enum SID : uint
{
    Stun = 149, // none->player, extra=0x0
    Invigoration = 611, // none->Typhon/Helper, extra=stacks, 8 at severe snort start
    WetPlate = 612, // none->player, extra=stacks, from aqua breath while an imp
    Imp = 613, // Boss->player, extra=0x6, not cleansable (esuna does nothing); ends when it times out or when all 8 invigoration stacks are punched off
    OutOfTheAction = 626, // none->player, extra=0x0
}

// 15y from the centre, stopped only by the outer wall, so standing within 5y of the centre keeps you on the platform
class SevereSnort(BossModule module) : Components.KnockbackFromCastTarget(module, AID.SevereSnort, 15)
{
    public const float SafeRadius = 5;

    public override void DrawArenaBackground(int pcSlot, Actor pc)
    {
        if (Casters.Count > 0)
            Arena.ZoneCircle(Arena.Center, SafeRadius, ArenaColor.SafeFromAOE);
    }
}
// TODO: fungah knocks the tank off the platform; the tank could face typhon toward the center to stay on top
class Fungahhh(BossModule module) : Components.StandardAOEs(module, AID.Fungahhh, new AOEShapeCone(12.5f, 45.Degrees()));
class Fireball(BossModule module) : Components.StandardAOEs(module, AID.Fireball, 8);
class FourTonzeWeight(BossModule module) : Components.StandardAOEs(module, AID.FourTonzeWeight, 4);

// ultros alternates aqua breath with his other moves: it lands 4.7-9s after megavolt, imp song or 4-tonze weight resolves, so the cleave only shows in that window
// imps want to stand in it: each hit gives a wet plate stack, which lets imp punch strip typhon's invigoration
class AquaBreath(BossModule module) : Components.Cleave(module, AID.AquaBreath, new AOEShapeCone(13.1f, 45.Degrees()))
{
    private DateTime _windowStart = DateTime.MaxValue;

    private bool Active => WorldState.CurrentTime >= _windowStart;

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        base.OnEventCast(caster, spell);
        switch ((AID)spell.Action.ID)
        {
            case AID.Megavolt:
            case AID.ImpSong:
            case AID.FourTonzeWeightVisual:
                _windowStart = WorldState.FutureTime(3.5f);
                NextExpected = WorldState.FutureTime(4.7f);
                break;
            case AID.AquaBreath:
                _windowStart = DateTime.MaxValue;
                break;
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (Active && !Imps.WantsWetPlate(actor))
            base.AddHints(slot, actor, hints);
    }

    // imps that still need wet plate get the cone in green, aimed at ultros' target, for as long as they need it
    private bool WantsBreath(Actor actor) => Imps.WantsWetPlate(actor) && Module.Enemies(OID.Typhon).All(t => t.FindStatus(SID.Invigoration) == null);

    public override void DrawArenaBackground(int pcSlot, Actor pc)
    {
        if (WantsBreath(pc))
            foreach (var (origin, _, angle) in OriginsAndTargets())
                Arena.ZoneCone(origin.Position, 0, Shape is AOEShapeCone c ? c.Radius : 0, angle, 45.Degrees(), ArenaColor.SafeFromAOE);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (Active || WantsBreath(pc))
            base.DrawArenaForeground(pcSlot, pc);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!Imps.WantsWetPlate(actor))
        {
            if (Active)
                base.AddAIHints(slot, actor, assignment, hints);
        }
        else if (WantsBreath(actor))
        {
            foreach (var (origin, _, angle) in OriginsAndTargets())
                hints.GoalZones.Add(AIHints.GoalSingleTarget(origin.Position + (origin.HitboxRadius + 3) * angle.ToDirection(), 3, 10));
        }
    }
}
class Megavolt(BossModule module) : Components.StandardAOEs(module, AID.Megavolt, 11.1f);
class Wallop(BossModule module) : Components.StandardAOEs(module, AID.Wallop, new AOEShapeRect(26.2f, 4));

// two opposite cones always start at 6 degrees and turn a full circle in 56 steps, with a 5y circle under typhon on every step
class Snortsault(BossModule module) : Components.GenericRotatingAOE(module)
{
    private static readonly AOEShapeCone _cone = new(20.5f, 30.Degrees());
    private static readonly AOEShapeCircle _center = new(5);
    private const int NumSteps = 56;
    private const float StepDelay = 0.625f;
    private DateTime _centerActivation;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        foreach (var aoe in base.ActiveAOEs(slot, actor))
            yield return aoe;
        if (Sequences.Count > 0)
            yield return new(_center, Arena.Center, default, _centerActivation);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.SnortsaultVisual)
        {
            _centerActivation = Module.CastFinishAt(spell, 2.7f);
            var increment = (360f / NumSteps).Degrees();
            Sequences.Add(new(_cone, Arena.Center, 6.Degrees(), increment, _centerActivation, StepDelay, NumSteps, 3));
            Sequences.Add(new(_cone, Arena.Center, 186.Degrees(), increment, _centerActivation, StepDelay, NumSteps, 3));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.SnortsaultCone:
                // resync to the real rotation, so small timing or angle drift doesn't add up over 56 steps
                var index = Sequences.FindIndex(s => s.Rotation.AlmostEqual(spell.Rotation, 0.3f));
                if (index >= 0)
                {
                    Sequences.AsSpan()[index].Rotation = spell.Rotation;
                    AdvanceSequence(index, WorldState.CurrentTime);
                }
                break;
            case AID.SnortsaultCenter:
                _centerActivation = WorldState.FutureTime(StepDelay);
                break;
        }
    }
}

// the platform is a 20y circle with a 1.4y ledge; falling off stuns and the only way back up is jumping from the lower ring
class Ledge(BossModule module) : BossComponent(module)
{
    private const float LowerRingHeight = 18.4f;

    private bool OnLowerRing(Actor actor) => actor.PosRot.Y < LowerRingHeight && !actor.Position.InCircle(Arena.Center, 19);

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (OnLowerRing(actor))
            hints.Add("Jump back onto the platform!", false);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!OnLowerRing(actor) || actor.FindStatus(SID.Stun) != null || !actor.Position.InCircle(Arena.Center, 21.2f))
            return;
        var toCenter = Arena.Center - actor.Position;
        var moved = actor.Position - actor.PrevPosition;
        if (moved.Dot(toCenter) > 0)
            hints.WantJump = true;
    }
}

// imp song turns the dps into imps until severe snort; imps with wet plate punch typhon during it, each punch strips one of 8 invigoration stacks, all 8 stops the cast
class Imps(BossModule module) : BossComponent(module)
{
    public const int WantedWetPlates = 2;

    public static int WetPlates(Actor actor) => actor.FindStatus(SID.WetPlate)?.Extra ?? 0;
    public static bool WantsWetPlate(Actor actor) => actor.FindStatus(SID.Imp) != null && WetPlates(actor) < WantedWetPlates;

    private Actor? InvigoratedTyphon() => Module.Enemies(OID.Typhon).FirstOrDefault(t => t.FindStatus(SID.Invigoration) != null);

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (actor.FindStatus(SID.Imp) == null)
            return;
        if (InvigoratedTyphon() != null)
            hints.Add(WetPlates(actor) > 0 ? "Imp Punch Typhon!" : "Stay close to Typhon!", false);
        else if (WantsWetPlate(actor))
            hints.Add("Get hit by Aqua Breath!", false);
    }

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (InvigoratedTyphon()?.FindStatus(SID.Invigoration) is { } inv)
            hints.Add($"Invigoration stacks: {inv.Extra}");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (actor.FindStatus(SID.Imp) == null || InvigoratedTyphon() is not { } typhon || WetPlates(actor) == 0)
            return;
        hints.ForcedTarget = typhon;
        hints.GoalZones.Add(AIHints.GoalSingleTarget(Arena.Center, SevereSnort.SafeRadius - 1, 10));
        hints.ActionsToExecute.Push(ActionID.MakeSpell(AID.ImpPunch), typhon, ActionQueue.Priority.High);
    }
}

class T10DragonsNeckStates : StateMachineBuilder
{
    public T10DragonsNeckStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<SevereSnort>()
            .ActivateOnEnter<Fungahhh>()
            .ActivateOnEnter<Fireball>()
            .ActivateOnEnter<FourTonzeWeight>()
            .ActivateOnEnter<AquaBreath>()
            .ActivateOnEnter<Megavolt>()
            .ActivateOnEnter<Wallop>()
            .ActivateOnEnter<Snortsault>()
            .ActivateOnEnter<Ledge>()
            .ActivateOnEnter<Imps>()
            .Raw.Update = () => module.PrimaryActor.IsDeadOrDestroyed && module.Enemies(OID.Typhon).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 81, NameID = 3047, Incomplete = true)]
public class T10DragonsNeck(ModuleInit init) : BossModule(init, new(-258.1f, 18.1f), new ArenaBoundsCircle(20))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor, ArenaColor.Enemy);
        Arena.Actors(Enemies(OID.Typhon), ArenaColor.Enemy);
    }
}
