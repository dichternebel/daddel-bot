using System.Threading.Tasks;
using MongoDB.Driver;
using System.Security.Authentication;
using System;

namespace Rcon.Function
{
    internal class CosmosDbContext
    {
        private MongoClientSettings settings {get; set;}

        private string connectionString = System.Environment.GetEnvironmentVariable("mongodb-connection-string");
        private string database = System.Environment.GetEnvironmentVariable("database");
        private string collectionName = System.Environment.GetEnvironmentVariable("collectionName");
        private string secKey = System.Environment.GetEnvironmentVariable("keyMaster");
        
        public CosmosDbContext()
        {
            this.settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            this.settings.SslSettings =  new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        }

        public async Task<ConnectionPayload> GetConnection(RconPayload rconPayload)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);
            var currentConnection = await coll.Find(x => x.AccessToken == rconPayload.AccessToken).SingleOrDefaultAsync();
            if (currentConnection?.Password != null)
            {
                currentConnection.Password = VinzClortho.Decrypt(currentConnection.Password, secKey + rconPayload.Salt);
            }
            return currentConnection;
        }

        public async Task SetConnection(ConnectionPayload connection)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);

            var foundConnection = await coll.Find(x => x.AccessToken == connection.AccessToken).SingleOrDefaultAsync();

            if (foundConnection == null)
            {
                if (connection.Password != null)
                {
                    connection.Password = VinzClortho.Encrypt(connection.Password, secKey + connection.Salt);
                }

                connection.InsertedOn = DateTime.UtcNow;
                await coll.InsertOneAsync(connection);
                return;
            }

            var currentPwd = connection.Password == null ? foundConnection.Password : VinzClortho.Encrypt(connection.Password, secKey + connection.Salt);
            var filter = Builders<ConnectionPayload>.Filter.Eq("Id", foundConnection.Id);
            var update = Builders<ConnectionPayload>.Update
                .Set("IsEnabled", connection.IsEnabled)
                .Set("Password", currentPwd)
                .Set("Port", connection.Port)
                .Set("Role", connection.Role)
                .Set("Server", connection.Server)
                .Set("UpdatedOn", DateTime.UtcNow);

            await coll.UpdateOneAsync(filter, update);
        }

        public async Task TouchConnection(ConnectionPayload connection)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);

            var foundConnection = await coll.Find(x => x.AccessToken == connection.AccessToken).SingleOrDefaultAsync();

            if (foundConnection == null)
            {
                return;
            }

            var filter = Builders<ConnectionPayload>.Filter.Eq("Id", connection.Id);
            var update = Builders<ConnectionPayload>.Update.Set("TouchedOn", DateTime.UtcNow);
            await coll.UpdateOneAsync(filter, update);
        }

        public async Task DeleteConnection(ConnectionPayload currentConnection)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);
            await coll.DeleteOneAsync(x => x.Id == currentConnection.Id);
        }

        public async Task DeleteConnections(string accessToken)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);

            var identifier = accessToken.Split("-"); 
            await coll.DeleteManyAsync(x => x.AccessToken.StartsWith(identifier[0]+'-'));
        }
    }
}