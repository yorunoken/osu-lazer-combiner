using Realms;
using System.Collections;
using System.Reflection;

using osu.Game.Scoring;
using osu.Game.Beatmaps;
using osu.Game.Collections;
using osu.Game.Skinning;
using osu.Game.Configuration;
using osu.Game.Rulesets;
using osu.Game.Models;
using osu.Game.Input.Bindings;
using OsuRealmMerger.Core;

class Program
{
    const int OSU_SCHEMA_VERSION = 51;

    static void Main(string[] args)
    {
        var sourceFiles = new List<string>();
        string? targetPath = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-o" || args[i] == "--output")
            {
                if (i + 1 < args.Length) targetPath = args[++i];
            }
            else if (args[i] == "-s" || args[i] == "--source")
            {
                if (i + 1 < args.Length) sourceFiles.Add(args[++i]);
            }
        }

        if (string.IsNullOrEmpty(targetPath) || sourceFiles.Count == 0)
        {
            Console.WriteLine("Usage: dotnet run -- -s <input.realm> -o <output.realm>");
            return;
        }

        var targetConfig = new RealmConfiguration(targetPath)
        {
            SchemaVersion = OSU_SCHEMA_VERSION
        };

        try
        {
            using var targetRealm = Realm.GetInstance(targetConfig);
            Console.WriteLine($"Target database opened at: {targetPath}");

            foreach (var sourcePath in sourceFiles)
            {
                if (!File.Exists(sourcePath)) continue;

                Console.WriteLine($"\nProcessing: {sourcePath}");
                using var sourceRealm = Realm.GetInstance(new RealmConfiguration(sourcePath) { IsReadOnly = true, SchemaVersion = OSU_SCHEMA_VERSION });
                var objectCache = new Dictionary<RealmObjectBase, RealmObjectBase>();


                Console.WriteLine("Merging Dependencies...");
                RealmMerger.Merge<RealmFile>(sourceRealm, targetRealm, objectCache);
                RealmMerger.Merge<RulesetInfo>(sourceRealm, targetRealm, objectCache);
                RealmMerger.Merge<RealmRulesetSetting>(sourceRealm, targetRealm, objectCache);
                RealmMerger.Merge<RealmKeyBinding>(sourceRealm, targetRealm, objectCache);
                RealmMerger.Merge<SkinInfo>(sourceRealm, targetRealm, objectCache);

                Console.WriteLine("Merging Beatmaps...");
                RealmMerger.Merge<BeatmapCollection>(sourceRealm, targetRealm, objectCache);
                RealmMerger.Merge<BeatmapSetInfo>(sourceRealm, targetRealm, objectCache);

                Console.WriteLine("Merging Scores...");
                RealmMerger.Merge<ScoreInfo>(sourceRealm, targetRealm, objectCache);
            }
            Console.WriteLine("\nDone! Database merged.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CRITICAL ERROR: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}