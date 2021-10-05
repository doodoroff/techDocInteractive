using System;
using System.Collections.Generic;
using System.Collections;
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
        readonly List<string> insertBaseFilePaths;
        readonly List<string> colletBaseFilePaths;
        readonly List<string> millBaseFilePaths;
        readonly List<string> drillBaseFilePaths;

        List<ProjectTool> projectTools;
        SpCamXMLReader spCamXMLReader;
        SpCamToolBaseReader spCamToolBaseReader;
        //ExcelToolInfoReader excelToolInfoReader;
        //CsvRedactor CsvRedactor;
        List<XMLTool> xmlTools;
        Operation operationDescription;
        List<ToolInfo> toolInfoContainer;
        //List<AuxToolInfo> auxToolInfoContainer;
        //List<string> auxToolsList;
        List<ExcelTool> inserts;
        List<ExcelTool> collets;
        List<ExcelTool> mills;
        List<ExcelTool> drills;
        //IEnumerable<Tool> distinctProjectTools;

        public ToolInfoPresenter(string toolBaseFilePath, 
                                 string xmlFilePath, 
                                 List<string> insertBaseFilePaths = null, 
                                 List<string> colletBaseFilePaths = null, 
                                 List<string> millBaseFilePaths = null,
                                 List<string> drillBaseFilePaths = null)
        {
            this.xmlFilePath = xmlFilePath;
            this.toolBaseFilePath = toolBaseFilePath;
            this.insertBaseFilePaths = insertBaseFilePaths;
            this.colletBaseFilePaths = colletBaseFilePaths;
            this.millBaseFilePaths = millBaseFilePaths;
            this.drillBaseFilePaths = drillBaseFilePaths;

            projectTools = new List<ProjectTool>();
            spCamXMLReader = new SpCamXMLReader(xmlFilePath);
            spCamToolBaseReader = new SpCamToolBaseReader(toolBaseFilePath);
        }

        public void InitializeAndStartDataProcessing()
        {
            OnFlagInsertDataProcessingEvent(FlagInsertDataProcessingStart);

            //InitializeDataProcessing();

            xmlTools = spCamXMLReader.ProjectToolList;
            operationDescription = spCamXMLReader.OperationDescription;

            toolInfoContainer = new List<ToolInfo>();
            //auxToolInfoContainer = new List<AuxToolInfo>();
            //auxToolsList = new List<string>();

            //AddHolderNamesToTools();

            try
            {
                /*inserts = new ExcelToolInfoReader(insertBaseFilePath).ExcelToolList;
                collets = new ExcelToolInfoReader(colletBaseFilePath).ExcelToolList;
                mills = new ExcelToolInfoReader(millBaseFilePath).ExcelToolList;*/
                inserts = ReadToolBaseFiles(insertBaseFilePaths);
                collets = ReadToolBaseFiles(colletBaseFilePaths);
                mills = ReadToolBaseFiles(millBaseFilePaths);
                drills = ReadToolBaseFiles(drillBaseFilePaths);
            }
            catch (Exception)
            {
                inserts = null;
                collets = null;
                mills = null;
                drills = null;
            }
            finally
            {
                InitializeProjectTools();
                AddCsvHolderDataToAuxTools();
                AddCsvToolDataToProjectTools();
                PerformToolInfo();
            }

            OnFlagInsertDataProcessingEvent(FlagInsertDataProcessingStop);
        }

        void InitializeProjectTools()
        {
            foreach (XMLTool toolFromXML in xmlTools)
            {
                ProjectTool projectTool = new ProjectTool();
                projectTool.Angle = toolFromXML.Angle;
                projectTool.Diametr = toolFromXML.Diametr;
                projectTool.Durability = toolFromXML.Durability;
                projectTool.EdgeRadius = toolFromXML.EdgeRadius;
                projectTool.HolderCode = toolFromXML.HolderCode;
                projectTool.ToolLength = toolFromXML.Length;
                projectTool.Name = toolFromXML.Name;
                projectTool.NumberOfTeeth = toolFromXML.NumberOfTeeth;
                projectTool.PositionNumber = toolFromXML.PositionNumber;
                projectTool.Type = toolFromXML.Type;
                projectTool.WorkingLength = toolFromXML.WorkingLength;
                projectTool.FullOverhang = toolFromXML.FullOverhang;
                projectTool.HolderSetLength = toolFromXML.HolderLength;
                projectTool.TailOverhang = toolFromXML.TailOverhang;
                projectTool.HolderOverhangValuesList = toolFromXML.HolderOverhangValuesList;

                projectTools.Add(projectTool);
            }
        }

        List<ExcelTool> ReadToolBaseFiles(List<string> toolBasePaths)
        {
            List<ExcelTool> excelToolsBase = new List<ExcelTool>();

            foreach (var toolBasePath in toolBasePaths)
            {
                excelToolsBase.AddRange(new ExcelToolInfoReader(toolBasePath).ExcelToolList);
            }

            return excelToolsBase;
        }

        void OnFlagInsertDataProcessingEvent(EventHandler eventHandler)
        {
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        void AddCsvHolderDataToAuxTools()
        {
            List<AuxToolAssembly> holderList = spCamToolBaseReader.GetHolderList();

            foreach (ProjectTool projectTool in projectTools)
            {
                AuxToolAssembly auxToolAssembly = holderList.Find(h => h.HolderCode.Equals(projectTool.HolderCode));

                //AuxToolInfo auxToolInfo = new AuxToolInfo();
                if (auxToolAssembly != null) // Nesceserity ?
                {
                    projectTool.AuxToolAssembly = auxToolAssembly;
                    //auxToolInfoCombiner.AuxToolAssemblyCode = auxToolAssembly.HolderCode;
                    //auxToolInfoCombiner.Holders = auxToolAssembly.AssemblyHoldersList;
                    //List<Holder> assemblyHoldersList = auxToolAssembly.AssemblyHoldersList;
                    //if (assemblyHoldersList.Count != 0)
                    //{
                    //    foreach (Holder holder in assemblyHoldersList)
                    //    {
                    //        auxToolInfo.HolderName = holder.Name + " " + holder.Description;
                    //        auxToolInfo.Overhang = holder.Overhang;
                    //        auxToolInfo.FixingDevices = PerformMatchedToolListFromExcel(holder.FromSpindelSideInterface, collets);
                    //    }
                    //}
                    /*currentTool.HolderNames = auxToolAssembly.AssemblyHoldersList;
                    currentTool.HolderSpindelSideInterface = auxToolAssembly.FromSpindelSideInterface;
                    currentTool.HolderCutSideInterface = auxToolAssembly.FromCutSideInterface;*/
                }
                //else
                //{

                //    currentTool.HolderNames = new List<Holder>();
                //    currentTool.HolderSpindelSideInterface = "";
                //    currentTool.HolderCutSideInterface = "";*/
                //}
                //auxToolInfoContainer.Add(auxToolInfo);
            }
        }

        void AddCsvToolDataToProjectTools()
        {
            List<CsvTool> csvToolDataList = spCamToolBaseReader.GetCsvToolDataList();

            foreach (ProjectTool currentTool in projectTools)
            {
                CsvTool csvToolData = csvToolDataList.Find(d => d.Name.Equals(currentTool.Name));
                if (csvToolData != null) 
                {
                    currentTool.InsertPattern1 = csvToolData.InsertPattern1;
                    currentTool.InsertPattern2 = csvToolData.InsertPattern2;
                    currentTool.CutToolFromSpindelSideInterface = csvToolData.FromSpindelSideInterface;
                }
                else
                {
                    currentTool.InsertPattern1 = "";
                    currentTool.InsertPattern2 = "";
                    currentTool.CutToolFromSpindelSideInterface = "";
                }

            }
        }

        void PerformToolInfo()
        {
            IEnumerable<ProjectTool> distinctProjectTools = projectTools.Distinct();

            foreach (ProjectTool projectTool in distinctProjectTools)
            {
                ToolInfo toolInfo = new ToolInfo();

                toolInfo.ToolPosition = projectTool.PositionNumber;
                toolInfo.SourceToolDiametr = projectTool.Diametr;
                toolInfo.SourceToolLength = projectTool.ToolLength;
                toolInfo.SourceCuttingLength = projectTool.WorkingLength;
                toolInfo.SourceCutRadius = projectTool.EdgeRadius;
                toolInfo.SourceCutAngle = projectTool.Angle;
                toolInfo.SourceNumberOfTeeth = projectTool.NumberOfTeeth;
                toolInfo.SourceToolOverhang = projectTool.ToolOverhang;
                toolInfo.SourceInsertNames1 = PerformMatchedToolListFromExcel(projectTool.InsertPattern1, inserts);
                toolInfo.SourceInsertNames2 = PerformMatchedToolListFromExcel(projectTool.InsertPattern2, inserts);
                toolInfo.SourceToolName = PerformToolStorageInfoFromExcel(projectTool.Type, projectTool.Name);
                toolInfo.SourceAuxToolsSpecification = PerformAuxToolInfo(projectTool.AuxToolSpecification);

                toolInfoContainer.Add(toolInfo);
            }
        } 

        //List<string> PerformAuxToolInfo()
        //{
        //    List<string> auxToolInfo = new List<string>();

        //    foreach (ProjectTool projectTool in projectTools)
        //    {
        //        List<Holder> holders = projectTool.AuxToolAssembly.AssemblyHoldersList;
        //    }

        //    return auxToolInfo;
        //}

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
                        return drills;
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

        List<AuxToolInfo> PerformAuxToolInfo(ArrayList auxToolInfoSpecification)
        {
            ArrayList localAuxToolSpecification = auxToolInfoSpecification;
            if (localAuxToolSpecification == null)
            {
                return null;
            }
            else
            {
                List<AuxToolInfo> auxToolInfoList = new List<AuxToolInfo>();

                for (int i = 0; i < localAuxToolSpecification.Count; i++)
                {
                    if (localAuxToolSpecification[i] is Holder)
                    {
                        AuxToolInfo auxToolInfo = new AuxToolInfo();
                        Holder holder = localAuxToolSpecification[i] as Holder;
                        auxToolInfo.AuxToolName = holder.Name + " " + holder.Description;
                        auxToolInfo.AuxToolOverhang = holder.Overhang;
                        if (localAuxToolSpecification[i + 1] is Hub)
                        {
                            Hub hub = localAuxToolSpecification[i + 1] as Hub;
                            List<string> matchedHubs = PerformMatchedToolListFromExcel(hub.NamePattern, collets);
                            auxToolInfo.HubList = matchedHubs;
                        }
                        auxToolInfoList.Add(auxToolInfo);
                    }
                }

                return auxToolInfoList;
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

        public IEnumerable<XMLTool> DistinctProjectTools
        {
            get { return xmlTools.Distinct<XMLTool>(); }
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
