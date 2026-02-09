namespace Middleware.Web.Domains.Common;

public class KlaviyoEventListSubscribeRequest
{

    public KlaviyoEventListSubscribeRequest()
    {
        data = new Data();
    }

    public Data data { get; set; }

    public class Data
    {
        public Data()
        {
            attributes = new Attributes();
        }

        public string type { get; set; }
        public Attributes attributes { get; set; }
    }

    public class Attributes
    {
        public Properties properties { get; set; }
        public Metric metric { get; set; }
        public Profile profile { get; set; }
        public string unique_id { get; set; }
    }

    public class Metric
    {
        public Metric()
        {
            data = new MetricData();
        }

        public MetricData data { get; set; }
    }

    public class MetricData
    {
        public MetricData()
        {
            attributes = new MetricAttributes();
        }

        public string type { get; set; }
        public MetricAttributes attributes { get; set; }
    }

    public class MetricAttributes
    {
        public string name { get; set; }
    }

    public class Profile
    {
        public Profile()
        {
            data = new ProfileData();
        }

        public ProfileData data { get; set; }
    }

    public class ProfileData
    {
        public ProfileData()
        {
            attributes = new ProfileAttributes();
        }

        public string type { get; set; }
        public ProfileAttributes attributes { get; set; }
    }

    public class ProfileAttributes
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public Location location { get; set; }
    }

    public class Location
    {
        public string city { get; set; }
        public string region { get; set; }
        public string zip { get; set; }
    }

    public class Properties
    {
        public string subscribed { get; set; }
    }
}
