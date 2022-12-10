using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataBaseManagement.DataBaseInitializer
{
    public interface IDataInitializer
    {
        Task<DataInitializer> Initialize();
    }
}
