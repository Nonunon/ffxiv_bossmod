namespace BossMod.Endwalker.Raid.P5NProtoCarbuncle;

public enum OID : uint
{
    Boss = 0x39E6, // R8.500, x1
    Helper = 0x233C, // R0.500, x20, Helper type
    Daphnia = 0x3432, // R1.500-3.000, x1
    Osculator = 0x3433, // R0.600, x1
    StampedeMarker = 0x39E7, // R1.000, x1, runs the starving stampede path before the boss
    StampedeSpot = 0x39E8, // R1.000, x5
    LivelyBait = 0x39E9, // R0.960, x1
    ActorFEC1B = 0xFEC1B, // R0.500, x1, EventNpc type
    ActorFEC1C = 0xFEC1C, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x1, EventObj type
    TopazStone = 0x1EB78C, // R0.500, x6, EventObj type
    TopazStonePoison = 0x1EB78D, // R0.500, x2, EventObj type
}

public enum AID : uint
{
    RubyGlow = 30420, // Boss->self, 5.0s cast, single-target
    RubyGlowAOE = 30421, // Helper->self, 5.0s cast, range 60 circle
    Ability30422 = 30422, // Boss->location, no cast, single-target
    SearingRay = 30423, // Boss->self, 7.0s cast, range 45 180-degree cone
    SearingRayCage = 30424, // Boss->self, 6.5+0.5s cast, hits the boss's side of the diagonal wall
    SearingRayCageReflection = 30426, // Helper->self, 7.0s cast, range 1 width 4 rect
    TopazStones = 30430, // Boss->self, 4.0s cast, single-target
    TopazStonesVisual = 30431, // Helper->self, 4.0s cast, single-target
    TopazRay = 30432, // Helper->self, 6.5s cast, range 4 circle
    RubyReflectionQuadrant = 30434, // Helper->self, 7.0s cast, range 15 width 15 rect
    Ability30435 = 30435, // Boss->self, no cast, single-target
    StampedeMarkerMove = 30436, // StampedeMarker->location, no cast, single-target
    StarvingStampedeJump = 30437, // Boss->location, no cast, single-target
    TopazCluster = 30438, // Boss->self, 4.0s cast, single-target
    VenomSquall = 30444, // Boss->self, 5.0s cast, single-target
    VenomRain = 30445, // Helper->player, 5.0s cast, range 5 circle spread
    VenomDrops = 30446, // Helper->location, 5.0s cast, range 5 circle
    VenomPool = 30447, // Helper->players, 8.0s cast, range 6 circle stack on a healer
    Crunch = 30448, // Boss->player, 5.0s cast, single-target
    ToxicCrunch = 30449, // Boss->player, 5.0s cast, single-target
    SonicHowl = 30450, // Boss->self, 5.0s cast, range 60 circle
    Devour = 30501, // Boss->player, no cast, single-target, eats players hit by the last stampede jump
    DevourAlt = 30502, // Boss->player, no cast, single-target
    Spit = 30503, // Boss->self, 2.0s cast, single-target
    DevourKill = 30504, // Helper->player, no cast, single-target
    Ability30954 = 30954, // Boss->location, no cast, single-target
    AcidicSlaver = 30955, // Boss->self, 3.0s cast, range 60 circle
    StarvingStampede = 31209, // Helper->self, 1.5s cast, range 12 circle
    SearingRayCageBounce = 31217, // Helper->self, no cast, range 0 ???
    RubyReflectionQuadrantRepeat = 31218, // Helper->self, no cast, range 15 width 15 rect
    TopazRayBent = 31228, // Helper->self, 6.5s cast, range 4 circle cut off by the wall it faces
    TopazRayBentCluster = 31229, // Helper->self, 6.5s cast, range 4 circle cut off by the wall it faces
    AutoAttack = 31245, // Boss->player, no cast, single-target
    Impact = 31263, // Helper->self, 4.5s cast, range 6 circle
}

public enum IconID : uint
{
    VenomRain = 100, // player
    Tankbuster = 218, // player
    VenomPool = 376, // player
}

class RubyGlow(BossModule module) : Components.RaidwideCast(module, AID.RubyGlowAOE);
class SearingRay(BossModule module) : Components.StandardAOEs(module, AID.SearingRay, new AOEShapeCone(45, 90.Degrees()));
class TopazRay(BossModule module) : Components.StandardAOEs(module, AID.TopazRay, 4);
class RubyReflectionQuadrant(BossModule module) : Components.StandardAOEs(module, AID.RubyReflectionQuadrant, new AOEShapeRect(15, 7.5f));
class VenomDrops(BossModule module) : Components.StandardAOEs(module, AID.VenomDrops, 5);
class VenomRain(BossModule module) : Components.SpreadFromCastTargets(module, AID.VenomRain, 5);
class VenomPool(BossModule module) : Components.StackWithCastTargets(module, AID.VenomPool, 6);
class Crunch(BossModule module) : Components.SingleTargetCast(module, AID.Crunch);
class ToxicCrunch(BossModule module) : Components.SingleTargetCast(module, AID.ToxicCrunch);
class SonicHowl(BossModule module) : Components.RaidwideCast(module, AID.SonicHowl);
class AcidicSlaver(BossModule module) : Components.RaidwideCast(module, AID.AcidicSlaver);
class Impact(BossModule module) : Components.StandardAOEs(module, AID.Impact, 6);

// a stone 1y from a wall faces it, and its circle stops at the wall; drawn as a half circle from the wall point
class TopazRayBent(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone Shape = new(5, 90.Degrees());
    private readonly List<(Actor caster, AOEInstance aoe)> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes.Select(a => a.aoe);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.TopazRayBent or AID.TopazRayBentCluster)
            _aoes.Add((caster, new(Shape, spell.LocXZ + spell.Rotation.ToDirection(), spell.Rotation + 180.Degrees(), Module.CastFinishAt(spell))));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.TopazRayBent or AID.TopazRayBentCluster)
            _aoes.RemoveAll(a => a.caster == caster);
    }
}

// with a diagonal wall up, searing ray hits the whole half of the arena on the boss's side of the wall (the boss faces the center from a corner)
class SearingRayCage(BossModule module) : Components.GenericAOEs(module, AID.SearingRayCage)
{
    private static readonly AOEShapeRect Shape = new(45, 45);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.SearingRayCage)
            _aoe = new(Shape, Module.Center, Angle.FromDirection(caster.Position - Module.Center), Module.CastFinishAt(spell, 0.5f));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.SearingRayCageBounce)
            _aoe = null;
    }
}

// a stone turning poisonous denies its whole quadrant until it goes dormant again
class PoisonStones(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect Shape = new(7.5f, 7.5f, 7.5f);
    private readonly List<(Actor stone, WPos center)> _stones = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _stones.Select(s => new AOEInstance(Shape, s.center));

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if ((OID)actor.OID != OID.TopazStone)
            return;
        if (state == 0x00100080)
        {
            var off = actor.Position - Module.Center;
            _stones.Add((actor, Module.Center + new WDir(off.X < 0 ? -7.5f : 7.5f, off.Z < 0 ? -7.5f : 7.5f)));
        }
        else if (state == 0x00040200)
        {
            _stones.RemoveAll(s => s.stone == actor);
        }
    }
}

// a marker runs the boss's jump path first; then the boss jumps along it, each landing a 12y circle, and devours whoever the last one hit
class StarvingStampede(BossModule module) : Components.GenericAOEs(module, AID.StarvingStampede)
{
    private static readonly AOEShapeCircle Shape = new(12);
    private readonly List<WPos> _path = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        for (var i = 0; i < Math.Min(2, _path.Count); ++i)
            yield return new(Shape, _path[i], default, default, i == 0 ? ArenaColor.Danger : 0, i == 0);
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.StampedeMarker)
        {
            _path.Clear();
            _path.Add(actor.Position);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.StampedeMarkerMove:
                _path.Add(spell.TargetXZ);
                break;
            case AID.StarvingStampede:
                if (_path.Count > 0)
                    _path.RemoveAt(0);
                break;
        }
    }
}

class P5NProtoCarbuncleStates : StateMachineBuilder
{
    public P5NProtoCarbuncleStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<RubyGlow>()
            .ActivateOnEnter<SearingRay>()
            .ActivateOnEnter<SearingRayCage>()
            .ActivateOnEnter<TopazRay>()
            .ActivateOnEnter<TopazRayBent>()
            .ActivateOnEnter<RubyReflectionQuadrant>()
            .ActivateOnEnter<PoisonStones>()
            .ActivateOnEnter<VenomDrops>()
            .ActivateOnEnter<VenomRain>()
            .ActivateOnEnter<VenomPool>()
            .ActivateOnEnter<Crunch>()
            .ActivateOnEnter<ToxicCrunch>()
            .ActivateOnEnter<SonicHowl>()
            .ActivateOnEnter<AcidicSlaver>()
            .ActivateOnEnter<StarvingStampede>()
            .ActivateOnEnter<Impact>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 872, NameID = 11440, PlanLevel = 90, Incomplete = true)]
public class P5NProtoCarbuncle(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsSquare(15));
