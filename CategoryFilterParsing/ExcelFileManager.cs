using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace CategoryFilterParsing
{
    internal class ExcelFileManager
    {
        private string filePath;

        public ExcelFileManager()
        {
            this.filePath = "";
        }
        public ExcelFileManager(string filePath)
        {
            Console.WriteLine(filePath);
            this.filePath = filePath;
        }

        public object[,] ReadExcelWorkbook()
        {
            var excelApp = new Excel.Application();

            Excel.Workbook wb = excelApp.Workbooks.Open(this.filePath);
            Excel.Worksheet ws = (Excel.Worksheet)wb.Sheets[1];

            Excel.Range range = ws.UsedRange;
            string sheetName = ws.Name;

            object[,] cellValues = (object[,])range.Value2;

            Console.WriteLine($"Sheet Name: {sheetName}, size: {range.Rows.Count}x{range.Columns.Count}");
            Console.WriteLine($"Object Size: {cellValues.GetLength(0)}x{cellValues.GetLength(1)}");

            wb.Close();
            excelApp.Quit();

            return cellValues;
        }
    }
}
