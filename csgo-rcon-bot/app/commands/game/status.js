const fetch = require('node-fetch');
const gamedigService = require('../../gamedig-service');

module.exports =
{
    name: "status",
    aliasses: ["stats", "info"],
    description: "get status and stats from server",
	execute: async (config, discordConfig, service, command, args) =>
	{
        try {
            // get stats from gameDig
            let response = await gamedigService.queryState(discordConfig);
            response.iconUrl = `${config.get('IMG_BASEURL')}csgo_icon.png`;
            response.thumbnailUrl = `${config.get('IMG_BASEURL')}csgo_wallpaper.jpg`;

            // server offline?
            if (response.offline) {
                const embed = service.getStatusEmbed(response);
                service.sendMessageToChannel(embed);
                return;
            }
            // else try to get an image for current map

            let thumbnailUrl = '';
            // check for workshop map: workshop/1871501511/dz_junglety
            if (response.map.toLowerCase().trim().startsWith('workshop/')) {
                const mapArray = response.map.split('/');
                if (mapArray.length === 3) {
                    thumbnailUrl = `${config.get('IMG_BASEURL')}${mapArray[1]}.jpg`;
                    response.map = mapArray[2];
                }
            }
            else {
                thumbnailUrl = `${config.get('IMG_BASEURL')}${response.map}.jpg`;
            }
            // can we get that map image?
            res = await fetch(thumbnailUrl, { method: 'HEAD' });
            if (res.ok) {
                response.thumbnailUrl = thumbnailUrl;
            }
            // create and send status embed
            const embed = service.getStatusEmbed(response);
            service.sendMessageToChannel(embed);

        } catch (err) {
            service.reactWithError(err);
        }
	}
};