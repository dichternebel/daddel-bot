const Discord = require("discord.js");
const fs = require("fs");

class DiscordContext {
    constructor(client, config) {
        this.config = config;
        this.client = client
        this.instantiateCommands();
        this.instatiateEvents();
        this.client.login(this.config.get('BOT_TOKEN'));
    }

    instantiateCommands() {
        this.client.commands = new Discord.Collection();
        this.client.aliasses = new Discord.Collection();
        this.client.publicCommands = new Discord.Collection();

        fs.readdirSync("./app/commands/").filter(file => !(file.endsWith(".js"))).forEach(dir => {
            const commandFiles = fs.readdirSync(`./app/commands/${dir}/`).filter(file => file.endsWith(".js"));
            for (let commandFile of commandFiles) {
                // pull commands and add them to the collection
                let pull = require(`./commands/${dir}/${commandFile}`);
                if (pull.name) {
                    this.client.commands.set(pull.name, pull);
                    if (dir != 'chat') {
                        this.client.publicCommands.set(pull.name, pull.description);
                    }
                } else continue;
                // get aliasses
                if (pull.aliasses && Array.isArray(pull.aliasses)) pull.aliasses.forEach(alias => this.client.aliasses.set(alias, pull.name));
            }
        });
    }

    // https://github.com/AnIdiotsGuide/discordjs-bot-guide/blob/master/understanding/events-and-handlers.md
    // https://discord.js.org/#/docs/main/stable/class/Client
    instatiateEvents() {
        fs.readdir("./app/events/", (err, files) => {
            if (err) return console.error(err);
            files.forEach(file => {
                const event = require(`./events/${file}`);
                const eventName = file.split(".")[0];
                this.client.on(eventName, event.bind(null, this.client, this.config));
            });
        });
    }
}
module.exports = DiscordContext;