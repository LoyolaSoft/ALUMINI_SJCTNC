using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{


    class AlumnaeModel
    {
    }



    public class ALU_REGISTRATION_INFO
    {
        public string ALU_REGISTER_ID { get; set; }
        public string BATCH_BEGIN_YEAR { get; set; }
        public string BATCH_END_YEAR { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string BATCH { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string EMAIL_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public string ADDRESS { get; set; }
        public string COUNTRY { get; set; }
        public string AADHAR_NUMBER { get; set; }
        public string PAN_NUMBER { get; set; }
        public string IS_HIGHER { get; set; }
        public string IS_RESEARCH { get; set; }
        public string IS_PLACEMENT { get; set; }
        public string IS_ENTREPRENEUR { get; set; }
        public string COMPETITIVE_EXAM { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ANY_OTHER { get; set; }
        public string ACHIEVEMENT { get; set; }
        public string VISIT_OUR_CAMPUS { get; set; }
        public string SUGGESTION { get; set; }
        public string EXPERIENCE { get; set; }
        public string PHOTO { get; set; }
        
        public string CHERISHED_MEMORY { get; set; }
        public string CONTRIBUTION_TYPE_ID { get; set; }
        public string CONTRIBUTION_TYPE_NAME { get; set; }
        public string MARITAL_STATUS { get; set; }
        public string PNAME_OF_ORGANIZATION { get; set; }
        public string PCUR_DESIGNATION { get; set; }
        public string PCONTACT_ADDRESS { get; set; }
        public string PMOBILE_NO { get; set; }
        public string PEMAIL_ID { get; set; }
    }

   
    public class ALU_ENTREPRENEUR_DETAILS
    {
        public string ALU_ENTREPRENEUR_ID { get; set; }
        public string ALU_REGISTER_ID { get; set; }
        public string ENT_NAME { get; set; }
        public string ENT_CONTACT_DETAILS { get; set; }
        public string NATURE_OF_ORGANIZATION { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

   
    public class ALU_HIGHER_DETAILS
    {
        public string ALU_HIGHER_ID { get; set; }
        public string ALU_REGISTER_ID { get; set; }
        public string COURSE { get; set; }
        public string INSTITUTION { get; set; }
        public string COUNTRY { get; set; }
        public string ADDRESS { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class ALU_PLACEMENT_DETAILS
    {
        public string ALU_PLACEMENT_ID { get; set; }
        public string ALU_REGISTER_ID { get; set; }
        public string PRESENT_ORGANIZATION { get; set; }
        public string POSITION { get; set; }
        public string EMP_CONTACTNO { get; set; }
        public string EMP_MAIL_ID { get; set; }
        public string EXPERIENCE_IN_PREORGANIZATION { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class ALU_RESEARCH_DETAILS
    {
        public string ALU_RESEARCH_ID { get; set; }
        public string ALU_REGISTER_ID { get; set; }
        public string SPECIALIZATION { get; set; }
        public string AREA_OF_RESEARCH { get; set; }
        public string COUNTRY { get; set; }
        public string INSTITUTION { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class SUP_ALU_CONTRIBUTION_TYPE
    {
        public string CONTRIBUTION_TYPE_ID { get; set; }
        public string CONTRIBUTION_TYPE_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class SUP_COMPETITIVE_EXAM
    {
        public string COMPETITIVE_EXAM_ID { get; set; }
        public string COMPETITIVE_EXAM { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class ALUMNAE_SAVE_FIELDS
    {
        public ALU_REGISTRATION_INFO objAluRegistration { get; set; }
        //public ALU_CONTRIBUTION_DETAILS objAluContributionDetails { get; set; }

        public ALU_PLACEMENT_DETAILS objAluPlacementDetails { get; set; }
        public ALU_HIGHER_DETAILS objAluHigherDetails { get; set; }
        public ALU_RESEARCH_DETAILS objAluResearchDetails { get; set; }
        public ALU_ENTREPRENEUR_DETAILS objAluEntreprenuerDetails { get; set; }
    }


    public class ALU_PASSEDOUT_REGISTER_INFO
    {
        public string ALU_PASSEDOUT_REGISTER_ID { get; set; }
        public string  FIRST_NAME { get; set; }
        public string  LAST_NAME { get; set; }
        public string  PROGRAMME_ID { get; set; }
        public string  BATCH_BEGIN_YEAR { get; set; }
        public string  BATCH_END_YEAR { get; set; }
        public string  DEPARTMENT { get; set; }
        public string  DATE_OF_BIRTH { get; set; }
        public string  DOB_PROOF { get; set; }
        public string  EMAIL_ID { get; set; }
        public string  MOBILE_NO { get; set; }
        public string  PERMANENT_ADDRESS { get; set; }
        public string  SCHOLARSHIP { get; set; }
        public string  SCHEME { get; set; }
        public string  AMOUNT_RECEIVED { get; set; }
        public string  SCHOLOR_BANK_PROOF { get; set; }
        public string  SCHOLOR_STATEMENT_PROOF { get; set; }
        public string  AADHAR_NUMBER { get; set; }
        public string  PAN_NUMBER { get; set; }
        public string  IS_HIGHER { get; set; }
        public string  IS_PLACEMENT { get; set; }
        public string  NAME_OF_COMPETITIVE_EXAM { get; set; }
        public string  EXAM_REG_NO { get; set; }
        public string COMPETITIVE_EXAM_PROOF { get; set; }
        public string  IS_CULTURAL { get; set; }
        public string  IS_SPORTS { get; set; }
        public string CC_YEAR { get; set; }
        public string CC_NAME { get; set; }
        public string CC_PROOF { get; set; }
        public string  SUGGESTION { get; set; }
        public string  EXPERIENCE { get; set; }
        public string  ACADEMIC_YEAR { get; set; }
        public string  REG_DATE { get; set; }
        public string  IS_DELETED { get; set; }
        public string  IS_ACTIVE { get; set; }
    }

    public class ALU_PASSEDOUT_HIGHER_DETAILS
    {
        public string ALU_PASSEDOUT_HIGHER_ID { get; set; }
        public string ALU_PASSEDOUT_REGISTER_ID { get; set; }
        public string COURSE { get; set; }
        public string ID_PROOF { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class ALU_PASSEDOUT_PLACEMENT_DETAILS
    {
        public string ALU_PASSEDOUT_PLACEMENT_ID { get; set; }
        public string ALU_PASSEDOUT_REGISTER_ID { get; set; }
        public string STATUS { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class ALU_PASSEDOUT_PLA_OFFCAMPUS_DETAILS
    {
        public string ALU_PASSEDOUT_PLACEMENT_OFFCAMPUS_ID { get; set; }
        public string ALU_PASSEDOUT_PLACEMENT_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string PACKAGE_RECEIVED { get; set; }
        public string ADDRESS { get; set; }
        public string PROOF { get; set; }
        public string PROOF2 { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class ALU_PASSEDOUT_PLA_ONCAMPUS_DETAILS
    {
        public string ALU_PASSEDOUT_PLACEMENT_ONCAMPUS_ID { get; set; }
        public string ALU_PASSEDOUT_PLACEMENT_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string PACKAGE_RECEIVED { get; set; }
        public string ADDRESS { get; set; }
        public string PROOF { get; set; }
        public string PROOF2 { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class ALU_PASSEDOUT_CULTURAL_DETAILS
    {
        public string ALU_PASSEDOUT_CULTURAL_ID { get; set; }
        public string ALU_PASSEDOUT_REGISTER_ID { get; set; }
        public string NAME_OF_EVENT { get; set; }
        public string PLACE { get; set; }
        public string CLGORINSTITUTION_NAME { get; set; }
        public string PROOF { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class ALU_PASSEDOUT_SPORTS_DETAILS
    {
         public string ALU_PASSEDOUT_SPORTS_ID { get; set; }
         public string ALU_PASSEDOUT_REGISTER_ID { get; set; }
        public string NAME_OF_EVENT { get; set; }
        public string PROOF { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

 

    public class SUP_ALU_SCHOLORSHIP_TYPE
    {
        public string ALU_SCHOLORSHIP_TYPE_ID { get; set; }
        public string SCHOLORSHIP_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class SUP_ALU_SCHOLORSHIP_SCHEME
    {
        public string ALU_SCHOLORSHIP_SCHEME_ID { get; set; }
        public string SCHEME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class ALUMNAE_PASSEDOUT_SAVE_FIELDS
    {
        public ALU_PASSEDOUT_REGISTER_INFO objAluPassedoutRegistration { get; set; }
        //public ALU_CONTRIBUTION_DETAILS objAluContributionDetails { get; set; }

        public ALU_PASSEDOUT_HIGHER_DETAILS objAluPassedoutHigherDetails { get; set; }
        public ALU_PASSEDOUT_PLACEMENT_DETAILS objAluPassedoutPlacementDetails { get; set; }
        public ALU_PASSEDOUT_PLA_ONCAMPUS_DETAILS objAluPassedoutPlaOnCampusDetails { get; set; }
        public ALU_PASSEDOUT_PLA_OFFCAMPUS_DETAILS objAluPassedoutPlaOffCampusDetails { get; set; }
        public List<ALU_PASSEDOUT_CULTURAL_DETAILS> objAluPassedoutCulturalDetails { get; set; }
        public List<ALU_PASSEDOUT_SPORTS_DETAILS> objAluPassedoutSportsDetails { get; set; }
      
    }

    public class CERTIFICATE_LIST
    {
        public string CERTIFICATE_ID { get; set; }
        public string CERTIFICATE_NAME { get; set; }
        public string IS_ACTIVE { get; set; }

        public string IS_DELETED { get; set; }
        public string ORDER_BY { get; set; }
        public string CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string DESCRIPTION { get; set; }




    }

    public class CERTIFICATE_CATEGORY
    {
        public string CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ORDER_BY { get; set; }
    }
}

