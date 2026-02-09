using System.Collections.Generic;

namespace Middleware.Web.Domains.Common;

public class KlaviyoSMSSubscribeProfileRequest
{
    public KlaviyoSMSSubscribeProfileRequest()
    {
        data = new Data();
    }

    public Data data { get; set; }

    public class Data
    {
        public Data()
        {
            attributes = new Attributes();
            relationships = new Relationships();
        }

        public string type { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
    }

    public class Attributes
    {
        public Attributes()
        {
            profiles = new Profiles();
        }

        public Profiles profiles { get; set; }
        public string custom_source { get; set; }
    }

    public class Profiles
    {
        public Profiles()
        {
            data = new List<Profile>();
        }

        public List<Profile> data { get; set; }
    }

    public class Profile
    {
        public Profile()
        {
            attributes = new ProfileAttributes();
        }

        public string type { get; set; }
        public ProfileAttributes attributes { get; set; }
    }

    public class ProfileAttributes
    {
        public ProfileAttributes()
        {
            subscriptions = new Subscriptions();
        }

        public string phone_number { get; set; }
        public Subscriptions subscriptions { get; set; }
    }

    public class Subscriptions
    {
        public Subscriptions()
        {
            sms = new SmsSubscription();
        }

        public SmsSubscription sms { get; set; }
    }

    public class SmsSubscription
    {
        public Marketing marketing { get; set; }
    }

    public class Marketing
    {
        public string consent { get; set; }
    }

    public class Relationships
    {
        public Relationships()
        {
            list = new ListData();
        }

        public ListData list { get; set; }
    }

    public class ListData
    {
        public ListData()
        {
            data = new ListType();
        }

        public ListType data { get; set; }
    }

    public class ListType
    {
        public string type { get; set; }
        public string id { get; set; }
    }
}