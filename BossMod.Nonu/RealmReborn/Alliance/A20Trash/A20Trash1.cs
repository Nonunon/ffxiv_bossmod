namespace BossMod.RealmReborn.Alliance.A20Trash1;

public enum OID : uint
{
    YellowDragon = 0xBCC, // R7.000, x1
    Shinobi = 0xBC8, // R0.600, x3
    Kunoichi = 0xBC9, // R0.600, x3
    DarkGeneral = 0xBCA, // R1.500, x2
    Oceanus = 0xBCB, // R3.450, x2
}

public enum AID : uint
{
    AutoAttack = 870, // YellowDragon/Shinobi/Kunoichi/DarkGeneral/Oceanus->player, no cast, single-target
    Douse = 2333, // Oceanus->location, 3.0s cast, range 8 circle
    Drench = 2334, // Oceanus->self, no cast, range 10+R 120-degree cone
    PlasmaRelease = 2375, // YellowDragon->self, 2.0s cast, range 30 circle, covers the whole arena
}

class Douse(BossModule module) : Components.StandardAOEs(module, AID.Douse, 8);
class Drench(BossModule module) : Components.Cleave(module, AID.Drench, new AOEShapeCone(13.45f, 60.Degrees()), (uint)OID.Oceanus);
class PlasmaRelease(BossModule module) : Components.RaidwideCast(module, AID.PlasmaRelease);

class A20Trash1States : StateMachineBuilder
{
    public A20Trash1States(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Douse>()
            .ActivateOnEnter<Drench>()
            .ActivateOnEnter<PlasmaRelease>()
            .Raw.Update = () => A20Trash1.Pack.All(oid => module.Enemies(oid).All(e => e.IsDead));
    }
}

[ModuleInfo(Contributors = "Nonu", PrimaryActorOID = (uint)OID.YellowDragon, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 102, NameID = 2799, SortOrder = 1)]
public class A20Trash1(ModuleInit init) : BossModule(init, new(0, 50), new ArenaBoundsCircle(29))
{
    public static readonly OID[] Pack = [OID.YellowDragon, OID.Shinobi, OID.Kunoichi, OID.DarkGeneral, OID.Oceanus];

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        foreach (var oid in Pack)
            Arena.Actors(Enemies(oid), ArenaColor.Enemy);
    }
}
