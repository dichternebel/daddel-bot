const simpleCommands = require('../simple-commands');

module.exports =
{
	name: "bot_add",
	description: "add a bot to current game",
	execute: async (config, discordConfig, service, command, args) =>
	{
		command = 'bot_quota_mode normal; bot_add';
        await simpleCommands.execute(config, discordConfig, service, command, args);
	}
};