using System.Text.Json.Serialization;

namespace Api.Middlewares;

public class Audit
{
    [JsonPropertyName("evt_time")]
    public DateTimeOffset EvtTime { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("src_ip")]
    public string? SrcIp { get; set; }

    [JsonPropertyName("src_username")]
    public string? SrcUsername { get; set; }

    [JsonPropertyName("component")]
    public string? Component => ReqUri;

    [JsonPropertyName("action_item")]
    public string? ActionItem => ReqUri;

    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("action_detail")]

    public ActionDetailContext? ActionDetail { get; set; }

    [JsonPropertyName("action_result")]
    public string? ActionResult { get; set; }

    [JsonPropertyName("req_uri")]
    public string? ReqUri { get; set; }

    [JsonPropertyName("req_domain")]
    public string? ReqDomain { get; set; }

    [JsonPropertyName("service_code")]
    public string? ServiceCode { get; set; }
}

public class ActionDetailContext
{
    public string? Description { get; set; }
    public string? Detail { get; set; }
}
