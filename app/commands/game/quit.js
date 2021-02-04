const simpleCommands = require('../simple-commands');

module.exports =
{
	name: "quit",
	aliasses: ["-q", "exit", "reboot", "shutdown"],
	description: `restart the server`,
	execute: async (config, discordConfig, service, command, args) =>
	{
        await simpleCommands.execute(config, discordConfig, service, 'quit', args);
	}
};