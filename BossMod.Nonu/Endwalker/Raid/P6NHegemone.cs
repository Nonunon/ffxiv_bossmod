namespace BossMod.Endwalker.Raid.P6NHegemone;

public enum OID : uint
{
    Boss = 0x3A35, // R10.010, x1
    Helper = 0x233C, // R0.500, x25, Helper type
    Parasitos = 0x3A36, // R1.000, x8
    Actor3AD9 = 0x3AD9, // R1.000, x4
    Parasitos1 = 0x3AEA, // R0.500, x8
    ActorFEC1D = 0xFEC1D, // R0.500, x1, EventNpc type
    ActorFEC1E = 0xFEC1E, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1E8536 = 0x1E8536, // R2.000, x1, EventObj type
}

public enum AID : uint
{
    AutoAttack = 30795, // Boss->player, no cast, single-target
    Ability30796 = 30796, // Boss->location, no cast, single-target
    AetherialExchange = 30797, // Boss->self, 3.0s cast, single-target
    HemitheossDarkIV = 30798, // Boss->self, 5.0s cast, range 60 circle
    Transmission = 30799, // Boss->self, 5.0s cast, single-target
    AethericPolyominoid = 30803, // Boss->self, 4.0s cast, single-target
    Ability30804 = 30804, // Helper->self, no cast, single-target
    PolyominousDarkIV = 30805, // Helper->self, 1.0s cast, range 10 width 10 rect
    PolyominoidSigma = 30806, // Boss->self, 4.0s cast, single-target
    ChorosIxou = 30807, // Boss->self, 4.5+0.5s cast, single-target
    ChorosIxou1 = 30808, // Boss->self, 4.5+0.5s cast, single-target
    ChorosIxou2 = 30809, // Helper->self, 5.0s cast, range 40 90-degree cone
    ChorosIxou3 = 30810, // Helper->self, 5.0s cast, range 40 90-degree cone
    Synergy = 30811, // Boss->self, 4.3+0.7s cast, single-target
    Synergy1 = 30812, // Helper->player, 5.0s cast, range 5 circle
    Synergy2 = 30813, // Helper->player, 5.0s cast, range 5 circle
    DarkAshes = 30814, // Boss->self, 4.0s cast, single-target
    DarkAshes1 = 30815, // Helper->player, 8.0s cast, range 6 circle
    ReekHavoc = 31213, // Parasitos1->self, 3.4s cast, range 60 30-degree cone
    StropheIxou = 31249, // Boss->self, 5.0s cast, single-target
    StropheIxou1 = 31251, // Boss->self, no cast, single-target
    PathogenicCells = 31252, // Helper->self, 5.0s cast, range 40 45-degree cone
    ChelicVector = 31253, // Helper->self, 5.0s cast, range 40 45-degree cone
    PathogenicCells1 = 31254, // Helper->self, no cast, range 40 ?-degree cone
    ChelicVector1 = 31255, // Helper->self, no cast, range 40 ?-degree cone
}

public enum IconID : uint
{
    Icon101 = 101, // player
    Icon168 = 168, // Boss
    Icon343 = 343, // player
    Icon354 = 354, // player
}

public enum TetherID : uint
{
    Tether207 = 207, // Actor3AD9->Actor3AD9
    Tether208 = 208, // player->Boss
}

class HemitheossDarkIV(BossModule module) : Components.RaidwideCast(module, AID.HemitheossDarkIV);
class PolyominousDarkIV(BossModule module) : Components.StandardAOEs(module, AID.PolyominousDarkIV, new AOEShapeRect(10, 5));
class ChorosIxou2(BossModule module) : Components.StandardAOEs(module, AID.ChorosIxou2, new AOEShapeCone(40, 45.Degrees()));
class ChorosIxou3(BossModule module) : Components.StandardAOEs(module, AID.ChorosIxou3, new AOEShapeCone(40, 45.Degrees()));
class ReekHavoc(BossModule module) : Components.StandardAOEs(module, AID.ReekHavoc, new AOEShapeCone(60, 15.Degrees()));
class PathogenicCells(BossModule module) : Components.StandardAOEs(module, AID.PathogenicCells, new AOEShapeCone(40, 22.5f.Degrees()));
class ChelicVector(BossModule module) : Components.StandardAOEs(module, AID.ChelicVector, new AOEShapeCone(40, 22.5f.Degrees()));

class P6NHegemoneStates : StateMachineBuilder
{
    public P6NHegemoneStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<HemitheossDarkIV>()
            .ActivateOnEnter<PolyominousDarkIV>()
            .ActivateOnEnter<ChorosIxou2>()
            .ActivateOnEnter<ChorosIxou3>()
            .ActivateOnEnter<ReekHavoc>()
            .ActivateOnEnter<PathogenicCells>()
            .ActivateOnEnter<ChelicVector>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 880, NameID = 11381, PlanLevel = 90, Incomplete = true)]
public class P6NHegemone(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsSquare(20));
