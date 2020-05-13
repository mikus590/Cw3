using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{

    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentsDbService _service;

        public EnrollmentsController(IStudentsDbService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = _service.EnrollStudent(request);

            switch (response.ResultCode)
            {

                case ResultCodes.brakKierunku:
                    return BadRequest("Studia o podanym kierunku nie istnieją");

                case ResultCodes.studentZostałZapisany:
                    return BadRequest("Student już jest zapisany na semestr 1");

                case ResultCodes.studentIstnieje:
                    return BadRequest("Podany student już istnieje");
            }
            return Created("", response.Response);

        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {

            var response = _service.PromoteStudents(request);

            return Created("", response);


        }
    }
}