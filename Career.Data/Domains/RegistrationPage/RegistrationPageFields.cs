namespace Career.Data.Domains.RegistrationPage;

/// <summary>
/// Represents a registration page fields
/// </summary>
public partial class RegistrationPageFields : BaseEntity
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
