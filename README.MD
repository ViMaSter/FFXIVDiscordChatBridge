# FFXIV Discord Chat Bridge [![codecov](https://codecov.io/github/ViMaSter/FFXIVDiscordChatBridge/branch/main/graph/badge.svg?token=2PBSK0I6UI)](https://codecov.io/github/ViMaSter/FFXIVDiscordChatBridge)

Connect a Discord channel and FFXIV in-game chat channel.

> **Note**
> This app requires a dedicated FFXIV client running on Windows with uninterrupted keyboard input.
> It is recommended to set up a Windows Service with VNC, to ensure a Windows user always remains logged into the game.

# Usage
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
9. Run the following command:
   ```ps
   ./discord-chat-bridge.exe \
       --discordChannelID=<discordChannelID> \
       --discordToken=<discordToken> \
       --ffxivChannelCode=<ffxivChannelCode> 
   ```
10. Wait for a few seconds and check if the bot has joined the server channel.    
    As soon as the bot has joined Discord, you should see in-game messages sent to the specified Discord channel and vice versa.
