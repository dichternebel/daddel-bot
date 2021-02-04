const apiService = require('../api-service');

module.exports =
{
	execute: async (config, discordConfig, service, command, args) =>
	{
        try {
            let param = "";
            if (args.length > 0) {
                param = args[0];
            }

            const response = await apiService.get(config, discordConfig, command, param);
            const messageTitle = "CS:GO Server '" + command + "' Change Message";
            const embed = service.getRichEmbed(messageTitle, "Trying to set `" + command + " " + param + "`...\n" + response);

            service.sendMessageToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};