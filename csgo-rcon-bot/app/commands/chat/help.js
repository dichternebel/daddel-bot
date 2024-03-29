module.exports =
{
	name: "help",
    aliasses: ["man", "-h", "--help", "how", "howto", "hilfe", "hä", "hä?", "wie", "aid", "подде́ржка"],
	execute: (config, discordConfig, service, command, args) =>
	{
        try {
            const list = "• **" + config.get('PREFIX') + " " + args.sort().map(x => x.join("** - ")).join("\n• **" + config.get('PREFIX') + " ");

            const embedDescription = "I can speak RCON to your CS:GO server. I am able to execute following commands on your behalf:\n\n"
                + list
                + "\n\nPlease run `" + config.get('PREFIX') + " status` and check for players before changing things. **Please be sure not to ruin a running match!\n\nGot it?** :face_with_monocle:";
            const embed = service.getRichEmbed("Me is helping out :woozy_face:!", embedDescription);
            service.sendMessageEmbedToChannel(embed, 120000);
        } catch (err) {
            service.reactWithError(err);
        }
	}
};