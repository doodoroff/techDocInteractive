using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TechDocInteractive
{
    class ToolInfoPresenter
    {
        public event EventHandler FlagInsertDataProcessingStart;
        public event EventHandler FlagInsertDataProcessingStop;

        readonly string xmlFilePath;
        readonly string toolBaseFilePath;
        readonly string insertBaseFilePath;
        readonly string colletBaseFilePath;

        SpCamXMLReader spCamXMLReader;
        SpCamToolBaseReader spCamToolBaseReader;
        //ExcelToolInfoReader excelToolInfoReader;
        //CsvRedactor CsvRedactor;
        List<Tool> projectTools;
        Operation operationDescription;
        List<ToolInfo> toolInfoContainer;
        List<ExcelTool> inserts;
        List<ExcelTool> collets;
        //IEnumerable<Tool> distinctProjectTools;

        public ToolInfoPresenter(string toolBaseFilePath, string xmlFilePath, string insertBaseFilePath = null, string colletBaseFilePath = null)
        {
            this.xmlFilePath = xmlFilePath;
            this.toolBaseFilePath = toolBaseFilePath;
            this.insertBaseFilePath = insertBaseFilePath;
            this.colletBaseFilePath = colletBaseFilePath;

            spCamXMLReader = new SpCamXMLReader(xmlFilePath);
            spCamToolBaseReader = new SpCamToolBaseReader(toolBaseFilePath);
        }

        public void InitializeAndStartDataProcessing()
        {
            OnFlagInsertDataProcessingEvent(FlagInsertDataProcessingStart);

            //InitializeDataProcessing();

            projectTools = spCamXMLReader.ProjectToolList;
            operationDescription = spCamXMLReader.OperationDescription;

            toolInfoContainer = new List<ToolInfo>();

            AddHolderNamesToTools();
            AddCsvDataToTools();

            try
            {
                inserts = new ExcelToolInfoReader(insertBaseFilePath).ExcelToolList;
                collets = new ExcelToolInfoReader(colletBaseFilePath).ExcelToolList;
            }
            catch (Exception)
            {
                inserts = null;
                collets = null;
                //throw new AppXmlAnalyzerExceptions("Файл с базой пластин не найден");
            }
            finally
            {
                PerformToolInfo();
            }

            OnFlagInsertDataProcessingEvent(FlagInsertDataProcessingStop);
        }

        /*void InitializeDataProcessing()
        {


            spCamXMLReader = new SpCamXMLReader(xmlFilePath);
            spCamToolBaseReader = new SpCamToolBaseReader(toolBaseFilePath);


        }*/

        void OnFlagInsertDataProcessingEvent(EventHandler eventHandler)
        {
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        /*void AddHolderNamesToTools() // TO DO: refactoring
        {
            SpCamToolBaseReader spCamToolBaseReader = new SpCamToolBaseReader(toolBaseFilePath);
            var holderList = spCamToolBaseReader.ReadHoldersList();

            foreach (var currentTool in projectTools)
            {
                CsvRedactor.AddInfoFromBaseToTool(currentTool);
                Holder holder = holderList.Find(h => h.HolderCode.Equals(currentTool.HolderCode));
                if (holder != null)
                {
                    currentTool.HolderName = holder.Name;
                }
                else
                {
                    currentTool.HolderName = "Неизвестная оправка";
                }
            }
        }*/

        void AddHolderNamesToTools() // TO DO: refactoring
        {
            var holderList = spCamToolBaseReader.GetHolderList();

            foreach (var currentTool in projectTools)
            {
                Holder holder = holderList.Find(h => h.HolderCode.Equals(currentTool.HolderCode));
                if (holder != null)
                {
                    currentTool.HolderName = holder.Name;
                    currentTool.HolderSpindelSideInterface = holder.FromSpindelSideInterface;
                    currentTool.HolderCutSideInterface = holder.FromCutSideInterface;
                }
                else
                {
                    currentTool.HolderName = "Неизвестная оправка";
                    currentTool.HolderSpindelSideInterface = "";
                    currentTool.HolderCutSideInterface = "";
                }
            }
        }

        void AddCsvDataToTools()
        {
            var csvToolDataList = spCamToolBaseReader.GetCsvToolDataList();

            foreach (var currentTool in projectTools)
            {
                CsvToolData csvToolData = csvToolDataList.Find(d => d.Name.Equals(currentTool.Name));
                if (csvToolData != null) 
                {
                    currentTool.InsertPattern1 = csvToolData.InsertPattern1;
                    currentTool.InsertPattern2 = csvToolData.InsertPattern2;
                    currentTool.FromSpindelSideInterface = csvToolData.FromSpindelSideInterface;
                }
                else
                {
                    currentTool.InsertPattern1 = "";
                    currentTool.InsertPattern2 = "";
                    currentTool.FromSpindelSideInterface = "";
                }

            }
        }

        void PerformToolInfo()
        {
            IEnumerable<Tool> distinctProjectTools = projectTools.Distinct();

            foreach (Tool tool in distinctProjectTools)
            {
                ToolInfo toolInfo = new ToolInfo();
                toolInfo.ToolPosition = tool.PositionNumber;
                toolInfo.SourceToolName = tool.Type + " " + tool.Name;
                toolInfo.SourceToolDiametr = tool.Diametr;
                toolInfo.SourceToolLength = tool.Length;
                toolInfo.SourceCuttingLength = tool.WorkingLength;
                toolInfo.SourceCutRadius = tool.EdgeRadius;
                toolInfo.SourceCutAngle = tool.Angle;
                toolInfo.SourceNumberOfTeeth = tool.NumberOfTeeth;
                toolInfo.SourceToolOverhang = tool.WorkingOverhang;
                toolInfo.SourceHolderName = tool.HolderName;
                toolInfo.SourceInsertNames1 = SearchMatchedToolFromExcel(tool.InsertPattern1, inserts);
                toolInfo.SourceInsertNames2 = SearchMatchedToolFromExcel(tool.InsertPattern2, inserts);
                toolInfo.SourceColletNames = SearchMatchedToolFromExcel(tool.FromSpindelSideInterface, collets);

                toolInfoContainer.Add(toolInfo);
            }
        }

        List<string> SearchMatchedToolFromExcel(string searchPattern, List<ExcelTool> excelTools)
        {
            if (excelTools==null)
            {
                return null;//listOfInsertsResults;
            }

            if (searchPattern == "")
            {
                //listOfInsertsResults.Add("отсутствует или указан не верно тип пластины");
                return null; //listOfInsertsResults;
            }

            List<string> listOfSearchResults = new List<string>();

            foreach (ExcelTool excelTool in excelTools)
            {
                string formatedExcelToolName = FormatExcelToolName(excelTool.ToolName);

                if (Regex.IsMatch(formatedExcelToolName, searchPattern))
                {
                    listOfSearchResults.Add(excelTool.ToolName + " [" + excelTool.StorageQuantity + " на складе, " + excelTool.ProductionQuantity + " в цехе]");
                }
            }

            return listOfSearchResults;
        }

        string FormatExcelToolName(string excelToolName)
        {
            return excelToolName.Replace(" ", "");
        }

        /*public List<Tool> ProjectTools
        {
            get { return projectTools; }
        }*/

        public List<ToolInfo> ProjectToolsInfo
        {
            get { return toolInfoContainer; }
        }

        public IEnumerable<Tool> DistinctProjectTools
        {
            get { return projectTools.Distinct<Tool>(); }
        }

        public Operation OperationDescription
        {
            get { return operationDescription; }
        }


        /*public void ShowToolInfoInExcel()
        {
            DistinctOperationTools();
            ExcelRedactor excelRedactor = new ExcelRedactor();
            excelRedactor.Visible = true;
            int rowIndex = 2;

            foreach (var currentTool in distinctProjectTools)
            {
                excelRedactor.SetCellValue("Инструмент:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.Type + " " + currentTool.Name, rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Диаметр:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.Diametr.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Длинна инструмента:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.Length.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Длинна режущей части:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.WorkingLength.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Радиус на кромке:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.EdgeRadius.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Угол кромки:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.Angle.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Кол-во зубов:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.NumberOfTeeth.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Вылет инструмента:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.WorkingOverhang.ToString(), rowIndex, 2);
                rowIndex++;
                excelRedactor.SetCellValue("Оправка:", rowIndex, 1);
                excelRedactor.SetCellValue(currentTool.HolderName, rowIndex, 2);
                rowIndex += 2;
            }
            excelRedactor.CellAutoFit();
        }

        void DistinctOperationTools()
        {
            distinctProjectTools = projectTools.Distinct<Tool>();
        }*/

    }
}
