const simpleCommands = require('../simple-commands');

module.exports =
{
	name: "bot_add",
	description: "add a bot to current game",
	execute: async (config, discordConfig, service, command, args) =>
	{
        await simpleCommands.execute(config, discordConfig, service, command, args);
	}
};