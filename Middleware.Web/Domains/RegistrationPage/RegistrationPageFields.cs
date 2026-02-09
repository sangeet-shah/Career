namespace Middleware.Web.Domains.RegistrationPage;

/// <summary>
/// Represents a registration page fields
/// </summary>
public class RegistrationPageFields : BaseEntity
{
    /// <summary>
    /// Gets or sets the contest id
    /// </summary>
    public int ContestId { get; set; }

    /// <summary>
    /// Gets or sets the field ids
    /// </summary>
    public string FieldIds { get; set; }
}
