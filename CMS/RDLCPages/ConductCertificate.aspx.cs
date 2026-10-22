using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Utility;
using CMS.ViewModel.ViewModel;
using System.Data;

namespace CMS.RDLCPages
{
    public partial class ConductCertificate : System.Web.UI.Page
    {
        string sStudentIds = string.Empty;
        string AcademicYear = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                sStudentIds = Request.QueryString["sStudentIds"].ToString();
                AcademicYear = Request.QueryString["sAcademicYear"].ToString();
                RenderReport();
            }
        }

        public void RenderReport()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            DataTable dtConductCertificate = new DataTable();
            using (StudentViewModel ObjViewModel=new StudentViewModel())
            {
                dtConductCertificate = ObjViewModel.FetchConductCertificate(sStudentIds, AcademicYear).DataSource.Table;
            }
            rvConductCertificate.Reset();
            rvConductCertificate.LocalReport.EnableExternalImages = true;
            rvConductCertificate.LocalReport.ReportPath = Server.MapPath("~/CMSReports/ConductCertificate.rdlc");
            rvConductCertificate.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ConductCertificate",dtConductCertificate));
        }
    }
}