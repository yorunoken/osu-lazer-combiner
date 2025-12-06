using Realms;
using System.Collections;
using System.Reflection;
using osu.Game.Beatmaps;

namespace OsuRealmMerger.Core
{
    public static class RealmMerger
    {
        public static void Merge<T>(Realm source, Realm target, Dictionary<RealmObjectBase, RealmObjectBase> cache) where T : RealmObject
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

        public static T CloneObject<T>(T source, Realm targetRealm, Dictionary<RealmObjectBase, RealmObjectBase> cache) where T : RealmObjectBase
        {
            if (cache.TryGetValue(source, out var existing))
                return (T)existing;

            T clone = (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
            cache[source] = clone;

            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.Name == "Realm" || prop.Name == "IsValid" || prop.Name == "IsFrozen" || prop.Name == "BacklinksCount") continue;
                if (Attribute.IsDefined(prop, typeof(IgnoredAttribute))) continue;

                try
                {
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
                catch
                {
                    Console.WriteLine($"[Warning] Skipped property '{prop.Name}' on {typeof(T).Name}");
                }
            }
            return clone;
        }

        public static RealmObjectBase? HandleChild(RealmObjectBase sourceChild, Realm targetRealm, Dictionary<RealmObjectBase, RealmObjectBase> cache)
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
}