using System;
using Excel = Microsoft.Office.Interop.Excel;

namespace TechDocInteractive
{
    class ExcelRedactor
    {
        Excel.Application excelApp = null;
        Excel.Workbook workbook = null;
        Excel.Worksheet worksheet = null;

        public ExcelRedactor()
        {
            excelApp = new Excel.Application();
            workbook = excelApp.Workbooks.Add();
            worksheet = workbook.Worksheets.Item[1];
        }

        public ExcelRedactor(string filePath)
        {
            excelApp = new Excel.Application();
            workbook = excelApp.Workbooks.Open(Filename: filePath, ReadOnly: true);
            worksheet = workbook.Worksheets.Item[1];
        }

        public bool Visible
        {
            get { return excelApp.Visible; }

            set { excelApp.Visible = value; }
        }

        public void CellAutoFit()
        {
            worksheet.Columns.EntireColumn.AutoFit();
        }

        public void SetCellValue(string cellValue, int rowIndex, int columnIndex)
        {
            worksheet.Cells[rowIndex, columnIndex] = cellValue;
        }

        public string GetCellValue(int rowIndex, int columnIndex)
        {
            Excel.Range cell = worksheet.Cells[rowIndex, columnIndex];
            var cellValue = cell.Value2;
            return Convert.ToString(cellValue);  //cell.Value2;
        }

        public void Close()
        {
            workbook.Close();
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            excelApp = null;
            workbook = null;
            worksheet = null;
            GC.Collect();
        }
    }
}
