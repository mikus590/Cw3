﻿using Cw3.Models;
using System.Collections.Generic;

namespace Cw3.DAL
{
    public interface IDbService
    {
         IEnumerable<Student> GetStudents();
    }
}