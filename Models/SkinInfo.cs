// https://github.com/ppy/osu/tree/master/osu.Game/Skinning/SkinInfo.cs

using OsuRealmMerger.Models.Embedded;
using Realms;

namespace OsuRealmMerger.Models
{
    [MapTo("Skin")]
    public class SkinInfo : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string Name { get; set; } = null!;

        public string Creator { get; set; } = null!;

        public string InstantiationInfo { get; set; } = null!;

        public string Hash { get; set; } = string.Empty;

        public bool Protected { get; set; }

        public IList<RealmNamedFileUsage> Files { get; } = null!;

        public bool DeletePending { get; set; }
    }
}