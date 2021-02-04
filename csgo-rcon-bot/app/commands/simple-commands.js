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
            if (service.isValid(response.text)) {
                service.message.react("👍").catch(err => console.log(err));
            }
            else {
                service.message.react("🛑").catch(err => console.log(err));
            }
        } catch (err) {
            service.reactWithError(err);
        }
	}
};