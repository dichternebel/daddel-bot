const Conf = require("conf");
const AppConfig = require("./config/app-config");
const Discord = require("discord.js");
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
config.set('RCON_CLI_PATH', process.env.RCON_CLI_PATH);

// Start that thing and inject client and config
const client = new Discord.Client({ disableMentions: 'everyone' });
new DiscordContext(client, config);
console.log('All engines running...');