using CategoryFilterParsing;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

Console.WriteLine("What would you like to do?");
Console.WriteLine("1. Get category facets for all categories");
Console.WriteLine("2. Use AI to rank the facets relevancy");
var input = Console.ReadLine();

if (input == "1")
{
    Console.WriteLine("Please enter the file path of the Excel workbook you would like to use:");
    var fileLoc = Console.ReadLine(); // C:\\Users\\Braden\\Downloads\\cats.xlsx
    ExcelFileManager EFM = new ExcelFileManager(fileLoc);
    DataManager DM = new DataManager(EFM.ReadExcelWorkbook());

    DM.ParseData();

    //DM.PrintCategories();

    List<Category> categories = DM.GetCategories().Take(10).ToList(); //For testing, only get the first 10 categories

    //List<Category> categories = DM.GetCategories();

    RequestManager RM = new RequestManager();

    await RM.GetCategoryData(categories);

    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    string filePath = Path.Combine(documentsPath, "CategoryFacetsOutput.txt");

    //Instead of streamwriting to a file, serialize all of this into json output so its easier to read in the file for the chatgpt api.
    /*using (StreamWriter writer = new StreamWriter(filePath))
    {
        foreach (var cat in categories)
        {
            if (cat.categoryFacets != null)
            {
                if (cat.isTopLevel())
                    await writer.WriteLineAsync($"-------------{cat.GetTopLevel()}-------------");
                else
                    await writer.WriteLineAsync($"-------------{cat.GetName()}-------------");

                foreach (var facet in cat.categoryFacets)
                {
                    await writer.WriteLineAsync($"- {facet.ShortDescription} | Number of Products: {facet.Count}");
                }
            }
            else
            {
                await writer.WriteLineAsync($"No category facets found for {cat.GetTopLevel()}");
            }
        }
    }*/

    var categoryDataList = new List<CategoryData>();

    foreach (var cat in categories)
    {
        var categoryData = new CategoryData
        {
            Name = cat.isTopLevel() ? cat.GetTopLevel() : cat.GetName(),
            Facets = cat.categoryFacets?.Select(facet => new FacetData
            {
                ShortDescription = facet.ShortDescription,
                Count = facet.Count
            }).ToList()
        };

        categoryDataList.Add(categoryData);
    }

    string jsonString = JsonSerializer.Serialize(categoryDataList, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(filePath, jsonString);


    Console.WriteLine($"Output written to {filePath}");
}

if(input == "2")
{
    Console.WriteLine("Please enter the file location of your category facet output: ");
    Console.ReadLine(); // C:\\Users\\Braden\\Documents\\CategoryFacetsOutput.txt
}
