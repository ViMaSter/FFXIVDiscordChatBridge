# FFXIV Discord Chat Bridge

Connect a Discord channel and FFXIV in-game chat channel

# Usage
1. Enable Discord Developer Mode ![image](https://i.vimaster.de/direct/Discord_rpRIBHtEjm.png)
2. Right-click on the Discord channel you want to connect and copy the channel ID ![image](https://i.vimaster.de/direct/Discord_DSrrSRjT44.png)  
   Replace future occurrences of `<discordChannelID>` with the copied ID 
3. Download [the latest release](https://github.com/ViMaSter/FFXIVDiscordChatBridge/releases)
4. Run FFXIV using DirectX11 on Windows (DirectX9 and other platforms are not supported)
5. Run `./discord-chat-bridge.exe --discordChannelID=<discordChannelID>`
6. Notice
   - the script typing `hello` into the FFXIV chat
   - FFXIV chat messages logged inside `./FFXIVDiscordChatBridge.log`