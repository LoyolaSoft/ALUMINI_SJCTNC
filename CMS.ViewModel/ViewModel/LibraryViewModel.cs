using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.ViewModel.Model;
using System.Web.Mvc;


namespace CMS.ViewModel.ViewModel
{
    public class LibraryViewModel : IDisposable
    {

        public string Message { get; set; }
        public string sLastIndexBookId { get; set; }

      
        #region Properties
        public SelectList sAcademicYearList { get; set; }
        public SelectList sImprintDDL { get; set; }
        public SelectList sSeriesDDL { get; set; }
        public SelectList sKeywordsDDL { get; set; }
        public SelectList sDisciplineDDL { get; set; }
        public SelectList AutoClassDDL { get; set; }
        public SelectList sDocumentTypeDDL { get; set; }
        public SelectList sIllustrationDDL { get; set; }
        public SelectList sStatusDDL { get; set; }
        public SelectList sTapsDDL { get; set; }
        public SelectList sSupAcDDL { get; set; }
        public SelectList sLibCode { get; set; }
        public SelectList LanguageDDL { get; set; }
        public SelectList UserType { get; set; }
        public SelectList ProgrammeLevel { get; set; }
        public SelectList SupRole { get; set; }
        public SelectList SupSemesterType { get; set; }
        public SelectList ComponentList { get; set; }
        public SelectList ApplicationType { get; set; }
        public SelectList BloodGroup { get; set; }
        public SelectList Degree { get; set; }
        public SelectList ArticleType { get; set; }
        public SelectList SubjectType { get; set; }
        public SelectList sBlock { get; set; }
        public SelectList sYear { get; set; }
        public SelectList sAuthor { get; set; }
        public SelectList sAutoclass { get; set; }
        public SelectList sCorparateAuthor { get; set; }
        public SelectList sClassNo { get; set; }
        public SelectList sRack { get; set; }
        public SelectList sCell { get; set; }
        public SelectList sBookStatus { get; set; }
        public SelectList sBookInvoice { get; set; }
        public SelectList sSearch { get; set; }
        public SelectList sCollection { get; set; }



        #endregion

        #region Models
        public LI_BOOK_DETAILS ObjBookDetails = new LI_BOOK_DETAILS();
        public LI_BOOK_CIRCULATION ObjBookCirculation = new LI_BOOK_CIRCULATION();
        public LI_USER_DETAILS objLiUserDetails = new LI_USER_DETAILS();
        public LI_LOGIN_FIELDS objLoginFields = new LI_LOGIN_FIELDS();
        public LI_BOOK_READING objBookReading = new LI_BOOK_READING();
        public LI_USER_PROVISIONS objUserProvision = new LI_USER_PROVISIONS();
        public LI_EXTERNAL_USER_INFO ObjExternalUserInfo = new LI_EXTERNAL_USER_INFO();
        #endregion

        #region List 

        public List<LI_RULES> liRules { get; set; }
        public List<SUP_LI_BOOK_COLLECTION> licollection { get; set; }
        public List<LI_REPORT_FIELDS> libookReview = new List<LI_REPORT_FIELDS>();
        public List<LISTENING_AUDIO_MODEL> liListeningAudio = new List<LISTENING_AUDIO_MODEL>();
        public List<LISTENING_QUIZ_QESTION> liQuizQuestion = new List<LISTENING_QUIZ_QESTION>();
        public List<LI_REPORT_FIELDS> EraSetting = new List<LI_REPORT_FIELDS>();
        public List<LI_REPORT_FIELDS> liStudentDetails = new List<LI_REPORT_FIELDS>();
        public List<LI_BOOK_RESERVE> liBookReserve { get; set; }
        public List<LI_ISSUE_DUES> liIssueDues = new List<LI_ISSUE_DUES>();
        public List<LI_BOOK_ISSUE> liBookIssue { get; set; }
        public List<LI_PERIODS> liPeriods { get; set; }
        public List<LI_RACK_INFO> liRackInfo { get; set; }
        public List<LI_BOOK_DETAILS> liBookDetails = new List<LI_BOOK_DETAILS>();
        public List<ListBookIssue> liBookIssueList = new List<ListBookIssue>();
        public List<ListBookIssue> liBookRetrunList = new List<ListBookIssue>();
        public List<LI_USER_PROVISIONS> LiUserProvisions { get; set; }
        public List<LI_USER_DETAILS> liUserDetails {get;set;}
        public List<LI_LOGIN_DETAILS> liLoginDetails { get; set; }
        public List<LI_REPORT_FIELDS> liReportFields { get; set; }
        public List<LI_REPORT_FIELDS> liTotalAutherCount { get; set; }

        public List<CollegeDetails> liCollegeDetails { get; set; }
        public List<LI_AUTHOR_INFO> liAuthorInfo { get; set; }
        
        public List<SUP_LI_STATUS> liStatus { get; set; }
        public List<SUP_LI_DISCIPLINE> liDiscipline { get; set; }
        public List<LI_BOOK_CIRCULATION> liBookCirculation = new List<LI_BOOK_CIRCULATION>();
        public List<LI_BOOK_CIRCULATION> liBookCirculation1 = new List<LI_BOOK_CIRCULATION>();
        public List<LI_BOOK_READING> liBookReading = new List<LI_BOOK_READING>();
        public List<LI_EXTERNAL_USER_INFO> liExternalUserInfo = new List<LI_EXTERNAL_USER_INFO>();
       
        #endregion

        #region Dispose Method
        // Dispose Method 
        public void Dispose()
        {
            GC.Collect();
        }
        #endregion
    }
}
