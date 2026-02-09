using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Models.CorporateManagement;
using Middleware.Web.Models.Vendors;
using Middleware.Web.Services.Career;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Stores;
using Middleware.Web.Services.Vendors;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class VendorController : ControllerBase
{
    private readonly ICareerService _careerService;
    private readonly IPictureService _pictureService;
    private readonly IVendorService _vendorService;
    private readonly IStoreService _storeService;

    public VendorController(
        ICareerService careerService,
        IPictureService pictureService,
        IVendorService vendorService,
        IStoreService storeService)
    {
        _careerService = careerService;
        _pictureService = pictureService;
        _vendorService = vendorService;
        _storeService = storeService;
    }

    [HttpGet]
    public async Task<IActionResult> OurBrands()
    {
        var model = new CorporateManagementSettingsModel();

        var careerBrands = await _careerService.GetAllCorporateBrandPagesAsync();
        foreach (var careerBrand in careerBrands)
        {
            var picture = await _pictureService.GetPictureByIdAsync(careerBrand.PictureId);
            model.CareerBrandListModel.Add(new Middleware.Web.Models.Career.CareerBrandModel
            {
                Id = careerBrand.Id,
                Description = careerBrand.Description,
                PictureUrl = await _pictureService.GetPictureUrlCachingAsync(careerBrand.PictureId),
                AltAttribute = picture?.AltAttribute,
                TitleAttribute = picture?.TitleAttribute,
                Url = careerBrand.Url
            });
        }

        var vendors = await _vendorService.GetAllVendorsAsync();
        foreach (var vendor in vendors)
        {
            var picture = await _pictureService.GetPictureByIdAsync(vendor.CorporatePictureId);
            model.CorporateVendors.Add(new VendorModel
            {
                IsCorporate = vendor.IsCorporate,
                CorporatePictureId = vendor.CorporatePictureId,
                CorporatePictureUrl = await _pictureService.GetPictureUrlAsync(vendor.CorporatePictureId),
                CorporatePictureAltText = picture?.AltAttribute,
                CorporatePictureTitle = picture?.TitleAttribute,
                CorporateShortDescription = vendor.CorporateShortDescription
            });
        }

        return Ok(model);
    }
}

