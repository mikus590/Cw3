using Cw3.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs
{
    public class EnrollStudentResult
    {
        public EnrollStudentResponse Response { get; set; }
        public ResultCodes ResultCode { get; set; }
    }
}
