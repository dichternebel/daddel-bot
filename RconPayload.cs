namespace Rcon.Function
{
    internal class RconPayload
    {
        public object Id { get; set; }
        public string AccessToken { get; set; }
        public long? Salt {get; set;}
        public string[] Parameter { get; set; }
        public bool? IsValid {get; set;}
    }
}