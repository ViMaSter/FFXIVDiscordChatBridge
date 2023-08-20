using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

class PulsewayReporter
{
    private const string ENDPOINT = "https://api.pulseway.com/v2/";
    private static string _instanceId = null!;
    private readonly HttpClient _client;

    public PulsewayReporter(string username, string password, string instanceId)
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(username+ ":" + password)));

        _instanceId = instanceId;
    }

    // ReSharper disable InconsistentNaming
    private class NotifyRequest
    {
        public string? instance_id { get; set; }
        public string? title { get; set; }
        public string? message { get; set; }
        public string? priority { get; set; }
    }
    // ReSharper restore InconsistentNaming

    public enum Priority
    {
        Low,
        Normal,
        Elevated,
        Critical
    }

    public void SendMessage(string title, string message, Priority priority)
    {
        var response = _client.PostAsync(ENDPOINT + "notifications", new StringContent(JsonConvert.SerializeObject(new NotifyRequest
        {
            title = title,
            message = message,
            priority = priority.ToString(),
            instance_id = _instanceId
        }), Encoding.UTF8, "application/json")).Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Pulseway returned {response.StatusCode} with message {response.Content.ReadAsStringAsync().Result}");
        }
    }
}