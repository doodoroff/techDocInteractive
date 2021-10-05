using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class AuxToolInfo
    {
        public string AuxToolName { get; set; }
        public double AuxToolOverhang { get; set; }
        public List<string> HubList { get; set; }

        public string CurrentHub { get; set; }
    }
}
