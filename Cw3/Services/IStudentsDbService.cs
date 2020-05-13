﻿using Cw3.DTOs;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IStudentsDbService
    {
         EnrollStudentResult EnrollStudent(EnrollStudentRequest request);
         EnrollStudentResponse PromoteStudents(PromoteStudentRequest request);
    }
}
