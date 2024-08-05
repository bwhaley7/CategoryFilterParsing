using CategoryFilterParsing;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

Console.WriteLine("What would you like to do?");
Console.WriteLine("1. Get category facets for all categories");
Console.WriteLine("2. Use AI to rank the facets relevancy");
var input = Console.ReadLine();
Console.Clear();

if (input == "1")
{
    Console.WriteLine("Please enter the file path of the Excel workbook you would like to use:");
    var fileLoc = Console.ReadLine(); // C:\\Users\\Braden\\Downloads\\cats.xlsx
    ExcelFileManager EFM = new ExcelFileManager(fileLoc);
    DataManager DM = new DataManager(EFM.ReadExcelWorkbook());

    DM.ParseData();

    //DM.PrintCategories();

    //List<Category> categories = DM.GetCategories().Take(10).ToList(); //For testing, only get the first 10 categories

    List<Category> categories = DM.GetCategories();

    RequestManager RM = new RequestManager();

    await RM.GetCategoryData(categories);

    string filePath = Path.Combine(documentsPath, "CategoryFacetsOutput.txt");

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

    //The text file is in JSON format. We need to read the JSON into a list of CategoryData objects. We will then use these objects to rank the facets via openAI api.

    string jsonString = File.ReadAllText("C:\\Users\\Braden\\Documents\\CategoryFacetsOutput.txt");
    List<CategoryData> categoryDataList = JsonSerializer.Deserialize<List<CategoryData>>(jsonString);
    StringBuilder prompt = new StringBuilder();
    OpenAIManager OAM = new OpenAIManager();

    foreach (var category in categoryDataList)
    {
        prompt.AppendLine($"Search Term: {category.Name}");
        prompt.AppendLine("Category Facets: ");
        foreach(var facet in category.Facets)
        {
            prompt.AppendLine(facet.ShortDescription);
        }
        var response = await OAM.GetChatCompletionAsync(prompt.ToString());

        Console.WriteLine(response);

        prompt.Clear();
    }
}
