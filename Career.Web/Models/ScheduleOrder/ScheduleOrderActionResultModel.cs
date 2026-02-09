namespace Career.Web.Models.ScheduleOrder;

public class ScheduleOrderActionResultModel
{
    public bool RedirectToRoute { get; set; }

    public string? RouteName { get; set; }

    public int? OrderId { get; set; }

    public string? RedirectAction { get; set; }

    public string? RedirectController { get; set; }

    public ScheduleOrderDetailsModel? Model { get; set; }
}
