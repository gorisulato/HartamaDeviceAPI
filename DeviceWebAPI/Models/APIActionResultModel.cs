using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceWebAPI.Models
{
    public class APIActionResultModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public int ResultCount { get; set; }
    }
}