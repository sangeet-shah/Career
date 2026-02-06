using System;

namespace Career.Data.Domains.Common;

public class KlaviyoProfileExistReponse
{
    public Data[] data { get; set; }

    public class Data
    {
        public string id { get; set; }
        public Attributes attributes { get; set; }
    }

    public class Attributes
    {
        public string email { get; set; }
        public DateTime created { get; set; }

    }
}
