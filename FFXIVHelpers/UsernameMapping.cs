using FFXIVHelpers.Models;
using FFXIVHelpers.Persistence;
using Microsoft.Extensions.Logging;

namespace FFXIVHelpers;

public class UsernameMapping
{
    private Character? _hostingFFXIVCharacter;
    private List<Mapping> Mappings { get; }

    private readonly ILogger<UsernameMapping> _logger;
    private readonly IPersistence _persistence;

    public UsernameMapping(ILogger<UsernameMapping> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;

        Mappings = persistence.LoadMappings();
    }
    
    public bool ReceiveFromFFXIV(Character ffxivName, string discordName, out string message)
    {
        var unconfirmedMatchingAccount = Mappings.FirstOrDefault(mapping => 
            mapping.Discord.Name?.ToLower() == discordName.ToLower() &&
            mapping.Discord.ConfirmationState == ConfirmationState.Confirmed &&
            Equals(mapping.FFXIV.Name, ffxivName) &&
            mapping.FFXIV.ConfirmationState == ConfirmationState.NotConfirmed);
        if (unconfirmedMatchingAccount != null)
        {
            _logger.LogInformation("Confirmed link of {DiscordName} to {FfxivName} inside FFXIV", discordName, ffxivName);
            unconfirmedMatchingAccount.FFXIV.Confirm();
            _persistence.WriteMappingsToFile(Mappings);
            message = $"Successfully linked your Discord username to your FFXIV character. You will now be shown as <{ffxivName.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{discordName}>.";
            return true;
        }
        
        var unconfirmedDiscordAccount = Mappings.FirstOrDefault(mapping => 
            mapping.Discord.Name?.ToLower() == discordName.ToLower() &&
            mapping.Discord.ConfirmationState == ConfirmationState.NotConfirmed);
        if (unconfirmedDiscordAccount != null)
        {
            _logger.LogInformation("Removed unconfirmed link of {DiscordName} to {FfxivName} inside FFXIV", discordName, ffxivName);
            Mappings.Remove(unconfirmedDiscordAccount);
        }
        
        _logger.LogInformation("Created unconfirmed link of {DiscordName} to {FfxivName} inside FFXIV", discordName, ffxivName);
        Mappings.Add(Mapping.CreateFromFFXIV(discordName, ffxivName));
        _persistence.WriteMappingsToFile(Mappings);
        message = $"To confirm your Final Fantasy character, send your character and world name to the bot as a direct message on discord: `{ffxivName.Format(CharacterNameDisplay.WITH_WORLD)}`";
        return false;
    }
    
    public void ReceiveFromDiscord(Character ffxivName, string discordName, out string message)
    {
        var unconfirmedMatchingAccount = Mappings.FirstOrDefault(mapping => 
            Equals(mapping.FFXIV.Name, ffxivName) &&
            mapping.FFXIV.ConfirmationState == ConfirmationState.Confirmed &&
            mapping.Discord.Name?.ToLower() == discordName.ToLower() &&
            mapping.Discord.ConfirmationState == ConfirmationState.NotConfirmed);
        if (unconfirmedMatchingAccount != null)
        {
            _logger.LogInformation("Confirmed link of {DiscordName} to {FfxivName} inside Discord", discordName, ffxivName);
            unconfirmedMatchingAccount.Discord.Confirm();
            _persistence.WriteMappingsToFile(Mappings);
            message = $"Successfully linked your Discord username to your FFXIV character. You will now be shown as <{ffxivName.Format(CharacterNameDisplay.WITHOUT_WORLD)}/@{discordName}>.";
            return;
        }
        
        var unconfirmedDiscordAccount = Mappings.FirstOrDefault(mapping => 
            Equals(mapping.FFXIV.Name, ffxivName) &&
            mapping.FFXIV.ConfirmationState == ConfirmationState.NotConfirmed);
        if (unconfirmedDiscordAccount != null)
        {
            _logger.LogInformation("Removed unconfirmed link of {DiscordName} to {FfxivName} inside Discord", discordName, ffxivName);
            Mappings.Remove(unconfirmedDiscordAccount);
        }
        
        _logger.LogInformation("Created unconfirmed link of {DiscordName} to {FfxivName} inside Discord", discordName, ffxivName);
        Mappings.Add(Mapping.CreateFromDiscord(discordName, ffxivName));
        _persistence.WriteMappingsToFile(Mappings);
        message = $"To confirm your Discord username, log into {ffxivName.Format(CharacterNameDisplay.WITH_WORLD)} and enter the following into the FFXIV chat: `/tell {_hostingFFXIVCharacter!.Format(CharacterNameDisplay.WITH_WORLD)} {discordName}`";
    }
    
    public string? GetMappingFromDiscordUsername(string discordName)
    {
        var confirmedMappings = Mappings.FirstOrDefault(mapping => 
            mapping.Discord.Name?.ToLower() == discordName.ToLower() && 
            mapping.Discord.ConfirmationState == ConfirmationState.Confirmed && 
            mapping.FFXIV.ConfirmationState == ConfirmationState.Confirmed);
        return confirmedMappings?.CombinedName;
    }
    
    public string GetMappingFromFFXIVUsername(Character ffxivName)
    {
        var confirmedMappings = Mappings.FirstOrDefault(mapping => 
            Equals(mapping.FFXIV.Name, ffxivName) && 
            mapping.FFXIV.ConfirmationState == ConfirmationState.Confirmed && 
            mapping.Discord.ConfirmationState == ConfirmationState.Confirmed);
        return confirmedMappings?.CombinedName ?? ffxivName.Format(CharacterNameDisplay.WITHOUT_WORLD);
    }

    public void SetHostingCharacter(Character character)
    {
        _hostingFFXIVCharacter = character;
    }
}