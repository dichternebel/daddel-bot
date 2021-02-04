const Discord = require("discord.js");
const DiscordService = require("../discord-service");
const DiscordConfig = require("../../config/discord-config");
const Conf = require("conf");

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
        const service = new DiscordService(message);
        service.sendMessageToChannel("Sorry, " + message.author.username +"...\nI'm not allowed to execute commands via DM. :disappointed_relieved:");
        return;
    }

    // remove command from args and grab it
    args = message.content.slice(config.get('PREFIX').length).trim().split(/ +/);
    const command = args.shift().toLowerCase();

    // delete the caller message to prevent spam in channels (not working in DM!)
    message.delete({ timeout : 60000}).catch(err => console.log(err));
    
    message.guild.members
    .fetch(message.author)
    .then(member => {
        // get info from sender
        const isAdmin = member.hasPermission("ADMINISTRATOR");
        const service = new DiscordService(message, isAdmin);

        // Get discord config
        let schema = DiscordConfig.schema;

        // grab the configuration for current session
        const discordConfig = new Conf({
            schema,
            configName: `${message.guild.id}/${message.channel.id}`
        });

        if (!discordConfig.get('isEnabled'))
        {
            if (!isAdmin) {
                service.sendMessageToChannel("Sorry, " + message.author.username + "!\nI am out of order in this channel. :no_mouth:");
                return;
            }
            service.configureBot(config, discordConfig);
            return;
        }

        // get permissions
        let isInRole = true;
        if (discordConfig.get('role')) {
            isInRole = member.roles.cache.some(role => role.name === discordConfig.get('role'));
        }

        // authorize
        if (!isInRole && !isAdmin) {
            service.sendMessageToChannel("Sorry, " + message.author.username + "!\nI am currently not allowed to execute your commands, because your are not in role `" + discordConfig.get('role') + "`.\nPlease contact an admin in charge. :sweat_smile:");
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
    .catch(err => console.log(err));
}