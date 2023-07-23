# FFXIV Discord Chat Bridge [![codecov](https://codecov.io/github/ViMaSter/FFXIVDiscordChatBridge/branch/main/graph/badge.svg?token=2PBSK0I6UI)](https://codecov.io/github/ViMaSter/FFXIVDiscordChatBridge)

Connect a Discord channel and FFXIV in-game chat channel.

![image](https://i.vimaster.de/direct/Zf9KKYLhu4.png)

> **Note**
> This app requires a dedicated FFXIV client running on Windows with uninterrupted keyboard input.  
> It is recommended to set up a Windows Service with VNC, to ensure a Windows user always remains logged into the game.

# Setup
1. [Create a Discord application with "Bot" functionality](https://discord.com/developers/applications)
2. Generate/reset and copy the bot token  
   ![image](https://i.vimaster.de/direct/chrome_wTwAcZZDxd.png)  
   Replace future occurrences of `<discordToken>` with the copied token
3. [Let the bot join the server you want to use](https://discord.com/developers/docs/topics/oauth2#bot-authorization-flow)
4. Inside Discord, open the settings, open the `Advanced` section and enable Discord Developer Mode  
   ![image](https://i.vimaster.de/direct/Discord_rpRIBHtEjm.png)
5. Close the settings and right-click on the Discord channel you want to connect your bot to and click on `Copy channel ID`    
   ![image](https://i.vimaster.de/direct/Discord_DSrrSRjT44.png)  
   Replace future occurrences of `<discordChannelID>` with the copied ID
6. Based on the Final Fantasy XIV channel you want the bot to use, [select the corresponding ID from this page](https://gist.github.com/quisquous/1a6ea3cf102c65e3c375186f1173dff3#file-ffxiv-game-log-ids)    
   For example: If you want your bot to read messages from linkshell #1, use `0010`, for your Free Company chat use `0018`, etc.    
   Replace future occurrences of `<ffxivChannelCode>` with this ID
7. Download [the latest release](https://github.com/ViMaSter/FFXIVDiscordChatBridge/releases)
8. Run FFXIV using DirectX11 on Windows (DirectX9 and other platforms are not supported) in windowed mode
9. Note down which world your bot is on  
   Replace future occurrences of `<ffxivWorldName>` with this world name
10. Run the following command:
   ```ps
   ./discord-chat-bridge.exe \
       --discordChannelID=<discordChannelID> \
       --discordToken=<discordToken> \
       --ffxivChannelCode=<ffxivChannelCode> \
       --ffxivWorldName=<ffxivWorldName>
   ```
1Wait for a few seconds and check if the bot has joined the server channel.    
    As soon as the bot has joined Discord, you should see in-game messages sent to the specified Discord channel and vice versa.

# Linking Discord Usernames and Character Names

> **Note**
> Currently, mappings are only stored in memory and will be lost when the bot is restarted.

To have both your character name and discord username shown whenever you send a message on either platform, you can link your Discord account to your FFXIV character.
1. Inside Discord, send a direct message to your bot containing your full character name and server (e.g. `Haurchefant Greystone@Zalera`)
2. You will receive a confirmation message from the bot in Discord; follow the instructions to confirm your character in FFXIV