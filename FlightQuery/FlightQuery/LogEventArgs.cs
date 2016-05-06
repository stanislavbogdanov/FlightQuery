using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightQuery
{
    public class LogEventArgs : EventArgs
    {
        public readonly string Message;
        public readonly bool LineFeedSign;
        public readonly bool ResetLogSign;

        public LogEventArgs(string msg, bool linefeed, bool resetlog)
        {
            Message = msg;
            LineFeedSign = linefeed;
            ResetLogSign = resetlog;
        }

        public LogEventArgs(string msg) : this(msg, true, false)
        { }

        public LogEventArgs(bool resetlog) : this("", false, resetlog)
        { }

        public LogEventArgs(string msg, bool resetlog) : this(msg, true, resetlog)
        { }
    }
}
