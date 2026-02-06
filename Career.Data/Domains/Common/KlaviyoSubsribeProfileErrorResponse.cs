using System.Collections.Generic;

namespace Career.Data.Domains.Common;

public class KlaviyoSubsribeProfileErrorResponse
{
    public KlaviyoSubsribeProfileErrorResponse()
    {
        errors = new List<Error>();
    }

    public List<Error> errors { get; set; }

    public class Error
    {
        public string id { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public string detail { get; set; }
        public Source source { get; set; }
    }

    public class Source
    {
        public string pointer { get; set; }
        public string parameter { get; set; }
    }
}