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
            const payload = {
                accessToken: discordConfig.accessToken,
                salt: discordConfig.salt,
                param: param
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
            const messageTitle = "CS:GO Server RCON";
            const embed = service.getRichEmbed(messageTitle, response.text);

            service.sendMessageEmbedToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};