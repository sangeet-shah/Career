using Career.Web.Models.Security;
using Career.Web.Models.SummerJams;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Career.Web.Controllers;

public class SummerJamController : BaseController
{
    private readonly IApiClient _apiClient;

    public SummerJamController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        

        SummerJamModel model;
        try
        {
            model = await _apiClient.GetAsync<SummerJamModel>("api/SummerJam/Index");
        }
        catch
        {
            return InvokeHttp404();
        }

        return View(model);
    }

    [HttpPost]
    public  async Task<IActionResult> Index(string slug, SummerJamModel model)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _apiClient.GetAsync<SummerJamModel>("api/SummerJam/Index") ?? new SummerJamModel();
            prepared.FirstName = model.FirstName;
            prepared.LastName = model.LastName;
            prepared.Email = model.Email;
            prepared.AddressLine1 = model.AddressLine1;
            prepared.AddressLine = model.AddressLine;
            prepared.City = model.City;
            prepared.State = model.State;
            prepared.StateName = model.StateName;
            prepared.ZipCode = model.ZipCode;
            prepared.Phone = model.Phone;
            prepared.DOB = model.DOB;

            return View(prepared);
        }

        var response = await _apiClient.PostAsync<SummerJamModel, SummerJamModel>("api/SummerJam/Index", model);
        if (response == null)
        {
            ModelState.AddModelError(string.Empty, "Unable to submit the form. Please try again.");
            var prepared = await _apiClient.GetAsync<SummerJamModel>("api/SummerJam/Index") ?? model;
            if (prepared != model)
            {
                prepared.FirstName = model.FirstName;
                prepared.LastName = model.LastName;
                prepared.Email = model.Email;
                prepared.AddressLine1 = model.AddressLine1;
                prepared.AddressLine = model.AddressLine;
                prepared.City = model.City;
                prepared.State = model.State;
                prepared.StateName = model.StateName;
                prepared.ZipCode = model.ZipCode;
                prepared.Phone = model.Phone;
                prepared.DOB = model.DOB;
            }
            return View(prepared);
        }

        return View(response);
    }

}
