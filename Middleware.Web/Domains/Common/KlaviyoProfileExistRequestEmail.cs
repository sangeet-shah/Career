using System.Collections.Generic;

namespace Career.Data.Domains.Common;

public class KlaviyoProfileExistRequestEmail
{
    public KlaviyoProfileExistRequestEmail()
    {
        emails = new List<string>();
    }

    public List<string> emails { get; set; }
}