using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.Data
{
    public class UserToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ValidTill { get; set; }
    }
}
