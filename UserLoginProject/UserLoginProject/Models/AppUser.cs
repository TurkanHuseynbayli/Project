using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserLoginProject.Models
{
    public class AppUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool isDelete { get; set; }
    }
}
