// https://github.com/ppy/osu/blob/master/osu.Game/Beatmaps/BeatmapMetadata.cs

using Realms;
using OsuRealmMerger.Models.Embedded;

namespace OsuRealmMerger.Models
{
    public class BeatmapMetadata : RealmObject
    {
        public string Title { get; set; } = string.Empty;

        public string TitleUnicode { get; set; } = string.Empty;

        public string Artist { get; set; } = string.Empty;

        public string ArtistUnicode { get; set; } = string.Empty;

        public RealmUser Author { get; set; } = null!;

        public string Source { get; set; } = string.Empty;

        public string Tags { get; set; } = string.Empty;

        public int PreviewTime { get; set; } = -1;

        public string AudioFile { get; set; } = string.Empty;

        public string BackgroundFile { get; set; } = string.Empty;

        public IList<string> UserTags { get; } = null!;
    }
}