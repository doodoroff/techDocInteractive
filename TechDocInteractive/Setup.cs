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
                    setupMachiningTime += shift.MachiningTime;
                }

                return setupMachiningTime;
            }
        }

        public double SetupAuxiliaryTime
        {
            get
            {
                double setupAuxiliaryTime = 0;

                foreach (var shift in shifts)
                {
                    setupAuxiliaryTime += shift.AuxiliaryTime;
                }

                return setupAuxiliaryTime;
            }
        }
    }
}
