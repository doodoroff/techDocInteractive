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
        string description;
        string fromSpindelSideInterFase;
        string fromCutSideInterFace;
        double overhang;

        public string Name
        {
            get { return name.Trim(); }

            set { name = value; }
        }
        public string Description
        {
            get { return description; }

            set { description = value; }
        }
        public string FromSpindelSideInterface
        {
            get { return fromSpindelSideInterFase; }

            set { fromSpindelSideInterFase = value; }
        }
        public string FromCutSideInterface
        {
            get { return fromCutSideInterFace; }

            set { fromCutSideInterFace = value; }
        }
        public double Overhang
        {
            get { return overhang; }

            set { overhang = value; }
        }
    }
}
