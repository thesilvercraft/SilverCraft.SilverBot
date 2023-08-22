<!-- markdownlint-capture -->
<!-- markdownlint-disable -->

# Commands and geeky info

This file is dynamically maintained by a bot, ~~a silverbot~~.

## /github/workspace/SilverBot.Web/SilverBot.Web.csproj

## /github/workspace/SilverBot.Shared/SilverBot.Shared.csproj

## /github/workspace/SilverBotDS/SilverBotDS.csproj

### AdminCommands

#### "setprefix"

Unknown description

##### Arguments

`"setprefix" <cake>`

- cake - No description (string[])

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/AdminCommands.cs#L43-L53

#### "emojipoll"

"Start a simple emoji poll for a simple yes/no question"

##### Arguments

`"emojipoll" <duration> [question...]`

- duration - "How long should the poll last. (e.g. 1m = 1 minute)" (TimeSpan)
- question - "Poll question" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/AdminCommands.cs#L63-L100

#### "GiveAway"

"Start a simple giveaway"

##### Arguments

`"GiveAway" <duration> [item...]`

- duration - "How long should the giveaway last. (e.g. 1m = 1 minute)" (TimeSpan)
- item - "Giveaway content" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/AdminCommands.cs#L102-L135

#### "exportemotestoguilded"

"Exports your guild's emotes into a \"Emote Pack\" Guilded can read"

##### Arguments

`"exportemotestoguilded"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/AdminCommands.cs#L137-L164

#### "exportemotes"

Unknown description

##### Arguments

`"exportemotes"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/AdminCommands.cs#L166-L207

### Audio

#### "playnext"

Unknown description

`"pn"`

##### Arguments

`"playnext" [song...]`

- song - No description (SongORSongs)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L27-L32

#### "play"

"Tell me to play a song"

`"p"`

##### Arguments

`"play"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L35-L41

##### Arguments

`"play" [song...]`

- song - No description (SongORSongs)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L50-L54

#### "musiccontroller"

"Controls music playback"

##### Arguments

`"musiccontroller"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L43-L48

#### "volume"

"Change the volume 1-100%"

`"vol"`

##### Arguments

`"volume" <volume>`

- volume - No description (ushort)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L56-L63

#### "seek"

"Seeks to the specified time"

##### Arguments

`"seek" <time>`

- time - No description (TimeSpan)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L65-L71

#### "clearqueue"

"Clears the queue"

##### Arguments

`"clearqueue"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L74-L80

#### "shuffle"

"Shuffles the queue"

##### Arguments

`"shuffle"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L82-L88

#### "export"

"Export the queue"

##### Arguments

`"export" [playlistName]`

- playlistName - No description (string?)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L90-L95

#### "remove"

"Remove song X from the queue. 0 < index > queue length"

##### Arguments

`"remove" <songindex>`

- songindex - No description (int)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L97-L102

#### "queuehistory"

"check what was playing"

`"qh"`,`"prevplaying"`,`"pq"`

##### Arguments

`"queuehistory"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L104-L110

#### "queue"

"check whats playing rn and whats next"

`"np"`,`"nowplaying"`,`"q"`

##### Arguments

`"queue"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L112-L118

#### "loop"

"Loops nothing/the queue/the currently playing song"

`"repeat"`

##### Arguments

`"loop" <settings>`

- settings - "Has to be none, song or queue" (LoopSettings)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L120-L126

#### "pause"

"pause the current song"

##### Arguments

`"pause"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L129-L134

#### "ovh"

"get the lyrics from ovh"

##### Arguments

`"ovh" <name> <artist>`

- name - No description (string)
- artist - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L136-L141

#### "songaliases"

"Get the hardcoded aliases in silverbots code"

##### Arguments

`"songaliases"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L143-L148

#### "resume"

"resume the current song"

##### Arguments

`"resume"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L150-L155

#### "join"

"to join the voice chat you're in"

##### Arguments

`"join"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L157-L162

#### "forceskip"

"skip a song. dj only command"

`"fs"`

##### Arguments

`"forceskip"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L165-L172

#### "voteskip"

"skip a song"

`"skip"`

##### Arguments

`"voteskip"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L174-L180

#### "forcedisconnect"

"Tell me to leave your channel of the voice type, without checking if its in a vc"

`"fuckoffisntworking"`

##### Arguments

`"forcedisconnect"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L182-L189

#### "disconnect"

"Tell me to leave your channel of the voice type"

`"fuckoff"`,`"minecraftbedrockisbetter"`,`"fockoff"`,`"leave"`

##### Arguments

`"disconnect"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Audio.cs#L191-L198

### BingCommandModule

#### "addBingChannel"

Unknown description

##### Arguments

`"addBingChannel" <bingchannel>`

- bingchannel - No description (DiscordChannel)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/BingCommandModule.cs#L30-L55

### Bubot

#### "silveryeet"

"Sends SilverYeet.gif"

##### Arguments

`"silveryeet"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/bubot.cs#L36-L44

### BibiCommands

#### "bibi"

"Makes a image with the great cat Bibi."

##### Arguments

`"bibi" [input...]`

- input - "Bibi is" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/bubot.cs#L67-L87

### BibiLib

#### Unknown command name

"Access the great cat bibi library."

##### Arguments

``


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/bubot.cs#L128-L136

#### "full"

"Access the great cat bibi library."

##### Arguments

`"full"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/bubot.cs#L138-L146

### Experience

#### "givexpbecausedowntimepercent"

Unknown description

##### Arguments

`"givexpbecausedowntimepercent" <percent>`

- percent - No description (byte)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Experience.cs#L55-L66

#### "givexpbecausedowntime"

Unknown description

##### Arguments

`"givexpbecausedowntime" <xp>`

- xp - No description (ulong)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Experience.cs#L68-L79

#### "xp"

Unknown description

##### Arguments

`"xp"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Experience.cs#L81-L100

##### Arguments

`"xp" <member>`

- member - No description (DiscordMember)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Experience.cs#L102-L119

#### "xptop"

Unknown description

##### Arguments

`"xptop"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Experience.cs#L121-L171

#### "xpcard"

Unknown description

##### Arguments

`"xpcard" <user>`

- user - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Experience.cs#L174-L254

##### Arguments

`"xpcard"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Experience.cs#L256-L260

### MinecraftModule

#### "getfromusername"

"Get a minecraft players UUID from their username"

`"username"`

##### Arguments

`"getfromusername" <input>`

- input - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Gamering/Minecraft.cs#L31-L45

### SteamCommands

#### "search"

"Search about a game"

`"s"`

##### Arguments

`"search" [game...]`

- game - "The game" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Gamering/Steam.cs#L32-L106

### Genericcommands

#### "hi"

"Hello fellow human! beep boop"

##### Arguments

`"hi"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L41-L49

#### "time"

"Get the time in UTC"

##### Arguments

`"time"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L51-L61

#### "invite"

"Invite me to your server"

##### Arguments

`"invite"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L63-L73

#### "ping"

Unknown description

##### Arguments

`"ping"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L75-L83

#### "dump"

"Dump a messages raw content (useful for emote walls)"

##### Arguments

`"dump" <message>`

- message - No description (DiscordMessage)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L85-L100

##### Arguments

`"dump"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L102-L114

#### "archive"

"Archive a message (and its attachments)"

##### Arguments

`"archive" <message>`

- message - No description (DiscordMessage)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L116-L145

#### "duckhosting"

"SilverHosting best"

`"dukthosting"`,`"ducthosting"`,`":duckhosting:"`,`"<:duckhosting:797225115837792367>"`

##### Arguments

`"duckhosting"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L148-L162

#### "user"

"Get the info I know about a specified user"

##### Arguments

`"user" <a>`

- a - "the user (left blank = person running command)" (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L169-L194

##### Arguments

`"user"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/genericcommands.cs#L196-L201

### ImageModule

#### "caption"

"Captions an image"

##### Arguments

`"caption" <image> [text...]`

- image - No description (SdImage)
- text - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L404-L413

##### Arguments

`"caption" [text...]`

- text - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L415-L419

#### "fail"

"epic embed fail"

##### Arguments

`"fail" [text...]`

- text - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L421-L434

#### "yeet"

"YEET"

##### Arguments

`"yeet"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L436-L441

##### Arguments

`"yeet" <img2>`

- img2 - No description (SdImage)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L443-L496

#### "jpeg"

Unknown description

##### Arguments

`"jpeg" <image>`

- image - "the url of the image" (SdImage)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L551-L557

##### Arguments

`"jpeg"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L559-L564

#### "tint"

Unknown description

##### Arguments

`"tint" <image> <color>`

- image - "the url of the image" (SdImage)
- color - "A hex color (RGB OR RGBA), or a dotnet KnownColor" (Color)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L599-L610

##### Arguments

`"tint" <color>`

- color - "A hex color (RGB OR RGBA), or a dotnet KnownColor" (Color)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L612-L618

#### "adventuretime"

Unknown description

##### Arguments

`"adventuretime"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L620-L625

##### Arguments

`"adventuretime" <friendo>`

- friendo - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L627-L631

##### Arguments

`"adventuretime" <person> <friendo>`

- person - No description (DiscordUser)
- friendo - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L633-L654

#### "seal"

"He was forced to use Microsoft Windows when he was 6"

##### Arguments

`"seal" [text...]`

- text - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L656-L664

#### "linus"

"NVIDIA, fuck you."

##### Arguments

`"linus" [company...]`

- company - "company,or thing you want linus to swear at" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L666-L714

#### "resize"

Unknown description

##### Arguments

`"resize" <image> [x] [y] [format]`

- image - "the url of the image" (SdImage)
- x - "Width" (int)
- y - "Height" (int)
- format - No description (string?)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L716-L723

##### Arguments

`"resize" <image> <format>`

- image - No description (SdImage)
- format - No description (string?)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L754-L758

##### Arguments

`"resize" <format>`

- format - No description (string?)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L760-L764

##### Arguments

`"resize" [x] [y] [format]`

- x - "Width" (int)
- y - "Height" (int)
- format - No description (string?)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L766-L772

#### "reliable"

Unknown description

##### Arguments

`"reliable"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L774-L778

##### Arguments

`"reliable" <koichi>`

- koichi - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L780-L784

##### Arguments

`"reliable" <jotaro> <koichi>`

- jotaro - No description (DiscordUser)
- koichi - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L786-L837

#### "ObMedal"

Unknown description

##### Arguments

`"ObMedal"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L839-L843

##### Arguments

`"ObMedal" <obama>`

- obama - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L845-L860

##### Arguments

`"ObMedal" <obama> <secondPerson>`

- obama - No description (DiscordUser)
- secondPerson - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L862-L883

#### "happynewyear"

Unknown description

##### Arguments

`"happynewyear"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L885-L889

##### Arguments

`"happynewyear" <person>`

- person - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L891-L901

#### "silver"

Unknown description

##### Arguments

`"silver"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L913-L919

##### Arguments

`"silver" <image>`

- image - "the url of the image" (SdImage)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L921-L930

#### "catgen"

Unknown description

##### Arguments

`"catgen" [name...]`

- name - "the name of the cat" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ImageModule.cs#L932-L995

### MiscCommands

#### "version"

"Get the version info"

`"ver"`,`"verinfo"`,`"versioninfo"`

##### Arguments

`"version"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Misc.cs#L59-L67

#### "setlang"

"set your language"

##### Arguments

`"setlang" <langName>`

- langName - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Misc.cs#L69-L98

#### "togglerepeat"

"toggle repeated strings"

##### Arguments

`"togglerepeat" <enable>`

- enable - No description (bool)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Misc.cs#L100-L134

#### "translateunknown"

"translate from an unknown language"

`"translate"`

##### Arguments

`"translateunknown" [text...]`

- text - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Misc.cs#L136-L156

#### "whereis"

"get the location of a silverbot command through the silvercraftspec.md file"

##### Arguments

`"whereis" [commandname...]`

- commandname - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Misc.cs#L158-L243

#### "dotnetrtfm"

"Gives you a documentation link for a .NET entity."

##### Arguments

`"dotnetrtfm" <term>`

- term - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Misc.cs#L286-L322

#### "fdroid"

"Searches f-droid for an app"

##### Arguments

`"fdroid" <term>`

- term - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Misc.cs#L324-L357

#### "translateunknownto"

"translate from an unknown language to a specified one"

`"translateto"`

##### Arguments

`"translateunknownto" <languageTo> [text...]`

- languageTo - No description (string)
- text - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Misc.cs#L359-L394

### ModCommands

#### "kick"

"Kick a specified user"

##### Arguments

`"kick" <a> [reason...]`

- a - "the user like duh" (DiscordMember)
- reason - "the reason" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ModCommands.cs#L29-L63

#### "ban"

"bans a specified user"

##### Arguments

`"ban" <a> [reason...]`

- a - "the user like duh" (DiscordUser)
- reason - "the reason" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ModCommands.cs#L65-L103

#### "kickme"

"Kicks yourself lmao (or ban if you're super daring)"

##### Arguments

`"kickme" [ban]`

- ban - "hardcore" (bool)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ModCommands.cs#L105-L120

#### "purge"

"Downloads and removes X messages from the current channel."

`"clean"`,`"clear"`

##### Arguments

`"purge" <amount>`

- amount - "number of messages" (int)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ModCommands.cs#L122-L162

### OwnerOnly

#### "reloadcolors"

"reloads the colors.json"

##### Arguments

`"reloadcolors"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L50-L68

#### "UnRegisterCommand"

Unknown description

##### Arguments

`"UnRegisterCommand" [cmdwithparm...]`

- cmdwithparm - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L70-L75

#### "RegisterModule"

Unknown description

##### Arguments

`"RegisterModule" <mod>`

- mod - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L77-L83

#### "sudo"

"Executes a command as another user."

##### Arguments

`"sudo" <member> [command...]`

- member - "Member to execute as." (DiscordMember)
- command - "Command text to execute." (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L85-L98

#### Unknown command name

Unknown description

##### Arguments

``


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L100-L101

##### Arguments

``


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L507-L508

#### "setupcategory"

"Set up a category in the silverbot dev server"

##### Arguments

`"setupcategory" <role>`

- role - "The role to set up a category for" (DiscordRole)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L103-L141

##### Arguments

`"setupcategory" <person>`

- person - "The person to set up a category for" (DiscordMember)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L143-L178

#### "dependencies"

"get the dependencies used"

##### Arguments

`"dependencies"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L188-L208

#### "lsschedtasks"

"list scheduled tasks"

##### Arguments

`"lsschedtasks"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L211-L237

#### "evaluate"

"EVALUATE SOME C# CODE"

`"eval"`,`"ev"`

##### Arguments

`"evaluate" [code...]`

- code - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L243-L350

#### "sh"

"runs some commands"

##### Arguments

`"sh" [command...]`

- command - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L352-L386

#### "runsql"

"runs some sql"

##### Arguments

`"runsql" [sql...]`

- sql - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L388-L394

#### "addemotes"

"testing shiz"

##### Arguments

`"addemotes"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L397-L465

#### "guilds"

Unknown description

##### Arguments

`"guilds"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L467-L479

#### "togglebanuser"

Unknown description

##### Arguments

`"togglebanuser" <userid> [ban]`

- userid - No description (DiscordUser)
- ban - No description (bool)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L481-L487

#### "shutdown"

"kill the bot"

##### Arguments

`"shutdown"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L489-L496

#### "removeuser"

Unknown description

##### Arguments

`"removeuser" <userid>`

- userid - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/OwnerOnly.cs#L499-L505

### ReactionRoleCommands

#### Unknown command name

Unknown description

##### Arguments

``


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ReactionRoleCommands.cs#L44-L46

#### "addmenu"

Unknown description

##### Arguments

`"addmenu"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ReactionRoleCommands.cs#L50-L218

### ReminderCommands

#### "remindme"

"simple reminder"

##### Arguments

`"remindme" <duration> [item...]`

- duration - "delay of reminder (e.g. 1m = 1 minute)" (TimeSpan)
- item - "content" (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ReminderCommands.cs#L33-L68

#### "listreminders"

"lists all reminders"

##### Arguments

`"listreminders"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ReminderCommands.cs#L70-L104

#### "listremindersguild"

"lists all reminders here"

##### Arguments

`"listremindersguild"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ReminderCommands.cs#L106-L141

#### "cancelreminder"

"deletes a specific reminder"

##### Arguments

`"cancelreminder" [id...]`

- id - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ReminderCommands.cs#L143-L173

#### "cancelreminderf"

"deletes a reminder with force"

##### Arguments

`"cancelreminderf" [id...]`

- id - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ReminderCommands.cs#L175-L208

### ServerStatsCommands

#### "emoteanalyse"

"analyse emote usage in a specified channel"

##### Arguments

`"emoteanalyse" <channel> [limit]`

- channel - No description (DiscordChannel)
- limit - No description (int)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ServerStatsCommands.cs#L36-L80

#### "setcategory"

Unknown description

##### Arguments

`"setcategory" <category>`

- category - No description (DiscordChannel)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ServerStatsCommands.cs#L82-L106

#### "setstatstrings"

Unknown description

##### Arguments

`"setstatstrings"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ServerStatsCommands.cs#L108-L119

##### Arguments

`"setstatstrings" <cake>`

- cake - No description (string[])

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/ServerStatsCommands.cs#L121-L132

### AudioSlash

#### "playnext"

"Tell me to play a song next in the queue"

##### Arguments

`"playnext" <sg>`

- sg - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L25-L34

#### "play"

"Tell me to play a song"

##### Arguments

`"play" <sg>`

- sg - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L36-L45

#### "volume"

"Change the volume 1-100%"

##### Arguments

`"volume" <volume>`

- volume - No description (long)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L47-L53

#### "seek"

"Seeks to the specified time"

##### Arguments

`"seek" <time>`

- time - No description (TimeSpan?)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L55-L64

#### "clearqueue"

"Clears the queue"

##### Arguments

`"clearqueue"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L66-L72

#### "shuffle"

"Shuffles the queue"

##### Arguments

`"shuffle"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L74-L80

#### "export"

"Export the queue"

##### Arguments

`"export" [playlistName]`

- playlistName - No description (string?)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L83-L90

#### "remove"

"Remove song X from the queue. 0 < index > queue length"

##### Arguments

`"remove" <songindex>`

- songindex - No description (long)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L93-L98

#### "loop"

"Loops nothing/the queue/the currently playing song\""

##### Arguments

`"loop" <settings>`

- settings - No description (LoopSettings)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L100-L107

#### "pause"

"pause the current song"

##### Arguments

`"pause"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L110-L115

#### "resume"

"resume the current song"

##### Arguments

`"resume"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L117-L122

#### "join"

"join the voice chat you're in"

##### Arguments

`"join"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L124-L129

#### "forceskip"

"skip a song. dj only command"

##### Arguments

`"forceskip"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L132-L138

#### "leave"

"disconnect from voice channel"

##### Arguments

`"leave"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L140-L146

#### "skip"

"vote skip"

##### Arguments

`"skip"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/AudioSlash.cs#L148-L153

### BubotSlash

#### "bibi"

"Makes a image with the great cat Bibi."

##### Arguments

`"bibi" <input>`

- input - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/BubotSlash.cs#L60-L79

### GeneralCommands

#### "hello"

"A simple hello command"

##### Arguments

`"hello"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/GeneralCommands.cs#L28-L33

#### "whois"

"Find out the info silverbot knows about someone."

##### Arguments

`"whois" <user>`

- user - No description (DiscordUser)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/GeneralCommands.cs#L63-L67

#### Unknown command name

Unknown description

##### Arguments

``


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/GeneralCommands.cs#L69-L73

##### Arguments

``


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/GeneralCommands.cs#L98-L107

#### "version"

"Find out the version info for this instance of silverbot"

##### Arguments

`"version"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/GeneralCommands.cs#L75-L82

#### "dukthosting"

"SilverHosting:tm: best"

##### Arguments

`"dukthosting"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/Slash/GeneralCommands.cs#L84-L96

### StableDiff

#### "sdimodels"

"Gets a list of the models available to the stable diff server"

##### Arguments

`"sdimodels"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/StableDiff.cs#L38-L50

#### "imagine"

Unknown description

##### Arguments

`"imagine" [prompt] [model] [seed] [guidance_scale] [prompt_strength] [negative_prompt] [resolution] [steps]`

- prompt - "The text prompt to generate" (string)
- model - "The model to use for generation" (string)
- seed - "The seed (RNG)" (int?)
- guidance_scale - "Guidance scale https://getimg.ai/guides/interactive-guide-to-stable-diffusion-guidance-scale-parameter" (double)
- prompt_strength - No description (double)
- negative_prompt - "The text prompt to AVOID generating" (string)
- resolution - No description (int)
- steps - No description (int)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/StableDiff.cs#L52-L244

### TranslatorCommands

#### "getobjectfromcurrentlanguage"

"yayayayayyayaya"

##### Arguments

`"getobjectfromcurrentlanguage" <name>`

- name - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/TranslatorCommands.cs#L45-L51

#### "setlangtranslator"

"set you're testing language"

##### Arguments

`"setlangtranslator" <lang>`

- lang - No description (string)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/TranslatorCommands.cs#L53-L117

#### "uploadlanguage"

Unknown description

##### Arguments

`"uploadlanguage"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/TranslatorCommands.cs#L119-L147

#### "generatelangtemplate"

"make a template for translation"

##### Arguments

`"generatelangtemplate" [lang]`

- lang - No description (string?)

https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS/Commands/TranslatorCommands.cs#L149-L207

## /github/workspace/SilverCraft.SnowdPlayer/SilverCraft.SnowdPlayer.csproj

## /github/workspace/SilverNeutralCommandHelper/SilverNeutralCommandHelper.csproj

## /github/workspace/SilverBot.SysAdminModule/SilverBot.SysAdminModule.csproj

## /github/workspace/SilverBotDS.AnimeModule/SilverBotDS.AnimeModule.csproj

### Anime

#### "hug"

"i have no idea what this means"

##### Arguments

`"hug"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/Anime.cs#L39-L44

#### "kiss"

"i have no idea what this means"

##### Arguments

`"kiss"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/Anime.cs#L46-L51

#### "slap"

"i have no idea what this means"

##### Arguments

`"slap"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/Anime.cs#L53-L58

#### "wink"

"i have no idea what this means"

##### Arguments

`"wink"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/Anime.cs#L60-L65

#### "pat"

"i have no idea what this means"

##### Arguments

`"pat"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/Anime.cs#L67-L72

#### "kill"

"the thing im gonna do to bub in fartnite"

##### Arguments

`"kill"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/Anime.cs#L74-L79

#### "cuddle"

"i have no idea what this means"

##### Arguments

`"cuddle"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/Anime.cs#L81-L86

#### "punch"

"i have no idea what this means"

##### Arguments

`"punch"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/Anime.cs#L88-L93

### AnimeSlash

#### "hug"

"i have no idea what this means"

##### Arguments

`"hug"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/AnimeSlash.cs#L34-L38

#### "kiss"

"i have no idea what this means"

##### Arguments

`"kiss"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/AnimeSlash.cs#L40-L44

#### "slap"

"i have no idea what this means"

##### Arguments

`"slap"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/AnimeSlash.cs#L46-L50

#### "wink"

"i have no idea what this means"

##### Arguments

`"wink"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/AnimeSlash.cs#L52-L56

#### "pat"

"i have no idea what this means"

##### Arguments

`"pat"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/AnimeSlash.cs#L58-L62

#### "kill"

"the thing im gonna do to bub in fartnite"

##### Arguments

`"kill"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/AnimeSlash.cs#L64-L68

#### "cuddle"

"i have no idea what this means"

##### Arguments

`"cuddle"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/AnimeSlash.cs#L70-L74

#### "punch"

"i have no idea what this means"

##### Arguments

`"punch"`


https://github.com/thesilvercraft/SilverCraft.SilverBot/blob/master/SilverBotDS.AnimeModule/AnimeSlash.cs#L76-L80

```json
{"/github/workspace/SilverBot.Web/SilverBot.Web.csproj":{"CommandModules":[]},"/github/workspace/SilverBot.Shared/SilverBot.Shared.csproj":{"CommandModules":[]},"/github/workspace/SilverBotDS/SilverBotDS.csproj":{"CommandModules":[{"Name":"AdminCommands","Commands":[{"Location":"/SilverBotDS/Commands/AdminCommands.cs#L43-L53","Name":"\u0022setprefix\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireUserPermissions"],"Arguments":[{"Name":"cake","Type":"string[]","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/AdminCommands.cs#L63-L100","Name":"\u0022emojipoll\u0022","Description":"\u0022Start a simple emoji poll for a simple yes/no question\u0022","Aliases":null,"CustomAttributes":["Cooldown"],"Arguments":[{"Name":"duration","Type":"TimeSpan","Description":"\u0022How long should the poll last. (e.g. 1m = 1 minute)\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"question","Type":"string","Description":"\u0022Poll question\u0022","RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/AdminCommands.cs#L102-L135","Name":"\u0022GiveAway\u0022","Description":"\u0022Start a simple giveaway\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"duration","Type":"TimeSpan","Description":"\u0022How long should the giveaway last. (e.g. 1m = 1 minute)\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"item","Type":"string","Description":"\u0022Giveaway content\u0022","RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/AdminCommands.cs#L137-L164","Name":"\u0022exportemotestoguilded\u0022","Description":"\u0022Exports your guild\u0027s emotes into a \\\u0022Emote Pack\\\u0022 Guilded can read\u0022","Aliases":null,"CustomAttributes":["RequireGuild"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/AdminCommands.cs#L166-L207","Name":"\u0022exportemotes\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireGuild"],"Arguments":[]}]},{"Name":"Audio","Commands":[{"Location":"/SilverBotDS/Commands/Audio.cs#L27-L32","Name":"\u0022playnext\u0022","Description":null,"Aliases":["\u0022pn\u0022"],"CustomAttributes":[],"Arguments":[{"Name":"song","Type":"SongORSongs","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Audio.cs#L35-L41","Name":"\u0022play\u0022","Description":"\u0022Tell me to play a song\u0022","Aliases":["\u0022p\u0022"],"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L43-L48","Name":"\u0022musiccontroller\u0022","Description":"\u0022Controls music playback\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L50-L54","Name":"\u0022play\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"song","Type":"SongORSongs","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Audio.cs#L56-L63","Name":"\u0022volume\u0022","Description":"\u0022Change the volume 1-100%\u0022","Aliases":["\u0022vol\u0022"],"CustomAttributes":["RequireDj"],"Arguments":[{"Name":"volume","Type":"ushort","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Audio.cs#L65-L71","Name":"\u0022seek\u0022","Description":"\u0022Seeks to the specified time\u0022","Aliases":null,"CustomAttributes":["RequireDj"],"Arguments":[{"Name":"time","Type":"TimeSpan","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Audio.cs#L74-L80","Name":"\u0022clearqueue\u0022","Description":"\u0022Clears the queue\u0022","Aliases":null,"CustomAttributes":["RequireDj"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L82-L88","Name":"\u0022shuffle\u0022","Description":"\u0022Shuffles the queue\u0022","Aliases":null,"CustomAttributes":["RequireDj"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L90-L95","Name":"\u0022export\u0022","Description":"\u0022Export the queue\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"playlistName","Type":"string?","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Audio.cs#L97-L102","Name":"\u0022remove\u0022","Description":"\u0022Remove song X from the queue. 0 \u003C index \u003E queue length\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"songindex","Type":"int","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Audio.cs#L104-L110","Name":"\u0022queuehistory\u0022","Description":"\u0022check what was playing\u0022","Aliases":["\u0022qh\u0022","\u0022prevplaying\u0022","\u0022pq\u0022"],"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L112-L118","Name":"\u0022queue\u0022","Description":"\u0022check whats playing rn and whats next\u0022","Aliases":["\u0022np\u0022","\u0022nowplaying\u0022","\u0022q\u0022"],"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L120-L126","Name":"\u0022loop\u0022","Description":"\u0022Loops nothing/the queue/the currently playing song\u0022","Aliases":["\u0022repeat\u0022"],"CustomAttributes":[],"Arguments":[{"Name":"settings","Type":"LoopSettings","Description":"\u0022Has to be none, song or queue\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Audio.cs#L129-L134","Name":"\u0022pause\u0022","Description":"\u0022pause the current song\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L136-L141","Name":"\u0022ovh\u0022","Description":"\u0022get the lyrics from ovh\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"name","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"artist","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Audio.cs#L143-L148","Name":"\u0022songaliases\u0022","Description":"\u0022Get the hardcoded aliases in silverbots code\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L150-L155","Name":"\u0022resume\u0022","Description":"\u0022resume the current song\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L157-L162","Name":"\u0022join\u0022","Description":"\u0022to join the voice chat you\u0027re in\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L165-L172","Name":"\u0022forceskip\u0022","Description":"\u0022skip a song. dj only command\u0022","Aliases":["\u0022fs\u0022"],"CustomAttributes":["RequireDj"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L174-L180","Name":"\u0022voteskip\u0022","Description":"\u0022skip a song\u0022","Aliases":["\u0022skip\u0022"],"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L182-L189","Name":"\u0022forcedisconnect\u0022","Description":"\u0022Tell me to leave your channel of the voice type, without checking if its in a vc\u0022","Aliases":["\u0022fuckoffisntworking\u0022"],"CustomAttributes":["RequireDj"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Audio.cs#L191-L198","Name":"\u0022disconnect\u0022","Description":"\u0022Tell me to leave your channel of the voice type\u0022","Aliases":["\u0022fuckoff\u0022","\u0022minecraftbedrockisbetter\u0022","\u0022fockoff\u0022","\u0022leave\u0022"],"CustomAttributes":["RequireDj"],"Arguments":[]}]},{"Name":"BingCommandModule","Commands":[{"Location":"/SilverBotDS/Commands/BingCommandModule.cs#L30-L55","Name":"\u0022addBingChannel\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireUserPermissions","RequireGuild"],"Arguments":[{"Name":"bingchannel","Type":"DiscordChannel","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]}]},{"Name":"Bubot","Commands":[{"Location":"/SilverBotDS/Commands/bubot.cs#L36-L44","Name":"\u0022silveryeet\u0022","Description":"\u0022Sends SilverYeet.gif\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]}]},{"Name":"BibiCommands","Commands":[{"Location":"/SilverBotDS/Commands/bubot.cs#L67-L87","Name":"\u0022bibi\u0022","Description":"\u0022Makes a image with the great cat Bibi.\u0022","Aliases":null,"CustomAttributes":["Cooldown"],"Arguments":[{"Name":"input","Type":"string","Description":"\u0022Bibi is\u0022","RemainingText":true,"Optional":false,"CustomAttributes":[]}]}]},{"Name":"BibiLib","Commands":[{"Location":"/SilverBotDS/Commands/bubot.cs#L128-L136","Name":null,"Description":"\u0022Access the great cat bibi library.\u0022","Aliases":null,"CustomAttributes":["GroupCommand"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/bubot.cs#L138-L146","Name":"\u0022full\u0022","Description":"\u0022Access the great cat bibi library.\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]}]},{"Name":"Experience","Commands":[{"Location":"/SilverBotDS/Commands/Experience.cs#L55-L66","Name":"\u0022givexpbecausedowntimepercent\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireOwner"],"Arguments":[{"Name":"percent","Type":"byte","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Experience.cs#L68-L79","Name":"\u0022givexpbecausedowntime\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireOwner"],"Arguments":[{"Name":"xp","Type":"ulong","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Experience.cs#L81-L100","Name":"\u0022xp\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Experience.cs#L102-L119","Name":"\u0022xp\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"member","Type":"DiscordMember","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Experience.cs#L121-L171","Name":"\u0022xptop\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireGuild"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Experience.cs#L174-L254","Name":"\u0022xpcard\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"user","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Experience.cs#L256-L260","Name":"\u0022xpcard\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]}]},{"Name":"MinecraftModule","Commands":[{"Location":"/SilverBotDS/Commands/Gamering/Minecraft.cs#L31-L45","Name":"\u0022getfromusername\u0022","Description":"\u0022Get a minecraft players UUID from their username\u0022","Aliases":["\u0022username\u0022"],"CustomAttributes":[],"Arguments":[{"Name":"input","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]}]},{"Name":"SteamCommands","Commands":[{"Location":"/SilverBotDS/Commands/Gamering/Steam.cs#L32-L106","Name":"\u0022search\u0022","Description":"\u0022Search about a game\u0022","Aliases":["\u0022s\u0022"],"CustomAttributes":[],"Arguments":[{"Name":"game","Type":"string","Description":"\u0022The game\u0022","RemainingText":true,"Optional":false,"CustomAttributes":[]}]}]},{"Name":"Genericcommands","Commands":[{"Location":"/SilverBotDS/Commands/genericcommands.cs#L41-L49","Name":"\u0022hi\u0022","Description":"\u0022Hello fellow human! beep boop\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L51-L61","Name":"\u0022time\u0022","Description":"\u0022Get the time in UTC\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L63-L73","Name":"\u0022invite\u0022","Description":"\u0022Invite me to your server\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L75-L83","Name":"\u0022ping\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L85-L100","Name":"\u0022dump\u0022","Description":"\u0022Dump a messages raw content (useful for emote walls)\u0022","Aliases":null,"CustomAttributes":["RequirePermissions"],"Arguments":[{"Name":"message","Type":"DiscordMessage","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L102-L114","Name":"\u0022dump\u0022","Description":"\u0022Dump a messages raw content (useful for emote walls)\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L116-L145","Name":"\u0022archive\u0022","Description":"\u0022Archive a message (and its attachments)\u0022","Aliases":null,"CustomAttributes":["RequirePermissions"],"Arguments":[{"Name":"message","Type":"DiscordMessage","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L148-L162","Name":"\u0022duckhosting\u0022","Description":"\u0022SilverHosting best\u0022","Aliases":["\u0022dukthosting\u0022","\u0022ducthosting\u0022","\u0022:duckhosting:\u0022","\u0022\u003C:duckhosting:797225115837792367\u003E\u0022"],"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L169-L194","Name":"\u0022user\u0022","Description":"\u0022Get the info I know about a specified user\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"a","Type":"DiscordUser","Description":"\u0022the user (left blank = person running command)\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/genericcommands.cs#L196-L201","Name":"\u0022user\u0022","Description":"\u0022Get the info I know about a specified user\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]}]},{"Name":"ImageModule","Commands":[{"Location":"/SilverBotDS/Commands/ImageModule.cs#L404-L413","Name":"\u0022caption\u0022","Description":"\u0022Captions an image\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"image","Type":"SdImage","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"text","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L415-L419","Name":"\u0022caption\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"text","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L421-L434","Name":"\u0022fail\u0022","Description":"\u0022epic embed fail\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"text","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L436-L441","Name":"\u0022yeet\u0022","Description":"\u0022YEET\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L443-L496","Name":"\u0022yeet\u0022","Description":"\u0022YEET\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"img2","Type":"SdImage","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L551-L557","Name":"\u0022jpeg\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"image","Type":"SdImage","Description":"\u0022the url of the image\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L559-L564","Name":"\u0022jpeg\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L599-L610","Name":"\u0022tint\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"image","Type":"SdImage","Description":"\u0022the url of the image\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"color","Type":"Color","Description":"\u0022A hex color (RGB OR RGBA), or a dotnet KnownColor\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L612-L618","Name":"\u0022tint\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"color","Type":"Color","Description":"\u0022A hex color (RGB OR RGBA), or a dotnet KnownColor\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L620-L625","Name":"\u0022adventuretime\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireGuild"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L627-L631","Name":"\u0022adventuretime\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"friendo","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L633-L654","Name":"\u0022adventuretime\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"person","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"friendo","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L656-L664","Name":"\u0022seal\u0022","Description":"\u0022He was forced to use Microsoft Windows when he was 6\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"text","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L666-L714","Name":"\u0022linus\u0022","Description":"\u0022NVIDIA, fuck you.\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"company","Type":"string","Description":"\u0022company,or thing you want linus to swear at\u0022","RemainingText":true,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L716-L723","Name":"\u0022resize\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"image","Type":"SdImage","Description":"\u0022the url of the image\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"x","Type":"int","Description":"\u0022Width\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"y","Type":"int","Description":"\u0022Height\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"format","Type":"string?","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L754-L758","Name":"\u0022resize\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"image","Type":"SdImage","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"format","Type":"string?","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L760-L764","Name":"\u0022resize\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"format","Type":"string?","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L766-L772","Name":"\u0022resize\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"x","Type":"int","Description":"\u0022Width\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"y","Type":"int","Description":"\u0022Height\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"format","Type":"string?","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L774-L778","Name":"\u0022reliable\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L780-L784","Name":"\u0022reliable\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"koichi","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L786-L837","Name":"\u0022reliable\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"jotaro","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"koichi","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L839-L843","Name":"\u0022ObMedal\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L845-L860","Name":"\u0022ObMedal\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"obama","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L862-L883","Name":"\u0022ObMedal\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"obama","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"secondPerson","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L885-L889","Name":"\u0022happynewyear\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L891-L901","Name":"\u0022happynewyear\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"person","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L913-L919","Name":"\u0022silver\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L921-L930","Name":"\u0022silver\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"image","Type":"SdImage","Description":"\u0022the url of the image\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ImageModule.cs#L932-L995","Name":"\u0022catgen\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"name","Type":"string","Description":"\u0022the name of the cat\u0022","RemainingText":true,"Optional":true,"CustomAttributes":[]}]}]},{"Name":"MiscCommands","Commands":[{"Location":"/SilverBotDS/Commands/Misc.cs#L59-L67","Name":"\u0022version\u0022","Description":"\u0022Get the version info\u0022","Aliases":["\u0022ver\u0022","\u0022verinfo\u0022","\u0022versioninfo\u0022"],"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Misc.cs#L69-L98","Name":"\u0022setlang\u0022","Description":"\u0022set your language\u0022","Aliases":null,"CustomAttributes":["RequireUserPermissions"],"Arguments":[{"Name":"langName","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Misc.cs#L100-L134","Name":"\u0022togglerepeat\u0022","Description":"\u0022toggle repeated strings\u0022","Aliases":null,"CustomAttributes":["RequireUserPermissions"],"Arguments":[{"Name":"enable","Type":"bool","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Misc.cs#L136-L156","Name":"\u0022translateunknown\u0022","Description":"\u0022translate from an unknown language\u0022","Aliases":["\u0022translate\u0022"],"CustomAttributes":[],"Arguments":[{"Name":"text","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Misc.cs#L158-L243","Name":"\u0022whereis\u0022","Description":"\u0022get the location of a silverbot command through the silvercraftspec.md file\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"commandname","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Misc.cs#L286-L322","Name":"\u0022dotnetrtfm\u0022","Description":"\u0022Gives you a documentation link for a .NET entity.\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"term","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Misc.cs#L324-L357","Name":"\u0022fdroid\u0022","Description":"\u0022Searches f-droid for an app\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"term","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/Misc.cs#L359-L394","Name":"\u0022translateunknownto\u0022","Description":"\u0022translate from an unknown language to a specified one\u0022","Aliases":["\u0022translateto\u0022"],"CustomAttributes":[],"Arguments":[{"Name":"languageTo","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"text","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]}]},{"Name":"ModCommands","Commands":[{"Location":"/SilverBotDS/Commands/ModCommands.cs#L29-L63","Name":"\u0022kick\u0022","Description":"\u0022Kick a specified user\u0022","Aliases":null,"CustomAttributes":["RequirePermissions"],"Arguments":[{"Name":"a","Type":"DiscordMember","Description":"\u0022the user like duh\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"reason","Type":"string","Description":"\u0022the reason\u0022","RemainingText":true,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ModCommands.cs#L65-L103","Name":"\u0022ban\u0022","Description":"\u0022bans a specified user\u0022","Aliases":null,"CustomAttributes":["RequirePermissions"],"Arguments":[{"Name":"a","Type":"DiscordUser","Description":"\u0022the user like duh\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"reason","Type":"string","Description":"\u0022the reason\u0022","RemainingText":true,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ModCommands.cs#L105-L120","Name":"\u0022kickme\u0022","Description":"\u0022Kicks yourself lmao (or ban if you\u0027re super daring)\u0022","Aliases":null,"CustomAttributes":["RequireBotPermissions"],"Arguments":[{"Name":"ban","Type":"bool","Description":"\u0022hardcore\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ModCommands.cs#L122-L162","Name":"\u0022purge\u0022","Description":"\u0022Downloads and removes X messages from the current channel.\u0022","Aliases":["\u0022clean\u0022","\u0022clear\u0022"],"CustomAttributes":["RequirePermissions"],"Arguments":[{"Name":"amount","Type":"int","Description":"\u0022number of messages\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]}]},{"Name":"OwnerOnly","Commands":[{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L50-L68","Name":"\u0022reloadcolors\u0022","Description":"\u0022reloads the colors.json\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L70-L75","Name":"\u0022UnRegisterCommand\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"cmdwithparm","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L77-L83","Name":"\u0022RegisterModule\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"mod","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L85-L98","Name":"\u0022sudo\u0022","Description":"\u0022Executes a command as another user.\u0022","Aliases":null,"CustomAttributes":["Hidden","RequireOwner"],"Arguments":[{"Name":"member","Type":"DiscordMember","Description":"\u0022Member to execute as.\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"command","Type":"string","Description":"\u0022Command text to execute.\u0022","RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L100-L101","Name":null,"Description":null,"Aliases":null,"CustomAttributes":["GeneratedRegex"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L103-L141","Name":"\u0022setupcategory\u0022","Description":"\u0022Set up a category in the silverbot dev server\u0022","Aliases":null,"CustomAttributes":["RequireBotPermissions"],"Arguments":[{"Name":"role","Type":"DiscordRole","Description":"\u0022The role to set up a category for\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L143-L178","Name":"\u0022setupcategory\u0022","Description":"\u0022Set up a category in the silverbot dev server\u0022","Aliases":null,"CustomAttributes":["RequireBotPermissions"],"Arguments":[{"Name":"person","Type":"DiscordMember","Description":"\u0022The person to set up a category for\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L188-L208","Name":"\u0022dependencies\u0022","Description":"\u0022get the dependencies used\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L211-L237","Name":"\u0022lsschedtasks\u0022","Description":"\u0022list scheduled tasks\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L243-L350","Name":"\u0022evaluate\u0022","Description":"\u0022EVALUATE SOME C# CODE\u0022","Aliases":["\u0022eval\u0022","\u0022ev\u0022"],"CustomAttributes":[],"Arguments":[{"Name":"code","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L352-L386","Name":"\u0022sh\u0022","Description":"\u0022runs some commands\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"command","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L388-L394","Name":"\u0022runsql\u0022","Description":"\u0022runs some sql\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"sql","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L397-L465","Name":"\u0022addemotes\u0022","Description":"\u0022testing shiz\u0022","Aliases":null,"CustomAttributes":["RequireGuild","RequirePermissions"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L467-L479","Name":"\u0022guilds\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L481-L487","Name":"\u0022togglebanuser\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"userid","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"ban","Type":"bool","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L489-L496","Name":"\u0022shutdown\u0022","Description":"\u0022kill the bot\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L499-L505","Name":"\u0022removeuser\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"userid","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/OwnerOnly.cs#L507-L508","Name":null,"Description":null,"Aliases":null,"CustomAttributes":["GeneratedRegex"],"Arguments":[]}]},{"Name":"ReactionRoleCommands","Commands":[{"Location":"/SilverBotDS/Commands/ReactionRoleCommands.cs#L44-L46","Name":null,"Description":null,"Aliases":null,"CustomAttributes":["GeneratedRegex"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ReactionRoleCommands.cs#L50-L218","Name":"\u0022addmenu\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[]}]},{"Name":"ReminderCommands","Commands":[{"Location":"/SilverBotDS/Commands/ReminderCommands.cs#L33-L68","Name":"\u0022remindme\u0022","Description":"\u0022simple reminder\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"duration","Type":"TimeSpan","Description":"\u0022delay of reminder (e.g. 1m = 1 minute)\u0022","RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"item","Type":"string","Description":"\u0022content\u0022","RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ReminderCommands.cs#L70-L104","Name":"\u0022listreminders\u0022","Description":"\u0022lists all reminders\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ReminderCommands.cs#L106-L141","Name":"\u0022listremindersguild\u0022","Description":"\u0022lists all reminders here\u0022","Aliases":null,"CustomAttributes":["RequireUserPermissions"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ReminderCommands.cs#L143-L173","Name":"\u0022cancelreminder\u0022","Description":"\u0022deletes a specific reminder\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"id","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ReminderCommands.cs#L175-L208","Name":"\u0022cancelreminderf\u0022","Description":"\u0022deletes a reminder with force\u0022","Aliases":null,"CustomAttributes":["RequireUserPermissions"],"Arguments":[{"Name":"id","Type":"string","Description":null,"RemainingText":true,"Optional":false,"CustomAttributes":[]}]}]},{"Name":"ServerStatsCommands","Commands":[{"Location":"/SilverBotDS/Commands/ServerStatsCommands.cs#L36-L80","Name":"\u0022emoteanalyse\u0022","Description":"\u0022analyse emote usage in a specified channel\u0022","Aliases":null,"CustomAttributes":["Cooldown"],"Arguments":[{"Name":"channel","Type":"DiscordChannel","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]},{"Name":"limit","Type":"int","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ServerStatsCommands.cs#L82-L106","Name":"\u0022setcategory\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireUserPermissions","RequireBotPermissions"],"Arguments":[{"Name":"category","Type":"DiscordChannel","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/ServerStatsCommands.cs#L108-L119","Name":"\u0022setstatstrings\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireUserPermissions","RequireBotPermissions"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/ServerStatsCommands.cs#L121-L132","Name":"\u0022setstatstrings\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireUserPermissions","RequireBotPermissions"],"Arguments":[{"Name":"cake","Type":"string[]","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]}]},{"Name":"AudioSlash","Commands":[{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L25-L34","Name":"\u0022playnext\u0022","Description":"\u0022Tell me to play a song next in the queue\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"sg","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":["Option"]}]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L36-L45","Name":"\u0022play\u0022","Description":"\u0022Tell me to play a song\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"sg","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":["Option"]}]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L47-L53","Name":"\u0022volume\u0022","Description":"\u0022Change the volume 1-100%\u0022","Aliases":null,"CustomAttributes":["RequireDjSlash"],"Arguments":[{"Name":"volume","Type":"long","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":["Option"]}]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L55-L64","Name":"\u0022seek\u0022","Description":"\u0022Seeks to the specified time\u0022","Aliases":null,"CustomAttributes":["RequireDjSlash"],"Arguments":[{"Name":"time","Type":"TimeSpan?","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":["Option"]}]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L66-L72","Name":"\u0022clearqueue\u0022","Description":"\u0022Clears the queue\u0022","Aliases":null,"CustomAttributes":["RequireDjSlash"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L74-L80","Name":"\u0022shuffle\u0022","Description":"\u0022Shuffles the queue\u0022","Aliases":null,"CustomAttributes":["RequireDjSlash"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L83-L90","Name":"\u0022export\u0022","Description":"\u0022Export the queue\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"playlistName","Type":"string?","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":["Option"]}]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L93-L98","Name":"\u0022remove\u0022","Description":"\u0022Remove song X from the queue. 0 \u003C index \u003E queue length\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"songindex","Type":"long","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":["Option"]}]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L100-L107","Name":"\u0022loop\u0022","Description":"\u0022Loops nothing/the queue/the currently playing song\\\u0022\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"settings","Type":"LoopSettings","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":["Option"]}]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L110-L115","Name":"\u0022pause\u0022","Description":"\u0022pause the current song\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L117-L122","Name":"\u0022resume\u0022","Description":"\u0022resume the current song\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L124-L129","Name":"\u0022join\u0022","Description":"\u0022join the voice chat you\u0027re in\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L132-L138","Name":"\u0022forceskip\u0022","Description":"\u0022skip a song. dj only command\u0022","Aliases":null,"CustomAttributes":["RequireDjSlash"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L140-L146","Name":"\u0022leave\u0022","Description":"\u0022disconnect from voice channel\u0022","Aliases":null,"CustomAttributes":["RequireDjSlash"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/AudioSlash.cs#L148-L153","Name":"\u0022skip\u0022","Description":"\u0022vote skip\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]}]},{"Name":"BubotSlash","Commands":[{"Location":"/SilverBotDS/Commands/Slash/BubotSlash.cs#L60-L79","Name":"\u0022bibi\u0022","Description":"\u0022Makes a image with the great cat Bibi.\u0022","Aliases":null,"CustomAttributes":["SlashCooldown"],"Arguments":[{"Name":"input","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":["Option"]}]}]},{"Name":"GeneralCommands","Commands":[{"Location":"/SilverBotDS/Commands/Slash/GeneralCommands.cs#L28-L33","Name":"\u0022hello\u0022","Description":"\u0022A simple hello command\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/GeneralCommands.cs#L63-L67","Name":"\u0022whois\u0022","Description":"\u0022Find out the info silverbot knows about someone.\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"user","Type":"DiscordUser","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":["Option"]}]},{"Location":"/SilverBotDS/Commands/Slash/GeneralCommands.cs#L69-L73","Name":null,"Description":null,"Aliases":null,"CustomAttributes":["ContextMenu"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/GeneralCommands.cs#L75-L82","Name":"\u0022version\u0022","Description":"\u0022Find out the version info for this instance of silverbot\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/GeneralCommands.cs#L84-L96","Name":"\u0022dukthosting\u0022","Description":"\u0022SilverHosting:tm: best\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/Slash/GeneralCommands.cs#L98-L107","Name":null,"Description":null,"Aliases":null,"CustomAttributes":["ContextMenu"],"Arguments":[]}]},{"Name":"StableDiff","Commands":[{"Location":"/SilverBotDS/Commands/StableDiff.cs#L38-L50","Name":"\u0022sdimodels\u0022","Description":"\u0022Gets a list of the models available to the stable diff server\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS/Commands/StableDiff.cs#L52-L244","Name":"\u0022imagine\u0022","Description":null,"Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"prompt","Type":"string","Description":"\u0022The text prompt to generate\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"model","Type":"string","Description":"\u0022The model to use for generation\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"seed","Type":"int?","Description":"\u0022The seed (RNG)\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"guidance_scale","Type":"double","Description":"\u0022Guidance scale https://getimg.ai/guides/interactive-guide-to-stable-diffusion-guidance-scale-parameter\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"prompt_strength","Type":"double","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"negative_prompt","Type":"string","Description":"\u0022The text prompt to AVOID generating\u0022","RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"resolution","Type":"int","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]},{"Name":"steps","Type":"int","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]}]}]},{"Name":"TranslatorCommands","Commands":[{"Location":"/SilverBotDS/Commands/TranslatorCommands.cs#L45-L51","Name":"\u0022getobjectfromcurrentlanguage\u0022","Description":"\u0022yayayayayyayaya\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"name","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/TranslatorCommands.cs#L53-L117","Name":"\u0022setlangtranslator\u0022","Description":"\u0022set you\u0027re testing language\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"lang","Type":"string","Description":null,"RemainingText":false,"Optional":false,"CustomAttributes":[]}]},{"Location":"/SilverBotDS/Commands/TranslatorCommands.cs#L119-L147","Name":"\u0022uploadlanguage\u0022","Description":null,"Aliases":null,"CustomAttributes":["RequireAttachment"],"Arguments":[]},{"Location":"/SilverBotDS/Commands/TranslatorCommands.cs#L149-L207","Name":"\u0022generatelangtemplate\u0022","Description":"\u0022make a template for translation\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[{"Name":"lang","Type":"string?","Description":null,"RemainingText":false,"Optional":true,"CustomAttributes":[]}]}]}]},"/github/workspace/SilverCraft.SnowdPlayer/SilverCraft.SnowdPlayer.csproj":{"CommandModules":[]},"/github/workspace/SilverNeutralCommandHelper/SilverNeutralCommandHelper.csproj":{"CommandModules":[]},"/github/workspace/SilverBot.SysAdminModule/SilverBot.SysAdminModule.csproj":{"CommandModules":[]},"/github/workspace/SilverBotDS.AnimeModule/SilverBotDS.AnimeModule.csproj":{"CommandModules":[{"Name":"Anime","Commands":[{"Location":"/SilverBotDS.AnimeModule/Anime.cs#L39-L44","Name":"\u0022hug\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/Anime.cs#L46-L51","Name":"\u0022kiss\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/Anime.cs#L53-L58","Name":"\u0022slap\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/Anime.cs#L60-L65","Name":"\u0022wink\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/Anime.cs#L67-L72","Name":"\u0022pat\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/Anime.cs#L74-L79","Name":"\u0022kill\u0022","Description":"\u0022the thing im gonna do to bub in fartnite\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/Anime.cs#L81-L86","Name":"\u0022cuddle\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/Anime.cs#L88-L93","Name":"\u0022punch\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]}]},{"Name":"AnimeSlash","Commands":[{"Location":"/SilverBotDS.AnimeModule/AnimeSlash.cs#L34-L38","Name":"\u0022hug\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/AnimeSlash.cs#L40-L44","Name":"\u0022kiss\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/AnimeSlash.cs#L46-L50","Name":"\u0022slap\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/AnimeSlash.cs#L52-L56","Name":"\u0022wink\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/AnimeSlash.cs#L58-L62","Name":"\u0022pat\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/AnimeSlash.cs#L64-L68","Name":"\u0022kill\u0022","Description":"\u0022the thing im gonna do to bub in fartnite\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/AnimeSlash.cs#L70-L74","Name":"\u0022cuddle\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]},{"Location":"/SilverBotDS.AnimeModule/AnimeSlash.cs#L76-L80","Name":"\u0022punch\u0022","Description":"\u0022i have no idea what this means\u0022","Aliases":null,"CustomAttributes":[],"Arguments":[]}]}]}}
```

***This file is maintained by a github action.***

<!-- markdownlint-restore -->
