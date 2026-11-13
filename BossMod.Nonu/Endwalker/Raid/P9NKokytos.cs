namespace BossMod.Endwalker.Raid.P9NKokytos;

public enum OID : uint
{
    Boss = 0x3ED5, // R9.500, x1
    Helper = 0x233C, // R0.500, x20, Helper type
    Comet = 0x3ED6, // R2.400, x3
    FireSphere = 0x3FCD, // R1.500, x4
    IceSphere = 0x3FCE, // R0.700, x4
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x1, EventObj type
    Actor1EB881 = 0x1EB881, // R0.500, x4, EventObj type
}

public enum AID : uint
{
    AutoAttack = 33045, // Boss->player, no cast, single-target
    GluttonysAugur = 33046, // Boss->self, 5.0s cast, single-target
    GluttonysAugur1 = 33047, // Helper->self, no cast, range 60 circle
    Ravening = 33048, // Boss->self, 4.0s cast, single-target
    Ravening1 = 33049, // Boss->self, 4.0s cast, single-target
    Ravening2 = 33050, // Boss->self, 3.0+1.0s cast, single-target
    AutoAttack1 = 33052, // Boss->player, no cast, single-target
    Dualspell = 33053, // Boss->self, 5.0s cast, single-target
    BlizzardIII = 33055, // Helper->self, 7.0s cast, range ?-30 donut
    FireIII = 33056, // Helper->players, 7.0s cast, range 12 circle
    BlizzardIII1 = 33057, // Helper->self, 7.0s cast, range ?-30 donut
    Ability33058 = 33058, // Helper->self, no cast, single-target
    Ability33059 = 33059, // Helper->self, no cast, single-target
    Disgorge = 33060, // Boss->self, no cast, single-target
    Disgorge1 = 33061, // Boss->self, no cast, single-target
    Disgorge2 = 33062, // Boss->self, no cast, single-target
    AutoAttack2 = 33063, // Boss->player, no cast, single-target
    ArchaicRockbreaker = 33064, // Boss->location, 5.0s cast, range 6 circle
    Shockwave = 33065, // Helper->self, no cast, range 60 circle
    ArchaicRockbreaker1 = 33066, // Helper->location, 7.0s cast, range 8 circle
    OutsideRoundhouse = 33068, // Boss->self, no cast, range ?-20 donut
    SwingingKick = 33069, // Boss->self, 1.0s cast, range 40 180-degree cone
    ArchaicDemolish = 33071, // Boss->self, 7.5+1.0s cast, single-target
    ArchaicDemolish1 = 33072, // Helper->players, 8.5s cast, range 6 circle
    AscendantFist = 33073, // Boss->player, 5.0s cast, single-target
    Charybdis = 33074, // Boss->self, 7.0+1.0s cast, single-target
    Charybdis1 = 33075, // Helper->players, 8.0s cast, range 6 circle
    Comet = 33076, // Boss->self, 8.0+1.0s cast, single-target
    CometImpact = 33077, // Comet->self, 8.0s cast, range 60 circle
    Burst = 33078, // Comet->self, 6.0s cast, range 10 circle
    Touchdown = 33079, // Boss->location, no cast, range 60 circle
    BeastlyRoar = 33080, // Boss->self, 4.0s cast, range 60 circle
    GluttonousRampage = 33081, // Boss->self, 8.0s cast, single-target
    RavenousBite = 33082, // Boss->player, no cast, single-target
    EclipticMeteor = 33083, // Boss->self, 7.0+2.0s cast, single-target
    EclipticMeteor1 = 33084, // Helper->self, no cast, single-target
    BeastlyBile = 33085, // Boss->self, no cast, single-target
    BeastlyBile1 = 33086, // Helper->player, 7.0s cast, range 6 circle
    PulverizingPounce = 33087, // Boss->players, 5.0s cast, range 6 circle
    AutoAttack3 = 33088, // Boss->player, no cast, single-target
    GlobalSpell = 33089, // Boss->self, 6.0s cast, single-target
    GlobalSpell1 = 33090, // Helper->self, no cast, range 60 circle
    Burst1 = 33091, // Comet->self, no cast, single-target
    Ability33092 = 33092, // Boss->location, no cast, single-target
    Ability33094 = 33094, // Helper->self, 13.0s cast, range 60 circle
    FrontCombination = 33096, // Boss->self, 5.0s cast, range 8-20 donut
    FireIII1 = 33102, // Boss->self, 5.0s cast, single-target
    BlizzardIII2 = 33103, // Boss->self, 5.0s cast, single-target
    EclipticMeteor2 = 33160, // Helper->self, 1.0s cast, range 60 circle
    IceflameSummoning = 34529, // Boss->self, 5.0s cast, single-target
    Explosion = 34530, // FireSphere->self, 7.0s cast, range 8 circle
    Explosion1 = 34531, // FireSphere->self, 7.0s cast, range 16 circle
    SphereShatter = 34532, // IceSphere->self, 7.0s cast, range 8 circle
    SphereShatter1 = 34533, // IceSphere->self, 7.0s cast, range 16 circle
    Ability34599 = 34599, // Helper->self, 7.0s cast, range 8 circle
    Ability34600 = 34600, // Helper->self, 7.0s cast, range 8 circle
    Ability34601 = 34601, // Helper->self, 7.0s cast, range 8 circle
    Ability34602 = 34602, // Helper->self, 7.0s cast, range 8 circle
}

public enum IconID : uint
{
    Icon100 = 100, // player
    Icon101 = 101, // player
    Icon161 = 161, // player
    Icon218 = 218, // player
    Icon314 = 314, // player
    Icon375 = 375, // player
    Icon453 = 453, // player
}

public enum TetherID : uint
{
    Tether1 = 1, // player->Boss
    Tether57 = 57, // player->Boss
}

class ArchaicRockbreaker(BossModule module) : Components.StandardAOEs(module, AID.ArchaicRockbreaker, 6);
class ArchaicRockbreaker1(BossModule module) : Components.StandardAOEs(module, AID.ArchaicRockbreaker1, 8);
class SwingingKick(BossModule module) : Components.StandardAOEs(module, AID.SwingingKick, new AOEShapeCone(40, 90.Degrees()));
class AscendantFist(BossModule module) : Components.SingleTargetCast(module, AID.AscendantFist);
class CometImpact(BossModule module) : Components.RaidwideCast(module, AID.CometImpact);
class Burst(BossModule module) : Components.StandardAOEs(module, AID.Burst, 10);
class BeastlyRoar(BossModule module) : Components.RaidwideCast(module, AID.BeastlyRoar);
class Ability33094(BossModule module) : Components.RaidwideCast(module, AID.Ability33094);
class FrontCombination(BossModule module) : Components.StandardAOEs(module, AID.FrontCombination, new AOEShapeDonut(8, 20));
class EclipticMeteor2(BossModule module) : Components.RaidwideCast(module, AID.EclipticMeteor2);
class Explosion(BossModule module) : Components.StandardAOEs(module, AID.Explosion, 8);
class Explosion1(BossModule module) : Components.StandardAOEs(module, AID.Explosion1, 16);
class SphereShatter(BossModule module) : Components.StandardAOEs(module, AID.SphereShatter, 8);
class SphereShatter1(BossModule module) : Components.StandardAOEs(module, AID.SphereShatter1, 16);
class Ability34599(BossModule module) : Components.StandardAOEs(module, AID.Ability34599, 8);
class Ability34600(BossModule module) : Components.StandardAOEs(module, AID.Ability34600, 8);
class Ability34601(BossModule module) : Components.StandardAOEs(module, AID.Ability34601, 8);
class Ability34602(BossModule module) : Components.StandardAOEs(module, AID.Ability34602, 8);

class P9NKokytosStates : StateMachineBuilder
{
    public P9NKokytosStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ArchaicRockbreaker>()
            .ActivateOnEnter<ArchaicRockbreaker1>()
            .ActivateOnEnter<SwingingKick>()
            .ActivateOnEnter<AscendantFist>()
            .ActivateOnEnter<CometImpact>()
            .ActivateOnEnter<Burst>()
            .ActivateOnEnter<BeastlyRoar>()
            .ActivateOnEnter<Ability33094>()
            .ActivateOnEnter<FrontCombination>()
            .ActivateOnEnter<EclipticMeteor2>()
            .ActivateOnEnter<Explosion>()
            .ActivateOnEnter<Explosion1>()
            .ActivateOnEnter<SphereShatter>()
            .ActivateOnEnter<SphereShatter1>()
            .ActivateOnEnter<Ability34599>()
            .ActivateOnEnter<Ability34600>()
            .ActivateOnEnter<Ability34601>()
            .ActivateOnEnter<Ability34602>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 936, NameID = 12369, PlanLevel = 90, Incomplete = true)]
public class P9NKokytos(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsCircle(20));
