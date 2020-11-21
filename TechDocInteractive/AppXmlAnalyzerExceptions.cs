using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDocInteractive
{

    [Serializable]
    public class AppXmlAnalyzerExceptions : Exception
    {
        public AppXmlAnalyzerExceptions() { }
        public AppXmlAnalyzerExceptions(string message) : base(message) { }
        public AppXmlAnalyzerExceptions(string message, Exception inner) : base(message, inner) { }
        protected AppXmlAnalyzerExceptions(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
