using System.Collections.Generic;

namespace Career.Data.Domains.Common;

public class KlaviyoProfileExistRequestSMS
{
    public KlaviyoProfileExistRequestSMS()
    {
        phone_numbers = new List<string>();
    }

    public List<string> phone_numbers { get; set; }
}