﻿namespace FFXIVHelpers.Models;

public class Mapping
{
    public class ServiceInfo
    {
        public ConfirmationState ConfirmationState { get; private set; }

        protected ServiceInfo(ConfirmationState confirmationState)
        {
            ConfirmationState = confirmationState;
        }

        public void Confirm()
        {   
            ConfirmationState = ConfirmationState.Confirmed;
        }
    }
    
    public class DiscordInfo : ServiceInfo
    {
        public string Name { get; }
        
        public DiscordInfo(string name, ConfirmationState confirmationState) : base(confirmationState)
        {
            Name = name;
        }
    }
    
    public class FFXIVInfo : ServiceInfo
    {
        public Character Name { get; }

        public FFXIVInfo(Character name, ConfirmationState confirmationState) : base(confirmationState)
        {
            Name = name;
        }
    }
    
    public FFXIVInfo FFXIV { get; }

    public DiscordInfo Discord { get; }

    public string CombinedName(Dictionary<string, string> discordDisplayNameMapping)
    {
        if (discordDisplayNameMapping.TryGetValue(Discord.Name, out var displayName))
        {
            return $"{FFXIV.Name.Format(CharacterNameDisplay.WithoutWorld)}/@{displayName}";
        }

        return $"{FFXIV.Name.Format(CharacterNameDisplay.WithoutWorld)}/@{Discord.Name}";
    }

    [Newtonsoft.Json.JsonConstructor]
    private Mapping(FFXIVInfo ffxiv, DiscordInfo discord)
    {
        FFXIV = ffxiv;
        Discord = discord;
    }
        
    public static Mapping CreateFromDiscord(string discordName, Character ffxivName)
    {
        return new Mapping(new FFXIVInfo(ffxivName, ConfirmationState.NotConfirmed), new DiscordInfo(discordName, ConfirmationState.Confirmed));
    }
        
    public static Mapping CreateFromFFXIV(string discordName, Character ffxivName)
    {
        return new Mapping(new FFXIVInfo(ffxivName, ConfirmationState.Confirmed), new DiscordInfo(discordName, ConfirmationState.NotConfirmed));
    }
}