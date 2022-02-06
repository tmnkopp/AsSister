using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AW.API.Models
{
    public class UserToLogin
    {
        public string User { get; set; }
        public string Attempt { get; set; }  
    }
}
