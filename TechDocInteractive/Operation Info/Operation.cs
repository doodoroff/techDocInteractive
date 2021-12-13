using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Operation
    {
        double operationMachiningTime;
        double operationAuxiliaryTime;
        double operationCommonTime;

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
            CalculateLaborIntensity();
        }

        void CalculateLaborIntensity()
        {
            double operationMachiningTime = 0;
            double operationAuxiliatyTime = 0;

            foreach (Setup setup in setups)
            {
                operationMachiningTime += setup.SetupMachiningTime;
                operationAuxiliatyTime += setup.SetupAuxiliaryTime;
            }

            this.operationMachiningTime = operationMachiningTime;
            this.operationAuxiliaryTime = operationAuxiliatyTime;
            this.operationCommonTime = operationMachiningTime + operationAuxiliatyTime;
        }

        public string OperationMachiningTime
        {
            get
            {
                return operationMachiningTime.ToString("0.00") + " мин.";
            }
        }

        public string OperationAuxiliaryTime
        {
            get
            {
                return operationAuxiliaryTime.ToString("0.00") + " мин.";
            }
        }

        public string OperationCommonTime
        {
            get
            {
                return operationCommonTime.ToString("0.00") + " мин.";
            }
        }


    }
}
