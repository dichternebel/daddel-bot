const paramCommands = require('../param-commands');

module.exports =
{
	name: "headshotonly",
	description: "change game mode, e.g. `headshotonly 1`",
	execute: async (config, discordConfig, service, command, args) =>
	{
        await paramCommands.execute(config, discordConfig, service, command, args);
	}
};