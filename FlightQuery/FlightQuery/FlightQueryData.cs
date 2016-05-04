using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightQuery
{
    class FlightQueryData
    {
        private bool _autodownload = true;
        public bool IsAutoDownload
        {
            get { return _autodownload; }
            set { _autodownload = value; }
        }
        public bool IsAutoPreprocess
        {
            get { return true; }
            set { }
        }
        public bool IsAutoReadSSIM
        {
            get { return true; }
            set { }
        }
        public bool IsAutoPostprocess
        {
            get { return true; }
            set { }
        }
    }
}
