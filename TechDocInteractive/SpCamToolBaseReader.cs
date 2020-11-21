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
        //StreamReader csvReader;
        List<CsvToolData> csvToolDataList;
        List<Holder> holderList;

        public SpCamToolBaseReader(string filePath)
        {
            this.filePath = filePath;
            this.csvToolDataList = new List<CsvToolData>();
            this.holderList = new List<Holder>();
            AddToolDataInfo();
            AddHolderDataInfo();
        }

        public List<CsvToolData> GetCsvToolDataList()
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
                CsvToolData csvToolData = new CsvToolData();
                csvToolData.Name = toolInfoLine[1];
                //csvToolData.InsertPattern = toolInfoLine[21];
                AddInsertPatterns(toolInfoLine[21], csvToolData);
                csvToolData.FromSpindelSideInterface = toolInfoLine[37];
                csvToolDataList.Add(csvToolData);
            }
            csvRedactor.Dispose();
        }

        void AddInsertPatterns(string insertPatternFromBase, CsvToolData csvToolData)
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

        /*void CheckCsvFile()
        {
            csvReader = new StreamReader(filePath, Encoding.Default);
            string csvHeader = csvReader.ReadLine();
            if (csvHeader != correctCsvHeader)
            {
                csvReader.Close();
                throw new AppXmlAnalyzerExceptions("Неверный заголовок файла базы инструмента");
            }
            csvReader.Close();
        }*/

        /*public List<CsvToolData> GetCsvToolData()
        {

        }*/

        /*string GetCellValue(int columnIndex, int rowIndex)
        {
            string cellValue = "";
            string currentString = PerformRow(rowIndex);
            cellValue = GetValueFromRow(currentString, columnIndex);
            return cellValue;
        }

        string PerformRow(int rowIndex)
        {
            csvReader = new StreamReader(filePath, Encoding.Default);
            RollToToolTable(csvReader);
            string currentString = "";
            for (int i = 0; i < rowIndex; i++)
            {
                currentString = csvReader.ReadLine();
            }
            return currentString;
        }

        string GetValueFromRow(string currentRow, int positionInRow)
        {
            string[] arrayOfValues = currentRow.Split(new char[] { ';' });
            if (positionInRow > arrayOfValues.Length) 
            {
                throw new AppXmlAnalyzerExceptions("Номер колонки за пределами диапазона таблицы инструмента");
            }
            return arrayOfValues[positionInRow];
        }

        void RollToToolTable(StreamReader csvReader)
        {
            string currentRow = csvReader.ReadLine();
            while (!currentRow.Contains(startMarker))
            {
                if (currentRow.Equals(csvReader.EndOfStream))
                {
                    csvReader.Close();
                    throw new AppXmlAnalyzerExceptions("Маркер начала раздела .csv файла не найден");
                }
                currentRow = csvReader.ReadLine();
            }
            currentRow = csvReader.ReadLine(); // skip headline row
            currentRow = csvReader.ReadLine(); // skip column names row
        }

        public List<CsvToolData> AddToolInfo()
        {
            StreamReader streamReader = new StreamReader(filePath, Encoding.Default);
            List<CsvToolData> csvToolDataList = new List<CsvToolData>();

        }*/

        /*public List<Holder> ReadHoldersList()
        {
            StreamReader reader = new StreamReader(filePath, Encoding.Default); // TO DO: Consider about common streamReader
            CheckToolbaseFile(reader);
            List<Holder> holderList = new List<Holder>();
            string currentRow = reader.ReadLine();
            while (currentRow != holderSectionStartMarker)
            {
                currentRow = reader.ReadLine();
            }
            currentRow = reader.ReadLine(); // skip headline row
            currentRow = reader.ReadLine(); // skip column names row
            while (currentRow != holderSectionStopMarker)
            {
                string pattern = @"[^\s;][^;]*";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(currentRow, 2);
                Holder holder = new Holder();
                holder.Name = match.Value;
                holder.HolderCode = match.NextMatch().Value.Replace(',','.');
                holderList.Add(holder);
                currentRow = reader.ReadLine();
            }
            reader.Close();
            return holderList;
        }*/

        /*void CheckToolbaseFile(StreamReader reader)
        {
            string firstRow = reader.ReadLine();
            if (firstRow != "SPRUTCAM TOOLS LIBRARY")
            {
                reader.Close();
                throw new AppXmlAnalyzerExceptions("Неверный формат файла базы инструмента");
            }
        }*/

    }
}
