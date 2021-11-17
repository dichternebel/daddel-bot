const apiService = require('../../api-service');

module.exports =
{
    name: "maps",
    description: `display available server maps`,
	execute: async (config, discordConfig, service, command, args) =>
	{
        try {
            const payload = {
                accessToken: discordConfig.accessToken,
                salt: discordConfig.salt
            }
            // guard for useless API calls
            if (!payload.accessToken || !payload.salt) {
                service.message.react('👎').catch(err => console.log(err));
                return;
            }
            const endpoint = {
                url: `${config.get('API_URL')}/${command}`,
                authKey: config.get('API_KEY')
            };
            const response = await apiService.get(endpoint, payload);
            const embed = service.getMapsEmbed(response.text);

            service.sendMessageEmbedToChannel(embed, 120000);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};