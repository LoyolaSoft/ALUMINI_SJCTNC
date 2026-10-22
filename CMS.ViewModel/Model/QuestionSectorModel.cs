using System.Collections.Generic;

namespace CMS.ViewModel.Model
{
    public class QS_QUESTION_PAPER
    {
        public string PAPER_ID          { get; set; }
        public string ACADEMIC_YEAR_ID  { get; set; }
        public string SEMESTER_ID       { get; set; }
        public string COURSE_ID         { get; set; }
        public string EXAM_SETTING_ID   { get; set; }
        public string TOTAL_MARK        { get; set; }
        public string DURATION          { get; set; }
        public string CREATED_BY        { get; set; }
        public string UPDATED_BY        { get; set; }
    }

    public class QS_QUESTION_DETAILS
    {
        public string DETAIL_ID         { get; set; }
        public string PAPER_ID          { get; set; }
        public string SECTION           { get; set; }
        public string QUESTION_NO       { get; set; }
        public string SUB_PART          { get; set; }
        public string QUESTION_TEXT     { get; set; }
        public string BLOOMS_LEVEL_ID   { get; set; }
        public string MARKS             { get; set; }
    }

    public class JSON_QUESTION_PAPER
    {
        public QS_QUESTION_PAPER          PaperHeader { get; set; }
        public List<QS_QUESTION_DETAILS>  Questions   { get; set; }
    }
}
