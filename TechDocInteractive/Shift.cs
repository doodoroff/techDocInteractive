using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Shift
    {
        public Tool Tool { get; set; }
        public string ShiftDescription { get; set; }
        public double MachiningTime { get; set;}
        public double AuxiliaryTime { get; set; }
        public double ToolPath { get; set; }
    }
}
