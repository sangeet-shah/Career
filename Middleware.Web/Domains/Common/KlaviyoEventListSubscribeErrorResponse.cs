using System.Collections.Generic;

namespace Middleware.Web.Domains.Common;

public class KlaviyoEventListSubscribeErrorResponse
{
    public KlaviyoEventListSubscribeErrorResponse()
    {
        errors = new List<Error>();
    }

    public List<Error> errors { get; set; }

    public class Error
    {
        public string id { get; set; }
        public int status { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public string detail { get; set; }
        public Source source { get; set; }
    }

    public class Source
    {
        public string pointer { get; set; }
    }
}
