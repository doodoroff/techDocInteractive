using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Insert:ExcelTool
    {
        string insertPattern;

        public string InsertPattern
        {
            get { return insertPattern; }

            set { insertPattern = value; }
        }

    }
}
