namespace BossMod.RealmReborn.Alliance.A22GlasyaLabolas;

public enum OID : uint
{
    Boss = 0xBD7, // R4.000, x1
    Helper = 0x1B2, // R0.500, x3
    ClockworkWright = 0xBD9, // R0.600, spawn in sets of 3 on the ring ~24y out, tethered to boss
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    AstralPunch = 2346, // Boss->self, no cast, range 8+R 90-degree cone
    Aura = 2348, // Helper->location, 3.5s cast, range 8 circle
    BloodMoon = 2349, // Boss->self, no cast, range 60 circle, raidwide ~6s after wrights spawn
    VileUtterance = 2353, // Boss->self, 3.0s cast, range 60+R 45-degree cone
}

public enum SID : uint
{
    Electrocution = 288, // none->player, extra=0x0, while a wright holds them
    Fetters = 510, // none->player, extra=0x0, while a wright holds them
}

public enum TetherID : uint
{
    Wright = 22, // ClockworkWright->Boss, later retargets to a player (applies fetters + electrocution)
}

class AstralPunch(BossModule module) : Components.Cleave(module, AID.AstralPunch, new AOEShapeCone(12, 45.Degrees()));
class Aura(BossModule module) : Components.StandardAOEs(module, AID.Aura, 8);
class VileUtterance(BossModule module) : Components.StandardAOEs(module, AID.VileUtterance, new AOEShapeCone(64, 22.5f.Degrees()));

class ClockworkWrights(BossModule module) : BossComponent(module)
{
    private IEnumerable<Actor> Wrights => Module.Enemies(OID.ClockworkWright).Where(w => !w.IsDead);

    private Actor? Held(Actor wright)
        => wright.Tether.ID == (uint)TetherID.Wright && WorldState.Actors.Find(wright.Tether.Target) is { } target && Raid.FindSlot(target.InstanceID) >= 0 ? target : null;

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (Wrights.Any(w => Held(w) == actor))
            hints.Add("Bound by a wright until it dies!", false);
    }

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (Wrights.Any(w => Held(w) != null))
            hints.Add("Kill the wright holding a player");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var w in Wrights)
            hints.SetPriority(w, Held(w) != null ? 1 : AIHints.Enemy.PriorityPointless);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        Arena.Actors(Wrights, ArenaColor.Enemy);
        foreach (var w in Wrights)
            if (w.Tether.ID == (uint)TetherID.Wright && WorldState.Actors.Find(w.Tether.Target) is { } target)
                Arena.AddLine(w.Position, target.Position, target == pc ? ArenaColor.Danger : ArenaColor.Object);
    }
}

class A22GlasyaLabolasStates : StateMachineBuilder
{
    public A22GlasyaLabolasStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<AstralPunch>()
            .ActivateOnEnter<Aura>()
            .ActivateOnEnter<VileUtterance>()
            .ActivateOnEnter<ClockworkWrights>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 102, NameID = 2815, SortOrder = 5)]
public class A22GlasyaLabolas(ModuleInit init) : BossModule(init, new(0, -200), new ArenaBoundsCircle(30));
