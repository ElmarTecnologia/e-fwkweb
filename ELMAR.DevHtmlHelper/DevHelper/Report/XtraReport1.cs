using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace DevHelper.Views.Reports {
    public partial class XtraReport1 : DevExpress.XtraReports.UI.XtraReport {
        public XtraReport1() {
            InitializeComponent();
        }

        private void xrLabel2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int categoryID = GetCurrentColumnValue<Int32>("CategoryID");
            XRLabel catLabel = sender as XRLabel;
            catLabel.NavigateUrl = string.Format("~/Home/DrillThrough?catId={0}", categoryID);
        }

    }
}
