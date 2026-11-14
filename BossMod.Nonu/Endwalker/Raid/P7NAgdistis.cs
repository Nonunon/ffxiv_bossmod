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
    Tower = 0x1EB793, // R0.500, x4, EventObj type
}

public enum AID : uint
{
    ImmortalsObol = 30709, // Boss->self, 5.5+2.0s cast, single-target
    ImmortalsObolAOE = 30710, // Helper->self, 7.5s cast, range 20 circle, raidwide, then the arena splits into three platforms
    HemitheossGlareIII = 30711, // Boss->self, 3.0s cast, single-target
    HemitheossGlareIIIAOE = 30712, // Helper->self, 7.0s cast, range 5-30 donut
    BoughOfAttisFront = 30713, // Boss->self, 6.8+1.2s cast, single-target
    BoughOfAttisFrontAOE = 30714, // Helper->self, 8.0s cast, range 19 circle
    BoughOfAttisLeft = 30715, // Boss->self, 4.0s cast, single-target
    BoughOfAttisRight = 30716, // Boss->self, 4.0s cast, single-target
    BoughOfAttisSideAOE = 30717, // Helper->self, 5.0s cast, range 50 width 25 rect
    BoughOfAttisBack = 30718, // Boss->self, 7.2+0.8s cast, single-target
    BoughOfAttisBackAOE = 30719, // Helper->self, 8.0s cast, range 25 circle
    ForbiddenFruit = 30720, // Boss->self, 4.0s cast, single-target
    ForbiddenFruit1 = 30721, // Boss->self, 3.0s cast, single-target
    StaticMoon = 30722, // ImmatureIo->self, 5.0s cast, range 10 circle
    StymphalianStrike = 30723, // ImmatureStymphalide->self, 5.0s cast, range 60 width 8 rect
    BladesOfAttis = 30724, // Boss->self, no cast, single-target
    BladesOfAttisFirst = 30725, // Helper->self, 5.0s cast, range 7 circle
    BladesOfAttisRest = 30726, // Helper->self, no cast, range 7 circle
    HemitheossHoly = 30727, // Boss->self, 3.0s cast, single-target
    HemitheossHolyAOE = 30728, // Helper->players, 6.0s cast, range 6 circle spread
    HemitheossAeroII = 30729, // Boss->self, 4.9s cast, single-target
    HemitheossAeroIIAOE = 30730, // Helper->players, 5.4s cast, range 8 circle, tankbuster on both tanks
    SparkOfLife = 30731, // Boss->self, 5.0s cast, range 60 circle
    ShadowOfAttis = 30780, // Boss->self, no cast, single-target
    Burst = 30781, // Helper->self, no cast, range 5 circle, tower
    AutoAttack = 30947, // Boss->self, no cast, single-target
    AutoAttack1 = 30948, // Helper->player, no cast, single-target
    HemitheossGlareIIIAOELong = 31226, // Helper->self, 10.0s cast, range 5-30 donut
}

public enum IconID : uint
{
    HemitheossHoly = 311, // player
    HemitheossAeroII = 364, // player
}

class ImmortalsObol(BossModule module) : Components.RaidwideCast(module, AID.ImmortalsObolAOE, "Raidwide + arena change");
class HemitheossGlareIII(BossModule module) : Components.StandardAOEs(module, AID.HemitheossGlareIIIAOE, new AOEShapeDonut(5, 30));
class HemitheossGlareIIILong(BossModule module) : Components.StandardAOEs(module, AID.HemitheossGlareIIIAOELong, new AOEShapeDonut(5, 30));
class BoughOfAttisFront(BossModule module) : Components.StandardAOEs(module, AID.BoughOfAttisFrontAOE, 19);
class BoughOfAttisSide(BossModule module) : Components.StandardAOEs(module, AID.BoughOfAttisSideAOE, new AOEShapeRect(50, 12.5f));
class BoughOfAttisBack(BossModule module) : Components.StandardAOEs(module, AID.BoughOfAttisBackAOE, 25);
class StaticMoon(BossModule module) : Components.StandardAOEs(module, AID.StaticMoon, 10);
class StymphalianStrike(BossModule module) : Components.StandardAOEs(module, AID.StymphalianStrike, new AOEShapeRect(60, 4));
class HemitheossHoly(BossModule module) : Components.SpreadFromCastTargets(module, AID.HemitheossHolyAOE, 6);
class HemitheossAeroII(BossModule module) : Components.BaitAwayCast(module, AID.HemitheossAeroIIAOE, new AOEShapeCircle(8), true);
class SparkOfLife(BossModule module) : Components.RaidwideCast(module, AID.SparkOfLife);

// 8 explosions, 7y apart, about 1.25s between them
class BladesOfAttis(BossModule module) : Components.Exaflare(module, 7)
{
    private readonly List<Actor> _casters = [];

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.BladesOfAttisFirst)
        {
            Lines.Add(new() { Next = caster.Position, Advance = 7 * spell.Rotation.ToDirection(), NextExplosion = Module.CastFinishAt(spell), TimeToMove = 1.25f, ExplosionsLeft = 8, MaxShownExplosions = 3 });
            _casters.Add(caster);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.BladesOfAttisFirst or AID.BladesOfAttisRest)
        {
            var index = _casters.IndexOf(caster);
            if (index < 0)
                return;
            AdvanceLine(Lines[index], caster.Position);
            if (Lines[index].ExplosionsLeft == 0)
            {
                Lines.RemoveAt(index);
                _casters.RemoveAt(index);
            }
        }
    }
}

// towers appear with the static moons; they go off 9.7s later, after the moons, and hit the whole party if left empty
class Towers(BossModule module) : Components.GenericTowers(module, AID.Burst)
{
    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Tower)
            Towers.Add(new(actor.Position, 5, 1, 2, default, WorldState.FutureTime(9.7f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.Burst)
        {
            ++NumCasts;
            Towers.RemoveAll(t => t.Position.AlmostEqual(caster.Position, 1));
        }
    }
}

// immortal's obol swaps the big platform for three small ones (P7S layout), then bridges join them; the big one comes back later
class Platforms(BossModule module) : BossComponent(module)
{
    public const float SmallRadius = 10;
    public const float SmallOffset = 16.5f;
    public const float BridgeHalfWidth = 4;

    private static readonly ArenaBoundsCircle _big = new(20);
    private static readonly WDir[] _centers = [SmallOffset * new WDir(0, 1), SmallOffset * 120.Degrees().ToDirection(), SmallOffset * (-120).Degrees().ToDirection()];
    private static readonly ArenaBoundsCustom _small = Build(false);
    private static readonly ArenaBoundsCustom _bridged = Build(true);

    private static ArenaBoundsCustom Build(bool bridges)
    {
        var pieces = _centers.Select(c => new PolygonClipper.Operand(CurveApprox.Circle(SmallRadius, 0.05f).Select(p => p + c))).ToList();
        if (bridges)
            for (var i = 0; i < 3; ++i)
            {
                var a = _centers[i];
                var b = _centers[(i + 1) % 3];
                pieces.Add(new(CurveApprox.Rect((a + b) * 0.5f, (b - a).Normalized(), BridgeHalfWidth, (b - a).Length() * 0.5f)));
            }
        return new(27, _big.Clipper.UnionAll(pieces[0], [.. pieces.Skip(1)]));
    }

    public override void OnMapEffect(byte index, uint state)
    {
        switch (index, state)
        {
            case (0, 0x00200010): // big platform gone
                Arena.Bounds = _small;
                break;
            case (0, 0x02000100): // big platform back
                Arena.Bounds = _big;
                break;
            case (1 or 2 or 3, 0x00020001): // bridges, all three appear together
                if (Arena.Bounds == _small)
                    Arena.Bounds = _bridged;
                break;
        }
    }
}

class P7NAgdistisStates : StateMachineBuilder
{
    public P7NAgdistisStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ImmortalsObol>()
            .ActivateOnEnter<HemitheossGlareIII>()
            .ActivateOnEnter<HemitheossGlareIIILong>()
            .ActivateOnEnter<BoughOfAttisFront>()
            .ActivateOnEnter<BoughOfAttisSide>()
            .ActivateOnEnter<BoughOfAttisBack>()
            .ActivateOnEnter<StaticMoon>()
            .ActivateOnEnter<StymphalianStrike>()
            .ActivateOnEnter<HemitheossHoly>()
            .ActivateOnEnter<HemitheossAeroII>()
            .ActivateOnEnter<SparkOfLife>()
            .ActivateOnEnter<BladesOfAttis>()
            .ActivateOnEnter<Towers>()
            .ActivateOnEnter<Platforms>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 876, NameID = 11374, PlanLevel = 90, Incomplete = true)]
public class P7NAgdistis(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsCircle(20));
