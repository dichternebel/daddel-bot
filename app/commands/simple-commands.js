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
            if (service.isValid(response)) {
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