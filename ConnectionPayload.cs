using System;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace Rcon.Function
{
    /// <summary>
    /// Payload defintion
    /// </summary>
    public class ConnectionPayload
    {
        [JsonIgnore, BsonIgnoreIfNull, SwaggerIgnore]
        public object Id { get; set; }
        public string AccessToken { get; set; }
        public string Server { get; set; }
        public int? Port { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public bool IsEnabled {get; set;}
        public string Role {get; set;}
        [JsonIgnore, BsonIgnore, SwaggerIgnore]
        public bool? IsValid { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public DateTime? InsertedOn {get; set; }
        [JsonIgnore, BsonIgnoreIfNull, SwaggerIgnore]
        public DateTime? UpdatedOn {get; set;}
        [JsonIgnore, BsonIgnoreIfNull, SwaggerIgnore]
        public DateTime? TouchedOn {get; set; }
    }
}