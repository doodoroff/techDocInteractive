using System;
using System.Collections.Generic;
using System.Collections;
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
        ArrayList shiftDescriptionNotes;
        //ArrayList shiftDescriptionNotesContainer;
        //ArrayList shiftStructure;
        //List<string> shiftGroupLabels;
        List<string> shiftDescription;

        public Shift()
        {
            shiftDescriptionNotes = new ArrayList();
            //shiftDescriptionNotesContainer = new ArrayList();
            //shiftGroupLabels = new List<string>();

            machiningTime = 0;
            auxiliaryTime = 0;
            toolPath = 0;
        }

        public int ShiftToolId { get; set; }

        public XMLTool Tool { get; set; }
        //public string ShiftDescription { get; set; }
        //public string ShiftLabel { get; set; }

        //public void AddShifDescriptionNote(string shiftDescriptionNote)
        //{
        //    shiftDescriptionNotes.Add(shiftDescriptionNote);
        //}

        public void AddShifDescriptionNote(List<string> shiftDescriptionNote)
        {
            shiftDescriptionNotes.Add(shiftDescriptionNote);
        }

        public ArrayList GetShiftDescriptionNotes()
        {
            return shiftDescriptionNotes;
        }

        //public void AddShifDescriptionNote(List<string> blockOfShiftDescriptionNotes)
        //{
        //    shiftDescriptionNotes.Add(blockOfShiftDescriptionNotes);
        //}

        //public void AddShiftGroupLabel(string shiftGroupLabel)
        //{
        //    shiftGroupLabels.Add(shiftGroupLabel);
        //}

        public void AddMachiningTime(string machiningTime)
        {
            this.machiningTime += ParceTechInfoToDouble(machiningTime);
        }

        public void AddMachiningTime(double machiningTime)
        {
            this.machiningTime += machiningTime;
        }

        public void AddIdlingTime(string idlingTime)
        {
            this.idlingTime += ParceTechInfoToDouble(idlingTime);
        }

        public void AddIdlingTime(double idlingTime)
        {
            this.idlingTime += idlingTime;
        }

        public void AddWorkingTime(string workingTime)
        {
            this.workingTime += ParceTechInfoToDouble(workingTime);
        }

        public void AddWorkingTime(double workingTime)
        {
            this.workingTime += workingTime;
        }

        public void AddAuxiliaryTime(string auxiliaryTime)
        {
            this.auxiliaryTime += ParceTechInfoToDouble(auxiliaryTime);
        }

        public void AddAuxiliaryTime(double auxiliaryTime)
        {
            this.auxiliaryTime += auxiliaryTime;
        }

        public void AddRapidTime(string rapidTime)
        {
            this.rapidTime += ParceTechInfoToDouble(rapidTime);
        }

        public void AddRapidTime(double rapidTime)
        {
            this.rapidTime += rapidTime;
        }

        public void AddToolPath(string toolPath)
        {
            this.toolPath += ParceTechInfoToDouble(toolPath);
        }

        public void AddToolPath(double toolPath)
        {
            this.toolPath += toolPath;
        }

        public double GetMachiningTimeSeconds()
        {
            return machiningTime;// / 60;
        }

        public double GetIdlingTimeSeconds()
        {
            return (idlingTime + workingTime);// / 60;
        }

        public double GetAuxiliaryTimeSeconds()
        {
            return auxiliaryTime;// / 60;
        }

        public double GetRapidTimeSeconds()
        {
            return rapidTime;// / 60;
        }

        public double GetMachiningTimeMinutes()
        {
            return machiningTime / 60;
        }

        public double GetIdlingTimeMinutes()
        {
            return (idlingTime + workingTime) / 60;
        }

        public double GetAuxiliaryTimeMinutes()
        {
            return auxiliaryTime / 60;
        }

        public double GetRapidTimeMinutes()
        {
            return rapidTime / 60;
        }

        public double GetToolPath()
        {
            return toolPath;
        }

        public List<string> ShiftDescription
        {
            get
            {
                return EditShiftDescriptionNotes(shiftDescriptionNotes);
            }
            //set
            //{
            //    shiftDescriptionNotes = value;
            //}
        }

        List<string> EditShiftDescriptionNotes(ArrayList shiftDescriptionNotes)
        {
            List<string> editedShiftDescriptionNotes = new List<string>();
            int labelIndex = 0;
            int noteIndex = 1;

            foreach (List<string> notes in shiftDescriptionNotes) 
            {
                foreach (string note in notes)
                {
                    if (notes.Count == 1) 
                    {
                        labelIndex++;
                        string editedNote = labelIndex.ToString() + ". " + note;
                        editedShiftDescriptionNotes.Add(editedNote);
                        labelIndex++;
                    }
                    else
                    {
                        if (note.StartsWith("#"))
                        {
                            labelIndex++;
                            string editedNote = labelIndex.ToString() + ". " + note.Remove(0, 1);
                            editedShiftDescriptionNotes.Add(editedNote);
                            noteIndex = 1;
                        }
                        else
                        {
                            string editedNote = "";

                            if (labelIndex == 0)
                            {
                                editedNote = noteIndex.ToString() + ". " + note;
                                editedShiftDescriptionNotes.Add(editedNote);
                                noteIndex++;
                            }
                            else
                            {
                                editedNote = "   " + labelIndex.ToString() + "." + noteIndex.ToString() + ". " + note;
                                editedShiftDescriptionNotes.Add(editedNote);
                                noteIndex++;
                            }
                        }
                    }                   
                }
            }

            return editedShiftDescriptionNotes;
        }
        //public ArrayList ShiftStructure
        //{
        //    set { this.shiftStructure = value; }
        //}

        //public List<string> GetShiftDescription()
        //{
        //    List<string> result = new List<string>();

        //    if (shiftStructure != null) 
        //    {
        //        result = PerformShiftDescription(shiftStructure);
        //    }

        //    return result;
        //}

        //List<string> PerformShiftDescription(ArrayList shiftStructure)
        //{
        //    List<string> shiftDescription = new List<string>();

        //    foreach (var node in shiftStructure)
        //    {
        //        if (node is ArrayList)
        //        {
        //            List<string> shiftDescriptionFromNode = PerformShiftDescription(node as ArrayList);
        //            shiftDescription.AddRange(shiftDescriptionFromNode);
        //        }
        //        else
        //        {
        //            string shiftDescriptionNote = node as string;
        //            shiftDescription.Add(shiftDescriptionNote);
        //        }
        //    }

        //    return shiftDescription;
        //}

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

        string ParceTechInfoToString(double techInfo)
        {
            if (techInfo.Equals(double.NaN))
            {
                return "0";
            }

            double valueToParce = techInfo / 60;

            string result = valueToParce.ToString("0.00");

            return result;
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
