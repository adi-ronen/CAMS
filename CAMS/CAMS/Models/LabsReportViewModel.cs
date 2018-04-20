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


        public LabsReportViewModel(List<LabReport> list, ReportsController controller)
        {
            this.Reports = list;
            this._lController = controller;
        }

        public ReportsController Controller()
        {
            return _lController;
        }
    }
}