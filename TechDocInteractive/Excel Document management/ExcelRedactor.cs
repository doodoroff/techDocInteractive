using System;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;

namespace TechDocInteractive
{
    class ExcelRedactor
    {
        //Excel.Application excelApp = null;
        //Excel.Workbook workbook = null;
        //Excel.Worksheet worksheet = null;

        ExcelPackage excelPackage;
        ExcelWorksheet worksheet = null;

        public ExcelRedactor()
        {
            excelPackage = new ExcelPackage();
            worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

            //excelApp = new Excel.Application();
            //workbook = excelApp.Workbooks.Add();
            //worksheet = workbook.Worksheets.Item[1];
        }

        public ExcelRedactor(string filePath)
        {
            FileInfo excelFilePath = new FileInfo(filePath);
            excelPackage = new ExcelPackage(excelFilePath);
            worksheet = excelPackage.Workbook.Worksheets[1];

            //excelApp = new Excel.Application();
            //workbook = excelApp.Workbooks.Open(Filename: filePath, ReadOnly: true);
            //worksheet = workbook.Worksheets.Item[1];
        }

        /*public bool Visible
        {
            get { return excelApp.Visible; }

            set { excelApp.Visible = value; }
        }*/

        public void CellAutoFormat()
        {
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        public void SetCellValue(string cellValue, int rowIndex, int columnIndex)
        {
            worksheet.Cells[rowIndex, columnIndex].Value = cellValue;
        }

        public string GetCellValue(int rowIndex, int columnIndex)
        {
            /*Excel.Range cell = worksheet.Cells[rowIndex, columnIndex];
            var cellValue = cell.Value2;
            return Convert.ToString(cellValue);  //cell.Value2;*/
            object cellValue = worksheet.Cells[rowIndex, columnIndex].Value;

            if (cellValue == null) 
            {
                return null;
            }
            else
            {
                return cellValue.ToString();
            }

        }

        public void MergeCells(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            try
            {
                worksheet.Cells[fromRow, fromColumn, toRow, toColumn].Merge = true;
            }
            catch (Exception)
            {
                throw new AppXmlAnalyzerExceptions("Не удалось объединить ячейки заголовка отчёта");
            }

        }

        public void SetBorderStyle(int row, int column, ReportBorderStyle borderStyle)
        {
            OfficeOpenXml.Style.ExcelBorderStyle excelBorderStyle;

            switch (borderStyle)
            {
                case ReportBorderStyle.Thin:
                    {
                        excelBorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    break;
                case ReportBorderStyle.Medium:
                    {
                        excelBorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    }
                    break;

                default:
                    {
                        excelBorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.None;
                    }
                    break;                    
            }

            worksheet.Cells[row, column].Style.Border.BorderAround(excelBorderStyle);
        }

        public void SetBorderStyle(int fromRow, int fromColumn, int toRow, int toColumn, ReportBorderStyle borderStyle)
        {
            OfficeOpenXml.Style.ExcelBorderStyle excelBorderStyle;

            switch (borderStyle)
            {
                case ReportBorderStyle.Thin:
                    {
                        excelBorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    break;
                case ReportBorderStyle.Medium:
                    {
                        excelBorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    }
                    break;

                default:
                    {
                        excelBorderStyle = OfficeOpenXml.Style.ExcelBorderStyle.None;
                    }
                    break;
            }

            worksheet.Cells[fromRow, fromColumn, toRow, toColumn].Style.Border.BorderAround(excelBorderStyle);
        }

        public byte[] CloseAndPackAsByteArray()
        {
            return excelPackage.GetAsByteArray();
        }

        public void Close()
        {
            /*workbook.Close();
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            excelApp = null;
            workbook = null;
            worksheet = null;*/
            GC.Collect();
        }
    }
}
