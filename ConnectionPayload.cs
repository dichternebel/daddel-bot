using System;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace Rcon.Function
{
    /// <summary>
    /// Payload defintion
    /// </summary>
    public class ConnectionPayload
    {
        [JsonIgnore, BsonIgnoreIfNull]
        internal object Id { get; set; }
        public string AccessToken { get; set; }
        public string Server { get; set; }
        public int? Port { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public bool IsEnabled {get; set;}
        public string Role {get; set;}
        [JsonIgnore, BsonIgnore]
        internal bool? IsValid { get; set; }
        [JsonIgnore]
        internal DateTime? InsertedOn {get; set; }
        [JsonIgnore, BsonIgnoreIfNull]
        internal DateTime? UpdatedOn {get; set;}
        [JsonIgnore, BsonIgnoreIfNull]
        internal DateTime? TouchedOn {get; set; }
    }
}