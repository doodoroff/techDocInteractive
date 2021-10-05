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
            get { return holderCode.Trim().Replace(',', '.'); }

            set { this.holderCode = value; }
        }

        /*public string FromSpindelSideInterface
        {
            get { return fromSpindelSideInterface; }

            set { this.fromSpindelSideInterface = value; }
        }

        public string FromCutSideInterface
        {
            get { return fromCutSideInterface; }

            set { fromCutSideInterface = value; }
        }*/

        public List<Holder> AssemblyHoldersList
        {
            get {
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

            return holders;
        }

        public List<Holder> AssignHolderOverhangAndGenerateHolderList(List<double> holderOverhangList)
        {
            List<Holder> holders = new List<Holder>();

            if (name == null || description == null)
            {
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
                    if (holderOverhangList.Count == holderNames.Length) 
                    {
                        holder.Overhang = holderOverhangList[i];
                    }
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
