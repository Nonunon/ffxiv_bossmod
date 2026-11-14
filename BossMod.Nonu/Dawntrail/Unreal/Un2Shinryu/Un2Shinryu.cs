namespace BossMod.Dawntrail.Unreal.Un2Shinryu;

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
            _aoes.Add(new(_shape, Un2Shinryu.TileCenter(caster.Position + (caster.Position - Arena.Center).Normalized() * 10), default, Module.CastFinishAt(spell)));
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
class HypernovaP3(BossModule module) : Components.CastHint(module, AID.HypernovaVisualP3, "Stack together");
class LevinboltP3Hint(BossModule module) : Components.CastHint(module, AID.LevinboltVisualP3, "Spread, puddles follow");
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
class BurningChains : Components.Chains
{
    public BurningChains(BossModule module) : base(module, (uint)TetherID.BurningChains, AID.BurningChains)
    {
        KeepOnPhaseChange = true;
    }
}

class TailSlap(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect _shape = new(20, 10, 20);
    private readonly List<(Actor Tail, DateTime Activation)> _tails = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _tails.Select(t => new AOEInstance(_shape, t.Tail.Position, t.Tail.Rotation, t.Activation));

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Tail)
            _tails.Add((actor, WorldState.FutureTime(5.7f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.TailSlap or AID.TailSlapAlt)
        {
            _tails.RemoveAll(t => t.Tail == caster);
            ++NumCasts;
        }
    }
}

class TailMarker(BossModule module) : BossComponent(module)
{
    private Actor? _target;

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == (uint)IconID.TailMarker)
            _target = actor;
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Tail)
            _target = null;
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (actor == _target)
            hints.Add("Tail marker: drop it away from the party, on a tile that can break");
    }

    public override PlayerPriority CalcPriority(int pcSlot, Actor pc, int playerSlot, Actor player, ref uint customColor) => player == _target ? PlayerPriority.Danger : PlayerPriority.Irrelevant;
}

class ThinIce(BossModule module) : BossComponent(module)
{
    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (actor.FindStatus(SID.ThinIce) != null)
            hints.Add("Thin ice: you slide when moving, mind the holes!", false);
    }
}

class Dragonflight : Components.CastHint
{
    public Dragonflight(BossModule module) : base(module, AID.Dragonflight, "Heal Reiryu to full!", true)
    {
        KeepOnPhaseChange = true;
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
class MeteorImpact(BossModule module) : Components.ProximityAOEs(module, AID.MeteorImpact, 10);
class MeteorImpactMarker(BossModule module) : Components.ProximityAOEs(module, AID.MeteorImpactMarker, 10);
class CocoonMarker(BossModule module) : Components.UniformStackSpread(module, 0, 5, alwaysShowSpreads: true)
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
class Fireball(BossModule module) : Components.StandardAOEs(module, AID.Fireball, 4);
class BlazingTrail(BossModule module) : Components.StandardAOEs(module, AID.BlazingTrail, new AOEShapeRect(15, 5.5f));
class SpikedTail(BossModule module) : Components.SingleTargetCast(module, AID.SpikedTail);
class DeathSentence(BossModule module) : Components.SingleTargetCast(module, AID.DeathSentence);

class TeraSlash(BossModule module) : Components.SingleTargetCast(module, AID.TeraSlash);
class AtomicRay(BossModule module) : Components.StackWithIcon(module, (uint)IconID.AtomicRay, AID.AtomicRay, 6, 5.7f, 2, 2);
class AetherialRay(BossModule module) : Components.StandardAOEs(module, AID.AetherialRay, new AOEShapeRect(100, 1.5f));
class Wormwail(BossModule module) : Components.StandardAOEs(module, AID.Wormwail, new AOEShapeDonut(10, 50));
class BenightingBreath(BossModule module) : Components.StandardAOEs(module, AID.BenightingBreath, new AOEShapeCone(50, 45.Degrees()));
class Enrage(BossModule module) : Components.CastHint(module, AID.TidalWaveEnrage, "Enrage", true);

class Un2ShinryuStates : StateMachineBuilder
{
    public Un2ShinryuStates(BossModule module) : base(module)
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
        ComponentCondition<Dragonflight>(id + 0xD0, 25.1f, comp => comp.Casters.Count > 0, "Reiryu (heal)", 2);
        ActorCast(id + 0xE0, LeftWing, AID.SummonIcicle, 31.2f, 4, false, "Icicles");
        Cast(id + 0xF0, AID.AkhMorn, 0, 4, "Shared tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        Cast(id + 0x100, AID.AerialBlastVisual, 9.3f, 10, "Knockback")
            .SetHint(StateMachine.StateHint.Knockback);
        Cast(id + 0x110, AID.EarthBreath, 21.1f, 6, "Baits");
        ComponentCondition<IceStorm>(id + 0x120, 3.7f, comp => comp.NumCasts > 3, "Raidwide")
            .SetHint(StateMachine.StateHint.Raidwide);
        ComponentCondition<GyreCharge>(id + 0x130, 7.0f, comp => comp.Casters.Count > 0, "Gyre charge", 3);
        ActorCast(id + 0x140, RightWing, AID.HypernovaVisual, 13.3f, 8, false, "Stack");
        CastStart(id + 0x150, AID.DarkMatter, 1.1f, "Raidwide");
    }

    private void Phase2(uint id)
    {
        CastEnd(id, 3, "Raidwide")
            .ActivateOnEnter<DarkMatter>()
            .ActivateOnEnter<Touchdown>()
            .ActivateOnEnter<Cocoons>()
            .ActivateOnEnter<DragonAdds>()
            .ActivateOnEnter<MeteorImpact>()
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
        Cast(id, AID.TeraSlash, 18.2f, 4, "Tankbuster")
            .ActivateOnEnter<TeraSlash>()
            .ActivateOnEnter<AtomicRay>()
            .ActivateOnEnter<AetherialRay>()
            .ActivateOnEnter<Wormwail>()
            .ActivateOnEnter<BenightingBreath>()
            .ActivateOnEnter<IceStormP3>()
            .ActivateOnEnter<HypernovaP3>()
            .ActivateOnEnter<LevinboltP3Hint>()
            .ActivateOnEnter<LevinboltP3>()
            .SetHint(StateMachine.StateHint.Tankbuster);
        ComponentCondition<AtomicRay>(id + 0x10, 6.2f, comp => comp.Active, "", 2);
        ComponentCondition<AtomicRay>(id + 0x11, 7.2f, comp => !comp.Active, "Stacks", 2);
        ActorCast(id + 0x20, LeftWingP3, AID.IceStormVisualP3, 7.0f, 6, false);
        ComponentCondition<IceStormP3>(id + 0x22, 0.6f, comp => comp.NumCasts > 0, "Raidwide")
            .SetHint(StateMachine.StateHint.Raidwide);
        CastMulti(id + 0x30, [AID.Wormwail, AID.BenightingBreathVisual], 6.4f, 4.5f, "Donut/cone");
        Cast(id + 0x40, AID.TeraSlash, 5.2f, 4, "Tankbuster")
            .SetHint(StateMachine.StateHint.Tankbuster);
        ComponentCondition<Dragonflight>(id + 0x50, 12.3f, comp => comp.Casters.Count > 0, "Reiryu (heal)", 3);
        Cast(id + 0x60, AID.TeraSlash, 34.0f, 4, "Tankbuster")
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
    private PolygonClipper.Operand _tiles = new(CurveApprox.Rect(new WDir(0, 1), 30, 30));
    private bool _p1Active = true;
    private readonly bool[] _broken = new bool[9];

    private static int TileIndex(WPos tile) => Math.Clamp((int)(tile.X / 20 + 1), 0, 2) + 3 * Math.Clamp((int)(tile.Z / 20 + 1), 0, 2);

    public bool Intact(WPos p) => Math.Abs(p.X) <= 30 && Math.Abs(p.Z) <= 30 && !_broken[TileIndex(Un2Shinryu.TileCenter(p))];

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
        if (!_p1Active || (OID)actor.OID != OID.Platform || (state >> 16) != 2 || (state & 0xFFFF) == 0x8000)
            return;
        var center = Un2Shinryu.TileCenter(actor.Position);
        _broken[TileIndex(center)] = true;
        var offset = center - Arena.Center;
        var tile = new PolygonClipper.Operand(CurveApprox.Rect(new WDir(0, 1), 10, 10).Select(c => c + offset));
        _tiles = new(_p1.Clipper.Difference(_tiles, tile));
        Arena.Bounds = new ArenaBoundsCustom(30, _p1.Clipper.Simplify(_tiles), MapResolution: 0.3f);
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
                Arena.Center = Un2Shinryu.P2Center;
                Arena.Bounds = Un2Shinryu.P2Bounds;
                break;
            case AID.TailSpit:
                Arena.Center = Un2Shinryu.ClimbCenter;
                Arena.Bounds = Un2Shinryu.ClimbBounds;
                break;
            case AID.Shatter:
                Shattered = true;
                Arena.Center = default;
                Arena.Bounds = _p1;
                break;
        }
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1118, NameID = 5640, PlanLevel = 100, Incomplete = true)]
public class Un2Shinryu(ModuleInit init) : BossModule(init, new(0, 0), new ArenaBoundsSquare(30, MapResolution: 0.3f))
{
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
