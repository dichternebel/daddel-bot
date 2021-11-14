const Conf = require("conf");
const AppConfig = require("./config/app-config");
const {Client, Intents} = require("discord.js");
const DiscordContext = require("./app/discord-context");

// Get app config
const schema = AppConfig.schema;
// Always start app clean and easy
const config = new Conf({schema});
config.clear();

// Get .env for local development
if (process.env.NODE_ENV != 'production') {
    require('dotenv').config();
}
// Configure app
config.set('BOT_TOKEN', process.env.BOT_TOKEN);
config.set('IMG_BASEURL', process.env.IMG_BASEURL);
config.set('API_KEY', process.env.API_KEY);
config.set('API_URL', process.env.API_URL);

// Start that thing and inject client and config
const client = new Client({ disableMentions: 'everyone', intents: [Intents.FLAGS.GUILDS, Intents.FLAGS.GUILD_MESSAGES, Intents.FLAGS.DIRECT_MESSAGES] });
new DiscordContext(client, config);
console.log('All engines running...');