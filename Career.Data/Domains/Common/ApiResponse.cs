using Newtonsoft.Json;

namespace Career.Data.Domains.Common;

public class ApiResponse
{
    public bool Success { get; set; }

    public string Error { get; set; }

    public string ResponseResult { get; set; }

    public string Message { get; set; }

    [JsonProperty(PropertyName = "error")]
    public ConversionError conversion_error { get; set; }

    public class ConversionError
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "fbtrace_id")]
        public string FbtraceId { get; set; }
    }
}
