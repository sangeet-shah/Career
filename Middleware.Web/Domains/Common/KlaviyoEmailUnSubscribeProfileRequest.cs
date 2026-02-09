
namespace Middleware.Web.Domains.Common;

public class KlaviyoEmailUnSubscribeProfileRequest
{
    public KlaviyoEmailUnSubscribeProfileRequest()
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
        public string emails { get; set; }
    }
}