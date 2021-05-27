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
        readonly string millBaseFilePath;

        SpCamXMLReader spCamXMLReader;
        SpCamToolBaseReader spCamToolBaseReader;
        //ExcelToolInfoReader excelToolInfoReader;
        //CsvRedactor CsvRedactor;
        List<ToolSet> projectTools;
        Operation operationDescription;
        List<ToolInfo> toolInfoContainer;
        List<ExcelTool> inserts;
        List<ExcelTool> collets;
        List<ExcelTool> mills;
        //IEnumerable<Tool> distinctProjectTools;

        public ToolInfoPresenter(string toolBaseFilePath, string xmlFilePath, string insertBaseFilePath = null, string colletBaseFilePath = null, string millBaseFilePath = null)
        {
            this.xmlFilePath = xmlFilePath;
            this.toolBaseFilePath = toolBaseFilePath;
            this.insertBaseFilePath = insertBaseFilePath;
            this.colletBaseFilePath = colletBaseFilePath;
            this.millBaseFilePath = millBaseFilePath;

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
                mills = new ExcelToolInfoReader(millBaseFilePath).ExcelToolList;
            }
            catch (Exception)
            {
                inserts = null;
                collets = null;
                mills = null;
                //throw new AppXmlAnalyzerExceptions("Файл с базой пластин не найден");
            }
            finally
            {
                PerformToolInfo();
            }

            OnFlagInsertDataProcessingEvent(FlagInsertDataProcessingStop);
        }

        void OnFlagInsertDataProcessingEvent(EventHandler eventHandler)
        {
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

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
                CsvTool csvToolData = csvToolDataList.Find(d => d.Name.Equals(currentTool.Name));
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
            IEnumerable<ToolSet> distinctProjectTools = projectTools.Distinct();

            foreach (ToolSet tool in distinctProjectTools)
            {
                ToolInfo toolInfo = new ToolInfo();
                toolInfo.ToolPosition = tool.PositionNumber;
                //toolInfo.SourceToolName = tool.Type + " " + tool.Name;
                toolInfo.SourceToolDiametr = tool.Diametr;
                toolInfo.SourceToolLength = tool.Length;
                toolInfo.SourceCuttingLength = tool.WorkingLength;
                toolInfo.SourceCutRadius = tool.EdgeRadius;
                toolInfo.SourceCutAngle = tool.Angle;
                toolInfo.SourceNumberOfTeeth = tool.NumberOfTeeth;
                toolInfo.SourceToolOverhang = tool.WorkingOverhang;
                toolInfo.SourceHolderName = tool.HolderName;
                toolInfo.SourceInsertNames1 = PerformMatchedToolListFromExcel(tool.InsertPattern1, inserts);
                toolInfo.SourceInsertNames2 = PerformMatchedToolListFromExcel(tool.InsertPattern2, inserts);
                toolInfo.SourceColletNames = PerformMatchedToolListFromExcel(tool.FromSpindelSideInterface, collets);
                //toolInfo.SourceToolStorageInfo = PerformToolStorageInfoFromExcel(tool.Type, tool.Name);
                toolInfo.SourceToolName = PerformToolStorageInfoFromExcel(tool.Type, tool.Name);

                toolInfoContainer.Add(toolInfo);
            }
        }

        List<string> PerformMatchedToolListFromExcel(string searchPattern, List<ExcelTool> excelTools)
        {
            if (excelTools == null)
            {
                return null;//listOfInsertsResults;
            }

            if (searchPattern == "")
            {
                //listOfInsertsResults.Add("отсутствует или указан не верно тип пластины");
                return null; //listOfInsertsResults;
            }

            List<ExcelTool> excelToolsToSearchIn = GatherExcelTools(searchPattern, excelTools);

            List<string> listOfSearchResults = new List<string>();

            foreach (ExcelTool excelTool in excelToolsToSearchIn)
            {
                listOfSearchResults.Add("("
                                        + excelTool.OmegaCode
                                        + ") "
                                        + excelTool.ToolName 
                                        + " [" 
                                        + excelTool.ProductionStorageQuantity 
                                        + " на складе, " 
                                        + excelTool.CentralStorageQuantity 
                                        + " на цент. складе, " 
                                        + excelTool.ProductionQuantity 
                                        + " в цехе]");
            }

            return listOfSearchResults;
        }

        List<ExcelTool> GatherExcelTools(string searchPattern, List<ExcelTool> excelTools)
        {
            List<ExcelTool> excelToolList = new List<ExcelTool>();

            foreach (ExcelTool excelTool in excelTools)
            {
                string formatedExcelToolName = FormatExcelToolName(excelTool.ToolName);

                if (Regex.IsMatch(formatedExcelToolName, searchPattern))
                {
                    excelToolList.Add(excelTool);
                }
            }

            return CombineExcelToolsQuantity(excelToolList);
        }

        List<ExcelTool> CombineExcelToolsQuantity(List<ExcelTool> excelTools)
        {
            List<ExcelTool> comparedExcelTools = excelTools;
            List<ExcelTool> resultedExcelToolList = new List<ExcelTool>();

            foreach (ExcelTool excelTool in excelTools)
            {
                ExcelTool resultedExcelTool = new ExcelTool();

                foreach (ExcelTool currentExcelTool in comparedExcelTools)
                {
                    if (currentExcelTool.ToolName.Equals(excelTool.ToolName))
                    {
                        resultedExcelTool.ToolName = currentExcelTool.ToolName;
                        resultedExcelTool.OmegaCode = currentExcelTool.OmegaCode;

                        if (currentExcelTool.ProductionQuantity.Equals(excelTool.ProductionQuantity) && currentExcelTool.ProductionQuantity != 0)
                        {

                        }
                        else
                        {
                            resultedExcelTool.ProductionQuantity += currentExcelTool.ProductionQuantity;
                        }

                        if (currentExcelTool.ProductionStorageQuantity.Equals(excelTool.ProductionStorageQuantity) && currentExcelTool.ProductionQuantity != 0)
                        {

                        }
                        else
                        {
                            resultedExcelTool.ProductionStorageQuantity += currentExcelTool.ProductionStorageQuantity;
                        }

                        if (currentExcelTool.CentralStorageQuantity.Equals(excelTool.CentralStorageQuantity) && currentExcelTool.ProductionQuantity != 0)
                        {

                        }
                        else
                        {
                            resultedExcelTool.CentralStorageQuantity += currentExcelTool.CentralStorageQuantity;
                        }
                    }
                }
                if (!resultedExcelToolList.Contains(resultedExcelTool))
                {
                    resultedExcelToolList.Add(resultedExcelTool);
                }

            }
            return resultedExcelToolList;
        }

        string FormatExcelToolName(string excelToolName) // TO DO Refactoring, Class formatter ?
        {
            string formatedExcelToolName = excelToolName.Replace(" ", "");
            if (formatedExcelToolName.Contains('('))
            {
                int comentStartPosition = formatedExcelToolName.IndexOf('(');
                formatedExcelToolName = formatedExcelToolName.Remove(comentStartPosition);
            }

            return formatedExcelToolName;
        }

        string PerformToolStorageInfoFromExcel(string toolDescription, string toolName)
        {
            ToolType toolType = DefineToolType(toolDescription);
            List<ExcelTool> listOfExcelTools = AssignExcelToolBase(toolType);

            if (toolName == null)
            {
                return "";
            }
            if (toolDescription == null)
            {
                return toolName;
            }
            if (listOfExcelTools == null)
            {
                return toolDescription + " " + toolName;
            }

            string searchPattern = FormatExcelToolName(toolName);

            List<ExcelTool> resultedExcelToolList = GatherExcelTools(searchPattern, listOfExcelTools);
            ExcelTool resultedExcelTool = null;

            if (resultedExcelToolList != null && resultedExcelToolList.Count != 0)
            {
                resultedExcelTool = resultedExcelToolList[0];
            }

            string stringToReturn = toolDescription + " " + toolName;

            if (resultedExcelTool != null)
            {
                stringToReturn = "("
                    + resultedExcelTool.OmegaCode
                    + ") "
                    + toolDescription
                    + " "
                    + toolName
                    + " [ "
                    + resultedExcelTool.ProductionStorageQuantity.ToString()
                    + " на складе, "
                    + resultedExcelTool.CentralStorageQuantity.ToString()
                    + " на цент. складе "
                    + resultedExcelTool.ProductionQuantity
                    + " в цехе]";
            }

            return stringToReturn;
        }

        ToolType DefineToolType(string toolDescription)
        {
            if (toolDescription.Contains("Фреза"))
            {
                return ToolType.mill;
            }
            if (toolDescription.Contains("Сверло"))
            {
                return ToolType.drill;
            }
            if (toolDescription.Contains("Оправка") || toolDescription.Contains("Патрон"))
            {
                return ToolType.holder;
            }

            return ToolType.unknown;
        }

        List<ExcelTool> AssignExcelToolBase(ToolType toolType)
        {
            switch (toolType)
            {
                case ToolType.unknown:
                    {
                        return null;
                    }
                case ToolType.mill:
                    {
                        return mills;
                    }
                case ToolType.reamer:
                    {
                        return null;
                    }
                case ToolType.boringTool:
                    {
                        return null;
                    }
                case ToolType.drill:
                    {
                        return null;
                    }
                case ToolType.tap:
                    {
                        return null;
                    }
                case ToolType.holder:
                    {
                        return null;
                    }
                case ToolType.tail:
                    {
                        return null;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        /*public List<Tool> ProjectTools
        {
            get { return projectTools; }
        }*/

        public List<ToolInfo> ProjectToolsInfo
        {
            get { return toolInfoContainer; }
        }

        public IEnumerable<ToolSet> DistinctProjectTools
        {
            get { return projectTools.Distinct<ToolSet>(); }
        }

        public Operation OperationDescription
        {
            get { return operationDescription; }
        }

        /*void DistinctOperationTools()
        {
            distinctProjectTools = projectTools.Distinct<Tool>();
        }*/

    }
}
