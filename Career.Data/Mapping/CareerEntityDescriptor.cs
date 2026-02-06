using System.Collections.Generic;

namespace Career.Data.Mapping;

public class CareerEntityDescriptor
{
    public CareerEntityDescriptor()
    {
        Fields = new List<CareerEntityFieldDescriptor>();
    }

    public string EntityName { get; set; }

    public string SchemaName { get; set; }

    public ICollection<CareerEntityFieldDescriptor> Fields { get; set; }
}
