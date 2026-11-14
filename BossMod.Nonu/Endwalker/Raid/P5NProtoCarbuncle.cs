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

// each cell of light hits twice more after the cast ends (0.3s and 0.7s later), so it stays until the second repeat
class RubyReflectionQuadrant(BossModule module) : Components.GenericAOEs(module, AID.RubyReflectionQuadrant)
{
    private static readonly AOEShapeRect Shape = new(15, 7.5f);
    private readonly List<(Actor caster, AOEInstance aoe, int repeats)> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes.Select(a => a.aoe);

    public override void Update()
    {
        _aoes.RemoveAll(a => WorldState.CurrentTime > a.aoe.Activation.AddSeconds(2));
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.RubyReflectionQuadrant)
        {
            _aoes.RemoveAll(a => a.caster == caster);
            _aoes.Add((caster, new(Shape, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)), 0));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID != AID.RubyReflectionQuadrantRepeat)
            return;
        var index = _aoes.FindIndex(a => a.caster == caster);
        if (index < 0)
            return;
        if (_aoes[index].repeats >= 1)
            _aoes.RemoveAt(index);
        else
            _aoes[index] = _aoes[index] with { repeats = 1 };
    }
}

// a stone turning poisonous gets a poison puddle about 5s later, which grows from 5y to 12y and stays until the stone goes dormant again
class PoisonStones(BossModule module) : Components.GenericAOEs(module)
{
    private const float StartRadius = 5;
    private const float MaxRadius = 12;
    private const float GrowthPerSecond = 0.75f;
    private static readonly AOEShapeCircle ShapeMax = new(MaxRadius);
    private readonly List<(Actor stone, DateTime activation)> _stones = [];

    private float RadiusAt(DateTime activation, DateTime time) => Math.Clamp(StartRadius + GrowthPerSecond * (float)(time - activation).TotalSeconds, StartRadius, MaxRadius);
    private DateTime FullSizeAt(DateTime activation) => activation.AddSeconds((MaxRadius - StartRadius) / GrowthPerSecond);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
        => _stones.Select(s => new AOEInstance(new AOEShapeCircle(RadiusAt(s.activation, WorldState.CurrentTime)), s.stone.Position, default, s.activation));

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_stones.Any(s => ShapeMax.Check(actor.Position, s.stone.Position, default)))
            hints.Add("Poison spreads here, move away!");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        foreach (var s in _stones)
            hints.AddForbiddenZone(ShapeMax, s.stone.Position, default, FullSizeAt(s.activation));
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        foreach (var s in _stones)
            Arena.AddCircle(s.stone.Position, MaxRadius, ArenaColor.Danger);
    }

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if ((OID)actor.OID != OID.TopazStone)
            return;
        if (state is 0x00100080 or 0x00100020 && !_stones.Any(s => s.stone == actor))
            _stones.Add((actor, WorldState.FutureTime(5)));
        else if (state == 0x00040200)
            _stones.RemoveAll(s => s.stone == actor);
    }
}

// a marker runs the boss's jump path first; then the boss jumps along it, each landing a 12y circle, and devours whoever the last one hit
// the edge side of the first spot is out of reach of every later jump, so that's where to go once the first jump has landed
class StarvingStampede(BossModule module) : Components.GenericAOEs(module, AID.StarvingStampede)
{
    private const float Radius = 12;
    private static readonly AOEShapeCircle Shape = new(Radius);
    private readonly List<WPos> _path = [];
    private int _numJumps;
    private WPos? _safeSpot;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        for (var i = _numJumps; i < Math.Min(_numJumps + 3, _path.Count); ++i)
            yield return new(Shape, _path[i], default, default, i == _numJumps ? ArenaColor.Danger : 0, i == _numJumps);
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        base.AddHints(slot, actor, hints);
        if (_safeSpot != null)
            hints.Add(_numJumps == 0 ? "Go to the first jump's spot after it lands" : "Stay at the first jump's spot", false);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        if (_safeSpot != null)
            hints.GoalZones.Add(AIHints.GoalProximity(_safeSpot.Value, 30, 0.5f));
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (_safeSpot != null)
            Arena.AddCircle(_safeSpot.Value, 1, ArenaColor.Safe);
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.StampedeMarker)
        {
            _path.Clear();
            _path.Add(actor.Position);
            _numJumps = 0;
            _safeSpot = null;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.StampedeMarkerMove:
                _path.Add(spell.TargetXZ);
                if (_path.Count >= Module.Enemies(OID.StampedeSpot).Count(s => !s.IsDestroyed))
                    _safeSpot = FindSafeSpot();
                break;
            case AID.StarvingStampede:
                if (++_numJumps >= _path.Count)
                {
                    _path.Clear();
                    _safeSpot = null;
                }
                break;
        }
    }

    // the point nearest the first spot that is inside the arena and out of reach of every later jump
    private WPos? FindSafeSpot()
    {
        WPos? best = null;
        var bestDist = float.MaxValue;
        for (var x = -6f; x <= 6; x += 0.5f)
        {
            for (var z = -6f; z <= 6; z += 0.5f)
            {
                var p = _path[0] + new WDir(x, z);
                var off = p - Module.Center;
                var dist = (p - _path[0]).LengthSq();
                if (dist < bestDist && Math.Abs(off.X) <= Module.Bounds.Radius - 1 && Math.Abs(off.Z) <= Module.Bounds.Radius - 1 && _path.Skip(1).All(s => !p.InCircle(s, Radius + 1)))
                {
                    best = p;
                    bestDist = dist;
                }
            }
        }
        return best;
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
