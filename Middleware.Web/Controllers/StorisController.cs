using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Middleware.Web.Data;
using Middleware.Web.Filters;
using Middleware.Web.Options;

namespace Middleware.Web.Controllers;


[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class StorisController : ControllerBase
{
    #region Fields

    private readonly IEBridgeRepository _erp;
    private readonly INopRepository _nop;
    private readonly MiddlewareOptions _opt;

    #endregion

    #region Ctor

    public StorisController(IEBridgeRepository erp,
        INopRepository nop,
        IOptions<MiddlewareOptions> opt)
    {
        _erp = erp;
        _nop = nop;
        _opt = opt.Value;
    }

    #endregion

    #region Methods

    [HttpGet("{productKey}")]
    public async Task<IActionResult> GetLocationKeys(string productKey)
    {
        if (string.IsNullOrEmpty(productKey))
            return BadRequest("productKey is required");

        try
        {
            var result = await _erp.GetLocationKeysByProductKeyAsync(productKey, CancellationToken.None);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex?.Message);
        }
    }

    [HttpGet("{productKey}")]
    public async Task<IActionResult> UpdateStorisProduct(string productKey)
    {
        if (string.IsNullOrEmpty(productKey))
            return BadRequest("productKey is required");

        try
        {
            var productRows = await _erp.GetProductsAsync(CancellationToken.None, sku: productKey);
            if (productRows.Count > 0)
            {
                var affected = await _nop.UpsertProductsAsync(productRows, _opt.BatchSize, CancellationToken.None);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex?.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPowerReviewInvoice()
    {
        try
        {
            return Ok(await _erp.GetPowerReviewInvoiceAsync());
        }
        catch (Exception ex)
        {
            return BadRequest(ex?.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetChatMeterReviewFeed()
    {
        try
        {
            return Ok(await _erp.GetChatMeterReviewFeedAsync());
        }
        catch (Exception ex)
        {
            return BadRequest(ex?.Message);
        }
    }

    [HttpGet("{sku}/{zipcode}")]
    public async Task<IActionResult> GetProductDeliveryDate(string sku, string zipcode)
    {
        if (string.IsNullOrEmpty(sku))
            return BadRequest("sku is required");
        if (string.IsNullOrEmpty(zipcode))
            return BadRequest("zipcode is required");

        try
        {
            return Ok(await _erp.GetProductDeliveryDateAsync(sku, zipcode));
        }
        catch (Exception ex)
        {
            return BadRequest(ex?.Message);
        }
    }

    #endregion
}
