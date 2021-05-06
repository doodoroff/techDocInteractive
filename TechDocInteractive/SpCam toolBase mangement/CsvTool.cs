using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class CsvTool
    {
        string name;
        string insertPattern1, insertPattern2;
        string fromSpindelSideInterface, fromCutSideInterface;

        public string Name
        {
            get { return name.Trim(); }

            set { this.name = value; }
        }

        public string InsertPattern1
        {
            get { return insertPattern1.Trim(); }

            set { insertPattern1 = value; }
        }

        public string InsertPattern2
        {
            get { return insertPattern2.Trim(); }

            set { insertPattern2 = value; }
        }

        public string FromSpindelSideInterface
        {
            get { return fromSpindelSideInterface.Trim(); }

            set { fromSpindelSideInterface = value; }
        }

        public string FromCutSideInterface
        {
            get { return fromCutSideInterface.Trim(); }

            set { fromCutSideInterface = value; }
        }
    }
}
