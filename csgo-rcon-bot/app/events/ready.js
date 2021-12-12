const schedule = require('node-schedule');

module.exports = function (client,config) {
    client.user.setActivity(config.get('PREFIX'), { type: 'LISTENING' });
    // set bot's id
    config.set('BOT_ID', client.user.id);
    // output current usage
    console.log("\nList of active Servers:");
    console.log("-----------------------");
    client.guilds.cache.forEach(g => {
        console.log(g.id + " - " + g.name +  "\n|--> joined: " + g.joinedAt + " [" + g.joinedTimestamp + "]");
    });

    // discord seems to lose activity after some time :-/
    // this will refresh activity every 3 hours
    schedule.scheduleJob('*/180 * * * *', function(){
        client.user.setActivity(config.get('PREFIX'), { type: 'LISTENING' });
    });
}