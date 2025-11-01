using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyNewsDb.Dtos
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string? Gender { get; set; }
        public int RoleId { get; set; }
        public bool IsPremium { get; set; }
    }

    public class LoginDto
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
    }
}
