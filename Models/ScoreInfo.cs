// https://github.com/ppy/osu/tree/master/osu.Game/Scoring/ScoreInfo.cs

using Realms;
using OsuRealmMerger.Models.Embedded;

namespace OsuRealmMerger.Models
{
    [MapTo("Score")]
    public class ScoreInfo : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public BeatmapInfo? BeatmapInfo { get; set; }

        public string ClientVersion { get; set; } = string.Empty;

        public string BeatmapHash { get; set; } = string.Empty;

        public RulesetInfo Ruleset { get; set; } = null!;

        public IList<RealmNamedFileUsage> Files { get; } = null!;

        public string Hash { get; set; } = string.Empty;

        public bool DeletePending { get; set; }

        public long TotalScore { get; set; }

        public int TotalScoreVersion { get; set; }

        public long? LegacyTotalScore { get; set; }

        public bool BackgroundReprocessingFailed { get; set; }

        public int MaxCombo { get; set; }

        public double Accuracy { get; set; }

        public DateTimeOffset Date { get; set; }

        public double? PP { get; set; }

        [Indexed]
        public long OnlineID { get; set; } = -1;

        [Indexed]
        public long LegacyOnlineID { get; set; } = -1;

        [MapTo("User")]
        public RealmUser RealmUser { get; set; } = null!;

        [MapTo("Mods")]
        public string ModsJson { get; set; } = string.Empty;

        [MapTo("Statistics")]
        public string StatisticsJson { get; set; } = string.Empty;

        [MapTo("MaximumStatistics")]
        public string MaximumStatisticsJson { get; set; } = string.Empty;

        [MapTo("Rank")]
        public int RankInt { get; set; }

        public int Combo { get; set; }

        public bool IsLegacyScore { get; set; }

        public long TotalScoreWithoutMods { get; set; }

        public IList<int> Pauses { get; } = null!;
    }
}