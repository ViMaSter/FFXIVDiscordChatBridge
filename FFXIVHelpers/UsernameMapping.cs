using FFXIVHelpers.Models;
using Microsoft.Extensions.Logging;

namespace FFXIVHelpers;

public class UsernameMapping
{
    private Character? _hostingFFXIVCharacter;
    private readonly ILogger<UsernameMapping> _logger;

    public UsernameMapping(ILogger<UsernameMapping> logger)
    {
        _logger = logger;
    }

    private class Mapping
    {
        private Mapping()
        {
        }
        
        public static Mapping CreateFromDiscord(string discordUsername, Character ffxivUsername)
        {
            return new Mapping()
            {
                Discord = (discordUsername, ConfirmationState.Confirmed),
                FFXIV = (ffxivUsername, ConfirmationState.NotConfirmed)
            };
        }
        
        public static Mapping CreateFromFFXIV(string discordUsername, Character ffxivUsername)
        {
            return new Mapping()
            {
                Discord = (discordUsername, ConfirmationState.NotConfirmed),
                FFXIV = (ffxivUsername, ConfirmationState.Confirmed)
            };
        }
        
        public void ConfirmFFXIVUsername()
        {
            FFXIV = (FFXIV.name, ConfirmationState.Confirmed);
        }
        
        public void ConfirmDiscordUsername()
        {
            Discord = (Discord.name, ConfirmationState.Confirmed);
        }

        public (Character name, ConfirmationState confirmationState) FFXIV { get; private set; }

        public (string? name, ConfirmationState confirmationState) Discord { get; private set; }

        public string CombinedName => $"{FFXIV.name.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{Discord.name}";
    }
    private List<Mapping> Mappings { get; } = new();

    private enum ConfirmationState
    {
        NotConfirmed = 0,
        Confirmed,
    }
    
    public bool ReceiveFromFFXIV(Character ffxivName, string discordName, out string message)
    {
        var unconfirmedMatchingAccount = Mappings.FirstOrDefault(mapping => 
            mapping.Discord.name == discordName &&
            mapping.Discord.confirmationState == ConfirmationState.Confirmed &&
            mapping.FFXIV.name == ffxivName &&
            mapping.FFXIV.confirmationState == ConfirmationState.NotConfirmed);
        if (unconfirmedMatchingAccount != null)
        {
            _logger.LogInformation("Confirmed link of {DiscordName} to {FFXIVName} inside FFXIV", discordName, ffxivName);
            unconfirmedMatchingAccount.ConfirmFFXIVUsername();
            message = $"Successfully linked your Discord username to your FFXIV username. You will now be shown as <{ffxivName.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{discordName}>.";
            return true;
        }
        
        var unconfirmedDiscordAccount = Mappings.FirstOrDefault(mapping => 
            mapping.Discord.name == discordName &&
            mapping.Discord.confirmationState == ConfirmationState.NotConfirmed);
        if (unconfirmedDiscordAccount != null)
        {
            _logger.LogInformation("Removed unconfirmed link of {DiscordName} to {FFXIVName} inside FFXIV", discordName, ffxivName);
            Mappings.Remove(unconfirmedDiscordAccount);
        }
        
        _logger.LogInformation("Created unconfirmed link of {DiscordName} to {FFXIVName} inside FFXIV", discordName, ffxivName);
        Mappings.Add(Mapping.CreateFromFFXIV(discordName, ffxivName));
        message = $"To confirm your Final Fantasy character, send your character and world name to the bot as a direct message on discord: `{ffxivName.Format(CharacterNameDisplay.WITH_WORLD)}`";
        return false;
    }
    
    public void ReceiveFromDiscord(Character ffxivName, string discordName, out string message)
    {
        var unconfirmedMatchingAccount = Mappings.FirstOrDefault(mapping => 
            mapping.FFXIV.name == ffxivName &&
            mapping.FFXIV.confirmationState == ConfirmationState.Confirmed &&
            mapping.Discord.name == discordName &&
            mapping.Discord.confirmationState == ConfirmationState.NotConfirmed);
        if (unconfirmedMatchingAccount != null)
        {
            _logger.LogInformation("Confirmed link of {DiscordName} to {FFXIVName} inside Discord", discordName, ffxivName);
            unconfirmedMatchingAccount.ConfirmDiscordUsername();
            message = $"Successfully linked your Discord username to your FFXIV username. You will now be shown as <{ffxivName.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{discordName}>.";
            return;
        }
        
        var unconfirmedDiscordAccount = Mappings.FirstOrDefault(mapping => 
            mapping.FFXIV.name == ffxivName &&
            mapping.FFXIV.confirmationState == ConfirmationState.NotConfirmed);
        if (unconfirmedDiscordAccount != null)
        {
            _logger.LogInformation("Removed unconfirmed link of {DiscordName} to {FFXIVName} inside Discord", discordName, ffxivName);
            Mappings.Remove(unconfirmedDiscordAccount);
        }
        
        _logger.LogInformation("Created unconfirmed link of {DiscordName} to {FFXIVName} inside Discord", discordName, ffxivName);
        Mappings.Add(Mapping.CreateFromDiscord(discordName, ffxivName));
        message = $"To confirm your Discord username, log into {ffxivName.Format(CharacterNameDisplay.WITH_WORLD)} and enter the following into the FFXIV chat: `/tell {_hostingFFXIVCharacter.Format(CharacterNameDisplay.WITH_WORLD)} {discordName}`";
    }
    
    public string? GetMappingFromDiscordUsername(string discordUsername)
    {
        var confirmedMappings = Mappings.FirstOrDefault(mapping => 
            mapping.Discord.name == discordUsername && 
            mapping.Discord.confirmationState == ConfirmationState.Confirmed && 
            mapping.FFXIV.confirmationState == ConfirmationState.Confirmed);
        return confirmedMappings?.CombinedName;
    }
    
    public string GetMappingFromFFXIVUsername(Character ffxivUsername)
    {
        var confirmedMappings = Mappings.FirstOrDefault(mapping => 
            mapping.FFXIV.name == ffxivUsername && 
            mapping.FFXIV.confirmationState == ConfirmationState.Confirmed && 
            mapping.Discord.confirmationState == ConfirmationState.Confirmed);
        return confirmedMappings?.CombinedName ?? ffxivUsername.Format(CharacterNameDisplay.WITHOUT_WORLD);
    }

    public void SetHostingCharacter(Character character)
    {
        _hostingFFXIVCharacter = character;
    }
}