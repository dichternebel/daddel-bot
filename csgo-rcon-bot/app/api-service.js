const util = require('util');
const exec = util.promisify(require('child_process').exec);

module.exports =
{
    get: async (config, discordConfig, command, param) => {
        let rconCommand = ` --host ${discordConfig.get('server')} --port ${discordConfig.get('port')} --password ${discordConfig.get('password')} ${command} `;
        if (param) rconCommand += param;

        try {
            const { stdout, stderr } = await exec(config.get('RCON_CLI_PATH') + rconCommand);
            return stdout;
        } catch (error) {
            return "Oops! Could not connect to server. :flushed:\n" + error.stderr;
        }
    }
};