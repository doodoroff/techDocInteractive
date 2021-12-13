using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace TechDocInteractive
{
    class SpCamXMLReader
    {
        List<XMLTool> projectTools;
        Operation operation;
        Setup setup;
        Shift shift;
        XmlDocument xmlDocument;
        XPathNavigator xPathNavigator;
        XPathNodeIterator xPathTechnologyIterator;
        int shiftPosition = 1;
        //string shiftLabel = "";
        //List<string> blockOfShifts;

        public SpCamXMLReader(string filePath)
        {
            this.projectTools = new List<XMLTool>();
            this.operation = new Operation();
            xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);
            this.xPathNavigator = xmlDocument.CreateNavigator();

            if (XmlDocumentHaveRightFormat())
            {
                ReadXML();
            }
            else
            {
                throw new AppXmlAnalyzerExceptions("Неверный формат XML файла");
            }
        }

        bool XmlDocumentHaveRightFormat()
        {
            if (xmlDocument.DocumentElement.Name.Equals("StcxProject"))
            {
                return true;
            }
            return false;
        }

        void ReadXML()
        {
            XmlNode customDataNode = xmlDocument.SelectSingleNode("StcxProject/CustomData");
            XmlAttributeCollection customDataAttributes = customDataNode.LastChild.Attributes;
            operation.DetailName = customDataAttributes[1].Value;

            /*XPathNodeIterator*/ xPathTechnologyIterator = CreateSectionIterator("Technology");
            XPathNodeIterator xPathOpXmlParamsIterator = CreateSectionIterator("OpXmlParams");
            XPathNodeIterator xPathOpTechTreeIterator = CreateSectionIterator("OpTechTree");

            ReadTechStructure(xPathOpTechTreeIterator);

            while (xPathOpXmlParamsIterator.MoveNext() && xPathTechnologyIterator.MoveNext())
            {
                XMLTool currentOperationTool = new XMLTool();
                XPathNavigator currentOpXmlParamsNode = xPathOpXmlParamsIterator.Current;
                XPathNavigator currentTechnologyNode = xPathTechnologyIterator.Current;

                if (currentOpXmlParamsNode.Name.Equals("TSTRootGroup"))
                {
                    currentOpXmlParamsNode.MoveToChild("Name", "");
                    operation.MachinetoolName = currentOpXmlParamsNode.Value;
                    currentOpXmlParamsNode.MoveToParent();

                    xPathOpXmlParamsIterator.MoveNext(); // Rewind to same operation position as xPathTechnologyIterator
                }

                if (currentTechnologyNode.Name.Equals("TSTTechMillOpGroupFin"))
                {

                }
                else if (currentOpXmlParamsNode.Name.Equals("TSTTechMillOpGroupFin"))
                {
                    //if (IsNodeSetup(currentOpXmlParamsNode))
                    //{
                    //    if (setup != null)
                    //    {
                    //        setup.AddShift(shift);
                    //        operation.AddSetup(setup);
                    //        shift = null;
                    //    }

                    //    this.setup = new Setup();
                    //    currentOpXmlParamsNode.MoveToChild("Comment", "");
                    //    setup.SetupName = currentOpXmlParamsNode.Value;
                    //    currentOpXmlParamsNode.MoveToParent();
                    //}
                    //else
                    //{
                    //    //shiftLabel = ReadTechDescription(currentOpXmlParamsNode);
                    //}
                }
                else
                {
                    GetToolInfoFromTechnologySection(currentOperationTool, currentTechnologyNode);
                    GetToolInfoFromOpXmlParamsSection(currentOperationTool, currentOpXmlParamsNode);
                    GetHolderInfoFromOpXmlParamsSection(currentOperationTool, currentOpXmlParamsNode);
                    //GetTechnologyInfo(currentOperationTool, currentOpXmlParamsNode, currentTechnologyNode, setup);
                    projectTools.Add(currentOperationTool);
                }

            }
            //setup.AddShift(shift);
            //operation.AddSetup(setup);
        }

        void ReadTechStructure(XPathNodeIterator xPathOpTechTreeIterator)
        {
            //ArrayList setupStructure = new ArrayList();
            List<Shift> setupStructure = new List<Shift>();
            XPathNavigator opTSTRootGroupNode = xPathOpTechTreeIterator.Current;

            opTSTRootGroupNode.MoveToChild("TSTRootGroup", "");
            //opTSTRootGroupNode.MoveToChild("OpParams", "");
            //opTSTRootGroupNode.MoveToChild("Name", "");
            operation.MachinetoolName = ReadTechDescription(opTSTRootGroupNode, "Comment");
            //opTSTRootGroupNode.MoveToParent();
            //opTSTRootGroupNode.MoveToParent();

            XPathNodeIterator techTreeSetupsIterator = opTSTRootGroupNode.SelectChildren(xPathNavigator.NodeType);

            while (techTreeSetupsIterator.MoveNext())
            {
                XPathNavigator currentOpTechTreeNode = techTreeSetupsIterator.Current;
                
                if (currentOpTechTreeNode.Name.Equals("TSTTechMillOpGroupFin"))
                {
                    if (IsNodeSetup(currentOpTechTreeNode))
                    {
                        if (setup != null)
                        {
                            //setup.AddShift(shift);
                            operation.AddSetup(setup);
                            //shift = null;
                        }

                        this.setup = new Setup();
                        //currentOpTechTreeNode.MoveToChild("OpParams", "");
                        //currentOpTechTreeNode.MoveToChild("Comment", "");
                        setup.SetupName = ReadTechDescription(currentOpTechTreeNode, "Comment");
                        //currentOpTechTreeNode.MoveToParent();
                        //currentOpTechTreeNode.MoveToParent();

                        setupStructure = ReadAndFormatBlockOfShifts(currentOpTechTreeNode);
                        setup.AddSetupShifts(setupStructure);
                    }
                }
                else if (currentOpTechTreeNode.Name.Equals("OpParams"))
                {

                }
                else
                {
                    if (setup == null)
                    {
                        shiftPosition = 0; // Rewind position back because of setup folder absence
                        this.setup = new Setup();
                        setup.SetupName = "Установ 1";
                        currentOpTechTreeNode.MoveToParent();
                        setupStructure = ReadAndFormatBlockOfShifts(currentOpTechTreeNode);
                        setup.AddSetupShifts(setupStructure);
                    }
                    //else
                    //{
                    //    int shiftToolId = ReadToolIDinTechTree(currentOpTechTreeNode);
                    //    if (shift == null)
                    //    {
                    //        shift = new Shift();
                    //    }
                    //    if (shift.ShiftToolId != shiftToolId && shift.ShiftToolId != 0)
                    //    {
                    //        shift = new Shift();
                    //        setup.AddSetupStructure(shift);
                    //    }
                    //    string shiftDescriptionNote = ReadTechDescription(currentOpTechTreeNode, "Comment");
                    //    shift.ShiftToolId = shiftToolId;
                    //    shift.AddShifDescriptionNote(shiftDescriptionNote);
                    //    if (!currentOpTechTreeNode.MoveToNext())
                    //    {
                    //        setup.AddSetupStructure(shift);
                    //    }
                    //}
                }
            }
            operation.AddSetup(setup);
        }

        List<Shift> ReadAndFormatBlockOfShifts(XPathNavigator currentOpTechTreeNode)
        {
            List<Shift> result = new List<Shift>();
            List<Shift> shifts = GetShiftTree(currentOpTechTreeNode);

            int previousToolId = 0;
            for (int i = 0; i < shifts.Count; i++)
            {
                if (shifts[i].ShiftToolId == previousToolId)
                {
                    previousToolId = shifts[i].ShiftToolId;

                    Shift shift = result[result.Count - 1];
                    foreach (List<string> note in shifts[i].GetShiftDescriptionNotes())
                    {
                        shift.AddShifDescriptionNote(note);
                    }
                    shift.AddMachiningTime(shifts[i].GetMachiningTimeSeconds());
                    shift.AddAuxiliaryTime(shifts[i].GetAuxiliaryTimeSeconds());
                    shift.AddIdlingTime(shifts[i].GetIdlingTimeSeconds());
                    shift.AddRapidTime(shifts[i].GetRapidTimeSeconds());
                    shift.AddToolPath(shifts[i].GetToolPath());

                    result[result.Count - 1] = shift;
                }
                else
                {
                    previousToolId = shifts[i].ShiftToolId;
                    result.Add(shifts[i]);
                }
            }

            return result;
        }

        List<Shift> GetShiftTree(XPathNavigator currentOpTechTreeNode, string blockOfShiftsLabel = "")
        {
            List<Shift> result = new List<Shift>();
            shift = new Shift();
            List<string> shiftDescriptionNotes = new List<string>();
            XPathNodeIterator shiftIterator = currentOpTechTreeNode.SelectChildren(currentOpTechTreeNode.NodeType);

            foreach (XPathNavigator currentNode in shiftIterator)
            {
                if (currentNode.Name.Equals("OpParams"))
                {
                    shiftPosition++;
                }
                else if (currentNode.Name.Equals("TSTTechMillOpGroupFin"))
                {

                    string blockOfShiftLabel = "#" + ReadTechDescription(currentNode, "Comment");
                    //shift = new Shift();
                    //shift.AddShifDescriptionNote(blockOfShiftLabel);
                    //List<Shift> shifts = ReadBlockOfShifts(currentNode, blockOfShiftLabel);
                    //result.AddRange(shifts);
                    //ArrayList blockOfShiftDescriptionNotes = new ArrayList();
                    //blockOfShiftDescriptionNotes.Add(blockOfShiftLabel);

                    List<Shift> shiftDescriptionNotesFromBlock = GetShiftTree(currentNode, blockOfShiftLabel);
                    //shiftDescriptionNotesFromBlock.Add(blockOfShiftLabel);
                    //shiftDescriptionNotesFromBlock.Add(ReadBlockOfShifts(currentNode));

                    //blockOfShiftDescriptionNotes = ReadBlockOfShifts(currentNode, blockOfShiftLabel);
                    //blockOfShiftDescriptionNotes.AddRange(shiftDescriptionNotesFromBlock);

                    //result.Add(blockOfShiftDescriptionNotes);
                    result.AddRange(shiftDescriptionNotesFromBlock);

                    //shift.AddShifDescriptionNote(shiftDescriptionNotesFromBlock);
                }
                else
                {
                    int shiftToolId = ReadToolIDinTechTree(currentNode);
                    //if (shift == null) 
                    //{
                    //    shift = new Shift();
                    //}
                    if (shift.ShiftToolId != shiftToolId && shift.ShiftToolId != 0)
                    {
                        shift.AddShifDescriptionNote(shiftDescriptionNotes);
                        result.Add(shift);
                        shiftDescriptionNotes = new List<string>();
                        shift = new Shift();
                    }
                    if (blockOfShiftsLabel != "")
                    {
                        //shift.AddShifDescriptionNote(blockOfShiftsLabel);
                        shiftDescriptionNotes.Add(blockOfShiftsLabel);
                    }
                    string shiftDescriptionNote = ReadTechDescription(currentNode, "Comment");
                    shift.ShiftToolId = shiftToolId;
                    //shift.AddShifDescriptionNote(shiftDescriptionNote);
                    shiftDescriptionNotes.Add(shiftDescriptionNote);
                    GetLaborIntencityInfo(shift, shiftPosition);
                    if (IsSingleShift(currentNode))
                    {
                        shift.AddShifDescriptionNote(shiftDescriptionNotes);
                        result.Add(shift);
                        shiftDescriptionNotes = new List<string>();
                        shift = new Shift();
                    }
                    blockOfShiftsLabel = "";
                    //result.Add(shiftDescriptionNote);
                    //result.Add(shift);
                    shiftPosition++;
                }
            }
            //shift.AddShifDescriptionNote(result);
            //result.Add(shift);
            //shift = new Shift();
            return result;
        }

        //ArrayList ReadBlockOfShifts(XPathNavigator currentOpTechTreeNode, string blockOfShiftsLabel = "")
        //{
        //    ArrayList result = new ArrayList();
        //    shift = new Shift();

        //    foreach (XPathNavigator currentNode in currentOpTechTreeNode.SelectChildren(currentOpTechTreeNode.NodeType))
        //    {
        //        if (currentNode.Name.Equals("OpParams"))
        //        {

        //        }
        //        else if (currentNode.Name.Equals("TSTTechMillOpGroupFin"))
        //        {
        //            string blockOfShiftLabel = "#" + ReadTechDescription(currentNode, "Comment");
        //            //shift = new Shift();
        //            ArrayList blockOfShiftDescriptionNotes = new ArrayList();
        //            //blockOfShiftDescriptionNotes.Add(blockOfShiftLabel);

        //            //ArrayList shiftDescriptionNotesFromBlock = ReadBlockOfShifts(currentNode, blockOfShiftLabel);
        //            //shiftDescriptionNotesFromBlock.Add(blockOfShiftLabel);
        //            //shiftDescriptionNotesFromBlock.Add(ReadBlockOfShifts(currentNode));

        //            blockOfShiftDescriptionNotes = ReadBlockOfShifts(currentNode, blockOfShiftLabel);
        //            //blockOfShiftDescriptionNotes.AddRange(shiftDescriptionNotesFromBlock);

        //            result.Add(blockOfShiftDescriptionNotes);
        //        }
        //        else
        //        {
        //            int shiftToolId = ReadToolIDinTechTree(currentNode);
        //            //if (shift == null) 
        //            //{
        //            //    shift = new Shift();
        //            //}
        //            if (blockOfShiftsLabel != "")
        //            {
        //                shift.AddShifDescriptionNote(blockOfShiftsLabel);
        //            }
        //            if (shift.ShiftToolId != shiftToolId && shift.ShiftToolId != 0)
        //            {
        //                result.Add(shift);
        //                shift = new Shift();
        //            }
        //            string shiftDescriptionNote = ReadTechDescription(currentNode, "Comment");
        //            shift.ShiftToolId = shiftToolId;
        //            shift.AddShifDescriptionNote(shiftDescriptionNote);
        //            GetLaborIntencityInfo(shift);
        //            if (IsSingleShift(currentNode))
        //            {
        //                result.Add(shift);
        //            }
        //            blockOfShiftsLabel = "";
        //            //result.Add(shiftDescriptionNote);
        //            //result.Add(shift);
        //        }
        //    }
        //    //result.Add(shift);
        //    shift = new Shift();
        //    return result;
        //}

        string ReadTechDescription(XPathNavigator currentNode, string parameterName)
        {
            string currentTechDescription;
            currentNode.MoveToChild("OpParams", "");
            currentNode.MoveToChild(parameterName, "");
            //currentNode.MoveToChild("Name", "");
            currentTechDescription = currentNode.Value;
            currentNode.MoveToParent();
            currentNode.MoveToParent();

            return currentTechDescription;
        }

        int ReadToolIDinTechTree(XPathNavigator currentNode)
        {
            int toolId;
            currentNode.MoveToChild("OpParams", "");
            currentNode.MoveToChild("ToolSection", "");
            currentNode.MoveToChild("Tools", "");
            currentNode.MoveToChild("TSTMillAxisTool", "");
            currentNode.MoveToChild("Number", "");
            toolId = currentNode.ValueAsInt;
            currentNode.MoveToParent();
            currentNode.MoveToParent();
            currentNode.MoveToParent();
            currentNode.MoveToParent();
            currentNode.MoveToParent();

            return toolId;
        }

        void GetLaborIntencityInfo(Shift shift, int shiftPosition)
        {
            while (xPathTechnologyIterator.MoveNext())
            {
                if (xPathTechnologyIterator.CurrentPosition == shiftPosition)
                {
                    XPathNavigator currentNode = xPathTechnologyIterator.Current;
                    ReadTechDescriptionFromTechnologySection(currentNode, shift);
                    break;
                }
            }
            xPathTechnologyIterator = CreateSectionIterator("Technology");
            //foreach (XPathNavigator currentNode in xPathTechnologyIterator)
            //{
            //    int toolId = 0;
            //    currentNode.MoveToChild("OperationType", "");
            //    string opType = currentNode.Value;
            //    currentNode.MoveToParent();
            //    if (opType != "TSTTechMillOpGroupFin")
            //    {
            //        currentNode.MoveToChild("Tool", "");
            //        currentNode.MoveToChild("ID", "");
            //        toolId = currentNode.ValueAsInt;
            //        currentNode.MoveToParent();
            //        currentNode.MoveToParent();
            //    }
            //    if (toolId == shift.ShiftToolId) 
            //    {
            //        ReadTechDescriptionFromTechnologySection(currentNode, shift);
            //    }
            //}
        }

        bool IsSingleShift(XPathNavigator currentNode)
        {
            if (!currentNode.MoveToNext())
            {
                currentNode.MoveToPrevious();
                return true;
            }

            //currentNode.MoveToNext();
            string nextNodeName = currentNode.Name;
            currentNode.MoveToPrevious();
            if (nextNodeName == "TSTTechMillOpGroupFin")
            {
                return true;
            }

            return false;
        }
        //Shift GenerateShift(XPathNavigator currentNode)
        //{
        //    int shiftToolId = ReadToolIDinTechTree(currentNode);
        //    if (shift == null)
        //    {
        //        shift = new Shift();
        //    }
        //    if (shift.ShiftToolId != shiftToolId && shift.ShiftToolId != 0)
        //    {
        //        shift = new Shift();
        //    }
        //    string shiftDescriptionNote = ReadTechDescription(currentNode, "Comment");
        //    shift.ShiftToolId = shiftToolId;
        //    shift.AddShifDescriptionNote(shiftDescriptionNote);
        //}

        bool IsNodeSetup(XPathNavigator currentNode)
        {
            //currentOpXmlParamsNode.MoveToChild("Icon", "");
            //string imageFullName = currentOpXmlParamsNode.Value;
            //currentOpXmlParamsNode.MoveToParent();
            //if (imageFullName.Contains("Group_setup"))
            //{
            //    return true;
            //}
            //return false;
            currentNode.MoveToChild("OpParams", "");
            currentNode.MoveToChild("Icon", "");
            string imageFullName = currentNode.Value;
            currentNode.MoveToParent();
            currentNode.MoveToParent();
            if (imageFullName.Contains("Group_setup"))
            {
                return true;
            }
            return false;
        }

        /*void GetTechnologyInfo(XMLTool currentOperationTool, XPathNavigator currentOpXmlParamsNode, XPathNavigator currentTechnologyNode, Setup currentSetup)
        {
            if (setup == null) 
            {
                setup = new Setup();
            }

            if (shift == null)
            {
                shift = new Shift();
                shift.Tool = currentOperationTool;
            }

            if (shift.Tool.PositionNumber != currentOperationTool.PositionNumber) 
            {
                currentSetup.AddShift(shift);
                shift = new Shift();
                shift.Tool = currentOperationTool;
                AddShiftDescription(currentOpXmlParamsNode, currentTechnologyNode);
                //if (shiftLabel != "no comment marker")
                //{
                //    shift.AddShiftGroupLabel(shiftLabel);
                //}
                //string shiftDescriptionNote = ReadTechDescriptionFromOpXmlSection(currentOpXmlParamsNode);
                //shift.AddShifDescriptionNote(shiftDescriptionNote);
                //ReadTechDescriptionFromTechnologySection(currentTechnologyNode, shift);
            }
            else
            {
                AddShiftDescription(currentOpXmlParamsNode, currentTechnologyNode);
                //if (shiftLabel != "no comment marker")
                //{
                //    shift.AddShiftGroupLabel(shiftLabel);
                //}
                //string shiftDescriptionNote = ReadTechDescriptionFromOpXmlSection(currentOpXmlParamsNode);
                //shift.AddShifDescriptionNote(shiftDescriptionNote);
                //ReadTechDescriptionFromTechnologySection(currentTechnologyNode, shift);
            }

            //shiftLabel = "no comment marker";
        }

        void AddShiftDescription(XPathNavigator currentOpXmlParamsNode, XPathNavigator currentTechnologyNode)
        {
            //if (shiftLabel != "no comment marker")
            //{
            //    if (blockOfShifts == null)
            //    {
            //        blockOfShifts = new List<string>();
            //        blockOfShifts.Add(shiftLabel);
            //    }

            //    string groupShiftDescriptionNote = ReadTechDescriptionFromOpXmlSection(currentOpXmlParamsNode);

            //    if (groupShiftDescriptionNote.Contains(shiftLabel))
            //    {
            //        blockOfShifts.Add(groupShiftDescriptionNote);
            //    }
            //    else
            //    {
            //        shift.AddShifDescriptionNote(blockOfShifts);
            //        blockOfShifts = null;
            //    }
            //}

            currentOpXmlParamsNode.MoveToChild("Comment", "");
            //currentNode.MoveToChild("Name", "");
            string shiftDescriptionNote = currentOpXmlParamsNode.Value;
            currentOpXmlParamsNode.MoveToParent();
            //string shiftDescriptionNote = ReadTechDescription(currentOpXmlParamsNode);
            shift.AddShifDescriptionNote(shiftDescriptionNote);
            ReadTechDescriptionFromTechnologySection(currentTechnologyNode, shift);
        }*/

        void ReadTechDescriptionFromTechnologySection(XPathNavigator currentTechnologyNode, Shift shift)
        {
            shift.AddMachiningTime(currentTechnologyNode.GetAttribute("MachiningTime", ""));
            shift.AddRapidTime(currentTechnologyNode.GetAttribute("RapidTime", ""));
            shift.AddIdlingTime(currentTechnologyNode.GetAttribute("IdlingTime", ""));
            shift.AddWorkingTime(currentTechnologyNode.GetAttribute("WorkTime", ""));
            shift.AddAuxiliaryTime(currentTechnologyNode.GetAttribute("AuxiliaryTime", ""));
            shift.AddToolPath(currentTechnologyNode.GetAttribute("ToolpathLength", ""));
        }

        /*double ParceTechDescription(string techDiscription)
        {
            if (techDiscription.Equals(""))
            {
                return 0;
            }

            double parcedValue;
            string formatedTechDiscription = techDiscription.Replace('.', ',');

            try
            {
                parcedValue = double.Parse(formatedTechDiscription);
            }
            catch (Exception)
            {
                throw new AppXmlAnalyzerExceptions("Технологический параметр " + formatedTechDiscription + " имеет не верный формат числа");
            }

            return parcedValue;
        }*/

        XPathNodeIterator CreateSectionIterator(string sectionName)
        {
            xPathNavigator.MoveToRoot();
            xPathNavigator.MoveToFirstChild();
            xPathNavigator.MoveToChild(sectionName, "");
            XPathNodeIterator xPathXmlIterator = xPathNavigator.SelectChildren(xPathNavigator.NodeType);
            return xPathXmlIterator;
        }

        void GetToolInfoFromTechnologySection(XMLTool currentOperationTool, XPathNavigator currentTechnologyNode)
        {
            currentTechnologyNode.MoveToChild("OperationType", "");
            if (currentTechnologyNode.Value.Equals("TSTTechMillOpGroupFin"))
            {

            }
            else
            {
                currentTechnologyNode.MoveToNext("Tool", "");
                currentTechnologyNode.MoveToChild("HolderString", "");
                currentOperationTool.HolderCode = currentTechnologyNode.Value;

                if (currentTechnologyNode.MoveToNext("ProfileText", ""))
                {
                    currentOperationTool.CustomTool = true;
                }
                currentTechnologyNode.MoveToParent();
            }
            currentTechnologyNode.MoveToParent();
        }

        void GetToolInfoFromOpXmlParamsSection(XMLTool currentOperationTool, XPathNavigator currentOpXmlParamsNode)
        {
            if (currentOperationTool.CustomTool)
            {
                ReadCustomToolOpXmlParamsCode(currentOpXmlParamsNode, currentOperationTool);
            }
            else
            {
                ReadToolOpXmlParamsCode(currentOpXmlParamsNode, currentOperationTool);
            }
        }

        void GetHolderInfoFromOpXmlParamsSection(XMLTool currentOperationTool, XPathNavigator currentOpXmlParamsNode)
        {
            currentOpXmlParamsNode.MoveToChild("ToolSection", "");
            currentOpXmlParamsNode.MoveToChild("Tools", "");
            currentOpXmlParamsNode.MoveToChild("TSTMillAxisTool", "");
            currentOpXmlParamsNode.MoveToChild("Properties", "");
            currentOpXmlParamsNode.MoveToChild("HolderSteps", "");

            ReadAndCalculateHolderAndTailLength(currentOperationTool, currentOpXmlParamsNode);

            currentOpXmlParamsNode.MoveToParent();
            currentOpXmlParamsNode.MoveToParent();
            currentOpXmlParamsNode.MoveToParent();
            currentOpXmlParamsNode.MoveToParent();
            currentOpXmlParamsNode.MoveToParent();
        }

        void ReadCustomToolOpXmlParamsCode(XPathNavigator currentOperationNode, XMLTool currentOperationTool)
        {
            currentOperationNode.MoveToChild("ToolSection", "");
            currentOperationNode.MoveToChild("Tools", "");
            currentOperationNode.MoveToChild("TSTMillAxisTool", "");
            currentOperationNode.MoveToChild("Comment", "");
            currentOperationTool.Name = currentOperationNode.Value;
            currentOperationNode.MoveToNext("Number", "");
            currentOperationTool.PositionNumber = currentOperationNode.ValueAsInt;
            currentOperationNode.MoveToNext("Durability", "");
            currentOperationTool.Durability = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("Properties", "");
            currentOperationNode.MoveToChild("ToolType", "");
            currentOperationTool.Type = currentOperationNode.Value;
            currentOperationNode.MoveToNext("ShapeTool", "");
            currentOperationNode.MoveToChild("Tool", "");
            //currentOperationNode.MoveToChild("DescriptorID", "");
            currentOperationNode.MoveToChild("Parameters", "");
            currentOperationNode.MoveToChild("Parameter", "");
            currentOperationNode.MoveToChild("Value", "");
            currentOperationTool.Diametr = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToNext();
            currentOperationNode.MoveToNext();
            currentOperationNode.MoveToChild("Value", "");
            currentOperationTool.WorkingLength = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToParent();
            //currentOperationNode.MoveToNext();
            //currentOperationNode.MoveToNext();
            //currentOperationTool.EdgeRadius = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToNext("Length", "");
            currentOperationTool.Length = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("Angle", "");
            currentOperationTool.Angle = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("Radius", "");
            currentOperationTool.EdgeRadius = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("TeethCount", "");
            currentOperationTool.NumberOfTeeth = currentOperationNode.ValueAsInt;
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToNext("AxialOverhang", "");
            currentOperationTool.FullOverhang = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToParent();
        }

        void ReadToolOpXmlParamsCode(XPathNavigator currentOperationNode, XMLTool currentOperationTool)
        {
            currentOperationNode.MoveToChild("ToolSection", "");
            currentOperationNode.MoveToChild("Tools", "");
            currentOperationNode.MoveToChild("TSTMillAxisTool", "");
            currentOperationNode.MoveToChild("Comment", "");
            currentOperationTool.Name = currentOperationNode.Value;
            currentOperationNode.MoveToNext("Number", "");
            currentOperationTool.PositionNumber = currentOperationNode.ValueAsInt;
            currentOperationNode.MoveToNext("Durability", "");
            currentOperationTool.Durability = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("Properties", "");
            currentOperationNode.MoveToChild("ToolType", "");
            currentOperationTool.Type = currentOperationNode.Value;
            currentOperationNode.MoveToNext("Diameter", "");
            currentOperationTool.Diametr = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("Length", "");
            currentOperationTool.Length = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("Angle", "");
            currentOperationTool.Angle = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("Radius", "");
            currentOperationTool.EdgeRadius = currentOperationNode.ValueAsDouble;
            currentOperationNode.MoveToNext("WorkingLength", "");
            try
            {
                currentOperationTool.WorkingLength = currentOperationNode.ValueAsDouble;
            }
            catch (Exception)
            {
                currentOperationTool.WorkingLength = currentOperationTool.Length;
            }
            currentOperationNode.MoveToNext("TeethCount", "");
            currentOperationTool.NumberOfTeeth = currentOperationNode.ValueAsInt;
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToNext("AxialOverhang", "");
            try
            {
                currentOperationTool.FullOverhang = currentOperationNode.ValueAsDouble;
            }
            catch (Exception)
            {
                /*throw new AppXmlAnalyzerExceptions("Неверный формат значения вылета инструмента \n" +
                                                    currentOperationTool.Name +
                                                    " поз. по проекту:" +
                                                    currentOperationTool.PositionNumber +
                                                    " поз. в коллекции: " +
                                                    projectTools.Count
                                                    );*/
                currentOperationTool.FullOverhang = 0;
            }
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToParent();
            currentOperationNode.MoveToParent();
        }

        void ReadAndCalculateHolderAndTailLength(XMLTool currentOperationTool, XPathNavigator currentOpXmlParamsNode)
        {
            XPathNodeIterator xPathHolderStepsIterator = currentOpXmlParamsNode.SelectChildren(currentOpXmlParamsNode.NodeType);
            double toolSetLength = 0;
            double holderNeckLength = 16;
            double holderMarkerValue = 0.999;
            double tailMarkerValue = 0.666;
            double tailOverhang = 0;
            double holderOverhang = 0;
            double sk50NeckDiameter = 97.55;
            double sk40NeckDiameter = 63.55;

            while (xPathHolderStepsIterator.MoveNext())
            {
                XPathNavigator currentHolderStep = xPathHolderStepsIterator.Current;
                currentHolderStep.MoveToChild("Diameter", "");
                double holderStepDiameter = currentHolderStep.ValueAsDouble;
                if (holderStepDiameter.Equals(tailMarkerValue) || holderStepDiameter.Equals(holderMarkerValue))
                {
                    currentHolderStep.MoveToPrevious();
                    if (holderStepDiameter.Equals(tailMarkerValue))
                    {
                        tailOverhang += toolSetLength;
                    }
                    holderOverhang += toolSetLength;
                    currentOperationTool.HolderOverhangValuesList.Add(toolSetLength);
                    toolSetLength = 0;
                }
                else if (holderStepDiameter != sk50NeckDiameter && holderStepDiameter != sk40NeckDiameter)
                {
                    currentHolderStep.MoveToPrevious();
                    toolSetLength += currentHolderStep.ValueAsDouble;
                }
                else
                {
                    break;
                }
                currentHolderStep.MoveToParent();
            }

            currentOperationTool.HolderLength = holderOverhang + holderNeckLength;
            currentOperationTool.TailOverhang = tailOverhang;
        }

        public List<XMLTool> ProjectToolList
        {
            get { return projectTools; }
        }

        public Operation OperationDescription
        {
            get { return operation; }
        }
    }
}
