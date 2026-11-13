namespace BossMod.Endwalker.Raid.P7NAgdistis;

public enum OID : uint
{
    Boss = 0x3A28, // R32.025, x1
    Helper = 0x233C, // R0.500, x22, Helper type
    ForbiddenFruit = 0x3A29, // R2.340, x3
    ForbiddenFruit1 = 0x3A2A, // R2.340, x4
    ImmatureIo = 0x3A2B, // R4.020, x3
    ImmatureStymphalide = 0x3A2C, // R3.024, x4
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R0.500-2.000, x1, EventObj type
}

public enum AID : uint
{
    ImmortalsObol = 30709, // Boss->self, 5.5+2.0s cast, single-target
    ImmortalsObol1 = 30710, // Helper->self, 7.5s cast, range 20 circle
    HemitheossGlareIII = 30711, // Boss->self, 3.0s cast, single-target
    HemitheossGlareIII1 = 30712, // Helper->self, 7.0s cast, range ?-30 donut
    BoughOfAttis = 30713, // Boss->self, 6.8+1.2s cast, single-target
    BoughOfAttis1 = 30714, // Helper->self, 8.0s cast, range 19 circle
    BoughOfAttis2 = 30715, // Boss->self, 4.0s cast, single-target
    BoughOfAttis3 = 30716, // Boss->self, 4.0s cast, single-target
    BoughOfAttis4 = 30717, // Helper->self, 5.0s cast, range 50 width 25 rect
    BoughOfAttis5 = 30718, // Boss->self, 7.2+0.8s cast, single-target
    BoughOfAttis6 = 30719, // Helper->self, 8.0s cast, range 25 circle
    ForbiddenFruit = 30720, // Boss->self, 4.0s cast, single-target
    ForbiddenFruit1 = 30721, // Boss->self, 3.0s cast, single-target
    StaticMoon = 30722, // ImmatureIo->self, 5.0s cast, range 10 circle
    StymphalianStrike = 30723, // ImmatureStymphalide->self, 5.0s cast, range 60 width 8 rect
    BladesOfAttis = 30724, // Boss->self, no cast, single-target
    BladesOfAttis1 = 30725, // Helper->self, 5.0s cast, range 7 circle
    BladesOfAttis2 = 30726, // Helper->self, no cast, range 7 circle
    HemitheossHoly = 30727, // Boss->self, 3.0s cast, single-target
    HemitheossHoly1 = 30728, // Helper->players, 6.0s cast, range 6 circle
    HemitheossAeroII = 30729, // Boss->self, 4.9s cast, single-target
    HemitheossAeroII1 = 30730, // Helper->players, 5.4s cast, range 8 circle
    SparkOfLife = 30731, // Boss->self, 5.0s cast, range 60 circle
    ShadowOfAttis = 30780, // Boss->self, no cast, single-target
    AutoAttack = 30947, // Boss->self, no cast, single-target
    AutoAttack1 = 30948, // Helper->player, no cast, single-target
    HemitheossGlareIII2 = 31226, // Helper->self, 10.0s cast, range ?-30 donut
}

public enum IconID : uint
{
    Icon311 = 311, // player
    Icon364 = 364, // player
}

class ImmortalsObol1(BossModule module) : Components.StandardAOEs(module, AID.ImmortalsObol1, 20);
class BoughOfAttis1(BossModule module) : Components.StandardAOEs(module, AID.BoughOfAttis1, 19);
class BoughOfAttis4(BossModule module) : Components.StandardAOEs(module, AID.BoughOfAttis4, new AOEShapeRect(50, 12.5f));
class BoughOfAttis6(BossModule module) : Components.StandardAOEs(module, AID.BoughOfAttis6, 25);
class StaticMoon(BossModule module) : Components.StandardAOEs(module, AID.StaticMoon, 10);
class StymphalianStrike(BossModule module) : Components.StandardAOEs(module, AID.StymphalianStrike, new AOEShapeRect(60, 4));
class BladesOfAttis1(BossModule module) : Components.StandardAOEs(module, AID.BladesOfAttis1, 7);
class SparkOfLife(BossModule module) : Components.RaidwideCast(module, AID.SparkOfLife);

class P7NAgdistisStates : StateMachineBuilder
{
    public P7NAgdistisStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ImmortalsObol1>()
            .ActivateOnEnter<BoughOfAttis1>()
            .ActivateOnEnter<BoughOfAttis4>()
            .ActivateOnEnter<BoughOfAttis6>()
            .ActivateOnEnter<StaticMoon>()
            .ActivateOnEnter<StymphalianStrike>()
            .ActivateOnEnter<BladesOfAttis1>()
            .ActivateOnEnter<SparkOfLife>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 876, NameID = 11374, PlanLevel = 90, Incomplete = true)]
public class P7NAgdistis(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsCircle(27));
