using System.Collections.Generic;

namespace Career.Web.Models.StoreManagement;

public record LocationStateListModel
{
    public LocationStateListModel()
    {
        CityListModel = new List<CityModel>();
    }        

    public string StateName { get; set; }

    public string State { get; set; }

    public IList<CityModel> CityListModel { get; set; }

    public class CityModel
    {
        public CityModel()
        {
            PhysicalStoreListModel = new List<LocationModel>();
        }
        
        public string City { get; set; }

        public IList<LocationModel> PhysicalStoreListModel { get; set; }

        public class LocationModel
        {
            public int Id { get; set; }

            public int WebsiteId { get; set; }

            public string StoreUrl { get; set; }

            public string Name { get; set; }
        }            
    }
}
