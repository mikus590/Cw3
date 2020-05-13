using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required]
        public string IndexNumber { get; set; }
        [Required(ErrorMessage = "Musisz podać imię")]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Musisz podać nazwisko")]
        [MaxLength(100)]
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        [Required(ErrorMessage = "Musisz podać nazwę studiów")]
        public string Studies { get; set; }
    }
}
