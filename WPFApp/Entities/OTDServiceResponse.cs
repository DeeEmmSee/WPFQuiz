using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFApp.Entities
{
    public class OTDServiceResponse
    {
        public int response_code {  get; set; }
        public OTDQuestion[] results { get; set; } = [];
    }
}
