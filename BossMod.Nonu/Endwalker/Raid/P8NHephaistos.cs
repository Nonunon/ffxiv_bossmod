namespace BossMod.Endwalker.Raid.P8NHephaistos;

public enum OID : uint
{
    Boss = 0x3AC5, // R10.000, x2
    Helper = 0x233C, // R0.500, x57, Helper type
    Actor3AC6 = 0x3AC6, // R0.000, x2, Part type
    Actor3AC7 = 0x3AC7, // R0.000, x2, Part type
    Gorgon = 0x3AC8, // R1.200, x2
    Suneater = 0x3ACF, // R7.000, x4
    IllusoryHephaistos = 0x3AEF, // R10.000, x6
    ActorFED4A = 0xFED4A, // R0.500, x2, EventNpc type
    ActorFED4B = 0xFED4B, // R0.500, x1, EventNpc type
    ActorFED4C = 0xFED4C, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x2, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x2, EventObj type
    MagitekArmor = 0x1EB703, // R0.500, x4, EventObj type
}

public enum AID : uint
{
    PetrifactionAOE = 26403, // Helper->self, no cast, range 60 circle, gaze from a gorgon
    GorgospitIllusory = 28631, // IllusoryHephaistos->self, 6.0s cast, range 60 width 10 rect
    QuadrupedalImpactAOE = 28931, // Helper->location, 0.8s cast, range 60 circle, knockback from the landing spot
    Ability28936 = 28936, // Boss->location, no cast, single-target
    VolcanicTorchesAlt = 29150, // Boss->self, 3.0s cast, single-target
    SunforgeSnake = 30956, // Boss->self, 7.0+1.0s cast, single-target
    SunforgePhoenix = 30957, // Boss->self, 7.0+1.0s cast, single-target
    ScorchingFang = 30958, // Helper->self, 8.0s cast, range 42 width 14 rect
    SunsPinion = 30959, // Helper->self, 8.0s cast, range 14 width 42 rect
    ChthonicVent = 30960, // Boss->self, 3.0s cast, single-target
    AbyssalFires = 30961, // Helper->location, 5.0s cast, range 60 circle, proximity from a corner
    FourfoldFires = 30962, // Boss->self, 3.0s cast, single-target
    ChthonicVentMoveDiag = 30963, // Suneater->self, no cast, single-target
    ChthonicVentMoveNear = 30964, // Suneater->self, no cast, single-target
    ChthonicVentAOE1 = 30965, // Helper->self, 5.0s cast, range 23 circle
    ChthonicVentAOE2 = 30966, // Suneater->self, 2.0s cast, range 23 circle
    VolcanicTorches = 30967, // Boss->self, 3.0s cast, single-target
    TorchFlame = 30968, // Helper->self, 10.0s cast, range 10 width 10 rect
    IntoTheShadows = 30971, // Boss->self, 3.0s cast, single-target
    Petrifaction = 30972, // Gorgon->self, 5.0s cast, single-target
    Gorgoneion = 30973, // Gorgon->self, 15.0s cast, range 60 circle
    Ektothermos = 30974, // Boss->self, 5.0s cast, range 60 circle
    Gorgospit = 30975, // Boss->self, 3.0s cast, range 60 width 10 rect
    BlazingFootfalls = 30976, // Boss->location, 10.0+0.6s cast, single-target
    QuadrupedalImpact = 30978, // Boss->location, no cast, single-target
    QuadrupedalCrush = 30980, // Boss->location, no cast, single-target
    GenesisOfFlame = 30981, // Boss->self, 5.0s cast, range 60 circle
    HemitheossFlare = 30982, // Boss->self, 3.0s cast, single-target
    HemitheossFlareAOE = 30983, // Helper->players, 5.0s cast, range 6 circle spread
    Flameviper = 30984, // Boss->self/player, 5.0s cast, range 60 width 5 rect
    AutoAttack = 30985, // Boss->player, no cast, single-target
    TrailblazePreview = 30987, // Helper->self, 2.5s cast, range 42 width 42 rect, covers the whole arena
    QuadrupedalImpactMarker = 30988, // Helper->location, 2.5s cast, range 60 circle, marks the next landing spot in order
    Trailblaze = 30989, // Helper->self, 0.6s cast, range 42 width 42 rect
    AutoAttack1 = 30990, // Boss->player, no cast, single-target
    AutoAttack2 = 30991, // Boss->player, no cast, single-target
    ReforgedReflectionCentaur = 31051, // Boss->self, 3.0s cast, single-target
    ReforgedReflectionSnake = 31052, // Boss->self, 3.0s cast, single-target
    ChthonicVentAOE3 = 31053, // Helper->self, 2.0s cast, range 23 circle
    RearingRampage = 31147, // Boss->self, 5.0s cast, range 60 circle
}

public enum IconID : uint
{
    HemitheossFlare = 139, // player
    Flameviper = 230, // player
}

public enum TetherID : uint
{
    ChthonicVent = 5, // Helper->Boss, the helper sits on the first vent
}

class ScorchingFang(BossModule module) : Components.StandardAOEs(module, AID.ScorchingFang, new AOEShapeRect(42, 7));
class SunsPinion(BossModule module) : Components.StandardAOEs(module, AID.SunsPinion, new AOEShapeRect(14, 21));
class AbyssalFires(BossModule module) : Components.ProximityAOEs(module, AID.AbyssalFires, 15);
class TorchFlame(BossModule module) : Components.StandardAOEs(module, AID.TorchFlame, new AOEShapeRect(10, 5));
class Gorgoneion(BossModule module) : Components.CastHint(module, AID.Gorgoneion, "Kill the gorgons!");
class Gorgons(BossModule module) : Components.Adds(module, (uint)OID.Gorgon, 1);
class Ektothermos(BossModule module) : Components.RaidwideCast(module, AID.Ektothermos);
class Gorgospit(BossModule module) : Components.StandardAOEs(module, AID.Gorgospit, new AOEShapeRect(60, 5));
class GorgospitIllusory(BossModule module) : Components.StandardAOEs(module, AID.GorgospitIllusory, new AOEShapeRect(60, 5));
class GenesisOfFlame(BossModule module) : Components.RaidwideCast(module, AID.GenesisOfFlame);
class Trailblaze(BossModule module) : Components.RaidwideCast(module, AID.TrailblazePreview, "Raidwide (charge across the arena)");
class RearingRampage(BossModule module) : Components.RaidwideCast(module, AID.RearingRampage);
class HemitheossFlare(BossModule module) : Components.SpreadFromCastTargets(module, AID.HemitheossFlareAOE, 6);
class Flameviper(BossModule module) : Components.BaitAwayCast(module, AID.Flameviper, new AOEShapeRect(60, 2.5f));

// each gorgon's petrifaction gaze goes off about 1.5s after its cast ends
class Petrifaction(BossModule module) : Components.GenericGaze(module, AID.PetrifactionAOE)
{
    private readonly List<Eye> _eyes = [];

    public override IEnumerable<Eye> ActiveEyes(int slot, Actor actor) => _eyes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Petrifaction)
            _eyes.Add(new(caster.Position, Module.CastFinishAt(spell, 1.5f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.PetrifactionAOE)
        {
            ++NumCasts;
            _eyes.RemoveAll(e => e.Position.AlmostEqual(caster.Position, 1));
        }
    }
}

// the first vents are known from tethers 4s before their casts; each suneater then jumps 30y (or 42.4y diagonally) along its facing to the next one
class ChthonicVent(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle _shape = new(23);
    private readonly List<WPos> _centers = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _centers.Select(c => new AOEInstance(_shape, c));

    public override void OnTethered(Actor source, in ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.ChthonicVent)
            Add(source.Position);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.ChthonicVentAOE1 or AID.ChthonicVentAOE2 or AID.ChthonicVentAOE3)
            Add(caster.Position);
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.ChthonicVentMoveNear:
                Add(caster.Position + caster.Rotation.ToDirection() * 30);
                break;
            case AID.ChthonicVentMoveDiag:
                Add(caster.Position + caster.Rotation.ToDirection() * 42.426407f);
                break;
            case AID.ChthonicVentAOE1:
            case AID.ChthonicVentAOE2:
            case AID.ChthonicVentAOE3:
                ++NumCasts;
                _centers.RemoveAll(c => c.AlmostEqual(caster.Position, 2));
                break;
        }
    }

    private void Add(WPos pos)
    {
        if (!_centers.Any(c => c.AlmostEqual(pos, 2)))
            _centers.Add(pos);
    }
}

// centaur: a marker cast announces each landing spot in order, and the landing knocks everyone 20y away from it about 6.7s later
class QuadrupedalImpact(BossModule module) : Components.Knockback(module, AID.QuadrupedalImpactAOE)
{
    private readonly List<(WPos pos, DateTime activation)> _spots = [];

    public override IEnumerable<Source> Sources(int slot, Actor actor) => _spots.Take(1).Select(s => new Source(s.pos, 20, s.activation));

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.QuadrupedalImpactMarker)
            _spots.Add((spell.LocXZ, Module.CastFinishAt(spell, 6.7f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.QuadrupedalImpactAOE && _spots.Count > 0)
        {
            ++NumCasts;
            _spots.RemoveAt(0);
        }
    }
}

class P8NHephaistosStates : StateMachineBuilder
{
    public P8NHephaistosStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ScorchingFang>()
            .ActivateOnEnter<SunsPinion>()
            .ActivateOnEnter<AbyssalFires>()
            .ActivateOnEnter<ChthonicVent>()
            .ActivateOnEnter<TorchFlame>()
            .ActivateOnEnter<Petrifaction>()
            .ActivateOnEnter<Gorgoneion>()
            .ActivateOnEnter<Gorgons>()
            .ActivateOnEnter<Ektothermos>()
            .ActivateOnEnter<Gorgospit>()
            .ActivateOnEnter<GorgospitIllusory>()
            .ActivateOnEnter<GenesisOfFlame>()
            .ActivateOnEnter<Trailblaze>()
            .ActivateOnEnter<QuadrupedalImpact>()
            .ActivateOnEnter<RearingRampage>()
            .ActivateOnEnter<HemitheossFlare>()
            .ActivateOnEnter<Flameviper>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 883, NameID = 11399, PlanLevel = 90, Incomplete = true)]
public class P8NHephaistos(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsSquare(20));
