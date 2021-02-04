const simpleCommands = require('../simple-commands');

module.exports =
{
	name: "warmup",
	description: "`0`: end warm-up, `1`: restart warm-up",
	execute: async (config, discordConfig, service, command, args) =>
	{
		if (!args || args.length < 1) {
			service.sendMessageToChannel('Please add a parameter to be executed.');
			return;
		}
		if (args[0] === '0' || args[0] === 'false' || args[0] === 'end' || args[0] === 'stop') {
			command = 'mp_warmup_end 10';
		}
		else {
			command = 'mp_warmup_start 3';
		}
        await simpleCommands.execute(config, discordConfig, service, command, args);
	}
};