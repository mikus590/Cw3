using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class ŚtudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17371;Integrated Security=True";
        public ŚtudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                String result = "";
                result = "select";
                result += "  s.FirstName,";
                result += "  s.IndexNumber,";
                result += "  s.LastName,";
                result += "  s.BirthDate,";
                result += "  e.Semester, ";
                result += "  ss.Name ";
                result += "from dbo.Student s with(nolock)";
                result += "  inner join dbo.Enrollment e with(nolock) on e.IdEnrollment = s.IdEnrollment";
                result += "  inner join dbo.Studies ss with(nolock) on ss.IdStudy = e.IdStudy";
                com.CommandText = result;
                

                con.Open();
                SqlDataReader dr = com.ExecuteReader();

                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate =  Convert.ToDateTime((dr["BirthDate"]));;
                    st.Semester = Convert.ToInt16(dr["Semester"]);
                    st.Name = dr["Name"].ToString();
                    list.Add(st);
                }
            }
            return Ok(list);
        }

        

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Malewski");
            }

            return NotFound("Nie znaleziono studenta");
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokoczona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }

        [HttpPost]
        public IActionResult CreateStudent([FromBody]Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
    }
}