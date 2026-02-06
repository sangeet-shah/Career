namespace Career.Data.Domains;

public class Setting : BaseEntity
{
    /// <summary>
    /// Gets or sets name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets storeid
    /// </summary>
    public int StoreId { get; set; }
}
