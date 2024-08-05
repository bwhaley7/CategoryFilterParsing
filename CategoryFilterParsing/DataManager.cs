using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoryFilterParsing
{
    internal class DataManager
    {
        private object[,] excelData;
        private List<Category> categories;

        public DataManager(object[,] excelData)
        {
            this.excelData = excelData;
        }

        public void ParseData()
        {
            categories = new List<Category>();

            if(excelData == null)
            {
                Console.WriteLine("Excel data is null");
                return;
            }

            for(int row=2; row <= excelData.GetLength(0); row++)
            {
                for(int col=1; col <= excelData.GetLength(1); col++)
                {
                    if (excelData[row, col] != null)
                    {
                        var tempString = excelData[row, col].ToString();
                        switch (col)
                        {
                            case 1:
                                if (!CategoryExists(tempString))
                                {
                                    Category newCategory = new Category(tempString);
                                    categories.Add(newCategory);
                                }
                                break;
                            default:
                                AddSubCategory(tempString, row, col);
                                break;

                        }
                    }
                }
            }
            Console.WriteLine($"{categories.Count} Categories created.");
        }

        private void AddSubCategory(string tempString, int row, int col)
        {
            var topLevel = excelData[row, col - (col-1)].ToString();
            if (!string.IsNullOrEmpty(tempString) && !SubCategoryExists(tempString))
            {
                Category newCategory = new Category(topLevel,tempString);
                categories.Add(newCategory);

            }
        }

        private bool CategoryExists(string topLevelName)
        {
            if (categories.Count == 0)
                return false;

            return categories.Any(c => c.GetTopLevel() == topLevelName);
        }

        private bool SubCategoryExists(string name)
        {
            if (categories.Count == 0)
                return false;
            return categories.Any(c => c.GetName() == name);
        }

        public void PrintCategories()
        {
            foreach(var cat in categories)
            {
                if(cat.isTopLevel())
                    Console.WriteLine($"Top Level: {cat.GetTopLevel()}");
                else
                    Console.WriteLine($"Top Level: {cat.GetTopLevel()} | Category: {cat.GetName()}");
            }
        }

        public List<Category> GetCategories()
        {
            return categories;
        }
    }
}
