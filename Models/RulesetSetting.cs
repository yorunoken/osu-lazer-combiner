// https://github.com/ppy/osu/tree/master/osu.Game/Configuration/RealmRulesetSetting.cs

using Realms;

namespace OsuRealmMerger.Models
{
    [MapTo("RulesetSetting")]
    public class RulesetSetting : RealmObject
    {
        [Indexed]
        public string RulesetName { get; set; } = string.Empty;

        [Indexed]
        public int Variant { get; set; }

        [Required]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;

        public override string ToString() => $"{Key} => {Value}";
    }
}