namespace BossMod.Stormblood.Extreme.Ex3Shinryu;

class EarthenFury(BossModule module) : Components.RaidwideCast(module, AID.EarthenFury);
class Hellfire(BossModule module) : Components.RaidwideCast(module, AID.Hellfire, "Raidwide, stand in water");
class JudgmentBolt(BossModule module) : Components.RaidwideCast(module, AID.JudgmentBolt, "Raidwide, get out of water");
class DiamondDust(BossModule module) : Components.RaidwideCast(module, AID.DiamondDust, "Raidwide, thin ice after");
class IceStorm(BossModule module) : Components.RaidwideCastDelay(module, AID.IceStormVisual, AID.IceStorm, 0.6f);
class IceStormP3(BossModule module) : Components.RaidwideCastDelay(module, AID.IceStormVisualP3, AID.IceStorm, 0.6f);
class DarkMatter(BossModule module) : Components.RaidwideCast(module, AID.DarkMatter);
class Protostar(BossModule module) : Components.RaidwideCast(module, AID.Protostar);
class TailSpit(BossModule module) : Components.RaidwideCast(module, AID.TailSpit);
class TailSpitCenter(BossModule module) : Components.StandardAOEs(module, AID.TailSpitCenter, 5);

class EarthenFuryTile(BossModule module) : Components.GenericAOEs(module, AID.EarthenFuryTile)
{
    private static readonly AOEShapeRect _shape = new(10, 10, 10);
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
            _aoes.Add(new(_shape, Ex3Shinryu.TileCenter(caster.Position + (caster.Position - Arena.Center).Normalized() * 10), default, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
            _aoes.Clear();
    }
}

class TidalWave(BossModule module) : Components.KnockbackFromCastTarget(module, AID.TidalWave, 35, kind: Kind.DirForward)
{
    public override PointTest DestinationUnsafe(int slot, Actor actor) => Module.FindComponent<ArenaChanges>() is { } arena ? arena.Broken : PointTest.Never;
}
class SuperCyclone(BossModule module) : Components.RaidwideMultiHit(module, AID.AerialBlast, [AID.SuperCyclone], -6.1f, 2.1f, 3);
class AerialBlast(BossModule module) : Components.Knockback(module, AID.AerialBlast)
{
    public override PointTest DestinationUnsafe(int slot, Actor actor) => Module.FindComponent<ArenaChanges>() is { } arena ? arena.Broken : PointTest.Never;

    private WPos _origin;
    private DateTime _activation;
    private DateTime _positionDeadline;
    private int _smallPushes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.AerialBlast)
        {
            _origin = caster.Position;
            _activation = Module.CastFinishAt(spell, 1.6f);
            _positionDeadline = Module.CastFinishAt(spell, -6.3f);
            _smallPushes = 0;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        base.OnEventCast(caster, spell);
        if ((AID)spell.Action.ID == AID.SuperCyclone && _activation != default)
            _positionDeadline = ++_smallPushes < 3 ? WorldState.FutureTime(2.1f) : _activation;
    }

    public override IEnumerable<Source> Sources(int slot, Actor actor)
    {
        if (_activation != default && WorldState.CurrentTime <= _activation)
            yield return new(_origin, 15, _activation);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (_activation == default || WorldState.CurrentTime > _activation.AddSeconds(0.5f) || IsImmune(slot, _activation))
            return;
        hints.AddForbiddenZone(ShapeDistance.InvertedCircle(_origin, 10), _positionDeadline);
        if (Module.FindComponent<ArenaChanges>() is not { } arena)
            return;
        var origin = _origin;
        hints.AddForbiddenZone(Sdf.Discrete(p => arena.Broken(AwayFromSource(p, origin, 15))), _positionDeadline);
    }
}

// Levinbolt and Judgment Bolt are deadly in water; Hellfire and Hypernova hit softer in water. Hellfire uses up every puddle, the
// Hypernova stack only the one it lands in, so the whole party takes the same puddle (the one nearest the center, (0,0) in every log).
class WaterPuddles(BossModule module) : BossComponent(module)
{
    private const float Radius = 5;
    private DateTime _avoid;
    private DateTime _seek;
    private DateTime _stack;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.LevinboltVisual:
            case AID.JudgmentBolt:
                _avoid = Module.CastFinishAt(spell);
                break;
            case AID.Hellfire:
                _seek = Module.CastFinishAt(spell);
                break;
            case AID.HypernovaVisual:
                _stack = Module.CastFinishAt(spell, 0.1f);
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.LevinboltVisual:
            case AID.JudgmentBolt:
                _avoid = default;
                break;
            case AID.Hellfire:
                _seek = default;
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.Hypernova)
            _stack = default;
    }

    private IEnumerable<Actor> Puddles() => Module.Enemies(OID.WaterPuddle).Where(p => !p.IsDead);
    private Actor? StackPuddle() => Puddles().MinBy(p => (p.Position - Arena.Center).LengthSq());

    private Actor? Target(Actor actor) => _stack != default ? StackPuddle() : _seek != default ? Puddles().MinBy(p => (p.Position - actor.Position).LengthSq()) : null;

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (_avoid != default)
        {
            foreach (var p in Puddles())
                hints.AddForbiddenZone(ShapeDistance.Circle(p.Position, Radius + 1), _avoid);
        }
        else if (Target(actor) is { } target)
        {
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(target.Position, Radius - 1.5f), _stack != default ? _stack : _seek);
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        var inWater = Puddles().Any(p => actor.Position.InCircle(p.Position, Radius));
        if (_avoid != default && inWater)
            hints.Add("Get out of water!");
        else if (_stack != default && StackPuddle() is { } puddle && !actor.Position.InCircle(puddle.Position, Radius))
            hints.Add("Stack in the water!");
        else if (_seek != default && !inWater && Puddles().Any())
            hints.Add("Stand in water!");
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        var stackPuddle = _stack != default ? StackPuddle() : null;
        foreach (var p in Puddles())
        {
            if (_avoid != default)
                Arena.ZoneCircle(p.Position, Radius, ArenaColor.AOE);
            else if (_seek != default || p == stackPuddle)
                Arena.ZoneCircle(p.Position, Radius, ArenaColor.SafeFromAOE);
            else
                Arena.AddCircle(p.Position, Radius, ArenaColor.Object);
        }
    }
}

class Paralysis : BossComponent
{
    public Paralysis(BossModule module) : base(module)
    {
        KeepOnPhaseChange = true;
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (!actor.Class.CanEsuna())
            return;
        foreach (var member in Raid.WithoutSlot())
            if (!member.IsDead && member.FindStatus(SID.Paralysis) != null && member.PendingDispels.Count == 0)
                hints.Add($"Cleanse {member.Name}!");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var (partySlot, member) in Raid.WithSlot())
            if (!member.IsDead && member.FindStatus(SID.Paralysis) != null)
                hints.ShouldCleanse.Set(partySlot);
    }
}

class Levinbolt(BossModule module) : Components.UniformStackSpread(module, 0, 5, alwaysShowSpreads: true)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.LevinboltVisual)
            AddSpreads(Raid.WithoutSlot(), Module.CastFinishAt(spell, 0.3f));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.Levinbolt)
            Spreads.Clear();
    }
}

class Hypernova(BossModule module) : Components.CastHint(module, AID.HypernovaVisual, "Stack together");
// After Shatter the left wing's Ice Storm (6s) starts together with the right wing's Hypernova or Levinbolt (10s). Ice Storm gives
// Deep Freeze (12s) to anyone standing still when it lands: before Hypernova that is wanted (the stack thaws everyone about 4s later,
// all 8 frozen in every Hypernova kill), before Levinbolt it is deadly (the puddles drop under each player, both frozen players in
// two wipes died there).
class IceStormFreeze(BossModule module) : Components.StayMove(module)
{
    private DateTime _iceStorm;
    private Requirement _requirement;

    private void Refresh()
    {
        if (_iceStorm == default || _requirement == Requirement.None)
            return;
        foreach (var (slot, _) in Raid.WithSlot())
            SetState(slot, new(_requirement, _iceStorm));
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.IceStormVisualP3:
                _iceStorm = Module.CastFinishAt(spell, 0.6f);
                break;
            case AID.HypernovaVisualP3:
                _requirement = Requirement.Stay;
                break;
            case AID.LevinboltVisualP3:
                _requirement = Requirement.Move;
                break;
            default:
                return;
        }
        Refresh();
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.IceStorm && _iceStorm != default)
        {
            _iceStorm = default;
            _requirement = Requirement.None;
            Array.Fill(PlayerStates, default);
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        base.AddHints(slot, actor, hints);
        if (PlayerStates[slot].Requirement == Requirement.Stay)
            hints.Add("Stand still for Ice Storm, Hypernova thaws you", false);
        else if (PlayerStates[slot].Requirement == Requirement.Move)
            hints.Add("Keep moving through Ice Storm, Levinbolt drops under you", false);
    }
}

// no icon: a random player is the 8y stack target, so the whole party stacks before the Ice Storm freeze locks them in place
class HypernovaP3(BossModule module) : Components.CastHint(module, AID.HypernovaVisualP3, "Stack together")
{
    private WPos StackSpot() => Raid.WithoutSlot().Aggregate((sum: new WDir(), n: 0), (a, p) => (a.sum + (p.Position - Arena.Center), a.n + 1)) is var t && t.n > 0 ? Arena.Center + t.sum / t.n : Arena.Center;

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Casters.Count > 0)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(StackSpot(), 2), Module.CastFinishAt(Casters[0].CastInfo));
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (Casters.Count > 0)
            Arena.AddCircle(StackSpot(), 2, ArenaColor.Safe);
    }
}

// no spread hit: at the end of the cast a 5y puddle drops under every player, so spread out to keep them apart
class LevinboltP3Hint(BossModule module) : Components.UniformStackSpread(module, 0, 5, alwaysShowSpreads: true, raidwideOnResolve: false)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.LevinboltVisualP3)
            AddSpreads(Raid.WithoutSlot(), Module.CastFinishAt(spell));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.LevinboltVisualP3)
            Spreads.Clear();
    }
}
class LevinboltP3(BossModule module) : Components.StandardAOEs(module, AID.LevinboltP3, 5);

class IcicleAdds(BossModule module) : Components.AddsMulti(module, [(uint)OID.Icicle]);
// The lanes are known when the icicles spawn, 8.8s before the hit; the second set lands during Hellfire and its knockback throws
// players out of their puddle, so they are drawn from the spawn.
class Spikesicle(BossModule module) : Components.GenericAOEs(module, AID.Spikesicle)
{
    private static readonly AOEShapeRect _shape = new(64.5f, 5);
    private readonly List<(Actor Icicle, DateTime Activation)> _icicles = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _icicles.Select(i => new AOEInstance(_shape, i.Icicle.Position, i.Icicle.Rotation, i.Activation));

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        base.AddHints(slot, actor, hints);
        if (_icicles.Count > 0 && Module.FindComponent<WaterPuddles>() != null && Module.Enemies(OID.WaterPuddle).Any(p => !p.IsDead && actor.Position.InCircle(p.Position, 5)) && ActiveAOEs(slot, actor).Any(a => a.Check(actor.Position)))
            hints.Add("Icicle knockback will push you out of the water!");
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Icicle)
            _icicles.Add((actor, WorldState.FutureTime(8.8f)));
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
        {
            var i = _icicles.FindIndex(i => i.Icicle == caster);
            if (i >= 0)
                _icicles[i] = (caster, Module.CastFinishAt(spell));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action == WatchedAction)
        {
            _icicles.RemoveAll(i => i.Icicle == caster);
            ++NumCasts;
        }
    }

    public override void Update() => _icicles.RemoveAll(i => i.Icicle.IsDestroyed);
}
class SpikesicleKnockback(BossModule module) : Components.KnockbackFromCastTarget(module, AID.Spikesicle, 10, maxCasts: 3, shape: new AOEShapeRect(64.5f, 5), kind: Kind.DirForward)
{
    public override PointTest DestinationUnsafe(int slot, Actor actor) => Module.FindComponent<ArenaChanges>() is { } arena ? arena.Broken : PointTest.Never;
}

class AkhMorn(BossModule module) : Components.CastSharedTankbuster(module, AID.AkhMorn, 4);

// Akh Rhai drops a lingering 4y puddle where a random non-tank stands about 2s after Akh Morn lands (one at the first Akh Morn,
// two at the second), so the non-tanks keep apart until it lands.
class AkhRhaiBait(BossModule module) : Components.UniformStackSpread(module, 0, 4, alwaysShowSpreads: true, raidwideOnResolve: false)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.AkhMorn)
            AddSpreads(Raid.WithoutSlot().Where(p => p.Role != Role.Tank && p.InstanceID != spell.TargetID), Module.CastFinishAt(spell, 2.1f));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.AkhRhaiFirst)
            Spreads.Clear();
    }
}
class AkhRhai(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle _shape = new(4);
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.AkhRhaiFirst or AID.AkhRhai)
        {
            _aoes.Add(new(_shape, spell.TargetXZ, default, WorldState.FutureTime(1)));
            ++NumCasts;
        }
    }

    public override void Update() => _aoes.RemoveAll(a => a.Activation < WorldState.CurrentTime);
}

class EarthBreath(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(80, 30.Degrees()), (uint)IconID.EarthBreath, AID.EarthBreathAOE, 5.6f);
class Dragonfist(BossModule module) : Components.StandardAOEs(module, AID.Dragonfist, 16);
class GyreCharge(BossModule module) : Components.StandardAOEs(module, AID.GyreCharge, new AOEShapeRect(100, 30));
// the chain breaks some 0.3s after the pair passes about 10y (186 chains in 13 Unreal and one Extreme pull; most break at 10-14y,
// a few held past 15y), so the AI goes for 15y past where they started; unbroken, it burns both 3.6s after the tether and every
// 1.6s after that
class BurningChains : Components.Chains
{
    public BurningChains(BossModule module) : base(module, (uint)TetherID.BurningChains, AID.BurningChains, 15, activationDelay: 3.6f)
    {
        KeepOnPhaseChange = true;
    }
}

// Shinryu dives 8.1s after the tail marker; the dive picks the side (north or south of the middle row) and the marked player's
// column at that moment picks the column, so the tail surfaces 4.4s later at (column, ±10) and slaps 20y to both sides of it
// (the middle-row tile and one outer tile), 10.1s after the dive. Matched 45 of 45 tails across the Unreal and Extreme logs.
class TailSlap(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect _shape = new(20, 10, 20);
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Tail)
        {
            _aoes.Clear();
            _aoes.Add(new(_shape, actor.Position, actor.Rotation, WorldState.FutureTime(5.7f)));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.TailDiveNorth:
            case AID.TailDiveSouth:
                if (Module.FindComponent<TailMarker>()?.Target is { } target)
                    _aoes.Add(new(_shape, new(Ex3Shinryu.TileCenter(target.Position).X, (AID)spell.Action.ID == AID.TailDiveNorth ? -10 : 10), default, WorldState.FutureTime(10.1f)));
                break;
            case AID.TailSlap:
            case AID.TailSlapAlt:
                _aoes.Clear();
                ++NumCasts;
                break;
        }
    }
}

// The slap always hits the tile the marked player stands on when Shinryu dives. Breaking the center tile once it is cracked
// shatters every tile (the Alt slap; both full-floor collapses in the logs, an Unreal wipe and the first Extreme pull, while
// breaking a cracked corner did nothing more), so the center is never used once it is cracked. The marker goes to the healer,
// then a DPS, then the tank, and each has a fixed drop tile (Nonu's strat): healer south-west, DPS north-east (the center if the
// north-east is cracked and the center isn't), tank north-west. The marked player holds that tile until the dive.
class TailMarker(BossModule module) : BossComponent(module)
{
    private static readonly WPos _center = new(0, 0);
    private static readonly WPos _southWest = new(-20, 20);
    private static readonly WPos _northEast = new(20, -20);
    private static readonly WPos _northWest = new(-20, -20);
    public Actor? Target { get; private set; }
    private DateTime _dive;

    private bool CenterCracked => Module.FindComponent<ArenaChanges>()?.Cracked(_center) == true;
    private bool Before(Actor actor) => actor == Target && _dive != default && WorldState.CurrentTime < _dive;
    private static bool OnCenter(Actor actor) => Math.Abs(actor.Position.X) < 10 && Math.Abs(actor.Position.Z) < 10;

    // null when the strat tile is already gone
    private WPos? Spot(Actor actor)
    {
        var arena = Module.FindComponent<ArenaChanges>();
        bool Clean(WPos tile) => arena == null || arena.Intact(tile) && !arena.Cracked(tile);
        var spot = actor.Role switch
        {
            Role.Healer => _southWest,
            Role.Tank => _northWest,
            _ => !Clean(_northEast) && Clean(_center) ? _center : _northEast
        };
        return arena == null || arena.Intact(spot) ? spot : null;
    }

    private static string SpotName(WPos spot) => spot == _southWest ? "south-west" : spot == _northWest ? "north-west" : spot == _northEast ? "north-east" : "center";

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == (uint)IconID.TailMarker)
        {
            Target = actor;
            _dive = WorldState.FutureTime(8.1f);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.TailDiveNorth or AID.TailDiveSouth)
            _dive = default;
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Tail)
            Target = null;
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Before(actor) && Spot(actor) is { } spot)
            hints.AddForbiddenZone(ShapeDistance.InvertedRect(spot, new WDir(0, 1), 8, 8, 8), _dive);
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (!Before(actor))
            return;
        if (CenterCracked && OnCenter(actor))
            hints.Add("Tail marker: get off the cracked center tile, breaking it drops the whole floor!");
        else if (Spot(actor) is { } spot)
            hints.Add($"Tail marker: drop it on the {SpotName(spot)} tile", false);
        else
            hints.Add("Tail marker: drop it away from the party, not on a cracked center", false);
    }

    public override void DrawArenaBackground(int pcSlot, Actor pc)
    {
        if (Before(pc) && Spot(pc) is { } spot)
            Arena.ZoneRect(spot, new WDir(0, 1), 10, 10, 10, ArenaColor.SafeFromAOE);
    }

    public override PlayerPriority CalcPriority(int pcSlot, Actor pc, int playerSlot, Actor player, ref uint customColor) => player == Target ? PlayerPriority.Danger : PlayerPriority.Irrelevant;
}

// Thin Ice lasts 5s after Diamond Dust, and any step on it slides you 25y in a straight line (30 of 38 slides in the logs measured
// 25.0y, the rest were cut short when it wore off), holes included, so the AI holds still from just before it lands until it ends.
class ThinIce(BossModule module) : BossComponent(module)
{
    private DateTime _lands;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.DiamondDust)
            _lands = Module.CastFinishAt(spell, 0.6f);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (actor.FindStatus(SID.ThinIce) != null)
            hints.AddSpecialMode(AIHints.SpecialMode.PyreticMove, WorldState.CurrentTime);
        else if (_lands != default && WorldState.CurrentTime < _lands)
            hints.AddSpecialMode(AIHints.SpecialMode.PyreticMove, _lands);
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (actor.FindStatus(SID.ThinIce) != null)
            hints.Add("Thin ice: any step slides you 25y, stand still!");
        else if (_lands != default && WorldState.CurrentTime < _lands && WorldState.CurrentTime > _lands.AddSeconds(-2))
            hints.Add("Stop moving before the ice lands", false);
    }
}

// Four Reiryu spawn at 1 HP (17.3s after Diamond Dust in P1, then waves in P3); players can only heal them. 8s in each tethers its
// own player and casts Dragonflight (30s), to be healed to full before it ends. One with nobody left to tether leaves 13.0s after
// spawning with Dragon Spirit, a raidwide adding a Damage Down stack (solo pulls: one tether, the rest left). Healed ones vanish.
// Dragonflight is not cast in every pull, so the timeline counts spawn waves.
class Dragonflight : Components.CastHint
{
    private const float HealWindow = 13;
    public int NumWaves { get; private set; }
    private DateTime _lastSpawn;

    public Dragonflight(BossModule module) : base(module, AID.Dragonflight, "Heal Reiryu to full!", true)
    {
        KeepOnPhaseChange = true;
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID != OID.Reiryu)
            return;
        if (WorldState.CurrentTime > _lastSpawn.AddSeconds(5))
            ++NumWaves;
        _lastSpawn = WorldState.CurrentTime;
    }

    // a tethered Reiryu has until its Dragonflight ends, an untethered one until it leaves
    private DateTime Deadline(Actor r) => r.CastInfo?.IsSpell(AID.Dragonflight) == true ? Module.CastFinishAt(r.CastInfo) : _lastSpawn.AddSeconds(HealWindow);

    public override void AddGlobalHints(GlobalHints hints)
    {
        var unhealed = Module.Enemies(OID.Reiryu).Where(r => !r.IsDead && r.IsTargetable && r.HPMP.CurHP < r.HPMP.MaxHP && Deadline(r) > WorldState.CurrentTime).ToList();
        if (unhealed.Count > 0)
            hints.Add($"Heal all Reiryu to full: {unhealed.Count} left, {(unhealed.Min(Deadline) - WorldState.CurrentTime).TotalSeconds:f0}s");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var r in Module.Enemies(OID.Reiryu))
            hints.SetPriority(r, AIHints.Enemy.PriorityForbidden);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc) => Arena.Actors(Module.Enemies(OID.Reiryu), ArenaColor.Vulnerable);
}

class Cocoons(BossModule module) : Components.AddsMulti(module, [(uint)OID.Cocoon, (uint)OID.MassiveCocoon]);
class DragonAdds(BossModule module) : Components.AddsMulti(module, [(uint)OID.Ginryu, (uint)OID.Hakkinryu], 1);
// each cocoon casts Meteor Impact for show (0 damage in every log); the helper on the same spot casts the real hit
class MeteorImpactMarker(BossModule module) : Components.ProximityAOEs(module, AID.MeteorImpactMarker, 10);
// the marked player drops a cocoon meteor, the same 10y proximity hit as the other cocoons
class CocoonMarker(BossModule module) : Components.UniformStackSpread(module, 0, 10, alwaysShowSpreads: true)
{
    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == (uint)IconID.CocoonMarker)
            AddSpread(actor, WorldState.FutureTime(8));
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.MeteorImpactMarker)
            Spreads.Clear();
    }

    public override void Update() => Spreads.RemoveAll(s => s.Activation.AddSeconds(1) < WorldState.CurrentTime);
}
class Touchdown(BossModule module) : Components.ProximityAOEs(module, AID.Touchdown, 14);

// After the Dark Matter fall the party lands on the origin square; Touchdown then throws everyone north onto the platform at
// (0,-80). All 8 players stood at (0,-28) in every kill; anywhere else the throw misses the platform.
class TouchdownSpot(BossModule module) : BossComponent(module)
{
    private static readonly WPos _spot = new(0, -28);
    public bool Done { get; private set; }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.Touchdown)
            Done = true;
    }

    private bool Active => !Done && Arena.Center == default;

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Active)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(_spot, 2));
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (Active && !actor.Position.InCircle(_spot, 2))
            hints.Add("Go to the north edge, Touchdown throws you onto the platform!");
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (Active)
            Arena.AddCircle(_spot, 2, ArenaColor.Safe, 2);
    }
}

// Tail Spit knocks everyone 15y from the platform center; standing about 6.5y to the north-west lands on the start of the tail
// at (-15,-97), the way up to the next arena.
class TailSpitKnockback(BossModule module) : Components.KnockbackFromCastTarget(module, AID.TailSpit, 15)
{
    private static readonly WPos _spot = Ex3Shinryu.P2Center + 6.5f * (-135f).Degrees().ToDirection();

    public override PointTest DestinationUnsafe(int slot, Actor actor) => PointTest.Never;

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Casters.Count > 0)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(_spot, 1.5f), Module.CastFinishAt(Casters[0].CastInfo));
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (Casters.Count > 0 && !actor.Position.InCircle(_spot, 1.5f))
            hints.Add("Stand north-west of the center, the knockback throws you onto the tail!");
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        base.DrawArenaForeground(pcSlot, pc);
        if (Casters.Count > 0)
            Arena.AddCircle(_spot, 1.5f, ArenaColor.Safe, 2);
    }
}
class Fireball(BossModule module) : Components.StandardAOEs(module, AID.Fireball, 4);
class BlazingTrail(BossModule module) : Components.StandardAOEs(module, AID.BlazingTrail, new AOEShapeRect(15, 5.5f));
class SpikedTail(BossModule module) : Components.SingleTargetCast(module, AID.SpikedTail);
class DeathSentence(BossModule module) : Components.SingleTargetCast(module, AID.DeathSentence);

class TeraSlash(BossModule module) : Components.SingleTargetCast(module, AID.TeraSlash);
class AtomicRay(BossModule module) : Components.StackWithIcon(module, (uint)IconID.AtomicRay, AID.AtomicRay, 6, 5.7f, 2, 2);
class AetherialRay(BossModule module) : Components.StandardAOEs(module, AID.AetherialRay, new AOEShapeRect(100, 1.5f));
class Wormwail(BossModule module) : Components.StandardAOEs(module, AID.Wormwail, new AOEShapeDonut(10, 50));

// The puddle under Shinryu after Shatter inflicts a 10s Doom while you stand in it and drops it once you leave. Across 9 kills
// players gained it at up to 13.8y from the center and stayed clear from 15y, hence 14. Wormwail's donut forces everyone in for a
// moment, so it is only drawn as an outline then. During the Tidal Wave enrage the party jumps onto Shinryu's back from the pads
// at (±20,0) and lands about (2,-8) some 13y up; anyone on the back is above the puddle, and anyone who falls off lands in it.
class DoomPuddle : BossComponent
{
    private const float Radius = 14;
    private static readonly WPos _back = new(2, -8);
    private readonly HashSet<ulong> _activePads = [];

    public DoomPuddle(BossModule module) : base(module)
    {
        KeepOnPhaseChange = true;
    }

    private bool OnBack(Actor actor) => actor.PosRot.Y > Module.PrimaryActor.PosRot.Y + 4;
    private bool WormwailActive => Module.PrimaryActor.CastInfo?.IsSpell(AID.Wormwail) == true;
    private IEnumerable<Actor> ActivePads() => Module.Enemies(OID.JumpPadWest).Concat(Module.Enemies(OID.JumpPadEast)).Where(p => _activePads.Contains(p.InstanceID));
    private Actor? ClosestPad(Actor actor) => ActivePads().MinBy(p => (p.Position - actor.Position).LengthSq());

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if ((OID)actor.OID is OID.JumpPadWest or OID.JumpPadEast)
        {
            if (state == 0x00040008)
                _activePads.Add(actor.InstanceID);
            else if (state == 0x00010002)
                _activePads.Remove(actor.InstanceID);
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (OnBack(actor))
        {
            if (_activePads.Count > 0)
                hints.AddForbiddenZone(ShapeDistance.InvertedCircle(_back, 4)); // don't walk off the back
            return;
        }
        if (!WormwailActive)
            hints.AddForbiddenZone(ShapeDistance.Circle(Module.PrimaryActor.Position, Radius));
        if (ClosestPad(actor) is { } pad)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(pad.Position, 2));
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (OnBack(actor))
            return;
        if (actor.FindStatus(SID.Doom) != null)
            hints.Add("Doom: get out of the puddle!");
        if (ClosestPad(actor) != null)
            hints.Add("Jump onto Shinryu's back from a pad!");
    }

    public override void DrawArenaBackground(int pcSlot, Actor pc)
    {
        if (!OnBack(pc) && !WormwailActive)
            Arena.ZoneCircle(Module.PrimaryActor.Position, Radius, ArenaColor.AOE);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (OnBack(pc))
            return;
        if (WormwailActive)
            Arena.AddCircle(Module.PrimaryActor.Position, Radius, ArenaColor.Danger);
        foreach (var pad in ActivePads())
            Arena.AddCircle(pad.Position, 2, ArenaColor.Safe, 2);
    }
}
class BenightingBreath(BossModule module) : Components.StandardAOEs(module, AID.BenightingBreath, new AOEShapeCone(50, 45.Degrees()));
class Enrage(BossModule module) : Components.CastHint(module, AID.TidalWaveEnrage, "Enrage", true);

// Kill order: Tail > Worm's Heart > Shinryu. On the back during the Tidal Wave enrage, the wing whose enrage lands first
// (Judgment Bolt, cast about 5s before Hellfire) goes down first.
class TargetPriority : BossComponent
{
    public TargetPriority(BossModule module) : base(module)
    {
        KeepOnPhaseChange = true;
    }

    public Actor? FirstWing() => Module.Enemies(OID.LeftWingP3).Concat(Module.Enemies(OID.RightWingP3))
        .Where(w => !w.IsDead && w.IsTargetable && w.CastInfo != null && w.CastInfo.IsSpell() && (AID)w.CastInfo.Action.ID is AID.JudgmentBoltEnrage or AID.HellfireEnrage)
        .MinBy(w => w.CastInfo!.RemainingTime);

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var first = FirstWing();
        foreach (var e in hints.PotentialTargets)
        {
            if (e.Priority < 0)
                continue;
            var prio = (OID)e.Actor.OID switch
            {
                OID.Tail => 3,
                OID.WormsHeart => 2,
                OID.LeftWingP3 or OID.RightWingP3 => e.Actor == first ? 2 : 1,
                _ => 0
            };
            e.Priority = Math.Max(e.Priority, prio);
        }
    }

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (Module.Enemies(OID.Tail).Any(t => !t.IsDead && t.IsTargetable))
            hints.Add("Kill the tail");
        else if (Module.Enemies(OID.WormsHeart).Any(h => !h.IsDead && h.IsTargetable))
            hints.Add("Kill the Worm's Heart");
        else if (FirstWing() is { } wing)
            hints.Add($"Kill the {((OID)wing.OID == OID.LeftWingP3 ? "left" : "right")} wing first");
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        Arena.Actors(Module.Enemies(OID.WormsHeart), ArenaColor.Enemy);
        if (FirstWing() is { } wing)
            Arena.Actor(wing, ArenaColor.Vulnerable);
    }
}

// Dark Matter (P2) is an HP push: in Unreal it comes at the first check (+275.8, +294.0 or +351.0 into the pull) with Shinryu
// below 45%. Every pull at 40.5-44.4% at the first check pushed there, every one at 46.3% or more waited for the next (13 pulls).
// Extreme's one pushed pull went at 52.3%, so its threshold is not known and no target is shown there yet.
class DarkMatterPush(BossModule module) : BossComponent(module)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        var hp = Module.PrimaryActor.HPMP;
        if (Ex3Shinryu.DarkMatterPushPct is not { } threshold || hp.MaxHP == 0 || Module.StateMachine.TimeSincePhaseEnter < 240)
            return;
        var pct = 100f * hp.CurHP / hp.MaxHP;
        if (pct > threshold)
            hints.Add($"Push to {threshold:f0}% for Dark Matter (now {pct:f1}%)");
    }
}

class Ex3ShinryuStates : StateMachineBuilder
{
    public Ex3ShinryuStates(BossModule module) : base(module)
    {
        SimplePhase(0, Phase1, "P1")
            .ActivateOnEnter<ArenaChanges>()
            .Raw.Update = () => Module.PrimaryActor.IsDeadOrDestroyed || Module.PrimaryActor.CastInfo?.IsSpell(AID.DarkMatter) == true;
        SimplePhase(1, Phase2, "P2")
            .Raw.Update = () => Module.PrimaryActor.IsDeadOrDestroyed || Module.FindComponent<ArenaChanges>()?.Shattered == true;
        SimplePhase(2, Phase3, "P3")
            .Raw.Update = () => Module.PrimaryActor.IsDeadOrDestroyed || Module.PrimaryActor.CastInfo?.IsSpell(AID.TidalWaveEnrage) == true;
        DeathPhase(3, Phase4);
    }

    private void Phase1(uint id)
    {
        ActorCast(id, () => Module.PrimaryActor, AID.EarthenFuryVisual, 10.2f, 10, true, "Raidwide + tiles")
            .ActivateOnEnter<EarthenFury>()
            .ActivateOnEnter<EarthenFuryTile>()
            .ActivateOnEnter<TidalWave>()
            .ActivateOnEnter<WaterPuddles>()
            .ActivateOnEnter<Paralysis>()
            .ActivateOnEnter<BurningChains>()
            .ActivateOnEnter<Dragonflight>()
            .ActivateOnEnter<TailMarker>()
            .ActivateOnEnter<TailSlap>()
            .ActivateOnEnter<IcicleAdds>()
            .ActivateOnEnter<Spikesicle>()
            .ActivateOnEnter<SpikesicleKnockback>()
            .ActivateOnEnter<Levinbolt>()
            .ActivateOnEnter<Hypernova>()
            .ActivateOnEnter<IceStorm>()
            .ActivateOnEnter<Dragonfist>()
            .ActivateOnEnter<AkhMorn>()
            .ActivateOnEnter<AkhRhai>()
            .ActivateOnEnter<AkhRhaiBait>()
            .ActivateOnEnter<Hellfire>()
            .ActivateOnEnter<JudgmentBolt>()
            .ActivateOnEnter<EarthBreath>()
            .ActivateOnEnter<DiamondDust>()
            .ActivateOnEnter<ThinIce>()
            .ActivateOnEnter<AerialBlast>()
            .ActivateOnEnter<SuperCyclone>()
            .ActivateOnEnter<GyreCharge>()
            .ActivateOnEnter<TargetPriority>()
            .ActivateOnEnter<DarkMatterPush>()
            .SetHint(StateMachine.StateHint.Raidwide);
        Cast(id + 0x10, AID.TidalWaveVisual, 13.2f, 10, "Knockback")
            .SetHint(StateMachine.StateHint.Knockback);
        ActorCast(id + 0x20, LeftWing, AID.SummonIcicle, 13.1f, 4, false, "Icicles");
        ActorCastMulti(id + 0x30, RightWing, [AID.LevinboltVisual, AID.HypernovaVisual], 10.1f, 8, false, "Spread/stack");
        ComponentCondition<IceStorm>(id + 0x40, 15.4f, comp => comp.NumCasts > 0, "Raidwide", 4)
            .SetHint(StateMachine.StateHint.Raidwide);
        Cast(id + 0x50, AID.AkhMorn, 8.9f, 4, "Shared tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        CastMulti(id + 0x60, [AID.HellfireVisual, AID.JudgmentBoltVisual], 5.3f, 10, "Raidwide (water)")
            .SetHint(StateMachine.StateHint.Raidwide);
        ActorCast(id + 0x70, RightWing, AID.LevinboltVisual, 18.2f, 8, false, "Spread");
        ActorCast(id + 0x80, LeftWing, AID.IceStormVisual, 9.0f, 6, false);
        ComponentCondition<IceStorm>(id + 0x82, 0.6f, comp => comp.NumCasts > 1, "Raidwide")
            .SetHint(StateMachine.StateHint.Raidwide);
        Cast(id + 0x90, AID.EarthBreath, 9.5f, 6, "Baits");
        Cast(id + 0xA0, AID.AkhMorn, 3.4f, 4, "Shared tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        ActorCast(id + 0xB0, LeftWing, AID.IceStormVisual, 0, 6, false);
        ComponentCondition<IceStorm>(id + 0xB2, 0.6f, comp => comp.NumCasts > 2, "Raidwide")
            .SetHint(StateMachine.StateHint.Raidwide);
        Cast(id + 0xC0, AID.DiamondDustVisual, 3.7f, 10, "Raidwide + thin ice")
            .SetHint(StateMachine.StateHint.Raidwide);
        ComponentCondition<Dragonflight>(id + 0xD0, 17.3f, comp => comp.NumWaves > 0, "Reiryu (heal)");
        ActorCast(id + 0xE0, LeftWing, AID.SummonIcicle, 39.1f, 4, false, "Icicles");
        Cast(id + 0xF0, AID.AkhMorn, 0, 4, "Shared tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        Cast(id + 0x100, AID.AerialBlastVisual, 9.3f, 10, "Knockback")
            .SetHint(StateMachine.StateHint.Knockback);
        Cast(id + 0x110, AID.EarthBreath, 21.1f, 6, "Baits");
        ComponentCondition<IceStorm>(id + 0x120, 3.7f, comp => comp.NumCasts > 3, "Raidwide")
            .SetHint(StateMachine.StateHint.Raidwide);
        ComponentCondition<GyreCharge>(id + 0x130, 7.0f, comp => comp.Casters.Count > 0, "Gyre charge", 3);
        ActorCast(id + 0x140, RightWing, AID.HypernovaVisual, 13.3f, 8, false, "Stack");
        // Dark Matter is an HP push; without it P1 goes on (seen once, a slow solo pull: Dark Matter came at +391)
        Cast(id + 0x150, AID.AkhMorn, 8.4f, 4, "Shared tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        ActorCastStart(id + 0x160, RightWing, AID.LevinboltVisual, 7.6f);
        CastStart(id + 0x170, AID.TidalWaveVisual, 4.0f);
        ActorCastEnd(id + 0x171, RightWing, 3.7f, false, "Spread");
        CastEnd(id + 0x172, 6.0f, "Knockback")
            .SetHint(StateMachine.StateHint.Knockback);
        SimpleState(id + 0xFF0000, 100, "???");
    }

    private void Phase2(uint id)
    {
        CastEnd(id, 3, "Raidwide")
            .ActivateOnEnter<DarkMatter>()
            .ActivateOnEnter<Touchdown>()
            .ActivateOnEnter<TouchdownSpot>()
            .ActivateOnEnter<TailSpitKnockback>()
            .ActivateOnEnter<Cocoons>()
            .ActivateOnEnter<DragonAdds>()
            .ActivateOnEnter<MeteorImpactMarker>()
            .ActivateOnEnter<CocoonMarker>()
            .ActivateOnEnter<Fireball>()
            .ActivateOnEnter<BlazingTrail>()
            .ActivateOnEnter<SpikedTail>()
            .ActivateOnEnter<DeathSentence>()
            .ActivateOnEnter<Protostar>()
            .ActivateOnEnter<TailSpit>()
            .ActivateOnEnter<TailSpitCenter>()
            .ActivateOnEnter<AetherialRay>()
            .SetHint(StateMachine.StateHint.Raidwide);
        ComponentCondition<Touchdown>(id + 0x20, 38.4f, comp => comp.NumCasts > 0, "Proximity", 2)
            .SetHint(StateMachine.StateHint.DowntimeEnd);
        ComponentCondition<CocoonMarker>(id + 0x30, 46.5f, comp => comp.Active, "Cocoon markers", 3);
        Cast(id + 0x40, AID.Protostar, 31.0f, 6, "Raidwide")
            .SetHint(StateMachine.StateHint.Raidwide);
        ComponentCondition<TailSpit>(id + 0x50, 9.4f, comp => comp.NumCasts > 0, "Raidwide", 2)
            .SetHint(StateMachine.StateHint.Raidwide);
        Timeout(id + 0x60, 22.2f, "Shatter");
    }

    private void Phase3(uint id)
    {
        CastStart(id, AID.TeraSlash, 18.2f)
            .ActivateOnEnter<TeraSlash>()
            .ActivateOnEnter<AtomicRay>()
            .ActivateOnEnter<AetherialRay>()
            .ActivateOnEnter<Wormwail>()
            .ActivateOnEnter<DoomPuddle>()
            .ActivateOnEnter<BenightingBreath>()
            .ActivateOnEnter<IceStormP3>()
            .ActivateOnEnter<IceStormFreeze>()
            .ActivateOnEnter<HypernovaP3>()
            .ActivateOnEnter<LevinboltP3Hint>()
            .ActivateOnEnter<LevinboltP3>();
        CastEnd(id + 1, 4, "Tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        ComponentCondition<AtomicRay>(id + 0x10, 6.2f, comp => comp.Active, "", 2);
        ComponentCondition<AtomicRay>(id + 0x11, 7.2f, comp => !comp.Active, "Stacks", 2);
        ActorCast(id + 0x20, LeftWingP3, AID.IceStormVisualP3, 7.0f, 6, false);
        ComponentCondition<IceStormP3>(id + 0x22, 0.6f, comp => comp.NumCasts > 0, "Raidwide")
            .SetHint(StateMachine.StateHint.Raidwide);
        CastMulti(id + 0x30, [AID.Wormwail, AID.BenightingBreathVisual], 6.4f, 4.5f, "Donut/cone");
        Cast(id + 0x40, AID.TeraSlash, 5.2f, 4, "Tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        ComponentCondition<Dragonflight>(id + 0x50, 4.3f, comp => comp.NumWaves > 1, "Reiryu (heal)", 3);
        Cast(id + 0x60, AID.TeraSlash, 42.0f, 4, "Tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        ComponentCondition<AtomicRay>(id + 0x70, 6.2f, comp => comp.Active, "", 2);
        ComponentCondition<AtomicRay>(id + 0x71, 7.2f, comp => !comp.Active, "Stacks", 2);
        ActorCast(id + 0x80, LeftWingP3, AID.IceStormVisualP3, 7.0f, 6, false);
        ComponentCondition<IceStormP3>(id + 0x82, 0.6f, comp => comp.NumCasts > 1, "Raidwide")
            .SetHint(StateMachine.StateHint.Raidwide);
        CastMulti(id + 0x90, [AID.Wormwail, AID.BenightingBreathVisual], 6.4f, 4.5f, "Donut/cone");
        SimpleState(id + 0xFF0000, 100, "???");
    }

    private void Phase4(uint id)
    {
        CastEnd(id, 70, "Enrage")
            .ActivateOnEnter<Enrage>()
            .SetHint(StateMachine.StateHint.DowntimeStart);
    }

    private Actor? LeftWing() => Module.Enemies(OID.LeftWing).FirstOrDefault();
    private Actor? RightWing() => Module.Enemies(OID.RightWing).FirstOrDefault();
    private Actor? LeftWingP3() => Module.Enemies(OID.LeftWingP3).FirstOrDefault();
}

class ArenaChanges : BossComponent
{
    public bool Shattered { get; private set; }
    private static readonly ArenaBoundsSquare _p1 = new(30, MapResolution: 0.3f);
    private bool _p1Active = true;
    private readonly bool[] _broken = new bool[9];
    private readonly bool[] _cracked = new bool[9];

    private static int TileIndex(WPos tile) => Math.Clamp((int)(tile.X / 20 + 1), 0, 2) + 3 * Math.Clamp((int)(tile.Z / 20 + 1), 0, 2);

    public bool Intact(WPos p) => Math.Abs(p.X) <= 30 && Math.Abs(p.Z) <= 30 && !_broken[TileIndex(Ex3Shinryu.TileCenter(p))];
    public bool Cracked(WPos p) => _cracked[TileIndex(Ex3Shinryu.TileCenter(p))] && Intact(p);

    public VBool Broken(WVec p)
    {
        Span<int> res = stackalloc int[VBool.Count];
        for (var i = 0; i < VBool.Count; ++i)
            res[i] = Intact(new(p.X[i], p.Z[i])) ? 0 : -1;
        return new(res);
    }

    public ArenaChanges(BossModule module) : base(module)
    {
        KeepOnPhaseChange = true;
    }

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (!_p1Active || (OID)actor.OID != OID.Platform)
            return;
        var center = Ex3Shinryu.TileCenter(actor.Position);
        if (state == 0x00040008)
            _cracked[TileIndex(center)] = true;
        if ((state >> 16) != 2 || (state & 0xFFFF) == 0x8000)
            return;
        _broken[TileIndex(center)] = true;
        Arena.Bounds = new ArenaBoundsCustom(30, _p1.Clipper.Simplify(BuildTiles(), Clipper2Lib.FillRule.NonZero), MapResolution: 0.3f);
    }

    // Intact tiles, plus a 2y crossing wherever two tiles only meet at a corner (players cross there; the tank's north-west tail
    // spot is often only reachable that way), since the pathfinder can't pass through a single point.
    private PolygonClipper.Operand BuildTiles()
    {
        var shape = new PolygonClipper.Operand();
        WDir TileOffset(int col, int row) => new(col * 20 - 20, row * 20 - 20);
        bool Whole(int col, int row) => !_broken[col + 3 * row];
        for (var row = 0; row < 3; ++row)
            for (var col = 0; col < 3; ++col)
                if (Whole(col, row))
                    shape.AddContour(CurveApprox.Rect(TileOffset(col, row), new WDir(0, 1), 10, 10));
        for (var row = 0; row < 2; ++row)
        {
            for (var col = 0; col < 2; ++col)
            {
                var corner = new WDir(col * 20 - 10, row * 20 - 10);
                bool a = Whole(col, row), b = Whole(col + 1, row), c = Whole(col, row + 1), d = Whole(col + 1, row + 1);
                if (a && d && !b && !c)
                    shape.AddContour(CurveApprox.Rect(corner, new WDir(1, 1).Normalized(), 1, 2));
                else if (b && c && !a && !d)
                    shape.AddContour(CurveApprox.Rect(corner, new WDir(-1, 1).Normalized(), 1, 2));
            }
        }
        return shape;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.DarkMatter && _p1Active)
        {
            _p1Active = false;
            Arena.Bounds = _p1;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.Touchdown:
                Arena.Center = Ex3Shinryu.P2Center;
                Arena.Bounds = Ex3Shinryu.P2Bounds;
                break;
            case AID.TailSpit:
                Arena.Center = Ex3Shinryu.ClimbCenter;
                Arena.Bounds = Ex3Shinryu.ClimbBounds;
                break;
            case AID.Shatter:
                Shattered = true;
                Arena.Center = default;
                Arena.Bounds = _p1;
                break;
        }
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 278, NameID = 5640, PlanLevel = 70, Incomplete = true)]
public class Ex3Shinryu(ModuleInit init) : BossModule(init, new(0, 0), new ArenaBoundsSquare(30, MapResolution: 0.3f))
{
    public static float? DarkMatterPushPct => null; // not measured in Extreme yet
    public static readonly WPos P2Center = new(0, -80);
    public static readonly ArenaBoundsSquare P2Bounds = new(20);
    public static readonly WPos ClimbCenter = new(0, -50);
    public static readonly ArenaBoundsCustom ClimbBounds = BuildClimbBounds();

    private static ArenaBoundsCustom BuildClimbBounds()
    {
        WPos[] tail = [new(-15, -97), new(-4, -86), new(2, -76), new(6, -66), new(10, -56), new(14, -46), new(17, -36), new(17, -26)];
        var clipper = new PolygonClipper();
        var shape = new PolygonClipper.Operand(CurveApprox.Rect(P2Center - ClimbCenter, new WDir(0, 1), 20, 20));
        shape.AddContour(CurveApprox.Rect(new WPos(0, 0) - ClimbCenter, new WDir(0, 1), 30, 30));
        for (int i = 1; i < tail.Length; ++i)
        {
            var dir = (tail[i] - tail[i - 1]).Normalized();
            shape.AddContour(CurveApprox.Rect((tail[i] - ClimbCenter + (tail[i - 1] - ClimbCenter)) * 0.5f, dir, 5, (tail[i] - tail[i - 1]).Length() * 0.5f + 2));
        }
        return new(85, clipper.Simplify(shape, Clipper2Lib.FillRule.NonZero));
    }

    public static WPos TileCenter(WPos p) => new(MathF.Round(p.X / 20) * 20, MathF.Round(p.Z / 20) * 20);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor, ArenaColor.Enemy, true);
        Arena.Actors(Enemies(OID.Tail), ArenaColor.Enemy, true);
    }
}
