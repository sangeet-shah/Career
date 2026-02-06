using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Career.Web.Models.Common;

public record WeeklyAdModel
{
    public WeeklyAdModel()
    {
        URLs = new List<SelectListItem>();
    }

    public string URL { get; set; }

    public string EcommPlugin { get; set; }

    public IList<SelectListItem> URLs { get; set; }
}
