using System.Collections.Generic;

namespace Middleware.Web.Models.LandingPage;

public class ContestPageResponseModel
{
    public bool ShowResult { get; set; }

    public LandingPageModel Model { get; set; } = new();

    public Dictionary<string, string> Errors { get; set; } = new();
}
