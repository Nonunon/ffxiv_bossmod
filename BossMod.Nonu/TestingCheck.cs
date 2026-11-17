namespace BossMod.Nonu;

[ConfigDisplay(Name = "Nonu pack", Order = 90)]
public sealed class NonuPackConfig : ConfigNode
{
    [PropertyDisplay("Testing check", tooltip: "When a boss whose module is marked WIP (Incomplete) is pulled, say so in chat with the module's contributors.")]
    public bool TestingCheck = true;
}

// A zone module for each duty with a WIP module (the list is generated, see TestingCheckDuties.cs). It draws nothing; it
// only watches for the pull of each WIP boss in the duty and posts one chat line per module per visit.
public sealed partial class TestingCheck(WorldState ws) : ZoneModule(ws)
{
    private readonly NonuPackConfig _config = Service.Config.Get<NonuPackConfig>();
    private readonly BossModuleConfig _bmConfig = Service.Config.Get<BossModuleConfig>();
    private readonly HashSet<Type> _announced = [];
    private Dictionary<uint, BossModuleRegistry.Info>? _modules; // by primary actor OID, built on first update

    public override void Update()
    {
        if (!_config.TestingCheck)
            return;

        var cfc = World.CurrentCFCID;
        _modules ??= BossModuleRegistry.RegisteredModules.Values
            .Where(m => m.GroupID == cfc && m.GroupType is not (BossModuleInfo.GroupType.None or BossModuleInfo.GroupType.Quest or BossModuleInfo.GroupType.Hunt))
            .ToDictionary(m => m.PrimaryActorOID);

        foreach (var actor in World.Actors)
        {
            if (!actor.InCombat || !_modules.TryGetValue(actor.OID, out var info) || !_announced.Add(info.ModuleType))
                continue;

            if (!info.Incomplete)
                continue;

            var boss = BossName(info, actor);
            var who = info.Contributors.Length > 0 ? $"the contributors are {info.Contributors}, please contact them with any issues" : "no contributors are listed";
            var off = _bmConfig.AllowIncompleteModules ? "" : " It is not running: \"Allow WIP modules\" is off.";
            Post($"[Testing check] The {boss} module is marked WIP (Incomplete); {who}.{off}");
        }
    }

    private static string BossName(BossModuleRegistry.Info info, Actor actor)
    {
        try
        {
            if (info.NameID != 0)
                return ModuleViewer.BNpcName(info.NameID);
        }
        catch (Exception)
        {
        }
        return actor.Name.Length > 0 ? actor.Name : info.ModuleType.Name;
    }

    private static void Post(string msg)
    {
        Service.Log($"[Nonu] {msg}");
        if (Service.ChatGui is { } chat)
            chat.Print(msg, "BM-N");
    }
}
