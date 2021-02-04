const apiService = require('../../api-service');

module.exports =
{
    name: "rcon",
    description: "*admin only!* use like `rcon banid 1440 someUserId`",
	execute: async (config, discordConfig, service, command, args) =>
	{
        if (!service.isAdmin) {
            service.sendMessageToChannel("Sorry, " + message.author.username +".\n `" + config.get('PREFIX') + " rcon` is only allowed for those cool admins. :sunglasses:");
            return;
        }
        try {
            let param = "";
            if (args.length > 0) {
                param = args.join(' ');
            }

            const response = await apiService.get(config, discordConfig, command, param);
            const messageTitle = "CS:GO Server RCON";
            const embed = service.getRichEmbed(messageTitle, response);

            service.sendMessageToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};