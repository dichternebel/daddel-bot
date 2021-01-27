using System.Threading.Tasks;
using MongoDB.Driver;
using System.Security.Authentication;
using System;

namespace Rcon.Function
{
    public class CosmosDbContext
    {
        private MongoClientSettings settings {get; set;}

        private string connectionString = System.Environment.GetEnvironmentVariable("mongodb-connection-string");
        private string database = System.Environment.GetEnvironmentVariable("database");
        private string collectionName = System.Environment.GetEnvironmentVariable("collectionName");
        
        public CosmosDbContext()
        {
            var connectionString = System.Environment.GetEnvironmentVariable("mongodb-connection-string");
            this.settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            this.settings.SslSettings =  new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        }

        public async Task<ConnectionPayload> GetConnection(string accessToken)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);
            return await coll.Find(x => x.AccessToken == accessToken).SingleOrDefaultAsync();
        }

        public async Task SetConnection(ConnectionPayload connection)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);

            var foundConnection = await coll.Find(x => x.AccessToken == connection.AccessToken).SingleOrDefaultAsync();

            if (foundConnection == null)
            {
                connection.InsertedOn = DateTime.UtcNow;
                await coll.InsertOneAsync(connection);
                return;
            }

            var currentPwd = connection.Password == null ? foundConnection.Password : connection.Password;
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

        // ToDo: unused & untested
        public async Task DeleteConnection(ConnectionPayload currentConnection)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);
            await coll.DeleteOneAsync(x => x.Id == currentConnection.Id);
        }

        // ToDo: unused & untested
        public async Task DeleteConnections(string accessToken)
        {
            var client = new MongoClient(this.settings);
            var db = client.GetDatabase(database);
            var coll = db.GetCollection<ConnectionPayload>(collectionName);

            var identifier = accessToken.Split("-"); 
            await coll.DeleteManyAsync(x => x.AccessToken.StartsWith(identifier[0]));
        }
    }
}