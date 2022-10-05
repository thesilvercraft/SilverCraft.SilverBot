# Silverbot Install Guide
Currently unfinished
## Dotnet install
https://learn.microsoft.com/en-gb/dotnet/core/install/linux OR https://learn.microsoft.com/en-gb/dotnet/core/install/windows   
You currently need to install just the .Net (core) runtime, older versions with websites need the ASP.Net (core) runtime and future versions might require it too   
## Download base silverbot
In linux / git shell
```sh
mkdir silverbot
cd silverbot
curl https://api.github.com/repos/thesilvercraft/SilverCraft.SilverBot/releases/latest | grep "/silverbot.zip"| cut -d : -f 2,3 |  tr -d \" | xargs -I foobar curl -L foobar --output silverbot.zip
unzip silverbot.zip
```
## For SystemD (if using it)
```sh
wget https://gist.githubusercontent.com/Silverdimond/9e5bddbc7286b3c634ac086ba27897c8/raw/ed366e36024d6cf3e482633fb36608e80e7c407d/silverbot.service
echo if using systemd modify silverbot.service and then run sudo mv silverbot.service /etc/systemd/system/
```
OR
create a service named `silverbot.service`, and add the following content  
```
[Unit]
Description=Silverbot
After=network.target

[Service]
WorkingDirectory= {{{{THE DIRECTORY YOU INSTALLED SILVERBOT IN}}}}
User= {{{{THE USER THAT YOU INSTALLED SILVERBOT WITH}}}}
Group=nogroup
Restart=always
ExecStart=dotnet SilverBotDS.dll

[Install]
WantedBy=multi-user.target
```
## For updates on bot restart (on linux)
cd into the directory you installed silverbot in and run
```sh
wget https://raw.githubusercontent.com/thesilvercraft/SilverBotAutoUpdater/main/autoupdater.sh
```
OR create a file named `autoupdater.sh` where you installed silverbot and add the following content or any other content that stops the bot, downloads the update (from the first paramater), extracts the update, and restarts the bot
```sh
#THIS SCRIPT IS MENT TO BE CALLED BY SILVERBOT ITSELF, PLEASE DO NOT ATTEMPT RUNNING IT YOURSELF
systemctl stop silverbot
rm -f sb.zip
wget -O sb.zip "$1"
unzip -o sb.zip
systemctl start silverbot
```
## First run
You should let the bot generate the config (in the directory it was installed in as silverbot.xml) and exit, then in the new config you should consider editing the following fields:   
- The prefixes (unless you're ok with the default ones) - Located as `<Prefix>`
- The logging settings - `<MinimumLogLevel>` and `<UseTxtFilesAsLogs>` 
- ***The DISCORD TOKEN***  
- The modules to load `<ModulesToLoad>` and `<ModulesToLoadExternal>`
- The services to load `<ServicesToLoadExternal>`
- Change the giphy token if using the giphy module `<Gtoken>`
- Change the fortniteapi token if using it `<FApiToken>`
- Change the java location if using silverbot to launch lavalink `<JavaLoc>`
- ***If not using lavalink change `<UseLavaLink>`***  
- ***If you dont want silverbot to download and launch lavalink change `<AutoDownloadAndStartLavalink>`***
- If you do optionally change `<LavalinkBuildsSourceGitHubUser>`, `<LavalinkBuildsSourceGitHubRepo>`, `<LavalinkRestUri>`, `<LavalinkWebSocketUri>`, `<LavalinkPassword>`
- ***Change the main server id `<ServerId>`***  
- Change the logging webhook `<LogWebhook>`
- Dont forget to change the `<Splashes>` to make it distinct :)
