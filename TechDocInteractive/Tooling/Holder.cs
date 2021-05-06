using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{
    class Holder
    {
        string name;
        string holderCode;
        string fromSpindelSideInterface;
        string fromCutSideInterface;

        public string Name
        {
            get { return name.Trim(); }

            set { this.name = value; }
        }

        public string HolderCode
        {
            get { return holderCode.Trim(); }

            set { this.holderCode = value; }
        }

        public string FromSpindelSideInterface
        {
            get { return fromSpindelSideInterface; }

            set { this.fromSpindelSideInterface = value; }
        }

        public string FromCutSideInterface
        {
            get { return fromCutSideInterface; }

            set { fromCutSideInterface = value; }
        }
    }
}
