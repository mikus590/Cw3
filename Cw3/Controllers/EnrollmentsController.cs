using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Request;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17371;Integrated Security=True";

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
           
            
            using(SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                var tran = con.BeginTransaction();
                com.Transaction = tran;
                try
                {
                    com.CommandText = "select s.IdStudy from dbo.Studies s where Name = @Studies";
                    com.Parameters.AddWithValue("Studies", request.Studies);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Studia o podanej nazwie nie istnieja");
                    }
                    int idStudy = (int)dr["IdStudy"];
                    dr.Close();


                    com.CommandText = "select * ";
                    com.CommandText += "from dbo.Enrollment e with(nolock) ";
                    com.CommandText += "where e.IdStuddy = @IdStudy ";
                    com.CommandText += "and e.Semester = 1 ";
                    com.CommandText += "order by IdEnrollment asc";

                    com.CommandText = "select ";
                    com.CommandText += "  e.StartDate, ";
                    com.CommandText += "  e.IdEnrollment ";
                    com.CommandText += "from Enrollment e with(nolock) ";
                    com.CommandText += "  inner join Student s on e.IdEnrollment = s.IdEnrollment ";
                    com.CommandText += "where e.Semester = 1 ";
                    com.CommandText += "  and s.IndexNumber = @IndexNumber ";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);

                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {

                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Student jest już zapisany na dany semestr");
                    }
                    dr.Close();
                    com.Parameters.Clear();

                    com.CommandText = "select max(IdEnrollment) from Enrollment";
                    int maxId = (int)com.ExecuteScalar() + 1;
                    DateTime startDate = DateTime.Now;

                    com.CommandText = "insert into Enrollment (IdEnrollment, Semester, IdStudy, StartDate) ";
                    com.CommandText += "values (@IdEnrollment, @Semester, @IdStudy, @StartDate)";
                    com.Parameters.AddWithValue("IdEnrollment", maxId);
                    com.Parameters.AddWithValue("Semester", 1);
                    com.Parameters.AddWithValue("IdStudy", idStudy);
                    com.Parameters.AddWithValue("StartDate", startDate);

                    com.ExecuteNonQuery();
                    com.Parameters.Clear();

                    com.CommandText = "select FirstName from Student where IndexNumber = @IndexNumber";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);

                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {

                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Student o podanym indeksie już istnieje");
                    }
                    dr.Close();
                    com.Parameters.Clear();

                    com.CommandText = "Insert into Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) ";
                    com.CommandText += "values(@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnroll)";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    com.Parameters.AddWithValue("FirstName", request.FirstName);
                    com.Parameters.AddWithValue("LastName", request.LastName);
                    com.Parameters.AddWithValue("BirthDate", DateTime.ParseExact(request.BirthDate, "dd.MM.yyyy", null));
                    com.Parameters.AddWithValue("IdEnroll", maxId);

                    com.ExecuteNonQuery();
                

                

                
                string FirstName = request.FirstName;
                string LastName = request.LastName;

                var response = new EnrollStudentResponse
                {
                    IdEnrollment = maxId,
                    FirstName = FirstName,
                    LastName = LastName,
                    IdStudy = idStudy,
                    Semester = 1,
                    StartDate = startDate
                };
                     tran.Commit();
                    return Ok(response);

                }
                catch (SqlException exc)
                {
                    return BadRequest(exc);
                }

            }
            
        }

    }
}