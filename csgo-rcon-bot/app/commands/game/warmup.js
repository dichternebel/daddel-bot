const simpleCommands = require('../simple-commands');

module.exports =
{
	name: "warmup",
	description: "`0`: end warm-up, `1`: restart warm-up",
	execute: async (config, discordConfig, service, command, args) =>
	{
        await simpleCommands.execute(config, discordConfig, service, command, args);
	}
};