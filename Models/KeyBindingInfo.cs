// https://github.com/osu/osu.Game/Input/Bindings/RealmKeyBinding.cs

using Realms;

namespace OsuRealmMerger.Models
{
    [MapTo("KeyBinding")]
    public class KeyBindingInfo : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string? RulesetName { get; set; }

        public int? Variant { get; set; }

        [MapTo("Action")]
        public int ActionInt { get; set; }

        [MapTo("KeyCombination")]
        public string KeyCombinationString { get; set; } = null!;
    }
}