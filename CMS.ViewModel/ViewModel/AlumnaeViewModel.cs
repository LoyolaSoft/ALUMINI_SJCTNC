using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class AlumnaeViewModel:IDisposable
    {
        public string Message { get; set; }
        public string sProgramme { get; set; }
        public string sBatch { get; set; }
        public string sDept { get; set; }

        public List<CP_PROGRAMME_2017> liProgramme { get; set; }
        public SelectList ProgrammeList { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList CountryList { get; set; }
        public SelectList CompetitiveExamList { get; set; }
        public SelectList Batch { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList MarritalStatusList { get; set; }
        public SelectList ScholorshipType { get; set; }
        public SelectList ScholorshipScheme { get; set; }

        public SelectList CertificateName { get; set; }
        public SelectList Active { get; set; }
        // classes
        public ALU_REGISTRATION_INFO objAluRegistration { get; set; }
        public ALU_HIGHER_DETAILS objAluHigherDetails { get; set; }
        public ALU_RESEARCH_DETAILS objAluResearchDetails { get; set; }
        public ALU_PLACEMENT_DETAILS objAluPlacementDetails { get; set; }
        public ALU_ENTREPRENEUR_DETAILS objAluEntreprenureDetails { get; set; }

      

        //list 
        public List<SUP_ALU_CONTRIBUTION_TYPE> liSupAluContriputionType { get; set; }
        public List<ALU_REGISTRATION_INFO> liAluRegistrationInfo { get; set; }
        public List<ALU_HIGHER_DETAILS> liAluHigherDetails { get; set; }
        public List<ALU_RESEARCH_DETAILS> liAluResearchDetails { get; set; }
        public List<ALU_PLACEMENT_DETAILS> liAluPlacementDetails { get; set; }
        public List<ALU_ENTREPRENEUR_DETAILS> liAluEntrepreneurDetails { get; set; }

        public List<CERTIFICATE_LIST> liCertificateList { get; set; }
        public List<CERTIFICATE_CATEGORY> liCertificateCategoryList{ get; set; }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
