const apiService = require('../../api-service');

module.exports =
{
	name: "map",
    aliasses: ["changelevel"],
    description: "change map, e.g. `map de_mirage`",
	execute: async (config, discordConfig, service, command, args) =>
	{
        try {
            let param = "";
            if (args.length > 0) {
                param = args[0];
            }

            const response = await apiService.get(config, discordConfig, 'changelevel', param);
            const messageTitle = "CS:GO Server Map Change Message";
            const embed = service.getRichEmbed(messageTitle, "Trying to change map to " + param +"...\n" + response);

            service.sendMessageToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};