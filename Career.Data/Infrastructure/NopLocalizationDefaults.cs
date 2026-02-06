namespace Career.Data.Infrastructure;

public class NopLocalizationDefaults
{
    /// <summary>
    /// Gets a prefix of locale resources for enumerations 
    /// </summary>
    public static string EnumLocaleStringResourcesPrefix => "Enums.";

	public const string PUBLIC_LEGAL_PAGE_TITLE = $"FM.Plugin.Core.LegalPages.{{0}}.Title";

}

public static class ContentManagement
{
	public const string GENERIC_ATTRIBUTE_KEY_GROUP = "LegalPage";
	public const string GENERIC_ATTRIBUTE_KEY_BODY = "LegalPage.Body";
	public const string GENERIC_ATTRIBUTE_KEY_MOBILE_BODY = "LegalPage.MobileBody";
}