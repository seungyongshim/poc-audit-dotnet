using System.Text.Json.Serialization;

namespace Api.Middlewares;

public class Audit 
{
    [JsonPropertyName("evt_time")]
    public DateTime EvtTime { get; set; } = DateTime.UtcNow.AddHours(9);

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

    public ActionDetailContext ActionDetail { get; } = new();

    [JsonPropertyName("action_result")]
    public string? ActionResult { get; set; }

    [JsonPropertyName("req_uri")]
    public string? ReqUri { get; set; }

    [JsonPropertyName("req_domain")]
    public string? ReqDomain { get; set; }

    [JsonPropertyName("service_code")]
    public string? ServiceCode { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object> AdditionalData { get; set; } = [];

    public object this[string key]
    {
        get => AdditionalData[key];
        set => AdditionalData[key] = value;
    }
}

public class ActionDetailContext
{
    [JsonPropertyName("privacy_menu")]
    [JsonConverter(typeof(JsonPrivacyMenuConverter))]
    public bool? PrivacyMenu { get; set; }

    [JsonPropertyName("dst_username")]
    public string? DstUsername { get; set; }

    [JsonPropertyName("dst_character")]
    public string? DstCharacter { get; set; }

    [JsonPropertyName("dst_hostname")]
    public string? DstHostname { get; set; }

    [JsonPropertyName("command")]
    public string? Command { get; set; }

    [JsonPropertyName("command_type")]
    public string? CommandType { get; set; }

    [JsonPropertyName("action_reason")]
    public string? ActionReason { get; set; }

    [JsonPropertyName("approval_user")]
    public string? ApprovalUser { get; set; }

    [JsonPropertyName("approval_time")]
    public DateTime? ApprovalTime { get; set; }

    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }

    [JsonPropertyName("file_hash")]
    public string? FileHash { get; set; }

    [JsonPropertyName("file_size")]
    public double? FileSize { get; set; } // KB
}
