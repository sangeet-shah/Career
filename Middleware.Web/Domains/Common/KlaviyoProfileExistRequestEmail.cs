using System.Collections.Generic;

namespace Middleware.Web.Domains.Common;

public class KlaviyoProfileExistRequestEmail
{
    public KlaviyoProfileExistRequestEmail()
    {
        emails = new List<string>();
    }

    public List<string> emails { get; set; }
}