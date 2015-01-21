using ININ.InteractionClient.AddIn;
using Plantronics.UC.SpokesWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn
{
    public class SpokesDebugLogger : DebugLogger
    {
        private ITraceContext _traceContext;

        public SpokesDebugLogger(ITraceContext traceContext)
        {
            _traceContext = traceContext;
        }

        public void DebugPrint(string methodname, string str)
        {
            Debug.WriteLine(String.Format("{0} - {1}", methodname, str));
            _traceContext.Note(String.Format("{0} - {1}", methodname, str));
        }
    }
}
