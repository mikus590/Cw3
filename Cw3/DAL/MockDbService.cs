using System.Collections.Generic;
using System.Data.SqlClient;
using Cw3.Models;

namespace Cw3.DAL
{
    public class MockDbService : IDbService  

    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
           
            _students = new List<Student>
            {
                new Student{FirstName="Jan", LastName="Kowalski"},
                new Student{FirstName="Anna", LastName="Malewski"},
                new Student{FirstName="Krzysztof", LastName="Andrzejewicz"}
            };

        }    

        public IEnumerable<Student> GetStudents()
        {
            
            return _students;
        }

    }
}
