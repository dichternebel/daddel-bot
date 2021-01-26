using MongoDB.Bson.Serialization.Attributes;

namespace Rcon.Function
{
    public class RconPayload
    {
        public object Id { get; set; }
        public string AccessToken { get; set; }
        public string[] Parameter { get; set; }
        [BsonIgnore]
        public bool? IsValid {get; set;}
    }
}