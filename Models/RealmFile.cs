// https://github.com/ppy/osu/tree/master/osu.Game/Moddels/RealmFile.cs

using Realms;

namespace OsuRealmMerger.Models
{
    [MapTo("File")]
    public class RealmFile : RealmObject
    {
        [PrimaryKey]
        public string Hash { get; set; } = string.Empty;
    }
}