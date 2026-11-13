namespace BossMod.Endwalker.Raid.P2NHippokampos;

public enum OID : uint
{
    Boss = 0x3599, // R9.002, x1
    Helper = 0x233C, // R0.500, x24, Helper type
    Hippokampos = 0x359A, // R8.002, x1
    Hippokampos1 = 0x3869, // R9.002, x1
    ActorFE36D = 0xFE36D, // R0.500, x1, EventNpc type
    ActorFE36E = 0xFE36E, // R0.500, x1, EventNpc type
    Exit = 0x1E850B, // R0.500, x1, EventObj type
    Actor1EA1A1 = 0x1EA1A1, // R2.000, x1, EventObj type
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    SewageDeluge = 26614, // Boss->self, 5.0s cast, range 60 circle
    SpokenCataract = 26616, // Hippokampos->self, 7.0s cast, range 60 180-degree cone
    SpokenCataract1 = 26617, // Hippokampos->self, 7.0s cast, range 60 180-degree cone
    SpokenCataract2 = 26621, // Boss->self, 7.0s cast, range 100 width 15 rect
    Coherence = 26624, // Hippokampos->location, no cast, range 60 circle
    Coherence1 = 26625, // Boss->self, 5.0s cast, single-target
    Coherence2 = 26626, // Boss->self, no cast, range 60 width 6 rect
    Ability26627 = 26627, // Boss->self, no cast, single-target
    Dissociation = 26628, // Boss->self, 4.0s cast, single-target
    Ability26629 = 26629, // Boss->self, no cast, single-target
    Dissociation1 = 26630, // Hippokampos1->self, 8.0s cast, range 50 width 20 rect
    Shockwave = 26631, // Boss->location, 6.0s cast, range 60 circle
    TaintedFlood = 26632, // Boss->self, 3.0s cast, single-target
    TaintedFlood1 = 26633, // Helper->players, 5.2s cast, range 6 circle
    PredatorySight = 26634, // Boss->self, 4.0s cast, single-target
    PredatorySight1 = 26635, // Helper->player, 5.0s cast, single-target
    DoubledImpact = 26638, // Boss->players, 5.0s cast, range 6 circle
    MurkyDepths = 26639, // Boss->self, 5.0s cast, range 60 circle
    Ability26677 = 26677, // Boss->location, no cast, single-target
    Ability27924 = 27924, // Helper->player, no cast, single-target
}

public enum IconID : uint
{
    Icon55 = 55, // player
    Icon87 = 87, // player
    Icon259 = 259, // player
    Icon310 = 310, // player
}

class SewageDeluge(BossModule module) : Components.RaidwideCast(module, AID.SewageDeluge);
class SpokenCataract(BossModule module) : Components.StandardAOEs(module, AID.SpokenCataract, new AOEShapeCone(60, 90.Degrees()));
class SpokenCataract1(BossModule module) : Components.StandardAOEs(module, AID.SpokenCataract1, new AOEShapeCone(60, 90.Degrees()));
class SpokenCataract2(BossModule module) : Components.StandardAOEs(module, AID.SpokenCataract2, new AOEShapeRect(100, 7.5f));
class Dissociation1(BossModule module) : Components.StandardAOEs(module, AID.Dissociation1, new AOEShapeRect(50, 10));
class Shockwave(BossModule module) : Components.RaidwideCast(module, AID.Shockwave);
class PredatorySight1(BossModule module) : Components.SingleTargetCast(module, AID.PredatorySight1);
class MurkyDepths(BossModule module) : Components.RaidwideCast(module, AID.MurkyDepths);

class P2NHippokamposStates : StateMachineBuilder
{
    public P2NHippokamposStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<SewageDeluge>()
            .ActivateOnEnter<SpokenCataract>()
            .ActivateOnEnter<SpokenCataract1>()
            .ActivateOnEnter<SpokenCataract2>()
            .ActivateOnEnter<Dissociation1>()
            .ActivateOnEnter<Shockwave>()
            .ActivateOnEnter<PredatorySight1>()
            .ActivateOnEnter<MurkyDepths>();
    }
}

[ModuleInfo(Contributors = "Nonu", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 810, NameID = 10348, PlanLevel = 90, Incomplete = true)]
public class P2NHippokampos(ModuleInit init) : BossModule(init, new(100, 100), new ArenaBoundsSquare(20));
