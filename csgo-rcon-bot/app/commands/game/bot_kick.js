const simpleCommands = require('../simple-commands');

module.exports =
{
	name: "bot_kick",
	description: "kick all bots",
	execute: async (config, discordConfig, service, command, args) =>
	{
        await simpleCommands.execute(config, discordConfig, service, command, args);
	}
};