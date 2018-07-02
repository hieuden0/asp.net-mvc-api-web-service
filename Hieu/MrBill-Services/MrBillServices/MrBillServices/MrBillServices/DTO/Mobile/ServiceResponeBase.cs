using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrBillServices.DTO.Mobile
{
    public class ServiceResponeBase
    {
        public bool Success { get; set; }
        public int errorCode { get; set; }
        public string Message { get; set; }
    }
}
