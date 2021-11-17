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
            // prepare the payload for API call
            const payload = {
                accessToken: discordConfig.accessToken,
                salt: discordConfig.salt,
                param: param
            }
            // guard for useless API calls
            if (!payload.accessToken || !payload.salt) {
                await service.message.react('👎').catch(err => console.log(err));
                return;
            }
            const endpoint = {
                url: `${config.get('API_URL')}/changelevel`,
                authKey: config.get('API_KEY')
            };
            const response = await apiService.get(endpoint, payload);
            const messageTitle = "CS:GO Server Map Change Message";
            const embed = service.getRichEmbed(messageTitle, "Trying to change map to " + param +"...\n" + response.text);

            service.sendMessageEmbedToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};