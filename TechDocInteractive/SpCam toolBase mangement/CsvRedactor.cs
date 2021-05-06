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
        char columnSeparator = ';';

        string currentRow = "";

        public string[] Current => GetRowArr(this.currentRow);

        object IEnumerator.Current => (object)GetRowArr(this.currentRow);

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
            string csvHeader = csvReader.ReadLine();
            if (csvHeader != correctCsvHeader)
            {
                csvReader.Close();
                throw new AppXmlAnalyzerExceptions("Неверный заголовок файла базы инструмента");
            }
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
            this.currentRow = currentRow;
        }

        string[] GetRowArr(string currentRow)
        {
            string[] arrayOfValues = currentRow.Split(new char[] { columnSeparator });
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
    }
}
