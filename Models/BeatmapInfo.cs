// https://github.com/ppy/osu/blob/master/osu.Game/Beatmaps/BeatmapInfo.cs

using Realms;
using OsuRealmMerger.Models.Embedded;

namespace OsuRealmMerger.Models
{
    [MapTo("Beatmap")]
    public class BeatmapInfo : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string DifficultyName { get; set; } = string.Empty;

        public RulesetInfo Ruleset { get; set; } = null!;

        public BeatmapDifficulty Difficulty { get; set; } = null!;

        public BeatmapMetadata Metadata { get; set; } = null!;

        public BeatmapUserSettings UserSettings { get; set; } = null!;

        public BeatmapSetInfo? BeatmapSet { get; set; }

        [MapTo("Status")]
        public int StatusInt { get; set; }

        [Indexed]
        public int OnlineID { get; set; } = -1;

        public double Length { get; set; }

        public double BPM { get; set; }

        public string Hash { get; set; } = string.Empty;

        public double StarRating { get; set; } = -1;

        [Indexed]
        public string MD5Hash { get; set; } = string.Empty;

        public string OnlineMD5Hash { get; set; } = string.Empty;

        public DateTimeOffset? LastLocalUpdate { get; set; }

        public DateTimeOffset? LastOnlineUpdate { get; set; }

        public bool Hidden { get; set; }

        public int EndTimeObjectCount { get; set; } = -1;

        public int TotalObjectCount { get; set; } = -1;

        public DateTimeOffset? LastPlayed { get; set; }

        public int BeatDivisor { get; set; } = 4;

        public double? EditorTimestamp { get; set; }
    }
}