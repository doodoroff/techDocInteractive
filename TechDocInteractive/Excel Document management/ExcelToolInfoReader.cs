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
            toolNameColumnNumber,
            storageCodeColumnNumber,
            omegaCodeColumnNumber;
        //string productionStorageCode = "1223";
        //string centralStorageCode = "1212";

        public ExcelToolInfoReader(string excelFilePath)
        {
            this.excelRedactor = new ExcelRedactor(excelFilePath);
            this.excelToolList = new List<ExcelTool>();
            this.toolLocationColumnNumber = SetColumnNumbers("Вид хранения");
            this.toolQuantityColumnNumber = SetColumnNumbers("Остаток");
            this.toolNameColumnNumber = SetColumnNumbers("Наименование");
            this.storageCodeColumnNumber = SetColumnNumbers("Подразделение");
            this.omegaCodeColumnNumber = SetColumnNumbers("Обозначение");
            GetToolInfoFromExcel();
        }

        int SetColumnNumbers(string header)
        {
            string nextNameCell = " ";
            int columnNumber = 2;
            int headerRow = 2;
            while (nextNameCell != "")
            {
                if (excelRedactor.GetCellValue(headerRow, columnNumber).Equals(header))
                {
                    return columnNumber;
                }
                nextNameCell = excelRedactor.GetCellValue(headerRow, columnNumber + 1);
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
                string toolLocation = excelRedactor.GetCellValue(row, toolLocationColumnNumber);
                string storageCode = excelRedactor.GetCellValue(row, storageCodeColumnNumber);

                if (toolLocation.Equals("Обычное хранение") && storageCode.Equals("1223"))
                {
                    storageBalance = excelRedactor.GetCellValue(row, toolQuantityColumnNumber);
                    excelTool.ProductionStorageQuantity = int.Parse(storageBalance);
                }
                else if (toolLocation.Equals("Обычное хранение") && storageCode.Equals("1212"))
                {
                    storageBalance = excelRedactor.GetCellValue(row, toolQuantityColumnNumber);
                    excelTool.CentralStorageQuantity = int.Parse(storageBalance);
                }
                else
                {
                    productionBalance = excelRedactor.GetCellValue(row, toolQuantityColumnNumber);
                    excelTool.ProductionQuantity = int.Parse(productionBalance);
                }
                excelTool.ToolName = excelRedactor.GetCellValue(row, toolNameColumnNumber);
                excelTool.StorageCode = excelRedactor.GetCellValue(row, storageCodeColumnNumber);
                excelTool.OmegaCode = excelRedactor.GetCellValue(row, omegaCodeColumnNumber);
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