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
    class CsvRedactor: IEnumerator<string[]>, IEnumerable<string[]>
    {
        string filePath;
        StreamReader csvReader;
        string correctCsvHeader = "SPRUTCAM TOOLS LIBRARY";
        string startMarker; // = "ID=INTEGER";
        string stopMarker; // = "<END MILLING TOOLS>";
        string rowSeparator = ";";

        string currentRow = "";

        public string[] Current => GetRowArr(this.currentRow);

        object IEnumerator.Current => (object)GetRowArr(this.currentRow);

        /*public CsvRedactor(string filePath)
        {
            this.filePath = filePath;
            CheckCsvFile();
        }*/

        public CsvRedactor(string filePath, string startMarker, string stopMarker)
        {
            this.filePath = filePath;
            this.startMarker = startMarker;
            this.stopMarker = stopMarker;
            this.csvReader = new StreamReader(filePath, Encoding.Default);
            CheckCsvFile(csvReader);
            RollToSection(csvReader);
        }

        void CheckCsvFile(StreamReader csvReader)
        {
            //csvReader = new StreamReader(filePath, Encoding.Default);
            string csvHeader = csvReader.ReadLine();
            if (csvHeader != correctCsvHeader)
            {
                csvReader.Close();
                throw new AppXmlAnalyzerExceptions("Неверный заголовок файла базы инструмента");
            }
            //csvReader.Close();
        }

        void RollToSection(StreamReader csvReader)
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
            //currentRow = csvReader.ReadLine(); // skip headline row
            //currentRow = csvReader.ReadLine(); // skip column names row
            this.currentRow = currentRow;
        }

        /*public string GetCellValue(int columnIndex, int rowIndex)
        {
            string cellValue = "";
            string currentString = PerformRow(rowIndex);
            cellValue = GetValueFromRow(currentString, columnIndex);
            return cellValue;
        }*/

        /*string PerformRow(int rowIndex)
        {
            csvReader = new StreamReader(filePath, Encoding.Default);
            RollToSection(csvReader);
            string currentString = "";
            for (int i = 0; i < rowIndex; i++)
            {
                currentString = csvReader.ReadLine();
            }
            return currentString;
        }*/



        string[] GetRowArr(string currentRow)
        {
            string[] arrayOfValues = currentRow.Split(new char[] { ';' });
            return arrayOfValues;
        }

        public void Dispose()
        {
            csvReader.Close();
        }

        public bool MoveNext()
        {
            currentRow = csvReader.ReadLine();
            if (!currentRow.Equals(stopMarker) && !currentRow.Equals(csvReader.EndOfStream))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            csvReader.Close();
            csvReader = new StreamReader(filePath, Encoding.Default);
            RollToSection(csvReader);
        }

        public IEnumerator<string[]> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this as IEnumerator;
        }

        /*StreamReader csvReader;
        string filePath;
        string toolTableStartMarker = "ID=INTEGER";
        string toolTableEndMarker = "<END MILLING TOOLS>";
        int insertPatternTablePosition = 12;

        public CsvRedactor(string filePath)
        {
            this.filePath = filePath;
        }

        public Tool AddInfoFromBaseToTool(Tool tool) // TO DO: refactoring
        {
            csvReader = new StreamReader(filePath, Encoding.Default);
            Tool currentTool = tool;
            string pattern = @"[^\s;][^;]*";
            string currentRow = csvReader.ReadLine();
            if (!currentRow.Contains("SPRUTCAM TOOLS LIBRARY")) // TO DO: develop exception handler
            {
                return currentTool;
            }
            while (!currentRow.Contains(tool.Name))
            {
                currentRow = csvReader.ReadLine();
            }
            Regex regex = new Regex(pattern);
            Match match = regex.Match(currentRow, insertPatternTablePosition);
            currentTool.InsertPattern = match.Value;
            csvReader.Close();
            return currentTool;
        }

        void CheckToolbaseFile(StreamReader csvReader)
        {
            string firstRow = csvReader.ReadLine();
            if (firstRow != "SPRUTCAM TOOLS LIBRARY")
            {
                csvReader.Close();
                throw new AppXmlAnalyzerExceptions("Неверный формат файла базы инструмента");
            }
        }*/
    }
}
