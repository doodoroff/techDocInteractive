using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Setup
    {
        double setupMachiningTime;
        double setupAuxiliaryTime;
        double setupCommonTime;

        public string SetupName { get; set; }
        public string SetupDevice { get; set; }
        //ArrayList shifts;
        List<Shift> shifts;

        public Setup()
        {
            shifts = new List<Shift>();
            //shifts = new ArrayList();
        }

        //public List<string> Shifts
        //{
        //    get
        //    {
        //        List<string> shiftListTest = GenerateShiftList();
        //        return shiftListTest;
        //    }
        //}

        public List<Shift> Shifts
        {
            get
            {
                return shifts;
            }
        }

        //public void Addshift(Shift shift)
        //{
        //    shiftList.Add(shift);
        //    //calculatelaborintensity();
        //}

        public void AddSetupShifts(List<Shift> blockOfShifts /*ArrayList blockOfShifts*/) // TO DO: Refactoring
        {
            //this.setupStructure.Add(blockOfShifts);
            this.shifts = blockOfShifts;
            CalculateLaborIntensity();
        }

        //public List<string> GenerateShiftList()
        //{
        //    List<string> result = new List<string>();

        //    if (shifts.Count != 0)
        //    {
        //        //result = GenerateShiftListAndCalculateWorkTime(shifts);
        //    }

        //    return result;
        //}

        //List<string> GenerateShiftListAndCalculateWorkTime(ArrayList blockOfshifts, int number = 1)
        //{
        //    List<string> shiftList = new List<string>();

        //    foreach (var item in blockOfshifts)
        //    {
        //        if (item is ArrayList)
        //        {
        //            ArrayList currentBlockOfShifts = item as ArrayList;
        //            List<string> currentShiftList = GenerateShiftListAndCalculateWorkTime(currentBlockOfShifts, number);
        //            shiftList.AddRange(currentShiftList);
        //            number++;
        //        }
        //        else if (item is string)
        //        {
        //            string blockOfShiftsLabel = item as string;
        //            //blockOfShiftsLabel = number.ToString() + ". " + blockOfShiftsLabel;
        //            shiftList.Add(blockOfShiftsLabel);
        //            //number++;
        //        }
        //        else
        //        {
        //            Shift currentShift = item as Shift;
        //            List<string> currentShiftDescription = currentShift.ShiftDescriptionNotes;
        //            if (currentShiftDescription.Count != 1)
        //            {
        //                currentShiftDescription = AssignOrderNumber(number, currentShiftDescription);
        //                shiftList.AddRange(currentShiftDescription);
        //            }
        //            else
        //            {
        //                string currentShiftDescriptionNote = currentShiftDescription.First();
        //                //currentShiftDescriptionNote = number.ToString() + ". " + currentShiftDescriptionNote;
        //                shiftList.Add(currentShiftDescriptionNote);
        //            }

        //            number++;
        //        }

        //        //number++;
        //    }

        //    return shiftList;
        //}

        //List<string> AssignOrderNumber(int startNumber, List<string> list)
        //{
        //    int orderNumber = 1;

        //    List<string> numberedList = new List<string>();

        //    foreach (string item in list)
        //    {
        //        string orderedNumber = startNumber.ToString() + "." + orderNumber.ToString() + " ";
        //        string numberedItem = item;
        //        numberedList.Add(numberedItem);
        //        orderNumber++;
        //    }

        //    return numberedList;
        //}

        //public void AddSetupStructure(Shift shift)
        //{
        //    this.setupStructure.Add(shift);
        //}

        void CalculateLaborIntensity()
        {
            double setupMachiningTime = 0;
            double setupAuxiliaryTime = 0;
            //double setupCommonTime = 0;

            foreach (Shift shift in shifts)
            {
                setupMachiningTime += shift.GetMachiningTimeMinutes();
                setupAuxiliaryTime += shift.GetAuxiliaryTimeMinutes();
                //setupCommonTime += setupMachiningTime + setupAuxiliaryTime;
            }

            this.setupMachiningTime = setupMachiningTime;
            this.setupAuxiliaryTime = setupAuxiliaryTime;
            this.setupCommonTime = setupMachiningTime + setupAuxiliaryTime;
        }

        public double SetupMachiningTime
        {
            get
            {
                return setupMachiningTime;
            }
        }

        public double SetupAuxiliaryTime
        {
            get
            {
                return setupAuxiliaryTime;
            }
        }

        public double SetupCommonTime
        {
            get
            {
                return setupCommonTime;
            }
        }

        public string SetupMachiningTimeInfo
        {
            get
            {
                return setupMachiningTime.ToString("0.00") + " мин.";
            }
        }

        public string SetupAuxiliaryTimeInfo
        {
            get
            {
                return setupAuxiliaryTime.ToString("0.00") + " мин.";
            }
        }

        public string SetupCommonTimeInfo
        {
            get
            {
                return setupCommonTime.ToString("0.00") + " мин.";
            }
        }
    }
}
