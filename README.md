# csgo-rcon-bot [![Licensed under the MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/dichternebel/csgo-rcon-bot/blob/main/LICENSE.md)[![PR's Welcome](https://img.shields.io/badge/PRs%20-welcome-brightgreen.svg)](http://makeapullrequest.com)
This discord bot can execute RCON commands on CS:GO/SRCDS servers.

> This discord bot was highly inspired by the [srcds-discord-bot](https://github.com/Retr0-01/srcds-discord-bot) by [Retr0-01](https://github.com/Retr0-01)

## Getting started

The purpose of this bot is to be run side-by-side with your CS:GO server. Although it can be run anywhere I highly recommend not to use it in a public manner. There is only minimal security enabled and configuration files are stored locally in plain text.

> I am currently working on a more complex, more secure 3-tier version of this bot to be published soon. Feel free to [invite that bot](https://discord.com/oauth2/authorize?client_id=797866820996169779&permissions=10304&scope=bot) to your server. 

### Prerequisites

* Get a discord bot for your account at https://discord.com/developers/applications

In order to build and run the bot you need to download and install following in advance

* [Node.js](https://nodejs.dev/) >= v12.x (developed with v15.7.0)
* [rcon-cli](https://github.com/itzg/rcon-cli) - matching your target platform
* `git clone` or [download](https://github.com/dichternebel/csgo-rcon-bot/archive/main.zip) bot as zip package

### Build

* unzip and run `npm i`

### Configuration

* for local development open the `/.env` file and configure your local settings
* for deployment edit either the `/config/csgo-rcon-bot.service` file or the `/config/start.bat`

### Customization

Adding new commands or changing existing can be done by simply adding/changing command files to the `/app/commands/game/` folder. You just have to respect some methods. Copy one file as a template to get started.

### Run

`node ./index.js`

### Installation

* On Linux Ubuntu
    * copy the `.service` file to /etc/systemd/system 
    * `sudo systemctl enable csgo-rcon-bot.service`
    * `sudo systemctl start csgo-rcon-bot.service`

* On Windows
    * download the [NSSM](https://nssm.cc/download) and unzip
    * `nssm install csgo-rcon-bot`
    * Path to where the `start.bat` is located
