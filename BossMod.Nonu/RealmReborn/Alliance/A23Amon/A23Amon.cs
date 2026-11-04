namespace BossMod.RealmReborn.Alliance.A23Amon;

public enum OID : uint
{
    Boss = 0xBE0, // R4.000, x1
    Helper = 0x1B2, // R0.500, x5
    IceCage = 0xBE1, // R1.800, spawns on players hit by midwinter tragedy
    DimensionalCompression = 0xBE2, // R1.000, tethered to a player, shrinks whatever it reaches
    ExperimentalByProduct33 = 0xBE3, // R1.000, casts the last song (raidwide silence) if not killed
    ExperimentalByProduct66 = 0xBE4, // R1.000, casts the last bout (raidwide pacification) if not killed
    Kichiknebik = 0xBE6, // R2.400, grows over time
    KumKum = 0xBE7, // R1.000-2.000, grows over time
}

public enum AID : uint
{
    AutoAttack = 872, // Boss/ExperimentalByProduct33/ExperimentalByProduct66->player, no cast, single-target
    AutoAttackKichiknebik = 870, // Kichiknebik->player, no cast, single-target
    Darkness = 1875, // KumKum->self, 1.0s cast, range 6+R 120-degree cone
    Mini = 2371, // DimensionalCompression->player/KumKum/Kichiknebik, no cast, single-target, applies minimum
    TheLastBout = 2372, // ExperimentalByProduct66->self, no cast, range 60 circle
    TheLastSong = 2374, // ExperimentalByProduct33->self, no cast, range 60 circle
    Coloratura = 2379, // Boss->self, no cast, range 8+R 120-degree cone
    FiragaForte = 2380, // Boss->self, no cast, range 60 circle, raidwide
    BlizzagaForte = 2382, // Boss->self, 3.0s cast, range 6+R circle
    MidwinterTragedy = 2385, // Boss->self, no cast, freezes icon targets into ice cages
    RanineComedy = 2386, // Boss->self, no cast, turns a few random players into toads
    CurtainCall = 2441, // Boss->self, 15.0s cast, range 60 circle, line of sight blocked by ice cages
}

public enum SID : uint
{
    Silence = 7, // ExperimentalByProduct33->player, extra=0x0
    Heavy = 14, // Boss->player, extra=0x32
    Minimum = 438, // DimensionalCompression->player/KumKum/Kichiknebik, extra=0x32
    FireToad = 511, // Boss->player, extra=0x2
    DeepFreeze = 3479, // Boss->player, extra=0x1
}

public enum IconID : uint
{
    MidwinterTragedy = 3, // player, freeze ~3s later
    DimensionalCompression = 27, // player, tethered orb spawns ~3s later
}

public enum TetherID : uint
{
    DimensionalCompression = 1, // DimensionalCompression->player
}

class Coloratura(BossModule module) : Components.Cleave(module, AID.Coloratura, new AOEShapeCone(12, 60.Degrees()));
class BlizzagaForte(BossModule module) : Components.StandardAOEs(module, AID.BlizzagaForte, new AOEShapeCircle(10));
class Darkness(BossModule module) : Components.StandardAOEs(module, AID.Darkness, new AOEShapeCone(8, 60.Degrees()));

class CurtainCall(BossModule module) : Components.CastLineOfSightAOE(module, AID.CurtainCall, 60, false)
{
    public override IEnumerable<Actor> BlockerActors() => Module.Enemies(OID.IceCage).Where(c => !c.IsDead);

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.IceCage)
            Refresh();
    }

    public override void OnActorDestroyed(Actor actor)
    {
        if ((OID)actor.OID == OID.IceCage)
            Refresh();
    }

    public override void OnIsDeadChanged(Actor actor)
    {
        if ((OID)actor.OID == OID.IceCage)
            Refresh();
    }
}

class IceCageHint(BossModule module) : BossComponent(module)
{
    private BitMask _targets;

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == (uint)IconID.MidwinterTragedy)
            _targets.Set(Raid.FindSlot(actor.InstanceID));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.MidwinterTragedy)
            _targets.Reset();
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_targets[slot])
            hints.Add("You will be frozen into an ice cage!", false);
    }

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (Module.Enemies(OID.IceCage).Any(c => !c.IsDead))
            hints.Add("Keep ice cages alive for curtain call");
    }
}

class DimensionalCompression(BossModule module) : BossComponent(module)
{
    private readonly List<Actor> _orbs = [];

    private IEnumerable<Actor> GrowingAdds => Module.Enemies(OID.Kichiknebik).Concat(Module.Enemies(OID.KumKum)).Where(a => !a.IsDead && a.FindStatus(SID.Minimum) == null);

    public override void OnTethered(Actor source, in ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.DimensionalCompression && !_orbs.Contains(source))
            _orbs.Add(source);
    }

    public override void OnUntethered(Actor source, in ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.DimensionalCompression)
            _orbs.Remove(source);
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_orbs.Any(o => o.Tether.Target == actor.InstanceID))
            hints.Add(GrowingAdds.Any() ? "Lead the orb into an add!" : "Orb will shrink you!");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!_orbs.Any(o => o.Tether.Target == actor.InstanceID))
            return;
        if (GrowingAdds.MinBy(a => (a.Position - actor.Position).LengthSq()) is { } add)
            hints.GoalZones.Add(AIHints.GoalSingleTarget(add.Position, add.HitboxRadius + 1, 5));
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        foreach (var o in _orbs)
        {
            Arena.Actor(o, ArenaColor.Object, true);
            if (o.Tether.Target == pc.InstanceID)
            {
                Arena.AddLine(o.Position, pc.Position, ArenaColor.Danger);
                foreach (var a in GrowingAdds)
                    Arena.AddCircle(a.Position, a.HitboxRadius + 1, ArenaColor.Safe);
            }
        }
    }
}

class ByProducts(BossModule module) : Components.AddsMulti(module, [OID.ExperimentalByProduct33, OID.ExperimentalByProduct66], 2);
class Adds(BossModule module) : Components.AddsMulti(module, [OID.Kichiknebik, OID.KumKum], 1);

class A23AmonStates : StateMachineBuilder
{
    public A23AmonStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Coloratura>()
            .ActivateOnEnter<BlizzagaForte>()
            .ActivateOnEnter<Darkness>()
            .ActivateOnEnter<CurtainCall>()
            .ActivateOnEnter<IceCageHint>()
            .ActivateOnEnter<DimensionalCompression>()
            .ActivateOnEnter<ByProducts>()
            .ActivateOnEnter<Adds>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 102, NameID = 2821, SortOrder = 7)]
public class A23Amon(ModuleInit init) : BossModule(init, new(0, -200), new ArenaBoundsCircle(30));
