using CMS.Utility;
using CMS.ViewModel.ViewModel;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;

namespace CMS.RDLCPages
{
    public partial class PrintView : System.Web.UI.Page
    {
        string sStaffId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
         {
            if (!IsPostBack)
            {
                sStaffId = Request.QueryString["sStaffIds"].ToString();
                RenderReport();
            }
        }
        private void RenderReport()
        {
            try
            {
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();            
                DataTable dtStaffRatting = new DataTable();
                DataTable dtFeedbackSetting = new DataTable();
                DataTable dtCollegeInfo = new DataTable();
               

                if (Session[Common.SESSION_VARIABLES.COLLEGE_INFO] != null)
                {
                    dtCollegeInfo = (DataTable)Session[Common.SESSION_VARIABLES.COLLEGE_INFO];
                  //  sCourseId = (Session[Common.SESSION_VARIABLES.COURSE_ID] != null) ? Session[Common.SESSION_VARIABLES.COURSE_ID].ToString() : "0";
                }
                FeedbackViewModel objFeedback = new FeedbackViewModel();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                dtStaffRatting = objFeedback.FetchQuestionwiseReportByStaffIds(sStaffId, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                   dtFeedbackSetting = objFeedback.FetchFBSettingInfo().DataSource.Table;
                // dv.Clear();
                //  dv.Add(Common.FBEVALUATION2017.COURSE_ID, sCourseId, EnumCommand.DataType.String);
                // dv.Add(Common.FBEVALUATION2017.ASSESSEE, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                //  dtStaffRatting = objFeedback.FetchFBResultByCourseId(dv).DataSource.Table;


                rvPrintView.Reset();
                if (dtStaffRatting != null && dtStaffRatting.Rows.Count > 0)
                {
                    rvPrintView.LocalReport.DisplayName = dtStaffRatting.Rows[0]["STAFF_NAME"].ToString() + "-" + dtStaffRatting.Rows[0]["STAFF_CODE"].ToString();
                }
                rvPrintView.LocalReport.EnableExternalImages = true;
                rvPrintView.LocalReport.ReportPath = Server.MapPath("~/CMSReports/StaffFeedbackResult.rdlc");
                rvPrintView.LocalReport.DataSources.Add(new ReportDataSource("StaffFeedback", dtStaffRatting));
                rvPrintView.LocalReport.DataSources.Add(new ReportDataSource("FeedbackInfo", dtFeedbackSetting));
                //rvPrintView.LocalReport.DataSources.Add(new ReportDataSource("FeedbackResult", dtStaffRatting));
                rvPrintView.LocalReport.DataSources.Add(new ReportDataSource("CollegeInfo", dtCollegeInfo));
            }
            catch (Exception ex)
            {

                using (ErrorLog ObjLog=new ErrorLog())
                {
                    ObjLog.WriteError("StffFeedbackResultPrintViewRDLC", "RenderReport", ex.Message);
                }
            }


        }
    }
}