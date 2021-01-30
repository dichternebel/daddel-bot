using System;
using System.Threading.Tasks;
using RconSharp;

namespace Rcon.Function
{
    internal class RconService
    {
        public ConnectionPayload connection { get; private set; }
        public CosmosDbContext context { get; private set; }

        public RconService(ConnectionPayload connection, CosmosDbContext context)
        {
            this.connection = connection;
            this.context = context;
        }

        public async Task<RconClient> GetClient()
        {
            if (this.connection == null) return null;
            // touched
            await this.context.TouchConnection(this.connection);

            // Create an instance of RconClient pointing to an IP and a PORT
            var client = RconClient.Create(this.connection.Server , this.connection.Port.Value);

            await client.ConnectAsync();
            // Send a RCON packet with type AUTH and the RCON password for the target server
            var authenticated = await client.AuthenticateAsync(this.connection.Password);
            if (authenticated)
            {
                // If the response is positive, the connection is authenticated and further commands can be sent
                //var status = await client.ExecuteCommandAsync("status");
                // Some responses will be split into multiple RCON pakcets when body length exceeds the maximum allowed
                // For this reason these commands needs to be issued with isMultiPacketResponse parameter set to true
                // An example is CS:GO cvarlist
                //var cvarlist = await client.ExecuteCommandAsync("cvarlist", true);
                //var userList = await client.ExecuteCommandAsync("users");
                return client;
            }
            return null;
        }
    }
}