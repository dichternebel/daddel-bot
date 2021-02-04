const paramCommands = require('../param-commands');

module.exports =
{
	name: "pistolonly",
	description: "change game mode, e.g. `pistolonly 1`",
	execute: async (config, discordConfig, service, command, args) =>
	{
        await paramCommands.execute(config, discordConfig, service, command, args);
	}
};