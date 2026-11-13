namespace BossMod.Endwalker.Raid.P5NProtoCarbuncle;

public enum OID : uint
{
    Boss = 0x39E6, // R8.500, x1
    Helper = 0x233C, // R0.500, x20, Helper type
    Daphnia = 0x3432, // R1.500-3.000, x1
    Osculator = 0x3433, // R0.600, x1
    Actor39E7 = 0x39E7, // R1.000, x1
    Actor39E8 = 0x39E8, // R1.000, x2
    LivelyBait = 0x39E9, // R0.960, x1
    ActorFEC1B = 0xFEC1B, // R0.500, x1, EventNpc type
    ActorFEC1C = 0xFEC1C, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x1, EventObj type
    Actor1EB78C = 0x1EB78C, // R0.500, x6, EventObj type
    Actor1EB78D = 0x1EB78D, // R0.500, x2, EventObj type
}

public enum AID : uint
{
    RubyGlow = 30420, // Boss->self, 5.0s cast, single-target
    RubyGlow1 = 30421, // Helper->self, 5.0s cast, range 60 circle
    Ability30422 = 30422, // Boss->location, no cast, single-target
    SearingRay = 30423, // Boss->self, 7.0s cast, range 45 180-degree cone
    SearingRay1 = 30424, // Boss->self, 6.5+0.5s cast, range 8 ???
    RubyReflection = 30426, // Helper->self, 7.0s cast, range 1 ???
    TopazStones = 30430, // Boss->self, 4.0s cast, single-target
    Ability30431 = 30431, // Helper->self, 4.0s cast, single-target
    TopazRay = 30432, // Helper->self, 6.5s cast, range 4 circle
    RubyReflection1 = 30434, // Helper->self, 7.0s cast, range 15 width 15 rect
    Ability30435 = 30435, // Boss->self, no cast, single-target
    Ability30436 = 30436, // Actor39E7->location, no cast, single-target
    VenomSquall = 30444, // Boss->self, 5.0s cast, single-target
    VenomRain = 30445, // Helper->player, 5.0s cast, range 5 circle
    VenomDrops = 30446, // Helper->location, 5.0s cast, range 5 circle
    VenomPool = 30447, // Helper->players, 8.0s cast, range 6 circle
    Crunch = 30448, // Boss->player, 5.0s cast, single-target
    ToxicCrunch = 30449, // Boss->player, 5.0s cast, single-target
    SonicHowl = 30450, // Boss->self, 5.0s cast, range 60 circle
    Ability30954 = 30954, // Boss->location, no cast, single-target
    AcidicSlaver = 30955, // Boss->self, 3.0s cast, range 60 circle
    RubyReflection2 = 31217, // Helper->self, no cast, range 0 ???
    RubyReflection3 = 31218, // Helper->self, no cast, range 15 width 15 rect
    TopazRay1 = 31228, // Helper->self, 6.5s cast, range 4 circle
    AutoAttack = 31245, // Boss->player, no cast, single-target
}

public enum IconID : uint
{
    Icon100 = 100, // player
    Icon218 = 218, // player
    Icon376 = 376, // player
}

class RubyGlow1(BossModule module) : Components.RaidwideCast(module, AID.RubyGlow1);
class SearingRay(BossModule module) : Components.StandardAOEs(module, AID.SearingRay, new AOEShapeCone(45, 90.Degrees()));
class TopazRay(BossModule module) : Components.StandardAOEs(module, AID.TopazRay, 4);
class RubyReflection1(BossModule module) : Components.StandardAOEs(module, AID.RubyReflection1, new AOEShapeRect(15, 7.5f));
class VenomDrops(BossModule module) : Components.StandardAOEs(module, AID.VenomDrops, 5);
class Crunch(BossModule module) : Components.SingleTargetCast(module, AID.Crunch);
class ToxicCrunch(BossModule module) : Components.SingleTargetCast(module, AID.ToxicCrunch);
class SonicHowl(BossModule module) : Components.RaidwideCast(module, AID.SonicHowl);
class AcidicSlaver(BossModule module) : Components.RaidwideCast(module, AID.AcidicSlaver);
class TopazRay1(BossModule module) : Components.StandardAOEs(module, AID.TopazRay1, 4);

class P5NProtoCarbuncleStates : StateMachineBuilder
{
    public P5NProtoCarbuncleStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<RubyGlow1>()
            .ActivateOnEnter<SearingRay>()
            .ActivateOnEnter<TopazRay>()
            .ActivateOnEnter<RubyReflection1>()
            .ActivateOnEnter<VenomDrops>()
            .ActivateOnEnter<Crunch>()
            .ActivateOnEnter<ToxicCrunch>()
            .ActivateOnEnter<SonicHowl>()
            .ActivateOnEnter<AcidicSlaver>()
            .ActivateOnEnter<TopazRay1>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 872, NameID = 11440, PlanLevel = 90, Incomplete = true)]
public class P5NProtoCarbuncle(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsSquare(15));
