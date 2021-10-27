using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserLoginProject.ViewModels
{
    public class UserVM
    {
       
            public string Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public bool IsDelete { get; set; }
            public string Role { get; set; }
            public List<string> Roles { get; set; }
        
    }
}
