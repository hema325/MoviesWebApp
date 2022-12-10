using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.Data
{
    public class MovieReviewer
    {
        public int MovieId{get;set;}
        public int ReviewerId { get; set; }
        public int Stars { get; set; }
    }
}
