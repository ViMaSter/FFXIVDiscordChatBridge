# FFXIV Discord Chat Bridge [![codecov](https://codecov.io/github/ViMaSter/FFXIVDiscordChatBridge/branch/main/graph/badge.svg?token=2PBSK0I6UI)](https://codecov.io/github/ViMaSter/FFXIVDiscordChatBridge)

Connect a Discord channel and FFXIV in-game chat channel.

![image](https://i.vimaster.de/direct/Zf9KKYLhu4.png)

> **Warning**
> This app requires a **dedicated** FFXIV account and client running **on Windows** with **uninterrupted keyboard input**.      
> **You can't run this bot using the same account or machine you normally play on.**

# Setup
## Prerequisites
You need:
- a Discord server with a channel you want to use for the chat bridge
- a server/virtual machine running **Windows** with **FFXIV constantly as your focussed window**
   - > **Note**
     While you can use a free trial account, channels like Free Company chats will require you to be a member of the Free Company, which is only possible with an active subscription
   - on it, the following software needs to be installed:
      - [.NET 7.0 Runtime](https://dotnet.microsoft.com/download/dotnet/7.0)
      - the latest version [Final Fantasy XIV](https://freetrial.finalfantasyxiv.com)
      - the latest release of [this app](https://github.com/ViMaSter/FFXIVDiscordChatBridge/releases) extracted into any directory  
        Replace future occurrences of `<appDirectory>` with this directory.

## Configuration
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
6. Using the same context menu, select `Edit Channel`
7. Under `Integrations` create a new Webhook and click the `Copy Webhook URL` button  
   ![image](https://i.vimaster.de/direct/ffxiv-webhook.png)  
   Replace future occurrences of `<discordWebhookURL>` with the copied URL
8. Based on the Final Fantasy XIV channel you want the bot to use, [select the corresponding ID from this page](https://gist.github.com/quisquous/1a6ea3cf102c65e3c375186f1173dff3#file-ffxiv-game-log-ids)    
   For example: If you want your bot to read messages from linkshell #1, use `0010`, for your Free Company chat use `0018`, etc.    
   Replace future occurrences of `<ffxivChannelCode>` with this ID
9. Download [the latest release](https://github.com/ViMaSter/FFXIVDiscordChatBridge/releases)
10. Run FFXIV using DirectX11 on Windows (DirectX9 and other platforms are not supported) in windowed mode
11. Note down which world your bot is on  
    Replace future occurrences of `<ffxivWorldName>` with this world name
12. Run the following command:
   ```ps
       <appDirectory>/discord-chat-bridge.exe \
           --discordWebhookURL=<discordWebhookURL> \
           --discordChannelID=<discordChannelID> \
           --discordToken=<discordToken> \
           --ffxivChannelCode=<ffxivChannelCode> \
           --ffxivWorldName=<ffxivWorldName>
   ```

# Usage
## Regular chats
1. Wait for a few seconds and check if the bot has joined the server channel
2. As soon as the bot has joined Discord, you should see in-game messages sent to the specified Discord channel and vice versa

## Linking Discord Usernames and Character Names
To have both your character name and discord username shown whenever you send a message on either platform, you can link your Discord account to your FFXIV character.
1. Inside Discord, send a direct message to your bot containing your full character name and server (e.g. `Haurchefant Greystone@Zalera`)
2. You will receive a confirmation message from the bot in Discord; follow the instructions to confirm your character in FFXIV

# Technical design

This app is split into consumers and producers respectively:
- **Producers** simply take a pre-formatted message and send it to their respective app
- **Consumers** watch for incoming messages from their respective app and convert the incoming message into a pre-formatted message for the other format

**FFXIV** and **Discord** have a **Consumer** and **Producer** each, that are linked.
The 
