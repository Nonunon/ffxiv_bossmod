namespace BossMod.RealmReborn.Alliance.A20Trash2;

public enum OID : uint
{
    Acheron = 0xBCE, // R2.500, x3, wave 1 (spawns as scylla dies)
    Gomory = 0xBCD, // R0.900, x6, wave 1
    GreenDragon = 0xBCF, // R7.000, x1, wave 2, lands in the center
    DogasClone = 0xBD0, // R0.600, x3, wave 2, while the dragon is up
    UneisClone = 0xBD6, // R0.600, x3, wave 2, after doga's clones die
}

public enum AID : uint
{
    AutoAttack = 870, // Acheron/Gomory/GreenDragon->player, no cast, single-target
    Aero = 969, // Gomory/UneisClone->player, 1.0s cast, single-target
    Fire = 966, // DogasClone->player, 1.0s cast, single-target
    Darkness = 1875, // Gomory->self, 1.0s cast, range 6+R 120-degree cone
    GreatDivide = 1725, // Acheron->self, no cast, range 5+R width 8 rect
    Touchdown = 564, // GreenDragon->self, no cast, range 10 circle, as it lands; resolves before the module can start, so no component
    SalivousSnap = 2445, // GreenDragon->player, no cast, single-target
    DragonBreath = 2338, // GreenDragon->self, no cast, range 30+R width 8 rect
}

class Darkness(BossModule module) : Components.StandardAOEs(module, AID.Darkness, new AOEShapeCone(6.9f, 60.Degrees()));
class GreatDivide(BossModule module) : Components.Cleave(module, AID.GreatDivide, new AOEShapeRect(7.5f, 4), (uint)OID.Acheron);
class DragonBreath(BossModule module) : Components.Cleave(module, AID.DragonBreath, new AOEShapeRect(37, 4), (uint)OID.GreenDragon);

class A20Trash2Pack1States : StateMachineBuilder
{
    public A20Trash2Pack1States(A20Trash2Pack1 module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Darkness>()
            .ActivateOnEnter<GreatDivide>()
            .Raw.Update = () => module.Enemies(OID.Acheron).All(e => e.IsDead) && module.Enemies(OID.Gomory).All(e => e.IsDead);
    }
}

[ModuleInfo(Contributors = "Nonu", PrimaryActorOID = (uint)OID.Acheron, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 102, NameID = 2795, SortOrder = 3)]
public class A20Trash2Pack1(ModuleInit init) : BossModule(init, new(0, -200), new ArenaBoundsCircle(29))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actors(Enemies(OID.Acheron), ArenaColor.Enemy);
        Arena.Actors(Enemies(OID.Gomory), ArenaColor.Enemy);
    }
}

class A20Trash2Pack2States : StateMachineBuilder
{
    public A20Trash2Pack2States(A20Trash2Pack2 module) : base(module)
    {
        var uneiSeen = false;
        TrivialPhase()
            .ActivateOnEnter<DragonBreath>()
            .Raw.Update = () =>
            {
                uneiSeen |= module.Enemies(OID.UneisClone).Count > 0;
                return uneiSeen && A20Trash2Pack2.Wave.All(oid => module.Enemies(oid).All(e => e.IsDead));
            };
    }
}

[ModuleInfo(Contributors = "Nonu", PrimaryActorOID = (uint)OID.GreenDragon, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 102, NameID = 2798, SortOrder = 4)]
public class A20Trash2Pack2(ModuleInit init) : BossModule(init, new(0, -200), new ArenaBoundsCircle(29))
{
    public static readonly OID[] Wave = [OID.GreenDragon, OID.DogasClone, OID.UneisClone];

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        foreach (var oid in Wave)
            Arena.Actors(Enemies(oid), ArenaColor.Enemy);
    }
}
