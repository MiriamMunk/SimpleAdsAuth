using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Ad
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public DateTime DateSubmitted { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

    }
}
