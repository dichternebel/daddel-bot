module.exports =
{
	name: "invite",
    aliasses: ["-i"],
    description: "invite me to another server",
	execute: (config, discordConfig, service, command, args) =>
	{
        try {
            const embedDescription = `You may invite me to other servers using this link:\n\nhttps://discord.com/oauth2/authorize?client_id=${config.get('BOT_ID')}&permissions=93248&scope=bot`
            const embed = service.getRichEmbed("Invitation :partying_face:!", embedDescription);
            service.sendMessageEmbedToChannel(embed);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};