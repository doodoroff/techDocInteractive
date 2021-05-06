using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class ExcelTool: IEquatable<ExcelTool>
    {
        public string ToolName { get; set; }

        public int ProductionStorageQuantity { get; set; }

        public int CentralStorageQuantity { get; set; }

        public int ProductionQuantity { get; set; }

        public string StorageCode { get; set; }

        public string OmegaCode { get; set; }

        public bool Equals(ExcelTool other)
        {
            if (other.ToolName.Equals(ToolName))
            {
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ExcelTool excelToolObj = obj as ExcelTool;
            if (excelToolObj == null)
            {
                return false;
            }
            else
            {
                return Equals(excelToolObj);
            }
        }

        public override int GetHashCode()
        {
            return this.ToolName.GetHashCode();
        }
    }
}
