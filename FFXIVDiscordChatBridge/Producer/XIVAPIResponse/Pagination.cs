// ReSharper disable NotAccessedPositionalProperty.Global - External API definition
// ReSharper disable InconsistentNaming - External API definition
// ReSharper disable ClassNeverInstantiated.Global - External API definition

namespace FFXIVDiscordChatBridge.Producer.XIVAPIResponse;

public record Pagination(
    int Page,
    object PageNext,
    object PagePrev,
    int PageTotal,
    int Results,
    int ResultsPerPage,
    int ResultsTotal
);