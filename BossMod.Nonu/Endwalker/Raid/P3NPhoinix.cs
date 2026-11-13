namespace BossMod.Endwalker.Raid.P3NPhoinix;

public enum OID : uint
{
    Boss = 0x353B, // R5.980, x1
    Helper = 0x233C, // R0.500, x45, Helper type
    Sparkfledged = 0x353C, // R1.500, x4
    Sunbird = 0x353E, // R1.800, x4
    Sparkfledged1 = 0x37FF, // R2.300, x4, Part type
    DarkenedFire = 0x3858, // R2.000, x4
    ActorFE36F = 0xFE36F, // R0.500, x1, EventNpc type
    ActorFE370 = 0xFE370, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x1, EventObj type
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    ExperimentalFireplume = 26262, // Boss->self, 5.0s cast, single-target
    Fireplume = 26263, // Helper->self, 6.0s cast, range 15 circle
    ExperimentalFireplume1 = 26264, // Boss->self, 5.0s cast, single-target
    Fireplume1 = 26265, // Helper->self, 5.0s cast, range 10 circle
    ExperimentalCharplume = 26268, // Boss->self, 3.0s cast, single-target
    Charplume = 26269, // Helper->player, 5.0s cast, range 6 circle
    DevouringBrand = 26270, // Boss->self, 3.0s cast, single-target
    DevouringBrand1 = 26271, // Helper->self, 3.0s cast, range 2 width 5 rect
    DevouringBrand2 = 26272, // Helper->self, no cast, range 4 width 5 rect
    DevouringBrand3 = 26273, // Helper->self, no cast, range 40 width 10 cross
    BlazingRain = 26274, // Helper->self, no cast, range 60 circle
    FlamesOfUndeath = 26275, // Boss->self, no cast, range 60 circle
    JointPyre = 26277, // Sparkfledged->self, no cast, range 9 circle
    ForeCarve = 26279, // Sunbird->self, 4.0s cast, range 15 180-degree cone
    RearCarve = 26280, // Sunbird->self, 4.0s cast, range 15 180-degree cone
    DeadRebirth = 26281, // Boss->self, 10.0s cast, range 60 circle
    FledglingFlight = 26282, // Boss->self, 3.0s cast, single-target
    AshenEye = 26283, // Sparkfledged->self, 7.0s cast, range 60 90-degree cone
    Ability26284 = 26284, // Boss->self, no cast, single-target
    HeatOfCondemnation = 26290, // Boss->self, 5.0s cast, single-target
    HeatOfCondemnation1 = 26291, // Helper->player, 6.0s cast, range 6 circle
    RightCinderwing = 26292, // Boss->self, 5.0s cast, range 60 180-degree cone
    SearingBreeze = 26294, // Boss->self, 3.0s cast, single-target
    SearingBreeze1 = 26295, // Helper->self, 3.0s cast, range 6 circle
    ScorchedExaltation = 26296, // Boss->self, 5.0s cast, range 60 circle
    AutoAttack1 = 27916, // Sunbird->player, no cast, single-target
    DevouringBrand4 = 28034, // Helper->self, 14.5s cast, range 40 width 10 cross
    AutoAttack2 = 28124, // Sparkfledged1->player, no cast, single-target
    DarkenedFire = 28378, // Boss->self, 6.0s cast, single-target
    DarkenedBlaze = 28379, // DarkenedFire->self, 15.0s cast, range 60 circle
    BrightenedFire = 28380, // Boss->self, 5.0s cast, single-target
    BrightenedFire1 = 28381, // Boss->players, no cast, range 7 circle
}

public enum IconID : uint
{
    Icon139 = 139, // player
    Icon218 = 218, // player
    Icon268 = 268, // player
    Icon269 = 269, // player
    Icon270 = 270, // player
    Icon271 = 271, // player
}

class Fireplume(BossModule module) : Components.StandardAOEs(module, AID.Fireplume, 15);
class Fireplume1(BossModule module) : Components.StandardAOEs(module, AID.Fireplume1, 10);
class DevouringBrand1(BossModule module) : Components.StandardAOEs(module, AID.DevouringBrand1, new AOEShapeRect(2, 2.5f));
class ForeCarve(BossModule module) : Components.StandardAOEs(module, AID.ForeCarve, new AOEShapeCone(15, 90.Degrees()));
class RearCarve(BossModule module) : Components.StandardAOEs(module, AID.RearCarve, new AOEShapeCone(15, 90.Degrees()));
class DeadRebirth(BossModule module) : Components.RaidwideCast(module, AID.DeadRebirth);
class AshenEye(BossModule module) : Components.StandardAOEs(module, AID.AshenEye, new AOEShapeCone(60, 45.Degrees()));
class RightCinderwing(BossModule module) : Components.StandardAOEs(module, AID.RightCinderwing, new AOEShapeCone(60, 90.Degrees()));
class SearingBreeze1(BossModule module) : Components.StandardAOEs(module, AID.SearingBreeze1, 6);
class ScorchedExaltation(BossModule module) : Components.RaidwideCast(module, AID.ScorchedExaltation);
class DevouringBrand4(BossModule module) : Components.StandardAOEs(module, AID.DevouringBrand4, new AOEShapeCross(40, 5));
class DarkenedBlaze(BossModule module) : Components.RaidwideCast(module, AID.DarkenedBlaze);

class P3NPhoinixStates : StateMachineBuilder
{
    public P3NPhoinixStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Fireplume>()
            .ActivateOnEnter<Fireplume1>()
            .ActivateOnEnter<DevouringBrand1>()
            .ActivateOnEnter<ForeCarve>()
            .ActivateOnEnter<RearCarve>()
            .ActivateOnEnter<DeadRebirth>()
            .ActivateOnEnter<AshenEye>()
            .ActivateOnEnter<RightCinderwing>()
            .ActivateOnEnter<SearingBreeze1>()
            .ActivateOnEnter<ScorchedExaltation>()
            .ActivateOnEnter<DevouringBrand4>()
            .ActivateOnEnter<DarkenedBlaze>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 806, NameID = 10720, PlanLevel = 90, Incomplete = true)]
public class P3NPhoinix(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsCircle(20));
