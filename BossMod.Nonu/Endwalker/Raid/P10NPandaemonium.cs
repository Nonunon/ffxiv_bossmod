namespace BossMod.Endwalker.Raid.P10NPandaemonium;

public enum OID : uint
{
    Boss = 0x3F19, // R35.000, x1
    Helper = 0x233C, // R0.500, x23, Helper type
    PandaemoniacPillar = 0x3F1A, // R1.000, x8
    PandaemoniacPillar1 = 0x3F1B, // R1.000, x8
    Actor3F1C = 0x3F1C, // R0.200, x6
    ActorFF38D = 0xFF38D, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x1, EventObj type
}

public enum AID : uint
{
    Silkspit = 33369, // Boss->self, 3.0s cast, single-target
    Silkspit1 = 33370, // Helper->player, no cast, range 7 circle
    EntanglingWeb = 33371, // Helper->location, 3.5s cast, range 10 circle
    EntanglingWeb1 = 33372, // Helper->location, 3.5s cast, range 5 circle
    PandaemoniacPillars = 33373, // Boss->self, 5.5+1.2s cast, single-target
    Bury = 33374, // PandaemoniacPillar/PandaemoniacPillar1->self, no cast, single-target
    Bury1 = 33375, // Helper->self, 6.7s cast, range 2 circle
    Imprisonment = 33377, // PandaemoniacPillar->self, no cast, single-target
    Imprisonment1 = 33378, // Helper->self, 4.0s cast, range 4 circle
    Cannonspawn = 33379, // PandaemoniacPillar1->self, no cast, single-target
    Cannonspawn1 = 33380, // Helper->self, 4.0s cast, range 3-7 donut
    PandaemoniacRay = 33382, // Helper->self, 6.2s cast, range 30 width 50 rect
    PandaemoniacRay1 = 33383, // Boss->self, 6.0+0.2s cast, single-target
    Touchdown = 33384, // Boss->self, 10.0+1.0s cast, single-target
    Touchdown1 = 33385, // Helper->self, 11.0s cast, range 4 circle
    HarrowingHell = 33386, // Boss->self, 5.0+0.8s cast, single-target
    HarrowingHell1 = 33387, // Helper->self, no cast, range 60 width 60 rect
    HarrowingHell2 = 33388, // Helper->self, no cast, range 60 width 60 rect
    HarrowingHell3 = 33389, // Helper->self, no cast, range 60 width 60 rect
    HarrowingHell4 = 33390, // Helper->self, no cast, range 60 width 60 rect
    HarrowingHell5 = 33391, // Helper->self, 3.9s cast, range 60 width 60 rect
    PartedPlumes = 33392, // Boss->self, 3.0s cast, single-target
    PartedPlumes1 = 33393, // Helper->self, 4.0s cast, range 50 20-degree cone
    WickedStep = 33394, // Boss->self, 6.0+1.1s cast, single-target
    WickedStep1 = 33395, // Helper->self, 7.1s cast, range 4 circle
    WickedStep2 = 33396, // Helper->self, 9.5s cast, range 4 circle
    PandaemoniacMeltdown = 33398, // Boss->self, 5.5+0.6s cast, single-target
    PandaemoniacMeltdown1 = 33399, // Helper->self, no cast, range 50 width 6 rect
    SoulGrasp = 33400, // Boss->self, 5.0+0.6s cast, single-target
    SoulGrasp1 = 33401, // Helper->players, no cast, range 4 circle
    AutoAttack = 33402, // Boss->player, no cast, single-target
    Ultima = 33403, // Boss->location, 5.0s cast, range 50 circle
    HarrowingHell6 = 34485, // Helper->self, no cast, range 60 width 60 rect
    HarrowingHell7 = 34486, // Helper->self, no cast, range 60 width 60 rect
    HarrowingHell8 = 34487, // Helper->self, no cast, range 60 width 60 rect
    HarrowingHell9 = 34488, // Helper->self, no cast, range 60 width 60 rect
    EntanglingWeb2 = 34593, // Boss->self, 3.0s cast, single-target
    Ability34722 = 34722, // Helper->player, no cast, single-target
}

public enum IconID : uint
{
    Icon462 = 462, // player
    Icon467 = 467, // player
}

public enum TetherID : uint
{
    Tether57 = 57, // Actor3F1C/player->Actor3F1C/player
    Tether226 = 226, // Actor3F1C->Actor3F1C
}

class EntanglingWeb(BossModule module) : Components.StandardAOEs(module, AID.EntanglingWeb, 10);
class EntanglingWeb1(BossModule module) : Components.StandardAOEs(module, AID.EntanglingWeb1, 5);
class Bury1(BossModule module) : Components.StandardAOEs(module, AID.Bury1, 2);
class Imprisonment1(BossModule module) : Components.StandardAOEs(module, AID.Imprisonment1, 4);
class Cannonspawn1(BossModule module) : Components.StandardAOEs(module, AID.Cannonspawn1, new AOEShapeDonut(3, 7));
class PandaemoniacRay(BossModule module) : Components.StandardAOEs(module, AID.PandaemoniacRay, new AOEShapeRect(30, 25));
class Touchdown1(BossModule module) : Components.StandardAOEs(module, AID.Touchdown1, 4);
class HarrowingHell5(BossModule module) : Components.StandardAOEs(module, AID.HarrowingHell5, new AOEShapeRect(60, 30));
class PartedPlumes1(BossModule module) : Components.StandardAOEs(module, AID.PartedPlumes1, new AOEShapeCone(50, 10.Degrees()));
class WickedStep1(BossModule module) : Components.StandardAOEs(module, AID.WickedStep1, 4);
class WickedStep2(BossModule module) : Components.StandardAOEs(module, AID.WickedStep2, 4);
class Ultima(BossModule module) : Components.RaidwideCast(module, AID.Ultima);

// P10S layout: a 26x30 main platform, two 8x30 side platforms 25y out, and 2y-wide bridges to them (ENVC 2 right, 3 left)
class Platforms(BossModule module) : BossComponent(module)
{
    private static readonly ArenaBoundsRect _rect = new(30, 22.5f);
    private bool _left;
    private bool _right;

    private static PolygonClipper.Operand Rect(float x, float z, float hx, float hz) => new(CurveApprox.Rect(new WDir(x, z), new WDir(hx, 0), new WDir(0, hz)));

    public static ArenaBoundsCustom Build(bool left, bool right)
    {
        // offsets are from the module centre (100, 92.5)
        List<PolygonClipper.Operand> pieces = [Rect(0, 7.5f, 13, 15), Rect(25, -7.5f, 4, 15), Rect(-25, -7.5f, 4, 15)];
        if (right)
            pieces.Add(Rect(17, 0, 4.5f, 1));
        if (left)
            pieces.Add(Rect(-17, 0, 4.5f, 1));
        return new(37, _rect.Clipper.UnionAll(pieces[0], [.. pieces.Skip(1)]));
    }

    public override void OnMapEffect(byte index, uint state)
    {
        if (index is not (2 or 3) || state is not (0x00020001 or 0x00080004))
            return;
        if (index == 2)
            _right = state == 0x00020001;
        else
            _left = state == 0x00020001;
        Arena.Bounds = Build(_left, _right);
    }
}

class P10NPandaemoniumStates : StateMachineBuilder
{
    public P10NPandaemoniumStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<EntanglingWeb>()
            .ActivateOnEnter<EntanglingWeb1>()
            .ActivateOnEnter<Bury1>()
            .ActivateOnEnter<Imprisonment1>()
            .ActivateOnEnter<Cannonspawn1>()
            .ActivateOnEnter<PandaemoniacRay>()
            .ActivateOnEnter<Touchdown1>()
            .ActivateOnEnter<HarrowingHell5>()
            .ActivateOnEnter<PartedPlumes1>()
            .ActivateOnEnter<WickedStep1>()
            .ActivateOnEnter<WickedStep2>()
            .ActivateOnEnter<Ultima>()
            .ActivateOnEnter<Platforms>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 938, NameID = 12354, PlanLevel = 90, Incomplete = true)]
public class P10NPandaemonium(ModuleInit init) : BossModule(init, new(100, 92.5f), Platforms.Build(false, false));
