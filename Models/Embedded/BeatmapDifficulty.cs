using Realms;

namespace OsuRealmMerger.Models.Embedded
{
    [MapTo("BeatmapDifficulty")]
    public class BeatmapDifficulty : EmbeddedObject
    {
        public const float DEFAULT_DIFFICULTY = 5;

        public float DrainRate { get; set; } = DEFAULT_DIFFICULTY;
        public float CircleSize { get; set; } = DEFAULT_DIFFICULTY;
        public float OverallDifficulty { get; set; } = DEFAULT_DIFFICULTY;
        public float ApproachRate { get; set; } = DEFAULT_DIFFICULTY;

        public double SliderMultiplier { get; set; } = 1.4;
        public double SliderTickRate { get; set; } = 1;
    }
}