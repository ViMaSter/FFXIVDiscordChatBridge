// ReSharper disable NotAccessedPositionalProperty.Global - External API definition
// ReSharper disable InconsistentNaming - External API definition
// ReSharper disable ClassNeverInstantiated.Global - External API definition

namespace FFXIVDiscordChatBridge.Producer.XIVAPIResponse;

public record RootObject(
    Pagination Pagination,
    Results[] Results
);