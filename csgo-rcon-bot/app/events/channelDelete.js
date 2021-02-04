const apiService = require("../api-service");

module.exports = function (client,config,channel) {
    if (channel.type != 'text') return;
    console.log(channel.name + " was deleted! Trying to clean up config...");
    //Remove configuration for deleted channel
    const endpoint = {
        url: `${config.get('API_URL')}/connect`,
        authKey: config.get('API_KEY')
    };
    const payload = {
        accessToken:  `${channel.guild.id}-${channel.id}`,
        salt: channel.createdTimestamp
    }

    apiService.delete(endpoint, payload)
    .then(response => console.log(response.text))
    .catch(error => console.log("> " + error.message));    
}