using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace TechDocInteractive
{
    class SpCamXMLReader
    {
        List<ToolSet> projectTools;
        Operation operation;
        Setup setup;
        Shift shift;
        XmlDocument xmlDocument;
        XPathNavigator xPathNavigator;
        string techFolderText = "";

        public SpCamXMLReader(string filePath)
        {
            this.projectTools = new List<ToolSet>();
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

            XPathNodeIterator xPathTechnologyIterator = CreateSectionIterator("Technology");
            XPathNodeIterator xPathOpXmlParamsIterator = CreateSectionIterator("OpXmlParams");

            while (xPathOpXmlParamsIterator.MoveNext() && xPathTechnologyIterator.MoveNext())
            {
                ToolSet currentOperationTool = new ToolSet();
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
                    if (IsNodeSetup(currentOpXmlParamsNode))
                    {
                        if (setup != null)
                        {
                            setup.AddShift(shift);
                            operation.AddSetup(setup);
                            shift = null;
                        }

                        this.setup = new Setup();
                        currentOpXmlParamsNode.MoveToChild("Comment", "");
                        setup.SetupName = currentOpXmlParamsNode.Value;
                        currentOpXmlParamsNode.MoveToParent();
                    }
                    else
                    {
                        techFolderText = ReadTechDescriptionFromOpXmlSection(currentOpXmlParamsNode) + "Обрабатываемые элементы:" + "\n";
                    }
                }
                else
                {
                    GetToolInfoFromTechnologySection(currentOperationTool, currentTechnologyNode);
                    GetToolInfoFromOpXmlParamsSection(currentOperationTool, currentOpXmlParamsNode);
                    GetHolderInfoFromOpXmlParamsSection(currentOperationTool, currentOpXmlParamsNode);
                    GetTechnologyInfo(currentOperationTool, currentOpXmlParamsNode, currentTechnologyNode, setup);
                    projectTools.Add(currentOperationTool);
                }

            }
            setup.AddShift(shift);
            operation.AddSetup(setup);
        }

        bool IsNodeSetup(XPathNavigator currentOpXmlParamsNode)
        {
            currentOpXmlParamsNode.MoveToChild("Icon", "");
            string imageFullName = currentOpXmlParamsNode.Value;
            currentOpXmlParamsNode.MoveToParent();
            if (imageFullName.Contains("Group_setup"))
            {
                return true;
            }
            return false;
        }

        void GetTechnologyInfo(ToolSet currentOperationTool, XPathNavigator currentOpXmlParamsNode, XPathNavigator currentTechnologyNode, Setup currentSetup)
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
                shift.ShiftDescription += techFolderText + ReadTechDescriptionFromOpXmlSection(currentOpXmlParamsNode);
                ReadTechDescriptionFromTechnologySection(currentTechnologyNode, shift);
            }
            else
            {
                shift.ShiftDescription += techFolderText + ReadTechDescriptionFromOpXmlSection(currentOpXmlParamsNode);
                ReadTechDescriptionFromTechnologySection(currentTechnologyNode, shift);
            }

            techFolderText = "";
        }

        string ReadTechDescriptionFromOpXmlSection(XPathNavigator currentOpXmlParamsNode)
        {
            string currentShiftDescription;
            currentOpXmlParamsNode.MoveToChild("Comment", "");
            currentShiftDescription = currentOpXmlParamsNode.Value + "\n";
            currentOpXmlParamsNode.MoveToParent();

            return currentShiftDescription;
        }

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

        void GetToolInfoFromTechnologySection(ToolSet currentOperationTool, XPathNavigator currentTechnologyNode)
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

        void GetToolInfoFromOpXmlParamsSection(ToolSet currentOperationTool, XPathNavigator currentOpXmlParamsNode)
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

        void GetHolderInfoFromOpXmlParamsSection(ToolSet currentOperationTool, XPathNavigator currentOpXmlParamsNode)
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

        void ReadCustomToolOpXmlParamsCode(XPathNavigator currentOperationNode, ToolSet currentOperationTool)
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

        void ReadToolOpXmlParamsCode(XPathNavigator currentOperationNode, ToolSet currentOperationTool)
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

        void ReadAndCalculateHolderAndTailLength(ToolSet currentOperationTool, XPathNavigator currentOpXmlParamsNode) // TO DO Refactoring, transfer to ToolSet class ?
        {
            XPathNodeIterator xPathHolderStepsIterator = currentOpXmlParamsNode.SelectChildren(currentOpXmlParamsNode.NodeType);
            double toolSetLength = 0;
            double holderNeckLength = 16;
            double tailMarkerValue = 0.0999;
            double tailOverhang = 0;
            double sk50NeckDiameter = 97.55;
            double sk40NeckDiameter = 63.55;

            while (xPathHolderStepsIterator.MoveNext())
            {
                XPathNavigator currentHolderStep = xPathHolderStepsIterator.Current;
                currentHolderStep.MoveToChild("Diameter", "");
                double holderStepDiameter = currentHolderStep.ValueAsDouble;
                if (holderStepDiameter != sk50NeckDiameter && holderStepDiameter != sk40NeckDiameter)
                {
                    currentHolderStep.MoveToPrevious();
                    if (currentHolderStep.ValueAsDouble.Equals(tailMarkerValue))
                    {
                        tailOverhang += toolSetLength;
                        toolSetLength = 0 - tailMarkerValue;
                    }
                    toolSetLength += currentHolderStep.ValueAsDouble;
                }
                else
                {
                    break;
                }
                currentHolderStep.MoveToParent();
            }

            currentOperationTool.HolderLength = toolSetLength + holderNeckLength;
            currentOperationTool.TailOverhang = tailOverhang;
        }

        public List<ToolSet> ProjectToolList
        {
            get { return projectTools; }
        }

        public Operation OperationDescription
        {
            get { return operation; }
        }
    }
}
