namespace BossMod.Endwalker.Raid.P11NThemis;

public enum OID : uint
{
    Boss = 0x3EEC, // R11.000, x1
    Helper = 0x233C, // R0.500, x28, Helper type
    IllusoryThemis = 0x3EED, // R11.000, x4
    ArcaneSphere = 0x3EEE, // R1.000, x4
    ArcaneSphere1 = 0x3EEF, // R1.000, x4
    ArcaneCylinder = 0x3EF1, // R1.000, x3
    ActorFF38E = 0xFF38E, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x1, EventObj type
}

public enum AID : uint
{
    DivisiveRulingLight = 33203, // Boss->self, 4.3+1.9s cast, single-target, line then it widens
    DivisiveRulingDark = 33204, // Boss->self, 4.3+1.9s cast, single-target, line then the two side lanes
    DivisiveRulingAOE = 33205, // Helper->self, 6.2s cast, range 46 width 16 rect
    DivineRuination = 33206, // Helper->self, 8.8s cast, range 46 width 26 rect
    RipplesOfGloomR = 33207, // Helper->self, 8.8s cast, range 46 width 16 rect
    RipplesOfGloomL = 33208, // Helper->self, 8.8s cast, range 46 width 16 rect
    UpheldRulingLightAOE = 33209, // Boss->players, no cast, range 6 circle, stack
    UpheldRulingDarkAOE = 33210, // Boss->player, no cast, range 13 circle, tankbuster
    LightburstAOE = 33213, // Helper->self, 5.0s cast, range 13 circle
    DarkPerimeterAOE = 33214, // Helper->self, 6.0s cast, range 8-50 donut
    DarkAndLight = 33219, // Boss->self, 4.0s cast, single-target
    SigilsOfDiscord = 33220, // Boss->self, 4.0s cast, single-target
    KatakrisisLight = 33221, // Helper->player, no cast, single-target, discord left unbalanced
    KatakrisisDark = 33222, // Helper->player, no cast, single-target
    EmissarysWill = 33223, // Boss->self, 4.0s cast, range 50 circle
    ShadowedMessengers = 33236, // Boss->self, 4.0s cast, single-target
    DivisiveRulingCloneLight = 33237, // IllusoryThemis->self, 7.7+0.5s cast, single-target
    DivisiveRulingCloneDark = 33238, // IllusoryThemis->self, 7.7+0.5s cast, single-target
    DivisiveRulingCloneAOE = 33239, // Helper->self, 8.2s cast, range 46 width 16 rect
    DivineRuinationClone = 33240, // Helper->self, 11.4s cast, range 46 width 26 rect
    RipplesOfGloomCloneR = 33241, // Helper->self, 11.4s cast, range 46 width 16 rect
    RipplesOfGloomCloneL = 33242, // Helper->self, 11.4s cast, range 46 width 16 rect
    Styx = 33243, // Boss->players, 5.0s cast, range 6 circle, stack, then 4 more hits
    StyxRepeat = 33244, // Boss->players, no cast, range 6 circle
    BlindingLight = 33248, // Boss->self, 3.0s cast, single-target
    BlindingLightAOE = 33249, // Helper->player, 5.2s cast, range 6 circle spread
    Eunomia = 33250, // Boss->self, 5.0s cast, range 50 circle
    Dike = 33251, // Boss->self, 5.0s cast, single-target
    DikeAOE1 = 33252, // Helper->player, 5.2s cast, single-target
    DikeAOE2 = 33253, // Helper->player, 5.2s cast, single-target
    DismissalRulingLight = 33272, // Boss->self, 5.5+0.5s cast, single-target
    DismissalRulingDark = 33273, // Boss->self, 5.5+0.5s cast, single-target
    DismissalRulingLightAOE = 33274, // Helper->self, 6.0s cast, range 40 circle, knockback
    DismissalRulingDarkAOE = 33275, // Helper->self, 6.0s cast, range 40 circle, knockback
    AutoAttack = 33331, // Boss->player, no cast, single-target
    Ability33333 = 33333, // Boss->location, no cast, single-target
    InnerLight = 34547, // Helper->self, 9.5s cast, range 13 circle
    OuterDark = 34548, // Helper->self, 9.5s cast, range 10-50 donut
    Dineis = 34597, // Boss->self, 3.0s cast, single-target
    DineisAOE = 34598, // Helper->location, 4.0s cast, range 4 circle
    KatakrisisLight2 = 34704, // Helper->player, no cast, single-target
    UpheldRulingLight = 34766, // Boss->self, 5.8+0.2s cast, single-target
    UpheldRulingDark = 34767, // Boss->self, 5.8+0.2s cast, single-target
}

public enum SID : uint
{
    LightsAccord = 3553, // none->player
    DarksAccord = 3554, // none->player
    LightsDiscord = 3555, // none->player, balanced by standing on the west half
    DarksDiscord = 3556, // none->player, balanced by standing on the east half
}

public enum IconID : uint
{
    Dike = 218, // player
    Styx = 305, // player
    UpheldRulingLight = 318, // player
    UpheldRulingDark = 463, // player
    BlindingLight = 466, // player
}

public enum TetherID : uint
{
    UpheldRulingDark = 192, // Boss->player
    UpheldRulingLight = 193, // Boss->player
}

class EmissarysWill(BossModule module) : Components.RaidwideCast(module, AID.EmissarysWill);
class Eunomia(BossModule module) : Components.RaidwideCast(module, AID.Eunomia, "Raidwide + bleed");
class DikeAOE1(BossModule module) : Components.SingleTargetCast(module, AID.DikeAOE1);
class DikeAOE2(BossModule module) : Components.SingleTargetCast(module, AID.DikeAOE2);
// Inner Light / Outer Dark are already casting during the knockback but land ~3.5s later, so only the wall (and, during
// Dark and Light, leaving your own sigil) counts as an unsafe landing; otherwise every landing is unsafe and the AI stands still.
// the wall kills on contact and pushes vary, so landings past 18.5 count as the wall (an AST died landing at 18.9)
class DismissalRuling(BossModule module, AID aid) : Components.KnockbackFromCastTarget(module, aid, 11)
{
    private const float MaxLanding = 18.5f;

    public override PointTest DestinationUnsafe(int slot, Actor actor)
    {
        var center = Module.Center;
        VBool wall(WVec p) => VV.GreaterThan((p - center).Length(), new VFloat(MaxLanding));
        if (Module.FindComponent<DarkAndLight>()?.Sigil(slot) is not var (c, r))
            return wall;
        return p => wall(p) | VV.GreaterThan((p - c).Length(), new VFloat(r - 0.5f));
    }
}
class DismissalRulingLight(BossModule module) : DismissalRuling(module, AID.DismissalRulingLightAOE);

// dark: knocked out, then back inside 8y before Outer Dark; until the push lands, a goal near the middle keeps the AI by the boss
// (afterwards Outer Dark's own zone brings it back in)
class DismissalRulingDark(BossModule module) : DismissalRuling(module, AID.DismissalRulingDarkAOE)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        if (Casters.Count > 0)
        {
            var center = Module.Center;
            hints.GoalZones.Add(p => VV.Max(VFloat.Zero, VFloat.One - (p - center).Length() / 8) * 5);
        }
    }
}
// sheet says 13; hit at 11.6 (PLD), every safe player 12.6 or more out over four pulls
class InnerLight(BossModule module) : Components.StandardAOEs(module, AID.InnerLight, 12.5f);
class OuterDark(BossModule module) : Components.StandardAOEs(module, AID.OuterDark, new AOEShapeDonut(8, 50));
class Lightburst(BossModule module) : Components.StandardAOEs(module, AID.LightburstAOE, 13);
class DarkPerimeter(BossModule module) : Components.StandardAOEs(module, AID.DarkPerimeterAOE, new AOEShapeDonut(8, 50));
class Dineis(BossModule module) : Components.StandardAOEs(module, AID.DineisAOE, 4);
class BlindingLight(BossModule module) : Components.SpreadFromCastTargets(module, AID.BlindingLightAOE, 6);

// lines from the boss or a clone: first a 16y-wide line, then either a 26y-wide one (light) or the two lanes beside it (dark)
// the wide one is shown together with the first line; the lanes only after it, since line + lanes would cover everything
class DivisiveRuling(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect _narrow = new(46, 8);
    private static readonly AOEShapeRect _wide = new(46, 13);
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var deadline = _aoes.FirstOrDefault().Activation.AddSeconds(1);
        return _aoes.Where(a => a.Activation < deadline || a.Shape == _wide);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        AOEShape? shape = (AID)spell.Action.ID switch
        {
            AID.DivisiveRulingAOE or AID.DivisiveRulingCloneAOE or AID.RipplesOfGloomR or AID.RipplesOfGloomL or AID.RipplesOfGloomCloneR or AID.RipplesOfGloomCloneL => _narrow,
            AID.DivineRuination or AID.DivineRuinationClone => _wide,
            _ => null
        };
        if (shape != null)
        {
            _aoes.Add(new(shape, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
            _aoes.SortBy(a => a.Activation);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.DivisiveRulingAOE or AID.DivisiveRulingCloneAOE or AID.RipplesOfGloomR or AID.RipplesOfGloomL or AID.RipplesOfGloomCloneR or AID.RipplesOfGloomCloneL or AID.DivineRuination or AID.DivineRuinationClone && _aoes.Count > 0)
        {
            ++NumCasts;
            _aoes.RemoveAt(0);
        }
    }
}

// dark: a 13y tankbuster on the tethered tank, then a donut at that spot to step into
class UpheldRulingDark(BossModule module) : Components.GenericBaitAway(module, AID.UpheldRulingDarkAOE, centerAtTarget: true, damageType: AIHints.PredictedDamageType.Tankbuster)
{
    private static readonly AOEShapeCircle _shape = new(13);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.UpheldRulingDark && WorldState.Actors.Find(caster.Tether.Target) is var target && target != null)
            CurrentBaits.Add(new(caster, target, _shape, Module.CastFinishAt(spell, 0.7f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        base.OnEventCast(caster, spell);
        if ((AID)spell.Action.ID == AID.UpheldRulingDarkAOE)
            CurrentBaits.Clear();
    }
}

// light: a 6y party stack on the tethered player, then a 13y circle at that spot to run out of
class UpheldRulingLight(BossModule module) : Components.UniformStackSpread(module, 6, 0, 4)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.UpheldRulingLight && WorldState.Actors.Find(caster.Tether.Target) is var target && target != null)
            AddStack(target, Module.CastFinishAt(spell, 0.7f));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.UpheldRulingLightAOE)
            Stacks.Clear();
    }
}

// five hits about 1.1s apart on the same stack target
class Styx(BossModule module) : Components.UniformStackSpread(module, 6, 0, 4)
{
    private int _hits;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Styx && WorldState.Actors.Find(spell.TargetID) is var target && target != null)
        {
            _hits = 0;
            AddStack(target, Module.CastFinishAt(spell));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.Styx or AID.StyxRepeat && ++_hits >= 5)
            Stacks.Clear();
    }
}

// discord turns into accord while standing in the matching sigil circle: light west, dark east, both 8y around (88,100) / (112,100)
// (fit to every position sample with its accord/discord status over five pulls); leaving it turns it back, and katakrisis hits anyone in discord every few seconds
class DarkAndLight(BossModule module) : BossComponent(module)
{
    private const float SafeMargin = 1.5f;
    private static readonly (WPos center, float radius) _lightSigil = (new(88, 100), 8);
    private static readonly (WPos center, float radius) _darkSigil = (new(112, 100), 8);
    private readonly int[] _side = new int[PartyState.MaxPartySize]; // -1 light, +1 dark, 0 none
    private bool _active;

    private static (WPos center, float radius) SigilAt(int side) => side < 0 ? _lightSigil : _darkSigil;
    public (WPos center, float radius)? Sigil(int slot) => _active && slot is >= 0 and < PartyState.MaxPartySize && _side[slot] != 0 ? SigilAt(_side[slot]) : null;

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (!_active || _side[slot] == 0)
            return;
        var (center, radius) = SigilAt(_side[slot]);
        var inside = actor.Position.InCircle(center, radius - SafeMargin);
        hints.Add(_side[slot] < 0 ? "Stand in the west (light) circle!" : "Stand in the east (dark) circle!", !inside);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (_active && _side[slot] != 0)
        {
            var (center, radius) = SigilAt(_side[slot]);
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(center, radius - SafeMargin));
        }
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (!_active)
            return;
        if (_side[pcSlot] != 0)
        {
            var (center, radius) = SigilAt(_side[pcSlot]);
            Arena.AddCircle(center, radius, ArenaColor.Safe, 2);
        }
        else
        {
            Arena.AddCircle(_lightSigil.center, _lightSigil.radius, ArenaColor.Object);
            Arena.AddCircle(_darkSigil.center, _darkSigil.radius, ArenaColor.Object);
        }
    }

    public override void OnStatusGain(Actor actor, in ActorStatus status)
    {
        var side = (SID)status.ID switch
        {
            SID.LightsDiscord or SID.LightsAccord => -1,
            SID.DarksDiscord or SID.DarksAccord => 1,
            _ => 0
        };
        if (side != 0 && Raid.TryFindSlot(actor, out var slot))
            _side[slot] = side;
    }

    public override void OnMapEffect(byte index, uint state)
    {
        if (index == 1 && state is 0x00020001 or 0x00080004)
        {
            _active = state == 0x00020001;
            if (!_active)
                Array.Fill(_side, 0);
        }
    }
}

class P11NThemisStates : StateMachineBuilder
{
    public P11NThemisStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<EmissarysWill>()
            .ActivateOnEnter<Eunomia>()
            .ActivateOnEnter<DikeAOE1>()
            .ActivateOnEnter<DikeAOE2>()
            .ActivateOnEnter<DismissalRulingLight>()
            .ActivateOnEnter<DismissalRulingDark>()
            .ActivateOnEnter<InnerLight>()
            .ActivateOnEnter<OuterDark>()
            .ActivateOnEnter<Lightburst>()
            .ActivateOnEnter<DarkPerimeter>()
            .ActivateOnEnter<Dineis>()
            .ActivateOnEnter<BlindingLight>()
            .ActivateOnEnter<DivisiveRuling>()
            .ActivateOnEnter<UpheldRulingDark>()
            .ActivateOnEnter<UpheldRulingLight>()
            .ActivateOnEnter<Styx>()
            .ActivateOnEnter<DarkAndLight>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 940, NameID = 12388, PlanLevel = 90, Incomplete = true)]
public class P11NThemis(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsCircle(20));
