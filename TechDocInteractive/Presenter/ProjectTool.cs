using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class ProjectTool: IEquatable<ProjectTool>
    {
        int positionNumber;
        string name;
        string type;
        double diameter;
        double toolLength;
        double angle;
        double edgeRadius;
        double workingLength;
        double holderSetLength;
        double fullOverhang;
        double toolOverhang;
        int numberOfTeeth;
        string holderCode;
        //double fullOverhang;
        //double holderLength;
        //double workingOverhang;
        double tailOverhang;
        double durability;
        //List<Holder> holders;
        string catalogInsertPattern1, catalogInsertPattern2;
        string toolFromSpindelSideInterface;
        string toolFromCutSideInterface;
        string holderSpindelSideInterface;
        string holderCutSideInterface;
        AuxToolAssembly auxToolAssembly;
        List<double> holderOverhangValuesList;


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
            get
            {
                switch (type)
                {
                    case "TrimAngle":
                        {
                            if (angle <= 45 && diameter > 32 && workingLength <= 8)
                            {
                                return "Торцевая фреза";
                            }
                            else if (angle >= 70)
                            {
                                return "Фреза для больших подач";
                            }
                            else
                            {
                                return "Фреза с угловой реж. кромкой";
                            }
                        }
                    case "EndMill":
                        {
                            return "Фреза";
                        }
                    case "Angle":
                        {
                            return "Фреза с угловой реж. кромкой";
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

        public double Diametr
        {
            get { return diameter; }

            set { this.diameter = value; }
        }

        public double ToolLength
        {
            get { return toolLength; }

            set { this.toolLength = value; }
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

        /*public List<Holder> HolderNames
        {
            get
            {
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

        /*public double TailOverhang
        {
            set { this.tailOverhang = value; }
        }*/

        /*public double FullOverhang
        {
            get { return fullOverhang; }

            set { this.fullOverhang = value; }
        }*/

        /*public double HolderLength
        {
            get { return holderLength; }

            set { this.holderLength = value; }
        }*/

        /*public double WorkingOverhang
        {
            get
            {
                if (type == "UndercutTool")
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

        public string InsertPattern1
        {
            get
            {
                string formatedinsertpattern = FormatInsertPattern(catalogInsertPattern1);

                return formatedinsertpattern;
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
        }

        string FormatCatalogInsertPattern(string catalogInsertPattern)
        {
            return catalogInsertPattern.Replace(" ", "");
        }

        string FormatCatalogEdgeRadius(string catalogEdgeRadius)
        {
            string formatedEdgeRadius = catalogEdgeRadius.Replace(",", "");

            if (catalogEdgeRadius.First().Equals('0'))
            {
                formatedEdgeRadius = formatedEdgeRadius.Remove(0, 1);
            }

            return formatedEdgeRadius;
        }

        public string CutToolFromSpindelSideInterface
        {
            //get
            //{
            //    string formatedSpindelSideInterface = FormatFromSpindelSideInterface(toolFromSpindelSideInterface);

            //    return formatedSpindelSideInterface;
            //}

            set {
                    List<Holder> holders = null;

                    if (auxToolAssembly != null)
                    {
                        holders = auxToolAssembly.AssemblyHoldersList;
                    }
                    if (holders != null && holders.Count > 0) 
                    {
                        string holderCutSideInterface = holders.Last().FromCutSideInterface;
                        toolFromSpindelSideInterface = FormatFromSpindelSideInterface(value, holderCutSideInterface);
                    }
                    else
                    {
                        toolFromSpindelSideInterface = "";
                    }
                }
        }

        public ArrayList AuxToolSpecification
        {
            get {
                if (auxToolAssembly != null)
                {
                    return CreateAuxToolSpecification();
                }
                return null;
            }
        }

        ArrayList CreateAuxToolSpecification()
        {
            List<Holder> holders;
            if (holderOverhangValuesList == null)
            {
                holders = auxToolAssembly.AssemblyHoldersList;
            }
            else
            {
                holders = auxToolAssembly.AssignHolderOverhangAndGenerateHolderList(holderOverhangValuesList);
            }

            ArrayList auxToolSpecification = new ArrayList();

            for (int i = 0; i < holders.Count; i++) 
            {
                if (i < holders.Count - 1) 
                {
                    Holder currentHolder = holders[i];
                    Holder nextHolder = holders[i + 1];
                    Hub hub = new Hub();
                    //string holderName = currentHolder.Name;
                    string holderHubPattern = FormatFromSpindelSideInterface(nextHolder.FromSpindelSideInterface, currentHolder.FromCutSideInterface);
                    hub.NamePattern = holderHubPattern;
                    auxToolSpecification.Add(currentHolder);
                    auxToolSpecification.Add(hub);
                }
                else
                {
                    Holder currentHolder = holders[i];
                    Hub hub = new Hub();
                    //string holderName = currentHolder.Name;
                    //string holderHubPattern = FormatFromSpindelSideInterface(toolFromSpindelSideInterface, currentHolder.FromCutSideInterface);
                    string holderHubPattern = toolFromSpindelSideInterface;
                    hub.NamePattern = holderHubPattern;
                    auxToolSpecification.Add(currentHolder);
                    auxToolSpecification.Add(hub);
                }
            }

            return auxToolSpecification;
        }

        string FormatFromSpindelSideInterface(string toolFromSpindelSideInterface, string holderCutSideInterface)
        {
            if (toolFromSpindelSideInterface != "")
            {
                string spindelSideInterfaceDiametr = ExtractDiameterValueFromInterface(toolFromSpindelSideInterface);
                string cutSideInterfaceDiametr = ExtractDiameterValueFromInterface(holderCutSideInterface);

                if (holderCutSideInterface.Contains("ER"))
                {
                    if (toolFromSpindelSideInterface.Contains("MCH"))
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
        }

        string ExtractDiameterValueFromInterface(string holderInterface)
        {
            if (holderInterface != "" && holderCutSideInterface != "")
            {
                return holderInterface.Remove(0, holderInterface.Length - 2);
            }
            return "0";
        }

        public string HolderSpindelSideInterface
        {
            //get { return holderSpindelSideInterface; }

            set { holderSpindelSideInterface = value; }
        }

        public string HolderCutSideInterface
        {
            //get { return holderCutSideInterface; }

            set { holderCutSideInterface = value; }
        }

        public AuxToolAssembly AuxToolAssembly
        {
            set { auxToolAssembly = value; }

            get { return auxToolAssembly; }
        }


        public List<double> HolderOverhangValuesList
        {
            get { return holderOverhangValuesList; }

            set
            {
                List<double> holderOverhangsWithCorrectOrder = value;
                holderOverhangsWithCorrectOrder.Reverse();
                holderOverhangValuesList = holderOverhangsWithCorrectOrder;
            }
        }

        public double HolderSetLength
        {
            //get { return holderSetLength; }

            set { holderSetLength = value; }
        }

        public double FullOverhang
        {
            //get { return fullOverhang; }

            set { fullOverhang = value; }
        }

        public double ToolOverhang
        {
            get
            {
                    if (/*type == "UndercutTool"*/ tailOverhang != 0) 
                    {
                        toolOverhang = tailOverhang + toolLength;
                    }
                    else
                    {
                        toolOverhang = fullOverhang - holderSetLength;
                    }

                    return toolOverhang;
            }
        }

        public double TailOverhang
        {
            set { this.tailOverhang = value; }
        }

        public bool Equals(ProjectTool other)
        {
            return positionNumber.Equals(other.positionNumber);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ProjectTool ToolObj = obj as ProjectTool;
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

    

