using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace CMS.RDLCPages
{
    public partial class TestReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RenderReport();
            }
        }

        private void RenderReport()
        {
 //            DataTable dt = new DataTable("test");
 //            dt.Columns.Add("test1");
 //            dt.Columns.Add("test2");
 //            dt.Rows.Add("Jesus", "loves you");
 //            dt.Rows.Add("Jesus", "loves you");
 //           dt.Rows.Add("Jesus", "loves you");
 //            ReportViewer1.Reset();
 //           // ReportViewer1.LocalReport.EnableExternalImages = true;
 //           // ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/CMSReports/Report1.rdlc");
 //           // ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("test", dt));

 //           ReportViewer1.LocalReport.DataSources.Add(
 //     new ReportDataSource("test", dt));

 ////           ReportBuilder reportBuilder = new ReportBuilder();
 ////           reportBuilder.DataSource = ds;
 ////           reportBuilder.Page = new ReportPage();
 ////           ReportSections reportFooter = new ReportSections();
 ////           ReportItems reportFooterItems = new ReportItems();
 ////           ReportTextBoxControl[] footerTxt = new ReportTextBoxControl[3];
 ////           string footer = string.Format
 ////           ("Copyright  {0}         Report Generated On  {1}          Page  {2}  of {3} ",
 ////           DateTime.Now.Year, DateTime.Now, ReportGlobalParameters.CurrentPageNumber,
 ////          ReportGlobalParameters.TotalPages);
 ////           footerTxt[0] = new ReportTextBoxControl()
 ////           { Name = "txtCopyright", ValueOrExpression = new string[] { footer } }
 ////reportFooterItems.TextBoxControls = footerTxt;
 ////           reportFooter.ReportControlItems = reportFooterItems;
 ////           reportBuilder.Page.ReportFooter = reportFooter;
 ////           ReportSections reportHeader = new ReportSections();
 ////           reportHeader.Size = new ReportScale();
 ////           reportHeader.Size.Height = 0.56849;
 ////           ReportItems reportHeaderItems = new ReportItems();
 ////           ReportTextBoxControl[] headerTxt = new ReportTextBoxControl[1];
 ////           headerTxt[0] = new ReportTextBoxControl()
 ////           {
 ////               Name = "txtReportTitle",
 ////               ValueOrExpression = new string[] { "Report Name: " + ReportTitle }
 ////           };
 ////           reportHeaderItems.TextBoxControls = headerTxt;
 ////           reportHeader.ReportControlItems = reportHeaderItems;
 ////           reportBuilder.Page.ReportHeader = reportHeader;
 ////           ReportViewer1.LocalReport.LoadReportDefinition(ReportEngine.GenerateReport(reportBuilder));
 ////           ReportViewer1.LocalReport.DisplayName = ReportName;



        }
    }
}