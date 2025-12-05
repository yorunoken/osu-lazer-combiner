// https://github.com/ppy/osu/blob/master/osu.Game/Beatmaps/BeatmapSetInfo.cs

using Realms;
using OsuRealmMerger.Models.Embedded;

namespace OsuRealmMerger.Models
{
    [MapTo("BeatmapSet")]
    public class BeatmapSetInfo : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        [Indexed]
        public int OnlineID { get; set; } = -1;

        public DateTimeOffset DateAdded { get; set; }

        public DateTimeOffset? DateSubmitted { get; set; }

        public DateTimeOffset? DateRanked { get; set; }

        public IList<BeatmapInfo> Beatmaps { get; } = null!;

        public IList<RealmNamedFileUsage> Files { get; } = null!;

        [MapTo("Status")]
        public int StatusInt { get; set; }

        public bool DeletePending { get; set; }

        public string Hash { get; set; } = string.Empty;

        public bool Protected { get; set; }
    }
}