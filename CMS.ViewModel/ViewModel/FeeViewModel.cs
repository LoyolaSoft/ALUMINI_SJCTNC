using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.ViewModel.Model;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{

    public class FeeMainHeadViewModel
    {
        public List<FeeMainHead> FeeMainHeadList { get; set; }
        public List<SUP_FEE_MAIN_HEAD> MainHeads { get; set; }
        public List<SUP_HEAD> SubHeads { get; set; }
        public List<Sup_Shift> SupShift { get; set; }
        public List<SUP_FEE_ROOT> LiSupFeeRoot{ get; set; }
        public List<SUP_APPLICTION_TYPE> LiSupApplictionType { get; set; }
        public List<SUP_BANK_ACCOUNT> LiSupBankAccount { get; set; }
        public List<SupProgrammeMode> SupProgrammeMode { get; set; }
        public List<SUP_FEE_FREQUENCY> SupFrequency { get; set; }
        public List<Sup_Fee_Category> FeeCategory { get; set; }
    }
    public class FeeFrequencyFeeModeViewModel
    {
        public List<FEE_FREQUENCY_FEE_MODE> FeeFrequencyFeeModeList { get; set; }
        public List<sup_frequency_type> SupFeeMode { get; set; }
        public List<SUP_FEE_FREQUENCY> SupFrequency { get; set; }
    }
    public class FeeViewModel
    {
        public string sMessage { get; set; }
        public SelectList liSupFeeFrequency;

        public SUP_HEAD ObjSup_Head { get; set; }
        public List<SUP_HEAD> liSup_Head { get; set; }
        public Sup_Fee_Category ObjFee_Category { get; set; } 
        public List<Sup_Fee_Category> liFee_Category { get; set; }
        public List<STU_PERSONAL_INFO> listudent { get; set; }
        public List<FEE_STUDENT_ACCOUNT> lifee_stu { get; set; }
        public List<FEE_REPORT> liFeeReport { get; set; }
        public List<STU_LATE_ENTRY> liStuLateFee { get; set; }
        public List<FEE_STUDENT_ACCOUNT> liFeeStudentAccount { get; set; }
        public SelectList ClassList { get; set; }
        public SelectList AcademicyearList { get; set; }
        public SelectList SemesterTypeList { get; set; }
        public SelectList ShiftList { get; set; }
        public SelectList programmeLevel { get; set; }
        public SelectList ProgramName { get; set; }
        public SelectList ClassYearList { get; set; }
        public SelectList ClassName { get; set; }
    }

    
    public class FeeTransactionViewModel
    {
        public SelectList liSupFeeFrequency { get; set; }
        public SelectList liStudentPersonalInfo { get; set; }
        public SelectList liClassList { get; set; }
        public SelectList liClassLevel { get; set; }
        public SelectList liClassYear { get; set; }
        public SelectList liHead { get; set; }
        public SelectList liMainHead { get; set; }
        public SelectList liShift { get; set; }
        public SelectList liProgrammeModeList { get; set; }
        public SelectList liAcademicYear { get; set; }
        public SelectList liProgrammeGroup { get; set; }
        public SelectList liFrequencyType { get; set; }
        public SelectList liApplicationType { get; set; }
        public SelectList FeeRoot { get; set; }
        public SelectList AcademicYear { get; set; }
        public SelectList FeeCategory { get; set; }
        public SelectList liBankAccount { get; set; }
        public FEE_TRANSACTION FeeTransaction = new FEE_TRANSACTION();
        public List<SUP_FEE_FREQUENCY> lstSup_Fee_Frequency { get; set; }
        public List<FEE_STUDENT_ACCOUNT> liFeeStudentAccount { get; set; }
        public List<FEE_STUDENT_ACCOUNT> liCreditAndDebit { get; set; }
        public List<FEE_TRANSACTION> liFeeTransaction { get; set; }
        public List<FEE_TRANSACTION> liMonth { get; set; }
        public List<CollegeDetails> liCollegeDetails { get; set; }
        public List<DayOrderCalender> liDate { get; set; }
        public List<Student_Personal_Info> lstStudentPersonalInfo { get; set; }
        public List<HOSTEL_STUDENT_INFO> lstHostelStudentInfo { get; set; }
        
        public List<SUP_HEAD> lstSupHead { get; set; }
        public List<FEE_RAZORPAY_ACCOUNTS> liFeeAccount { get; set; }
        public List<SUP_FEE_MAIN_HEAD> lstSupMainHead { get; set; }
        public List<FEE_REPORT> liFeeReport { get; set; }
        public List<FEE_STUDENT_DUE> liFeeDueReport { get; set; }
        public List<EXAM_REGISTRATION> liExamRegistration { get; set; }
        public List<SUP_FEE_ROOT> LiSupFeeRoot { get; set; }
        public  OrderIdResponse objOrderId { get; set; }
        public string sMessage { get; set; }
        public List<ATT_ATTENDANCE_REPORT> liAttendanceReport { get; set; }
        public List<STU_LATE_ENTRY> liStuLateFee { get; set; }
        public List<FeeMainHead> liFeeMainHeads { get; set; }
        public List<Fee_Account> liFeeAccountList { get; set; }
        public List<FEE_PAYMENT_MODE_CHARGES> liPAYMENT_MODE_CHARGES { get; set; }

        public List<FEE_RECEIPT_GENERATION> liFeeReceiptGeneration { get; set; }


        public string Status { get; set; }
        public string Message { get; set; }
        public string Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentDate { get; set; }
        public string ReceiptNo { get; set; }
    }
    public class FeeStructureViewModel
    {
        public SelectList ClassYearList { get; set; }
        public SelectList ClassName { get; set; }
        public SelectList ProgramName { get; set; }
        public SelectList programmeLevel { get; set; }
        public SelectList FrequencyList { get; set; }
        public SelectList MainHeadList { get; set; }
        public SelectList AcademicyearList { get; set; }
        public SelectList ShiftList { get; set; }
        public SelectList ProgrammeModeList { get; set; }
        public List<FEE_STRUCTURE> lstFeeStructure { get; set; }
        public List<FEE_STRUCTURE> LiFeeStructure { get; set; }
        public List<Sup_Fee_Category> FeeCategory { get; set; }
        public List<SUP_BANK_ACCOUNT> Bank { get; set; }
        public FEE_STRUCTURE FeeStructure = new FEE_STRUCTURE();
        public SelectList ProgrammeGroupList { get; set; }
    }
    public class FeeDiscountViewModel
    {
        public List<FEE_DISCOUNT> lstFeeDiscount { get; set; }
        public FEE_DISCOUNT FeeDiscount = new FEE_DISCOUNT();
    }
    public class FeeDiscountAllotmentviewModel
    {
        public SelectList AcademicyearList { get; set; }
        public SelectList ClassList { get; set; }
        public List<FEE_DISCOUNT_ALLOTMENT> lstFeeDiscountAllotment { get; set; }
        public FEE_DISCOUNT_ALLOTMENT FeeDiscountAllotment = new FEE_DISCOUNT_ALLOTMENT();
    }
    public class FeeDiscountHeadViewModel
    {
        public List<FEE_DISCOUNT_HEAD> lstFeeDiscountHead { get; set; }
        public FEE_DISCOUNT_HEAD FeeDiscountHead = new FEE_DISCOUNT_HEAD();
    }
    public class FeeAdvancePaymentViewModel
    {
        public List<FEE_ADVANCE_PAYMENT> lstFeeAdvancePayment { get; set; }
        public FEE_ADVANCE_PAYMENT FeeAdvancePayment = new FEE_ADVANCE_PAYMENT();
        public SelectList StudentList { get; set; }
        public SelectList ClassList { get; set; }
        public SelectList AcademicyearList { get; set; }
    }
    public class HallTicketValidationViewModel
    {
        public SelectList liClassList { get; set; }
        public SelectList StudentList { get; set; }
        public List<HALL_TICKET_VALIDATION> lstHallTicketValidation { get; set; }
        public HALL_TICKET_VALIDATION HallTicketValidation = new HALL_TICKET_VALIDATION();
    }

    // Fee Report ...
    public class FeeReportViewModel
    {
        // String Peoperties ....
        
        
        // Select List Properties ....
        public SelectList liFrequency { get; set; }
        public SelectList HostelList { get; set; }
        public SelectList BusList { get; set; }
        public SelectList StageList { get; set; }
        public SelectList AcademicYearList { get; set; }
        public SelectList FrequencyList { get; set; }
        public SelectList MainHeadList { get; set; }
        public SelectList ClassList { get; set; }

        // Class Objects ....

        // List Properties ....
        public List<FEE_REPORT> liFeeReport { get; set; }
        public List<HOSTEL_FEE_REPORT> liHostelFeeReport { get; set; }
        public List<BUS_FEE_REPORT> liBusFeeReport { get; set; }
        public List<EXAM_FEE_REPORT> liExamFeeReport { get; set; }
    }
}
