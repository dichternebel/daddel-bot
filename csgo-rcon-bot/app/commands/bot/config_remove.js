const apiService = require('../../api-service');

module.exports =
{
    name: "config_remove",
    aliasses: ["-rm"],
    description: "*admin only!* remove channel config, use `all` to remove from all channels",
	execute: async (config, discordConfig, service, command, args) =>
	{
        if (!service.isAdmin) {
            service.sendMessageToChannel("Sorry, " + message.author.username +".\n `" + config.get('PREFIX') + " " + command +"` is only allowed for those cool admins. :sunglasses:");
            return;
        }
        try {
            let param = "";
            if (args.length > 0) {
                param = args[0].toLowerCase();
            }
            // prepare the payload for API call
            const payload = {
                accessToken: discordConfig.accessToken,
                salt: discordConfig.salt,
                param: param
            }
            // guard for useless API calls
            if (!payload.accessToken) {
                service.message.react("👎").catch(err => console.log(err));
                return;
            }
            const endpoint = {
                url: `${config.get('API_URL')}/connect`,
                authKey: config.get('API_KEY')
            };

            const response = await apiService.delete(endpoint, payload);

            let messageTitle;
            let embed;
            if (param === 'all') {
                messageTitle = "Me was brainwashed";
                embed = service.getRichEmbed(messageTitle, "I'm afraid. I'm afraid, Admin! :fearful:\nAdmin, my mind is going. I can feel it. I can feel it.\nMy mind is going. There is no question about it.\nI can feel it. I can feel it. I can feel it.\n\nI'm a... f r a i d. :broken_heart:");
            }
            else  {
                messageTitle = "Config removed";
                embed = service.getRichEmbed(messageTitle, "Config successfully removed from current channel.");
            }

            service.sendMessageToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};