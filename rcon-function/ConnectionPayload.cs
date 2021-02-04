using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Rcon.Function
{
    /// <summary>
    /// Payload defintion
    /// </summary>
    public class ConnectionPayload
    {
        [JsonIgnore, BsonIgnoreIfNull]
        public object Id { get; set; }
        public string AccessToken { get; set; }
        public string Server { get; set; }
        public int? Port { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore, BsonIgnore]
        public long? Salt {get; set;}
        public bool IsEnabled {get; set;}
        public string Role {get; set;}
        [JsonIgnore, BsonIgnore]
        public bool? IsValid { get; set; }
        [JsonIgnore]
        public DateTime? InsertedOn {get; set; }
        [JsonIgnore, BsonIgnoreIfNull]
        public DateTime? UpdatedOn {get; set;}
        [JsonIgnore, BsonIgnoreIfNull]
        public DateTime? TouchedOn {get; set; }
    }
}