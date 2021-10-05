using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class ToolInfo
    {
        public int ToolPosition { get; set; }

        public string SourceToolName { get; set; }
        public double SourceToolDiametr { get; set; }
        public double SourceToolLength { get; set; }
        public double SourceCuttingLength { get; set; }
        public double SourceCutRadius { get; set; }
        public double SourceCutAngle { get; set; }
        public int SourceNumberOfTeeth { get; set; }
        public double SourceToolOverhang { get; set; }
        public string SourceHolderName { get; set; }
        public string SourceToolStorageInfo { get; set; }
        public List<string> SourceInsertNames1 { get; set; }
        public List<string> SourceInsertNames2 { get; set; }
        public List<AuxToolInfo> SourceAuxToolsSpecification { get; set; }

        public string CurrentInsert1 { get; set; }
        public string CurrentInsert2 { get; set; }
    }
}
