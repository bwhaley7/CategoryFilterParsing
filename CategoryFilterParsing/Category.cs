using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
namespace CategoryFilterParsing
{
    internal class Category
    {
        public List<CategoryFacets> categoryFacets;

        private string TopLevel, Name;

        public Category(string topLevel)
        {
            this.TopLevel = topLevel;
            this.Name = "";
            this.categoryFacets = new List<CategoryFacets>();
        }

        public Category(string topLevel, string name)
        {
            this.TopLevel = topLevel;
            this.Name = name;
            this.categoryFacets = new List<CategoryFacets>();
        }

        public string GetName()
        {
            return this.Name;
        }

        public string GetTopLevel() {
            return this.TopLevel;
        }

        public bool isTopLevel()
        {
            return string.IsNullOrEmpty(this.Name);
        }
    }
}
