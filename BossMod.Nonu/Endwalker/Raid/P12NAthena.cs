namespace BossMod.Endwalker.Raid.P12NAthena;

public enum OID : uint
{
    Boss = 0x3F23, // R10.000, x2
    Helper = 0x233C, // R0.500, x28, Helper type
    Anthropos = 0x3F24, // R2.000, x8
    LogouIdea = 0x3F25, // R1.000, x11
    ThymouIdea = 0x3F26, // R1.000, x11
    Anthropos1 = 0x3F27, // R2.000, x8
    Athena = 0x3F28, // R0.500, x20
    Athena1 = 0x3F29, // R0.600, x14
    Athena2 = 0x3F2A, // R0.600, x6
    Actor3F65 = 0x3F65, // R1.000, x20
    ActorFF38F = 0xFF38F, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x2, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x2, EventObj type
}

public enum AID : uint
{
    SuperchainTheory = 33468, // Boss->self, 5.0s cast, single-target
    SuperchainBurst = 33469, // Helper->self, 1.0s cast, range 7 circle
    SuperchainCoil = 33470, // Helper->self, 1.0s cast, range 6-70 donut
    UnnaturalEnchainment = 33471, // Boss->self, 7.0s cast, single-target
    Sample = 33472, // Helper->self, no cast, range 10 width 20 rect
    TrinityOfSouls = 33473, // Boss->self, 10.0s cast, range 60 ?-degree cone
    TrinityOfSouls1 = 33474, // Boss->self, 10.0s cast, range 60 ?-degree cone
    TrinityOfSouls2 = 33475, // Boss->self, no cast, range 60 ?-degree cone
    TrinityOfSouls3 = 33476, // Boss->self, no cast, range 60 ?-degree cone
    TrinityOfSouls4 = 33477, // Boss->self, no cast, range 60 ?-degree cone
    TrinityOfSouls5 = 33478, // Boss->self, no cast, range 60 ?-degree cone
    Paradeigma = 33479, // Boss->self, 3.0s cast, single-target
    RayOfLight = 33480, // Anthropos->self, 5.0s cast, range 60 width 10 rect
    WhiteFlame = 33481, // Anthropos->self, no cast, single-target
    WhiteFlame1 = 33482, // Helper->self, no cast, range 100 width 4 rect
    UltimaBlade = 33483, // Boss->self, no cast, single-target
    UltimaBlade1 = 33484, // Helper->self, no cast, range 60 circle
    Palladion = 33485, // Boss->self, 8.0s cast, single-target
    Palladion1 = 33486, // Boss->location, no cast, range 6 circle
    Shockwave = 33487, // Helper->players, no cast, width 4 rect charge
    Palladion2 = 33488, // Boss->self, no cast, single-target
    Palladion3 = 33489, // Helper->self, 5.8s cast, range 20 width 40 rect
    TheossUltima = 33490, // Boss->self, 7.0s cast, range 60 circle
    ClearCut = 33491, // Anthropos1->self, 2.5s cast, range 4 270-degree cone
    RayOfLight1 = 33492, // Anthropos1->self, 4.0s cast, range 60 width 10 rect
    Glaukopis = 33493, // Boss->self/players, 5.0s cast, range 60 width 5 rect
    Dialogos = 33494, // Boss->players, 5.0s cast, range 6 circle
    Dialogos1 = 33495, // Helper->players, 5.0s cast, range 6 circle
    Parthenos = 33496, // Boss->self, 7.0s cast, range 120 width 16 rect
    OnTheSoul = 33497, // Boss->self, 5.0s cast, range 60 circle
    Ability33556 = 33556, // LogouIdea/ThymouIdea->location, no cast, single-target
    Ability33557 = 33557, // Boss->location, no cast, single-target
    AutoAttack = 34425, // Boss->player, no cast, single-target
    AutoAttack1 = 34721, // Anthropos1->player, no cast, single-target
}

public enum IconID : uint
{
    Icon62 = 62, // player
    Icon139 = 139, // player
    Icon421 = 421, // Actor3F65
    Icon422 = 422, // Actor3F65
    Icon423 = 423, // Actor3F65
    Icon424 = 424, // Actor3F65
    Icon433 = 433, // Actor3F65
    Icon434 = 434, // Actor3F65
    Icon471 = 471, // player
}

public enum TetherID : uint
{
    Tether17 = 17, // Anthropos->player
    Tether228 = 228, // Athena1->Athena
    Tether229 = 229, // Athena2->Athena
    Tether232 = 232, // Actor3F65->Boss
}

class SuperchainBurst(BossModule module) : Components.StandardAOEs(module, AID.SuperchainBurst, 7);
class SuperchainCoil(BossModule module) : Components.StandardAOEs(module, AID.SuperchainCoil, new AOEShapeDonut(6, 70));
class RayOfLight(BossModule module) : Components.StandardAOEs(module, AID.RayOfLight, new AOEShapeRect(60, 5));
class Palladion3(BossModule module) : Components.StandardAOEs(module, AID.Palladion3, new AOEShapeRect(20, 20));
class TheossUltima(BossModule module) : Components.RaidwideCast(module, AID.TheossUltima);
class ClearCut(BossModule module) : Components.StandardAOEs(module, AID.ClearCut, new AOEShapeCone(4, 135.Degrees()));
class RayOfLight1(BossModule module) : Components.StandardAOEs(module, AID.RayOfLight1, new AOEShapeRect(60, 5));
class Parthenos(BossModule module) : Components.StandardAOEs(module, AID.Parthenos, new AOEShapeRect(120, 8));
class OnTheSoul(BossModule module) : Components.RaidwideCast(module, AID.OnTheSoul);

class P12NAthenaStates : StateMachineBuilder
{
    public P12NAthenaStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<SuperchainBurst>()
            .ActivateOnEnter<SuperchainCoil>()
            .ActivateOnEnter<RayOfLight>()
            .ActivateOnEnter<Palladion3>()
            .ActivateOnEnter<TheossUltima>()
            .ActivateOnEnter<ClearCut>()
            .ActivateOnEnter<RayOfLight1>()
            .ActivateOnEnter<Parthenos>()
            .ActivateOnEnter<OnTheSoul>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 942, NameID = 12377, PlanLevel = 90, Incomplete = true)]
public class P12NAthena(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsSquare(20));
