using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.Data
{
    public class Genere
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public MovieGenere MovieGenere { get; set; }
    }
}
