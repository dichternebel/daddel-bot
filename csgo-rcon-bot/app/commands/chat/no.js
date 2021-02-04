module.exports =
{
	name: "no",
	aliasses: ["nö", "nein", "ne", "nope", "nee", "non", "nada", "niente", "нет"],
	execute: (config, discordConfig, service, command, args) =>
	{
		service.sendMessageToChannel("I don't give a fuck... :robot:");
	}
};