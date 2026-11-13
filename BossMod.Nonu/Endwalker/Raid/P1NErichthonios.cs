namespace BossMod.Endwalker.Raid.P1NErichthonios;

public enum OID : uint
{
    Boss = 0x3521, // R9.975, x1
    Helper = 0x233C, // R0.500, x24, Helper type
    Actor3523 = 0x3523, // R3.500, x5, Part type
    Actor3524 = 0x3524, // R3.500, x2, Part type
    ActorFE36C = 0xFE36C, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1E8536 = 0x1E8536, // R2.000, x1, EventObj type
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    GaolersFlail = 26073, // Boss->self, 8.0+1.0s cast, range 60 ?-degree cone
    GaolersFlail1 = 26074, // Boss->self, 8.0+1.0s cast, range 60 ?-degree cone
    Aetherflail = 26079, // Boss->self, 8.0+1.0s cast, range 60 ?-degree cone
    Aetherflail1 = 26080, // Boss->self, 8.0+1.0s cast, range 60 ?-degree cone
    PitilessFlail = 26085, // Boss->self/player, 5.0s cast, range 60 width 6 rect
    TrueHoly = 26086, // Boss->self, no cast, single-target
    TrueHoly1 = 26087, // Helper->players, no cast, range 6 circle
    ShiningCells = 26089, // Boss->self, 7.0s cast, range 60 circle
    SlamShut = 26090, // Boss->self, 7.0s cast, range 60 circle
    Aetherchain = 26091, // Boss->self, 5.0s cast, single-target
    PowerfulFire = 26092, // Helper->self, no cast, range 0 ???
    PowerfulLight = 26093, // Helper->self, no cast, range 0 ???
    Intemperance = 26094, // Boss->self, 2.0s cast, single-target
    IntemperateTorment = 26095, // Boss->self, 6.0s cast, single-target
    HotSpell = 26096, // Helper->self, 1.5s cast, range 19 width 19 rect
    ColdSpell = 26097, // Helper->self, 1.5s cast, range 19 width 19 rect
    PainfulFlux = 26098, // Helper->self, no cast, range 0 ???
    HeavyHand = 26099, // Boss->player, 5.0s cast, single-target
    WardersWrath = 26100, // Boss->self, 5.0s cast, range 60 circle
    Ability26101 = 26101, // Boss->location, no cast, single-target
    HotSpell1 = 27845, // Helper->self, 4.0s cast, range 19 width 19 rect
    ColdSpell1 = 27846, // Helper->self, 4.0s cast, range 19 width 19 rect
    Ability27925 = 27925, // Helper->player, no cast, single-target
    GaolersFlail2 = 28066, // Helper->self, 8.7s cast, range 60 ?-degree cone
    GaolersFlail3 = 28067, // Helper->self, 8.7s cast, range 60 ?-degree cone
}

public enum IconID : uint
{
    Icon1 = 1, // player
    Icon62 = 62, // player
    Icon218 = 218, // player
}

class ShiningCells(BossModule module) : Components.RaidwideCast(module, AID.ShiningCells);
class SlamShut(BossModule module) : Components.RaidwideCast(module, AID.SlamShut);
class HotSpell(BossModule module) : Components.StandardAOEs(module, AID.HotSpell, new AOEShapeRect(19, 9.5f));
class ColdSpell(BossModule module) : Components.StandardAOEs(module, AID.ColdSpell, new AOEShapeRect(19, 9.5f));
class HeavyHand(BossModule module) : Components.SingleTargetCast(module, AID.HeavyHand);
class WardersWrath(BossModule module) : Components.RaidwideCast(module, AID.WardersWrath);
class HotSpell1(BossModule module) : Components.StandardAOEs(module, AID.HotSpell1, new AOEShapeRect(19, 9.5f));
class ColdSpell1(BossModule module) : Components.StandardAOEs(module, AID.ColdSpell1, new AOEShapeRect(19, 9.5f));

class P1NErichthoniosStates : StateMachineBuilder
{
    public P1NErichthoniosStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ShiningCells>()
            .ActivateOnEnter<SlamShut>()
            .ActivateOnEnter<HotSpell>()
            .ActivateOnEnter<ColdSpell>()
            .ActivateOnEnter<HeavyHand>()
            .ActivateOnEnter<WardersWrath>()
            .ActivateOnEnter<HotSpell1>()
            .ActivateOnEnter<ColdSpell1>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 808, NameID = 10345, PlanLevel = 90, Incomplete = true)]
public class P1NErichthonios(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsSquare(20));
