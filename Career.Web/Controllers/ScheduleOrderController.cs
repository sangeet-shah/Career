using Career.Data;
using Career.Data.Domains.ScheduleOrder;
using Career.Data.Services.Common;
using Career.Data.Services.Localization;
using Career.Web.Models.ScheduleOrder;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class ScheduleOrderController : BaseController
{
    #region Fields

    private readonly IHttpClientService _httpClientService;
    private readonly ILocalizationService _localizationService;
    private readonly AppSettings _appSettings;

    #endregion

    #region Ctor

    public ScheduleOrderController(IHttpClientService httpClientService,
        ILocalizationService localizationService,
        AppSettings appSettings)
    {
        _httpClientService = httpClientService;
        _localizationService = localizationService;
        _appSettings = appSettings;
    }

    #endregion

    #region Methods

    public  async Task<IActionResult> Schedule(int orderId)
    {
        if(orderId == 0)
            return RedirectToAction("Index", "Home");

        // get order
        var requestUrl = _appSettings.fmebridgeUrl+"orders/" + orderId + ",1";
        var response = await _httpClientService.GetAsync(requestUri: requestUrl,requestHeaders:null);
        if (string.IsNullOrEmpty(response.responseResult))
            return RedirectToAction("Index", "Home");

        var scheduleOrderResponse = JsonConvert.DeserializeObject<ScheduleOrderResponse>(response.responseResult);
        if (scheduleOrderResponse == null || scheduleOrderResponse.salesOrder == null)
            return RedirectToAction("Index", "Home");

        var model = new ScheduleOrderDetailsModel
        {
            OrderId = Convert.ToInt32(scheduleOrderResponse.salesOrder.orderId),
            IsZipCodeVerified = true
        };

        return View(model);
    }

    [HttpPost]
    public  async Task<IActionResult> Schedule(ScheduleOrderDetailsModel model)
    {
        if (model.IsZipCodeVerified)
        {
            // verify by zip
            if (!string.IsNullOrEmpty(model.ZipCode))
            {
                // get order
                var requestUrl = _appSettings.fmebridgeUrl+"orders/" + model.OrderId + ",1";
                var response = await _httpClientService.GetAsync(requestUri: requestUrl, requestHeaders:null);
                if (string.IsNullOrEmpty(response.responseResult))
                    return RedirectToAction("Index", "Home");

                var scheduleOrderResponse = JsonConvert.DeserializeObject<ScheduleOrderResponse>(response.responseResult);
                if (scheduleOrderResponse == null || scheduleOrderResponse.salesOrder == null)
                    return RedirectToAction("Index", "Home");

                var zipCode = scheduleOrderResponse.salesOrder.addresses.Any() ? scheduleOrderResponse.salesOrder.addresses.Where(x => x.addressType == (int)AddressType.Billing).Select(x => x.zipCode).FirstOrDefault() : string.Empty;
                if (model.ZipCode != zipCode)
                {
                    model.IsZipCodeVerified = true;
                    model.ErrorMessage = await _localizationService.GetLocaleStringResourceByNameAsync("FurnitureMart.DeliverySchedule.ZipCode.NotMatchWithOrder");
                }
                else
                    return RedirectToRoute("ScheduleOrderDetail", new { orderId = model.OrderId });
            }
            else
            {
                model.IsZipCodeVerified = true;
                model.ErrorMessage = await _localizationService.GetLocaleStringResourceByNameAsync("FurnitureMart.DeliverySchedule.ZipCode.Required");
            }
        }
        else
        {
            // verify by phone
            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                // get order
                var orderRequestUrl = _appSettings.fmebridgeUrl+"orders/" + model.OrderId + ",1";
                var orderResponse = await _httpClientService.GetAsync(requestUri: orderRequestUrl,requestHeaders:null);
                if (string.IsNullOrEmpty(orderResponse.responseResult))
                    return RedirectToAction("Index", "Home");

                var scheduleOrderResponse = JsonConvert.DeserializeObject<ScheduleOrderResponse>(orderResponse.responseResult);
                if (scheduleOrderResponse == null || scheduleOrderResponse.salesOrder == null)
                    return RedirectToAction("Index", "Home");

                // get customer
                var orderCustomerRequestUrl = _appSettings.fmebridgeUrl+"customers/" + scheduleOrderResponse.salesOrder.customerId + ",1";
                var orderCustomerResponse = await _httpClientService.GetAsync(requestUri: orderCustomerRequestUrl, requestHeaders: null);
                if (string.IsNullOrEmpty(orderCustomerResponse.responseResult))
                    return RedirectToAction("Index", "Home");

                var cutomerResponse = JsonConvert.DeserializeObject<CustomerResponse>(orderCustomerResponse.responseResult);
                if (cutomerResponse == null || cutomerResponse.customer == null)
                    return RedirectToAction("Index", "Home");

                var phoneNumber = cutomerResponse.customer.phones.Any() ? cutomerResponse.customer.phones.Where(x => x.number != "").Select(x => x.number).FirstOrDefault() : string.Empty;
                if (model.PhoneNumber != phoneNumber)
                {
                    model.IsZipCodeVerified = false;
                    model.ErrorMessage = await _localizationService.GetLocaleStringResourceByNameAsync("FurnitureMart.DeliverySchedule.PhoneNumber.NotMatchWithOrder");
                }
                else
                    return RedirectToRoute("ScheduleOrderDetail", new { orderId = model.OrderId });
            }
            else
            {
                model.IsZipCodeVerified = false;
                model.ErrorMessage = await _localizationService.GetLocaleStringResourceByNameAsync("FurnitureMart.DeliverySchedule.PhoneNumber.Required");
            }
        }

        return View(model);
    }

    public  async Task<IActionResult> ScheduleOrderVerifyInformation(int orderId)
    {
        // get order
        var requestUrl = _appSettings.fmebridgeUrl+"orders/" + orderId + ",1";
        var response = await _httpClientService.GetAsync(requestUri: requestUrl, requestHeaders: null);
        if (string.IsNullOrEmpty(response.responseResult))
            return RedirectToAction("Index", "Home");

        var scheduleOrderResponse = JsonConvert.DeserializeObject<ScheduleOrderResponse>(response.responseResult);
        if (scheduleOrderResponse == null || scheduleOrderResponse.salesOrder == null)
            return RedirectToAction("Index", "Home");

        // get customer
        var orderCustomerRequestUrl = _appSettings.fmebridgeUrl+"customers/" + scheduleOrderResponse.salesOrder.customerId + ",1";
        var orderCustomerResponse = await _httpClientService.GetAsync(requestUri: orderCustomerRequestUrl, requestHeaders: null);

        var cutomerResponse = new CustomerResponse();
        if (string.IsNullOrEmpty(orderCustomerResponse.responseResult))
            cutomerResponse = JsonConvert.DeserializeObject<CustomerResponse>(orderCustomerResponse.responseResult);

        var model = new ScheduleOrderDetailsModel
        {
            OrderId = Convert.ToInt32(scheduleOrderResponse.salesOrder.orderId),
            OrderDate = scheduleOrderResponse.salesOrder.orderDate,
            MerchandiseTotal = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.subTotal),
            DeliveryCharge = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.delivery),
            Tax = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.tax),
            TotalSalesOrder = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.invoiceTotal),
            AmountPaid = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.payments),
            AmountDue = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.invoiceTotal) - Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.payments)
        };

        if (scheduleOrderResponse.salesOrder.addresses.Any())
            model.DeliverTo = scheduleOrderResponse.salesOrder.addresses.Where(x => x.addressType == (int)AddressType.Shipping).Select(x => new ScheduleOrderDetailsModel.Address
            {
                Name = x.name,
                Address1 = x.address1,
                Address2 = x.address2,
                City = x.city,
                State = x.state,
                ZipCode = x.zipCode,
                Email = (cutomerResponse.customer != null ? cutomerResponse.customer.emailAddress : string.Empty),
                PhoneNumber = ((cutomerResponse.customer != null && cutomerResponse.customer.phones.Any()) ? cutomerResponse.customer.phones.Where(c => c.number != "").Select(c => c.number).FirstOrDefault() : string.Empty),
            }).FirstOrDefault();

        foreach (var lineItem in scheduleOrderResponse.salesOrder.lineItems)
        {
            var orderItemModel = new ScheduleOrderDetailsModel.OrderItemModel
            {
                Description = lineItem.description,
                Quantity = lineItem.quantity,
                Price = Convert.ToDecimal(lineItem.price)
            };

            model.Items.Add(orderItemModel);
        }

        return View(model);
    }

    [HttpPost]
    public  async Task<IActionResult> ScheduleOrderVerifyInformation(ScheduleOrderDetailsModel model)
    {
        // get order
        var requestUrl = _appSettings.fmebridgeUrl+"orders/" + model.OrderId + ",1";
        var response = await _httpClientService.GetAsync(requestUri: requestUrl, requestHeaders: null);
        if (string.IsNullOrEmpty(response.responseResult))
            return RedirectToAction("Index", "Home");

        var scheduleOrderResponse = JsonConvert.DeserializeObject<ScheduleOrderResponse>(response.responseResult);
        if (scheduleOrderResponse == null || scheduleOrderResponse.salesOrder == null)
            return RedirectToAction("Index", "Home");

        var shippingAddress = scheduleOrderResponse.salesOrder.addresses.Where(x => x.addressType == (int)AddressType.Shipping).FirstOrDefault();

        // update order            
        var updateOrderRequest = new UpdateOrderRequest
        {
            OrderId = Convert.ToInt32(scheduleOrderResponse.salesOrder.orderId),
            order = new UpdateOrderRequest.UpdatedOrder
            {
                DeliveryDate = scheduleOrderResponse.salesOrder.deliveryDate.HasValue ? scheduleOrderResponse.salesOrder.deliveryDate.Value : (DateTime?)null,
                DeliveryInstructions = "",
                OrderComments = "",
                ShippingAddress = new UpdateOrderRequest.Address
                {
                    Address1 = shippingAddress?.address1 ?? string.Empty,
                    Address2 = shippingAddress?.address2 ?? string.Empty,
                    City = shippingAddress?.city ?? string.Empty,
                    State = shippingAddress?.state ?? string.Empty,
                    ZipCode = shippingAddress?.zipCode ?? string.Empty,
                }
            }
        };

        var updateOrderRequestUrl = _appSettings.fmebridgeUrl+"orders";
        var body = JsonConvert.SerializeObject(updateOrderRequest);
        await _httpClientService.PutAsync(updateOrderRequestUrl, new StringContent(body, Encoding.UTF8, "application/json"));

        return RedirectToRoute("ScheduleOrderPayment", new { orderId = model.OrderId });
    }

    public  async Task<IActionResult> ScheduleOrderPayment(int orderId)
    {
        // get order
        var requestUrl = _appSettings.fmebridgeUrl+"orders/" + orderId + ",1";
        var response = await _httpClientService.GetAsync(requestUri: requestUrl, requestHeaders: null);
        if (string.IsNullOrEmpty(response.responseResult))
            return RedirectToAction("Index", "Home");

        var scheduleOrderResponse = JsonConvert.DeserializeObject<ScheduleOrderResponse>(response.responseResult);
        if (scheduleOrderResponse == null || scheduleOrderResponse.salesOrder == null)
            return RedirectToAction("Index", "Home");

        var model = new ScheduleOrderDetailsModel
        {
            OrderId = Convert.ToInt32(scheduleOrderResponse.salesOrder.orderId),
            OrderDate = scheduleOrderResponse.salesOrder.orderDate,
            MerchandiseTotal = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.subTotal),
            DeliveryCharge = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.delivery),
            Tax = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.tax),
            TotalSalesOrder = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.invoiceTotal),
            AmountPaid = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.payments),
            AmountDue = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.invoiceTotal) - Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.payments)
        };

        if (scheduleOrderResponse.salesOrder.addresses.Any())
            model.DeliverTo = scheduleOrderResponse.salesOrder.addresses.Where(x => x.addressType == (int)AddressType.Billing).Select(x => new ScheduleOrderDetailsModel.Address
            {
                FirstName = x.name.Split(' ').FirstOrDefault(),
                LastName = x.name.Split(' ').LastOrDefault(),
                Address1 = x.address1,
                Address2 = x.address2,
                City = x.city,
                State = x.state,
                ZipCode = x.zipCode
            }).FirstOrDefault();

        return View(model);
    }

    [HttpPost]
    public  async Task<IActionResult> ScheduleOrderPayment(ScheduleOrderDetailsModel model)
    {
        // get order
        var requestUrl = _appSettings.fmebridgeUrl+"orders/" + model.OrderId + ",1";
        var response = await _httpClientService.GetAsync(requestUri: requestUrl, requestHeaders: null);
        if (string.IsNullOrEmpty(response.responseResult))
            return RedirectToAction("Index", "Home");

        var scheduleOrderResponse = JsonConvert.DeserializeObject<ScheduleOrderResponse>(response.responseResult);
        if (scheduleOrderResponse == null || scheduleOrderResponse.salesOrder == null)
            return RedirectToAction("Index", "Home");

        // submit payemnt
        var paymentRequest = new PaymentRequest
        {
            CCVN = "",
            CreditCardNumber = model.CardPaymentInfo.CardNumber,
            CustomerId = scheduleOrderResponse.salesOrder.customerId,
            EmvToken = "",
            ExpirationMonth = model.CardPaymentInfo.ExpireMonth,
            ExpirationYear = model.CardPaymentInfo.ExpireYear,
            LocationId = "",
            OverrideBlockPayment = true,
            PaymentAmount = Convert.ToDecimal(scheduleOrderResponse.salesOrder.orderTotals.payments),
            PaymentType = "VISA"
        };

        var paymentRequestUrl = _appSettings.fmebridgeUrl+"customers/payment";
        var body = JsonConvert.SerializeObject(paymentRequest);
        var paymentResponse = _httpClientService.PostAsync(paymentRequestUrl, new StringContent(body, Encoding.UTF8, "application/json"));

        if (paymentResponse.Result.Success)
            return RedirectToRoute("ScheduleOrderFulfillment", new { orderId = model.OrderId });
        else
            return RedirectToRoute("ScheduleOrderPayment", new { orderId = model.OrderId });
    }

    public  async Task<IActionResult> ScheduleOrderFulfillment(int orderId)
    {
        // get order
        var requestUrl = _appSettings.fmebridgeUrl+"orders/" + orderId + ",1";
        var response = await _httpClientService.GetAsync(requestUri: requestUrl, requestHeaders: null);
        if (string.IsNullOrEmpty(response.responseResult))
            return RedirectToAction("Index", "Home");

        var scheduleOrderResponse = JsonConvert.DeserializeObject<ScheduleOrderResponse>(response.responseResult);
        if (scheduleOrderResponse == null || scheduleOrderResponse.salesOrder == null)
            return RedirectToAction("Index", "Home");

        var model = new ScheduleOrderDetailsModel
        {
            OrderId = Convert.ToInt32(scheduleOrderResponse.salesOrder.orderId),
        };

        // fulfillment
        var fulfillmentRequest = new FulfillmentRequest
        {
            FulfillmentLocation = scheduleOrderResponse.salesOrder.locationId,
            RouteCode = "",
            RouteStartDate = (DateTime?)null,
            RouteEndDate = (DateTime?)null,
            DeliveryStatus = scheduleOrderResponse.salesOrder.deliveryStatus,
            FullyReservedFulfillmentsOnly = "N",
            RouteType = (int)RouteType.DeliveryRoute
        };

        var fulfillmentRequestUrl = _appSettings.fmebridgeUrl+"orders/fulfillments";
        var body = JsonConvert.SerializeObject(fulfillmentRequest);
        var fulfillmentResponse = _httpClientService.PutAsync(fulfillmentRequestUrl, new StringContent(body, Encoding.UTF8, "application/json"));


        return View(model);
    }

    #endregion
}
