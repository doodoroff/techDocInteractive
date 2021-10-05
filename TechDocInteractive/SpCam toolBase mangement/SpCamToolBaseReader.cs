using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace TechDocInteractive
{
    class SpCamToolBaseReader
    {
        string filePath;
        readonly string holderSectionStartMarker = "ID=STRING";
        readonly string holderSectionStopMarker = "<END HOLDERACC SECTION>";
        readonly string toolSectionStartMarker = "ID=INTEGER";
        readonly string toolSectionStopMarker = "<END MILLING TOOLS>";
        List<CsvTool> csvToolDataList;
        List<AuxToolAssembly> holderList;

        public SpCamToolBaseReader(string filePath)
        {
            this.filePath = filePath;
            this.csvToolDataList = new List<CsvTool>();
            this.holderList = new List<AuxToolAssembly>();
            AddToolDataInfo();
            AddHolderDataInfo();
        }

        public List<CsvTool> GetCsvToolDataList()
        {
            return csvToolDataList;
        }

        public List<AuxToolAssembly> GetHolderList()
        {
            return holderList;
        }


        void AddToolDataInfo()
        {
            CsvRedactor csvRedactor = new CsvRedactor(filePath, toolSectionStartMarker, toolSectionStopMarker);

            foreach (var toolInfoLine in csvRedactor)
            {
                CsvTool csvToolData = new CsvTool();
                csvToolData.Name = toolInfoLine[1];
                AddInsertPatterns(toolInfoLine[21], csvToolData);
                csvToolData.FromSpindelSideInterface = toolInfoLine[37];
                csvToolDataList.Add(csvToolData);
            }
            csvRedactor.Dispose();
        }

        void AddInsertPatterns(string insertPatternFromBase, CsvTool csvToolData)
        {
            char splitChar = '|';
            string[] splitPatterns = insertPatternFromBase.Split(splitChar);
            if (splitPatterns.Length > 1) 
            {
                csvToolData.InsertPattern1 = splitPatterns[0];
                csvToolData.InsertPattern2 = splitPatterns[1];
            }
            else
            {
                csvToolData.InsertPattern1 = splitPatterns[0];
                csvToolData.InsertPattern2 = "";
            }
        }

        void AddHolderDataInfo()
        {
            CsvRedactor csvRedactor = new CsvRedactor(filePath, holderSectionStartMarker, holderSectionStopMarker);
            //Holder holder = new Holder();

            foreach (var toolInfoLine in csvRedactor)
            {
                AuxToolAssembly auxToolAssembly = new AuxToolAssembly();
                auxToolAssembly.HolderCode = toolInfoLine[2];
                auxToolAssembly.Name = toolInfoLine[1];
                auxToolAssembly.Description = toolInfoLine[6];
                //AddHolderInterfaces(toolInfoLine[6], auxToolAssembly);
                //auxToolAssembly.AssemblyHoldersList = GenerateHolderList(toolInfoLine);
                holderList.Add(auxToolAssembly);
            }
            csvRedactor.Dispose();
        }
        


        /*List<Holder> GenerateHolderList(string[] toolInfoLine)
        {
            List<Holder> holders = new List<Holder>();
            string[] holderInterfaces = AddHolderInterfaces(toolInfoLine[6]);
            string[] holderNames = AddHolderNames(toolInfoLine[1]);

            if ((holderNames.Count() * 2) == holderInterfaces.Count()) 
            {
                for (int i = 0; i < holderNames.Count(); i++)
                {
                    Holder holder = new Holder();
                    holder.Name = holderNames[i];
                    holder.FromSpindelSideInterface = holderInterfaces[i * 2];
                    holder.FromCutSideInterface = holderInterfaces[(i * 2) + 1];
                    holders.Add(holder);
                }
            }

            return holders;
        }

        string[] AddHolderNames(string nameStringFromBase)
        {
            string nameString = SeparateCommentPartFromName(nameStringFromBase);
            char splitChar = '+';
            string[] splitNames = nameString.Split(splitChar);
            return splitNames;
        }

        void AddHolderInterfaces(string interfaceStringFromBase, AuxToolAssembly holder)
        {
            string interfacesString = SeparateCommentPartFromInterface(interfaceStringFromBase);
            char splitChar = '/';
            string[] splitInterfaces = interfacesString.Split(splitChar);
            if (splitInterfaces.Length > 1)
            {
                holder.FromSpindelSideInterface = splitInterfaces[0];
                holder.FromCutSideInterface = splitInterfaces[splitInterfaces.Length - 1];
            }
            else
            {
                holder.FromSpindelSideInterface = "";
                holder.FromCutSideInterface = "";
            }
        }

        string[] AddHolderInterfaces(string interfaceStringFromBase)
        {
            string interfacesString = SeparateCommentPartFromInterface(interfaceStringFromBase);
            char splitChar = '/';
            string[] splitInterfaces = interfacesString.Split(splitChar);
            return splitInterfaces;
        }

        string SeparateCommentPartFromInterface(string interfaceStringFromBase)
        {
            string formatedInterfaceStringFromBase = interfaceStringFromBase.Trim();
            char splitChar = ' ';
            string[] splitString = formatedInterfaceStringFromBase.Split(splitChar);
            return splitString[0];
        }

        string SeparateCommentPartFromName(string nameStringFromBase)
        {
            int splitStartPosition = nameStringFromBase.IndexOf('[');
            string separatedNamePart;
            if (splitStartPosition > 0) 
            {
                separatedNamePart = nameStringFromBase.Remove(splitStartPosition);
            }
            else
            {
                separatedNamePart = nameStringFromBase;
            }
            return separatedNamePart;
        }*/

       

    }
}
