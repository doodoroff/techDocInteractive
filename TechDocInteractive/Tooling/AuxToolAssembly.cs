using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class AuxToolAssembly
    {
        string name;
        string description;
        string holderCode;
        //string fromSpindelSideInterface;
        //string fromCutSideInterface;

        List<Holder> assemblyHoldersList;

        public AuxToolAssembly()
        {
            assemblyHoldersList = new List<Holder>();
        }

        public string Name
        {
            //get { return name.Trim(); }

            set { this.name = value; }
        }

        public string Description
        {
            //get { return description; }

            set { description = value; }
        }

        public string HolderCode
        {
            get
            {
                if (holderCode != null)
                {
                    return holderCode.Trim().Replace(',', '.');
                }
                else
                {
                    return "0";
                }
            }

            set { this.holderCode = value; }
        }

        /*public string FromSpindelSideInterface
        {
            get { return fromSpindelSideInterface; }

            set { this.fromSpindelSideInterface = value; }
        }*/

        //public string FromCutSideInterface
        //{
        //    get { return fromCutSideInterface; }
        //}

        public List<Holder> AssemblyHoldersList
        {
            get
            {
                assemblyHoldersList = GenerateHolderList();
                return assemblyHoldersList;
            }

            //set { assemblyHoldersList = value; }
        }

        List<Holder> GenerateHolderList()
        {
            List<Holder> holders = new List<Holder>();

            if (name == null || description == null)
            {
                Holder holder = new Holder();
                holder.Name = "Инструмент не найден в базе SprutCam";
                holder.Description = "";
                holder.FromCutSideInterface = "";
                holder.FromSpindelSideInterface = "";
                holders.Add(holder);
                return holders;
            }

            string[] holderInterfaces = AddHolderInterfaces(description);
            string[] holderNames = AddHolderNames(name);
            string[] holderDescriptions = AddHolderDescriptions(name);

            if ((holderNames.Count() * 2) == holderInterfaces.Count())
            {
                for (int i = 0; i < holderNames.Count(); i++)
                {
                    Holder holder = new Holder();
                    holder.Name = holderNames[i];
                    if (i >= holderDescriptions.Length)
                    {
                        holder.Description = "Тип держателя не распознан";
                    }
                    else
                    {
                        holder.Description = holderDescriptions[i];
                    }
                    holder.FromSpindelSideInterface = holderInterfaces[i * 2];
                    holder.FromCutSideInterface = holderInterfaces[(i * 2) + 1];
                    holders.Add(holder);
                }
            }
            else
            {
                Holder errorHolder = new Holder();
                errorHolder.Name = "Тип держателя не распознан";
                errorHolder.Description = "!!! Ошибка соответствия интерфейсов держателя";
                holders.Add(errorHolder);
            }

            return holders;
        }

        public List<Holder> AssignHolderOverhangAndGenerateHolderList(List<double> holderOverhangList)
        {
            List<Holder> holders = new List<Holder>();
            holders = GenerateHolderList();

            if (holderOverhangList.Count != 0) 
            {
                for (int i = 0; i < holders.Count; i++)
                {
                    holders[i].Overhang = holderOverhangList[i];

                    if (holders[i].Description.Contains("SK50") || holders[i].Description.Contains("SK40"))
                    {
                        double holderNeckLength = 16;
                        holders[i].Overhang += holderNeckLength;
                    }
                    else if (holders[i].Description.Contains("HSK-A100"))
                    {
                        double holderNeckLength = 29;
                        holders[i].Overhang += holderNeckLength;
                    }
                }
            }

            return holders;
        }

        /*string[] GetParametrArrayFromString(string inputString, char separator)
        {
            string[] resultedArr = inputString.Split(separator);
            return resultedArr;
        }*/

        string[] AddHolderNames(string nameString)
        {
            string currentNameString = SeparateCommentPartFromName(nameString);
            char splitChar = '+';
            string[] splitNames = currentNameString.Split(splitChar);
            return splitNames;
        }

        string[] AddHolderInterfaces(string interfaceString)
        {
            string currentInterfaceString = SeparateCommentPartFromInterface(interfaceString);
            char splitChar = '/';
            string[] splitInterfaces = currentInterfaceString.Split(splitChar);
            return splitInterfaces;
        }

        string[] AddHolderDescriptions(string descriptionString)
        {
            string currentDescriptionString = SeparateNamePartFromString(descriptionString);
            char splitChar = '+';
            string[] splitComment = currentDescriptionString.Split(splitChar);
            return splitComment;
        }

        string SeparateCommentPartFromInterface(string interfaceString)
        {
            string formatedInterfaceString = interfaceString.Trim();
            char splitChar = ' ';
            string[] splitString = formatedInterfaceString.Split(splitChar);
            return splitString[0];
        }

        string SeparateCommentPartFromName(string nameString)
        {
            int splitStartPosition = nameString.IndexOf('[');
            string separatedNamePart;
            if (splitStartPosition > 0) 
            {
                separatedNamePart = nameString.Remove(splitStartPosition);
            }
            else
            {
                separatedNamePart = nameString;
            }
            return separatedNamePart;
        }

        string SeparateNamePartFromString(string nameString)
        {
            int splitStartPosition = nameString.IndexOf('[');
            string separatedCommentPart;
            if (splitStartPosition > 0) 
            {
                separatedCommentPart = nameString.Remove(0, splitStartPosition - 1);
            }
            else
            {
                separatedCommentPart = nameString;
            }
            return separatedCommentPart;
        }
    }
}
