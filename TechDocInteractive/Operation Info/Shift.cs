using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Shift
    {
        double machiningTime;
        double idlingTime;
        double workingTime;
        double auxiliaryTime;
        double rapidTime;
        double toolPath;

        public Shift()
        {
            machiningTime = 0;
            auxiliaryTime = 0;
            toolPath = 0;
        }

        public XMLTool Tool { get; set; }
        public string ShiftDescription { get; set; }

        public void AddMachiningTime(string machiningTime)
        {
            this.machiningTime += ParceTechInfoToDouble(machiningTime);
        }

        public void AddIdlingTime(string idlingTime)
        {
            this.idlingTime += ParceTechInfoToDouble(idlingTime);
        }

        public void AddWorkingTime(string workingTime)
        {
            this.workingTime += ParceTechInfoToDouble(workingTime);
        }

        public void AddAuxiliaryTime(string auxiliaryTime)
        {
            this.auxiliaryTime += ParceTechInfoToDouble(auxiliaryTime);
        }

        public void AddRapidTime(string rapidTime)
        {
            this.rapidTime += ParceTechInfoToDouble(rapidTime);
        }

        public void AddToolPath(string toolPath)
        {
            this.toolPath += ParceTechInfoToDouble(toolPath);
        }

        public double GetMachiningTime()
        {
            return machiningTime / 60;
        }

        public double GetIdlingTime()
        {
            return (idlingTime + workingTime) / 60;
        }

        public double GetAuxiliaryTime()
        {
            return auxiliaryTime / 60;
        }

        public double GetRapidTime()
        {
            return rapidTime / 60;
        }

        public double GetToolPath()
        {
            return toolPath;
        }

        double ParceTechInfoToDouble(string techInfo)
        {
            if (techInfo.Equals(""))
            {
                return 0;
            }

            double parcedValue;
            string formatedTechDiscription = techInfo.Replace('.', ',');

            try
            {
                parcedValue = double.Parse(formatedTechDiscription);
            }
            catch (Exception)
            {
                throw new AppXmlAnalyzerExceptions("Технологический параметр " + formatedTechDiscription + " имеет не верный формат числа");
            }

            return parcedValue;
        }

        /*string FormatAndParceTechInfoToString(double techInfo, string format)
        {
            double techInfoInMinutes = techInfo / 60;
            string formatedTechInfo = "";

            try
            {
                formatedTechInfo = techInfoInMinutes.ToString(format);
            }
            catch (Exception)
            {
                throw new AppXmlAnalyzerExceptions("задан не верный формат вывода числа");
            }

            return formatedTechInfo;
        }*/
    }
}
