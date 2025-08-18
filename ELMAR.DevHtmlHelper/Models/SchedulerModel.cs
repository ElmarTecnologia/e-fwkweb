using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELMAR.DevHtmlHelper.Models
{
    public class SchedulerModel
    {
        public SchedulerModel()
        {
            ShowViewNavigator = true;
            ShowViewVisibleInterval = true;

            AppointmentsStatus = AppointmentStatusDisplayType.Bounds;
            DayHeaderOrientation = AgendaDayHeaderOrientation.Auto;

            AllowFixedDayHeaders = true;
            ShowResource = true;
            ShowLabel = true;
            ShowRecurrence = true;

            DayCount = 5;
        }

        public bool ShowViewNavigator { get; set; }
        public bool ShowViewVisibleInterval { get; set; }

        public AppointmentStatusDisplayType AppointmentsStatus { get; set; }
        public bool ShowResource { get; set; }
        public bool ShowLabel { get; set; }
        public bool ShowRecurrence { get; set; }
        public int DayCount { get; set; }
        public bool AllowFixedDayHeaders { get; set; }

        public AgendaDayHeaderOrientation DayHeaderOrientation { get; set; }
    }
}