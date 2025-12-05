// https://github.com/ppy/osu/blob/master/osu.Game/Collection/BeatmapCollection.cs

using Realms;

namespace OsuRealmMerger.Models
{
    public class BeatmapCollection : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public IList<string> BeatmapMD5Hashes { get; } = null!;

        public DateTimeOffset LastModified { get; set; }
    }
}