using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.Data
{
    public class MovieActor
    {
        public int ActorId { get; set; }
        public int MovieId { get; set; }
        public string Role { get; set; }
    }
}
