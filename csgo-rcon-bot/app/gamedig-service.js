const GameDig = require('gamedig');

module.exports =
{
	queryState: async (discordConfig) =>
	{
        let state;

        try {
            state = await GameDig.query({
              type: 'csgo',
              host: discordConfig.get('server'),
              port: discordConfig.get('port')
            });
            state.offline = false;
            state.connect = `<steam://connect/${state.connect}>`;
            state.numplayers = state.raw.numplayers ? state.raw.numplayers : 0;
            state.numbots = state.raw.numbots ? state.raw.numbots : 0;
            state.version = state.raw.version;
            state.tags = state.raw.tags;
        } catch(e) {
            state = {
              name: 'The server seems **OFFLINE !** ',
              map: '*none*',
              password: false,
              numplayers: 0,
              maxplayers: 0,
              numbots: 0,
              ping: 999,
              offline: true,
              connect: 'n.a.',
              password: false,
              version: '00,OK,00,00',
              tags: 'empty, unknown',
            };
          }
        
        return state;        
    }
};