using Realms;

namespace OsuRealmMerger.Models.Embedded
{
    public class RealmUser : EmbeddedObject
    {
        public int OnlineID { get; set; } = 1;

        public string Username { get; set; } = string.Empty;

        [MapTo("CountryCode")]
        public string CountryString { get; set; } = "0";
    }
}