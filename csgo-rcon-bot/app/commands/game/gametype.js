const apiService = require('../../api-service');

module.exports =
{
	name: "game",
    aliasses: ["gametype", "mode", "type", "game_type", "game_mode"],
    description: "change game type, e.g. `game wingman`",
	execute: async (config, discordConfig, service, command, args) =>
	{
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
                service.message.react("👎").catch(err => console.log(err));
                return;
            }
            const endpoint = {
                url: `${config.get('API_URL')}/gametype`,
                authKey: config.get('API_KEY')
            };
            const response = await apiService.get(endpoint, payload);
            const messageTitle = "CS:GO Server Gametype Change Message";
            let descr = response.text;
            if(param === "dangerzone"){
                descr = descr + "\nFor joining e.g. team #1, open console during warmup and type:\n`dz_jointeam 1`\n"
            }
            const embed = service.getRichEmbed(messageTitle, descr);

            service.sendMessageToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};