const Discord = require("discord.js");
const escape = require('escape-markdown');
const splitLines = require('split-lines');
const apiService = require("./api-service");

class DiscordService {
    constructor(message, isAdmin) {
        this.message = message;
        this.isAdmin = isAdmin;
    }

    // gives back a MessageEmbed object based on given content
    getRichEmbed(title, description) {
        const color = this.isValid(description)? "#0099ff" : "#cc3300";
        // respect discord limit for embeds
        if (description.length > 2048) {
            description = description.substring(1,2044) + " ...";
        }
        const embed = new Discord.MessageEmbed()
            .setTitle(title)
            .setColor(color)
            .setDescription(description);

        return embed;
    }

    // creates and gives back a fancy status embed
    getStatusEmbed(state) {
        let embedColor = state.offline? "#cc3300" : "#99cc33";
        // rcon checks if server is up to date
        if (state.tags.includes('invalid')) {
            embedColor = "#ffcc00";
        }
        // create the message
        let embed = new Discord.MessageEmbed()
            .setColor(embedColor)
            .setTitle(state.name)
            .setDescription(state.connect)
            .setAuthor('CS:GO Server Stats',state.iconUrl)
            .setThumbnail(state.thumbnailUrl)
            .addFields(
                { name: 'Current Map', value: state.map, inline: true },
                { name: 'Players', value: `${state.numplayers}(${state.numbots}) / ${state.maxplayers}`, inline: true },
                { name: 'Ping', value: state.ping, inline: true },
                { name: 'Access', value: (state.password)? ':lock:' : ':unlock:', inline: true},
                { name: 'Server Version', value: state.version, inline: true},
                { name: 'Tags', value: state.tags, inline: true},
            )
            .setTimestamp()
            .setFooter('EOM');
        
        return embed;
    }

    // creates and gives back a fancy maps embed
    getMapsEmbed(responseText) {
        const title = "CS:GO Server Maps";
        if (!this.isValid(responseText)) {
            return this.getRichEmbed(title, responseText);
        }

        // create an array from response
        let responseArray = splitLines(responseText);
        responseArray.shift();
        responseArray.pop();
        
        // get those mapnames only
        let mapArray = [];
        responseArray.forEach(element => {
            const elements = element.trim().split(/ +/);
            if (elements.length > 2) mapArray.push(elements[2].slice(0,-4));
        });

        // now group those things into known mapg types
        // https://counterstrike.fandom.com/wiki/Map
        /*
        Bomb Defusal (de)
        Hostage Rescue (cs)
        Arsenal: Arms Race (ar)
        Arsenal: Demolition (ar)
        Assassination (as)
        Escape (es)
        Danger Zone (dz)
         */
        const deMaps = mapArray.filter((x) => { return x.includes('de_'); });
        const csMaps = mapArray.filter((x) => { return x.includes('cs_'); });
        const arMaps = mapArray.filter((x) => { return x.includes('ar_'); });
        const dzMaps = mapArray.filter((x) => { return x.includes('dz_'); });
        const otherMaps = mapArray.filter((x) => { return !(x.includes('de_')) && !(x.includes('cs_')) && !(x.includes('ar_')) && !(x.includes('dz_')); });

        // create the message
        let embed = new Discord.MessageEmbed()
            .setColor( "#0099ff")
            .setTitle(title)
            .addFields(
                { name: 'Bomb Defusal', value: "• `" + deMaps.sort().join("`\n• `") + "`" },
                { name: 'Hostage Rescue', value: "• `" + csMaps.sort().join("`\n• `") + "`" },
                { name: 'Arsenal (Arms Race/Demolition)', value: "• `" + arMaps.sort().join("`\n• `") + "`" },
                { name: 'Danger Zone', value: "• `" + dzMaps.sort().join("`\n• `") + "`" },
                { name: 'Other', value: "• `" + otherMaps.sort().join("`\n• `") + "`" }
            )
            .setTimestamp()
            .setFooter('EOM');
        
        return embed;
    }

    // check response for being valid (Crap! I hate myself! Needs refactoring...)
    isValid(responseMessage) {
        return !responseMessage.toLowerCase().includes("oops!")
            && !responseMessage.toLowerCase().includes('not found') && !responseMessage.toLowerCase().includes('invalid') && !responseMessage.toLowerCase().includes('failed');
    }

    // react when something wents wrong
    reactWithError(error) {
        this.message.react("🛑");
        this.sendMessageToChannel("⚠️**rrgghhh!**\nSomething went terribly wrong...\n`" + escape(error.message) + "`");
    }

    // send a self-destructing response to injected context
    sendMessageToContext(messageContext, description, ttl) {
        if (ttl === undefined) ttl = 60000; // 1min.
        
        messageContext.send(description)
            .then(message => message.delete({ timeout: ttl }))
            .catch(err => console.log(err));
    }

    // sends a self-destructing response to current channel
    sendMessageToChannel(description, ttl) {
        this.sendMessageToContext(this.message.channel, description, ttl);
    }

    // collecting configuration entries
    configureBot(config, discordConfiguration) {
        let ttl = 90000;

        // gets and sets the ACCESS_TOKEN and SALT
        if (!discordConfiguration.accessToken) {
            discordConfiguration.accessToken = `${this.message.channel.guild.id}-${this.message.channel.id}`;
            discordConfiguration.salt = this.message.channel.createdTimestamp;
        }

        let currentServer = discordConfiguration.server? discordConfiguration.server : 'empty';
        let currentPort = discordConfiguration.port;
        let currentRole = discordConfiguration.role? discordConfiguration.role : 'none';

        // prepare the payload for API call
        const endpoint = {
            url: `${config.get('API_URL')}/connect`,
            authKey: config.get('API_KEY')
        };
        
        let messageContext;
        let initialMessage;

        this.message.author.send(
            "**Moin Admin!**\nLet's have some configuration fun together! :sweat_smile:\nHINT: You can call this dialog using `" + config.get('PREFIX') + " config` anytime again.\n\nDo you want to enable the bot for this channel? [Y/N]"
        )
        .then((msg) => {
            initialMessage = msg;
            messageContext = initialMessage.channel;
        })
        .then(() => {
            let filter = (msg) => !msg.author.bot;
            let options = {
                max: 1,
                time: ttl
            };
            return messageContext.awaitMessages(filter, options);
        })
        .then((collected) => {
            if (!(collected.array().length)) {
                throw ('Timed out.');
            }
            if (collected.array()[0].content.toLowerCase() != 'n') {
                discordConfiguration.isEnabled = true;
            }
            else {
                discordConfiguration.isEnabled = false;
                apiService.postJson(endpoint, discordConfiguration);
                throw ('Disabled. :no_mouth:');
            }
            this.sendMessageToContext(
                messageContext,
                "Do you want to change the CS:GO server address? [N = NO]\n*currently ` " + currentServer +" `*",
                ttl
            );
        })
        .then(() => {
            let filter = (msg) => !msg.author.bot;
            let options = {
              max: 1,
              time: ttl
            };
            return messageContext.awaitMessages(filter, options);
        })
        .then((collected) => {
            if (!(collected.array().length)) {
                throw ('Timed out.');
            }
            if (collected.array()[0].content.toLowerCase() != 'n') {
                discordConfiguration.server = collected.array()[0].content.trim();
            }
            this.sendMessageToContext(
                messageContext,
                "Do you want to change the CS:GO server port? [N = NO]\n*currently ` " + currentPort + " `*",
                ttl
            );
        })
        .then(() => {
            let filter = (msg) => !msg.author.bot;
            let options = {
              max: 1,
              time: ttl
            };
            return messageContext.awaitMessages(filter, options);
        })
        .then((collected) => {
            if (!(collected.array().length)) {
                throw ('Timed out.');
            }

            let content = collected.array()[0].content.trim().toLowerCase();
            // entered a number?
            if (content === 'n' || !isNaN(content)) {
                if (content != 'n') discordConfiguration.port = content;
                return this.sendMessageToContext(
                    messageContext,
                    "Do you want to change the CS:GO server RCON password? [N = NO]",
                    ttl
                );
            }
            else {
                throw (':bat: WATMAN! NaN... Start over again and enter a number, please!');
            }
        })
        .then(() => {
            let filter = (msg) => !msg.author.bot;
            let options = {
              max: 1,
              time: ttl
            };
            return messageContext.awaitMessages(filter, options);
        })
        .then((collected) => {
            if (!(collected.array().length)) {
                throw ('Timed out.');
            }
            if (collected.array()[0].content.toLowerCase() != 'n') {
                discordConfiguration.password = collected.array()[0].content.trim();
            }
            this.sendMessageToContext(
                messageContext,
                "**OPTIONAL**:\nEnter a custom *ROLE* name to limit user communication with the bot. Admins are always allowed. [N = NONE]\n*currently ` " + currentRole + " `*",
                ttl
            );
        })
        .then(() => {
            let filter = (msg) => !msg.author.bot;
            let options = {
              max: 1,
              time: ttl
            };
            return messageContext.awaitMessages(filter, options);
        })
        .then((collected) => {
            if (!(collected.array().length)) {
                throw ('Timed out.');
            }
            if (collected.array()[0].content.toLowerCase() === 'n') {
                discordConfiguration.role = null;
            }
            else {
                discordConfiguration.role = collected.array()[0].content;
            }
            apiService.postJson(endpoint, discordConfiguration)
            .then(() => {
                this.sendMessageToContext(
                    messageContext,
                    "You may safely delete your answers now. Done! :thumbsup:",
                    ttl
                )
            })
            .catch( err => {
                console.log(err);
                this.reactWithError(err);
            });
        })
        .catch( err => {
            this.sendMessageToContext(messageContext, err, ttl);
        })
        .finally(() => {
            initialMessage.delete({ timeout: 30000 }).catch(err => console.log(err));
        });
    };
}

module.exports = DiscordService;