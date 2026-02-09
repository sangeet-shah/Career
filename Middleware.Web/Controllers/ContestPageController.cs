using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.Klaviyo;
using Middleware.Web.Domains.LandingPages;
using Middleware.Web.Domains.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Middleware.Web.Filters;
using Middleware.Web.Models.LandingPage;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.DeliveryCharges;
using Middleware.Web.Services.Helpers;
using Middleware.Web.Services.LandingPages;
using Middleware.Web.Services.Locations;
using Middleware.Web.Services.Logs;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Messages;
using Middleware.Web.Services.RegistrationPage;
using Middleware.Web.Services.Seo;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using System.Linq;
using System.Text.RegularExpressions;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class ContestPageController : ControllerBase
{
    #region Fields

    private readonly ILandingPageRecordService _landingPageRecordService;
    private readonly ICommonService _commonService;
    private readonly IPictureService _pictureService;
    private readonly ILocationService _locationService;
    private readonly ILandingPageService _landingPageService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IDeliveryChargeService _deliveryChargeService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IRegistrationPageFieldsService _registrationPageFieldsService;
    private readonly ILandingPageClosedService _landingPageClosedService;
    private readonly IKlaviyoService _klaviyoService;
    private readonly ILogService _logService;
    private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public ContestPageController(ILandingPageRecordService landingPageRecordService,
        ICommonService commonService,
        IPictureService pictureService,
        ILocationService locationService,
        ILandingPageService contestService,
        IUrlRecordService urlRecordService,
        IDeliveryChargeService deliveryChargeService,
        IDateTimeHelper dateTimeHelper,
        IRegistrationPageFieldsService registrationPageFieldsService,
        ILandingPageClosedService landingPageClosedService,
        IKlaviyoService klaviyoService,
        ILogService logService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        ISettingService settingService,
        ILandingPageService landingPageService,
        IStoreService storeService)
    {
        _landingPageRecordService = landingPageRecordService;
        _commonService = commonService;
        _pictureService = pictureService;
        _locationService = locationService;
        _urlRecordService = urlRecordService;
        _deliveryChargeService = deliveryChargeService;
        _dateTimeHelper = dateTimeHelper;
        _registrationPageFieldsService = registrationPageFieldsService;
        _landingPageClosedService = landingPageClosedService;
        _klaviyoService = klaviyoService;
        _logService = logService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _settingService = settingService;
        _landingPageService = landingPageService;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    [NonAction]
    public async Task<LandingPageModel> PrepareContestPageLogClosedFormModelAsync(LandingPageModel model, int contestId)
    {
        var closedForm = await _landingPageClosedService.GetClosedFromByLandingPageIdAsync(contestId);
        if (closedForm == null)
            return model;

        model.ClosedFormEnabled = true;
        model.ClosedFormModel.PictureUrl = await _pictureService.GetPictureUrlAsync(closedForm.BannerPictureId, 1280);
        model.ClosedFormModel.MobilePictureUrl = await _pictureService.GetPictureUrlAsync(closedForm.BannerMobilePictureId, 350);
        model.ClosedFormModel.Description = closedForm.Description;
        model.ClosedFormModel.MobileDescription = closedForm.MobileDescription;
        model.ClosedFormModel.EventListEnabled = closedForm.EventListEnabled;
        model.ClosedFormModel.NewsLetterEnabled = closedForm.NewsLetterEnabled;
        model.ClosedFormModel.SMSEnabled = closedForm.SMSEnabled;
        model.ClosedFormModel.TitleEnabled = closedForm.TitleEnabled;

        return model;
    }

    [NonAction]
    public async Task<LandingPageModel> PrepareContestPageModelAsync(LandingPageModel model, LandingPage contest)
    {
        if (contest != null)
        {
            if (contest.LandingPageTypeId == (int)LandingPageTypeEnum.Registration)
            {
                if (contest.EndDateUtc != null)
                {
                    var currentDate = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
                    if (contest.EndDateUtc > currentDate && contest.MaxRegistrationEnabled)
                    {
                        var contestPageLogsCount = await _landingPageRecordService.GetLandingPageRecordsCountBylandingPageIdAsync(contest.Id);
                        if (contest.MaxRegistrations.HasValue && contest.MaxRegistrations.Value <= contestPageLogsCount)
                            await PrepareContestPageLogClosedFormModelAsync(model, contest.Id);
                    }
                    else if (contest.EndDateUtc <= currentDate)
                        await PrepareContestPageLogClosedFormModelAsync(model, contest.Id);
                }
                else
                {
                    if (contest.MaxRegistrationEnabled)
                    {
                        var contestPageLogsCount = await _landingPageRecordService.GetLandingPageRecordsCountBylandingPageIdAsync(contest.Id);
                        if (contest.MaxRegistrations.HasValue && contest.MaxRegistrations.Value <= contestPageLogsCount)
                            await PrepareContestPageLogClosedFormModelAsync(model, contest.Id);
                    }
                }

                if (model.ClosedFormEnabled && !model.ClosedFormModel.NewsLetterEnabled && !model.ClosedFormModel.SMSEnabled)
                    return null;

                if (!model.ClosedFormEnabled)
                {
                    var mobilePicture = await _pictureService.GetPictureByIdAsync(contest.BannerMobilePictureId);
                    model.MobilePictureUrl = mobilePicture != null ? await _pictureService.GetPictureUrlAsync(mobilePicture.Id, 350) : string.Empty;
                    model.MobileDescription = contest.MobileDescription;

                    var registrationPageFields = await _registrationPageFieldsService.GetRegistrationPageFieldsByContestIdAsync(contest.Id);
                    if (registrationPageFields != null)
                        model.LandingPageFormFieldsSortOrder = registrationPageFields.FieldIds;
                }
            }

            if (!model.ClosedFormEnabled)
            {
                model.StartDate = contest.StartDateUtc;
                model.EndDate = contest.EndDateUtc;
                model.PictureUrl = await _pictureService.GetPictureUrlAsync(contest.BannerPictureId);
                model.MetaDescription = contest.MetaDescription;
                model.MetaTitle = contest.MetaTitle;
                model.DisclaimerText = contest.Disclaimer;
                model.ZipCodeVerification = contest.ZipCodeVerification;
                model.Description = contest.Description;
                model.FirstNameEnabled = contest.FirstNameEnabled;
                model.FirstNameRequired = contest.FirstNameRequired;
                model.LastNameEnabled = contest.LastNameEnabled;
                model.LastNameRequired = contest.LastNameRequired;
                model.DateOfBirthEnabled = contest.DOBEnabled;
                model.DateOfBirthRequired = contest.DOBRequired;
                model.EmailAddressEnabled = contest.EmailEnabled;
                model.EmailAddressRequired = contest.EmailRequired;
                model.PhoneNumberEnabled = contest.PhoneNumberEnabled;
                model.PhoneNumberRequired = contest.PhoneNumberRequired;
                model.StreetAddressEnabled = contest.AddressEnabled;
                model.StreetAddressRequired = contest.AddressRequired;
                model.CityEnabled = contest.CityEnabled;
                model.CityRequired = contest.CityRequired;
                model.StateEnabled = contest.StateProvinceEnabled;
                model.StateRequired = contest.StateProvinceRequired;
                model.ZipEnabled = contest.ZipCodeEnabled;
                model.ZipRequired = contest.ZipCodeRequired;
                model.InstagramHandleEnabled = contest.InstagrameHandleEnabled;
                model.InstagramHandleRequired = contest.InstagrameHandleRequired;
                model.TwitterHandleEnabled = contest.TwitterHandleEnabled;
                model.TwitterHandleRequired = contest.TwitterHandleRequired;
                model.StoreDropdownEnabled = contest.LocationEnabled;
                model.StoreDropdownRequired = contest.LocationRequired;

                if (contest.StateProvinceEnabled)
                {
                    var states = await _locationService.GetStateProvincesAsync();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = "State", Value = "0" });
                        foreach (var state in states)
                            model.AvailableStates.Add(new SelectListItem { Text = state.Name, Value = state.Id.ToString(), Selected = (state.Id == model.StateProvinceId) });
                    }
                }

                if (contest.LocationEnabled)
                {
                    model.AvailableStoreLocation.Add(new SelectListItem { Text = "Choose you local store", Value = "0" });
                    var physicalStores = await _locationService.GetLocationsAsync(((int)WebsiteEnum.FMUSA));
                    foreach (var physicalStore in physicalStores)
                    {
                        model.AvailableStoreLocation.Add(new SelectListItem
                        {
                            Text = physicalStore.Name,
                            Value = physicalStore.Id.ToString(),
                            Selected = (physicalStore.Id == model.LocationId)
                        });
                    }
                }
            }
        }

        model.Title = contest.Title;
        model.DisplayTitle = contest.DisplayTitle;
        model.NewsLetterEnabled = contest.NewsLetterEnabled;
        model.EventListEnabled = contest.EventListEnabled;
        model.SMSEnabled = contest.SMSEnabled;
        model.EventFlowEnabled = contest.EventFlowEnabled;
        model.EventFlow = contest.EventFlow;
        model.LandingPageTypeId = contest.LandingPageTypeId;
        model.EmailSubscribed = true;
        model.SubscribedEventList = true;

        return model;
    }

    protected async Task<Dictionary<string, string>> CustomModelStateErrorAsync(LandingPageModel model, LandingPage contest)
    {
        var error = new Dictionary<string, string>();
        if (!model.ClosedFormEnabled && contest.FirstNameEnabled && contest.FirstNameRequired)
        {
            if (model.FirstName == null)
                error.Add(nameof(model.FirstName), "First name is required");
        }

        if (!model.ClosedFormEnabled && contest.LastNameEnabled && contest.LastNameRequired)
        {
            if (model.LastName == null)
                error.Add(nameof(model.LastName), "Last name is required");
        }

        if (!model.ClosedFormEnabled && contest.DOBEnabled && contest.DOBRequired)
        {
            if (model.DateOfBirth == null)
                error.Add(nameof(model.DateOfBirth), "Date of birth is required");
            else
            {
                var ageLimit = 18;
                if (NopDefaults.GetDifferenceInYears(model.DateOfBirth.Value, DateTime.Today) < ageLimit)
                    error.Add(nameof(model.DateOfBirth), $"Must be {ageLimit} years or older to be eligible for this registration.");
            }
        }

        if ((contest.EmailEnabled && contest.EmailRequired) || model.EmailSubscribed || model.SubscribedEventList || model.Email != null)
        {
            if ((model.EmailSubscribed || (!model.ClosedFormEnabled && contest.EmailEnabled && contest.EmailRequired)) && model.Email == null)
                error.Add(nameof(model.Email), "Valid email address is required.");
            else if (model.SubscribedEventList && model.Email == null && model.Phone == null)
                error.Add(nameof(model.Email), "Please provide email or phone to get event notification.");
            else if (model.Email != null)
            {
                Regex re = new Regex(NopDefaults.EmailValidationExpression);
                if (!re.IsMatch(model.Email))
                    error.Add(nameof(model.Email), "Wrong email.");
            }
        }

        if ((contest.PhoneNumberEnabled && contest.PhoneNumberRequired) || model.SMSSubscribed || model.SubscribedEventList || model.Phone != null)
        {
            if ((model.SMSSubscribed || (!model.ClosedFormEnabled && contest.PhoneNumberEnabled && contest.PhoneNumberRequired)) && model.Phone == null)
                error.Add(nameof(model.Phone), "Phone number is required.");
            else if (model.SubscribedEventList && model.Phone == null && model.Email == null)
                error.Add(nameof(model.Phone), "Please provide email or phone to get event notification.");
            else if (model.Phone != null)
            {
                string phoneRegex = @"^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$";
                Regex re = new Regex(phoneRegex);
                if (!re.IsMatch(model.Phone))
                    error.Add(nameof(model.Phone), "This phone number is invalid or not from USA.");
            }
        }

        if (!model.ClosedFormEnabled && contest.AddressEnabled && contest.AddressRequired)
        {
            if (model.StreetAddress == null)
                error.Add(nameof(model.StreetAddress), "Street address is required.");
        }

        if (!model.ClosedFormEnabled && contest.CityEnabled && contest.CityRequired)
        {
            if (model.City == null)
                error.Add(nameof(model.City), "City is required.");
        }

        if (!model.ClosedFormEnabled && contest.StateProvinceEnabled && contest.StateProvinceRequired)
        {
            if (model.StateProvinceId == 0)
                error.Add(nameof(model.StateProvinceId), "State is required.");
        }

        if (!model.ClosedFormEnabled && contest.LocationEnabled && contest.LocationRequired)
        {
            if (model.LocationId == 0)
                error.Add(nameof(model.LocationId), "Local store is required.");
        }

        if (!model.ClosedFormEnabled && contest.ZipCodeEnabled && contest.ZipCodeRequired)
        {
            if (model.ZipPostalCode == null)
                error.Add(nameof(model.ZipPostalCode), "Zip is required.");
            else
            {
                if (contest.ZipCodeVerification)
                {
                    var deliveryCharge = await _deliveryChargeService.GetDeliveryChargeByZipPostalCodeAsync(model.ZipPostalCode);
                    if (deliveryCharge == null)
                        error.Add(nameof(model.ZipPostalCode), "We're sorry, the zip code you entered is not within our delivery area.");
                }
            }
        }

        if (!model.ClosedFormEnabled && contest.InstagrameHandleEnabled && contest.InstagrameHandleRequired)
        {
            if (model.InstagramHandle == null)
                error.Add(nameof(model.InstagramHandle), "Instagram handle is required.");
        }

        if (!model.ClosedFormEnabled && contest.TwitterHandleEnabled && contest.TwitterHandleRequired)
        {
            if (model.TwitterHandle == null)
                error.Add(nameof(model.TwitterHandle), "Twitter handle is required.");
        }

        return error;
    }

    #endregion

    #region Method

    [HttpGet("{contestId}")]
    public async Task<IActionResult> ContestPage(int contestId)
    {
        var contest = await _landingPageService.GetlandingPageByIdAsync(contestId);
        var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
        if (contest == null)
            return NotFound();
        if (!contest.Published)
            return NotFound();
        if (contest.StartDateUtc != null && contest.StartDateUtc >= currentDate)
            return NotFound();
        if (contest.EndDateUtc != null && contest.EndDateUtc <= currentDate && contest.LandingPageTypeId == (int)LandingPageTypeEnum.Contest)
            return NotFound();
        if (contest.StoreId != 0 && contest.StoreId != (await _storeService.GetCurrentStoreAsync()).Id)
            return NotFound();

        var seName = await _urlRecordService.GetSeNameAsync(contest.Id, nameof(LandingPage));
        var model = new LandingPageModel { LandingPageId = contest.Id, SeName = seName };
        model = await PrepareContestPageModelAsync(model, contest);

        if (model == null)
            return NotFound();

        return Ok(new ContestPageResponseModel { ShowResult = false, Model = model });
    }

    [HttpPost("{contestId}")]
    public async Task<IActionResult> ContestPage(int contestId, [FromBody] LandingPageModel model)
    {
        var contest = await _landingPageService.GetlandingPageByIdAsync(contestId);
        var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
        if (contest == null)
            return NotFound();
        if (!contest.Published)
            return NotFound();
        if (contest.StartDateUtc != null && contest.StartDateUtc >= currentDate)
            return NotFound();
        if (contest.EndDateUtc != null && contest.EndDateUtc <= currentDate && contest.LandingPageTypeId == (int)LandingPageTypeEnum.Contest)
            return NotFound();

        foreach (var customError in await CustomModelStateErrorAsync(model, contest))
            ModelState.AddModelError(customError.Key, customError.Value);

        var validationErrors = ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value?.Errors.First().ErrorMessage ?? string.Empty);

        if (ModelState.IsValid)
        {
            var contestPageLog = new LandingPageRecord
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DOB = model.DateOfBirth.HasValue ? model.DateOfBirth.Value : (DateTime?)null,
                Email = !string.IsNullOrEmpty(model.Email) ? model.Email.ToLower() : null,
                PhoneNumber = model.Phone,
                Address = model.StreetAddress,
                City = model.City,
                StateProvinceId = model.StateProvinceId > 0 ? model.StateProvinceId : null,
                ZipCode = model.ZipPostalCode,
                InstagramHandle = model.InstagramHandle,
                TwitterHandle = model.TwitterHandle,
                CreatedOnUtc = DateTime.UtcNow,
                StoreId = (await _storeService.GetCurrentStoreAsync()).Id,
                LandingPageId = contest.Id,
                LocationId = model.LocationId == 0 ? null : model.LocationId
            };
            var klaviyoSettings = await _settingService.LoadSettingAsync<KlaviyoSettings>((await _storeService.GetCurrentStoreAsync())?.Id ?? 0);

            if (klaviyoSettings.Enable && !string.IsNullOrEmpty(klaviyoSettings.PrivateAPIKey))
            {
                if (!string.IsNullOrEmpty(klaviyoSettings.NewsLetterListId) && !string.IsNullOrEmpty(model.Email) && model.NewsLetterEnabled)
                {
                    model.Email = model.Email.Trim();
                    var userExist = await _klaviyoService.IsKlaviyoProfileExistByEmailAsync(model.Email, klaviyoSettings.PrivateAPIKey, klaviyoSettings.NewsLetterListId);
                    if (userExist && !model.EmailSubscribed)
                    {
                        var responseErrorMessage = await _klaviyoService.UnSubscribeEmailAsync(model.Email, klaviyoSettings.PrivateAPIKey, klaviyoSettings.NewsLetterListId);
                        if (!string.IsNullOrEmpty(responseErrorMessage))
                        {
                            _logService.Error(responseErrorMessage);
                            contestPageLog.EmailSubscribed = true;
                        }
                        else
                            contestPageLog.EmailSubscribed = model.EmailSubscribed;
                    }
                    else if (!userExist && model.EmailSubscribed)
                    {
                        var responseErrorMessage = await _klaviyoService.SubscribeEmailAsync(model.Email, klaviyoSettings.PrivateAPIKey, klaviyoSettings.NewsLetterListId);
                        if (!string.IsNullOrEmpty(responseErrorMessage))
                        {
                            _logService.Error(responseErrorMessage);
                            contestPageLog.EmailSubscribed = false;
                        }
                        else
                            contestPageLog.EmailSubscribed = model.EmailSubscribed;
                    }
                    else if (userExist && model.EmailSubscribed)
                        contestPageLog.EmailSubscribed = model.EmailSubscribed;

                    if (!(!userExist && !model.EmailSubscribed))
                    {
                        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(model.Email, (await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
                        if (subscription == null)
                        {
                            var newsLetterSubscription = new NewsLetterSubscription
                            {
                                NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                Email = model.Email,
                                Active = model.EmailSubscribed,
                                StoreId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0,
                                CreatedOnUtc = DateTime.UtcNow,
                            };
                            await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(newsLetterSubscription);
                        }
                        else
                        {
                            subscription.Active = model.EmailSubscribed;
                            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(klaviyoSettings.SMSListId) && !string.IsNullOrEmpty(model.Phone) && model.SMSEnabled)
                {
                    model.Phone = model.Phone.Trim();
                    var userExist = await _klaviyoService.IsKlaviyoProfileExistByPhoneAsync(model.Phone, klaviyoSettings.PrivateAPIKey, klaviyoSettings.SMSListId);
                    if (userExist && !model.SMSSubscribed)
                    {
                        var responseErrorMessage = await _klaviyoService.UnSubscribeSMSAsync(model.Phone, klaviyoSettings.PrivateAPIKey, klaviyoSettings.SMSListId);
                        if (!string.IsNullOrEmpty(responseErrorMessage))
                        {
                            contestPageLog.SMSSubscribed = true;
                            _logService.Error(responseErrorMessage);
                        }
                        else
                            contestPageLog.SMSSubscribed = model.SMSSubscribed;
                    }
                    else if (!userExist && model.SMSSubscribed)
                    {
                        var responseErrorMessage = await _klaviyoService.SubscribeSMSAsync(model.Phone, klaviyoSettings.PrivateAPIKey, klaviyoSettings.SMSListId);
                        if (!string.IsNullOrEmpty(responseErrorMessage))
                        {
                            contestPageLog.SMSSubscribed = false;
                            _logService.Error(responseErrorMessage);
                        }
                        else
                            contestPageLog.SMSSubscribed = model.SMSSubscribed;
                    }
                    else if (userExist && model.SMSSubscribed)
                        contestPageLog.SMSSubscribed = model.SMSSubscribed;
                }

                if (model.SubscribedEventList && (!string.IsNullOrEmpty(model.Email) || !string.IsNullOrEmpty(model.Phone)))
                {
                    if (!string.IsNullOrEmpty(klaviyoSettings.EventListId))
                    {
                        var profileProperties = new Dictionary<string, object>
                        {
                            { "email", model.Email }
                        };
                        await _klaviyoService.AddToListAsync(listId: klaviyoSettings.EventListId, privateAPIKey: klaviyoSettings.PrivateAPIKey, profileProperties: profileProperties);

                        var responseErrorMessage = await _klaviyoService.SubscribeEventListAsync(model.FirstName, model.LastName, model.Email, model.Phone, model.ZipPostalCode, model.City, model.StateProvinceId, model.EventFlow, klaviyoSettings.PrivateAPIKey, klaviyoSettings.EventListId);
                        if (!string.IsNullOrEmpty(responseErrorMessage))
                            _logService.Error(responseErrorMessage);
                        else
                            contestPageLog.EmailSubscribed = model.EmailSubscribed;
                    }

                    if (!string.IsNullOrEmpty(klaviyoSettings.EventNewsletterListId))
                    {
                        var profileProperties = new Dictionary<string, object>
                        {
                            { "email", model.Email }
                        };
                        await _klaviyoService.AddToListAsync(listId: klaviyoSettings.EventNewsletterListId, privateAPIKey: klaviyoSettings.PrivateAPIKey, profileProperties: profileProperties);
                    }
                }
            }

            await _landingPageRecordService.InsertLandingPageRecordAsync(contestPageLog);
            model = await PrepareContestPageModelAsync(model, contest);
            return Ok(new ContestPageResponseModel
            {
                ShowResult = true,
                Model = model,
                Errors = validationErrors
            });
        }

        model.ContestId = contestId;
        model.LandingPageId = contestId;
        model = await PrepareContestPageModelAsync(model, contest);
        return Ok(new ContestPageResponseModel
        {
            ShowResult = false,
            Model = model,
            Errors = validationErrors
        });
    }

    #endregion
}
