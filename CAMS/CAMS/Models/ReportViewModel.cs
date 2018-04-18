using CAMS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAMS.Models
{
    public class ReportViewModel
    {
        private BaseController _lController;


        public ReportViewModel(BaseController controller)
        {
            this._lController = controller;
        }
        public List<Lab> GetLabsOfDepartment(int DepartmentId)
        {
            return _lController.GetLabsOfDepartment(DepartmentId);
        }
        public List<Department> GetDepartments()
        {
            return _lController.GetDepartments();
        }
    }
}