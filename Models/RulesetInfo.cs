// https://github.com/ppy/osu/tree/master/osu.Game/Rulesets/RulesetInfo.cs

using Realms;

namespace OsuRealmMerger.Models
{
    [MapTo("Ruleset")]
    public class RulesetInfo : RealmObject
    {
        [PrimaryKey]
        public string ShortName { get; set; } = string.Empty;

        [Indexed]
        public int OnlineID { get; set; } = -1;

        public string Name { get; set; } = string.Empty;

        public string InstantiationInfo { get; set; } = string.Empty;

        public int LastAppliedDifficultyVersion { get; set; }

        public bool Available { get; set; }
    }
}