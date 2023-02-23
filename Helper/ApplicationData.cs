using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_lingk_leadsquare_bdm_util.Helper
{
    public class ApplicationInfo
    {
        public string opportunityId { get; set; }
        public string BatchName { get; set; }
        public string BatchDescription { get; set; }
        public string BannerID { get; set; }
        public string BDMDocumentType { get; set; }
        public string Term { get; set; }
        public string BannerApplicationNumber { get; set; }
        public string BannerChecklistItem { get; set; }


        public List<DocumentData> DocumentData { get; set; }
    }

    public class DocumentData
    {
        public string DocumentDisplayName { get; set; }
        public string DocumentURL { get; set; }
        public string DocumentName { get; set; }
    }

    public class DocumentInfo
    {
        public string DocumentDisplayName { get; set; }
        public string DocumentName { get; set; }
        public byte[] DocumentByteData { get; set; }
    }
}

