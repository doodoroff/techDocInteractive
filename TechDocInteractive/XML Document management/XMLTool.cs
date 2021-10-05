﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TechDocInteractive
{
    class XMLTool: IEquatable<XMLTool>
    {
        int positionNumber;
        string name;
        string type;
        bool customTool;
        double diameter;
        double length;
        double angle;
        double edgeRadius;
        double workingLength;
        int numberOfTeeth;
        string holderCode;
        double fullOverhang;
        double holderLength;
        //double workingOverhang;
        double tailOverhang;
        double durability;
        //List<Holder> holders;
        //string catalogInsertPattern1, catalogInsertPattern2;
        //string fromSpindelSideInterface;
        //string fromCutSideInterface; // TO DO Refactoring, necessity ?
        //string holderSpindelSideInterface;
        //string holderCutSideInterface;
        List<double> holderOverhangValuesList;

        public XMLTool()
        {
            holderOverhangValuesList = new List<double>();
        }

        public int PositionNumber
        {
            get { return positionNumber; }

            set { this.positionNumber = value; }
        }

        public string Name
        {
            get { return name; }

            set { this.name = value; }
        }

        public string Type
        {
            get { return type; }

            set { this.type = value; }
        }

        public bool CustomTool
        {
            get { return customTool; }

            set { this.customTool = value; }
        }

        public double Diametr
        {
            get { return diameter; }

            set { this.diameter = value; }
        }

        public double Length
        {
            get { return length; }

            set { this.length = value; }
        }

        public double Angle
        {
            get { return angle; }

            set { this.angle = value; }
        }

        public double EdgeRadius
        {
            get { return edgeRadius; }

            set { this.edgeRadius = value; }
        }

        public double WorkingLength
        {
            get { return workingLength; }

            set { this.workingLength = value; }
        }

        public int NumberOfTeeth
        {
            get { return numberOfTeeth; }

            set { this.numberOfTeeth = value; }
        }

        public string HolderCode
        {
            get { return holderCode; }

            set { this.holderCode = value; }
        }

        /*public List<Holder> HolderNames
        {
            get {
                    /*if (holders != null)
                    {
                        foreach (Holder holder in holders)
                        {
                            holder.Name.Trim();
                        }
                    }
                    return holders;
                }

            set { holders = value; }
        }*/

        public double TailOverhang
        {
            set { this.tailOverhang = value; }

            get { return tailOverhang; }
        }

        public double FullOverhang
        {
            get { return fullOverhang; }

            set { this.fullOverhang = value; }
        }

        public double HolderLength
        {
            get { return holderLength; }

            set { this.holderLength = value; }
        }

        /*public double WorkingOverhang
        {
            get {
                    if (type== "UndercutTool")
                    {
                        workingOverhang = tailOverhang + length;
                    }
                    else
                    {
                        workingOverhang = fullOverhang - HolderLength;
                    }
                    
                    return workingOverhang;
                }
        }*/

        public double Durability
        {
            get { return durability; }

            set { this.durability = value; }
        }

        /*public string InsertPattern1
        {
            get {
                string formatedInsertPattern = FormatInsertPattern(catalogInsertPattern1);

                return formatedInsertPattern;
                }

            set { catalogInsertPattern1 = value; }
        }*/
        /*public string InsertPattern2
        {
            get
            {
                string formatedInsertPattern = FormatInsertPattern(catalogInsertPattern2);

                return formatedInsertPattern;
            }

            set { catalogInsertPattern2 = value; }
        }*/

        /*string FormatInsertPattern(string catalogInsertPattern)
        {
            if (catalogInsertPattern == "")
            {
                return "";
            }

            string formatedCatalogInsertPattern = FormatCatalogInsertPattern(catalogInsertPattern);
            string formatedCatalogEdgeRadius = FormatCatalogEdgeRadius(edgeRadius.ToString());

            string splitString = "..";
            string[] catalogInsertPatternsParts = formatedCatalogInsertPattern.Split(new string[] { splitString }, StringSplitOptions.None);
            string pattern;
            if (catalogInsertPatternsParts.Length > 1)
            {
                //XO...X10T3 pattern with 0.8 radius example (no "space" char in string): ((XO\S*\s*\d*\s*08\S*\s*\d*\s*X10T3)|(XO\S*\s*\d*\s*X10T3\S*\s*\d*\s*08\D))

                pattern = "((" +
                            catalogInsertPatternsParts[0] +
                            @"\S*\s*\d*\s*" +
                            edgeRadius.ToString().Replace(",", "") +
                            @"\S*\s*\d*\s*" +
                            catalogInsertPatternsParts[1] +
                            ")|(" +
                            catalogInsertPatternsParts[0] +
                            @"\S*\s*\d*\s*" +
                            catalogInsertPatternsParts[1] +
                            @"\S*\s*\d*\s*" +
                            edgeRadius.ToString().Replace(",", "") +
                            @"\D" +
                            "))";
            }
            else if (catalogInsertPatternsParts.Length == 0)
            {
                pattern = "";
            }
            else
            {
                pattern = "(" + catalogInsertPatternsParts[0] + @"\S*\d" + formatedCatalogEdgeRadius + "[SPERMHT-]*" + ")";
            }

            return pattern;
        }*/

        /*string FormatCatalogInsertPattern(string catalogInsertPattern)
        {
            return catalogInsertPattern.Replace(" ", "");
        }*/

        /*string FormatCatalogEdgeRadius(string catalogEdgeRadius)
        {
            string formatedEdgeRadius = catalogEdgeRadius.Replace(",", "");

            if (catalogEdgeRadius.First().Equals('0'))
            {
                formatedEdgeRadius = formatedEdgeRadius.Remove(0, 1);
            }

            return formatedEdgeRadius;
        }*/

        /*public string FromSpindelSideInterface
        {
            get
            {
                string formatedSpindelSideInterface = FormatFromSpindelSideInterface(fromSpindelSideInterface);

                return formatedSpindelSideInterface;
            }

            set { fromSpindelSideInterface = value; }
        }*/

        /*string FormatFromSpindelSideInterface(string fromSpindelSideInterface)
        {
            if (fromSpindelSideInterface != "")
            {
                string spindelSideInterfaceDiametr = ExtractDiameterValueFromInterface(fromSpindelSideInterface);
                string cutSideInterfaceDiametr = ExtractDiameterValueFromInterface(holderCutSideInterface);

                if (holderCutSideInterface.Contains("ER") )
                {
                    if (fromSpindelSideInterface.Contains("MCH"))
                    {
                        return "1810" + cutSideInterfaceDiametr + spindelSideInterfaceDiametr;
                    }
                    else
                    {
                        return spindelSideInterfaceDiametr + holderCutSideInterface;
                    }
                }

                if (holderCutSideInterface.Contains("D"))
                {
                    return "05F5832" + cutSideInterfaceDiametr + spindelSideInterfaceDiametr;
                }

                if (holderCutSideInterface.Contains("ERHP"))
                {
                    return "5672S" + cutSideInterfaceDiametr + spindelSideInterfaceDiametr;
                }
            }
            return "";
        }*/

        /*string ExtractDiameterValueFromInterface(string holderInterface)
        {
            if (holderInterface != "" && holderCutSideInterface != "") 
            {
                return holderInterface.Remove(0, holderInterface.Length - 2);
            }
            return "0";
        }*/

        /*public string HolderSpindelSideInterface
        {
            get { return holderSpindelSideInterface; }

            set { holderSpindelSideInterface = value; }
        }*/

        /*public string HolderCutSideInterface
        {
            get { return holderCutSideInterface; }

            set { holderCutSideInterface = value; }
        }*/

        public List<double> HolderOverhangValuesList
        {
            get { return holderOverhangValuesList; }

            set { holderOverhangValuesList = value; }
        }

        public bool Equals(XMLTool other)
        {
            return positionNumber.Equals(other.positionNumber);
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            XMLTool ToolObj = obj as XMLTool;
            if (ToolObj == null)
            {
                return false;
            }
            else
            {
                return Equals(ToolObj);
            }
        }

        public override int GetHashCode()
        {
            return this.PositionNumber.GetHashCode();
        }
    }
}