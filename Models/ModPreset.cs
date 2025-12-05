// https://github.com/ppy/osu/tree/master/osu.Game/Rulesets/Mods/ModPreset.cs

using Realms;

namespace OsuRealmMerger.Models
{
    public class ModPreset : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; } = Guid.NewGuid();

        public RulesetInfo Ruleset { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [MapTo("Mods")]
        public string ModsJson { get; set; } = string.Empty;

        public bool DeletePending { get; set; }
    }
}