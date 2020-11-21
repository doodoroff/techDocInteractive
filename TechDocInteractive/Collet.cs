using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Collet:ExcelTool
    {
        string fromSpindelSideInterface;

        public string FromSpindelSideInterface
        {
            get { return fromSpindelSideInterface; }

            set { fromSpindelSideInterface = value; }
        }
    }
}
