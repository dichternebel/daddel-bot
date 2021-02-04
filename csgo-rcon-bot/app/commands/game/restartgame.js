const simpleCommands = require('../simple-commands');

module.exports =
{
	name: "restartgame",
	description: `restart match`,
	execute: async (config, discordConfig, service, command, args) =>
	{
        await simpleCommands.execute(config, discordConfig, service, command, args);
	}
};