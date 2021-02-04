const apiService = require('../../api-service');

module.exports =
{
    name: "maps",
    description: `display available server maps`,
	execute: async (config, discordConfig, service, command, args) =>
	{
        try {
            command = 'maps *';
            const response = await apiService.get(config, discordConfig, command);
            const embed = service.getMapsEmbed(response);

            service.sendMessageToChannel(embed, 120000);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};