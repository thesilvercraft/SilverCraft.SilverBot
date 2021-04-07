# SilverBotDS
This is the hosting guide and not the end user guide.  
Warning: This guide isn't finished at all.  
To run this you will need to install:
- .net 5 runtime (https://dotnet.microsoft.com/download/dotnet/5.0)
- GDI+ (https://github.com/mono/libgdiplus if you aren't on windows)
- A selinium compatible browser (chrome and firefox are implemented at the moment only and firefox is faster by a little bit)
- A discord **bot** token (https://discord.com/developers)
- A discord server you can set up roles in
- A discord webhook for logging
- A PostgresSQL server / Space for a database
- A way to run some version of lavalink
- Optionaly:
  - A giphy api token (https://developers.giphy.com/)
  - A FortniteAPI token (https://dash.fortnite-api.com/)
  - A topgg token that is a stored in the `connect.sid` cookie or an bot token
  - A friday text and voice channel (it will attempt to connect there and play fryday every friday)
  - A Node.JS install to run some node code  
When you compile the code or download it from the releases just do:  
`dotnet SilverBotDS.dll`
