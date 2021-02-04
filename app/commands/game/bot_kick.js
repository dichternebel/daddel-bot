const simpleCommands = require('../simple-commands');

module.exports =
{
	name: "bot_kick",
	description: "kick all bots",
	execute: async (config, discordConfig, service, command, args) =>
	{
		command = 'bot_quota 0; bot_kick';
        await simpleCommands.execute(config, discordConfig, service, command, args);
	}
};