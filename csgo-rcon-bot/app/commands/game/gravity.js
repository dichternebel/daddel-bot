const paramCommands = require('../param-commands');

module.exports =
{
	name: "gravity",
	description: "change gravity, e.g. `gravity moon`",
	execute: async (config, discordConfig, service, command, args) =>
	{
        await paramCommands.execute(config, discordConfig, service, command, args);
	}
};