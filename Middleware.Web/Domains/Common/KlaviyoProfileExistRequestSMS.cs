using System.Collections.Generic;

namespace Middleware.Web.Domains.Common;

public class KlaviyoProfileExistRequestSMS
{
    public KlaviyoProfileExistRequestSMS()
    {
        phone_numbers = new List<string>();
    }

    public List<string> phone_numbers { get; set; }
}