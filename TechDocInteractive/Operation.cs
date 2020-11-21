using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Operation
    {
        public string DetailName { get; set; }
        public string MachinetoolName { get; set; }
        List<Setup> setups;

        public Operation()
        {
            setups = new List<Setup>();
        }

        public List<Setup> Setups
        {
            get { return setups; }
        }

        public void AddSetup (Setup setup)
        {
            setups.Add(setup);
        }

        public double OperationMachiningTime
        {
            get
            {
                double operationMachiningTime = 0;

                foreach (var setup in setups)
                {
                    operationMachiningTime += setup.SetupMachiningTime;
                }

                return operationMachiningTime;
            }
        }

        public double OperationAuxiliaryTime
        {
            get
            {
                double operationAuxiliaryTime = 0;

                foreach (var setup in setups)
                {
                    operationAuxiliaryTime += setup.SetupAuxiliaryTime;
                }

                return operationAuxiliaryTime;
            }
        }
    }
}
