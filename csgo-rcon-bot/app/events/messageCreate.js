const Ajv = require("ajv").default;
const Discord = require("discord.js");
const DiscordService = require("../discord-service");
const apiService = require("../api-service");
const DiscordConfig = require("../../config/discord-config");

module.exports = function (client,config,message,newMessage) {
    // guard for bots
    if (message.system || message.author.bot) return;
    // only accept prefixed message
    if (!message.content.startsWith(config.get('PREFIX'))) return;
    // is this an updated message?
    if (newMessage) message = newMessage;
    // check prefix
    let args = message.content.trim().split(/ +/);
    if (args[0] != config.get('PREFIX')) return;

    const isDirectMessage = message.channel instanceof Discord.DMChannel;
    if (isDirectMessage) {
        const service = new DiscordService(message, false);
        service.sendMessageToChannel("Sorry, " + message.author.username +"...\nI'm not allowed to execute commands via DM. :disappointed_relieved:");
        return;
    }

    // remove command from args and grab it
    args = message.content.slice(config.get('PREFIX').length).trim().split(/ +/);
    const command = args.shift().toLowerCase();

    // delete the caller message to prevent spam in channels (not working in DM!)
    setTimeout(() => message.delete().catch(err => console.log(err)), 60000);
    
    message.guild.members
    .fetch(message.author)
    .then(member => {
        // get info from sender
        const isAdmin = member.permissions.has("ADMINISTRATOR");
        const service = new DiscordService(message, isAdmin);

        // Get discord config
        const endpoint = {
            url: `${config.get('API_URL')}/connect`,
            authKey: config.get('API_KEY')
        };
        const param = {
            accessToken: `${message.guild.id}-${message.channel.id}`,
            salt: message.channel.createdTimestamp
        }
        apiService.getJson(endpoint, param)
        .then((response) => {
            // Instantiate a config object with defaults
            const ajv = new Ajv({useDefaults: true})
            const schema = {
                type: 'object',
                properties: DiscordConfig.schema
            };
            const validate = ajv.compile(schema);
            const discordConfig = {};
            validate(discordConfig);
            // assign the repsonse to the default config
            Object.assign(discordConfig, response.body);
            // add salt
            discordConfig.salt = param.salt;
            
            if (!discordConfig.isEnabled)
            {
                if (!isAdmin) {
                    service.sendMessageToChannel("Sorry, " + message.author.username + "!\nI am out of order in this channel. :no_mouth:");
                    return;
                }
                else if (command != 'config_remove' && command != '-rm') {
                    service.configureBot(config, discordConfig);
                    return;
                }
            }

            // get permissions
            let isInRole = true;
            if (discordConfig.role) {
                isInRole = member.roles.cache.some(role => role.name === discordConfig.role);
            }

            // authorize
            if (!isInRole && !isAdmin) {
                service.sendMessageToChannel("Sorry, " + message.author.username + "!\nI am currently not allowed to execute your commands, because your are not in role `" + discordConfig.role + "`.\nPlease contact an admin in charge. :sweat_smile:");
                return;
            }
            if (!command) {
                service.sendMessageToChannel("What can I do for you, " + message.author.username + "?\nHint: You could start by typing `" + config.get('PREFIX') + " help`");
                return;
            }
            else { // let's check if we know the command or not
                let knownCommand = client.commands.get(command);
                if (!knownCommand) knownCommand = client.commands.get(client.aliasses.get(command));
                if (knownCommand) {
                    if (knownCommand === 'help' || knownCommand.name === 'help') args = Array.from(client.publicCommands);
                    knownCommand.execute(config, discordConfig, service, command, args);
                }
                else {
                    service.sendMessageToChannel("Uhmm... I didn't get that, " + message.author.username +".\nImho, you should try `" + config.get('PREFIX') + " help` :smirk:");
                }
            }
        })
        .catch(err => {
            console.log(err);
            service.reactWithError(err);
        });
    })
    .catch(err => console.log(err));
}