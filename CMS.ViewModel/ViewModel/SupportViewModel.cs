using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Utility;
using CMS.SQL.SupportData;
using CMS.DAO;
using System.Web.Mvc;
using CMS.ViewModel.Model;

namespace CMS.ViewModel.ViewModel
{
    public class SupUserType {
        public List<SUP_USER_TYPE> liUserType = new List<SUP_USER_TYPE>();
        //public List<BarCharData> liChartJs = new List<BarCharData>();
        public object testObj { get; set; }
    }
    public class SupFeeMainHead
    {
        public List<SUP_FEE_MAIN_HEAD> lstSupFeeMainHead = new List<SUP_FEE_MAIN_HEAD>();
        public SUP_FEE_MAIN_HEAD  FeeMainHead= new SUP_FEE_MAIN_HEAD();
    }
    public class SupBank
    {
        public List<SUP_BANK> lstSupBank = new List<SUP_BANK>();
        public SUP_BANK Bank = new SUP_BANK();
    }
    public class SupBankAccount
    {
        public List<SUP_BANK_ACCOUNT> lstSupBankAccount = new List<SUP_BANK_ACCOUNT>();
        public SelectList BankList { get; set; }
        public SUP_BANK_ACCOUNT BankAccount = new SUP_BANK_ACCOUNT();
    }
    public class SubFrequencyViewModel
    {
        public List<SUP_FEE_FREQUENCY> lstSupFeeFrequency { get; set; }
        public SUP_FEE_FREQUENCY SubFrequency = new SUP_FEE_FREQUENCY();
    }
    public class Gender
    {
        [Key]
        public string GenderId { get; set; }
        public string GenderName { get; set; }
        public SelectList GenderList { get; set; }
    }

    public class BloodGroup
    {
        [Key]
        public string BloodGroupId { get; set; }
        public string BloodGroupName { get; set; }
    }

    public static class SupportDataMethod
    {

        static string sSQL = string.Empty;
        static ResultArgs resultArg = new ResultArgs();
        
        //this method to fetch course items
        public static ResultArgs FetchCourseItems(string sAcademicYear)
        {
           
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseType);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchSemester()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchClassType()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassType);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchClassLevel()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchClassYear()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchNMECourses(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllCourseList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        //This method used to fetch department
        public static ResultArgs FetchDepartment(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;

        }
        //public static ResultArgs FetchOptionName()
        //{
        //    using (MySQLHandler objHandler = new MySQLHandler())
        //    {
        //        sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOptionName);
        //        resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
        //    }
        //    return resultArg;
        //}
        public static ResultArgs FetchPaperCode()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchPaperCode);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchAcademicYearList()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        //public static ResultArgs FetchIsNME()
        //{
        //    using (MySQLHandler objHandler = new MySQLHandler())
        //    {
        //        sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchIsNME);
        //        resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
        //    }
        //    return resultArg;
        //}
        //public static ResultArgs FetchIsAllied()
        //{
        //    using (MySQLHandler objHandler = new MySQLHandler())
        //    {
        //        sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchIsAllied);
        //        resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
        //    }
        //    return resultArg;
        //}
        //public static ResultArgs FetchIsCompulsory()
        //{
        //    using (MySQLHandler objHandler = new MySQLHandler())
        //    {
        //        sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchIsCompulsory);
        //        resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
        //    }
        //    return resultArg;
        //}
        public static ResultArgs FetchUGCCoursetype()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUGCCoursetype);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchCourses(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourse);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        public static ResultArgs FetchAllCourseList(string sAcademicYear) {

            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllCourseList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR,sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchOnlyNMECoursesList(string sAcademicYear)
        {

            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOnlyNMECoursesList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchOnlyAlliedOptionalCoursesList(string sAcademicYear)
        {

            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOnlyAlliedOptionalCoursesList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchOnlyRegularCoursesList(string sAcademicYear)
        {

            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOnlyRegularCoursesList);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchStaff()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffConcat);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchTeachingStaffOnly()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTeachingStaffOnlyForTT);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        //This Method Used to Fetch Nationality
        public static ResultArgs FetchNationality()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchNationality);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }

            return resultArg;
        }
        //This Method Used to Fetch Mother Tongue
        public static ResultArgs FetchMotherTongue()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMotherTongue);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        //This Method Used to Fetch Marital Status
        public static ResultArgs FetchMaritalStatus()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMarritalStatus);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }

            return resultArg;
        }
        //This Method Used to Fetch Community
        public static ResultArgs FetchCommunity()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCommunity);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        //This Method Used to Fetch Gender
        public static ResultArgs FetchGender()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchGender);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        //This Method Used to Fetch Blood Group
        public static ResultArgs FetchBloodGroup()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBloodGroup);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        //This Method Used to Fetch Religion
        public static ResultArgs FetchReligion()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReligion);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        //This Method Used to Fetch Degree
        public static ResultArgs FetchDegree()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDegree);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To Fetch Level
        public static ResultArgs FetchLevel()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLevel);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To Fetch PublishCategory
        public static ResultArgs FetchPublishCategory()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchPublishCategory);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To Fetch Volume
        public static ResultArgs FetchVolume()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchVolume);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is used To Fetch SubCategory ...
        public static ResultArgs FetchSubCategory()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSubCategory);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is used To Fetch Qualification ...
        public static ResultArgs FetchQualification()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchQualification);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is used To Fetch Shift ...
        public static ResultArgs FetchShift()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is used To FetchDesignation ...
        public static ResultArgs FetchDesignation()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDesignation);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchFamily ...
        public static ResultArgs FetchRelaion()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRelaion);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchClass ...
        public static ResultArgs FetchClass(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To RegularFetchClass ...
        public static ResultArgs FetchRegularClass(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchRegularclass);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchProgram
        public static ResultArgs FetchProgram(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgram);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchProgrammeGroupProgramme()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme);              
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchBatch
        public static ResultArgs FetchBatch()
        {
            DataValue dv = new DataValue();
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBatch);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable,dv);
            }
            return resultArg;
        }
        // This Method is Used To FetchBatch
        public static ResultArgs FetchBatchByProgramId(DataValue dv ,string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBatchByProgramId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable, dv);
            }
            return resultArg;
        }
        // This Method is Used To FetchFaculty
        public static ResultArgs FetchFaculty(string sAcademicYear)
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFaculty);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchSecondLanguage
        public static ResultArgs FetchSecondLanguage()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSecondLanguage);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchSpokenLanguage
        public static ResultArgs FetchSpokenLanguage()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSpokenLanguage);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchFirstLanguage
        public static ResultArgs FetchLanguage()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLanguage);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchSpeciallyAbled
        public static ResultArgs FetchSpeciallyAbled()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSpeciallyAbled);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchResidencyType
        public static ResultArgs FetchResidencyType()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchResidencyType);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchFatherOccupation
        public static ResultArgs Occupation()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOccupation);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchFatherEducation
        public static ResultArgs Education()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchEducation);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchCountry
        public static ResultArgs FetchCountry()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCountry);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchFatherEmployer
        //public static ResultArgs FetchFatherEmployer()
        //{
        //    using (MySQLHandler objHandler = new MySQLHandler())
        //    {
        //        sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFatherEmployer);
        //        resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
        //    }
        //    return resultArg;
        //}
        //// This Method is Used To FetchFatherCountryOfIssue
        //public static ResultArgs FetchFatherCountryOfIssue()
        //{
        //    using (MySQLHandler objHandler = new MySQLHandler())
        //    {
        //        sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFatherCountryOfIssue);
        //        resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
        //    }
        //    return resultArg;
        //}

        // This Method is Used To FetchMotherEmployer
        //public static ResultArgs FetchMotherEmployer()
        //{
        //    using (MySQLHandler objHandler = new MySQLHandler())
        //    {
        //        sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMotherEmployer);
        //        resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
        //    }
        //    return resultArg;
        //}
        //// This Method is Used To FetchMotherCountryOfIssue
        //public static ResultArgs FetchMotherCountryOfIssue()
        //{
        //    using (MySQLHandler objHandler = new MySQLHandler())
        //    {
        //        sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMotherCountryOfIssue);
        //        resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
        //    }
        //    return resultArg;
        //}
        // This Method is Used To FetchActivity
        public static ResultArgs FetchActvity()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActivity);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchActivity
        public static ResultArgs FetchAchievement()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAchievement);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchPart
        public static ResultArgs FetchPart()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchPart);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To FetchWeekNo
        public static ResultArgs FetchWeekNo()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchWeekNo);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchTalentActivityType
        public static ResultArgs FetchTalentActivityType()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTalentActivityType);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchTalentArea
        public static ResultArgs FetchActivityArea()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActivityArea);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchExamPassed
        public static ResultArgs FetchExamPassed()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchExamPassed);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchOption
        public static ResultArgs FetchOption()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOption);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }

        // This Method is Used To FetchGrade
        public static ResultArgs FetchGrade()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchGrade);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchActiveBatch()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveBatch);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To Non Teaching Category 
        public static ResultArgs FetchNonTeachingCategory()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchNonTeachingCategory);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchProgrammeLevel()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchProgrammeType()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeType);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchDurationUnit()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDurationUnit);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchChannel()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchChannel);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchSubjects()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSubjects);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchMediumOfInstruction()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMediumOfInstruction);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        // This Method is Used To Board Mmber
        public static ResultArgs FetchBoardMmber()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchBoardMmber);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchPaperType()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupPaperType);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
        public static ResultArgs FetchSubjectType()
        {
            using (MySQLHandler objHandler = new MySQLHandler())
            {
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSubjectType);
                resultArg = objHandler.FetchData(sSQL, EnumCommand.DataSource.DataTable);
            }
            return resultArg;
        }
    }

    public class SupportDataViewModel
    {
        public List<SUP_CIA_COMPONENTS> liSupCIAComponents { get; set; }
        public List<SUP_ROOM> liSupRoom { get; set; }
        public List<MENU_ITEMS> liMenuItems { get; set; }
        public List<USER_ROLES_RIGHTS> liUserRolesRights { get; set; }
        public List<MODULES> liModules { get; set; }
        public List<SUP_COLLEGE_BLOCK> liCollegeBlock { get; set; }
        public SelectList CollgeBlock;
        public SelectList ExamType;
        public SelectList ModuleList;
        public SelectList MenuItemList;
        public SelectList RoleList;
        public SelectList liLCasslist;
        public List<SUP_ATTENDANCE_TYPE> lisupAttendanceType { get; set; }
        public List<MENTORING_STUDENT_LIST> liMentorStudent { get; set; }
        public List<MENTORING_STUDENT_LIST_FOR_STAFF> liMentorForStaff { get; set; }
        public List<MENTORING_STUDENT_LIST> liUnallocatedMentorList { get; set; }
        public List<MENTORING_STUDENT_LIST> liAllotedMentorList { get; set; }
        public List<Sup_Designation> lisupDesignation { get; set; }
        public List<SUP_METHODOLOGY> liMethodology { get; set; }
        public List<Sup_Shift> liShift { get; set; }
        public List<SUP_LMS_UNIT> liLms { get; set; }
        public List<SUP_NON_TEACHING_CATEGORY> liSupNonTeachingStaff { get; set; }

        public List<SEMESTER_TYPE> liSemesterType { get; set; }
        public List<SUP_SEMESTER_TYPE> liSupSemesterType { get; set; }
        public List<SUP_SEMESTER> liSupSemester { get; set; }
        public SelectList SupSemesterTypeList { get; set; }
    }

}
