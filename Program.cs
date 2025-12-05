using Realms;
using OsuRealmMerger.Models;
using System.Collections;
using System.Reflection;

class Program
{
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
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run -- -o <output.realm> -s <input1.realm> -s <input2.realm>");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  -o, --output   The path where the merged database will be saved.");
            Console.WriteLine("  -s, --source   A path to a source database (can be used multiple times).");
            return;
        }

        if (File.Exists(targetPath))
            Console.WriteLine("WARNING: Appending to existing file. For a clean merge, delete the output file first.");

        var outputDir = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        var targetConfig = new RealmConfiguration(targetPath) { SchemaVersion = 51 };

        try
        {
            using var targetRealm = Realm.GetInstance(targetConfig);
            Console.WriteLine($"Target database opened at: {targetPath}");

            foreach (var sourcePath in sourceFiles)
            {
                if (!File.Exists(sourcePath)) { Console.WriteLine($"Skipping: {sourcePath}, file doesn't exist"); continue; }

                Console.WriteLine($"\nProcessing: {sourcePath}");
                using var sourceRealm = Realm.GetInstance(new RealmConfiguration(sourcePath) { IsReadOnly = true, SchemaVersion = 51 });

                var objectCache = new Dictionary<RealmObjectBase, RealmObjectBase>();

                // 1. Dependencies
                Console.WriteLine("Merging Files...");
                Merge<RealmFile>(sourceRealm, targetRealm, objectCache);

                Console.WriteLine("Merging Rulesets...");
                Merge<RulesetInfo>(sourceRealm, targetRealm, objectCache);

                Console.WriteLine("Merging RulesetSettings...");
                Merge<RulesetSetting>(sourceRealm, targetRealm, objectCache);

                Console.WriteLine("Merging ModPreset...");
                Merge<ModPreset>(sourceRealm, targetRealm, objectCache);

                Console.WriteLine("Merging KeyBindings...");
                Merge<KeyBindingInfo>(sourceRealm, targetRealm, objectCache);

                Console.WriteLine("Merging Skins...");
                Merge<SkinInfo>(sourceRealm, targetRealm, objectCache);

                // 2. Beatmaps
                Console.WriteLine("Merging Collections...");
                Merge<BeatmapCollection>(sourceRealm, targetRealm, objectCache);

                Console.WriteLine("Merging BeatmapSets...");
                Merge<BeatmapSetInfo>(sourceRealm, targetRealm, objectCache);

                // 3. Scores
                Console.WriteLine("Merging Scores...");
                Merge<ScoreInfo>(sourceRealm, targetRealm, objectCache);
            }
            Console.WriteLine("\nDone! Database merged.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CRITICAL ERROR: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    static void Merge<T>(Realm source, Realm target, Dictionary<RealmObjectBase, RealmObjectBase> cache) where T : RealmObject, new()
    {
        var sourceObjects = source.All<T>().ToList();
        int addedCount = 0;

        target.Write(() =>
        {
            foreach (var srcObj in sourceObjects)
            {
                var pkProp = typeof(T).GetProperties().FirstOrDefault(p => Attribute.IsDefined(p, typeof(PrimaryKeyAttribute)));

                if (pkProp != null)
                {
                    var id = pkProp.GetValue(srcObj);
                    var existing = FindGeneric<T>(target, id!);

                    if (existing == null)
                    {
                        T newObj = CloneObject(srcObj, target, cache);
                        target.Add(newObj);
                        addedCount++;
                    }
                }
                else
                {
                    T newObj = CloneObject(srcObj, target, cache);
                    target.Add(newObj);
                    addedCount++;
                }
            }
        });
        if (addedCount > 0) Console.WriteLine($"   + Added {addedCount} {typeof(T).Name}s");
    }

    private static T? FindGeneric<T>(Realm realm, object id) where T : RealmObject
    {
        return realm.Find<T>((dynamic)id);
    }

    static T CloneObject<T>(T source, Realm targetRealm, Dictionary<RealmObjectBase, RealmObjectBase> cache) where T : RealmObjectBase, new()
    {
        if (cache.TryGetValue(source, out var existing))
            return (T)existing;

        T clone = new();
        cache[source] = clone;

        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.Name == "Realm" || prop.Name == "IsValid" || prop.Name == "IsFrozen" || prop.Name == "BacklinksCount") continue;

            var val = prop.GetValue(source);
            if (val == null) continue;

            if (val is IList sourceList && prop.PropertyType.IsGenericType)
            {
                if (prop.GetValue(clone) is IList targetList)
                {
                    foreach (var item in sourceList)
                    {
                        if (item is IRealmObjectBase roItem)
                        {
                            var child = HandleChild((RealmObjectBase)roItem, targetRealm, cache);

                            if (clone is BeatmapSetInfo parentSet && child is BeatmapInfo childMap)
                                childMap.BeatmapSet = parentSet;

                            if (child != null) targetList.Add(child);
                        }
                        else targetList.Add(item);
                    }
                }
                continue;
            }

            if (!prop.CanWrite) continue;

            if (val is IRealmObjectBase roVal)
            {
                if (prop.Name == "BeatmapSet") continue;

                var child = HandleChild((RealmObjectBase)roVal, targetRealm, cache);
                if (child != null)
                {
                    try { prop.SetValue(clone, child); } catch { }
                }
            }
            else
            {
                try { prop.SetValue(clone, val); } catch { }
            }
        }
        return clone;
    }

    static RealmObjectBase? HandleChild(RealmObjectBase sourceChild, Realm targetRealm, Dictionary<RealmObjectBase, RealmObjectBase> cache)
    {
        var pkProp = sourceChild.GetType().GetProperties().FirstOrDefault(p => Attribute.IsDefined(p, typeof(PrimaryKeyAttribute)));
        if (pkProp != null)
        {
            var id = pkProp.GetValue(sourceChild);
            if (id != null)
            {
                MethodInfo findMethod = typeof(Program).GetMethod(nameof(FindGeneric), BindingFlags.Static | BindingFlags.NonPublic)!;
                MethodInfo generic = findMethod.MakeGenericMethod(sourceChild.GetType());
                var existing = (RealmObjectBase?)generic.Invoke(null, [targetRealm, id]);

                if (existing != null) return existing;
            }
        }

        MethodInfo cloneMethod = typeof(Program).GetMethod(nameof(CloneObject), BindingFlags.Static | BindingFlags.NonPublic)!;
        MethodInfo cloneGeneric = cloneMethod.MakeGenericMethod(sourceChild.GetType());
        return (RealmObjectBase)cloneGeneric.Invoke(null, [sourceChild, targetRealm, cache])!;
    }
}