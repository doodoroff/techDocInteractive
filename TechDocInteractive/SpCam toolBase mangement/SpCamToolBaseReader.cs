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
        List<Holder> holderList;

        public SpCamToolBaseReader(string filePath)
        {
            this.filePath = filePath;
            this.csvToolDataList = new List<CsvTool>();
            this.holderList = new List<Holder>();
            AddToolDataInfo();
            AddHolderDataInfo();
        }

        public List<CsvTool> GetCsvToolDataList()
        {
            return csvToolDataList;
        }

        public List<Holder> GetHolderList()
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

            foreach (var toolInfoLine in csvRedactor)
            {
                Holder holder = new Holder();
                holder.HolderCode = toolInfoLine[2].Replace(',', '.');
                holder.Name = toolInfoLine[1];
                AddHolderInterfaces(toolInfoLine[6], holder);
                holderList.Add(holder);
            }
            csvRedactor.Dispose();
        }
        
        void AddHolderInterfaces(string interfaceStringFromBase, Holder holder)
        {
            string interfacesString = SeparateCommentPartFromInterfaces(interfaceStringFromBase);
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

        string SeparateCommentPartFromInterfaces(string interfaceStringFromBase)
        {
            string formatedInterfaceStringFromBase = interfaceStringFromBase.Trim();
            char splitChar = ' ';
            string[] splitString = formatedInterfaceStringFromBase.Split(splitChar);
            return splitString[0];
        }

       

    }
}
