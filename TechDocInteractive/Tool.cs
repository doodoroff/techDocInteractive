using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TechDocInteractive
{
    class Tool: IEquatable<Tool>
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
        double workingOverhang;
        double tailOverhang;
        double durability;
        string holderName;
        string catalogInsertPattern1, catalogInsertPattern2;
        string fromSpindelSideInterface;
        string fromCutSideInterface;
        string holderSpindelSideInterface;
        string holderCutSideInterface;

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
            get {
                    switch (type)
                    {
                        case "TrimAngle":
                            {
                                return "Фреза для больших подач";
                            }
                        case "EndMill":
                            {
                                return "Фреза";
                            }
                        case "Angle":
                            {
                                return "Угловая фреза";
                            }
                        case "Torus":
                            {
                                return "Фреза";
                            }
                        case "Spherical":
                            {
                                return "Сферическая фреза";
                            }
                        case "UndercutTool":
                            {
                                return "Дисковая фреза";
                            }
                        case "NegativeRad":
                            {
                                return "Фреза с обратным радиусом";
                            }
                        case "Reamer":
                            {
                                return "Развёртка";
                            }
                        case "BoringTool":
                            {
                                return "Расточка";
                            }
                        case "ThreadMill":
                            {
                                return "Резьбовая фреза";
                            }
                        case "Drill":
                            {
                                return "Сверло";
                            }
                        case "Tap":
                            {
                                return "Метчик";
                            }

                        default:
                            {
                                return "Неизвестный инструмент";
                            }
                    }

            }

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
            get { return holderCode.Replace(';', ':'); }

            set { this.holderCode = value; }
        }

        public string HolderName
        {
            get { return holderName; }

            set { holderName = value; }
        }

        public double TailOverhang
        {
            set { this.tailOverhang = value; }
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

        public double WorkingOverhang
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
        }

        public double Durability
        {
            get { return durability; }

            set { this.durability = value; }
        }

        public string InsertPattern1
        {
            get {
                string formatedInsertPattern = FormatInsertPattern(catalogInsertPattern1);

                return formatedInsertPattern;
                }

            set { catalogInsertPattern1 = value; }
        }
        public string InsertPattern2
        {
            get
            {
                string formatedInsertPattern = FormatInsertPattern(catalogInsertPattern2);

                return formatedInsertPattern;
            }

            set { catalogInsertPattern2 = value; }
        }

        string FormatInsertPattern(string catalogInsertPattern)
        {
            if (catalogInsertPattern == "")
            {
                return "";
            }

            string formatedCatalogInsertPattern = FormatCatalogInsertPattern(catalogInsertPattern);

            string splitString = "..";
            string[] catalogInsertPatternsParts = formatedCatalogInsertPattern.Split(new string[] { splitString }, StringSplitOptions.None);
            string pattern;
            if (catalogInsertPatternsParts.Length > 1)
            {
                //XO...X10T3 pattern with 0.8 radius example: ((XO\S*\s*\d*\s*08\S*\s*\d*\s*X10T3)|(XO\S*\s*\d*\s*X10T3\S*\s*\d*\s*08\D))

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
                pattern = "(" + catalogInsertPatternsParts[0] + @"\s*\S*\d*\s*" + edgeRadius.ToString().Replace(",", "") + ")";
            }

            return pattern;
        }

        string FormatCatalogInsertPattern(string catalogInsertPattern)
        {
            return catalogInsertPattern.Replace(" ", "");
        }

        public string FromSpindelSideInterface
        {
            get
            {
                string formatedSpindelSideInterface = FormatFromSpindelSideInterface(fromSpindelSideInterface);

                return formatedSpindelSideInterface;
            }

            set { fromSpindelSideInterface = value; }
        }

        string FormatFromSpindelSideInterface(string fromSpindelSideInterface)
        {
            if (fromSpindelSideInterface != "")
            {
                if (holderCutSideInterface.Contains("ER") )
                {
                    if (fromSpindelSideInterface.Contains("MCH"))
                    {
                        return "1810" + holderCutSideInterface.Remove(0, holderCutSideInterface.Length - 2) + fromSpindelSideInterface.Remove(0, fromSpindelSideInterface.Length - 2);
                    }
                    else
                    {
                        return fromSpindelSideInterface.Remove(0, fromSpindelSideInterface.Length - 2) + holderCutSideInterface;
                    }
                }

                if (holderCutSideInterface.Contains("D"))
                {
                    return "05F5832" + holderCutSideInterface.Remove(0, 1) + fromSpindelSideInterface.Remove(0, fromSpindelSideInterface.Length - 2);
                }
            }
            return "";
        }

        public string HolderSpindelSideInterface
        {
            get { return holderSpindelSideInterface; }

            set { holderSpindelSideInterface = value; }
        }

        public string HolderCutSideInterface
        {
            get { return holderCutSideInterface; }

            set { holderCutSideInterface = value; }
        }

        public bool Equals(Tool other)
        {
            return positionNumber.Equals(other.positionNumber);
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            Tool ToolObj = obj as Tool;
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
