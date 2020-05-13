using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;

namespace Cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17371;Integrated Security=True";
        public EnrollStudentResult EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResult result = new EnrollStudentResult();

            using (SqlConnection con = new SqlConnection(ConString))
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
                        //Exception = "Studia o podanej nazwie nie istnieja";
                        result.ResultCode = ResultCodes.brakKierunku;
                        return result;
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
                        result.ResultCode = ResultCodes.studentZostałZapisany;
                        return result;
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
                        result.ResultCode = ResultCodes.studentIstnieje;
                        return result;
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

                    result.ResultCode = ResultCodes.studentDodany;
                    result.Response = response;
                    return result;
                }
                catch (SqlException exc)
                {
                    return BadRequest(exc);
                }

            }

        }

        

        public EnrollStudentResponse PromoteStudents(PromoteStudentRequest request)
        {
            EnrollStudentResult result = new EnrollStudentResult();

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())

            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.Transaction = tran;

                try
                {
                    com.CommandText = "select e.IdEnrollment from Enrollment e join Studies s on e.idstudy = s.idstudy where s.name=@Studies and e.semester=@Semester;";
                    com.Parameters.AddWithValue("Studies", request.Studies);
                    com.Parameters.AddWithValue("Semester", request.Semester);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return StatusCode(404, "Brak powiązania w tabeli Enrollment");
                    }
                    dr.Close();
                    com.CommandText = "exec promoteStudents @Studies, @semester;";
                    dr = com.ExecuteReader();
                    dr.Read();
                    var response = new EnrollStudentResponse()
                    {
                        IdEnrollment = (int)dr["IdEnrollment"],
                        Semester = (int)dr["semester"],
                        IdStudy = (int)dr["IdStudy"],
                        StartDate = (DateTime)dr["StartDate"]
                    };
                    dr.Close();
                    tran.Commit();
                    return response;
                }
                catch (SqlException e)
                {
                    tran.Rollback();
                    return Bad(e);
                }
            }
        }

        private EnrollStudentResponse StatusCode(int v1, string v2)
        {
            throw new NotImplementedException(v1 + " " + v2);
        }

        private EnrollStudentResult BadRequest(SqlException exc)
        {
            throw new NotImplementedException("Connection error");
        }
        private EnrollStudentResponse Bad(SqlException exc)
        {
            throw new NotImplementedException("Connection error");
        }
    }
    
}



/* -- com.CommandType = CommandType.StoredProcedure;
 -- com.Connection = con;
 -- con.Open();
 -- com.CommandText = "PromoteStudents";
 -- com.Parameters.AddWithValue("@Studies", SqlDbType.Int).Value = request.Studies;
 -- com.Parameters.AddWithValue("@Semester", SqlDbType.NVarChar).Value = request.Semester;
 -- try
 -- {
 --     com.ExecuteNonQuery();
 -- }
 -- catch (SqlException exc)
 -- {
 --     return Bad(exc);
 -- }
 -- var response = new EnrollStudentResponse
 -- {
 --     IdEnrollment = (int)com.Parameters["@IdEnrollment"].Value,
 --     IdStudy = (int)com.Parameters["@IdStudies"].Value,
 --     //Semester = request.Semester + 1,
 --     StartDate = (DateTime)com.Parameters["@StartDate"].Value
 -- };
 -- return response; .*/
