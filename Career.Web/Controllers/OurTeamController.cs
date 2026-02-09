using Career.Web.Models.Customers;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class OurTeamController : BaseController
{
	#region Fields

	private readonly IApiClient _apiClient;

    #endregion

    #region Ctor
    public OurTeamController(IApiClient apiClient)
	{
		_apiClient = apiClient;
    }

	#endregion

	public async Task<IActionResult> List()
	{
		var customerListModel = await _apiClient.GetAsync<CustomerListModel>("api/OurTeam/List") ?? new CustomerListModel();
		return View(customerListModel);
	}

	public async Task<IActionResult> Detail(int id)
	{
		CustomerModel model;
		try
		{
			model = await _apiClient.GetAsync<CustomerModel>($"api/OurTeam/Detail/{id}");
            var maybe = NotFoundIfNull(model);
            if (maybe != null) return maybe;
		}
		catch
		{
			return InvokeHttp404();
		}

		return View(model);
	}
}
