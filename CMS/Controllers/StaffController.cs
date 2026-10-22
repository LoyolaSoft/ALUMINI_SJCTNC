using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CMS.ViewModel.ViewModel;
using CMS.Utility;
using CMS.ViewModel.Model;
using Newtonsoft.Json;
using CMS.Models;
using CMS.DAO;
using CMS.SQL.StaffSQL;
using CMS.SQL.SupportData;
using System.IO;
using CMS.SQL.Student;
using CMS.SQL.Examination;
using System.Threading.Tasks;
using System.Configuration;
using System.Text;
using CMS.SQL.Fee;
using System.Globalization;

namespace CMS.Controllers
{
    public class StaffController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        DataValue dv = new DataValue();
        JsonResultData ObjJsonData = new JsonResultData();
        string sInsertQuery = string.Empty;
        string sResult = string.Empty;
        string sUpdateQuery = string.Empty;

        public ActionResult ViewDataTable()
        {
            return View();
        }

        #region Navigation Tabs
        // GET: Staff

        public ActionResult StaffInfo()
        {
            return View();
        }

        // Edit Staff Details ...

        public ActionResult EditStaffInfomation(string Id)
        {
            Session[Common.SESSION_VARIABLES.STAFF_ID] = Id;
            return View();
        }
        #endregion

        #region Staff personal Infomation
        // Staff Personal Information ...

        public ActionResult StaffPersoanlInfo()
        {
            StaffPersonalInfo objStaffPersonalInfo = new StaffPersonalInfo();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Shift ...
                DataTable dtFetchShift = new DataTable();
                dtFetchShift = SupportDataMethod.FetchShift().DataSource.Table;
                List<Sup_Shift> liShift = new List<Sup_Shift>();
                if (dtFetchShift != null && dtFetchShift.Rows.Count > 0)
                {
                    liShift = (from DataRow dr in dtFetchShift.Rows
                               select new Sup_Shift()
                               {
                                   SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                   SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                               }).ToList();
                    objStaffPersonalInfo.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }

                // Department ...
                DataTable dtFetchDepartment = new DataTable();
                dtFetchDepartment = SupportDataMethod.FetchDepartment(sAcademicYear).DataSource.Table;
                List<SupDepartment> liDepartment = new List<SupDepartment>();
                if (dtFetchDepartment != null && dtFetchDepartment.Rows.Count > 0)
                {
                    liDepartment = (from DataRow dr in dtFetchDepartment.Rows
                                    select new SupDepartment()
                                    {
                                        DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                        DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                    }).ToList();
                    objStaffPersonalInfo.Department = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }

                // Community ...

                DataTable dtFetchCommunity = new DataTable();
                dtFetchCommunity = SupportDataMethod.FetchCommunity().DataSource.Table;
                List<SupCommunity> liCommunity = new List<SupCommunity>();
                if (dtFetchCommunity != null && dtFetchCommunity.Rows.Count > 0)
                {
                    liCommunity = (from DataRow dr in dtFetchCommunity.Rows
                                   select new SupCommunity()
                                   {
                                       COMMUNITYID = dr[Common.SUP_COMMUNITY.COMMUNITYID].ToString(),
                                       COMMUNITY = dr[Common.SUP_COMMUNITY.COMMUNITY].ToString()
                                   }).ToList();
                    objStaffPersonalInfo.Community = new SelectList(liCommunity, Common.SUP_COMMUNITY.COMMUNITYID, Common.SUP_COMMUNITY.COMMUNITY);
                }

                // Gender ...

                DataTable dtFetchGender = new DataTable();
                dtFetchGender = SupportDataMethod.FetchGender().DataSource.Table;
                List<SupGender> liGender = new List<SupGender>();
                if (dtFetchGender != null && dtFetchGender.Rows.Count > 0)
                {
                    liGender = (from DataRow dr in dtFetchGender.Rows
                                select new SupGender()
                                {
                                    GENDER_ID = dr[Common.SUP_GENDER.GENDER_ID].ToString(),
                                    GENDER_NAME = dr[Common.SUP_GENDER.GENDER_NAME].ToString()
                                }).ToList();
                    objStaffPersonalInfo.Gender = new SelectList(liGender, Common.SUP_GENDER.GENDER_ID, Common.SUP_GENDER.GENDER_NAME);

                }

                // MaritalStatus ...
                DataTable dtFetchMaritalStatus = new DataTable();
                dtFetchMaritalStatus = SupportDataMethod.FetchMaritalStatus().DataSource.Table;
                List<SupMaritalStatus> liMaritalStatus = new List<SupMaritalStatus>();
                if (dtFetchMaritalStatus != null && dtFetchMaritalStatus.Rows.Count > 0)
                {
                    liMaritalStatus = (from DataRow dr in dtFetchMaritalStatus.Rows
                                       select new SupMaritalStatus()
                                       {
                                           MARITAL_STAUS_ID = dr[Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID].ToString(),
                                           MARITAL_STATUS_NAME = dr[Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME].ToString()
                                       }).ToList();
                    objStaffPersonalInfo.Maritalstatus = new SelectList(liMaritalStatus, Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID, Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME);
                }

                // MotherTongue ...
                DataTable dtFetchMotherTongue = new DataTable();
                dtFetchMotherTongue = SupportDataMethod.FetchMotherTongue().DataSource.Table;
                List<SupMotherTongue> liMotherTongue = new List<SupMotherTongue>();
                if (dtFetchMotherTongue != null && dtFetchMotherTongue.Rows.Count > 0)
                {
                    liMotherTongue = (from DataRow dr in dtFetchMotherTongue.Rows
                                      select new SupMotherTongue()
                                      {
                                          MOTHER_TONGUE_ID = dr[Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID].ToString(),
                                          MOTHER_TONGUE_NAME = dr[Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME].ToString()
                                      }).ToList();
                    objStaffPersonalInfo.MotherTongue = new SelectList(liMotherTongue, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME);
                }

                // Nationality ...
                DataTable dtFetchNationality = new DataTable();
                dtFetchNationality = SupportDataMethod.FetchNationality().DataSource.Table;
                List<SupNationality> liNationality = new List<SupNationality>();
                if (dtFetchNationality != null && dtFetchNationality.Rows.Count > 0)
                {
                    liNationality = (from DataRow dr in dtFetchNationality.Rows
                                     select new SupNationality()
                                     {
                                         NATIONALITY_ID = dr[Common.SUP_NATIONALITY.NATIONALITY_ID].ToString(),
                                         NATIONALITY_NAME = dr[Common.SUP_NATIONALITY.NATIONALITY_NAME].ToString()
                                     }).ToList();
                    objStaffPersonalInfo.Nationality = new SelectList(liNationality, Common.SUP_NATIONALITY.NATIONALITY_ID, Common.SUP_NATIONALITY.NATIONALITY_NAME);
                }

                // Religion ...
                DataTable dtFetchReligion = new DataTable();
                dtFetchReligion = SupportDataMethod.FetchReligion().DataSource.Table;
                List<SUP_RELIGION> liReligion = new List<SUP_RELIGION>();
                if (dtFetchReligion != null && dtFetchReligion.Rows.Count > 0)
                {
                    liReligion = (from DataRow dr in dtFetchReligion.Rows
                                  select new SUP_RELIGION()
                                  {
                                      RELIGIONID = dr[Common.SUP_RELIGION.RELIGIONID].ToString(),
                                      RELIGION = dr[Common.SUP_RELIGION.RELIGION].ToString()
                                  }).ToList();
                    objStaffPersonalInfo.Religion = new SelectList(liReligion, Common.SUP_RELIGION.RELIGIONID, Common.SUP_RELIGION.RELIGION);
                }

                // BloodGroup ...
                DataTable dtFetchBloodGroup = new DataTable();
                dtFetchBloodGroup = SupportDataMethod.FetchBloodGroup().DataSource.Table;
                List<SupBloodGroup> liBloodGroup = new List<SupBloodGroup>();
                if (dtFetchBloodGroup != null && dtFetchBloodGroup.Rows.Count > 0)
                {
                    liBloodGroup = (from DataRow dr in dtFetchBloodGroup.Rows
                                    select new SupBloodGroup()
                                    {
                                        BLOOD_GROUP_ID = dr[Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID].ToString(),
                                        BLOOD_GROUP_NAME = dr[Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME].ToString()
                                    }).ToList();
                    objStaffPersonalInfo.BloodGroup = new SelectList(liBloodGroup, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME);
                }

                // Sup_Role ...
                var liRole = new List<SUP_ROLE>();
                liRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchSupRole)).DataSource.Data;
                objStaffPersonalInfo.Role = new SelectList(liRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);

                // UserType ...
                var liUserType = new List<SUP_USER_TYPE>();
                liUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserType)).DataSource.Data;
                objStaffPersonalInfo.UserType = new SelectList(liUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);
            }
            return View(objStaffPersonalInfo);
        }

        //Insert Staff Personal ...
        public string StaffPersonal(string str, string sBaseImage, string sExtension)
        {

            if (str != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffPersonal>(str);
                string DateOfBirth = CommonMethods.ConvertdatetoyearFromat(Result.DateofBirth);
                string DateOfJoin = CommonMethods.ConvertdatetoyearFromat(Result.DateOfJoin);
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.FIRST_NAME, Result.FirstName, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.LAST_NAME, Result.Lastname, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.KNOWN_AS, Result.NickName, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.GENDER, Result.Gender, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.PLACE_OF_BIRTH, Result.PlaceOfBirth, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.COMMUNITY, Result.Community, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.NATIONALITY, Result.Nationality, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.MARITAL_STATUS, Result.Maritalstatus, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DATE_OF_JOIN, DateOfJoin, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DATE_OF_BIRTH, DateOfBirth, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.BLOOD_GROUP, Result.BloodGroup, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.RELIGION, Result.Religion, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.MOTHER_TONGUE, Result.MotherTongue, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DEPARTMENT, Result.Department, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_CODE, Result.StaffCode, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.SHIFT, Result.Shift, EnumCommand.DataType.String);

                if (!string.IsNullOrEmpty(sBaseImage) && (!string.IsNullOrEmpty(sExtension)))
                {
                    Result.Photo = SaveImage(sBaseImage, sExtension, Result.StaffCode);

                }
                else
                    Result.Photo = "";
                dv.Add(Common.STF_PERSONAL_INFO.PHOTO, Result.Photo, EnumCommand.DataType.String);
                using (StaffViewModel objStaffView = new StaffViewModel())
                {
                    DataTable dtIfExits = new DataTable();
                    dtIfExits = objStaffView.PersonalIfExits(dv).DataSource.Table;
                    if (dtIfExits != null && dtIfExits.Rows.Count > 0)
                    {
                        sResult = "Staff Already Exit Try Again ...!";
                        if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
                        {
                            string staffId = dtIfExits.Rows[0][Common.STF_PERSONAL_INFO.STAFF_ID].ToString();
                            string staffcode = dtIfExits.Rows[0][Common.STF_PERSONAL_INFO.STAFF_CODE].ToString();
                            if (staffId == Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() && staffcode == Result.StaffCode)
                            {
                                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, staffId, EnumCommand.DataType.String);
                                resultArgs = objStaffView.UpdateStaffPersonalInfo(dv);
                                sResult = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                    else
                    {
                        resultArgs = objStaffView.InsertStaffPersonal(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] = resultArgs.RowUniqueId.ToString();
                            var objAccountInfo = new USER_ACCOUNT_INFO();
                            var liSuPAccountDetails = new List<SUP_USER>();
                            objAccountInfo.NAME = Result.FirstName;
                            objAccountInfo.USERNAME = Result.StaffCode.ToUpper();
                            objAccountInfo.EMAIL = "";
                            objAccountInfo.MOBILE = "";
                            objAccountInfo.USER_ID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
                            objAccountInfo.PASSWORD = CommonMethods.GetSha256FromString(objAccountInfo.USERNAME + "@" + DateOfBirth.Substring(0, 4));

                            objAccountInfo.ROLE = Result.Role;
                            objAccountInfo.USER_TYPE = Result.UserType;
                            resultArgs = CMSHandler.SaveOrUpdate(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.SaveUserAccount), "", true);
                            //objAccountInfo.USER_ID = sStaffID;
                            sResult = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.Messages.FailedToSaveRecords;

                        }
                        else
                        {
                            sResult = Common.Messages.FailedToSaveRecords;
                        }
                    }

                }
            }
            return sResult;
        }

        // Fetch Staff PersonalInfo
        public JsonResult FetchPersonalInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchRecordbyId = new DataTable();
            var ObjUserAccount = new USER_ACCOUNT_INFO();
            StaffViewModel objStaff = new StaffViewModel();
            dtFetchRecordbyId = objStaff.FetchRecordById(dv).DataSource.Table;
            if (dtFetchRecordbyId != null && dtFetchRecordbyId.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.STAFF_ID].ToString();
                StaffPersonal objstaffInfo = new StaffPersonal()
                {
                    FirstName = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.FIRST_NAME].ToString(),
                    Lastname = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.LAST_NAME].ToString(),
                    NickName = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.KNOWN_AS].ToString(),
                    DateofBirth = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DATE_OF_BIRTH].ToString(),
                    PlaceOfBirth = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PLACE_OF_BIRTH].ToString(),
                    DateOfJoin = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DATE_OF_JOIN].ToString(),
                    Maritalstatus = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MARITAL_STATUS].ToString(),
                    Gender = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.GENDER].ToString(),
                    Department = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DEPARTMENT].ToString(),
                    Community = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.COMMUNITY].ToString(),
                    Nationality = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.NATIONALITY].ToString(),
                    Religion = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.RELIGION].ToString(),
                    MotherTongue = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MOTHER_TONGUE].ToString(),
                    BloodGroup = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.BLOOD_GROUP].ToString(),
                    StaffCode = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.STAFF_CODE].ToString(),
                    Shift = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.SHIFT].ToString(),
                    Photo = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PHOTO].ToString(),
                    Is_left = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.IS_LEFT].ToString()

                };
                ObjUserAccount.USERNAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.STAFF_CODE].ToString();
                var liUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(ObjUserAccount, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchUserAccountByStaffCode)).DataSource.Data;
                if (liUserAccount != null && liUserAccount.Count > 0)
                {
                    objstaffInfo.Role = liUserAccount.FirstOrDefault().ROLE;
                    objstaffInfo.UserType = liUserAccount.FirstOrDefault().USER_TYPE;
                }
                return Json(objstaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Staff Personal Information ...
        public ActionResult EditStaffPersoanlInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            StaffPersonalInfo objStaffPersonalInfo = new StaffPersonalInfo();
            if (!string.IsNullOrEmpty(sStaffID))
            {
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
                DataTable dtFetchRecordbyId = new DataTable();
                StaffViewModel objStaff = new StaffViewModel();
                dtFetchRecordbyId = objStaff.FetchRecordById(dv).DataSource.Table;

                // Shift ...
                DataTable dtFetchShift = new DataTable();
                dtFetchShift = SupportDataMethod.FetchShift().DataSource.Table;
                List<Sup_Shift> liShift = new List<Sup_Shift>();
                if (dtFetchShift != null && dtFetchShift.Rows.Count > 0)
                {
                    liShift = (from DataRow dr in dtFetchShift.Rows
                               select new Sup_Shift()
                               {
                                   SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                   SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                               }).ToList();
                    objStaffPersonalInfo.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }

                // Department ...
                DataTable dtFetchDepartment = new DataTable();
                dtFetchDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                List<SupDepartment> liDepartment = new List<SupDepartment>();
                if (dtFetchDepartment != null && dtFetchDepartment.Rows.Count > 0)
                {
                    liDepartment = (from DataRow dr in dtFetchDepartment.Rows
                                    select new SupDepartment()
                                    {
                                        DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                        DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                    }).ToList();
                    objStaffPersonalInfo.Department = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }

                // Community ...

                DataTable dtFetchCommunity = new DataTable();
                dtFetchCommunity = SupportDataMethod.FetchCommunity().DataSource.Table;
                List<SupCommunity> liCommunity = new List<SupCommunity>();
                if (dtFetchCommunity != null && dtFetchCommunity.Rows.Count > 0)
                {
                    liCommunity = (from DataRow dr in dtFetchCommunity.Rows
                                   select new SupCommunity()
                                   {
                                       COMMUNITYID = dr[Common.SUP_COMMUNITY.COMMUNITYID].ToString(),
                                       COMMUNITY = dr[Common.SUP_COMMUNITY.COMMUNITY].ToString()
                                   }).ToList();
                    objStaffPersonalInfo.Community = new SelectList(liCommunity, Common.SUP_COMMUNITY.COMMUNITYID, Common.SUP_COMMUNITY.COMMUNITY);
                }

                // Gender ...

                DataTable dtFetchGender = new DataTable();
                dtFetchGender = SupportDataMethod.FetchGender().DataSource.Table;
                List<SupGender> liGender = new List<SupGender>();
                if (dtFetchGender != null && dtFetchGender.Rows.Count > 0)
                {
                    liGender = (from DataRow dr in dtFetchGender.Rows
                                select new SupGender()
                                {
                                    GENDER_ID = dr[Common.SUP_GENDER.GENDER_ID].ToString(),
                                    GENDER_NAME = dr[Common.SUP_GENDER.GENDER_NAME].ToString()
                                }).ToList();
                    objStaffPersonalInfo.Gender = new SelectList(liGender, Common.SUP_GENDER.GENDER_ID, Common.SUP_GENDER.GENDER_NAME);

                }

                // MaritalStatus ...
                DataTable dtFetchMaritalStatus = new DataTable();
                dtFetchMaritalStatus = SupportDataMethod.FetchMaritalStatus().DataSource.Table;
                List<SupMaritalStatus> liMaritalStatus = new List<SupMaritalStatus>();
                if (dtFetchMaritalStatus != null && dtFetchMaritalStatus.Rows.Count > 0)
                {
                    liMaritalStatus = (from DataRow dr in dtFetchMaritalStatus.Rows
                                       select new SupMaritalStatus()
                                       {
                                           MARITAL_STAUS_ID = dr[Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID].ToString(),
                                           MARITAL_STATUS_NAME = dr[Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME].ToString()
                                       }).ToList();
                    objStaffPersonalInfo.Maritalstatus = new SelectList(liMaritalStatus, Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID, Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME);
                }

                // MotherTongue ...
                DataTable dtFetchMotherTongue = new DataTable();
                dtFetchMotherTongue = SupportDataMethod.FetchMotherTongue().DataSource.Table;
                List<SupMotherTongue> liMotherTongue = new List<SupMotherTongue>();
                if (dtFetchMotherTongue != null && dtFetchMotherTongue.Rows.Count > 0)
                {
                    liMotherTongue = (from DataRow dr in dtFetchMotherTongue.Rows
                                      select new SupMotherTongue()
                                      {
                                          MOTHER_TONGUE_ID = dr[Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID].ToString(),
                                          MOTHER_TONGUE_NAME = dr[Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME].ToString()
                                      }).ToList();
                    objStaffPersonalInfo.MotherTongue = new SelectList(liMotherTongue, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME);
                }

                // Nationality ...
                DataTable dtFetchNationality = new DataTable();
                dtFetchNationality = SupportDataMethod.FetchNationality().DataSource.Table;
                List<SupNationality> liNationality = new List<SupNationality>();
                if (dtFetchNationality != null && dtFetchNationality.Rows.Count > 0)
                {
                    liNationality = (from DataRow dr in dtFetchNationality.Rows
                                     select new SupNationality()
                                     {
                                         NATIONALITY_ID = dr[Common.SUP_NATIONALITY.NATIONALITY_ID].ToString(),
                                         NATIONALITY_NAME = dr[Common.SUP_NATIONALITY.NATIONALITY_NAME].ToString()
                                     }).ToList();
                    objStaffPersonalInfo.Nationality = new SelectList(liNationality, Common.SUP_NATIONALITY.NATIONALITY_ID, Common.SUP_NATIONALITY.NATIONALITY_NAME);
                }

                // Religion ...
                DataTable dtFetchReligion = new DataTable();
                dtFetchReligion = SupportDataMethod.FetchReligion().DataSource.Table;
                List<SUP_RELIGION> liReligion = new List<SUP_RELIGION>();
                if (dtFetchReligion != null && dtFetchReligion.Rows.Count > 0)
                {
                    liReligion = (from DataRow dr in dtFetchReligion.Rows
                                  select new SUP_RELIGION()
                                  {
                                      RELIGIONID = dr[Common.SUP_RELIGION.RELIGIONID].ToString(),
                                      RELIGION = dr[Common.SUP_RELIGION.RELIGION].ToString()
                                  }).ToList();
                    objStaffPersonalInfo.Religion = new SelectList(liReligion, Common.SUP_RELIGION.RELIGIONID, Common.SUP_RELIGION.RELIGION);
                }

                // BloodGroup ...
                DataTable dtFetchBloodGroup = new DataTable();
                dtFetchBloodGroup = SupportDataMethod.FetchBloodGroup().DataSource.Table;
                List<SupBloodGroup> liBloodGroup = new List<SupBloodGroup>();
                if (dtFetchBloodGroup != null && dtFetchBloodGroup.Rows.Count > 0)
                {
                    liBloodGroup = (from DataRow dr in dtFetchBloodGroup.Rows
                                    select new SupBloodGroup()
                                    {
                                        BLOOD_GROUP_ID = dr[Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID].ToString(),
                                        BLOOD_GROUP_NAME = dr[Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME].ToString()
                                    }).ToList();
                    objStaffPersonalInfo.BloodGroup = new SelectList(liBloodGroup, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME);
                }
                // Sup_Role ...
                var liRole = new List<SUP_ROLE>();
                liRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchSupRole)).DataSource.Data;
                objStaffPersonalInfo.Role = new SelectList(liRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);

                // UserType ...
                var liUserType = new List<SUP_USER_TYPE>();
                liUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserType)).DataSource.Data;
                objStaffPersonalInfo.UserType = new SelectList(liUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);
            }

            return View(objStaffPersonalInfo);
        }

        // Update Personal ...
        public string UpdatePersonal()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            var str = Request.Form["str"];
            string filePath = string.Empty;
            string sImagePath = string.Empty;
            string _sDirectorypath = string.Empty;
            STF_PERSONAL_INFO objModel = new STF_PERSONAL_INFO();
            if (str != null)
            {

                _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STAFF_IMGAE_PATH;

                if (!Directory.Exists(_sDirectorypath))
                {
                    Directory.CreateDirectory(_sDirectorypath);
                }

                var Result = JsonConvert.DeserializeObject<StaffModel>(str);
                string DateOfBirth = CommonMethods.ConvertdatetoyearFromat(Result.DateofBirth);
                string DateOfJoin = CommonMethods.ConvertdatetoyearFromat(Result.DateOfJoin);
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.FIRST_NAME, Result.FirstName, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.LAST_NAME, Result.Lastname, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_CODE, Result.StaffCode, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.KNOWN_AS, Result.NickName, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.GENDER, Result.Gender, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.PLACE_OF_BIRTH, Result.PlaceOfBirth, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.COMMUNITY, Result.Community, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.NATIONALITY, Result.Nationality, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.MARITAL_STATUS, Result.Maritalstatus, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DATE_OF_JOIN, DateOfJoin, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DATE_OF_BIRTH, DateOfBirth, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.BLOOD_GROUP, Result.BloodGroup, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.RELIGION, Result.Religion, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.MOTHER_TONGUE, Result.MotherTongue, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DEPARTMENT, Result.Department, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.SHIFT, Result.Shift, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.IS_LEFT, Result.IS_LEFT, EnumCommand.DataType.String);

                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        // Image Updation ....
                        var Rollno = Request.Form["Staffcode"];
                        if (Request.Files.Count > 0)
                        {
                            var File = Request.Files[0];
                            string ImgName = File.FileName;
                            var Img = ImgName.Split('.');
                            string fileName = ImgName.Replace(Img[0], Result.StaffCode);

                            if (!string.IsNullOrEmpty(fileName))
                            {
                                filePath = _sDirectorypath + fileName;
                                sImagePath = "\\" + Common.URLPages.STAFF_IMGAE_PATH + fileName;
                                File.SaveAs(filePath);
                                Result.Photo = sImagePath;

                            }
                            string sSql = StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffImage).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID);
                            objModel.PHOTO = Result.Photo;
                            var UpdatePhoto = CMSHandler.SaveOrUpdate(objModel, sSql);
                        }
                        resultArgs = objStaff.UpdateStaffPersonalInfo(dv);

                        if (resultArgs.Success)
                        {
                            var objAccountInfo = new USER_ACCOUNT_INFO();
                            var liAccountInfo = new List<USER_ACCOUNT_INFO>();
                            var liSuPAccountDetails = new List<SUP_USER>();

                            objAccountInfo.NAME = Result.FirstName;
                            objAccountInfo.USERNAME = Result.StaffCode.ToUpper();
                            objAccountInfo.PASSWORD = CommonMethods.GetSha256FromString(Result.StaffCode.ToUpper() + "@" + DateOfBirth.Substring(0, 4));
                            objAccountInfo.USER_ID = sStaffID;
                            objAccountInfo.ROLE = Result.Role;
                            objAccountInfo.USER_TYPE = Result.UserType;

                            liAccountInfo = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserAccountByRoleAndId)).DataSource.Data;
                            if (liAccountInfo != null && liAccountInfo.Count > 0)
                            {
                                objAccountInfo.USER_ACCOUNT_ID = liAccountInfo.FirstOrDefault().USER_ACCOUNT_ID;
                                objAccountInfo.EMAIL = liAccountInfo.FirstOrDefault().EMAIL;
                                objAccountInfo.MOBILE = liAccountInfo.FirstOrDefault().MOBILE;

                                resultArgs = CMSHandler.SaveOrUpdate(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.UpdateUserAccount));

                                sResult = (resultArgs.Success) ? "Record updated successfully ...!" : "Record not updated try again ...!";
                            }
                            else
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.SaveUserAccount), "", true);
                                //sResult = "Please verify all the Details are filled correctly  ...!";
                            }

                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }

                    }
                }
                else
                {
                    using (StaffViewModel objStaffView = new StaffViewModel())
                    {
                        resultArgs = objStaffView.InsertStaffPersonal(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }

            }
            return sResult;
        }
        private string SaveImage(string baseImage, string sExtension, string fileName)
        {
            string sImagePath = string.Empty;
            //this is to rename baseimgae64 formart.
            string sToReplaceBaseFormat = string.Format("data:image/{0};base64,", (sExtension == "jpg") ? "jpeg" : sExtension);
            string strBaseimage64 = string.Empty;
            string filePath = string.Empty;
            //actual replacement 
            strBaseimage64 = baseImage.Replace(sToReplaceBaseFormat, string.Empty);

            string _sDirectorypath = string.Empty;
            // bool _bSuccess = true;
            _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STAFF_IMGAE_PATH;

            if (!Directory.Exists(_sDirectorypath))
            {
                Directory.CreateDirectory(_sDirectorypath);
            }
            //Convert Base64 Encoded string to Byte Array.
            byte[] imageBytes = Convert.FromBase64String(strBaseimage64);
            if (!string.IsNullOrEmpty(fileName))
            {
                filePath = _sDirectorypath + fileName + "." + sExtension;
                sImagePath = "\\" + Common.URLPages.STAFF_IMGAE_PATH + fileName + "." + sExtension;
            }
            //Save the Byte Array as Image File.and replace file if exists
            System.IO.File.WriteAllBytes(filePath, imageBytes);
            return sImagePath;
        }

        #endregion

        #region Staff Addrress Infomation
        // Staff Address Information ...

        public ActionResult StaffAddressInfo()
        {
            StaffAddressInfo objStaffAddressInfo = new StaffAddressInfo();
            //Country ...
            DataTable dtCountry = new DataTable();
            dtCountry = SupportDataMethod.FetchCountry().DataSource.Table;
            List<SUP_COUNTRY> liCountry = new List<SUP_COUNTRY>();
            if (dtCountry != null && dtCountry.Rows.Count > 0)
            {
                liCountry = (from DataRow dr in dtCountry.Rows
                             select new SUP_COUNTRY()
                             {
                                 COUNTRY_ID = dr[Common.SUP_COUNTRY.COUNTRY_ID].ToString(),
                                 COUNTRY_NAME = dr[Common.SUP_COUNTRY.COUNTRY_NAME].ToString()
                             }).ToList();
                objStaffAddressInfo.CCOUNTRY = new SelectList(liCountry, Common.SUP_COUNTRY.COUNTRY_ID, Common.SUP_COUNTRY.COUNTRY_NAME);
                objStaffAddressInfo.PCOUNTRY = new SelectList(liCountry, Common.SUP_COUNTRY.COUNTRY_ID, Common.SUP_COUNTRY.COUNTRY_NAME);
            }
            return View(objStaffAddressInfo);
        }

        //Insert Address ...
        public string StaffAddress(string str)
        {
            var sStaffId = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            if (sStaffId != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffAddress>(str);
                dv.Clear();
                dv.Add(Common.STF_ADDRESS.STAFFNO, sStaffId, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CDOOR_DETAILS, Result.CDOORDETAILS, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CSTREET, Result.CSTREET, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CPLACE, Result.CPLACE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CCITY, Result.CCITY, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CPIN_CODE, Result.CPINCODE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CDISTRICT, Result.CDISTRICT, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CCOUNTRY, Result.CCOUNTRY, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CVILLAGE, Result.CVILLAGE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CPHONE_NO, Result.CPHONENO, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CCELL_NO, Result.CCELLNO, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CEMAIL, Result.CEMAIL, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PSTREET, Result.PSTREET, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PVILLAGE, Result.PVILLAGE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PPLACE, Result.PPLACE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PCITY, Result.PCITY, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PPIN_CODE, Result.PPINCODE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PDISTRICT, Result.PDISTRICT, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PCOUNTRY, Result.PCOUNTRY, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PPHONE_NO, Result.PPHONENO, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PCELL_NO, Result.PCELLNO, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PDOOR_DETAILS, Result.PDOORDETAILS, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PEMAIL, Result.PEMAIL, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    // Fetch Address Details ......
                    DataTable dtFetchAddressData = new DataTable();
                    dtFetchAddressData = objStaff.FetchAddressById(dv).DataSource.Table;
                    // Update Or Insert Staff Address Table ...
                    if (dtFetchAddressData != null && dtFetchAddressData.Rows.Count > 0)
                    {
                        string AddressId = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.ADDRESS_NO].ToString();
                        dv.Add(Common.STF_ADDRESS.ADDRESS_NO, AddressId, EnumCommand.DataType.String);
                        resultArgs = objStaff.UpdateStaffAddress(dv);
                    }
                    else
                        resultArgs = objStaff.InsertStaffAddress(dv);

                    // Check The Statement Exicuted Successfully ....
                    if (resultArgs.Success)
                        sResult = "Record saved successfully ...!";
                    else
                        sResult = "Record not saved successfully ...!";
                }
            }

            return sResult;
        }

        // Fetch Staff AddressData...
        public JsonResult FetchAddressData(string sStaffId)
        {
            DataTable dtFetchAddressData = new DataTable();
            dv.Clear();
            dv.Add(Common.STF_ADDRESS.STAFFNO, sStaffId, EnumCommand.DataType.String);
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchAddressData = objStaff.FetchAddressById(dv).DataSource.Table;
            }
            if (dtFetchAddressData != null && dtFetchAddressData.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.ADDRESS_NO].ToString();
                StaffAddress objStaffInfo = new StaffAddress()
                {
                    CDOORDETAILS = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CDOOR_DETAILS].ToString(),
                    CSTREET = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CSTREET].ToString(),
                    CPLACE = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CPLACE].ToString(),
                    CCITY = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CCITY].ToString(),
                    CPINCODE = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CPIN_CODE].ToString(),
                    CDISTRICT = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CDISTRICT].ToString(),
                    CVILLAGE = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CVILLAGE].ToString(),
                    CPHONENO = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CPHONE_NO].ToString(),
                    CCELLNO = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CCELL_NO].ToString(),
                    CEMAIL = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CEMAIL].ToString(),
                    PDOORDETAILS = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PDOOR_DETAILS].ToString(),
                    PSTREET = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PSTREET].ToString(),
                    PPLACE = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PPLACE].ToString(),
                    PCITY = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PCITY].ToString(),
                    PPINCODE = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PPIN_CODE].ToString(),
                    PDISTRICT = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PDISTRICT].ToString(),
                    PVILLAGE = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PVILLAGE].ToString(),
                    PPHONENO = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PPHONE_NO].ToString(),
                    PCELLNO = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PCELL_NO].ToString(),
                    PEMAIL = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PEMAIL].ToString(),
                    CCOUNTRY = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.CCOUNTRY].ToString(),
                    PCOUNTRY = dtFetchAddressData.Rows[0][Common.STF_ADDRESS.PCOUNTRY].ToString()
                };

                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Staff Address Information...

        public ActionResult EditStaffAddressInfo()
        {
            StaffAddressInfo objStaffAddressInfo = new StaffAddressInfo();
            //Country ...
            DataTable dtCountry = new DataTable();
            dtCountry = SupportDataMethod.FetchCountry().DataSource.Table;
            List<SUP_COUNTRY> liCountry = new List<SUP_COUNTRY>();
            if (dtCountry != null && dtCountry.Rows.Count > 0)
            {
                liCountry = (from DataRow dr in dtCountry.Rows
                             select new SUP_COUNTRY()
                             {
                                 COUNTRY_ID = dr[Common.SUP_COUNTRY.COUNTRY_ID].ToString(),
                                 COUNTRY_NAME = dr[Common.SUP_COUNTRY.COUNTRY_NAME].ToString()
                             }).ToList();
                objStaffAddressInfo.CCOUNTRY = new SelectList(liCountry, Common.SUP_COUNTRY.COUNTRY_ID, Common.SUP_COUNTRY.COUNTRY_NAME);
                objStaffAddressInfo.PCOUNTRY = new SelectList(liCountry, Common.SUP_COUNTRY.COUNTRY_ID, Common.SUP_COUNTRY.COUNTRY_NAME);
            }
            return View(objStaffAddressInfo);
        }

        // Update Staff Address ..
        public string UpdateAddress(string str)
        {
            var sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sStaffID))
            {
                var Result = JsonConvert.DeserializeObject<StaffAddress>(str);
                dv.Clear();
                dv.Add(Common.STF_ADDRESS.CDOOR_DETAILS, Result.CDOORDETAILS, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CSTREET, Result.CSTREET, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CPLACE, Result.CPLACE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CCITY, Result.CCITY, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CPIN_CODE, Result.CPINCODE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CDISTRICT, Result.CDISTRICT, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CCOUNTRY, Result.CCOUNTRY, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CVILLAGE, Result.CVILLAGE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CPHONE_NO, Result.CPHONENO, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CCELL_NO, Result.CCELLNO, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.CEMAIL, Result.CEMAIL, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PSTREET, Result.PSTREET, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PVILLAGE, Result.PVILLAGE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PPLACE, Result.PPLACE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PCITY, Result.PCITY, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PPIN_CODE, Result.PPINCODE, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PDISTRICT, Result.PDISTRICT, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PCOUNTRY, Result.PCOUNTRY, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PPHONE_NO, Result.PPHONENO, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PCELL_NO, Result.PCELLNO, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PDOOR_DETAILS, Result.PDOORDETAILS, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.PEMAIL, Result.PEMAIL, EnumCommand.DataType.String);
                dv.Add(Common.STF_ADDRESS.STAFFNO, sStaffID, EnumCommand.DataType.String);
                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        var sAddressNo = Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID].ToString();
                        dv.Add(Common.STF_ADDRESS.ADDRESS_NO, sAddressNo, EnumCommand.DataType.String);

                        resultArgs = objStaff.UpdateStaffAddress(dv);
                        if (resultArgs.Success)
                        {
                            sResult = "Record updated successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }
                    }
                }
                else
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.InsertStaffAddress(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }

            }
            return sResult;
        }
        #endregion

        #region Staff Service Infomation
        // Staff Service Information ...

        public ActionResult StaffServiceInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_SERVICES.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchService = new DataTable();
            IList<StaffService> liService = new List<StaffService>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchService = objViewModel.FetchServiceById(dv).DataSource.Table;
            }
            if (dtFetchService != null && dtFetchService.Rows.Count > 0)
            {
                liService = (from DataRow dr in dtFetchService.Rows
                             select new StaffService
                             {
                                 SERVICEID = dr[Common.STF_SERVICES.SERVICE_NO].ToString(),
                                 APPOINTMENTNATURE = dr[Common.STF_SERVICES.APPOINTMENT_NATURE].ToString(),
                                 DATEOFAPPOINT = dr[Common.STF_SERVICES.DATE_OF_APPOINT].ToString(),
                                 DATEOFTERMINATION = dr[Common.STF_SERVICES.DATE_OF_TERMINATION].ToString(),
                                 INSTITUTE = dr[Common.STF_SERVICES.INSTITUTE].ToString(),
                                 NAME = dr[Common.STF_SERVICES.APPOINTMENT_NAME].ToString(),
                                 Place = dr[Common.STF_SERVICES.PLACE].ToString(),
                                 REMARKS = dr[Common.STF_SERVICES.REMARKS].ToString()
                             }).ToList();
                return View(liService);
            }
            else
            {
                return View();
            }

        }

        //Insert Service ...
        public string StaffService(string JsonService)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            else
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sStaffID))
            {
                var Result = JsonConvert.DeserializeObject<StaffService>(JsonService);
                string DateOfAppoint = CommonMethods.ConvertdatetoyearFromat(Result.DATEOFAPPOINT);
                string DateOfTermination = CommonMethods.ConvertdatetoyearFromat(Result.DATEOFTERMINATION);
                dv.Clear();
                dv.Add(Common.STF_SERVICES.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.DATE_OF_APPOINT, DateOfAppoint, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.APPOINTMENT_NAME, Result.NAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.DATE_OF_TERMINATION, DateOfTermination, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.INSTITUTE, Result.INSTITUTE, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.REMARKS, Result.REMARKS, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.PLACE, Result.Place, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    // Insert Staff Service Table ...
                    resultArgs = objStaff.InsertStaffservices(dv);

                    // Check the Statement is success or not ...
                    if (resultArgs.Success)
                        sResult = "Record saved successfully ...!";
                    else
                        sResult = "Record not saved successfully ...!";
                }
            }

            return sResult;
        }

        // Fetch Staff SeviceData...
        public JsonResult FetchSeviceData(string sStaffId)
        {
            dv.Clear();
            dv.Add(Common.STF_SERVICES.STAFF_NO, sStaffId, EnumCommand.DataType.String);
            DataTable dtFetchServiceData = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchServiceData = objStaff.FetchServiceById(dv).DataSource.Table;
            }
            if (dtFetchServiceData != null && dtFetchServiceData.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchServiceData.Rows[0][Common.STF_SERVICES.SERVICE_NO].ToString();
                return Json(sResult);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Bind Service Infomation ...
        public JsonResult BindService(string sServiceID)
        {
            dv.Clear();
            dv.Add(Common.STF_SERVICES.SERVICE_NO, sServiceID, EnumCommand.DataType.String);
            DataTable dtFetchServiceData = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchServiceData = objStaff.BindService(dv).DataSource.Table;
            }
            if (dtFetchServiceData != null && dtFetchServiceData.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchServiceData.Rows[0][Common.STF_SERVICES.SERVICE_NO].ToString();
                StaffService objStaffInfo = new StaffService()
                {
                    DATEOFAPPOINT = dtFetchServiceData.Rows[0][Common.STF_SERVICES.DATE_OF_APPOINT].ToString(),
                    NAME = dtFetchServiceData.Rows[0][Common.STF_SERVICES.APPOINTMENT_NAME].ToString(),
                    APPOINTMENTNATURE = dtFetchServiceData.Rows[0][Common.STF_SERVICES.APPOINTMENT_NATURE].ToString(),
                    DATEOFTERMINATION = dtFetchServiceData.Rows[0][Common.STF_SERVICES.DATE_OF_TERMINATION].ToString(),
                    INSTITUTE = dtFetchServiceData.Rows[0][Common.STF_SERVICES.INSTITUTE].ToString(),
                    REMARKS = dtFetchServiceData.Rows[0][Common.STF_SERVICES.REMARKS].ToString(),
                    Place = dtFetchServiceData.Rows[0][Common.STF_SERVICES.PLACE].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Staff Service Information ...

        public ActionResult EditStaffServiceInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sStaffID))
            {
                dv.Clear();
                dv.Add(Common.STF_SERVICES.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                DataTable dtFetchService = new DataTable();
                IList<StaffService> liService = new List<StaffService>();
                using (StaffViewModel objViewModel = new StaffViewModel())
                {
                    dtFetchService = objViewModel.FetchServiceById(dv).DataSource.Table;
                }
                if (dtFetchService != null && dtFetchService.Rows.Count > 0)
                {
                    liService = (from DataRow dr in dtFetchService.Rows
                                 select new StaffService
                                 {
                                     SERVICEID = dr[Common.STF_SERVICES.SERVICE_NO].ToString(),
                                     APPOINTMENTNATURE = dr[Common.STF_SERVICES.APPOINTMENT_NATURE].ToString(),
                                     DATEOFAPPOINT = dr[Common.STF_SERVICES.DATE_OF_APPOINT].ToString(),
                                     DATEOFTERMINATION = dr[Common.STF_SERVICES.DATE_OF_TERMINATION].ToString(),
                                     INSTITUTE = dr[Common.STF_SERVICES.INSTITUTE].ToString(),
                                     NAME = dr[Common.STF_SERVICES.APPOINTMENT_NAME].ToString(),
                                     Place = dr[Common.STF_SERVICES.PLACE].ToString(),
                                     REMARKS = dr[Common.STF_SERVICES.REMARKS].ToString()
                                 }).ToList();
                    return View(liService);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                sResult = "Session Has Expired Please Login Again ...!";
                return Json(sResult);
            }

        }

        // Update Staff Service ..
        public string UpdateStaffServices(string JsonService)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }

            if (JsonService != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffService>(JsonService);
                string DateofAppoint = CommonMethods.ConvertdatetoyearFromat(Result.DATEOFAPPOINT);
                string DateofTermination = CommonMethods.ConvertdatetoyearFromat(Result.DATEOFTERMINATION);
                dv.Clear();
                dv.Add(Common.STF_SERVICES.DATE_OF_APPOINT, DateofAppoint, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.APPOINTMENT_NAME, Result.NAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.DATE_OF_TERMINATION, DateofTermination, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.INSTITUTE, Result.INSTITUTE, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.REMARKS, Result.REMARKS, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.PLACE, Result.Place, EnumCommand.DataType.String);
                dv.Add(Common.STF_SERVICES.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        var sServiceID = Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID].ToString();
                        dv.Add(Common.STF_SERVICES.SERVICE_NO, sServiceID, EnumCommand.DataType.String);

                        resultArgs = objStaff.UpdateStaffServices(dv);
                        if (resultArgs.Success)
                        {
                            sResult = "Record updated successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }
                    }
                }
                else
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.InsertStaffservices(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }

            }
            return sResult;
        }

        // Delete Staff Service ...
        public string DeleteServices(string sServiceId)
        {
            dv.Clear();
            dv.Add(Common.STF_SERVICES.SERVICE_NO, sServiceId, EnumCommand.DataType.String);
            dv.Add(Common.STF_SERVICES.ISDELETED, "1", EnumCommand.DataType.String);
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                resultArgs = objViewModel.DeleteServics(dv);
                if (resultArgs.Success)
                {
                    sResult = "Record deleted successfully ...!";
                }
                else
                {
                    sResult = "Record not deleted try again...!";
                }
            }
            return sResult;
        }
        #endregion

        #region Staff Qualification Infomation
        // Staff Qualification Information ...

        public ActionResult StaffQualificationInfo()
        {
            string sStaffID = string.Empty;
            sStaffID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_QUALIFICATION.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchService = new DataTable();
            IList<StaffQualification> liQualification = new List<StaffQualification>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchService = objViewModel.FetchQualificationById(dv).DataSource.Table;
            }
            if (dtFetchService != null && dtFetchService.Rows.Count > 0)
            {
                liQualification = (from DataRow dr in dtFetchService.Rows
                                   select new StaffQualification
                                   {
                                       QualificationId = dr[Common.STF_QUALIFICATION.QUALIFICATION_NO].ToString(),
                                       AilledSubject = dr[Common.STF_QUALIFICATION.ALLIED_SUBJECT].ToString(),
                                       CompletionMonth = dr[Common.STF_QUALIFICATION.COMPLETION_MONTH].ToString(),
                                       CompletionYear = dr[Common.STF_QUALIFICATION.COMPLETION_YEAR].ToString(),
                                       Degree = dr[Common.STF_QUALIFICATION.DEGREE].ToString(),
                                       DegreeType = dr[Common.STF_QUALIFICATION.DEGREE_TYPE].ToString(),
                                       InstituteOfStudy = dr[Common.STF_QUALIFICATION.INSTITUTE_OF_STUDY].ToString(),
                                       MainSubject = dr[Common.STF_QUALIFICATION.MAIN_SUBJECT].ToString(),
                                       Percentange = dr[Common.STF_QUALIFICATION.PERCENTAGE].ToString(),
                                       University = dr[Common.STF_QUALIFICATION.UNIVERSITY].ToString()
                                   }).ToList();
                return View(liQualification);
            }
            else
            {
                return View();
            }

        }

        //Inssert Staff Qualification ...
        public string Qualification(string JsonQualification)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            else
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sStaffID))
            {
                var Result = JsonConvert.DeserializeObject<StaffQualification>(JsonQualification);
                string CompletionYear = CommonMethods.ConvertdatetoyearFromat(Result.CompletionYear);
                //string CompletionMonth = CommonMethods.ConvertdatetoyearFromat(Result.Completion);
                dv.Clear();
                dv.Add(Common.STF_QUALIFICATION.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.MAIN_SUBJECT, Result.MainSubject, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.ALLIED_SUBJECT, Result.AilledSubject, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.COMPLETION_YEAR, CompletionYear, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.COMPLETION_MONTH, Result.Completion, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.INSTITUTE_OF_STUDY, Result.InstituteOfStudy, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.DEGREE, Result.Degree, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.UNIVERSITY, Result.University, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.PERCENTAGE, Result.Percentange, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.InsertStaffQualification(dv);
                    sResult = (resultArgs.Success) ? "Record saved successfully ...!" : "Record not saved successfully ...!";
                }
            }
            else
                sResult = "Session Has Expired Try Again ...!";

            return sResult;
        }

        // Fetch Staff QualificationData...
        public JsonResult FetchQualification(string sSatffID)
        {
            dv.Clear();
            dv.Add(Common.STF_QUALIFICATION.STAFF_NO, sSatffID, EnumCommand.DataType.String);
            DataTable dtFetchQualification = new DataTable();
            using (StaffViewModel objStaffViewModel = new StaffViewModel())
            {
                dtFetchQualification = objStaffViewModel.FetchQualificationById(dv).DataSource.Table;
            }
            if (dtFetchQualification != null && dtFetchQualification.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.QUALIFICATION_NO].ToString();
                StaffQualification objStaffInfo = new StaffQualification()
                {
                    QualificationId = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.QUALIFICATION_NO].ToString(),
                    MainSubject = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.MAIN_SUBJECT].ToString(),
                    AilledSubject = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.ALLIED_SUBJECT].ToString(),
                    CompletionMonth = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.COMPLETION_MONTH].ToString(),
                    CompletionYear = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.COMPLETION_YEAR].ToString(),
                    InstituteOfStudy = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.INSTITUTE_OF_STUDY].ToString(),
                    University = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.UNIVERSITY].ToString(),
                    Percentange = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.PERCENTAGE].ToString(),
                    Degree = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.DEGREE].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Bind Qualification ...
        public JsonResult BindQualification(string sQualification)
        {
            dv.Clear();
            dv.Add(Common.STF_QUALIFICATION.QUALIFICATION_NO, sQualification, EnumCommand.DataType.String);
            DataTable dtFetchQualification = new DataTable();
            using (StaffViewModel objStaffViewModel = new StaffViewModel())
            {
                dtFetchQualification = objStaffViewModel.BindQualification(dv).DataSource.Table;
            }
            if (dtFetchQualification != null && dtFetchQualification.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.QUALIFICATION_NO].ToString();
                StaffQualification objStaffInfo = new StaffQualification()
                {
                    MainSubject = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.MAIN_SUBJECT].ToString(),
                    AilledSubject = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.ALLIED_SUBJECT].ToString(),
                    CompletionMonth = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.COMPLETION_MONTH].ToString(),
                    CompletionYear = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.COMPLETION_YEAR].ToString(),
                    InstituteOfStudy = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.INSTITUTE_OF_STUDY].ToString(),
                    University = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.UNIVERSITY].ToString(),
                    Percentange = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.PERCENTAGE].ToString(),
                    Degree = dtFetchQualification.Rows[0][Common.STF_QUALIFICATION.DEGREE].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Fetch DropDown Qualification ...
        public string DDLQualification()
        {
            string sOption = string.Empty;
            // Degree ...
            DataTable dtFetchDegree = new DataTable();
            dtFetchDegree = SupportDataMethod.FetchDegree().DataSource.Table;
            if (dtFetchDegree != null && dtFetchDegree.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchDegree.Rows)
                {
                    sOption += "<option value='" + item[Common.SUP_DEGREE.DEGREE_ID].ToString() + "'>" + item[Common.SUP_DEGREE.DEGREE].ToString() + "</option>";
                }
            }
            return sOption;
        }

        // Staff Qualification Information ...

        public ActionResult EditStaffQualificationInfo()
        {
            string sStaffID = string.Empty;
            sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_QUALIFICATION.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchService = new DataTable();
            IList<StaffQualification> liQualification = new List<StaffQualification>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchService = objViewModel.FetchQualificationById(dv).DataSource.Table;
            }
            if (dtFetchService != null && dtFetchService.Rows.Count > 0)
            {
                liQualification = (from DataRow dr in dtFetchService.Rows
                                   select new StaffQualification
                                   {
                                       QualificationId = dr[Common.STF_QUALIFICATION.QUALIFICATION_NO].ToString(),
                                       AilledSubject = dr[Common.STF_QUALIFICATION.ALLIED_SUBJECT].ToString(),
                                       CompletionMonth = dr[Common.STF_QUALIFICATION.COMPLETION_MONTH].ToString(),
                                       CompletionYear = dr[Common.STF_QUALIFICATION.COMPLETION_YEAR].ToString(),
                                       Degree = dr[Common.STF_QUALIFICATION.DEGREE].ToString(),
                                       DegreeType = dr[Common.STF_QUALIFICATION.DEGREE_TYPE].ToString(),
                                       InstituteOfStudy = dr[Common.STF_QUALIFICATION.INSTITUTE_OF_STUDY].ToString(),
                                       MainSubject = dr[Common.STF_QUALIFICATION.MAIN_SUBJECT].ToString(),
                                       Percentange = dr[Common.STF_QUALIFICATION.PERCENTAGE].ToString(),
                                       University = dr[Common.STF_QUALIFICATION.UNIVERSITY].ToString()
                                   }).ToList();
                return View(liQualification);
            }
            else
            {
                return View();
            }

        }

        // UPdate Qualification ...
        public string UpdateStaffQualification(string JsonQualification)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            else
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sStaffID))
            {
                var Result = JsonConvert.DeserializeObject<StaffQualification>(JsonQualification);
                string CompletionYear = CommonMethods.ConvertdatetoyearFromat(Result.CompletionYear);
                //string CompletionMonth = CommonMethods.ConvertdatetoyearFromat(Result.Completion);
                dv.Clear();
                dv.Add(Common.STF_QUALIFICATION.MAIN_SUBJECT, Result.MainSubject, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.ALLIED_SUBJECT, Result.AilledSubject, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.COMPLETION_YEAR, CompletionYear, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.COMPLETION_MONTH, Result.Completion, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.INSTITUTE_OF_STUDY, Result.InstituteOfStudy, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.DEGREE, Result.Degree, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.UNIVERSITY, Result.University, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.PERCENTAGE, Result.Percentange, EnumCommand.DataType.String);
                dv.Add(Common.STF_QUALIFICATION.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    var sQualificationNo = Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID].ToString();
                    dv.Add(Common.STF_QUALIFICATION.QUALIFICATION_NO, sQualificationNo, EnumCommand.DataType.String);
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.UpdateStaffQualification(dv);
                        sResult = (resultArgs.Success) ? "Record saved successfully ...!" : "Record not saved try again ...!";
                    }
                }
                else
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.InsertStaffQualification(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record updated successfully ...!";
                        }
                        else
                            sResult = "Record not updated try again ...!";
                    }
                }
            }
            else
                sResult = "Session Has Expired Login And Try Again ...!";
            return sResult;
        }

        // Delete Staff Qualification ...
        public string DeleteQualification(string sQualificationId)
        {
            dv.Clear();
            dv.Add(Common.STF_QUALIFICATION.QUALIFICATION_NO, sQualificationId, EnumCommand.DataType.String);
            dv.Add(Common.STF_QUALIFICATION.ISDELETED, "1", EnumCommand.DataType.String);
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                resultArgs = objViewModel.DeleteQualification(dv);
                sResult = (resultArgs.Success) ? "Record deleted successfully ...!" : "Record not deleted try again...!";
            }
            return sResult;
        }
        #endregion

        #region Staff paper Infomation
        // Staff Paper Information ...

        public ActionResult StaffPaperInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_PAPER.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchPaperInfo = new DataTable();
            IList<StaffPaper> liPaper = new List<StaffPaper>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchPaperInfo = objViewModel.FetchPaperById(dv).DataSource.Table;
            }
            if (dtFetchPaperInfo != null && dtFetchPaperInfo.Rows.Count > 0)
            {
                liPaper = (from DataRow dr in dtFetchPaperInfo.Rows
                           select new StaffPaper
                           {
                               PAPERID = dr[Common.STF_PAPER.PAPER_ID].ToString(),
                               JOURNALNAME = dr[Common.STF_PAPER.JOURNAL_NAME].ToString(),
                               LEVEL = dr[Common.sup_LEVEL.LEVEL].ToString(),
                               NOOFPAGES = dr[Common.STF_PAPER.NO_OF_PAGES].ToString(),
                               PAGESFROM = dr[Common.STF_PAPER.PAGES_FROM].ToString(),
                               PAGESTO = dr[Common.STF_PAPER.PAGES_TO].ToString(),
                               PAPERNAME = dr[Common.STF_PAPER.PAPER_NAME].ToString(),
                               PUBLISHINGCATEGORY = dr[Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY].ToString(),
                               VOLUME = dr[Common.Sup_VOLUME.VOLUME].ToString(),
                               YEARPUBLISHED = dr[Common.STF_PAPER.YEAR_PUBLISHED].ToString()
                           }).ToList();
                return View(liPaper);
            }
            else
            {
                return View();
            }

        }

        //Insert Staff Paper ..
        public string Paper(string JsonPaperInfo)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sStaffID))
            {
                var Result = JsonConvert.DeserializeObject<StaffPaper>(JsonPaperInfo);
                string YearPublished = CommonMethods.ConvertdatetoyearFromat(Result.YEARPUBLISHED);
                dv.Clear();
                dv.Add(Common.STF_PAPER.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.PAPER_NAME, Result.PAPERNAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.LEVEL, Result.LEVEL, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.PUBLISHING_CATEGORY, Result.PUBLISHINGCATEGORY, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.JOURNAL_NAME, Result.JOURNALNAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.NO_OF_PAGES, Result.NOOFPAGES, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.PAGES_FROM, Result.PAGESFROM, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.PAGES_TO, Result.PAGESTO, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.VOLUME, Result.VOLUME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.YEAR_PUBLISHED, YearPublished, EnumCommand.DataType.String);

                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.InsertStaffPaper(dv);
                    sResult = (resultArgs.Success) ? "Record saved successfully ...!" : "Record not saved try again ...!";
                }
            }
            else
                sResult = "Session Has Expired Login And Try Again ...!";
            return sResult;
        }

        // Fetch Staff PaperData..
        public JsonResult FetchpaperData(string sStaffId)
        {
            if (sStaffId == null)
            {
                sStaffId = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            }
            dv.Clear();
            dv.Add(Common.STF_PAPER.STAFF_NO, sStaffId, EnumCommand.DataType.String);
            DataTable dtFetchPaperData = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchPaperData = objStaff.FetchPaperById(dv).DataSource.Table;
            }
            if (dtFetchPaperData != null && dtFetchPaperData.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchPaperData.Rows[0][Common.STF_PAPER.PAPER_ID].ToString();
                StaffPaper objStaffInfo = new StaffPaper()
                {
                    PAPERID = dtFetchPaperData.Rows[0][Common.STF_PAPER.PAPER_ID].ToString(),
                    PAPERNAME = dtFetchPaperData.Rows[0][Common.STF_PAPER.PAPER_NAME].ToString(),
                    LEVEL = dtFetchPaperData.Rows[0][Common.STF_PAPER.LEVEL].ToString(),
                    JOURNALNAME = dtFetchPaperData.Rows[0][Common.STF_PAPER.JOURNAL_NAME].ToString(),
                    NOOFPAGES = dtFetchPaperData.Rows[0][Common.STF_PAPER.NO_OF_PAGES].ToString(),
                    PAGESTO = dtFetchPaperData.Rows[0][Common.STF_PAPER.PAGES_TO].ToString(),
                    PAGESFROM = dtFetchPaperData.Rows[0][Common.STF_PAPER.PAGES_FROM].ToString(),
                    PUBLISHINGCATEGORY = dtFetchPaperData.Rows[0][Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY].ToString(),
                    VOLUME = dtFetchPaperData.Rows[0][Common.STF_PAPER.VOLUME].ToString(),
                    YEARPUBLISHED = dtFetchPaperData.Rows[0][Common.STF_PAPER.YEAR_PUBLISHED].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }

        }

        //Bind Values To Controlers ..
        public JsonResult BindControles(string sPaperID)
        {
            dv.Clear();
            dv.Add(Common.STF_PAPER.PAPER_ID, sPaperID, EnumCommand.DataType.String);
            DataTable dtFetchpaper = new DataTable();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchpaper = objViewModel.BindPaperInfo(dv).DataSource.Table;
            }
            if (dtFetchpaper != null && dtFetchpaper.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchpaper.Rows[0][Common.STF_PAPER.PAPER_ID].ToString();
                StaffPaper objPaper = new StaffPaper()
                {
                    JOURNALNAME = dtFetchpaper.Rows[0][Common.STF_PAPER.JOURNAL_NAME].ToString(),
                    LEVEL = dtFetchpaper.Rows[0][Common.STF_PAPER.LEVEL].ToString(),
                    NOOFPAGES = dtFetchpaper.Rows[0][Common.STF_PAPER.NO_OF_PAGES].ToString(),
                    PAGESFROM = dtFetchpaper.Rows[0][Common.STF_PAPER.PAGES_FROM].ToString(),
                    PAGESTO = dtFetchpaper.Rows[0][Common.STF_PAPER.PAGES_TO].ToString(),
                    PAPERNAME = dtFetchpaper.Rows[0][Common.STF_PAPER.PAPER_NAME].ToString(),
                    PUBLISHINGCATEGORY = dtFetchpaper.Rows[0][Common.STF_PAPER.PUBLISHING_CATEGORY].ToString(),
                    VOLUME = dtFetchpaper.Rows[0][Common.STF_PAPER.VOLUME].ToString(),
                    YEARPUBLISHED = dtFetchpaper.Rows[0][Common.STF_PAPER.YEAR_PUBLISHED].ToString()
                };
                return Json(objPaper);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Staff Paper Information ...

        public ActionResult EditStaffPaperInfo()
        {
            string sStaffID = string.Empty;
            sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_PAPER.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchPaperInfo = new DataTable();
            IList<StaffPaper> liPaper = new List<StaffPaper>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchPaperInfo = objViewModel.FetchPaperById(dv).DataSource.Table;
            }
            if (dtFetchPaperInfo != null && dtFetchPaperInfo.Rows.Count > 0)
            {
                liPaper = (from DataRow dr in dtFetchPaperInfo.Rows
                           select new StaffPaper
                           {
                               PAPERID = dr[Common.STF_PAPER.PAPER_ID].ToString(),
                               JOURNALNAME = dr[Common.STF_PAPER.JOURNAL_NAME].ToString(),
                               LEVEL = dr[Common.sup_LEVEL.LEVEL].ToString(),
                               NOOFPAGES = dr[Common.STF_PAPER.NO_OF_PAGES].ToString(),
                               PAGESFROM = dr[Common.STF_PAPER.PAGES_FROM].ToString(),
                               PAGESTO = dr[Common.STF_PAPER.PAGES_TO].ToString(),
                               PAPERNAME = dr[Common.STF_PAPER.PAPER_NAME].ToString(),
                               PUBLISHINGCATEGORY = dr[Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY].ToString(),
                               VOLUME = dr[Common.Sup_VOLUME.VOLUME].ToString(),
                               YEARPUBLISHED = dr[Common.STF_PAPER.YEAR_PUBLISHED].ToString()
                           }).ToList();
                return View(liPaper);
            }
            else
            {
                return View();
            }

        }

        // Bind DropDown List ....
        public string StaffPaperDropDown()
        {
            string sVolume = string.Empty;
            string sLevel = string.Empty;
            string sPublishCategory = string.Empty;
            string JsonData = string.Empty;

            // Volume ...
            DataTable dtFetchVolume = new DataTable();
            dtFetchVolume = SupportDataMethod.FetchVolume().DataSource.Table;
            if (dtFetchVolume != null && dtFetchVolume.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchVolume.Rows)
                {
                    sVolume += "<option value='" + item[Common.Sup_VOLUME.VOLUME_ID].ToString() + "'>" + item[Common.Sup_VOLUME.VOLUME].ToString() + "</option>";
                }
            }

            // Level ...
            DataTable dtFetchLevel = new DataTable();
            dtFetchLevel = SupportDataMethod.FetchLevel().DataSource.Table;
            if (dtFetchVolume != null && dtFetchVolume.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchLevel.Rows)
                {
                    sLevel += "<option value='" + item[Common.sup_LEVEL.LEVEL_ID].ToString() + "'>" + item[Common.sup_LEVEL.LEVEL].ToString() + "</option>";
                }
            }

            // Publish Category ...
            DataTable dtFetchPublishCategory = new DataTable();
            dtFetchPublishCategory = SupportDataMethod.FetchPublishCategory().DataSource.Table;
            if (dtFetchPublishCategory != null && dtFetchPublishCategory.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchPublishCategory.Rows)
                {
                    sPublishCategory += "<option value='" + item[Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY_ID].ToString() + "'>" + item[Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY].ToString() + "</option>";
                }
            }

            var objJsonData = new DDLForPaperInfo() { Level = sLevel, PublishCategory = sPublishCategory, Volume = sVolume };
            JsonData = JsonConvert.SerializeObject(objJsonData);

            //   JsonData = "{'Volume':'"+ sVolume + "','Level':'" + sLevel + "','PublishCategory':'" + sPublishCategory + "'}";

            return JsonData;
        }

        // Update Staff Paper ..
        public string UpdateStaffPaper(string JsonPaperInfo)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }

            if (JsonPaperInfo != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffPaper>(JsonPaperInfo);
                string YearPublished = CommonMethods.ConvertdatetoyearFromat(Result.YEARPUBLISHED);
                dv.Clear();
                dv.Add(Common.STF_PAPER.PAPER_NAME, Result.PAPERNAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.LEVEL, Result.LEVEL, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.PUBLISHING_CATEGORY, Result.PUBLISHINGCATEGORY, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.JOURNAL_NAME, Result.JOURNALNAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.NO_OF_PAGES, Result.NOOFPAGES, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.PAGES_FROM, Result.PAGESFROM, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.PAGES_TO, Result.PAGESTO, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.VOLUME, Result.VOLUME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.YEAR_PUBLISHED, YearPublished, EnumCommand.DataType.String);
                dv.Add(Common.STF_PAPER.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    var sPaperID = Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID].ToString();
                    dv.Add(Common.STF_PAPER.PAPER_ID, sPaperID, EnumCommand.DataType.String);

                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.UpdateStaffPaper(dv);
                        if (resultArgs.Success)
                        {
                            sResult = "Record updated successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }
                    }
                }
                else
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.InsertStaffPaper(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }
            }
            return sResult;
        }

        // Delete Paper Record ..
        public string DeleteStaffPaper(string sPaperID)
        {
            dv.Clear();
            dv.Add(Common.STF_PAPER.PAPER_ID, sPaperID, EnumCommand.DataType.String);
            dv.Add(Common.STF_PAPER.IS_DELETED, "1", EnumCommand.DataType.String);
            using (StaffViewModel objViwModel = new StaffViewModel())
            {
                resultArgs = objViwModel.DeletePaperInfo(dv);
                if (resultArgs.Success)
                {
                    sResult = "Record deleted successfully ...!";
                }
                else
                {
                    sResult = "Record not  deleted try again...!";
                }
            }
            return sResult;
        }
        #endregion

        #region Staff Book Publish Infomation
        // Staff Publish Information ...

        public ActionResult StaffPublishInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_PUBLISH.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchPublish = new DataTable();
            IList<StaffPublish> liPublish = new List<StaffPublish>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchPublish = objViewModel.FetchPublishById(dv).DataSource.Table;
            }
            if (dtFetchPublish != null && dtFetchPublish.Rows.Count > 0)
            {
                liPublish = (from DataRow dr in dtFetchPublish.Rows
                             select new StaffPublish
                             {
                                 PUBLISHID = dr[Common.STF_PUBLISH.PUBLISH_ID].ToString(),
                                 BOOKNAME = dr[Common.STF_PUBLISH.BOOK_NAME].ToString(),
                                 EDITION = dr[Common.STF_PUBLISH.EDITION].ToString(),
                                 PJOURNALNAME = dr[Common.STF_PUBLISH.JOURNAL_NAME].ToString(),
                                 PLEVEL = dr[Common.STF_PUBLISH.LEVEL].ToString(),
                                 PPUBLISHINGCATEGORY = dr[Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY].ToString(),
                                 PUBLISHER = dr[Common.STF_PUBLISH.PUBLISHER].ToString(),
                                 PVOLUME = dr[Common.STF_PUBLISH.VOLUME].ToString(),
                                 PYEAR = dr[Common.STF_PUBLISH.PYEAR].ToString()
                             }).ToList();
                return View(liPublish);
            }
            else
            {
                return View();
            }

        }

        //Insert Staff Publish ..
        public string Publish(string JsonPublish)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }
            if (JsonPublish != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffPublish>(JsonPublish);
                string Year = CommonMethods.ConvertdatetoyearFromat(Result.PYEAR);
                dv.Clear();
                dv.Add(Common.STF_PUBLISH.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.BOOK_NAME, Result.BOOKNAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.JOURNAL_NAME, Result.PJOURNALNAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.PUBLISHER, Result.PUBLISHER, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.PYEAR, Year, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.EDITION, Result.EDITION, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.LEVEL, Result.PLEVEL, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.VOLUME, Result.PVOLUME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.PUBLISHING_CATEGORY, Result.PPUBLISHINGCATEGORY, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.InsertStaffPublish(dv);
                    if (resultArgs.Success)
                    {
                        sResult = "Record saved successfully ...!";
                    }
                    else
                    {
                        sResult = "Record not saved successfully ...!";
                    }
                }
            }

            return sResult;
        }

        // Fetch Staff PublishData...
        public JsonResult FetchPublishData(string sStaffID)
        {
            dv.Clear();
            dv.Add(Common.STF_PUBLISH.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchPublish = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchPublish = objStaff.FetchPublishById(dv).DataSource.Table;
            }
            if (dtFetchPublish != null && dtFetchPublish.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchPublish.Rows[0][Common.STF_PUBLISH.PUBLISH_ID].ToString();
                StaffPublish objStaffInfo = new StaffPublish()
                {
                    PUBLISHID = dtFetchPublish.Rows[0][Common.STF_PUBLISH.PUBLISH_ID].ToString(),
                    BOOKNAME = dtFetchPublish.Rows[0][Common.STF_PUBLISH.BOOK_NAME].ToString(),
                    PJOURNALNAME = dtFetchPublish.Rows[0][Common.STF_PUBLISH.JOURNAL_NAME].ToString(),
                    PUBLISHER = dtFetchPublish.Rows[0][Common.STF_PUBLISH.PUBLISHER].ToString(),
                    PYEAR = dtFetchPublish.Rows[0][Common.STF_PUBLISH.PYEAR].ToString(),
                    EDITION = dtFetchPublish.Rows[0][Common.STF_PUBLISH.EDITION].ToString(),
                    PLEVEL = dtFetchPublish.Rows[0][Common.STF_PUBLISH.LEVEL].ToString(),
                    PPUBLISHINGCATEGORY = dtFetchPublish.Rows[0][Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY].ToString(),
                    PVOLUME = dtFetchPublish.Rows[0][Common.STF_PUBLISH.VOLUME].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // BindPublish Details...
        public JsonResult BindPublish(string sPublishID)
        {
            dv.Clear();
            dv.Add(Common.STF_PUBLISH.PUBLISH_ID, sPublishID, EnumCommand.DataType.String);
            DataTable dtFetchPublish = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchPublish = objStaff.BindPublish(dv).DataSource.Table;
            }
            if (dtFetchPublish != null && dtFetchPublish.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchPublish.Rows[0][Common.STF_PUBLISH.PUBLISH_ID].ToString();
                StaffPublish objStaffInfo = new StaffPublish()
                {
                    BOOKNAME = dtFetchPublish.Rows[0][Common.STF_PUBLISH.BOOK_NAME].ToString(),
                    PJOURNALNAME = dtFetchPublish.Rows[0][Common.STF_PUBLISH.JOURNAL_NAME].ToString(),
                    PUBLISHER = dtFetchPublish.Rows[0][Common.STF_PUBLISH.PUBLISHER].ToString(),
                    PYEAR = dtFetchPublish.Rows[0][Common.STF_PUBLISH.PYEAR].ToString(),
                    EDITION = dtFetchPublish.Rows[0][Common.STF_PUBLISH.EDITION].ToString(),
                    PLEVEL = dtFetchPublish.Rows[0][Common.STF_PUBLISH.LEVEL].ToString(),
                    PPUBLISHINGCATEGORY = dtFetchPublish.Rows[0][Common.STF_PUBLISH.PUBLISHING_CATEGORY].ToString(),
                    PVOLUME = dtFetchPublish.Rows[0][Common.STF_PUBLISH.VOLUME].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Load DropDown List ..
        public string DDLPublish()
        {
            string sLevel = string.Empty;
            string sVolume = string.Empty;
            string sPublishCategory = string.Empty;
            string JsonData = string.Empty;

            // Volume ...
            DataTable dtFetchVolume = new DataTable();
            dtFetchVolume = SupportDataMethod.FetchVolume().DataSource.Table;
            if (dtFetchVolume != null && dtFetchVolume.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchVolume.Rows)
                {
                    sVolume += "<option value='" + item[Common.Sup_VOLUME.VOLUME_ID].ToString() + "'>" + item[Common.Sup_VOLUME.VOLUME].ToString() + "</option>";
                }
            }

            // Level ...
            DataTable dtFetchLevel = new DataTable();
            dtFetchLevel = SupportDataMethod.FetchLevel().DataSource.Table;
            if (dtFetchVolume != null && dtFetchVolume.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchLevel.Rows)
                {
                    sLevel += "<option value='" + item[Common.sup_LEVEL.LEVEL_ID].ToString() + "'>" + item[Common.sup_LEVEL.LEVEL].ToString() + "</option>";
                }
            }

            // Publish Category ...
            DataTable dtFetchPublishCategory = new DataTable();
            dtFetchPublishCategory = SupportDataMethod.FetchPublishCategory().DataSource.Table;
            if (dtFetchPublishCategory != null && dtFetchPublishCategory.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchPublishCategory.Rows)
                {
                    sPublishCategory += "<option value='" + item[Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY_ID].ToString() + "'>" + item[Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY].ToString() + "</option>";
                }
            }

            var objJsonData = new DDLForPaperInfo() { Level = sLevel, PublishCategory = sPublishCategory, Volume = sVolume };
            JsonData = JsonConvert.SerializeObject(objJsonData);

            return JsonData;
        }

        // Staff Publish Information ...

        public ActionResult EditStaffPublishInfo()
        {
            string sStaffID = string.Empty;
            sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_PUBLISH.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchPublish = new DataTable();
            IList<StaffPublish> liPublish = new List<StaffPublish>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchPublish = objViewModel.FetchPublishById(dv).DataSource.Table;
            }
            if (dtFetchPublish != null && dtFetchPublish.Rows.Count > 0)
            {
                liPublish = (from DataRow dr in dtFetchPublish.Rows
                             select new StaffPublish
                             {
                                 PUBLISHID = dr[Common.STF_PUBLISH.PUBLISH_ID].ToString(),
                                 BOOKNAME = dr[Common.STF_PUBLISH.BOOK_NAME].ToString(),
                                 EDITION = dr[Common.STF_PUBLISH.EDITION].ToString(),
                                 PJOURNALNAME = dr[Common.STF_PUBLISH.JOURNAL_NAME].ToString(),
                                 PLEVEL = dr[Common.STF_PUBLISH.LEVEL].ToString(),
                                 PPUBLISHINGCATEGORY = dr[Common.Sup_PUBLISH_CATEGORY.PUBLISH_CATEGORY].ToString(),
                                 PUBLISHER = dr[Common.STF_PUBLISH.PUBLISHER].ToString(),
                                 PVOLUME = dr[Common.STF_PUBLISH.VOLUME].ToString(),
                                 PYEAR = dr[Common.STF_PUBLISH.PYEAR].ToString()
                             }).ToList();
                return View(liPublish);
            }
            else
            {
                return View();
            }
        }

        // Update Staff Publish ..
        public string UpdateStaffPublish(string JsonPublish)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }

            if (JsonPublish != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffPublish>(JsonPublish);
                string PublishYear = CommonMethods.ConvertdatetoyearFromat(Result.PYEAR);
                dv.Clear();
                dv.Add(Common.STF_PUBLISH.BOOK_NAME, Result.BOOKNAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.JOURNAL_NAME, Result.PJOURNALNAME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.PUBLISHER, Result.PUBLISHER, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.PYEAR, PublishYear, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.EDITION, Result.EDITION, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.VOLUME, Result.PVOLUME, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.PUBLISHING_CATEGORY, Result.PPUBLISHINGCATEGORY, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.LEVEL, Result.PLEVEL, EnumCommand.DataType.String);
                dv.Add(Common.STF_PUBLISH.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        var sPublishID = Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID].ToString();
                        dv.Add(Common.STF_PUBLISH.PUBLISH_ID, sPublishID, EnumCommand.DataType.String);

                        resultArgs = objStaff.UpdateStaffPublish(dv);
                        if (resultArgs.Success)
                        {
                            sResult = "Record updated successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }
                    }
                }
                else
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.InsertStaffPublish(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }

            }
            return sResult;
        }

        // Delete Staff Publish ...
        public string DeletePublish(string sPublishId)
        {
            dv.Clear();
            dv.Add(Common.STF_PUBLISH.PUBLISH_ID, sPublishId, EnumCommand.DataType.String);
            dv.Add(Common.STF_PUBLISH.ISDELETED, "1", EnumCommand.DataType.String);
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                resultArgs = objViewModel.DeletePublish(dv);
                if (resultArgs.Success)
                {
                    sResult = "Record deleted successfully ...!";
                }
                else
                {
                    sResult = "Record not deleted try again...!";
                }
            }
            return sResult;
        }
        #endregion

        #region Staff Counselling Infomation
        // Staff Counselling Information ...

        public ActionResult StaffCounsellingInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_COUNSELING.STAFF, sStaffID, EnumCommand.DataType.String);
            DataTable dtCounselling = new DataTable();
            IList<StaffCounseling> liCounselling = new List<StaffCounseling>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtCounselling = objViewModel.FetchCounselingById(dv).DataSource.Table;
            }
            if (dtCounselling != null && dtCounselling.Rows.Count > 0)
            {
                liCounselling = (from DataRow dr in dtCounselling.Rows
                                 select new StaffCounseling
                                 {
                                     CounsellingID = dr[Common.STF_COUNSELING.STF_COUN_ID].ToString(),
                                     ACTIONTAKEN = dr[Common.STF_COUNSELING.ACTION_TAKEN].ToString(),
                                     DateOfCounsel = dr[Common.STF_COUNSELING.DATE_OF_COUN].ToString(),
                                     DURATION = dr[Common.STF_COUNSELING.DURATION].ToString(),
                                     GIVEN = dr[Common.STF_COUNSELING.GIVEN].ToString(),
                                     REASON = dr[Common.STF_COUNSELING.REASON].ToString(),
                                     REMARK = dr[Common.STF_COUNSELING.REMARK].ToString()
                                 }).ToList();
                return View(liCounselling);
            }
            else
            {
                return View();
            }
        }

        // Insert Staff Counseling ...
        public string Counseling(string JsonCounselling)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }
            if (JsonCounselling != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffCounseling>(JsonCounselling);
                string DateOfCounselling = CommonMethods.ConvertdatetoyearFromat(Result.DateOfCounsel);
                dv.Clear();
                dv.Add(Common.STF_COUNSELING.STAFF, sStaffID, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.DATE_OF_COUN, DateOfCounselling, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.DURATION, Result.DURATION, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.REASON, Result.REASON, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.GIVEN, Result.GIVEN, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.REMARK, Result.REMARK, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.ACTION_TAKEN, Result.ACTIONTAKEN, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.InsertStaffCounseling(dv);
                    if (resultArgs.Success)
                    {
                        sResult = "Record saved successfully ...!";
                    }
                    else
                    {
                        sResult = "Record not saved successfully ...!";
                    }
                }
            }

            return sResult;
        }

        // Fetch Staff CounselingData...
        public JsonResult FetchCounselingData(string sStaffID)
        {
            dv.Clear();
            dv.Add(Common.STF_COUNSELING.STAFF, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchCounseling = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchCounseling = objStaff.FetchCounselingById(dv).DataSource.Table;
            }
            if (dtFetchCounseling != null && dtFetchCounseling.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchCounseling.Rows[0][Common.STF_COUNSELING.STF_COUN_ID].ToString();
                StaffCounseling objStaffInfo = new StaffCounseling()
                {
                    DateOfCounsel = dtFetchCounseling.Rows[0][Common.STF_COUNSELING.DATE_OF_COUN].ToString(),
                    REASON = dtFetchCounseling.Rows[0][Common.STF_COUNSELING.REASON].ToString(),
                    GIVEN = dtFetchCounseling.Rows[0][Common.STF_COUNSELING.GIVEN].ToString(),
                    ACTIONTAKEN = dtFetchCounseling.Rows[0][Common.STF_COUNSELING.ACTION_TAKEN].ToString(),
                    REMARK = dtFetchCounseling.Rows[0][Common.STF_COUNSELING.REMARK].ToString(),
                    DURATION = dtFetchCounseling.Rows[0][Common.STF_COUNSELING.DURATION].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Bind Values In Counselling Countrols ...
        public JsonResult BindCounselling(string sCounsellingID)
        {
            dv.Clear();
            dv.Add(Common.STF_COUNSELING.STF_COUN_ID, sCounsellingID, EnumCommand.DataType.String);
            DataTable dtFetchCounselling = new DataTable();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchCounselling = objViewModel.BindCounselling(dv).DataSource.Table;
            }
            if (dtFetchCounselling != null && dtFetchCounselling.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchCounselling.Rows[0][Common.STF_COUNSELING.STF_COUN_ID].ToString();
                StaffCounseling objCounselling = new StaffCounseling()
                {
                    ACTIONTAKEN = dtFetchCounselling.Rows[0][Common.STF_COUNSELING.ACTION_TAKEN].ToString(),
                    DateOfCounsel = dtFetchCounselling.Rows[0][Common.STF_COUNSELING.DATE_OF_COUN].ToString(),
                    DURATION = dtFetchCounselling.Rows[0][Common.STF_COUNSELING.DURATION].ToString(),
                    GIVEN = dtFetchCounselling.Rows[0][Common.STF_COUNSELING.GIVEN].ToString(),
                    REASON = dtFetchCounselling.Rows[0][Common.STF_COUNSELING.REASON].ToString(),
                    REMARK = dtFetchCounselling.Rows[0][Common.STF_COUNSELING.REMARK].ToString()
                };
                return Json(objCounselling);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }

        }

        // Staff Counselling Information ...

        public ActionResult EditStaffCounsellingInfo()
        {
            string sStaffID = string.Empty;
            sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_COUNSELING.STAFF, sStaffID, EnumCommand.DataType.String);
            DataTable dtCounselling = new DataTable();
            IList<StaffCounseling> liCounselling = new List<StaffCounseling>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtCounselling = objViewModel.FetchCounselingById(dv).DataSource.Table;
            }
            if (dtCounselling != null && dtCounselling.Rows.Count > 0)
            {
                liCounselling = (from DataRow dr in dtCounselling.Rows
                                 select new StaffCounseling
                                 {
                                     CounsellingID = dr[Common.STF_COUNSELING.STF_COUN_ID].ToString(),
                                     ACTIONTAKEN = dr[Common.STF_COUNSELING.ACTION_TAKEN].ToString(),
                                     DateOfCounsel = dr[Common.STF_COUNSELING.DATE_OF_COUN].ToString(),
                                     DURATION = dr[Common.STF_COUNSELING.DURATION].ToString(),
                                     GIVEN = dr[Common.STF_COUNSELING.GIVEN].ToString(),
                                     REASON = dr[Common.STF_COUNSELING.REASON].ToString(),
                                     REMARK = dr[Common.STF_COUNSELING.REMARK].ToString()
                                 }).ToList();
                return View(liCounselling);
            }
            else
            {
                return View();
            }

        }

        // Update Staff Counseling ...
        public string UpdateCounseling(string JsonCounselling)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }

            if (JsonCounselling != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffCounseling>(JsonCounselling);
                string sDateOfCounselling = CommonMethods.ConvertdatetoyearFromat(Result.DateOfCounsel);
                dv.Clear();
                dv.Add(Common.STF_COUNSELING.DATE_OF_COUN, sDateOfCounselling, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.DURATION, Result.DURATION, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.REASON, Result.REASON, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.GIVEN, Result.GIVEN, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.REMARK, Result.REMARK, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.ACTION_TAKEN, Result.ACTIONTAKEN, EnumCommand.DataType.String);
                dv.Add(Common.STF_COUNSELING.STAFF, sStaffID, EnumCommand.DataType.String);
                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        var sCounselingNo = Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID].ToString();
                        dv.Add(Common.STF_COUNSELING.STF_COUN_ID, sCounselingNo, EnumCommand.DataType.String);
                        resultArgs = objStaff.UpdateStaffCounseling(dv);
                        if (resultArgs.Success)
                        {
                            sResult = "Record updated successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }
                    }
                }
                else
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.InsertStaffCounseling(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }

            }
            return sResult;
        }

        // Delete Staff Counselling ..
        public string DeleteCounselling(string sCounsellingId)
        {
            dv.Clear();
            dv.Add(Common.STF_COUNSELING.STF_COUN_ID, sCounsellingId, EnumCommand.DataType.String);
            dv.Add(Common.STF_COUNSELING.IS_DELETED, "1", EnumCommand.DataType.String);
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                resultArgs = objViewModel.DeleteStaffCounselling(dv);
                if (resultArgs.Success)
                {
                    sResult = "Record deleted successfully ...!";
                }
                else
                {
                    sResult = "Record not deleted try again...!";
                }
            }
            return sResult;
        }
        #endregion

        #region Staff Leaving Infomation
        // Staff Leaving Information ...

        public ActionResult StaffLeavingInfo()
        {
            StaffLeavingInfo objStaffLeavingInfo = new StaffLeavingInfo();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            // Department ...
            DataTable dtFetchDepartment = new DataTable();
            dtFetchDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
            List<SupDepartment> liDepartment = new List<SupDepartment>();
            if (dtFetchDepartment != null && dtFetchDepartment.Rows.Count > 0)
            {
                liDepartment = (from DataRow dr in dtFetchDepartment.Rows
                                select new SupDepartment()
                                {
                                    DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                    DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                }).ToList();
                objStaffLeavingInfo.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
            }
            return View(objStaffLeavingInfo);
        }

        //Insert Staff Leaving ...
        public string Leaving(string str)
        {
            var sStaffId = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sStaffId))
            {
                var Result = JsonConvert.DeserializeObject<StaffLeaving>(str);
                string LeavingDate = CommonMethods.ConvertdatetoyearFromat(Result.LeavingDate);
                string RetirementDate = CommonMethods.ConvertdatetoyearFromat(Result.RetirementDate);
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.LEAVING_DATE, LeavingDate, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.LEAVING_REMARKS, Result.LeavingRemark, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DEPARTMENT, Result.Department, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.RETIREMENT_DATE, RetirementDate, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.LEAVING_REASON, Result.LeavingReason, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.IS_LEFT, "1", EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffId, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.UpdateStaffLeaving(dv);
                    sResult = (resultArgs.Success) ? "Record saved successfully ...!" : "Record not saved successfully ...!";
                }
            }
            else
                sResult = "Session Has Expired Please Login And Try Again ...!";
            return sResult;
        }

        // Fetch Staff Leaving ...
        public JsonResult FetchLeavingData(string sStaffID)
        {
            if (!string.IsNullOrEmpty(sStaffID))
            {
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
                DataTable dtFetchLeaving = new DataTable();
                using (StaffViewModel objStaffViewModel = new StaffViewModel())
                {
                    dtFetchLeaving = objStaffViewModel.FetchLeavingById(dv).DataSource.Table;
                }
                if (dtFetchLeaving != null && dtFetchLeaving.Rows.Count > 0)
                {
                    StaffLeavingInfo objStaffInfo = new StaffLeavingInfo()
                    {
                        LEAVINGDATE = dtFetchLeaving.Rows[0][Common.STF_PERSONAL_INFO.LEAVING_DATE].ToString(),
                        LEAVINGREMARKS = dtFetchLeaving.Rows[0][Common.STF_PERSONAL_INFO.LEAVING_REMARKS].ToString(),
                        RETIREMENTDATE = dtFetchLeaving.Rows[0][Common.STF_PERSONAL_INFO.RETIREMENT_DATE].ToString(),
                        LEAVINGREASON = dtFetchLeaving.Rows[0][Common.STF_PERSONAL_INFO.LEAVING_REASON].ToString(),
                        //sDepartment = dtFetchLeaving.Rows[0][Common.STF_PERSONAL_INFO.DEPARTMENT].ToString()
                    };
                    return Json(objStaffInfo);
                }
                else
                {
                    Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                    return Json(sResult);
                }
            }
            else
            {
                sResult = "Bad Request ...!";
                return Json(sResult);
            }
        }

        // Staff Leaving Information ...
        //[UserRights("STAFF")]
        public ActionResult EditStaffLeavingInfo()
        {
            StaffLeavingInfo objStaffLeavingInfo = new StaffLeavingInfo();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            // Department ...
            DataTable dtFetchDepartment = new DataTable();
            dtFetchDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
            List<SupDepartment> liDepartment = new List<SupDepartment>();
            if (dtFetchDepartment != null && dtFetchDepartment.Rows.Count > 0)
            {
                liDepartment = (from DataRow dr in dtFetchDepartment.Rows
                                select new SupDepartment()
                                {
                                    DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                    DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                }).ToList();
                objStaffLeavingInfo.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
            }
            return View(objStaffLeavingInfo);
        }

        //Update Staff Leaving ...
        public JsonResult UpdateLeaving(string str)
        {
            var sStaffId = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            if (str != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffLeaving>(str);
                string LeavingDate = CommonMethods.ConvertdatetoyearFromat(Result.LeavingDate);
                string RetirementDate = CommonMethods.ConvertdatetoyearFromat(Result.RetirementDate);
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.LEAVING_DATE, LeavingDate, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.LEAVING_REMARKS, Result.LeavingRemark, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DEPARTMENT, Result.Department, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.RETIREMENT_DATE, RetirementDate, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.LEAVING_REASON, Result.LeavingReason, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.IS_LEFT, "1", EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffId, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.UpdateStaffLeaving(dv);
                    if (resultArgs.Success)
                    {
                        sResult = "Record updated successfully ...!";
                    }
                    else
                    {
                        sResult = "Record not updated try again ...!";
                    }
                }

            }

            return Json(sResult);
        }

        #endregion

        #region Staff Training Infomation
        // Staff Training Information ...
        public ActionResult StaffTrainingInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_TRAINING.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchTraining = new DataTable();
            IList<StaffTraining> liTraining = new List<StaffTraining>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchTraining = objViewModel.FetchTrainingById(dv).DataSource.Table;
            }
            if (dtFetchTraining != null && dtFetchTraining.Rows.Count > 0)
            {
                liTraining = (from DataRow dr in dtFetchTraining.Rows
                              select new StaffTraining
                              {
                                  TraininngId = dr[Common.STF_TRAINING.TRAINING_NO].ToString(),
                                  COMMENTS = dr[Common.STF_TRAINING.COMMENTS].ToString(),
                                  DateFrom = dr[Common.STF_TRAINING.DATE_FROM].ToString(),
                                  DateTo = dr[Common.STF_TRAINING.DATE_TO].ToString(),
                                  LEVEL = dr[Common.STF_TRAINING.TLEVEL].ToString(),
                                  PLACE = dr[Common.STF_TRAINING.PLACE].ToString(),
                                  PROGRAMME = dr[Common.STF_TRAINING.PROGRAMME].ToString()
                              }).ToList();
                return View(liTraining);
            }
            else
            {
                return View();
            }

        }

        //Insert Staff Training ..
        public string Training(string JsonTraining)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }
            if (JsonTraining != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffTraining>(JsonTraining);
                string DateFrom = CommonMethods.ConvertdatetoyearFromat(Result.DateFrom);
                string DateTo = CommonMethods.ConvertdatetoyearFromat(Result.DateTo);
                dv.Clear();
                dv.Add(Common.STF_TRAINING.DATE_FROM, DateFrom, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.DATE_TO, DateTo, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.PROGRAMME, Result.PROGRAMME, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.PLACE, Result.PLACE, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.TLEVEL, Result.LEVEL, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.COMMENTS, Result.COMMENTS, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.InsertStaffTraining(dv);
                    if (resultArgs.Success)
                    {
                        sResult = "Record saved successfully ...!";
                    }
                    else
                    {
                        sResult = "Record not saved successfully ...!";
                    }
                }
            }

            return sResult;
        }

        // Fetch Staff TrainingData...
        public JsonResult FetchTrainingData(string sStaffID)
        {
            dv.Clear();
            dv.Add(Common.STF_TRAINING.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchTraining = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchTraining = objStaff.FetchTrainingById(dv).DataSource.Table;
            }

            if (dtFetchTraining != null && dtFetchTraining.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchTraining.Rows[0][Common.STF_TRAINING.TRAINING_NO].ToString();

                StaffTraining objStaffInfo = new StaffTraining()
                {
                    TraininngId = dtFetchTraining.Rows[0][Common.STF_TRAINING.TRAINING_NO].ToString(),
                    DateFrom = dtFetchTraining.Rows[0][Common.STF_TRAINING.DATE_FROM].ToString(),
                    DateTo = dtFetchTraining.Rows[0][Common.STF_TRAINING.DATE_TO].ToString(),
                    PROGRAMME = dtFetchTraining.Rows[0][Common.STF_TRAINING.PROGRAMME].ToString(),
                    PLACE = dtFetchTraining.Rows[0][Common.STF_TRAINING.PLACE].ToString(),
                    COMMENTS = dtFetchTraining.Rows[0][Common.STF_TRAINING.COMMENTS].ToString(),
                    LEVEL = dtFetchTraining.Rows[0][Common.STF_TRAINING.TLEVEL].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Bind Values To Controles ...
        public JsonResult BindTraining(string sTrainingID)
        {
            dv.Clear();
            dv.Add(Common.STF_TRAINING.TRAINING_NO, sTrainingID, EnumCommand.DataType.String);
            DataTable dtFetchTraining = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchTraining = objStaff.BindTraining(dv).DataSource.Table;
            }

            if (dtFetchTraining != null && dtFetchTraining.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchTraining.Rows[0][Common.STF_TRAINING.TRAINING_NO].ToString();

                StaffTraining objStaffInfo = new StaffTraining()
                {
                    DateFrom = dtFetchTraining.Rows[0][Common.STF_TRAINING.DATE_FROM].ToString(),
                    DateTo = dtFetchTraining.Rows[0][Common.STF_TRAINING.DATE_TO].ToString(),
                    PROGRAMME = dtFetchTraining.Rows[0][Common.STF_TRAINING.PROGRAMME].ToString(),
                    PLACE = dtFetchTraining.Rows[0][Common.STF_TRAINING.PLACE].ToString(),
                    COMMENTS = dtFetchTraining.Rows[0][Common.STF_TRAINING.COMMENTS].ToString(),
                    LEVEL = dtFetchTraining.Rows[0][Common.STF_TRAINING.TLEVEL].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Staff Training Information ...
        public ActionResult EditStaffTrainingInfo()
        {
            string sStaffID = string.Empty;
            sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_TRAINING.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchTraining = new DataTable();
            IList<StaffTraining> liTraining = new List<StaffTraining>();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchTraining = objViewModel.FetchTrainingById(dv).DataSource.Table;
            }
            if (dtFetchTraining != null && dtFetchTraining.Rows.Count > 0)
            {
                liTraining = (from DataRow dr in dtFetchTraining.Rows
                              select new StaffTraining
                              {
                                  TraininngId = dr[Common.STF_TRAINING.TRAINING_NO].ToString(),
                                  COMMENTS = dr[Common.STF_TRAINING.COMMENTS].ToString(),
                                  DateFrom = dr[Common.STF_TRAINING.DATE_FROM].ToString(),
                                  DateTo = dr[Common.STF_TRAINING.DATE_TO].ToString(),
                                  LEVEL = dr[Common.STF_TRAINING.TLEVEL].ToString(),
                                  PLACE = dr[Common.STF_TRAINING.PLACE].ToString(),
                                  PROGRAMME = dr[Common.STF_TRAINING.PROGRAMME].ToString()
                              }).ToList();
                return View(liTraining);
            }
            else
            {
                return View();
            }

        }

        // Update Training ..
        public string UpdateStaffTraining(string JsonTraining)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }

            if (JsonTraining != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffTraining>(JsonTraining);
                string DateFrom = CommonMethods.ConvertdatetoyearFromat(Result.DateFrom);
                string DateTo = CommonMethods.ConvertdatetoyearFromat(Result.DateTo);
                dv.Clear();
                dv.Add(Common.STF_TRAINING.DATE_FROM, DateFrom, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.DATE_TO, DateTo, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.PROGRAMME, Result.PROGRAMME, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.PLACE, Result.PLACE, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.TLEVEL, Result.LEVEL, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.COMMENTS, Result.COMMENTS, EnumCommand.DataType.String);
                dv.Add(Common.STF_TRAINING.STAFF_NO, sStaffID, EnumCommand.DataType.String);

                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    var sTrainingNo = Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID].ToString();
                    dv.Add(Common.STF_TRAINING.TRAINING_NO, sTrainingNo, EnumCommand.DataType.String);
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.UpdateStaffTraining(dv);
                        if (resultArgs.Success)
                        {
                            sResult = "Record updated successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }
                    }
                }
                else
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.InsertStaffTraining(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }

            }
            return sResult;
        }

        // Delete Staff Training ..
        public string DeleteTraining(string sTrainingId)
        {
            dv.Clear();
            dv.Add(Common.STF_TRAINING.TRAINING_NO, sTrainingId, EnumCommand.DataType.String);
            dv.Add(Common.STF_TRAINING.ISDELETED, "1", EnumCommand.DataType.String);
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                resultArgs = objViewModel.DeleteTraining(dv);
                if (resultArgs.Success)
                {
                    sResult = "Record deleted successfully ...!";
                }
                else
                {
                    sResult = "Record not deleted try again...!";
                }
            }
            return sResult;
        }
        #endregion

        #region Staff Roles Infomation
        // Staff Roles Information ...
        public ActionResult StaffRolesInfo()
        {
            StaffRolesInfo objStaffRolesInfo = new StaffRolesInfo();

            // Shift ...
            DataTable dtFetchShift = new DataTable();
            dtFetchShift = SupportDataMethod.FetchShift().DataSource.Table;
            List<Sup_Shift> liShift = new List<Sup_Shift>();
            if (dtFetchShift != null && dtFetchShift.Rows.Count > 0)
            {
                liShift = (from DataRow dr in dtFetchShift.Rows
                           select new Sup_Shift()
                           {
                               SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                               SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                           }).ToList();
                objStaffRolesInfo.SHIFT = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
            }

            // Qualification ...
            DataTable dtFetchQualification = new DataTable();
            dtFetchQualification = SupportDataMethod.FetchQualification().DataSource.Table;
            List<Sup_Qualification> liQualification = new List<Sup_Qualification>();
            if (dtFetchQualification != null && dtFetchQualification.Rows.Count > 0)
            {
                liQualification = (from DataRow dr in dtFetchQualification.Rows
                                   select new Sup_Qualification()
                                   {
                                       QUALIFICATION_ID = dr[Common.Sup_Qualification.QUALIFICATION_ID].ToString(),
                                       QUALIFICATION = dr[Common.Sup_Qualification.QUALIFICATION].ToString()
                                   }).ToList();
                objStaffRolesInfo.QUALIFICATION = new SelectList(liQualification, Common.Sup_Qualification.QUALIFICATION_ID, Common.Sup_Qualification.QUALIFICATION);
            }

            // Designation ...
            DataTable dtFetchDesignation = new DataTable();
            dtFetchDesignation = SupportDataMethod.FetchDesignation().DataSource.Table;
            List<Sup_Designation> liDesignation = new List<Sup_Designation>();
            if (dtFetchDesignation != null && dtFetchDesignation.Rows.Count > 0)
            {
                liDesignation = (from DataRow dr in dtFetchDesignation.Rows
                                 select new Sup_Designation()
                                 {
                                     DESIGNATIONID = dr[Common.sup_designation.DESIGNATIONID].ToString(),
                                     DESIGNATION = dr[Common.sup_designation.DESIGNATION].ToString()
                                 }).ToList();
                objStaffRolesInfo.DESIGNATION = new SelectList(liDesignation, Common.sup_designation.DESIGNATIONID, Common.sup_designation.DESIGNATION);
            }

            // Sub_Category ...
            DataTable dtFetchSubCategory = new DataTable();
            dtFetchSubCategory = SupportDataMethod.FetchSubCategory().DataSource.Table;
            List<Sup_SubCategory> liSubCategory = new List<Sup_SubCategory>();
            if (dtFetchSubCategory != null && dtFetchSubCategory.Rows.Count > 0)
            {
                liSubCategory = (from DataRow dr in dtFetchSubCategory.Rows
                                 select new Sup_SubCategory()
                                 {
                                     STF_CATEGORY_ID = dr[Common.sup_staff_subcategory.STF_CATEGORY_ID].ToString(),
                                     STF_CATEGORY = dr[Common.sup_staff_subcategory.STF_CATEGORY].ToString()
                                 }).ToList();
                objStaffRolesInfo.SUBCATEGORY = new SelectList(liSubCategory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
            }

            // Non Teaching Category ..
            DataTable dtFetchNonTeachingCategory = new DataTable();
            dtFetchNonTeachingCategory = SupportDataMethod.FetchNonTeachingCategory().DataSource.Table;
            List<SUP_NON_TEACHING_CATEGORY> liNonTeachingCategory = new List<SUP_NON_TEACHING_CATEGORY>();
            if (dtFetchNonTeachingCategory != null && dtFetchNonTeachingCategory.Rows.Count > 0)
            {
                liNonTeachingCategory = (from DataRow dr in dtFetchNonTeachingCategory.Rows
                                         select new SUP_NON_TEACHING_CATEGORY
                                         {
                                             NON_TEACHING_CATEGORY_ID = dr[Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_ID].ToString(),
                                             NON_TEACHING_CATEGORY_NAME = dr[Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_NAME].ToString()
                                         }).ToList();
                objStaffRolesInfo.NONTEACHINGCATEGORY = new SelectList(liNonTeachingCategory, Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_ID, Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_NAME);
            }
            // Board Member ..
            DataTable dtBoardMember = new DataTable();
            dtBoardMember = SupportDataMethod.FetchBoardMmber().DataSource.Table;
            List<SUP_BOARD_MEMBER> liBoardMember = new List<SUP_BOARD_MEMBER>();
            if (dtBoardMember != null && dtBoardMember.Rows.Count > 0)
            {
                liBoardMember = (from DataRow dr in dtBoardMember.Rows
                                 select new SUP_BOARD_MEMBER()
                                 {
                                     BOARD_MEMBER_ID = dr[Common.SUP_BOARD_MEMBER.BOARD_MEMBER_ID].ToString(),
                                     BOARD_MEMBER_NAME = dr[Common.SUP_BOARD_MEMBER.BOARD_MEMBER_NAME].ToString(),
                                 }).ToList();
                objStaffRolesInfo.BOARDMEMBER = new SelectList(liBoardMember, Common.SUP_BOARD_MEMBER.BOARD_MEMBER_ID, Common.SUP_BOARD_MEMBER.BOARD_MEMBER_NAME);
            }
            return View(objStaffRolesInfo);
        }

        //Insert Staff Roles Details ...
        public string SubjectDetails(string JsonRoles)
        {
            var sStaffID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            if (JsonRoles != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffRoles>(JsonRoles);

                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.BOARD_MEMBER, Result.BOARDMEMBER, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DESIGNATION, Result.DESIGNATION, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.KNOWLEDGE_IN_COMPUTER, Result.KNOWLEDGEINCOMPUTER, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.MAIN_SUBJECT, Result.MAINSUBJECT, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.NON_TEACHING_CATEGORY, Result.NONTEACHINGCATEGORY, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.QUALIFICATION, Result.QUALIFICATION, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.SESSIONS, Result.SESSIONS, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.SHIFT, Result.SHIFT, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.SUB_CATEGORY, Result.SUBCATEGORY, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_CODE, Result.StaffCode, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.UpdateStaffSubjectDetails(dv);
                    if (resultArgs.Success)
                    {
                        sResult = "Record saved successfully ...!";
                    }
                    else
                    {
                        sResult = "Record not saved successfully ...!";
                    }
                }
            }

            return sResult;
        }

        // Fetch Staff Category ..
        public string FetchStaffCategory(string sCategory)
        {
            string sOption = string.Empty;
            var ObjCategory = new CATEGORY();
            if (sCategory != "1")
            {
                ObjCategory.liNonTeachingCategory = (List<SUP_NON_TEACHING_CATEGORY>)CMSHandler.FetchData<SUP_NON_TEACHING_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchNonTeachingCategory)).DataSource.Data;
                foreach (var item in ObjCategory.liNonTeachingCategory)
                {
                    sOption += "<option value='" + item.NON_TEACHING_CATEGORY_ID + "'>" + item.NON_TEACHING_CATEGORY_NAME + "</option>";
                }
            }
            else
            {
                ObjCategory.liTeachingCategory = (List<SUP_TEACHING_CATEGORY>)CMSHandler.FetchData<SUP_TEACHING_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTeachingCategory)).DataSource.Data;
                foreach (var item in ObjCategory.liTeachingCategory)
                {
                    sOption += "<option value='" + item.TEACHING_CATEGORY_ID + "'>" + item.TEACHING_CATEGORY + "</option>";
                }
            }
            return sOption;
        }

        // Fetch Staff Roles ..
        public JsonResult FetchSubjectDetailsData(string sStaffID)
        {
            dv.Clear();
            dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchSubjectDetails = new DataTable();
            using (StaffViewModel objStaffViewModel = new StaffViewModel())
            {
                dtFetchSubjectDetails = objStaffViewModel.FetchStaffRolesById(dv).DataSource.Table;
            }
            if (dtFetchSubjectDetails != null && dtFetchSubjectDetails.Rows.Count > 0)
            {
                //StaffInfo objStaffInfo = new StaffInfo()
                //{
                //    BOARDMEMBER = dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.BOARD_MEMBER].ToString(),
                //    StaffCode = dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.STAFF_CODE].ToString(),
                //    KNOWLEDGEINCOMPUTER = dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.KNOWLEDGE_IN_COMPUTER].ToString(),
                //    NONTEACHINGCATEGORY = dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.NON_TEACHING_CATEGORY].ToString(),
                //    SESSIONS = dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.SESSIONS].ToString(),
                //    MAINSUBJECT = dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.MAIN_SUBJECT].ToString(),
                //    sShift=dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.SHIFT].ToString(),
                //    //sNonTeachingCategory=dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.NON_TEACHING_CATEGORY].ToString(),
                //    sDesignation=dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.DESIGNATION].ToString(),
                //    sQualification=dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.QUALIFICATION].ToString(),
                //    sSubCategory=dtFetchSubjectDetails.Rows[0][Common.STF_PERSONAL_INFO.SUB_CATEGORY].ToString()
                //};
                return Json(sResult);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }

        }

        // Load Dropdown List ...
        public string LoadStaffRolesDropdown()
        {
            string sShift = string.Empty;
            string sQualification = string.Empty;
            string sDesignation = string.Empty;
            string sSubcategory = string.Empty;
            string sNonteachingCategory = string.Empty;
            string JsonData = string.Empty;
            // Non Teaching Category ..
            DataTable dtFetchNonTeachingCategory = new DataTable();
            dtFetchNonTeachingCategory = SupportDataMethod.FetchNonTeachingCategory().DataSource.Table;
            if (dtFetchNonTeachingCategory != null && dtFetchNonTeachingCategory.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchNonTeachingCategory.Rows)
                {
                    sNonteachingCategory += "<option value='" + item[Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_ID].ToString() + "'>" + item[Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_NAME].ToString() + "</option>";
                }
            }
            // Shift ...
            DataTable dtFetchShift = new DataTable();
            dtFetchShift = SupportDataMethod.FetchShift().DataSource.Table;
            if (dtFetchShift != null && dtFetchShift.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchShift.Rows)
                {
                    sShift += "<option value='" + item[Common.SUP_SHIFT.SHIFT_ID].ToString() + "'>" + item[Common.SUP_SHIFT.SHIFT_NAME].ToString() + "</option>";
                }
            }

            // Qualification ...
            DataTable dtFetchQualification = new DataTable();
            dtFetchQualification = SupportDataMethod.FetchQualification().DataSource.Table;
            if (dtFetchQualification != null && dtFetchQualification.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchQualification.Rows)
                {
                    sQualification += "<option value='" + item[Common.Sup_Qualification.QUALIFICATION_ID].ToString() + "'>" + item[Common.Sup_Qualification.QUALIFICATION].ToString() + "</option>";
                }
            }
            // Designation ...
            DataTable dtFetchDesignation = new DataTable();
            dtFetchDesignation = SupportDataMethod.FetchDesignation().DataSource.Table;
            if (dtFetchDesignation != null && dtFetchDesignation.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchDesignation.Rows)
                {
                    sDesignation += "<option value='" + item[Common.SUP_DESIGNATION.DESIGNATIONID].ToString() + "'>" + item[Common.SUP_DESIGNATION.DESIGNATION].ToString() + "</option>";
                }
            }
            // Sub_Category ...
            DataTable dtFetchSubCategory = new DataTable();
            dtFetchSubCategory = SupportDataMethod.FetchSubCategory().DataSource.Table;
            if (dtFetchSubCategory != null && dtFetchSubCategory.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchSubCategory.Rows)
                {
                    sSubcategory += "<option value='" + item[Common.sup_staff_subcategory.STF_CATEGORY_ID].ToString() + "'>" + item[Common.sup_staff_subcategory.STF_CATEGORY].ToString() + "</option>";
                }
            }
            var objJson = new DDLForStaffRolesInfo() { Qualification = sQualification, Shift = sShift, SubCategory = sSubcategory, Designation = sDesignation, NonTeachingCategory = sNonteachingCategory };
            JsonData = JsonConvert.SerializeObject(objJson);
            return JsonData;
        }

        // Staff Roles Information ...
        public ActionResult EditStaffRolesInfo()
        {
            StaffRolesInfo objStaffRolesInfo = new StaffRolesInfo();

            // Shift ...
            DataTable dtFetchShift = new DataTable();
            dtFetchShift = SupportDataMethod.FetchShift().DataSource.Table;
            List<Sup_Shift> liShift = new List<Sup_Shift>();
            if (dtFetchShift != null && dtFetchShift.Rows.Count > 0)
            {
                liShift = (from DataRow dr in dtFetchShift.Rows
                           select new Sup_Shift()
                           {
                               SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                               SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                           }).ToList();
                objStaffRolesInfo.SHIFT = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
            }

            // Qualification ...
            DataTable dtFetchQualification = new DataTable();
            dtFetchQualification = SupportDataMethod.FetchQualification().DataSource.Table;
            List<Sup_Qualification> liQualification = new List<Sup_Qualification>();
            if (dtFetchQualification != null && dtFetchQualification.Rows.Count > 0)
            {
                liQualification = (from DataRow dr in dtFetchQualification.Rows
                                   select new Sup_Qualification()
                                   {
                                       QUALIFICATION_ID = dr[Common.Sup_Qualification.QUALIFICATION_ID].ToString(),
                                       QUALIFICATION = dr[Common.Sup_Qualification.QUALIFICATION].ToString()
                                   }).ToList();
                objStaffRolesInfo.QUALIFICATION = new SelectList(liQualification, Common.Sup_Qualification.QUALIFICATION_ID, Common.Sup_Qualification.QUALIFICATION);
            }

            // Designation ...
            DataTable dtFetchDesignation = new DataTable();
            dtFetchDesignation = SupportDataMethod.FetchDesignation().DataSource.Table;
            List<Sup_Designation> liDesignation = new List<Sup_Designation>();
            if (dtFetchDesignation != null && dtFetchDesignation.Rows.Count > 0)
            {
                liDesignation = (from DataRow dr in dtFetchDesignation.Rows
                                 select new Sup_Designation()
                                 {
                                     DESIGNATIONID = dr[Common.sup_designation.DESIGNATIONID].ToString(),
                                     DESIGNATION = dr[Common.sup_designation.DESIGNATION].ToString()
                                 }).ToList();
                objStaffRolesInfo.DESIGNATION = new SelectList(liDesignation, Common.sup_designation.DESIGNATIONID, Common.sup_designation.DESIGNATION);
            }

            // Sub_Category ...
            DataTable dtFetchSubCategory = new DataTable();
            dtFetchSubCategory = SupportDataMethod.FetchSubCategory().DataSource.Table;
            List<Sup_SubCategory> liSubCategory = new List<Sup_SubCategory>();
            if (dtFetchSubCategory != null && dtFetchSubCategory.Rows.Count > 0)
            {
                liSubCategory = (from DataRow dr in dtFetchSubCategory.Rows
                                 select new Sup_SubCategory()
                                 {
                                     STF_CATEGORY_ID = dr[Common.sup_staff_subcategory.STF_CATEGORY_ID].ToString(),
                                     STF_CATEGORY = dr[Common.sup_staff_subcategory.STF_CATEGORY].ToString()
                                 }).ToList();
                objStaffRolesInfo.SUBCATEGORY = new SelectList(liSubCategory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
            }
            // Non Teaching Category ..
            DataTable dtFetchNonTeachingCategory = new DataTable();
            dtFetchNonTeachingCategory = SupportDataMethod.FetchNonTeachingCategory().DataSource.Table;
            List<SUP_NON_TEACHING_CATEGORY> liNonTeachingCategory = new List<SUP_NON_TEACHING_CATEGORY>();
            if (dtFetchNonTeachingCategory != null && dtFetchNonTeachingCategory.Rows.Count > 0)
            {
                liNonTeachingCategory = (from DataRow dr in dtFetchNonTeachingCategory.Rows
                                         select new SUP_NON_TEACHING_CATEGORY
                                         {
                                             NON_TEACHING_CATEGORY_ID = dr[Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_ID].ToString(),
                                             NON_TEACHING_CATEGORY_NAME = dr[Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_NAME].ToString()
                                         }).ToList();
                objStaffRolesInfo.NONTEACHINGCATEGORY = new SelectList(liNonTeachingCategory, Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_ID, Common.SUP_NON_TEACHING_CATEGORY.NON_TEACHING_CATEGORY_NAME);
            }
            // Board Member ..
            DataTable dtBoardMember = new DataTable();
            dtBoardMember = SupportDataMethod.FetchBoardMmber().DataSource.Table;
            List<SUP_BOARD_MEMBER> liBoardMember = new List<SUP_BOARD_MEMBER>();
            if (dtBoardMember != null && dtBoardMember.Rows.Count > 0)
            {
                liBoardMember = (from DataRow dr in dtBoardMember.Rows
                                 select new SUP_BOARD_MEMBER()
                                 {
                                     BOARD_MEMBER_ID = dr[Common.SUP_BOARD_MEMBER.BOARD_MEMBER_ID].ToString(),
                                     BOARD_MEMBER_NAME = dr[Common.SUP_BOARD_MEMBER.BOARD_MEMBER_NAME].ToString(),
                                 }).ToList();
                objStaffRolesInfo.BOARDMEMBER = new SelectList(liBoardMember, Common.SUP_BOARD_MEMBER.BOARD_MEMBER_ID, Common.SUP_BOARD_MEMBER.BOARD_MEMBER_NAME);
            }
            return View(objStaffRolesInfo);

        }

        //Update Subject Details ...
        public JsonResult UpdateSubjectDetails(string str)
        {
            var sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            if (str != null)
            {
                var Result = JsonConvert.DeserializeObject<StaffRoles>(str);

                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.BOARD_MEMBER, Result.BOARDMEMBER, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.DESIGNATION, Result.DESIGNATION, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.KNOWLEDGE_IN_COMPUTER, Result.KNOWLEDGEINCOMPUTER, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.MAIN_SUBJECT, Result.MAINSUBJECT, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.NON_TEACHING_CATEGORY, Result.NONTEACHINGCATEGORY, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.QUALIFICATION, Result.QUALIFICATION, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.SESSIONS, Result.SESSIONS, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.SHIFT, Result.SHIFT, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.SUB_CATEGORY, Result.SUBCATEGORY, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_CODE, Result.StaffCode, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.UpdateStaffSubjectDetails(dv);
                    if (resultArgs.Success)
                    {
                        sResult = "Record updated successfully ...!";
                    }
                    else
                    {
                        sResult = "Record not updated try again ...!";
                    }
                }
            }

            return Json(sResult);
        }
        #endregion

        #region Staff Family Infomation
        // Staff Family Information ...
        public ActionResult StaffFamilyInfo()
        {
            string sStaffID = string.Empty;
            sStaffID = (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null) ? Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_FAMILY.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchFamilyById = new DataTable();

            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchFamilyById = objViewModel.FetchFamilyById(dv).DataSource.Table;
            }
            if (dtFetchFamilyById != null && dtFetchFamilyById.Rows.Count > 0)
            {
                IList<Family> liFamily = new List<Family>();
                liFamily = (from DataRow dr in dtFetchFamilyById.Rows
                            select new Family
                            {
                                FamilyId = dr[Common.STF_FAMILY.FAMILY_NO].ToString(),
                                Name = dr[Common.STF_FAMILY.NAME].ToString(),
                                Relation = dr[Common.STF_FAMILY.RELATION].ToString(),
                                DateOfBirth = dr[Common.STF_FAMILY.DATE_OF_BIRTH].ToString()
                            }).ToList();
                return View(liFamily);
            }
            else
            {
                return View();
            }
        }

        //Insert Staff Family ..
        public JsonResult AddFamily(string JsonFamily)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }

            if (JsonFamily != null)
            {
                var Result = JsonConvert.DeserializeObject<Family>(JsonFamily);
                string DateOfBirth = CommonMethods.ConvertdatetoyearFromat(Result.DateOfBirth);

                dv.Clear();
                dv.Add(Common.STF_FAMILY.NAME, Result.Name, EnumCommand.DataType.String);
                dv.Add(Common.STF_FAMILY.RELATION, Result.Relation, EnumCommand.DataType.String);
                dv.Add(Common.STF_FAMILY.DATE_OF_BIRTH, DateOfBirth, EnumCommand.DataType.String);
                dv.Add(Common.STF_FAMILY.STAFF_NO, sStaffID, EnumCommand.DataType.String);
                using (StaffViewModel objStaff = new StaffViewModel())
                {
                    resultArgs = objStaff.InsertStaffFamily(dv);
                    if (resultArgs.Success)
                    {
                        sResult = "Record saved successfully ...!";
                    }
                    else
                    {
                        sResult = "Record not saved successfully ...!";
                    }
                }
            }
            return Json(sResult);
        }

        // Fetch Staff Family ...
        public JsonResult FetchFamily(string sStaffID)
        {
            dv.Clear();
            dv.Add(Common.STF_FAMILY.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchFamily = new DataTable();
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                dtFetchFamily = objStaff.FetchFamilyById(dv).DataSource.Table;
            }
            if (dtFetchFamily != null && dtFetchFamily.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchFamily.Rows[0][Common.STF_FAMILY.FAMILY_NO].ToString();
                Family objStaffInfo = new Family()
                {
                    FamilyId = dtFetchFamily.Rows[0][Common.STF_FAMILY.FAMILY_NO].ToString(),
                    Name = dtFetchFamily.Rows[0][Common.STF_FAMILY.NAME].ToString(),
                    Relation = dtFetchFamily.Rows[0][Common.STF_FAMILY.RELATION].ToString(),
                    DateOfBirth = dtFetchFamily.Rows[0][Common.STF_FAMILY.DATE_OF_BIRTH].ToString()
                };
                return Json(objStaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }

        }

        // Bind values To Controles...
        public JsonResult BindFamily(string sFamilyID)
        {
            dv.Clear();
            dv.Add(Common.STF_FAMILY.FAMILY_NO, sFamilyID, EnumCommand.DataType.String);
            DataTable dtFetchFamily = new DataTable();
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchFamily = objViewModel.BindFamily(dv).DataSource.Table;
            }
            if (dtFetchFamily != null && dtFetchFamily.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchFamily.Rows[0][Common.STF_FAMILY.FAMILY_NO].ToString();
                Family objFamily = new Family()
                {
                    DateOfBirth = dtFetchFamily.Rows[0][Common.STF_FAMILY.DATE_OF_BIRTH].ToString(),
                    Name = dtFetchFamily.Rows[0][Common.STF_FAMILY.NAME].ToString(),
                    Relation = dtFetchFamily.Rows[0][Common.STF_FAMILY.RELATION].ToString()
                };
                return Json(objFamily);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }

        // Bind DropDown List ..
        public string DDLFamily()
        {
            string sOption = string.Empty;
            // Relation ...
            DataTable dtFetchRelation = new DataTable();
            dtFetchRelation = SupportDataMethod.FetchRelaion().DataSource.Table;
            if (dtFetchRelation != null && dtFetchRelation.Rows.Count > 0)
            {
                foreach (DataRow item in dtFetchRelation.Rows)
                {
                    sOption += "<option value='" + item[Common.SUP_FAMILY.RELATION_ID].ToString() + "'>" + item[Common.SUP_FAMILY.RELATION].ToString() + "</option>";
                }
            }
            return sOption;
        }

        // Staff Family Information ...
        public ActionResult EditStaffFamilyInfo()
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }

            dv.Clear();
            dv.Add(Common.STF_FAMILY.STAFF_NO, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchFamilyById = new DataTable();

            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                dtFetchFamilyById = objViewModel.FetchFamilyById(dv).DataSource.Table;
            }
            if (dtFetchFamilyById != null && dtFetchFamilyById.Rows.Count > 0)
            {
                IList<Family> liFamily = new List<Family>();
                liFamily = (from DataRow dr in dtFetchFamilyById.Rows
                            select new Family
                            {
                                FamilyId = dr[Common.STF_FAMILY.FAMILY_NO].ToString(),
                                Name = dr[Common.STF_FAMILY.NAME].ToString(),
                                Relation = dr[Common.STF_FAMILY.RELATION].ToString(),
                                DateOfBirth = dr[Common.STF_FAMILY.DATE_OF_BIRTH].ToString()
                            }).ToList();
                return View(liFamily);
            }
            else
            {
                return View();
            }
        }

        // Update Family ...
        public JsonResult UpdateFamily(string JsonFamily)
        {
            string sStaffID = string.Empty;
            if (Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] != null)
            {
                sStaffID = Session[Common.SESSION_VARIABLES.LAST_INSERT_ID].ToString();
            }
            else
            {
                sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            }


            if (JsonFamily != null)
            {
                var Result = JsonConvert.DeserializeObject<Family>(JsonFamily);
                string DateOfBirth = CommonMethods.ConvertdatetoyearFromat(Result.DateOfBirth);
                dv.Clear();
                dv.Add(Common.STF_FAMILY.NAME, Result.Name, EnumCommand.DataType.String);
                dv.Add(Common.STF_FAMILY.RELATION, Result.Relation, EnumCommand.DataType.String);
                dv.Add(Common.STF_FAMILY.DATE_OF_BIRTH, DateOfBirth, EnumCommand.DataType.String);
                dv.Add(Common.STF_FAMILY.STAFF_NO, sStaffID, EnumCommand.DataType.String);

                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        var PrimaryID = Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID].ToString();
                        dv.Add(Common.STF_FAMILY.FAMILY_NO, PrimaryID, EnumCommand.DataType.String);
                        resultArgs = objStaff.UpdateStaffFamily(dv);
                        if (resultArgs.Success)
                        {
                            sResult = "Record updated successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }
                    }
                }
                else
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        resultArgs = objStaff.InsertStaffFamily(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }
            }
            return Json(sResult);
        }

        // Delete Staff Family ..
        public string DeleteFamily(string sFamilyId)
        {
            dv.Clear();
            dv.Add(Common.STF_FAMILY.FAMILY_NO, sFamilyId, EnumCommand.DataType.String);
            dv.Add(Common.STF_FAMILY.ISDELETED, "1", EnumCommand.DataType.String);
            using (StaffViewModel objViewModel = new StaffViewModel())
            {
                resultArgs = objViewModel.DeleteFamily(dv);
                if (resultArgs.Success)
                {
                    sResult = "Record deleted successfully ...!";
                }
                else
                {
                    sResult = "Record not deleted try again...!";
                }
            }
            return sResult;
        }
        #endregion

        #region List Staff Details
        //Staff List ...
        //[UserRights("STAFF")]
        public ActionResult ListStaffDetails()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            try
            {

                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] = null;
                    DataTable dtStaffList = new DataTable();

                    dtStaffList = objViewModel.FetchStaffInfo(sAcademicYear).DataSource.Table;

                    var liList = new List<StaffList>();
                    if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                    {
                        objViewModel.lstStaffList = (from DataRow dr in dtStaffList.Rows
                                                     select new StaffList
                                                     {
                                                         STAFF_ID = dr[Common.STF_PERSONAL_INFO.STAFF_ID].ToString(),
                                                         STAFF_CODE = dr[Common.STF_PERSONAL_INFO.STAFF_CODE].ToString(),
                                                         FIRST_NAME = dr[Common.STF_PERSONAL_INFO.FIRST_NAME].ToString(),
                                                         GENDER_NAME = dr[Common.SUP_GENDER.GENDER_NAME].ToString(),
                                                         DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                         SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString(),
                                                         DATE_OF_BIRTH = dr[Common.STF_PERSONAL_INFO.DATE_OF_BIRTH].ToString(),
                                                         IS_LEFT = dr[Common.STF_PERSONAL_INFO.IS_LEFT].ToString(),

                                                     }).ToList();
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog obj = new ErrorLog())
                {
                    obj.WriteError("StaffController", "ListStaffDetails", ex.Message);
                }
            }

            return View(objViewModel);
        }


        public ActionResult StaffList()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] = null;
                DataTable dtStaffList = new DataTable();

                dtStaffList = objViewModel.FetchStaffInfo(sAcademicYear).DataSource.Table;

                var liList = new List<StaffList>();
                if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                {
                    objViewModel.lstStaffList = (from DataRow dr in dtStaffList.Rows
                                                 select new StaffList
                                                 {
                                                     STAFF_ID = dr[Common.STF_PERSONAL_INFO.STAFF_ID].ToString(),
                                                     STAFF_CODE = dr[Common.STF_PERSONAL_INFO.STAFF_CODE].ToString(),
                                                     FIRST_NAME = dr[Common.STF_PERSONAL_INFO.FIRST_NAME].ToString(),
                                                     GENDER_NAME = dr[Common.SUP_GENDER.GENDER_NAME].ToString(),
                                                     DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                     SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString(),
                                                     DATE_OF_BIRTH = dr[Common.STF_PERSONAL_INFO.DATE_OF_BIRTH].ToString(),
                                                     IS_LEFT = dr[Common.STF_PERSONAL_INFO.IS_LEFT].ToString()
                                                 }).ToList();
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

            return View(objViewModel);
        }
        #endregion

        #region Delete Staff Details 
        // Delete Staff Details ...
        public string DeleteStaff(string sStaffId, string UserName)
        {
            dv.Clear();
            dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffId, EnumCommand.DataType.String);
            dv.Add(Common.STF_PERSONAL_INFO.IS_DELETED, "1", EnumCommand.DataType.String);
            var ObjUserAccount = new USER_ACCOUNT_INFO();
            var liUserAccount = new List<USER_ACCOUNT_INFO>();
            ObjUserAccount.USERNAME = UserName;
            liUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(ObjUserAccount, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserAccountByName)).DataSource.Data;
            if (liUserAccount != null && liUserAccount.Count > 0)
            {
                ObjUserAccount.USER_ACCOUNT_ID = liUserAccount.FirstOrDefault().USER_ACCOUNT_ID;
            }
            resultArgs = CMSHandler.SaveOrUpdate(ObjUserAccount, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.DeleteUserAccount));
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                resultArgs = objStaff.DeleteStaffInfo(dv);
                if (resultArgs.Success)
                {
                    sResult = "Record deleted successfully ...!";
                }
                else
                {
                    sResult = "Record deleted successfully ..!";
                }
            }
            return sResult;
        }
        #endregion

        #region RDLC Report
        public ActionResult TransferCertificate()
        {
            return View();
        }
        #endregion

        #region Edit Staff Profile
        public ActionResult EditStaffProfile()
        {
            //Session[Common.SESSION_VARIABLES.USER_ID]=Id;
            return View();
        }

        #region Edit Staff Personal Profile
        //public ActionResult EditStaffPersonalProfile()
        //{
        //    //string sStaffID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
        //    dv.Clear();
        //    //dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
        //    //DataTable dtFetchRecordbyId = new DataTable();
        //    StaffViewModel objStaff = new StaffViewModel();
        //    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


        //    // Department ...
        //    var Department = new List<SupDepartment>();
        //    Department = (List<SupDepartment>)CMSHandler.FetchData<SupDepartment>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
        //    objStaff.DEPARTMENT = new SelectList(Department, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);

        //    // Community ...

        //    var Community = new List<SupCommunity>();
        //    Community = (List<SupCommunity>)CMSHandler.FetchData<SupCommunity>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCommunity)).DataSource.Data;
        //    objStaff.COMMUNITY = new SelectList(Community, Common.SUP_COMMUNITY.COMMUNITYID, Common.SUP_COMMUNITY.COMMUNITY);

        //    // Gender ...

        //    var Gender = new List<SupGender>();
        //    Gender = (List<SupGender>)CMSHandler.FetchData<SupGender>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchGender)).DataSource.Data;
        //    objStaff.GENDER = new SelectList(Gender, Common.SUP_GENDER.GENDER_ID, Common.SUP_GENDER.GENDER_NAME);

        //    // MaritalStatus ...

        //    var MaritalStatus = new List<SupMaritalStatus>();
        //    MaritalStatus = (List<SupMaritalStatus>)CMSHandler.FetchData<SupMaritalStatus>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMarritalStatus)).DataSource.Data;
        //    objStaff.MARITAL_STATUS = new SelectList(MaritalStatus, Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID, Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME);

        //    // MotherTongue ...

        //    var MotherTongue = new List<SupMotherTongue>();
        //    MotherTongue = (List<SupMotherTongue>)CMSHandler.FetchData<SupMotherTongue>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMotherTongue)).DataSource.Data;
        //    objStaff.MOTHER_TONGUE = new SelectList(MotherTongue, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME);

        //    // Nationality ...

        //    var Nationality = new List<SupNationality>();
        //    Nationality = (List<SupNationality>)CMSHandler.FetchData<SupNationality>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchNationality)).DataSource.Data;
        //    objStaff.NATIONALITY = new SelectList(Nationality, Common.SUP_NATIONALITY.NATIONALITY_ID, Common.SUP_NATIONALITY.NATIONALITY_NAME);

        //    // Religion ...

        //    var Religion = new List<SUP_RELIGION>();
        //    Religion = (List<SUP_RELIGION>)CMSHandler.FetchData<SUP_RELIGION>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReligion)).DataSource.Data;
        //    objStaff.RELIGION = new SelectList(Religion, Common.SUP_RELIGION.RELIGIONID, Common.SUP_RELIGION.RELIGION);

        //    // BloodGroup ...

        //    var BloodGroup = new List<SupBloodGroup>();
        //    BloodGroup = (List<SupBloodGroup>)CMSHandler.FetchData<SupBloodGroup>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBloodGroup)).DataSource.Data;
        //    objStaff.BLOOD_GROUP = new SelectList(BloodGroup, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME);

        //    return View(objStaff);
        //}
        public JsonResult FetchStaffPersonalProfile()
        {
            var StaffPersonalProfile = new List<STAFF_PERSONAL>();
            var StaffPersonal = new STAFF_PERSONAL();
            StaffPersonal.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            StaffPersonalProfile = (List<STAFF_PERSONAL>)CMSHandler.FetchData<STAFF_PERSONAL>(StaffPersonal, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchPersonalRecordByID)).DataSource.Data;
            return new JsonResult { Data = StaffPersonalProfile, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public string UpdateStaffPersonalProflie(string StaffPersonal)
        {
            var Result = JsonConvert.DeserializeObject<STAFF_PERSONAL>(StaffPersonal);
            Result.DATE_OF_BIRTH = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_BIRTH);
            Result.DATE_OF_JOIN = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_JOIN);
            var StaffPersonalProfile = new List<STAFF_PERSONAL>();
            var StaffId = new STAFF_PERSONAL();
            Result.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            var StaffPersonalResult = CMSHandler.SaveOrUpdate(Result, StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffPersonal), string.Empty, false).DataSource.Data;
            if (StaffPersonalResult != null)
            {
                sResult = Common.Messages.RecordsSavedSuccessfully;
            }
            else
            {
                sResult = Common.Messages.FailedToSaveRecords;
            }
            return sResult;
        }
        #endregion
        #region Edit Staff Address Profile
        //public ActionResult EditStaffAddressProfile()
        //{
        //    STAFF_ADDRESS objStaffAddressProfile = new STAFF_ADDRESS();
        //    StaffViewModel objStaff = new StaffViewModel();
        //    //C_Country ...
        //    var cCountry = new List<SUP_COUNTRY>();
        //    cCountry = (List<SUP_COUNTRY>)CMSHandler.FetchData<SUP_COUNTRY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCountry)).DataSource.Data;
        //    objStaff.CCOUNTRY = new SelectList(cCountry, Common.SUP_COUNTRY.COUNTRY_ID, Common.SUP_COUNTRY.COUNTRY_NAME);
        //    // P_Country
        //    var pCountry = new List<SUP_COUNTRY>();
        //    pCountry = (List<SUP_COUNTRY>)CMSHandler.FetchData<SUP_COUNTRY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCountry)).DataSource.Data;
        //    objStaff.PCOUNTRY = new SelectList(pCountry, Common.SUP_COUNTRY.COUNTRY_ID, Common.SUP_COUNTRY.COUNTRY_NAME);

        //    return View(objStaff);
        //}
        public JsonResult FetchStaffAddressProfile()
        {
            var StaffAddressProfile = new List<STAFF_ADDRESS>();
            var StaffAddress = new STAFF_ADDRESS();
            StaffAddress.STAFFNO = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            StaffAddressProfile = (List<STAFF_ADDRESS>)CMSHandler.FetchData<STAFF_ADDRESS>(StaffAddress, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchAddressById)).DataSource.Data;
            return new JsonResult { Data = StaffAddressProfile, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public string UpdateStaffAddressProflie(string StaffAddress)
        {
            var Result = JsonConvert.DeserializeObject<STAFF_ADDRESS>(StaffAddress);
            var StaffAddressProfile = new List<STAFF_ADDRESS>();
            var StaffId = new STAFF_ADDRESS();
            Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = Result.ADDRESS_NO;
            var StaffAddressResult = CMSHandler.SaveOrUpdate(Result, StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffAddress), string.Empty, false).DataSource.Data;
            if (StaffAddressResult != null)
            {
                sResult = Common.Messages.RecordsSavedSuccessfully;
            }
            else
            {
                sResult = Common.Messages.FailedToSaveRecords;
            }
            return sResult;
        }
        #endregion
        #region Edit Staff Services Profile
        public ActionResult EditStaffServicesProfile()
        {
            return View();
        }
        public JsonResult FetchStaffServiceProfile()
        {
            var StaffServicesProfile = new List<STAFF_SERVICES>();
            var StaffServices = new STAFF_SERVICES();
            StaffServices.STAFF_NO = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            StaffServicesProfile = (List<STAFF_SERVICES>)CMSHandler.FetchData<STAFF_SERVICES>(StaffServices, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchServiceById)).DataSource.Data;
            return new JsonResult { Data = StaffServicesProfile, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
        public ActionResult StaffProfileView()
        {
            StaffViewModel objViewModel = new StaffViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            var StaffProfile = new STAFFPROFILEVIEW();
            var StaffService = new STAFF_SERVICES();
            var StaffCounseling = new STAFF_COUNSELING();
            StaffProfile.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            StaffService.STAFF_NO = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            StaffCounseling.STAFF = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            var StaffProfileView = (List<STAFFPROFILEVIEW>)CMSHandler.FetchData<STAFFPROFILEVIEW>(StaffProfile, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffProfileView), sAcademicYear).DataSource.Data;
            var GetStaffServices = (List<STAFF_SERVICES>)CMSHandler.FetchData<STAFF_SERVICES>(StaffService, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchServiceById)).DataSource.Data;
            var GetStaffPaper = (List<STAFF_PAPER>)CMSHandler.FetchData<STAFF_PAPER>(StaffService, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchPaperById)).DataSource.Data;
            var GetStaffPublish = (List<STAFF_PUBLISH>)CMSHandler.FetchData<STAFF_PUBLISH>(StaffService, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchPublishById)).DataSource.Data;
            var GetStaffTraining = (List<STAFF_TRAINING>)CMSHandler.FetchData<STAFF_TRAINING>(StaffService, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchTrainingById)).DataSource.Data;
            var GetStaffCounseling = (List<STAFF_COUNSELING>)CMSHandler.FetchData<STAFF_COUNSELING>(StaffCounseling, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchCounselingById)).DataSource.Data;
            objViewModel.lstStaffServicesProfile = GetStaffServices;
            objViewModel.lstStaffPaper = GetStaffPaper;
            objViewModel.lstStaffPublish = GetStaffPublish;
            objViewModel.lstStaffTraining = GetStaffTraining;
            objViewModel.lstStaffCounseling = GetStaffCounseling;
            objViewModel.StaffProfileView = (StaffProfileView != null && StaffProfileView.Count > 0) ? StaffProfileView[0] : null;
            objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
            return View(objViewModel);
        }
        #endregion

        #region Class Wise Stduent For Class Staff
        public ActionResult ListClassWiseStudent()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //var liClsStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new stf_Personal_Info() { STAFF_ID=sStaffId},StaffSQL.GetStaffSQL(StaffSQLCommands.FetchClassWiseStudentForStaff),sAcademicYear).DataSource.Data;
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new stf_Personal_Info() { STAFF_ID = sStaffId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassesByStaffId), sAcademicYear).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjViewModel.liLCasslist = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "ListClassWiseStudent", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "ListClassWiseStudent", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Fetch Student By ClassId  ....
        public ActionResult FetchStudentByClassId(string sClassId)
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                    string sSQL = string.Empty;
                    sSQL = StaffSQL.GetStaffSQL(StaffSQLCommands.FetchClassWiseStudentForStaff).Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_STAFF_2017.SEMESTER_ID, sActiveSemester);
                    ObjViewModel.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new CourseWiseStaffList() { STAFF_ID = sStaffId, CLASS_ID = sClassId }, sSQL, sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "FetchStudentByClassId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "FetchStudentByClassId", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(ObjViewModel);
        }

        #endregion

        #region Student Course Remark
        // List Student Course Remark ....
        public ActionResult ListStuCourseRemark()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    var ClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        ObjViewModel.liLCasslist = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult FetchStuCourseRemarkByCourseId(string sCourseId, string sClassId)
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    objModel.COURSE_ID = sCourseId;
                    objModel.CLASS_ID = sClassId;
                    ObjViewModel.liStu_Course_Remark = (List<STU_COURSE_REMARK>)CMSHandler.FetchData<STU_COURSE_REMARK>(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.ListStudentCourseRemarkList), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "ListStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult AddStuCourseRemark()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    //var Component = (List<CIA_COMPONENT>)CMSHandler.FetchData<CIA_COMPONENT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCiaComponent), sAcademicYear).DataSource.Data;
                    var ClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        ObjViewModel.liLCasslist = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    //if (Component != null && Component.Count > 0)
                    //{
                    //    ObjViewModel.Component = new SelectList(Component, Common.SUP_CIA_COMPONENTS.COMPONENT_ID, Common.SUP_CIA_COMPONENTS.COMPONENT);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "AddStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "AddStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchSupCIAComponentbyCourseId(string sCourseId)
        {
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var FetchProgramme = (List<CIA_COMPONENT>)CMSHandler.FetchData<CIA_COMPONENT>(new CIA_TOTAL { COURSE_ID = sCourseId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupCIAComponentbyCourseId)).DataSource.Data;
                    if (FetchProgramme != null && FetchProgramme.Count > 0)
                    {
                        foreach (var item in FetchProgramme)
                        {
                            sOption += "<option value='" + item.COMPONENT_ID + "' >" + item.COMPONENT + "</option>";
                        }
                    }
                }
                else
                    return Json(sOption);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchSupCIAComponentbyCourseId", "Staff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchSupCIAComponentbyCourseId", "Staff", ex.Message);
                }
            }
            return Json(sOption);
        }

        public ActionResult BindStudentForStuCourseRemark(string sCourseId, string sClassId, string sComponentId)
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    objModel.COURSE_ID = sCourseId;
                    objModel.CLASS_ID = sClassId;
                    objModel.COMPONENT_ID = sComponentId;
                    ObjViewModel.liCIA_Staff_Info = (List<CIA_STAFF_INFO>)CMSHandler.FetchData<CIA_STAFF_INFO>(objModel, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseAndClassInfoByClassAndCourseId), sAcademicYear).DataSource.Data;
                    ObjViewModel.liStu_Course_Remark = (List<STU_COURSE_REMARK>)CMSHandler.FetchData<STU_COURSE_REMARK>(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStudentByCourseIdClassId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "BindStudentForStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "BindStudentForStuCourseRemark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveStuCourseRemark(string JsonAddStuCourseRemark)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    STU_COURSE_REMARK objModel = new STU_COURSE_REMARK();
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var objJson = serializer.Deserialize<JSONStaffModel>(JsonAddStuCourseRemark);
                    if (objJson.LiJsonStuCourseRemark != null && objJson.LiJsonStuCourseRemark.Count > 0)
                    {
                        foreach (var item in objJson.LiJsonStuCourseRemark)
                        {
                            objModel.COURSE_ID = item.COURSE_ID;
                            objModel.CLASS_ID = item.CLASS_ID;
                            objModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                            objModel.STUDENT_ID = item.STUDENT_ID;
                            objModel.ACADEMIC_YEAR = sAcademicYear;
                            objModel.COURSE_ID = item.COURSE_ID;
                            objModel.REMARK_ID = item.REMARK_ID;
                            objModel.REMARK = item.REMARK;
                            objModel.COURSE_GROUP_ID = item.COURSE_GROUP_ID;
                            if (string.IsNullOrEmpty(objModel.REMARK_ID))
                            {
                                if (string.IsNullOrEmpty(item.REMARK))
                                    continue;
                                resultArgs = CMSHandler.SaveOrUpdate(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.InsertStuCourseRemark), sAcademicYear);
                            }
                            else
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStuCourseRemark), sAcademicYear);
                            }
                        }
                        ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("STAFF", "SaveStuCourseRemark", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("STAFF", "SaveStuCourseRemark", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Staff Designation

        public ActionResult StaffDesignation()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var AcYearList = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var Shift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (AcYearList != null && AcYearList.Count > 0)
                    {
                        ObjViewModel.AcademicYear = new SelectList(AcYearList, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    if (Shift != null && Shift.Count > 0)
                    {
                        ObjViewModel.ShiftList = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "StaffDesignation", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "StaffDesignation", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult FetchStaffDesignationByAcademicYear(string AcademicYear, string sShift)
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            STF_DESIGNATION objModel = new STF_DESIGNATION();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objModel.ACADEMIC_YEAR = AcademicYear;
                    objModel.SHIFT = sShift;
                    ObjViewModel.liStfDesignation = (List<STF_DESIGNATION>)CMSHandler.FetchData<STF_DESIGNATION>(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffDesignationByAcademicYear), sAcademicYear).DataSource.Data;
                    ObjViewModel.liDesignation = (List<Sup_Designation>)CMSHandler.FetchData<Sup_Designation>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDesignation), sAcademicYear).DataSource.Data;
                    //ObjViewModel.liInchargeStaff = (List<STF_DESIGNATION>)CMSHandler.FetchData<STF_DESIGNATION>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "FetchStaffDesignationByAcademicYear", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "FetchStaffDesignationByAcademicYear", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveStaffDesignation(string JsonStaffDesignation)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    STF_DESIGNATION objModel = new STF_DESIGNATION();
                    MySQLHandler ObjHandler = new MySQLHandler();
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var objJson = serializer.Deserialize<JSONStaffModel>(JsonStaffDesignation);
                    if (objJson.LiJsonStaffDesignation != null && objJson.LiJsonStaffDesignation.Count > 0)
                    {
                        foreach (var item in objJson.LiJsonStaffDesignation)
                        {
                            ObjHandler.ExecuteAsScripts(" SET SQL_SAFE_UPDATES=0;");
                            resultArgs = CMSHandler.SaveOrUpdate(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateIsDeleteForStaffInStaffIncharge));
                            ObjHandler.ExecuteAsScripts(" SET SQL_SAFE_UPDATES=1;");
                            string sDesgination = !string.IsNullOrEmpty(item.DESIGNATION) ? item.DESIGNATION : "0";
                            var Selcourses = sDesgination.Split(',').ToList();
                            foreach (var items in Selcourses)
                            {
                                objModel.STAFF_ID = item.STAFF_ID;
                                objModel.DESIGNATION = items;
                                objModel.INCHARGE = "0";
                                objModel.ACADEMIC_YEAR = item.ACADEMIC_YEAR;
                                var Designationlist = (List<STF_DESIGNATION>)CMSHandler.FetchData<STF_DESIGNATION>(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffInchargeByDesignation)).DataSource.Data;
                                if (Designationlist != null && Designationlist.Count > 0)
                                {
                                    objModel.DESIGNATION_ID = Designationlist.FirstOrDefault().DESIGNATION_ID;
                                    resultArgs = CMSHandler.SaveOrUpdate(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffDesignation));
                                }
                                else
                                {
                                    if (item.DESIGNATION == "0")
                                        continue;
                                    resultArgs = CMSHandler.SaveOrUpdate(objModel, SQL.StaffSQL.StaffSQL.GetStaffSQL(StaffSQLCommands.InsertStaffDesignation));


                                }
                            }
                        }
                        ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HomeWorkController", "BulkAddHomeWork", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HomeWorkController", "BulkAddHomeWork", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Staff LeaveManagement
        //Staff Leave Apply
        /// <summary>
        /// This Action is used to show list leave applied by staff and Leave Transcation
        /// </summary>
        /// <returns></returns>
        public ActionResult ListStaffLeaveApply()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            string settingids = string.Empty;
            string sSQL = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch Leave Applied
                    objViewModel.lileaveapply = (List<LM_STAFF_LEAVE_DETAILS>)CMSHandler.FetchData<LM_STAFF_LEAVE_DETAILS>(new LM_STAFF_LEAVE_DETAILS() { STAFF_ID = sUserId },
                         StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffAppliedList), sAcademicYear).DataSource.Data;

                    //Fetch Leave type Category
                    var lilmleavetranscation = (List<LM_TRANSACTION_DETAILS>)CMSHandler.FetchData<LM_TRANSACTION_DETAILS>(new LM_TRANSACTION_DETAILS() { STAFF_ID = sUserId }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeaveTranscationByStaffId)).DataSource.Data;
                    if (lilmleavetranscation != null && lilmleavetranscation.Count > 0)
                    {
                        foreach (var item in lilmleavetranscation)
                        {
                            if (Convert.ToInt32(item.CREDIT_DAYS) > Convert.ToInt32(item.DEBIT_DAYS) && item.IS_CARRIED_NEXT_YEAR == "1")
                            {
                                settingids += item.LEAVE_SETTING_ID + ",";
                            }
                        }
                        settingids = settingids.TrimEnd(',');
                        sSQL = StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeavesettingByCategoryId).Replace(Common.Delimiter.QUS + Common.PR_LEAVE_SETTING.STF_LEAVE_SETTING_ID, settingids);
                        var lileavetype = (List<PR_LEAVE_SETTING>)CMSHandler.FetchData<PR_LEAVE_SETTING>(new LM_TRANSACTION_DETAILS() { STAFF_ID = sUserId }, sSQL, sAcademicYear).DataSource.Data;
                        if (lileavetype != null && lileavetype.Count > 0)
                        {
                            objViewModel.LeaveType = new SelectList(lileavetype, Common.PR_LEAVE_SETTING.STF_LEAVE_SETTING_ID, Common.PR_LEAVE_SETTING.LEAVE_TYPE);
                        }

                    }



                    //Fetch Session
                    var lisession = (List<SUP_SESSION>)CMSHandler.FetchData<SUP_SESSION>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSession), sAcademicYear).DataSource.Data;
                    if (lisession != null && lisession.Count > 0)
                    {
                        objViewModel.SESSION = new SelectList(lisession, Common.SUP_SESSION.SESSION_ID, Common.SUP_SESSION.SESSION_NAME);
                    }
                    //Fetch all leave transaction 
                    var liLeaveTransactionDetails = (List<LM_TRANSACTION_DETAILS>)CMSHandler.FetchData<LM_TRANSACTION_DETAILS>(new LM_TRANSACTION_DETAILS() { STAFF_ID = sUserId }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLmTransactionDetailsByStaffId), sAcademicYear).DataSource.Data;

                    if (liLeaveTransactionDetails != null && liLeaveTransactionDetails.Count > 0)
                    {
                        objViewModel.liTransactionDetails = liLeaveTransactionDetails;

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "ListStaffLeaveApply", ex.Message);
                    }
                }


            }
            return View(objViewModel);
        }
        public JsonResult FetchLeaveAppliedById(string sleaveid)
        {
            var objModel = new LM_STAFF_LEAVE_DETAILS();

            if (!string.IsNullOrEmpty(sleaveid))
            {
                try
                {
                    objModel.LEAVE_APPLY_ID = sleaveid;


                    var Lileaveapplied = (List<LM_STAFF_LEAVE_DETAILS>)CMSHandler.FetchData<LM_STAFF_LEAVE_DETAILS>(objModel, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeaveAppliedById)).DataSource.Data;
                    if (Lileaveapplied != null && Lileaveapplied.Count > 0)
                    {
                        objModel.LEAVE_APPLY_ID = Lileaveapplied.FirstOrDefault().LEAVE_APPLY_ID;
                        objModel.LEAVE_SETTING_ID = Lileaveapplied.FirstOrDefault().LEAVE_SETTING_ID;

                        objModel.DESCRIPTION = Lileaveapplied.FirstOrDefault().DESCRIPTION;
                        objModel.FROM_DATE = Lileaveapplied.FirstOrDefault().FROM_DATE;
                        objModel.TO_DATE = Lileaveapplied.FirstOrDefault().TO_DATE;
                        objModel.FROM_SESSION = Lileaveapplied.FirstOrDefault().FROM_SESSION;
                        objModel.TO_SESSION = Lileaveapplied.FirstOrDefault().TO_SESSION;
                    }
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "FetchLeaveAppliedById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateStaffLeaveApply(string sJsonData)
        {
            string sNoOfDays = string.Empty;
            double noofday = 0;
            int remaindays = 0;
            string sSQL = string.Empty;
            if (!string.IsNullOrEmpty(sJsonData))
            {

                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    var Staffleaveapply = JsonConvert.DeserializeObject<LM_STAFF_LEAVE_DETAILS>(sJsonData);
                    if (Staffleaveapply != null)
                    {
                        Staffleaveapply.STAFF_ID = sUserId;
                        Staffleaveapply.ACADEMIC_YEAR = sAcademicYear;
                        Staffleaveapply.FROM_DATE = CommonMethods.ConvertdatetoyearFromat(Staffleaveapply.FROM_DATE);
                        Staffleaveapply.TO_DATE = CommonMethods.ConvertdatetoyearFromat(Staffleaveapply.TO_DATE);
                        var liFetchCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchCalenderDetails), sAcademicYear).DataSource.Data;
                        List<CALENDER_2017> liCalender = new List<CALENDER_2017>();

                        if (liFetchCalender != null && liFetchCalender.Count > 0)
                        {
                            //filter only working days
                            liCalender = liFetchCalender.Where(x => x.IS_HOLIDAY != "1").ToList();

                            //get date from day id
                            var sDayFromDayId = liCalender.Where(x => x.DAY_ORDER_DATE == Staffleaveapply.FROM_DATE).Select(x => x.DAY_ID).ToList();

                            //get date from to id
                            var sDayToDayId = liCalender.Where(x => x.DAY_ORDER_DATE == Staffleaveapply.TO_DATE).Select(x => x.DAY_ID).ToList();

                            if (sDayFromDayId != null && sDayToDayId != null && sDayFromDayId.Count > 0 && sDayToDayId.Count > 0)
                            {
                                var liBetweenDays = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) >= Convert.ToInt32(sDayFromDayId.FirstOrDefault()) && Convert.ToInt32(x.DAY_ID) <= Convert.ToInt32(sDayToDayId.FirstOrDefault())).ToList();

                                if (liBetweenDays != null && liBetweenDays.Count > 0)
                                {
                                    sNoOfDays = liBetweenDays.Count.ToString();
                                }

                            }


                            var lilmleavetranscation = (List<LM_TRANSACTION_DETAILS>)CMSHandler.FetchData<LM_TRANSACTION_DETAILS>(new LM_TRANSACTION_DETAILS() { STAFF_ID = sUserId }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeaveTranscationByStaffId)).DataSource.Data;
                            if (lilmleavetranscation != null && lilmleavetranscation.Count > 0)
                            {
                                var lileavetranscation = lilmleavetranscation.Where(o => o.LEAVE_SETTING_ID == Staffleaveapply.LEAVE_SETTING_ID).ToList();
                                if (lileavetranscation != null && lileavetranscation.Count > 0)
                                {
                                    remaindays = Convert.ToInt32(lileavetranscation.FirstOrDefault().CREDIT_DAYS) - Convert.ToInt32(lileavetranscation.FirstOrDefault().DEBIT_DAYS);
                                    if (remaindays >= Convert.ToInt32(sNoOfDays))
                                    {
                                        if (!string.IsNullOrEmpty(sNoOfDays))
                                        {
                                            //validate if it already exists
                                            if (!ValidateLeaveDate(Staffleaveapply.FROM_DATE, Staffleaveapply.TO_DATE))
                                            {

                                                //Staffleaveapply.NO_OF_DAYS = sNoOfDays;
                                            }
                                            else
                                            {
                                                ObjJsonData.Message = "The given date is already exist!";
                                                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;

                                                return Json(ObjJsonData);
                                            }
                                        }
                                        else
                                        {

                                            ObjJsonData.Message = "The given date is not valid!";
                                            ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;

                                            return Json(ObjJsonData);
                                        }
                                        if (Staffleaveapply.FROM_SESSION == Common.SESSION.AF)
                                        {
                                            noofday = noofday + 0.5;
                                        }
                                        if (Staffleaveapply.TO_SESSION == Common.SESSION.FN)
                                        {
                                            noofday = noofday + 0.5;
                                        }
                                        Staffleaveapply.NO_OF_DAYS = (Convert.ToDouble(sNoOfDays) - noofday).ToString();
                                        var ResultArg = CMSHandler.SaveOrUpdate(Staffleaveapply, StaffSQL.GetStaffSQL((!string.IsNullOrEmpty(Staffleaveapply.LEAVE_APPLY_ID) ? StaffSQLCommands.UpdateStaffleaveapply : StaffSQLCommands.SaveStaffleaveapply)));
                                        if (ResultArg.Success)
                                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                        else
                                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                                    }
                                    else
                                    {

                                        ObjJsonData.Message = "There is only " + remaindays + " days are Remaining...!";
                                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;

                                        return Json(ObjJsonData);

                                    }
                                }

                            }


                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "SaveorUpdateStaffLeaveApply", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public bool ValidateLeaveDate(string sDateFrom, string sDateTo)
        {
            Boolean sStatus = false;
            string sAcademicYearId = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sDateFrom) && !string.IsNullOrEmpty(sDateTo))
            {
                var liFetchCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchCalenderDetails), sAcademicYear).DataSource.Data;
                List<CALENDER_2017> liCalender = new List<CALENDER_2017>();

                if (liFetchCalender != null && liFetchCalender.Count > 0)
                {
                    //filter only working days
                    liCalender = liFetchCalender.Where(x => x.IS_HOLIDAY != "1").ToList();

                    //get day id from date
                    var sDayFromDayId = liCalender.Where(x => x.DAY_ORDER_DATE == sDateFrom).Select(x => x.DAY_ID).ToList();

                    //get day id from date
                    var sDayToDayId = liCalender.Where(x => x.DAY_ORDER_DATE == sDateTo).Select(x => x.DAY_ID).ToList();

                    if (sDayFromDayId != null && sDayToDayId != null && sDayFromDayId.Count > 0 && sDayToDayId.Count > 0)
                    {
                        //get between days form from date and end date
                        var liBetweenDays = liCalender.Where(x => Convert.ToInt32(x.DAY_ID) >= Convert.ToInt32(sDayFromDayId.FirstOrDefault()) && Convert.ToInt32(x.DAY_ID) <= Convert.ToInt32(sDayToDayId.FirstOrDefault())).ToList();

                        if (liBetweenDays != null && liBetweenDays.Count > 0)
                        {
                            foreach (var item in liBetweenDays)
                            {
                                //check if it date alredy taken for leave

                                var liMaintainDetails = (List<LM_STAFF_LEAVE_DETAILS>)CMSHandler.FetchData<LM_STAFF_LEAVE_DETAILS>(new LM_STAFF_LEAVE_DETAILS()
                                {
                                    STAFF_ID = sUserId,
                                    FROM_DATE = item.DAY_ORDER_DATE,
                                    ACADEMIC_YEAR = sAcademicYearId
                                }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLmMaintainDetailsByDateFrom), sAcademicYear).DataSource.Data;

                                if (liMaintainDetails != null && liMaintainDetails.Count > 0)
                                {
                                    //if date found return
                                    sStatus = true;
                                    return sStatus;
                                }
                            }
                        }

                    }

                }

            }
            return sStatus;
        }
        public JsonResult DeleteAppliedLeave(string sleaveid)
        {
            if (!string.IsNullOrEmpty(sleaveid))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new STU_LEAVE_APPLY() { LEAVE_APPLY_ID = sleaveid }, StaffSQL.GetStaffSQL(StaffSQLCommands.DeleteAppliedLeave));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "DeleteAppliedLeave", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        //Staff Leave Applied 
        public ActionResult ListLeaveAppliedStaff()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sSQL = string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch Role based leave requested list
                    sSQL = StaffSQL.GetStaffSQL(StaffSQLCommands.FetchApprovalroleByUserId).Replace(Common.Delimiter.QUS + Common.LM_ROLE_BASED_APPROVAL.APPROVER_ROLE_ID, Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString());
                    var liapprovalrole = (List<LM_ROLE_BASED_APPROVAL>)CMSHandler.FetchData<LM_ROLE_BASED_APPROVAL>(null, sSQL, sAcademicYear).DataSource.Data;
                    if (liapprovalrole != null && liapprovalrole.Count > 0)
                    {
                        string sRoleId = string.Join(",", liapprovalrole.Select(x => x.ROLE_ID).ToArray());
                        sSQL = string.Empty;
                        //Fetch Leave Applied
                        sSQL = StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeaveAppliedStaff).Replace(Common.Delimiter.QUS + Common.LM_ROLE_BASED_APPROVAL.ROLE_ID, sRoleId);
                        objViewModel.lileaveapply = (List<LM_STAFF_LEAVE_DETAILS>)CMSHandler.FetchData<LM_STAFF_LEAVE_DETAILS>(null, sSQL, sAcademicYear).DataSource.Data;
                        objViewModel.lirolebasedapproval = liapprovalrole.ToList();
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Staff", "ListLeaveAppliedStaff", ex.Message);
                    }
                }


            }
            return View(objViewModel);
        }
        public ActionResult FetchLeaveAppliedDetails(string sleaveapplyid)
        {
            StaffViewModel objViewModel = new StaffViewModel();
            var objModel = new LM_STAFF_LEAVE_DETAILS();
            string sSQL = string.Empty;
            if (!string.IsNullOrEmpty(sleaveapplyid))
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                try
                {
                    objModel.LEAVE_APPLY_ID = sleaveapplyid;
                    var Lileaveapplied = (List<LM_STAFF_LEAVE_DETAILS>)CMSHandler.FetchData<LM_STAFF_LEAVE_DETAILS>(objModel, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeaveAppliedDetails), sAcademicYear).DataSource.Data;
                    if (Lileaveapplied != null && Lileaveapplied.Count > 0)
                    {
                        objModel.STAFF_CODE = Lileaveapplied.FirstOrDefault().STAFF_CODE;
                        objModel.FIRST_NAME = Lileaveapplied.FirstOrDefault().FIRST_NAME;

                        objModel.LEAVE_APPLY_ID = Lileaveapplied.FirstOrDefault().LEAVE_APPLY_ID;
                        objModel.LEAVE_TYPE = Lileaveapplied.FirstOrDefault().LEAVE_TYPE;
                        objModel.LEAVE_TYPE_ID = Lileaveapplied.FirstOrDefault().LEAVE_TYPE_ID;
                        objModel.DESCRIPTION = Lileaveapplied.FirstOrDefault().DESCRIPTION;
                        objModel.FROM_DATE = Lileaveapplied.FirstOrDefault().FROM_DATE;
                        objModel.TO_DATE = Lileaveapplied.FirstOrDefault().TO_DATE;
                        objModel.APPROVAL_STATUS1 = Lileaveapplied.FirstOrDefault().APPROVAL_STATUS1;
                        objModel.APPROVAL_STATUS2 = Lileaveapplied.FirstOrDefault().APPROVAL_STATUS2;

                        objViewModel.lileaveapply = Lileaveapplied;
                    }
                    var lileavestatus = (List<SUP_LEAVE_STATUS>)CMSHandler.FetchData<SUP_LEAVE_STATUS>(objModel, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchLeaveStatus)).DataSource.Data;
                    if (lileavestatus != null && lileavestatus.Count > 0)
                    {
                        objViewModel.LeaveStatus = new SelectList(lileavestatus, Common.SUP_LEAVE_STATUS.LEAVE_STATUS_ID, Common.SUP_LEAVE_STATUS.LEAVE_STATUS);
                    }
                    sSQL = StaffSQL.GetStaffSQL(StaffSQLCommands.FetchApprovalroleByUserId).Replace(Common.Delimiter.QUS + Common.LM_ROLE_BASED_APPROVAL.APPROVER_ROLE_ID, Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString());
                    objViewModel.lirolebasedapproval = (List<LM_ROLE_BASED_APPROVAL>)CMSHandler.FetchData<LM_ROLE_BASED_APPROVAL>(null, sSQL, sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Staff", "FetchLeaveAppliedDetails", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveorUpdateLeaveStatus(string sLeaveapplyid, string sLevel1, string sLevel2)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sLeaveapplyid) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new LM_STAFF_LEAVE_DETAILS() { LEAVE_APPLY_ID = sLeaveapplyid, APPROVAL_STATUS1 = sLevel1, APPROVAL_STATUS2 = sLevel2 }, StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateLeaveStatus));
                    if (ResultArg.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        if (sLevel2 == Common.LEAVE_STATUS.Approved)
                        {
                            //Insert Leave Transcation
                            var Leavedetails = (List<LM_STAFF_LEAVE_DETAILS>)CMSHandler.FetchData<LM_STAFF_LEAVE_DETAILS>(new LM_STAFF_LEAVE_DETAILS() { LEAVE_APPLY_ID = sLeaveapplyid }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeaveAppliedDetails)).DataSource.Data;
                            if (Leavedetails != null && Leavedetails.Count > 0)
                            {
                                //Check Is Exist
                                var Transcationdetails = (List<LM_TRANSACTION_DETAILS>)CMSHandler.FetchData<LM_TRANSACTION_DETAILS>(new LM_STAFF_LEAVE_DETAILS() { LEAVE_APPLY_ID = sLeaveapplyid }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeaveTranscation)).DataSource.Data;
                                if (Transcationdetails != null && Transcationdetails.Count > 0)
                                {
                                    //code has write
                                }
                                else
                                {
                                    foreach (var item in Leavedetails)
                                    {

                                        ResultArg = CMSHandler.SaveOrUpdate(new LM_TRANSACTION_DETAILS() { STAFF_ID = item.STAFF_ID, DEBIT_DAYS = item.NO_OF_DAYS, LEAVE_SETTING_ID = item.LEAVE_SETTING_ID, ACADEMIC_YEAR = sAcademicYear, LEAVE_APPLY_ID = item.LEAVE_APPLY_ID }, StaffSQL.GetStaffSQL(StaffSQLCommands.SaveLeaveTranscations), sAcademicYear);
                                    }

                                }
                            }
                        }
                        else
                        {
                            var Transcationdetails = (List<LM_TRANSACTION_DETAILS>)CMSHandler.FetchData<LM_TRANSACTION_DETAILS>(new LM_STAFF_LEAVE_DETAILS() { LEAVE_APPLY_ID = sLeaveapplyid }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchLeaveTranscation)).DataSource.Data;
                            if (Transcationdetails != null && Transcationdetails.Count > 0)
                            {
                                ResultArg = CMSHandler.SaveOrUpdate(new LM_TRANSACTION_DETAILS() { TRANSACTION_ID = Transcationdetails.FirstOrDefault().TRANSACTION_ID }, StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateIsDeletedByTranscationId));
                            }
                        }
                    }
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Staff", "SaveorUpdateLeaveStatus", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        //Role Based Approval
        public ActionResult ListRoleBasedApproval()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    //Fetch Role
                    var lirole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademicYear).DataSource.Data;
                    if (lirole != null && lirole.Count > 0)
                    {
                        objViewModel.ROLE = new SelectList(lirole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                    }
                    //Fetch apporval level
                    var liapprovallevel = (List<LM_SUP_APPROVAL_LEVEL>)CMSHandler.FetchData<LM_SUP_APPROVAL_LEVEL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsupApprovallevel), sAcademicYear).DataSource.Data;
                    if (liapprovallevel != null && liapprovallevel.Count > 0)
                    {
                        objViewModel.APPROVAL_LEVEL = new SelectList(liapprovallevel, Common.LM_SUP_APPROVAL_LEVEL.LEVEL_ID, Common.LM_SUP_APPROVAL_LEVEL.LEVEL_NAME);
                    }

                    objViewModel.lirolebasedapproval = (List<LM_ROLE_BASED_APPROVAL>)CMSHandler.FetchData<LM_ROLE_BASED_APPROVAL>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchRoleBasedApproval)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "ListRoleBasedApproval", ex.Message);
                    }
                }


            }
            return View(objViewModel);
        }
        public JsonResult FetchRoleBasedApprovalById(string sApprovalId)
        {
            var objModel = new LM_ROLE_BASED_APPROVAL();

            if (!string.IsNullOrEmpty(sApprovalId))
            {
                try
                {
                    objModel.APPROVAL_ID = sApprovalId;


                    var lirolebasedapproval = (List<LM_ROLE_BASED_APPROVAL>)CMSHandler.FetchData<LM_ROLE_BASED_APPROVAL>(objModel, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchRoleBasedApprovalById)).DataSource.Data;
                    if (lirolebasedapproval != null && lirolebasedapproval.Count > 0)
                    {
                        objModel.APPROVAL_ID = lirolebasedapproval.FirstOrDefault().APPROVAL_ID;
                        objModel.ROLE_ID = lirolebasedapproval.FirstOrDefault().ROLE_ID;

                        objModel.APPROVER_ROLE_ID = lirolebasedapproval.FirstOrDefault().APPROVER_ROLE_ID;
                        objModel.APPROVAL_LEVEL = lirolebasedapproval.FirstOrDefault().APPROVAL_LEVEL;

                    }
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "FetchRoleBasedApprovalById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateRoleBasedApproval(string sJsonData)
        {
            string sNoOfDays = string.Empty;

            if (!string.IsNullOrEmpty(sJsonData))
            {

                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

                    var rolebasedapproval = JsonConvert.DeserializeObject<LM_ROLE_BASED_APPROVAL>(sJsonData);
                    if (rolebasedapproval != null)
                    {

                        var ResultArg = CMSHandler.SaveOrUpdate(rolebasedapproval, StaffSQL.GetStaffSQL((!string.IsNullOrEmpty(rolebasedapproval.APPROVAL_ID) ? StaffSQLCommands.UpdateRoleBasedApproval : StaffSQLCommands.SaveRoleBasedApproval)));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "SaveorUpdateRoleBasedApproval", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteRoleBasedApproval(string sApprovalId)
        {
            if (!string.IsNullOrEmpty(sApprovalId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new LM_ROLE_BASED_APPROVAL() { APPROVAL_ID = sApprovalId }, StaffSQL.GetStaffSQL(StaffSQLCommands.DeleteRoleBasedApproval));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "DeleteRoleBasedApproval", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  Report(Class Incharge,HOD,Principal)
        public ActionResult ClassStudentAttendanceMarkProfile()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch classess                 
                    var liclass = (List<SUP_CLASS>)CMSHandler.FetchData<SUP_CLASS>(new stf_Personal_Info() { STAFF_ID = sUserId },
                         StaffSQL.GetStaffSQL(StaffSQLCommands.FetchclasslistByStaffId), sAcademicYear).DataSource.Data;
                    if (liclass != null && liclass.Count > 0)
                    {
                        objViewModel.ClassList = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "ClassStudentAttendanceMarkProfile", ex.Message);
                    }
                }


            }
            return View(objViewModel);
        }
        public ActionResult BindClasswiseStudent(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch class student                
                    objViewModel.liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(new CP_CLASSES() { CLASS_ID = sClassId }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchClassWiseStudent), sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "BindClasswiseStudent", ex.Message);
                    }
                }


            }
            return View(objViewModel);
        }
        public ActionResult StudentAttendance(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STU_CLASS();
                    ObjModel.STUDENT_ID = sStudentId;
                    objViewModel.liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(ObjModel,
                             SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoForReport), sAcademicYear).DataSource.Data;

                    string sSQL = string.Empty;
                    // Fetch absent Hours ...
                    var liFetchAbsentHours = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new STU_CLASS() { STUDENT_ID = sStudentId },
                                ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTotalAbsentHours), sAcademicYear).DataSource.Data;


                    // Get All The Regular Papers By Student_Id ...
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sStudentId },
                        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                    var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sStudentId },
                          ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;


                    // Loop Optional Papers  To Remove Unnecessary Courses...
                    if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                    {
                        foreach (var item in liOptionalCourses)
                        {
                            var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == item.COURSE_ROOT_ID).ToList();
                            if (IsExist != null && IsExist.Count > 0)
                            {
                                liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                                item.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                liRegularCourses.Add(item);
                            }
                        }
                    }

                    string sCourseIds = string.Join(",", liRegularCourses.Select(x => x.COURSE_ROOT_ID).ToArray());
                    sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTotalHours).Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_CLASS.STUDENT_ID, sStudentId);
                    // Fetch Total Hours ...
                    var liFetchTotalHours = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, sAcademicYear).DataSource.Data;

                    // Attendace Calculation ...
                    if (liFetchTotalHours != null && liFetchTotalHours.Count > 0 && liFetchAbsentHours != null && liFetchAbsentHours.Count > 0)
                    {
                        int nTotal = Convert.ToInt32(liFetchTotalHours.FirstOrDefault().TOTAL_HRS);
                        double nTotalHrs = Convert.ToDouble(liFetchTotalHours.FirstOrDefault().TOTAL_HRS);
                        double nAbsentHrs = Convert.ToDouble(liFetchAbsentHours.FirstOrDefault().TOTAL_HRS);
                        double percenthrs = nTotalHrs - nAbsentHrs;
                        double nAverageHrs = (percenthrs / (double)nTotalHrs) * 100;
                        ViewBag.Attendancepercentage = Math.Round(nAverageHrs);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "StudentAttendance", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult StudentCIAMark(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;


            var ObjViewModel = new ExaminationViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STU_CLASS();
                    ObjModel.STUDENT_ID = sStudentId;
                    var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(ObjModel,
                             SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoForReport), sAcademicYear).DataSource.Data;
                    if (liStudent != null && liStudent.Count > 0)
                    {
                        ObjViewModel.liStudentList = (ObjViewModel.liStudentList != null && ObjViewModel.liStudentList.Count > 0) ? ObjViewModel.liStudentList.Concat(liStudent).ToList() : liStudent;
                    }
                    // Fetch Regular Papers .
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sStudentId },
                            SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOddAndEvenSemCourseByStudent), sAcademicYear).DataSource.Data;

                    // Fetch Optional Papers .
                    var liOptionalCourses = (List<EXAM_REGISTRATION>)DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sStudentId },
                                 SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOddAndEvenSemOptionalCourseByStudent), sAcademicYear).DataSource.Data;

                    // Loop Optional Papers  To Remove Unnecessary Courses...
                    if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                    {
                        foreach (var item in liOptionalCourses)
                        {
                            var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == item.COURSE_ROOT_ID).ToList();
                            if (IsExist != null && IsExist.Count > 0)
                            {
                                liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                                item.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                liRegularCourses.Add(item);
                            }
                        }
                    }

                    if (liRegularCourses != null && liRegularCourses.Count > 0)
                    {
                        ObjViewModel.liExamRegistrationList = liRegularCourses;
                    }

                    if (liRegularCourses != null && liRegularCourses.Count > 0)
                    {
                        foreach (var item in liRegularCourses)
                        {
                            item.STUDENT_ID = sStudentId;
                            item.COURSE_ID = item.COURSE_ROOT_ID;
                            var liCIAMarks = (List<CIA_MARKS_INFO>)CMSHandler.FetchData<CIA_MARKS_INFO>(item, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCIAMarksByCourseIdAndStudentId), sAcademicYear).DataSource.Data;
                            if (liCIAMarks != null && liCIAMarks.Count > 0)
                            {
                                ObjViewModel.liCIA_Marks_Info = (ObjViewModel.liCIA_Marks_Info != null && ObjViewModel.liCIA_Marks_Info.Count > 0) ? ObjViewModel.liCIA_Marks_Info.Concat(liCIAMarks).ToList() : liCIAMarks;
                            }

                            var liCIA_Total = (List<CIA_TOTAL>)CMSHandler.FetchData<CIA_TOTAL>(item, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCIATotalByStudentAndCourseId), sAcademicYear).DataSource.Data;
                            if (liCIA_Total != null && liCIA_Total.Count > 0)
                            {
                                liCIA_Total.ForEach(o => o.TOTAL = Math.Round(Convert.ToDecimal((o.TOTAL).ToUpper().Equals("A") ? "0" : o.TOTAL), MidpointRounding.AwayFromZero).ToString());
                                if (liCIA_Total != null && liCIA_Total.Count > 0)
                                {
                                    ObjViewModel.liCIA_Total = (ObjViewModel.liCIA_Total != null && ObjViewModel.liCIA_Total.Count > 0) ? ObjViewModel.liCIA_Total.Concat(liCIA_Total).ToList() : liCIA_Total;
                                }
                            }
                        }
                    }



                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StudentController", "StudentCIAMark", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult StudentSemsterMark(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            StudentViewModel objViewModel = new StudentViewModel();
            DashboardViewModel objDashboard = new DashboardViewModel();
            SUP_BATCH_DURATION objbatchduartion = new SUP_BATCH_DURATION();
            List<SEMESTER_MARKS> liSemestermarks = new List<SEMESTER_MARKS>();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    dv.Add(Common.STUDENT_INFO.ACADEMIC_YEAR, sAcademicYear, EnumCommand.DataType.String);
                    dv.Add(Common.STUDENT_INFO.STUDENT_ID, sStudentId, EnumCommand.DataType.String);
                    resultArgs = objDashboard.FetchStudentInfoById(dv, sAcademicYear);
                    if (resultArgs.Success)
                    {
                        if (resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {
                            Session[Common.SESSION_VARIABLES.USER_INFO] = resultArgs.DataSource.Table;
                            Session[Common.SESSION_VARIABLES.CLASS_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_ID];
                            Session[Common.SESSION_VARIABLES.DEPARTMENT_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.DEPT_ID];
                            Session[Common.SESSION_VARIABLES.SHIFT_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.SHIFT_ID];
                            Session[Common.SESSION_VARIABLES.CLASS_NAME] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_NAME];
                            Session[Common.SESSION_VARIABLES.PHOTO] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.PHOTO];
                            Session[Common.SESSION_VARIABLES.ADMITTED_BATCH] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.ADMITTED_BATCH];
                            Session[Common.SESSION_VARIABLES.CLASS_LEVEL] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_LEVEL];
                        }
                    }
                    objbatchduartion.BATCH_ID = Session[Common.SESSION_VARIABLES.ADMITTED_BATCH].ToString();
                    objbatchduartion.APPLICATION_TYPE_ID = Session[Common.SESSION_VARIABLES.CLASS_LEVEL].ToString();
                    string sApplicationtype = Session[Common.SESSION_VARIABLES.CLASS_LEVEL].ToString();
                    var BatchExpiredYear = (List<SUP_BATCH_DURATION>)CMSHandler.FetchData<SUP_BATCH_DURATION>(objbatchduartion, SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.FetchBatchExpiredYear)).DataSource.Data;
                    if (BatchExpiredYear != null && BatchExpiredYear.Count > 0)
                    {

                        for (int i = Convert.ToInt16(BatchExpiredYear.FirstOrDefault().BEGIN_YEAR); i < Convert.ToInt16(BatchExpiredYear.FirstOrDefault().EXPIRED_YEAR); i++)
                        {
                            var Studentmarks = (List<SEMESTER_MARKS>)CMSHandler.FetchData<SEMESTER_MARKS>(new SEMESTER_MARKS() { STUDENT_ID = sStudentId, ACADEMIC_YEAR = sAcademicYear }, StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentMarks), i.ToString()).DataSource.Data;
                            if (Studentmarks != null && Studentmarks.Count > 0)
                            {
                                liSemestermarks = liSemestermarks.Concat(Studentmarks).ToList();
                            }
                        }
                        if (liSemestermarks != null && liSemestermarks.Count > 0)
                        {
                            objViewModel.liSemestermark = liSemestermarks.OrderBy(o => o.SEMESTER).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StudentController", "StudentSemsterMark", ex.Message);
                    }
                }


            }
            return View(objViewModel);
        }
        public ActionResult StudentProfile(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STU_CLASS();
                    ObjModel.STUDENT_ID = sStudentId;
                    objViewModel.liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(ObjModel,
                             SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoForProfile), sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "StudentProfile", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult StudentAttendanceMarkProfileforHOD()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch Department                 
                    var lidep = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                         SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (lidep != null && lidep.Count > 0)
                    {
                        objViewModel.DepartmentList = new SelectList(lidep, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "StudentAttendanceMarkProfileforHOD", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchClassessByDepartment(string sDepId)
        {
            string sSQL = string.Empty;
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sDepId))
            {
                try
                {
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmentIds).Replace(Common.Delimiter.QUS + Common.CP_DEPARTMENT_2017.DEPARTMENT, sDepId);
                    var licls = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, sSQL).DataSource.Data;
                    if (licls != null && licls.Count > 0)
                    {
                        foreach (var cls in licls)
                        {
                            sOption += "<option value=" + cls.CLASS_ID + ">" + cls.CLASS_NAME + "</option>";
                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "FetchClassessByDepartment", ex.Message);
                    }

                }
            }
            return Json(sOption, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DepartmentWiseStudentandStaffList()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch Dep of HOD
                    var Department = (List<Department>)CMSHandler.FetchData<Department>(new STF_PERSONAL_INFO() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentByStaffId)).DataSource.Data;
                    if (Department != null && Department.Count > 0)
                    {
                        //Fetch Student List
                        var Liststudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(new Department() { DEPARTMENT_ID = Department.FirstOrDefault().DEPARTMENT }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStudentListByDepartmentId), sAcademicYear).DataSource.Data;
                        objViewModel.liStudentList = Liststudent;
                        //Fetch Staff List
                        var Liststaff = (List<StaffList>)CMSHandler.FetchData<StaffList>(new Department() { DEPARTMENT_ID = Department.FirstOrDefault().DEPARTMENT }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffListByDepartmentId), sAcademicYear).DataSource.Data;
                        objViewModel.lstStaffList = Liststaff;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "DepartmentWiseStudentandStaffList", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindStudentProfile(string sStudentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sStudentId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STU_CLASS();
                    ObjModel.STUDENT_ID = sStudentId;
                    objViewModel.liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(ObjModel,
                             SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoForProfile), sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "BindStudentProfile", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindStaffProfile(string sStaffId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sStaffId) && !string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new StaffList();
                    ObjModel.STAFF_ID = sStaffId;
                    objViewModel.lstStaffList = (List<StaffList>)CMSHandler.FetchData<StaffList>(ObjModel, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffProfileById)).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "BindStaffProfile", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult ListLackofAttendanceforPrincipal()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            StaffViewModel objViewModel = new StaffViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var lidepartment = (List<Department>)CMSHandler.FetchData<Department>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;
                    if (lidepartment != null && lidepartment.Count > 0)
                    {
                        objViewModel.DepartmentList = new SelectList(lidepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "ListLackofAttendanceforPrincipal", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindStudentlistofLackofAttendance(string sClassId)
        {
            StaffViewModel objViewModel = new StaffViewModel();
            var Liststudent = new List<ListStudents>();
            double nAverageHrs = 0;
            string sSQL = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sAcademicYear))
            {

                try
                {
                    sSQL = StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStudentListByClassId).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                    var listudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null,
                            sSQL, sAcademicYear).DataSource.Data;
                    if (listudent != null && listudent.Count > 0)
                    {

                        foreach (var item in listudent)
                        {

                            // Fetch absent Hours ...
                            var liFetchAbsentHours = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                                        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTotalAbsentHours), sAcademicYear).DataSource.Data;


                            // Get All The Regular Papers By Student_Id ...
                            var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                                ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                            var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                                  ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;


                            // Loop Optional Papers  To Remove Unnecessary Courses...
                            if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                            {
                                foreach (var oplcourse in liOptionalCourses)
                                {
                                    var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == oplcourse.COURSE_ROOT_ID).ToList();
                                    if (IsExist != null && IsExist.Count > 0)
                                    {
                                        liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                                        oplcourse.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                        liRegularCourses.Add(oplcourse);
                                    }
                                }
                            }
                            sSQL = string.Empty;
                            string sCourseIds = string.Join(",", liRegularCourses.Select(x => x.COURSE_ROOT_ID).ToArray());
                            sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTotalHours).Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                            sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_CLASS.STUDENT_ID, item.STUDENT_ID);
                            // Fetch Total Hours ...
                            var liFetchTotalHours = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, sAcademicYear).DataSource.Data;

                            // Attendace Calculation ...
                            if (liFetchTotalHours != null && liFetchTotalHours.Count > 0 && liFetchAbsentHours != null && liFetchAbsentHours.Count > 0)
                            {
                                int nTotal = Convert.ToInt32(liFetchTotalHours.FirstOrDefault().TOTAL_HRS);
                                double nTotalHrs = Convert.ToDouble(liFetchTotalHours.FirstOrDefault().TOTAL_HRS);
                                double nAbsentHrs = Convert.ToDouble(liFetchAbsentHours.FirstOrDefault().TOTAL_HRS);
                                if (nTotal == 0)
                                {
                                    nAverageHrs = 0;
                                }
                                else
                                {
                                    double percenthrs = nTotalHrs - nAbsentHrs;
                                    nAverageHrs = (percenthrs / (double)nTotalHrs) * 100;
                                }

                                if (Math.Round(nAverageHrs) < Convert.ToInt32(ConfigurationManager.AppSettings["LACKOFPERCENTAGE"]))
                                {
                                    Liststudent.Add(new ListStudents() { STUDENT_ID = item.STUDENT_ID, FIRST_NAME = item.FIRST_NAME, CLASS_NAME = item.CLASS_NAME, ROLL_NO = item.ROLL_NO, ATTENDANCE_PERCENTAGE = Math.Round(nAverageHrs).ToString() });
                                }
                            }


                        }
                        objViewModel.liStudentList = Liststudent;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "BindStudentlistofLackofAttendance", ex.Message);
                    }
                }

            }
            return View(objViewModel);
        }
        public ActionResult ListStudentLackofAttendance()
        {
            StaffViewModel objViewModel = new StaffViewModel();
            string sCourseIds = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Liststudent = new List<ListStudents>();
                    double nAverageHrs = 0;
                    string sSQL = string.Empty;
                    var listudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStudentList), sAcademicYear).DataSource.Data;
                    if (listudent != null && listudent.Count > 0)
                    {

                        foreach (var item in listudent)
                        {

                            // Fetch absent Hours ...
                            var liFetchAbsentHours = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                                        ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTotalAbsentHours), sAcademicYear).DataSource.Data;


                            // Get All The Regular Papers By Student_Id ...
                            var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                                ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                            var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                                  ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;

                            // Loop Optional Papers  To Remove Unnecessary Courses...
                            if (liOptionalCourses != null && liOptionalCourses.Count > 0 && liRegularCourses != null && liRegularCourses.Count > 0)
                            {
                                foreach (var oplcourse in liOptionalCourses)
                                {
                                    var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == oplcourse.COURSE_ROOT_ID).ToList();
                                    if (IsExist != null && IsExist.Count > 0)
                                    {
                                        liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                                        oplcourse.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                        liRegularCourses.Add(oplcourse);
                                    }
                                }
                                sCourseIds = string.Join(",", liRegularCourses.Select(x => x.COURSE_ROOT_ID).ToArray());
                            }
                            sSQL = string.Empty;

                            sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTotalHours).Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                            sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_CLASS.STUDENT_ID, item.STUDENT_ID);
                            // Fetch Total Hours ...
                            var liFetchTotalHours = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, sAcademicYear).DataSource.Data;

                            // Attendace Calculation ...
                            if (liFetchTotalHours != null && liFetchTotalHours.Count > 0 && liFetchAbsentHours != null && liFetchAbsentHours.Count > 0)
                            {
                                int nTotal = Convert.ToInt32(liFetchTotalHours.FirstOrDefault().TOTAL_HRS);
                                double nTotalHrs = Convert.ToDouble(liFetchTotalHours.FirstOrDefault().TOTAL_HRS);
                                double nAbsentHrs = Convert.ToDouble(liFetchAbsentHours.FirstOrDefault().TOTAL_HRS);
                                if (nTotal == 0)
                                {
                                    nAverageHrs = 0;
                                }
                                else
                                {
                                    double percenthrs = nTotalHrs - nAbsentHrs;
                                    nAverageHrs = (percenthrs / (double)nTotalHrs) * 100;
                                }

                                if (Math.Round(nAverageHrs) < Convert.ToInt32(ConfigurationManager.AppSettings["LACKOFPERCENTAGE"]))
                                {
                                    Liststudent.Add(new ListStudents() { STUDENT_ID = item.STUDENT_ID, FIRST_NAME = item.FIRST_NAME, CLASS_NAME = item.CLASS_NAME, ROLL_NO = item.ROLL_NO, ATTENDANCE_PERCENTAGE = Math.Round(nAverageHrs).ToString() });
                                }
                            }
                        }
                    }
                    objViewModel.liStudentList = Liststudent;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "ListStudentLackofAttendance", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region Staff Resume Entry 
        public ActionResult ListStaffResume()
        {
            var ObjViewModel = new StaffViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liStaffResume = DapperHandler.FetchAll<STF_RESUME>(StaffSQL.GetStaffSQL(StaffSQLCommands.FetchListStaffResume), null).ToList();

                    var liReligion = DapperHandler.FetchAll<SUP_RELIGION>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReligion), null).ToList();
                    if (liReligion != null && liReligion.Count > 0)
                    {
                        ObjViewModel.Religion = new SelectList(liReligion, Common.SUP_RELIGION.RELIGIONID, Common.SUP_RELIGION.RELIGION);
                    }

                    //var liQualification = DapperHandler.FetchAll<Sup_Qualification>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchQualification), null).ToList();
                    //if (liQualification != null && liQualification.Count > 0)
                    //{
                    //    ObjViewModel.Qualification = new SelectList(liQualification, Common.Sup_Qualification.QUALIFICATION_ID, Common.Sup_Qualification.QUALIFICATION);
                    //}

                }
                catch (Exception ex)
                {
                    using (ErrorLog Objlog = new ErrorLog())
                    {
                        Objlog.WriteError("StaffController", "ListStaffResume", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult ViewStaffResume()
        {
            return View();
        }

        public JsonResult FetchStaffResumeById(string sResumeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liStaffResume = DapperHandler.FetchAll<STF_RESUME>(StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffResumeByResumeId), new { RESUME_ID = sResumeId }, sAcademicYear).ToList();
                    if (liStaffResume != null && liStaffResume.Count > 0)
                    {
                        return Json(liStaffResume, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "FetchStaffResumeById", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveStaffResume(string sJson)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = JsonConvert.DeserializeObject<STF_RESUME>(sJson);
                    var Save = DapperHandler.SaveOrUpdate((!string.IsNullOrEmpty(ObjModel.RESUME_ID)) ? StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffResume) : StaffSQL.GetStaffSQL(StaffSQLCommands.SaveStaffResume), ObjModel, sAcademicYear);
                    if (Convert.ToInt32(Save) != 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "SaveStaffResume", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteResume(string sResumeId)
        {
            var sDelete = DapperHandler.SaveOrUpdate(StaffSQL.GetStaffSQL(StaffSQLCommands.DeleteStaffResume), new { RESUME_ID = sResumeId });
            if (Convert.ToInt32(sDelete) != 0)
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Staff details
        public ActionResult StaffDetailsforEdit()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            StaffViewModel objViewModel = new StaffViewModel();
            try
            {

                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] = null;
                    DataTable dtStaffList = new DataTable();

                    dtStaffList = objViewModel.FetchStaffInfo(sAcademicYear).DataSource.Table;

                    var liList = new List<StaffList>();
                    if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                    {
                        objViewModel.lstStaffList = (from DataRow dr in dtStaffList.Rows
                                                     select new StaffList
                                                     {
                                                         STAFF_ID = dr[Common.STF_PERSONAL_INFO.STAFF_ID].ToString(),
                                                         STAFF_CODE = dr[Common.STF_PERSONAL_INFO.STAFF_CODE].ToString(),
                                                         FIRST_NAME = dr[Common.STF_PERSONAL_INFO.FIRST_NAME].ToString(),
                                                         GENDER_NAME = dr[Common.SUP_GENDER.GENDER_NAME].ToString(),
                                                         DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                         SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString(),
                                                         DATE_OF_BIRTH = dr[Common.STF_PERSONAL_INFO.DATE_OF_BIRTH].ToString(),
                                                         IS_LEFT = dr[Common.STF_PERSONAL_INFO.IS_LEFT].ToString(),

                                                     }).ToList();
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog obj = new ErrorLog())
                {
                    obj.WriteError("StaffController", "ListStaffDetails", ex.Message);
                }
            }

            return View(objViewModel);
        }

        public ActionResult StaffSelfDetailsForEdit()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (string.IsNullOrEmpty(sUserId) && Session[Common.SESSION_VARIABLES.STAFF_ID] != null)
            {
                sUserId = Session[Common.SESSION_VARIABLES.STAFF_ID].ToString();
            }
            if (string.IsNullOrEmpty(sUserId) && Session[Common.SESSION_VARIABLES.USER_CODE] != null)
            {
                sUserId = Session[Common.SESSION_VARIABLES.USER_CODE].ToString();
            }

            StaffViewModel objViewModel = new StaffViewModel();
            try
            {
                if (!string.IsNullOrEmpty(sUserId))
                {
                    Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] = null;
                    DataTable dtStaffList = new DataTable();

                    var result = objViewModel.FetchStaffInfoByUserId(sUserId, sAcademicYear);
                    if (result != null && result.DataSource != null && result.DataSource.Table != null)
                    {
                        dtStaffList = result.DataSource.Table;
                    }

                    if (dtStaffList != null && dtStaffList.Rows.Count > 0)
                    {
                        objViewModel.lstStaffList = (from DataRow dr in dtStaffList.Rows
                                                     select new StaffList
                                                     {
                                                         STAFF_ID = dr[Common.STF_PERSONAL_INFO.STAFF_ID].ToString(),
                                                         STAFF_CODE = dr[Common.STF_PERSONAL_INFO.STAFF_CODE].ToString(),
                                                         FIRST_NAME = dr[Common.STF_PERSONAL_INFO.FIRST_NAME].ToString(),
                                                         GENDER_NAME = dr[Common.SUP_GENDER.GENDER_NAME].ToString(),
                                                         DEPARTMENT = dr[Common.CP_DEPARTMENT_2017.DEPARTMENT].ToString(),
                                                         SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString(),
                                                         DATE_OF_BIRTH = dr[Common.STF_PERSONAL_INFO.DATE_OF_BIRTH].ToString(),
                                                         IS_LEFT = dr[Common.STF_PERSONAL_INFO.IS_LEFT].ToString(),
                                                     }).ToList();
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog obj = new ErrorLog())
                {
                    obj.WriteError("StaffController", "StaffSelfDetailsForEdit", ex.Message);
                }
            }

            return View(objViewModel);
        }

        public ActionResult StaffSelfEdit()
        {
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (string.IsNullOrEmpty(sUserId) && Session[Common.SESSION_VARIABLES.STAFF_ID] != null)
            {
                sUserId = Session[Common.SESSION_VARIABLES.STAFF_ID].ToString();
            }
            if (string.IsNullOrEmpty(sUserId) && Session[Common.SESSION_VARIABLES.USER_CODE] != null)
            {
                sUserId = Session[Common.SESSION_VARIABLES.USER_CODE].ToString();
            }
            if (string.IsNullOrEmpty(sUserId))
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return RedirectToAction("EditStaffInfomationByID", new { Id = sUserId, fromSelfEdit = true });
        }

        public ActionResult ExportStaffList()
        {
            var staffList = CMSHandler.FetchData<STF_PERSONAL_INFO>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffListForExport)).DataSource.Data as List<STF_PERSONAL_INFO>;
            var csv = new StringBuilder();

            // Add headers
            csv.AppendLine("STAFF_CODE,FIRST_NAME,DATE_OF_BIRTH,DATE_OF_JOIN,GSUITE_MAIL_ID,EMAIL,DESIGNATION,DATE_OF_EPF,MOBILE_NO,CATEGORY,ACADEMIC_YEAR,BLOOD_GROUP,MARITAL_STATUS,DEPARTMENT,STAFF_STATUS,GENDER,COMMUNITY,NATIONALITY,RELIGION,QUALIFICATION,CASTE,AADTHAR_NO,DATE_OF_ENTRY_TEACHING,PAN_NO,RETIREMENT_DATE,LEAVING_DATE,STAFF_APPROVAL_NO,MPHIL_APPROVAL_NO,PHD_APPROVAL_NO,FATHER_NAME,MOTHER_NAME,SPOUSE_NAME,ADDRESS_PRESENT,ADDRESS_PERMANENT,EPF_NO,UAN_NO,PHD_COMPLETION,NET_COMPLETION,SET_COMPLETION,MPHIL_COMPLETION,PG_COMPLETION,UG_COMPLETION,HSC_COMPLETION,SSLC_COMPLETION,UG_EXPERIENCE,PG_EXPERIENCE,MPHIL_EXPERIENCE,PHD_EXPERIENCE,PHYSICAL_STATUS,BANK_AC_NO,BANK_NAME,BRANCH_NAME,IFSC_CODE,SALARY_BASIC,SALARY_DA,PACKAGE,SPECIAL_ALLOWANCE,OTHERS_SALARY,PP_SALARY,GROSS_SALARY");

            // Add data rows
            foreach (var staff in staffList)
            {
                csv.AppendLine($"\"{staff.STAFF_CODE}\",\"{staff.FIRST_NAME}\",\"{staff.DATE_OF_BIRTH}\",\"{staff.DATE_OF_JOIN}\",\"{staff.GSUITE_MAIL_ID}\",\"{staff.EMAIL}\",\"{staff.DESIGNATION}\",\"{staff.DATE_OF_EPF}\",\"{staff.MOBILE_NO}\",\"{staff.CATEGORY}\",\"{staff.ACADEMIC_YEAR}\",\"{staff.BLOOD_GROUP}\",\"{staff.MARITAL_STATUS}\",\"{staff.DEPARTMENT}\",\"{staff.STAFF_STATUS}\",\"{staff.GENDER}\",\"{staff.COMMUNITY}\",\"{staff.NATIONALITY}\",\"{staff.RELIGION}\",\"{staff.QUALIFICATION}\",\"{staff.CASTE}\",\"{staff.AADTHAR_NO}\",\"{staff.DATE_OF_ENTRY_TEACHING}\",\"{staff.PAN_NO}\",\"{staff.RETIREMENT_DATE}\",\"{staff.LEAVING_DATE}\",\"{staff.STAFF_APPROVAL_NO}\",\"{staff.MPHIL_APPROVAL_NO}\",\"{staff.PHD_APPROVAL_NO}\",\"{staff.FATHER_NAME}\",\"{staff.MOTHER_NAME}\",\"{staff.SPOUSE_NAME}\",\"{staff.ADDRESS_PRESENT}\",\"{staff.ADDRESS_PERMANENT}\",\"{staff.EPF_NO}\",\"{staff.UAN_NO}\",\"{staff.PHD_COMPLETION}\",\"{staff.NET_COMPLETION}\",\"{staff.SET_COMPLETION}\",\"{staff.MPHIL_COMPLETION}\",\"{staff.PG_COMPLETION}\",\"{staff.UG_COMPLETION}\",\"{staff.HSC_COMPLETION}\",\"{staff.SSLC_COMPLETION}\",\"{staff.UG_EXPERIENCE}\",\"{staff.PG_EXPERIENCE}\",\"{staff.MPHIL_EXPERIENCE}\",\"{staff.PHD_EXPERIENCE}\",\"{staff.PHYSICAL_STATUS}\",\"{staff.BANK_AC_NO}\",\"{staff.BANK_NAME}\",\"{staff.BRANCH_NAME}\",\"{staff.IFSC_CODE}\",\"{staff.SALARY_BASIC}\",\"{staff.SALARY_DA}\",\"{staff.PACKAGE}\",\"{staff.SPECIAL_ALLOWANCE}\",\"{staff.OTHERS_SALARY}\",\"{staff.PP_SALARY}\",\"{staff.GROSS_SALARY}\"");
            }

            return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "StaffList.csv");
        }


        public ActionResult AddStaffPersoanlInfo()
        {
            Stf_Personal_Info objStaffPersonalInfo = new Stf_Personal_Info();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Shift ...
                DataTable dtFetchShift = new DataTable();
                dtFetchShift = SupportDataMethod.FetchShift().DataSource.Table;
                List<Sup_Shift> liShift = new List<Sup_Shift>();
                if (dtFetchShift != null && dtFetchShift.Rows.Count > 0)
                {
                    liShift = (from DataRow dr in dtFetchShift.Rows
                               select new Sup_Shift()
                               {
                                   SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                   SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                               }).ToList();
                    objStaffPersonalInfo.SHIFT = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }

                // Department ...
                DataTable dtFetchDepartment = new DataTable();
                dtFetchDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                List<SupDepartment> liDepartment = new List<SupDepartment>();
                if (dtFetchDepartment != null && dtFetchDepartment.Rows.Count > 0)
                {
                    liDepartment = (from DataRow dr in dtFetchDepartment.Rows
                                    select new SupDepartment()
                                    {
                                        DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                        DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                    }).ToList();
                    objStaffPersonalInfo.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }

                // Community ...

                DataTable dtFetchCommunity = new DataTable();
                dtFetchCommunity = SupportDataMethod.FetchCommunity().DataSource.Table;
                List<SupCommunity> liCommunity = new List<SupCommunity>();
                if (dtFetchCommunity != null && dtFetchCommunity.Rows.Count > 0)
                {
                    liCommunity = (from DataRow dr in dtFetchCommunity.Rows
                                   select new SupCommunity()
                                   {
                                       COMMUNITYID = dr[Common.SUP_COMMUNITY.COMMUNITYID].ToString(),
                                       COMMUNITY = dr[Common.SUP_COMMUNITY.COMMUNITY].ToString()
                                   }).ToList();
                    objStaffPersonalInfo.COMMUNITY = new SelectList(liCommunity, Common.SUP_COMMUNITY.COMMUNITYID, Common.SUP_COMMUNITY.COMMUNITY);
                }

                // Gender ...

                DataTable dtFetchGender = new DataTable();
                dtFetchGender = SupportDataMethod.FetchGender().DataSource.Table;
                List<SupGender> liGender = new List<SupGender>();
                if (dtFetchGender != null && dtFetchGender.Rows.Count > 0)
                {
                    liGender = (from DataRow dr in dtFetchGender.Rows
                                select new SupGender()
                                {
                                    GENDER_ID = dr[Common.SUP_GENDER.GENDER_ID].ToString(),
                                    GENDER_NAME = dr[Common.SUP_GENDER.GENDER_NAME].ToString()
                                }).ToList();
                    objStaffPersonalInfo.GENDER = new SelectList(liGender, Common.SUP_GENDER.GENDER_ID, Common.SUP_GENDER.GENDER_NAME);

                }

                // MaritalStatus ...
                DataTable dtFetchMaritalStatus = new DataTable();
                dtFetchMaritalStatus = SupportDataMethod.FetchMaritalStatus().DataSource.Table;
                List<SupMaritalStatus> liMaritalStatus = new List<SupMaritalStatus>();
                if (dtFetchMaritalStatus != null && dtFetchMaritalStatus.Rows.Count > 0)
                {
                    liMaritalStatus = (from DataRow dr in dtFetchMaritalStatus.Rows
                                       select new SupMaritalStatus()
                                       {
                                           MARITAL_STAUS_ID = dr[Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID].ToString(),
                                           MARITAL_STATUS_NAME = dr[Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME].ToString()
                                       }).ToList();
                    objStaffPersonalInfo.MARITAL_STATUS = new SelectList(liMaritalStatus, Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID, Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME);
                }

                // MotherTongue ...
                DataTable dtFetchMotherTongue = new DataTable();
                dtFetchMotherTongue = SupportDataMethod.FetchMotherTongue().DataSource.Table;
                List<SupMotherTongue> liMotherTongue = new List<SupMotherTongue>();
                if (dtFetchMotherTongue != null && dtFetchMotherTongue.Rows.Count > 0)
                {
                    liMotherTongue = (from DataRow dr in dtFetchMotherTongue.Rows
                                      select new SupMotherTongue()
                                      {
                                          MOTHER_TONGUE_ID = dr[Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID].ToString(),
                                          MOTHER_TONGUE_NAME = dr[Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME].ToString()
                                      }).ToList();
                    objStaffPersonalInfo.MOTHER_TONGUE = new SelectList(liMotherTongue, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME);
                }

                // Nationality ...
                DataTable dtFetchNationality = new DataTable();
                dtFetchNationality = SupportDataMethod.FetchNationality().DataSource.Table;
                List<SupNationality> liNationality = new List<SupNationality>();
                if (dtFetchNationality != null && dtFetchNationality.Rows.Count > 0)
                {
                    liNationality = (from DataRow dr in dtFetchNationality.Rows
                                     select new SupNationality()
                                     {
                                         NATIONALITY_ID = dr[Common.SUP_NATIONALITY.NATIONALITY_ID].ToString(),
                                         NATIONALITY_NAME = dr[Common.SUP_NATIONALITY.NATIONALITY_NAME].ToString()
                                     }).ToList();
                    objStaffPersonalInfo.NATIONALITY = new SelectList(liNationality, Common.SUP_NATIONALITY.NATIONALITY_ID, Common.SUP_NATIONALITY.NATIONALITY_NAME);
                }

                // Religion ...
                DataTable dtFetchReligion = new DataTable();
                dtFetchReligion = SupportDataMethod.FetchReligion().DataSource.Table;
                List<SUP_RELIGION> liReligion = new List<SUP_RELIGION>();
                if (dtFetchReligion != null && dtFetchReligion.Rows.Count > 0)
                {
                    liReligion = (from DataRow dr in dtFetchReligion.Rows
                                  select new SUP_RELIGION()
                                  {
                                      RELIGIONID = dr[Common.SUP_RELIGION.RELIGIONID].ToString(),
                                      RELIGION = dr[Common.SUP_RELIGION.RELIGION].ToString()
                                  }).ToList();
                    objStaffPersonalInfo.RELIGION = new SelectList(liReligion, Common.SUP_RELIGION.RELIGIONID, Common.SUP_RELIGION.RELIGION);
                }

                // BloodGroup ...
                DataTable dtFetchBloodGroup = new DataTable();
                dtFetchBloodGroup = SupportDataMethod.FetchBloodGroup().DataSource.Table;
                List<SupBloodGroup> liBloodGroup = new List<SupBloodGroup>();
                if (dtFetchBloodGroup != null && dtFetchBloodGroup.Rows.Count > 0)
                {
                    liBloodGroup = (from DataRow dr in dtFetchBloodGroup.Rows
                                    select new SupBloodGroup()
                                    {
                                        BLOOD_GROUP_ID = dr[Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID].ToString(),
                                        BLOOD_GROUP_NAME = dr[Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME].ToString()
                                    }).ToList();
                    objStaffPersonalInfo.BLOOD_GROUP = new SelectList(liBloodGroup, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME);
                }
                // Sup_Role ...
                var liRole = new List<SUP_ROLE>();
                liRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchSupRole)).DataSource.Data;
                objStaffPersonalInfo.ROLE = new SelectList(liRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);

                var liCategories = (List<Sup_SubCategory>)CMSHandler.FetchData<Sup_SubCategory>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchsupCategory)).DataSource.Data;
                if (liCategories != null)
                {
                    objStaffPersonalInfo.CATEGORY = new SelectList(liCategories, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                }
                var liDesignation = (List<Sup_Designation>)CMSHandler.FetchData<Sup_Designation>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupDesignation)).DataSource.Data;
                if (liDesignation != null)
                {
                    objStaffPersonalInfo.DESIGNATION = new SelectList(liDesignation, Common.sup_designation.DESIGNATIONID, Common.sup_designation.DESIGNATION);
                }
                var liPhysicalStatus = (List<Sup_Physical_Status>)CMSHandler.FetchData<Sup_Physical_Status>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupPhysicalStatus)).DataSource.Data;
                if (liPhysicalStatus != null)
                {
                    objStaffPersonalInfo.PHYSICAL_STATUS = new SelectList(liPhysicalStatus, Common.sup_physical_status.PHYSICAL_STATUS_ID, Common.sup_physical_status.PHYSICAL_STATUS_NAME);
                }
                var liStaffStatus = (List<Sup_Staff_Status>)CMSHandler.FetchData<Sup_Staff_Status>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupStaffStatus)).DataSource.Data;
                if (liStaffStatus != null)
                {
                    objStaffPersonalInfo.STAFF_STATUS = new SelectList(liStaffStatus, Common.sup_Staff_Status.STATUS_ID, Common.sup_Staff_Status.STATUS);
                }
                // Sup_Role ...
                var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (liAcYear != null)
                {
                    objStaffPersonalInfo.ACADEMIC_YEAR = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }

                // UserType ...
                var liUserType = new List<SUP_USER_TYPE>();
                liUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserType)).DataSource.Data;
                objStaffPersonalInfo.USER_TYPE = new SelectList(liUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);
            }

            return View(objStaffPersonalInfo);
        }
        public JsonResult SaveStaffPersonal(string str, string sBaseImage, string sExtension)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            var objAccountInfo = new USER_ACCOUNT_INFO();
            try
            {
                if (str != null)
                {
                    var Result = JsonConvert.DeserializeObject<STF_PERSONAL_INFO>(str);
                    Result.DATE_OF_BIRTH = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_BIRTH);
                    Result.DATE_OF_JOIN = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_JOIN);
                    Result.DATE_OF_ENTRY_TEACHING = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_ENTRY_TEACHING);
                    Result.RETIREMENT_DATE = CommonMethods.ConvertdatetoyearFromat(Result.RETIREMENT_DATE);
                    Result.LEAVING_DATE = CommonMethods.ConvertdatetoyearFromat(Result.LEAVING_DATE);
                    Result.DATE_OF_EPF = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_EPF);

                    objAccountInfo.ACADEMIC_YEAR = sAcademicYear;
                    objAccountInfo.EMAIL = Result.EMAIL;
                    Result.STAFF_CODE = Result.STAFF_CODE.ToUpper();
                    Result.BAR_CODE = Result.STAFF_CODE;
                    Result.RFID = Result.STAFF_CODE;
                    objAccountInfo.USERNAME = Result.STAFF_CODE.ToUpper();
                    objAccountInfo.PASSWORD = CommonMethods.GetSha256FromString(Result.STAFF_CODE.ToUpper() + "@" + Result.DATE_OF_BIRTH.Substring(0, 4));

                    objAccountInfo.ROLE = Result.ROLE;
                    objAccountInfo.USER_TYPE = Result.USER_TYPE;
                    objAccountInfo.ROLE_ID = Result.ROLE;

                    objAccountInfo.NAME = Result.FIRST_NAME + " " + Result.LAST_NAME;

                    if (!string.IsNullOrEmpty(sBaseImage) && (!string.IsNullOrEmpty(sExtension)))
                    {
                        Result.PHOTO = SaveImage(sBaseImage, sExtension, Result.STAFF_CODE);
                        objAccountInfo.PHOTO = SaveImage(sBaseImage, sExtension, Result.STAFF_CODE);
                    }
                    else
                        Result.PHOTO = "";



                    var liExits = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(Result, StaffSQL.GetStaffSQL(StaffSQLCommands.PersonalIfExits)).DataSource.Data;
                    if (liExits != null && liExits.Count > 0)
                    {
                        ObjJsonData.Message = Common.ErrorCode.Conflict;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        var InsertStaffInfo = CMSHandler.SaveOrUpdate(Result, StaffSQL.GetStaffSQL(StaffSQLCommands.SaveStaffPersonalInfo), "", true);
                        if (InsertStaffInfo.Success)
                        {
                            string sStaffID = InsertStaffInfo.RowUniqueId.ToString();
                            Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] = sStaffID;
                            objAccountInfo.USER_ID = sStaffID;


                            // Insert Stu Class ...
                            // Insert User Account ...

                            var InsertUserAccount = CMSHandler.SaveOrUpdate(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.SaveUserAccount));
                            if (InsertUserAccount.Success)
                            {
                                var InsertUserRolebyAcYear = CMSHandler.SaveOrUpdate(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.SaveUserRoleByAcademicYear));

                            }
                            ObjJsonData.Message = (InsertUserAccount.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("staffController", "SaveStaffPersonal", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("staffController", "SaveStaffPersonal", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditStaffInfomationByID(string Id, bool fromSelfEdit = false)
        {
            string sRoleName = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString().ToUpper().Replace(" ", "") : string.Empty;
            bool isSelfEdit = fromSelfEdit || sRoleName == "STAFF";
            if (isSelfEdit)
            {
                ViewBag.BackUrl = "/Staff/StaffSelfDetailsForEdit";
                ViewBag.FromSelfEdit = true;
            }
            else
            {
                ViewBag.BackUrl = "/Staff/StaffDetailsforEdit";
                ViewBag.FromSelfEdit = false;
            }
            Session["IS_SELF_EDIT"] = isSelfEdit;
            Session[Common.SESSION_VARIABLES.STAFF_ID] = Id;
            return View();
        }

        public ActionResult EditStaffPersoanlInfobyId(bool fromSelfEdit = false)
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            string sRoleName = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString().ToUpper().Replace(" ", "") : string.Empty;
            bool isSelfEdit = fromSelfEdit || sRoleName == "STAFF" || (Session["IS_SELF_EDIT"] != null && (bool)Session["IS_SELF_EDIT"]);
            ViewBag.IsSalaryReadOnly = isSelfEdit;
            Stf_Personal_Info objStaffPersonalInfo = new Stf_Personal_Info();
            if (!string.IsNullOrEmpty(sStaffID))
            {
                dv.Clear();
                dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
                DataTable dtFetchRecordbyId = new DataTable();
                StaffViewModel objStaff = new StaffViewModel();
                dtFetchRecordbyId = objStaff.FetchRecordById(dv).DataSource.Table;

                // Shift ...
                DataTable dtFetchShift = new DataTable();
                dtFetchShift = SupportDataMethod.FetchShift().DataSource.Table;
                List<Sup_Shift> liShift = new List<Sup_Shift>();
                if (dtFetchShift != null && dtFetchShift.Rows.Count > 0)
                {
                    liShift = (from DataRow dr in dtFetchShift.Rows
                               select new Sup_Shift()
                               {
                                   SHIFT_ID = dr[Common.SUP_SHIFT.SHIFT_ID].ToString(),
                                   SHIFT_NAME = dr[Common.SUP_SHIFT.SHIFT_NAME].ToString()
                               }).ToList();
                    objStaffPersonalInfo.SHIFT = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }

                // Department ...
                DataTable dtFetchDepartment = new DataTable();
                dtFetchDepartment = SupportDataMethod.FetchDepartment((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table;
                List<SupDepartment> liDepartment = new List<SupDepartment>();
                if (dtFetchDepartment != null && dtFetchDepartment.Rows.Count > 0)
                {
                    liDepartment = (from DataRow dr in dtFetchDepartment.Rows
                                    select new SupDepartment()
                                    {
                                        DEPARTMENT_ID = dr[Common.SUP_DEPARTMENT.DEPARTMENT_ID].ToString(),
                                        DEPARTMENT = dr[Common.SUP_DEPARTMENT.DEPARTMENT].ToString()
                                    }).ToList();
                    objStaffPersonalInfo.DEPARTMENT = new SelectList(liDepartment, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }

                // Community ...

                DataTable dtFetchCommunity = new DataTable();
                dtFetchCommunity = SupportDataMethod.FetchCommunity().DataSource.Table;
                List<SupCommunity> liCommunity = new List<SupCommunity>();
                if (dtFetchCommunity != null && dtFetchCommunity.Rows.Count > 0)
                {
                    liCommunity = (from DataRow dr in dtFetchCommunity.Rows
                                   select new SupCommunity()
                                   {
                                       COMMUNITYID = dr[Common.SUP_COMMUNITY.COMMUNITYID].ToString(),
                                       COMMUNITY = dr[Common.SUP_COMMUNITY.COMMUNITY].ToString()
                                   }).ToList();
                    objStaffPersonalInfo.COMMUNITY = new SelectList(liCommunity, Common.SUP_COMMUNITY.COMMUNITYID, Common.SUP_COMMUNITY.COMMUNITY);
                }

                // Gender ...

                DataTable dtFetchGender = new DataTable();
                dtFetchGender = SupportDataMethod.FetchGender().DataSource.Table;
                List<SupGender> liGender = new List<SupGender>();
                if (dtFetchGender != null && dtFetchGender.Rows.Count > 0)
                {
                    liGender = (from DataRow dr in dtFetchGender.Rows
                                select new SupGender()
                                {
                                    GENDER_ID = dr[Common.SUP_GENDER.GENDER_ID].ToString(),
                                    GENDER_NAME = dr[Common.SUP_GENDER.GENDER_NAME].ToString()
                                }).ToList();
                    objStaffPersonalInfo.GENDER = new SelectList(liGender, Common.SUP_GENDER.GENDER_ID, Common.SUP_GENDER.GENDER_NAME);

                }

                // MaritalStatus ...
                DataTable dtFetchMaritalStatus = new DataTable();
                dtFetchMaritalStatus = SupportDataMethod.FetchMaritalStatus().DataSource.Table;
                List<SupMaritalStatus> liMaritalStatus = new List<SupMaritalStatus>();
                if (dtFetchMaritalStatus != null && dtFetchMaritalStatus.Rows.Count > 0)
                {
                    liMaritalStatus = (from DataRow dr in dtFetchMaritalStatus.Rows
                                       select new SupMaritalStatus()
                                       {
                                           MARITAL_STAUS_ID = dr[Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID].ToString(),
                                           MARITAL_STATUS_NAME = dr[Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME].ToString()
                                       }).ToList();
                    objStaffPersonalInfo.MARITAL_STATUS = new SelectList(liMaritalStatus, Common.SUP_MARRITAL_STATUS.MARITAL_STAUS_ID, Common.SUP_MARRITAL_STATUS.MARITAL_STATUS_NAME);
                }

                // MotherTongue ...
                DataTable dtFetchMotherTongue = new DataTable();
                dtFetchMotherTongue = SupportDataMethod.FetchMotherTongue().DataSource.Table;
                List<SupMotherTongue> liMotherTongue = new List<SupMotherTongue>();
                if (dtFetchMotherTongue != null && dtFetchMotherTongue.Rows.Count > 0)
                {
                    liMotherTongue = (from DataRow dr in dtFetchMotherTongue.Rows
                                      select new SupMotherTongue()
                                      {
                                          MOTHER_TONGUE_ID = dr[Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID].ToString(),
                                          MOTHER_TONGUE_NAME = dr[Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME].ToString()
                                      }).ToList();
                    objStaffPersonalInfo.MOTHER_TONGUE = new SelectList(liMotherTongue, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_ID, Common.SUP_MOTHER_TONGUE.MOTHER_TONGUE_NAME);
                }

                // Nationality ...
                DataTable dtFetchNationality = new DataTable();
                dtFetchNationality = SupportDataMethod.FetchNationality().DataSource.Table;
                List<SupNationality> liNationality = new List<SupNationality>();
                if (dtFetchNationality != null && dtFetchNationality.Rows.Count > 0)
                {
                    liNationality = (from DataRow dr in dtFetchNationality.Rows
                                     select new SupNationality()
                                     {
                                         NATIONALITY_ID = dr[Common.SUP_NATIONALITY.NATIONALITY_ID].ToString(),
                                         NATIONALITY_NAME = dr[Common.SUP_NATIONALITY.NATIONALITY_NAME].ToString()
                                     }).ToList();
                    objStaffPersonalInfo.NATIONALITY = new SelectList(liNationality, Common.SUP_NATIONALITY.NATIONALITY_ID, Common.SUP_NATIONALITY.NATIONALITY_NAME);
                }

                // Religion ...
                DataTable dtFetchReligion = new DataTable();
                dtFetchReligion = SupportDataMethod.FetchReligion().DataSource.Table;
                List<SUP_RELIGION> liReligion = new List<SUP_RELIGION>();
                if (dtFetchReligion != null && dtFetchReligion.Rows.Count > 0)
                {
                    liReligion = (from DataRow dr in dtFetchReligion.Rows
                                  select new SUP_RELIGION()
                                  {
                                      RELIGIONID = dr[Common.SUP_RELIGION.RELIGIONID].ToString(),
                                      RELIGION = dr[Common.SUP_RELIGION.RELIGION].ToString()
                                  }).ToList();
                    objStaffPersonalInfo.RELIGION = new SelectList(liReligion, Common.SUP_RELIGION.RELIGIONID, Common.SUP_RELIGION.RELIGION);
                }

                // BloodGroup ...
                DataTable dtFetchBloodGroup = new DataTable();
                dtFetchBloodGroup = SupportDataMethod.FetchBloodGroup().DataSource.Table;
                List<SupBloodGroup> liBloodGroup = new List<SupBloodGroup>();
                if (dtFetchBloodGroup != null && dtFetchBloodGroup.Rows.Count > 0)
                {
                    liBloodGroup = (from DataRow dr in dtFetchBloodGroup.Rows
                                    select new SupBloodGroup()
                                    {
                                        BLOOD_GROUP_ID = dr[Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID].ToString(),
                                        BLOOD_GROUP_NAME = dr[Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME].ToString()
                                    }).ToList();
                    objStaffPersonalInfo.BLOOD_GROUP = new SelectList(liBloodGroup, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_ID, Common.SUP_BLOOD_GROUP.BLOOD_GROUP_NAME);
                }
                // Sup_Role ...
                var liRole = new List<SUP_ROLE>();
                liRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchSupRole)).DataSource.Data;
                objStaffPersonalInfo.ROLE = new SelectList(liRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);

                var liCategories = (List<Sup_SubCategory>)CMSHandler.FetchData<Sup_SubCategory>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchsupCategory)).DataSource.Data;
                if (liCategories != null)
                {
                    objStaffPersonalInfo.CATEGORY = new SelectList(liCategories, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                }
                var liDesignation = (List<Sup_Designation>)CMSHandler.FetchData<Sup_Designation>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupDesignation)).DataSource.Data;
                if (liDesignation != null)
                {
                    objStaffPersonalInfo.DESIGNATION = new SelectList(liDesignation, Common.sup_designation.DESIGNATIONID, Common.sup_designation.DESIGNATION);
                }
                var liStaffStatus = (List<Sup_Staff_Status>)CMSHandler.FetchData<Sup_Staff_Status>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupStaffStatus)).DataSource.Data;
                if (liStaffStatus != null)
                {
                    objStaffPersonalInfo.STAFF_STATUS = new SelectList(liStaffStatus, Common.sup_Staff_Status.STATUS_ID, Common.sup_Staff_Status.STATUS);
                }
                var liPhysicalStatus = (List<Sup_Physical_Status>)CMSHandler.FetchData<Sup_Physical_Status>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupPhysicalStatus)).DataSource.Data;
                if (liPhysicalStatus != null)
                {
                    objStaffPersonalInfo.PHYSICAL_STATUS = new SelectList(liPhysicalStatus, Common.sup_physical_status.PHYSICAL_STATUS_ID, Common.sup_physical_status.PHYSICAL_STATUS_NAME);
                }
                var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                if (liAcYear != null)
                {
                    objStaffPersonalInfo.ACADEMIC_YEAR = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                // UserType ...
                var liUserType = new List<SUP_USER_TYPE>();
                liUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserType)).DataSource.Data;
                objStaffPersonalInfo.USER_TYPE = new SelectList(liUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);
            }

            return View(objStaffPersonalInfo);
        }
        public string UpdateStaffPersonal()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var str = Request.Form["str"];
            string filePath = string.Empty;
            string sImagePath = string.Empty;
            string _sDirectorypath = string.Empty;
            STF_PERSONAL_INFO objModel = new STF_PERSONAL_INFO();
            if (str != null)
            {

                _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STAFF_IMGAE_PATH;

                if (!Directory.Exists(_sDirectorypath))
                {
                    Directory.CreateDirectory(_sDirectorypath);
                }

                var Result = JsonConvert.DeserializeObject<STF_PERSONAL_INFO>(str);
                Result.DATE_OF_BIRTH = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_BIRTH);
                Result.DATE_OF_JOIN = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_JOIN);
                Result.DATE_OF_ENTRY_TEACHING = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_ENTRY_TEACHING);
                Result.RETIREMENT_DATE = CommonMethods.ConvertdatetoyearFromat(Result.RETIREMENT_DATE);
                Result.LEAVING_DATE = CommonMethods.ConvertdatetoyearFromat(Result.LEAVING_DATE);
                Result.DATE_OF_EPF = CommonMethods.ConvertdatetoyearFromat(Result.DATE_OF_EPF);

                Result.STAFF_ID = sStaffID;
                Result.STAFF_CODE = Result.STAFF_CODE.ToUpper();
                Result.BAR_CODE = Result.STAFF_CODE;
                Result.RFID = Result.STAFF_CODE;


                if (Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] != null)
                {
                    using (StaffViewModel objStaff = new StaffViewModel())
                    {
                        // Image Updation ....
                        var Rollno = Request.Form["STAFF_CODE"];
                        if (Request.Files.Count > 0)
                        {
                            var File = Request.Files[0];
                            string ImgName = File.FileName;
                            var Img = ImgName.Split('.');
                            string fileName = ImgName.Replace(Img[0], Result.STAFF_CODE);

                            if (!string.IsNullOrEmpty(fileName))
                            {
                                filePath = _sDirectorypath + fileName;
                                sImagePath = "\\" + Common.URLPages.STAFF_IMGAE_PATH + fileName;
                                File.SaveAs(filePath);
                                Result.PHOTO = sImagePath;

                            }
                            string sSql = StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffImage).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID);
                            objModel.PHOTO = Result.PHOTO;
                            var UpdatePhoto = CMSHandler.SaveOrUpdate(objModel, sSql);
                        }

                        string sUserRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString().ToUpper().Replace(" ", "") : string.Empty;
                        bool isSelfEditSave = sUserRole == "STAFF" || (Session["IS_SELF_EDIT"] != null && (bool)Session["IS_SELF_EDIT"]);
                        if (isSelfEditSave)
                        {
                            dv.Clear();
                            dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
                            DataTable dtExistingStaff = objStaff.FetchStaffDetailsById(dv).DataSource.Table;
                            if (dtExistingStaff != null && dtExistingStaff.Rows.Count > 0)
                            {
                                Result.SALARY_BASIC = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.SALARY_BASIC].ToString();
                                Result.SALARY_DA = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.SALARY_DA].ToString();
                                Result.PACKAGE = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.PACKAGE].ToString();
                                Result.SPECIAL_ALLOWANCE = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.SPECIAL_ALLOWANCE].ToString();
                                Result.OTHERS_SALARY = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.OTHERS_SALARY].ToString();
                                Result.PP_SALARY = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.PP_SALARY].ToString();
                                Result.GROSS_SALARY = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.GROSS_SALARY].ToString();

                                Result.STAFF_CODE = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.STAFF_CODE].ToString();
                                Result.CATEGORY = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.CATEGORY].ToString();
                                Result.STAFF_STATUS = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.STAFF_STATUS].ToString();
                                Result.IS_LEFT = dtExistingStaff.Rows[0][Common.STF_PERSONAL_INFO.IS_LEFT].ToString();

                                var ObjUserAcc = new USER_ACCOUNT_INFO();
                                ObjUserAcc.USERNAME = Result.STAFF_CODE;
                                var liUserAcc = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(ObjUserAcc, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchUserAccountByStaffCode)).DataSource.Data;
                                if (liUserAcc != null && liUserAcc.Count > 0)
                                {
                                    Result.ROLE = liUserAcc.FirstOrDefault().ROLE;
                                    Result.USER_TYPE = liUserAcc.FirstOrDefault().USER_TYPE;
                                }
                            }
                        }

                        resultArgs = CMSHandler.SaveOrUpdate(Result, StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffPersonalInfo));
                        if (resultArgs.Success)
                        {
                            var objAccountInfo = new USER_ACCOUNT_INFO();
                            var liAccountInfo = new List<USER_ACCOUNT_INFO>();
                            var liSuPAccountDetails = new List<SUP_USER>();

                            objAccountInfo.NAME = Result.FIRST_NAME;
                            objAccountInfo.USERNAME = Result.STAFF_CODE.ToUpper();
                            objAccountInfo.PASSWORD = CommonMethods.GetSha256FromString(Result.STAFF_CODE.ToUpper() + "@" + Result.DATE_OF_BIRTH.Substring(0, 4));
                            objAccountInfo.USER_ID = sStaffID;
                            objAccountInfo.ROLE = Result.ROLE;
                            objAccountInfo.ROLE_ID = Result.ROLE;
                            objAccountInfo.USER_TYPE = Result.USER_TYPE;
                            objAccountInfo.PHOTO = Result.PHOTO;
                            objAccountInfo.MOBILE = Result.MOBILE_NO;
                            objAccountInfo.EMAIL = Result.EMAIL;
                            objAccountInfo.ACADEMIC_YEAR = sAcademicYear;

                            liAccountInfo = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserAccountByRoleAndUserId)).DataSource.Data;
                            if (liAccountInfo != null && liAccountInfo.Count > 0)
                            {
                                objAccountInfo.USER_ACCOUNT_ID = liAccountInfo.FirstOrDefault().USER_ACCOUNT_ID;
                                resultArgs = CMSHandler.SaveOrUpdate(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.UpdateUserAccountWithoutPassword));

                                sResult = (resultArgs.Success) ? "Record updated successfully ...!" : "Record not updated try again ...!";
                            }
                            else
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.SaveUserAccount), "", true);
                                //sResult = "Please verify all the Details are filled correctly  ...!";
                            }
                            var userRoleByACYear = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserRolebyAcYearRoleAndUserIdAndAcYear)).DataSource.Data;
                            if (userRoleByACYear != null && userRoleByACYear.Count > 0)
                            {
                            }
                            else
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(objAccountInfo, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.SaveUserRoleByAcademicYear));

                            }
                        }
                        else
                        {
                            sResult = "Record not updated try again ...!";
                        }

                    }
                }
                else
                {
                    using (StaffViewModel objStaffView = new StaffViewModel())
                    {
                        resultArgs = objStaffView.InsertStaffPersonal(dv);
                        if (resultArgs.Success)
                        {
                            Session[Common.SESSION_VARIABLES.LAST_INSERT_ID] = resultArgs.RowUniqueId.ToString();
                            sResult = "Record saved successfully ...!";
                        }
                        else
                        {
                            sResult = "Record not saved try again ...!";
                        }
                    }
                }

            }
            return sResult;
        }
        public string DeleteStaffPersonalInfo(string sStaffId, string UserName)
        {
            dv.Clear();
            dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffId, EnumCommand.DataType.String);
            dv.Add(Common.STF_PERSONAL_INFO.IS_DELETED, "1", EnumCommand.DataType.String);
            var ObjUserAccount = new USER_ACCOUNT_INFO();
            var liUserAccount = new List<USER_ACCOUNT_INFO>();
            ObjUserAccount.USERNAME = UserName;
            liUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(ObjUserAccount, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchUserAccountByUserIdandName)).DataSource.Data;
            if (liUserAccount != null && liUserAccount.Count > 0)
            {
                ObjUserAccount.USER_ACCOUNT_ID = liUserAccount.FirstOrDefault().USER_ACCOUNT_ID;
            }
            resultArgs = CMSHandler.SaveOrUpdate(ObjUserAccount, SQL.UserAccount.UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.DeleteUserAccount));
            using (StaffViewModel objStaff = new StaffViewModel())
            {
                resultArgs = objStaff.DeleteStaffInfo(dv);
                if (resultArgs.Success)
                {
                    sResult = "Record deleted successfully ...!";
                }
                else
                {
                    sResult = "Record deleted successfully ..!";
                }
            }
            return sResult;
        }

        public JsonResult FetchStfPersonalInfo()
        {
            string sStaffID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            dv.Clear();
            dv.Add(Common.STF_PERSONAL_INFO.STAFF_ID, sStaffID, EnumCommand.DataType.String);
            DataTable dtFetchRecordbyId = new DataTable();
            var ObjUserAccount = new USER_ACCOUNT_INFO();
            StaffViewModel objStaff = new StaffViewModel();
            dtFetchRecordbyId = objStaff.FetchStaffDetailsById(dv).DataSource.Table;
            if (dtFetchRecordbyId != null && dtFetchRecordbyId.Rows.Count > 0)
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.STAFF_ID].ToString();
                STF_PERSONAL_INFO objstaffInfo = new STF_PERSONAL_INFO()
                {
                    FIRST_NAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.FIRST_NAME].ToString(),
                    LAST_NAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.LAST_NAME].ToString(),
                    KNOWN_AS = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.KNOWN_AS].ToString(),
                    DATE_OF_BIRTH = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DATE_OF_BIRTH].ToString(),
                    PLACE_OF_BIRTH = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PLACE_OF_BIRTH].ToString(),
                    DATE_OF_JOIN = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DATE_OF_JOIN].ToString(),
                    MARITAL_STATUS = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MARITAL_STATUS].ToString(),
                    GENDER = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.GENDER].ToString(),
                    DEPARTMENT = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DEPARTMENT].ToString(),
                    COMMUNITY = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.COMMUNITY].ToString(),
                    NATIONALITY = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.NATIONALITY].ToString(),
                    RELIGION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.RELIGION].ToString(),
                    MOTHER_TONGUE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MOTHER_TONGUE].ToString(),
                    BLOOD_GROUP = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.BLOOD_GROUP].ToString(),
                    STAFF_CODE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.STAFF_CODE].ToString(),
                    SHIFT = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.SHIFT].ToString(),
                    PHOTO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PHOTO].ToString(),
                    IS_LEFT = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.IS_LEFT].ToString(),
                    CASTE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.CASTE].ToString(),
                    AADTHAR_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.AADTHAR_NO].ToString(),
                    DATE_OF_ENTRY_TEACHING = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DATE_OF_ENTRY_TEACHING].ToString(),
                    PAN_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PAN_NO].ToString(),
                    STAFF_APPROVAL_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.STAFF_APPROVAL_NO].ToString(),
                    MPHIL_APPROVAL_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MPHIL_APPROVAL_NO].ToString(),
                    PHD_APPROVAL_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PHD_APPROVAL_NO].ToString(),
                    FATHER_NAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.FATHER_NAME].ToString(),
                    MOTHER_NAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MOTHER_NAME].ToString(),
                    ADDRESS_PRESENT = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.ADDRESS_PRESENT].ToString(),
                    SPOUSE_NAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.SPOUSE_NAME].ToString(),
                    ADDRESS_PERMANENT = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.ADDRESS_PERMANENT].ToString(),
                    UAN_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.UAN_NO].ToString(),
                    EPF_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.EPF_NO].ToString(),
                    PHD_COMPLETION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PHD_COMPLETION].ToString(),
                    NET_COMPLETION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.NET_COMPLETION].ToString(),
                    SET_COMPLETION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.SET_COMPLETION].ToString(),
                    MPHIL_COMPLETION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MPHIL_COMPLETION].ToString(),
                    UG_COMPLETION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.UG_COMPLETION].ToString(),
                    PG_COMPLETION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PG_COMPLETION].ToString(),
                    SSLC_COMPLETION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.SSLC_COMPLETION].ToString(),
                    HSC_COMPLETION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.HSC_COMPLETION].ToString(),
                    UG_EXPERIENCE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.UG_EXPERIENCE].ToString(),
                    PG_EXPERIENCE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PG_EXPERIENCE].ToString(),
                    PHD_EXPERIENCE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PHD_EXPERIENCE].ToString(),
                    MPHIL_EXPERIENCE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MPHIL_EXPERIENCE].ToString(),
                    BANK_AC_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.BANK_AC_NO].ToString(),
                    BANK_NAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.BANK_NAME].ToString(),
                    BRANCH_NAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.BRANCH_NAME].ToString(),
                    IFSC_CODE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.IFSC_CODE].ToString(),
                    SALARY_BASIC = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.SALARY_BASIC].ToString(),
                    SALARY_DA = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MPHIL_EXPERIENCE].ToString(),
                    PACKAGE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.SALARY_DA].ToString(),
                    SPECIAL_ALLOWANCE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.SPECIAL_ALLOWANCE].ToString(),
                    OTHERS_SALARY = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.OTHERS_SALARY].ToString(),
                    PP_SALARY = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PP_SALARY].ToString(),
                    GROSS_SALARY = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.GROSS_SALARY].ToString(),
                    GSUITE_MAIL_ID = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.GSUITE_MAIL_ID].ToString(),
                    MOBILE_NO = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.MOBILE_NO].ToString(),
                    QUALIFICATION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.QUALIFICATION].ToString(),
                    EMAIL = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.EMAIL].ToString(),
                    DESIGNATION = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DESIGNATION].ToString(),
                    RETIREMENT_DATE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.RETIREMENT_DATE].ToString(),
                    LEAVING_DATE = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.LEAVING_DATE].ToString(),
                    DATE_OF_EPF = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.DATE_OF_EPF].ToString(),
                    CATEGORY = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.CATEGORY].ToString(),
                    PHYSICAL_STATUS = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.PHYSICAL_STATUS].ToString(),
                    STAFF_STATUS = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.STAFF_STATUS].ToString(),

                };
                ObjUserAccount.USERNAME = dtFetchRecordbyId.Rows[0][Common.STF_PERSONAL_INFO.STAFF_CODE].ToString();
                var liUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(ObjUserAccount, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchUserAccountByStaffCode)).DataSource.Data;
                if (liUserAccount != null && liUserAccount.Count > 0)
                {
                    objstaffInfo.ROLE = liUserAccount.FirstOrDefault().ROLE;
                    objstaffInfo.USER_TYPE = liUserAccount.FirstOrDefault().USER_TYPE;
                }
                return Json(objstaffInfo);
            }
            else
            {
                Session[Common.SESSION_VARIABLES.TABLE_PRIMARY_ID] = null;
                return Json(sResult);
            }
        }
        #endregion

        #region Payslip Month wise For Staff
        public ActionResult MonthWisePayslipForStaff()
        {
            StaffViewModel objViewModel = new StaffViewModel();

            try
            {

                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    var AcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (AcademicYear != null && AcademicYear.Count > 0)
                    {
                        objViewModel.AcademicYear = new SelectList(AcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var supMonth = (List<SUP_MONTHS>)CMSHandler.FetchData<SUP_MONTHS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupMonths)).DataSource.Data;
                    if (supMonth != null && supMonth.Count > 0)
                    {
                        objViewModel.MonthList = new SelectList(supMonth, Common.SUP_MONTHS.MONTH_ID, Common.SUP_MONTHS.MONTH_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FeeStudentAccountCreation", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindPayslipByAcYearAndMonth(string sMonth, string Academicyear)
        {
            //string sFrequency,
            StaffViewModel objViewModel = new StaffViewModel();
            FEE_STRUCTURE objModel = new FEE_STRUCTURE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                    objViewModel.liStfPayslip = (List<STF_PAYSLIP>)CMSHandler.FetchData<STF_PAYSLIP>(new STF_PAYSLIP() { STAFF_ID = sUserId }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffPayslipMonthWise), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindStudentsByAcademicYearAndClassId", "Fee", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region Staff Report For  Office

        /// <summary>
        /// Author : Yesudass 
        /// Date : 21 April 2025
        /// Description: New Developement Community Report  for Staff
        /// </summary>
        /// <returns></returns>
        public ActionResult StaffReportsForOffice()
        {
            string sAcademicYear = string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var acadmicplusOne = Convert.ToInt64(sAcademicYear) + 1;
                    ViewBag.AcademicYear = sAcademicYear + "-" + acadmicplusOne;
                    StaffViewModel objTransferCertificate = new StaffViewModel();
                    List<SUP_STAFF_CATEGORY> listaffCategory = new List<SUP_STAFF_CATEGORY>();
                    List<cp_Department_2017> liDepartmentList = new List<cp_Department_2017>();
                    listaffCategory = (List<SUP_STAFF_CATEGORY>)CMS.DAO.CMSHandler.FetchData<SUP_STAFF_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffCategory)).DataSource.Data;
                    if (listaffCategory != null && listaffCategory.Count > 0)
                    {
                        objTransferCertificate.LiStfCategory = new SelectList(listaffCategory, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);

                    }
                    liDepartmentList = (List<cp_Department_2017>)CMS.DAO.CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;
                    if (liDepartmentList != null && liDepartmentList.Count > 0)
                    {
                        objTransferCertificate.DepartmentList = new SelectList(liDepartmentList, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);

                    }
                    // Program ...
                    return View(objTransferCertificate);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StudentController", "TransferCertificateSerialNo", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StudentController", "TransferCertificateSerialNo", ex.Message);
                    }
                    sResult = "Fail To Fetch Record(s) ...!";
                    return Json(sResult, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }

        public ActionResult BindCommunityForStaff(string sStfCategory)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StaffViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sStfCategory))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null).DataSource.Data;

                    ObjViewModel.liCommunityReport = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffListForCommunity).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.CATEGORY, sStfCategory), null).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "BindCommunityForStaff", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindDepartmentWiseStaffList(string sDepartment)
        {
            //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new StaffViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sDepartment))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null).DataSource.Data;

                    ObjViewModel.liCommunityReport = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffDetailsByDepartmentId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.DEPARTMENT, sDepartment), null).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("StaffController", "BindCommunityForStaff", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        #endregion

        #region Staff Personal Information
        public ActionResult PersonalInformationforStaff()
        {
            StaffViewModel ObjViewModel = new StaffViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sRoleid = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (sRoleid == Common.SupRole.STAFF)
                    {
                        string sSQL = string.Empty;
                        sSQL = StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentDetailsbyStaffId);
                        ObjViewModel.liStaffReport = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO() { STAFF_ID = sUserId }, sSQL, sAcademicYear).DataSource.Data;
                    }
                 

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffController", "PersonalInformationForStaff", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StaffController", "PersonalInformationForStaff", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(ObjViewModel);
        }
        #endregion


    }
}