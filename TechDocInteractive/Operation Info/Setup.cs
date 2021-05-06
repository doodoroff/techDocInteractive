using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Setup
    {
        public string SetupName { get; set; }
        public string SetupDevice { get; set; }
        List<Shift> shifts;

        public Setup()
        {
            shifts = new List<Shift>();
        }

        public List<Shift> Shifts
        {
            get { return shifts; }
        }

        public void AddShift(Shift shift)
        {
            shifts.Add(shift);
        }

        public double SetupMachiningTime
        {
            get
            {
                double setupMachiningTime = 0;

                foreach (var shift in shifts)
                {
                    setupMachiningTime += shift.GetMachiningTime();
                }

                return setupMachiningTime;
            }
        }

        public double SetupIdlingTime
        {
            get
            {
                double setupIdlingTime = 0;

                foreach (var shift in shifts)
                {
                    setupIdlingTime += shift.GetIdlingTime();
                }

                return setupIdlingTime;
            }
        }

        public double SetupAuxiliaryTime
        {
            get
            {
                double setupAuxiliaryTime = 0;

                foreach (var shift in shifts)
                {
                    setupAuxiliaryTime += shift.GetAuxiliaryTime();
                }

                return setupAuxiliaryTime;
            }
        }

        public double SetupRapidTime
        {
            get
            {
                double setupRapidTime = 0;

                foreach (var shift in shifts)
                {
                    setupRapidTime += shift.GetRapidTime();
                }

                return setupRapidTime;
            }
        }
    }
}
