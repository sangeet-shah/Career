using System.Collections.Generic;

namespace Career.Data.Domains.Common;

public class KlaviyoAddToListRequest
{
    public List<Data> data { get; set; }

    public KlaviyoAddToListRequest()
    {
        data = new List<Data>();
    }

    public class Data
    {
        public string type { get; set; }
        public string id { get; set; }
    }
}
