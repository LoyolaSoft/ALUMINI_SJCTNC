using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.SQL.AjaxDataTableSQL;
using CMS.ViewModel.Model;
using CMS.DAO;
using CMS.SQL.Student;
using CMS.Utility;
namespace CMS.Controllers
{
    public class AjaxDataTableController : Controller
    {
        // GET: AjaxDataTable
        public JsonResult StudentList(int length, int start)
        {
            string searchVal = HttpContext.Request.Form["search[value]"];
            string sSQLForSearch = string.Empty;
            
            if (!string.IsNullOrEmpty(searchVal))            
                sSQLForSearch = string.Format("AND outT.ROLL_NO LIKE '%{0}%' OR outT.REGISTER_NO LIKE '%{0}%' OR outT.FIRST_NAME LIKE '%{0}%'", searchVal);

            string sSQL = string.Format(AjaxDataTableSQL.GetAjaxDataTableSQL(AjaxDataTableCommands.StudentList),start,(!string.IsNullOrEmpty(searchVal)?sSQLForSearch:string.Empty),length);
            List<ListStudents> liStudentList = new List<ListStudents>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null, sSQL, sAcademicYear).DataSource.Data;
            var liStudentCount = (List<StudentInfo>)CMSHandler.FetchData<StudentInfo>(null, AjaxDataTableSQL.GetAjaxDataTableSQL(AjaxDataTableCommands.StudentTotalCount), sAcademicYear).DataSource.Data;
            var response = new { data = liStudentList, recordsFiltered = liStudentCount.FirstOrDefault().COUNT_FIELD, recordsTotal = liStudentCount.FirstOrDefault().COUNT_FIELD };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AcademicyearwiseStudentList(int length, int start)
        {
            string searchVal = HttpContext.Request.Form["search[value]"];
            string sSQLForSearch = string.Empty;

            if (!string.IsNullOrEmpty(searchVal))
                sSQLForSearch = string.Format("AND outT.ROLL_NO LIKE '%{0}%' OR outT.REGISTER_NO LIKE '%{0}%' OR outT.FIRST_NAME LIKE '%{0}%'", searchVal);

            string sSQL = string.Format(AjaxDataTableSQL.GetAjaxDataTableSQL(AjaxDataTableCommands.StudentList), start, (!string.IsNullOrEmpty(searchVal) ? sSQLForSearch : string.Empty), length);

            List<ListStudents> liStudentList = new List<ListStudents>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null, sSQL, sAcademicYear).DataSource.Data;
            var liStudentCount = (List<StudentInfo>)CMSHandler.FetchData<StudentInfo>(null, AjaxDataTableSQL.GetAjaxDataTableSQL(AjaxDataTableCommands.StudentTotalCount), sAcademicYear).DataSource.Data;
            var response = new { data = liStudentList, recordsFiltered = liStudentCount.FirstOrDefault().COUNT_FIELD, recordsTotal = liStudentCount.FirstOrDefault().COUNT_FIELD };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IssuedTransaction(int length, int start)
        {
            string searchVal = HttpContext.Request.Form["search[value]"];
            string sSQLForSearch = string.Empty;

            if (!string.IsNullOrEmpty(searchVal))
                sSQLForSearch = string.Format("AND OT.USER_TYPE_NAME LIKE '%{0}%' OR OT.ROLE LIKE '%{0}%' OR OT.FIRST_NAME LIKE '%{0}%' OR OT.BOOK_TITLE LIKE '%{0}%' OR OT.AUTHOR LIKE '%{0}%'", searchVal);

            string sSQL = string.Format(SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllReturnedTransactionWhichIsNotReturned), start, (!string.IsNullOrEmpty(searchVal) ? sSQLForSearch : string.Empty), length);

            List<LI_BOOK_ISSUE> liIssueList = new List<LI_BOOK_ISSUE>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            liIssueList = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(null, sSQL, sAcademicYear).DataSource.Data;

            var liIssueCount = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllReturnedTransactionCountWhichIsNotReturned), sAcademicYear).DataSource.Data;
            var response = new { data = liIssueList, recordsFiltered = liIssueCount.FirstOrDefault().TOTAL_COUNT, recordsTotal = liIssueCount.FirstOrDefault().TOTAL_COUNT };
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ReturnedTransaction(int length, int start)
        {
            string searchVal = HttpContext.Request.Form["search[value]"];
            string sSQLForSearch = string.Empty;

            if (!string.IsNullOrEmpty(searchVal))
                sSQLForSearch = string.Format("AND OT.USER_TYPE_NAME LIKE '%{0}%' OR OT.ROLE LIKE '%{0}%' OR OT.FIRST_NAME LIKE '%{0}%' OR OT.BOOK_TITLE LIKE '%{0}%' OR OT.AUTHOR LIKE '%{0}%'", searchVal);

            string sSQL = string.Format(SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllReturnedTransactionWhichNotConfirmed), start, (!string.IsNullOrEmpty(searchVal) ? sSQLForSearch : string.Empty), length);

            List<LI_BOOK_ISSUE> liIssueList = new List<LI_BOOK_ISSUE>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            liIssueList = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(null, sSQL, sAcademicYear).DataSource.Data;

            var liIssueCount = (List<LI_BOOK_ISSUE>)CMSHandler.FetchData<LI_BOOK_ISSUE>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchAllReturnedTransactionCountWhichNotConfirmed), sAcademicYear).DataSource.Data;
            var response = new { data = liIssueList, recordsFiltered = liIssueCount.FirstOrDefault().TOTAL_COUNT, recordsTotal = liIssueCount.FirstOrDefault().TOTAL_COUNT };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        // library external user list

        public JsonResult LiUserList(int length, int start)
        {
            string searchVal = HttpContext.Request.Form["search[value]"];
            string sSQLForSearch = string.Empty;

            if (!string.IsNullOrEmpty(searchVal))
                sSQLForSearch = string.Format("AND outT.USERNAME LIKE '%{0}%' OR outT.USER_TYPE LIKE '%{0}%' OR outT.FIRST_NAME LIKE '%{0}%' OR outT.RFID LIKE '%{0}%' OR outT.BOID LIKE '%{0}%'", searchVal);

            string sSQL = string.Format(SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberDetails), start, (!string.IsNullOrEmpty(searchVal) ? sSQLForSearch : string.Empty), length);

            List<LI_EXTERNAL_USER_INFO> liList = new List<LI_EXTERNAL_USER_INFO>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            liList = (List<LI_EXTERNAL_USER_INFO>)CMSHandler.FetchData<LI_EXTERNAL_USER_INFO>(null, sSQL, sAcademicYear).DataSource.Data;

            var liCount = (List<LI_EXTERNAL_USER_INFO>)CMSHandler.FetchData<LI_EXTERNAL_USER_INFO>(null, SQL.Library.LibrarySQL.GetLibrarySQL(LibrarySQLCommands.FetchLiMemberDetailsTotalCount), sAcademicYear).DataSource.Data;
            var response = new { data = liList, recordsFiltered = liCount.FirstOrDefault().COUNT_FIELD, recordsTotal = liCount.FirstOrDefault().COUNT_FIELD };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}