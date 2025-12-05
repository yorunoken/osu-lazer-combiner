using Realms;

namespace OsuRealmMerger.Models.Embedded
{
    public class RealmNamedFileUsage : EmbeddedObject
    {
        public RealmFile File { get; set; } = null!;

        public string Filename { get; set; } = null!;

    }
}
