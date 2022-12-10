using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataBaseManagement.DataBaseBuilder
{
    public interface IDataBaseBuilder
    {
        Task<DataBaseBuilder> Build();
    }
}
