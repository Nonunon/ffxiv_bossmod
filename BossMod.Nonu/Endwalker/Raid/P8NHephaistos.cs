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
    Petrifaction = 26403, // Helper->self, no cast, range 60 circle
    QuadrupedalImpact = 28931, // Helper->location, 0.8s cast, range 60 circle
    Ability28936 = 28936, // Boss->location, no cast, single-target
    Sunforge = 30956, // Boss->self, 7.0+1.0s cast, single-target
    Sunforge1 = 30957, // Boss->self, 7.0+1.0s cast, single-target
    ScorchingFang = 30958, // Helper->self, 8.0s cast, range 42 width 14 rect
    SunsPinion = 30959, // Helper->self, 8.0s cast, range 14 width 42 rect
    ChthonicVent = 30960, // Boss->self, 3.0s cast, single-target
    AbyssalFires = 30961, // Helper->location, 5.0s cast, range 60 circle
    FourfoldFires = 30962, // Boss->self, 3.0s cast, single-target
    ChthonicVent1 = 30963, // Suneater->self, no cast, single-target
    ChthonicVent2 = 30964, // Suneater->self, no cast, single-target
    ChthonicVent3 = 30965, // Helper->self, 5.0s cast, range 23 circle
    ChthonicVent4 = 30966, // Suneater->self, 2.0s cast, range 23 circle
    VolcanicTorches = 30967, // Boss->self, 3.0s cast, single-target
    TorchFlame = 30968, // Helper->self, 10.0s cast, range 10 width 10 rect
    IntoTheShadows = 30971, // Boss->self, 3.0s cast, single-target
    Petrifaction1 = 30972, // Gorgon->self, 5.0s cast, single-target
    Gorgoneion = 30973, // Gorgon->self, 15.0s cast, range 60 circle
    Ektothermos = 30974, // Boss->self, 5.0s cast, range 60 circle
    Gorgospit = 30975, // Boss->self, 3.0s cast, range 60 width 10 rect
    BlazingFootfalls = 30976, // Boss->location, 10.0+0.6s cast, single-target
    QuadrupedalImpact1 = 30978, // Boss->location, no cast, single-target
    QuadrupedalCrush = 30980, // Boss->location, no cast, single-target
    GenesisOfFlame = 30981, // Boss->self, 5.0s cast, range 60 circle
    HemitheossFlare = 30982, // Boss->self, 3.0s cast, single-target
    HemitheossFlare1 = 30983, // Helper->players, 5.0s cast, range 6 circle
    Flameviper = 30984, // Boss->self/player, 5.0s cast, range 60 width 5 rect
    AutoAttack = 30985, // Boss->player, no cast, single-target
    Ability30987 = 30987, // Helper->self, 2.5s cast, range 42 width 42 rect
    Ability30988 = 30988, // Helper->location, 2.5s cast, range 60 circle
    Trailblaze = 30989, // Helper->self, 0.6s cast, range 42 width 42 rect
    AutoAttack1 = 30990, // Boss->player, no cast, single-target
    AutoAttack2 = 30991, // Boss->player, no cast, single-target
    ReforgedReflection = 31051, // Boss->self, 3.0s cast, single-target
    ReforgedReflection1 = 31052, // Boss->self, 3.0s cast, single-target
    ChthonicVent5 = 31053, // Helper->self, 2.0s cast, range 23 circle
    RearingRampage = 31147, // Boss->self, 5.0s cast, range 60 circle
}

public enum IconID : uint
{
    Icon139 = 139, // player
    Icon230 = 230, // player
}

public enum TetherID : uint
{
    Tether5 = 5, // Helper->Boss
}

class QuadrupedalImpact(BossModule module) : Components.RaidwideCast(module, AID.QuadrupedalImpact);
class ScorchingFang(BossModule module) : Components.StandardAOEs(module, AID.ScorchingFang, new AOEShapeRect(42, 7));
class SunsPinion(BossModule module) : Components.StandardAOEs(module, AID.SunsPinion, new AOEShapeRect(14, 21));
class AbyssalFires(BossModule module) : Components.RaidwideCast(module, AID.AbyssalFires);
class ChthonicVent3(BossModule module) : Components.StandardAOEs(module, AID.ChthonicVent3, 23);
class ChthonicVent4(BossModule module) : Components.StandardAOEs(module, AID.ChthonicVent4, 23);
class TorchFlame(BossModule module) : Components.StandardAOEs(module, AID.TorchFlame, new AOEShapeRect(10, 5));
class Gorgoneion(BossModule module) : Components.RaidwideCast(module, AID.Gorgoneion);
class Ektothermos(BossModule module) : Components.RaidwideCast(module, AID.Ektothermos);
class Gorgospit(BossModule module) : Components.StandardAOEs(module, AID.Gorgospit, new AOEShapeRect(60, 5));
class GenesisOfFlame(BossModule module) : Components.RaidwideCast(module, AID.GenesisOfFlame);
class Ability30987(BossModule module) : Components.StandardAOEs(module, AID.Ability30987, new AOEShapeRect(42, 21));
class Ability30988(BossModule module) : Components.RaidwideCast(module, AID.Ability30988);
class Trailblaze(BossModule module) : Components.StandardAOEs(module, AID.Trailblaze, new AOEShapeRect(42, 21));
class ChthonicVent5(BossModule module) : Components.StandardAOEs(module, AID.ChthonicVent5, 23);
class RearingRampage(BossModule module) : Components.RaidwideCast(module, AID.RearingRampage);

class P8NHephaistosStates : StateMachineBuilder
{
    public P8NHephaistosStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<QuadrupedalImpact>()
            .ActivateOnEnter<ScorchingFang>()
            .ActivateOnEnter<SunsPinion>()
            .ActivateOnEnter<AbyssalFires>()
            .ActivateOnEnter<ChthonicVent3>()
            .ActivateOnEnter<ChthonicVent4>()
            .ActivateOnEnter<TorchFlame>()
            .ActivateOnEnter<Gorgoneion>()
            .ActivateOnEnter<Ektothermos>()
            .ActivateOnEnter<Gorgospit>()
            .ActivateOnEnter<GenesisOfFlame>()
            .ActivateOnEnter<Ability30987>()
            .ActivateOnEnter<Ability30988>()
            .ActivateOnEnter<Trailblaze>()
            .ActivateOnEnter<ChthonicVent5>()
            .ActivateOnEnter<RearingRampage>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 883, NameID = 11399, PlanLevel = 90, Incomplete = true)]
public class P8NHephaistos(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsSquare(20));
