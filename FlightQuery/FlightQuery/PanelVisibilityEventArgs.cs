using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace FlightQuery
{
    public class PanelVisibilityEventArgs : EventArgs
    {
        public enum PanelType
        {
            TimetableUpdate,
            DateRange,
            QueryExecute
        }

        public readonly PanelType Panel;
        public readonly System.Windows.Visibility Visibility;

        public PanelVisibilityEventArgs(PanelType panel, System.Windows.Visibility visibility)
        {
            Panel = panel;
            Visibility = visibility;
        }
    }
}
