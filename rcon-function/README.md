# rcon-function  [![Licensed under the MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/dichternebel/csgo-rcon-bot/blob/main/LICENSE.md) [![PR's Welcome](https://img.shields.io/badge/PRs%20-welcome-brightgreen.svg)](http://makeapullrequest.com) ![Azure Function App CI/CD](https://github.com/dichternebel/daddel-bot/workflows/Azure%20Function%20App%20CI/CD/badge.svg?branch=main)
This Azure Function API can execute RCON commands on CS:GO/SRCDS servers using [rconsharp](https://github.com/stefanodriussi/rconsharp).

## Getting started

This is a serverless API that runs on Azure Functions.  
It's written entirely in C# using .Net Core 3.1 and uses a Cosmos DB for storing configuration doscuments. Credentials are stored using the TripleDESCryptoServiceProvider with a combination of a strong master key and a Discord snowflake coming from the current discord user.

These are the targets I am running the backend on:  
* Azure Cosmos DB instance for [free](https://azure.microsoft.com/en-us/services/cosmos-db/)
* Azure Function instance for [free](https://azure.microsoft.com/en-us/services/functions/)

### Prerequisites

In order to build and run the API you need to download and install following in advance

* [MongoDB](https://www.mongodb.com/try/download/community) or alternatively use a Cosmos DB on Azure
* [VS Code](https://code.visualstudio.com/download) or alternatively you might want to try *GitHub Codespaces*.
* `git clone` the whole repo or [download zip](https://github.com/dichternebel/daddel-bot/archive/main.zip) package (discord bot included)
* In order to clone the API only you can try these commands:\
  `git init rcon-function\
  cd rcon-function\
  git remote add origin https://github.com/dichternebel/daddel-bot.git\
  git config core.sparsecheckout true\
  echo "rcon-function/*" >> .git/info/sparse-checkout\
  git pull --depth=1 origin main`\

### Build

* Open the folder in VS Code and let it install extensions for at least `C#` and `Azure Functions` and you are good to go.
* `CTRL + SHIFT + B`

### Configuration

* for local development rename the `/local.settings.json-example` file to `/local.settings.json` and configure your local settings.
* for deployment make sure you push/sync your settings with the app settings of your Azure Function or use Azure Portal.

### Customization

Adding new endpoints or changing existing can be done by simply adding/changing files in/to the `/Endpoints/` folder. Copy one file as a template to get started.

### Run

`Hit F5`

### CI/CD

I'm using GitHub Actions to build and deploy the API on every push or PR to the main branch. If your target platform is also an Azure Function App then you might just grab the deployment script from the `/.github/workflows` folder and change it to match your own function.

### API Defintion

This project generates a Swagger API Enpoint that you can browse here:\
https://rcon.azurewebsites.net/api/swagger/ui
