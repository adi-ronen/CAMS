using CAMS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAMS.Models
{
    public class LabsReportViewModel
    {

        public List<LabReport> Reports;
        private ReportsController _lController;
        public string ReportTitle;


        public LabsReportViewModel(List<LabReport> list, ReportsController controller, string title)
        {
            this.Reports = list;
            this._lController = controller;
            this.ReportTitle = title;
        }

        public ReportsController Controller()
        {
            return _lController;
        }
    }
}