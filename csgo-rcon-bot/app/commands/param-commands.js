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
            // prepare the payload for API call
            const payload = {
                accessToken: discordConfig.accessToken,
                salt: discordConfig.salt,
                param: param
            }
            // guard for useless API calls
            if (!payload.accessToken || !payload.salt) {
                service.message.react("👎").catch(err => console.log(err));
                return;
            }
            const endpoint = {
                url: `${config.get('API_URL')}/${command}`,
                authKey: config.get('API_KEY')
            };
            const response = await apiService.get(endpoint, payload);
            const messageTitle = "CS:GO Server '" + command + "' Change Message";
            const embed = service.getRichEmbed(messageTitle, "Trying to set `" + command + " " + param + "`...\n" + response.text);

            service.sendMessageToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};