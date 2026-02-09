
namespace Middleware.Web.Domains.Common;

public class KlaviyoSMSUnSubscribeProfileRequest
{
    public KlaviyoSMSUnSubscribeProfileRequest()
    {
        data = new Data();
    }

    public Data data { get; set; }

    public class Data
    {
        public string type { get; set; }
        public Attributes attributes { get; set; }
    }

    public class Attributes
    {
        public string list_id { get; set; }
        public string phone_numbers { get; set; }
    }
}