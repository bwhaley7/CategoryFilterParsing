using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
//
namespace CategoryFilterParsing
{
    public class CategoryFacets
    {
        [JsonPropertyName("categoryId")]
        public string CategoryId { get; set; }

        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("selected")]
        public bool Selected { get; set; }
    }

    public class ApiResponse
    {
        [JsonPropertyName("categoryFacets")]
        public List<CategoryFacets> CategoryFacets { get; set; }
    }

    public class CategoryData
    {
        public string Name { get; set; }
        public List<FacetData> Facets { get; set; }
    }

    public class FacetData
    {
        public string ShortDescription { get; set; }
        public int Count { get; set; }
    }
}
