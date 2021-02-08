# csgo-rcon-bot [![Licensed under the MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/dichternebel/csgo-rcon-bot/blob/main/LICENSE.md) [![PR's Welcome](https://img.shields.io/badge/PRs%20-welcome-brightgreen.svg)](http://makeapullrequest.com) ![CI/CD](https://github.com/dichternebel/daddel-bot/workflows/Node.js%20CI/CD/badge.svg?branch=main)
This discord bot can execute RCON commands on CS:GO/SRCDS servers using an API backend.

## Getting started

This discord bot does communication with the CS:GO server done by using simple HTTP requests to an API that handles authentication, authorization and execution of the commands.

Since this is plain and pure JavaScript (CommonJS) it could be run on anything e.g. Linux, Windows, ARM and Mac. There is no dependency to any platform specific library whatsoever. If your machine runs NodeJs this bot will work.

### Prerequisites

* Get a discord bot for your account at https://discord.com/developers/applications

In order to build and run the bot you need to download and install following in advance

* [Node.js](https://nodejs.dev/) >= v12.x (developed with v15.7.0)
* `git clone` the whole repo or [download zip](https://github.com/dichternebel/daddel-bot/archive/main.zip) package (backend included)
* In order to clone the bot only you can use these commands:
```shell
git init csgo-rcon-bot
cd csgo-rcon-bot
git remote add origin https://github.com/dichternebel/daddel-bot.git
git config core.sparsecheckout true
echo "csgo-rcon-bot/*" >> .git/info/sparse-checkout
git pull --depth=1 origin main
```

### Build

* unzip and run `npm i`

### Configuration

* for local development rename the `/.env-example` file to `/.env` and configure your local settings.
* for deployment rename and edit either the `/server/daddelbot.service-example`  file to `/server/daddelbot.service` or edit the `/server/start.bat` file.

### Customization

Adding new commands or changing existing can be done by simply adding/changing command files to/in the `/app/commands/game/` folder. You just have to respect some methods. Copy one file as a template to get started.

### Run

`node ./index.js`

### Installation

* On Linux Ubuntu
    * copy the edited `/server/daddelbot.service` file to /etc/systemd/system 
    * `sudo systemctl enable daddelbot.service`
    * `sudo systemctl start daddelbot.service`

* On Windows
    * download the [NSSM](https://nssm.cc/download) and unzip
    * `nssm install daddel-bot`
    * Path to where the edited `start.bat` is located

### CI/CD

I'm using GitHub Actions to build and deploy the bot on every push or PR to the main branch. If your target platform is Linux then you might just grab the deployment script from the `/.github/workflows` folder and change it to match your own target server.
