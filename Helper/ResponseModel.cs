using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_lingk_leadsquare_bdm_util.Helper
{
    public class ResponseModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Details { get; set; }
    }
}
