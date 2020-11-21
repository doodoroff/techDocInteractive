using System;
using System.Collections.Generic;

namespace TechDocInteractive
{
    class ExcelToolInfoReader
    {
        ExcelRedactor excelRedactor;
        List<ExcelTool> excelToolList;

        int toolLocationColumnNumber,
            toolQuantityColumnNumber,
            toolNameColumnNumber;

        public ExcelToolInfoReader(string excelFilePath)
        {
            this.excelRedactor = new ExcelRedactor(excelFilePath);
            this.excelToolList = new List<ExcelTool>();
            this.toolLocationColumnNumber = SetColumnNumbers("Вид хранения");
            this.toolQuantityColumnNumber = SetColumnNumbers("Остаток");
            this.toolNameColumnNumber = SetColumnNumbers("Наименование");
            GetToolInfoFromExcel();
        }

        int SetColumnNumbers(string header)
        {
            string nextNameCell = " ";
            int columnNumber = 2;
            int headerRow = 2;
            while (nextNameCell != null)
            {
                if (excelRedactor.GetCellValue(headerRow, columnNumber).Equals(header))
                {
                    return columnNumber;
                }
                columnNumber++;
            }
            throw new AppXmlAnalyzerExceptions("Столбец *" + header + "* не найден");
        }

        void GetToolInfoFromExcel()
        {
            string toolName = " ";
            string storageBalance;
            string productionBalance;
            int row = 3;
            string nextNameCell = " ";

            while (nextNameCell != null)
            {
                ExcelTool excelTool = new ExcelTool();
                string insertLocation = excelRedactor.GetCellValue(row, toolLocationColumnNumber);

                if (insertLocation.Equals("Обычное хранение"))
                {
                    storageBalance = excelRedactor.GetCellValue(row, toolQuantityColumnNumber);
                    excelTool.StorageQuantity = int.Parse(storageBalance);
                }
                else
                {
                    productionBalance = excelRedactor.GetCellValue(row, toolQuantityColumnNumber);
                    excelTool.ProductionQuantity = int.Parse(productionBalance);
                }
                toolName = excelRedactor.GetCellValue(row, toolNameColumnNumber);
                excelTool.ToolName = toolName;

                excelToolList.Add(excelTool);

                row++;
                nextNameCell = excelRedactor.GetCellValue(row + 1, toolNameColumnNumber);
            }
            excelRedactor.Close();
        }

        public List<ExcelTool> ExcelToolList
        {
            get { return excelToolList; }
        }
            
    }
}