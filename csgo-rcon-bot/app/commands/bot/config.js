module.exports =
{
    name: "config",
    aliasses: ["-c", "edit", "setup"],
    description: "*admin only!* configure me per channel",
	execute: (config, discordConfig, service, command, args) =>
	{
        if (!service.isAdmin) {
            service.sendMessageToChannel("Sorry, " + message.author.username +".\n `" + config.get('PREFIX') + " " + command +"` is only allowed for those cool admins. :sunglasses:");
            return;
        }
        try {
            service.configureBot(config, discordConfig);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};