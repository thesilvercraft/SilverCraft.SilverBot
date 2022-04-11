## :exclamation::exclamation::exclamation:💀EOL NOTICE
SilverBot and all of its successors made by us will enter a End OF Life state soon, you may self host using the guide below but no guarantees of any kind will be made.  


# SilverBot

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/f1f2a69af2b341238cd40b2f54684095)](https://app.codacy.com/gh/thesilvercraft/SilverBot?utm_source=github.com&utm_medium=referral&utm_content=thesilvercraft/SilverBot&utm_campaign=Badge_Grade_Settings)
[![Crowdin](https://badges.crowdin.net/silverbot/localized.svg)](https://crowdin.com/project/silverbot)  
A multipurpose discord bot written in DSharpPlus using Lavalink4Net for audio.  
~~You can invite my instance [here](https://discord.com/api/oauth2/authorize?client_id=702445582559739976&permissions=1278602326&scope=bot%20applications.commands).
(<https://silverbot.cf>)~~ 
## Install guide
| :exclamation: This guide is not finished and will never be|
|-----------------------------------------------------------|

To run this you will need to install:

-   .net 6 runtime (<https://dotnet.microsoft.com/download/dotnet/6.0>)

-   A discord **bot** token (<https://discord.com/developers>)

-   A discord server you can set up roles in

-   Space for a database fine unless you compile your own version that does not use sqllite



-   Optionally:

    -   A giphy api token (<https://developers.giphy.com/>)

    -   A FortniteAPI token (<https://dash.fortnite-api.com/>)

    -   A topgg token that is a stored in the `connect.sid` cookie or an bot token

    -   A friday text and voice channel (it will attempt to connect there and play fryday every friday)

    -   A Node.JS install to run some node code
    
    -   GDI+ (<https://github.com/mono/libgdiplus> if you aren't on windows)

    -   A selinium compatible browser (chrome and firefox are implemented at the moment only and firefox is faster by a little bit)
    
    -   A discord webhook for logging

    -   A configuration for analytics and other stuff
    -   A proxy webserver if using the webserver option
    -   A way to connect to some version of lavalink

When you compile the code or download it from the releases just do:
`dotnet SilverBotDS.dll`  
After that https://github.com/thesilvercraft/SilverBot/blob/508b1306ae0edb5a26c567260595e331202ff8fa/SilverBotDS/Objects/Config.cs#L448 should kick in and you should be able to use this basic config creator dialog but further configuration will be needed  
If you would prefer using unfinished basic installer scripts and are on rasbpian like platforms check out https://gist.github.com/Silverdimond/9e5bddbc7286b3c634ac086ba27897c8  
NOTE: lavalink on arm/arm64 sucks unless you compile or use a different version that contains the libraries so do your research before choosing to download a version of lavalink  
