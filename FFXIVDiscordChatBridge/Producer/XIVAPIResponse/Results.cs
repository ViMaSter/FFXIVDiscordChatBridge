// ReSharper disable NotAccessedPositionalProperty.Global - External API definition
// ReSharper disable InconsistentNaming - External API definition
// ReSharper disable ClassNeverInstantiated.Global - External API definition

namespace FFXIVDiscordChatBridge.Producer.XIVAPIResponse;

public record Results(
    string Avatar,
    int FeastMatches,
    int ID,
    string Lang,
    string Name,
    object Rank,
    object RankIcon,
    string Server
);