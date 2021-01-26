using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Rcon.Function
{
    public class ConnectionPayload
    {
        [BsonIgnoreIfNull]
        public object Id { get; set; }
        public string AccessToken { get; set; }
        public string Server { get; set; }
        public int? Port { get; set; }
        public string Password { get; set; }
        public bool IsEnabled {get; set;}
        public string Role {get; set;}
        [BsonIgnore]
        public bool? IsValid { get; set; }
    }
}