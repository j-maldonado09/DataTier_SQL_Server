using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace DataTier
{
    // ********************************************************************
    //                      Interfaces Class.
    // ********************************************************************
    public class Interfaces
    {
        // -----------------------------------------------------------------
        //                      General Repository Interface.
        // -----------------------------------------------------------------
        public interface IRepository<T>
        {
            int Create(T entity);
            void Delete(int id);
            void Update(T entity, int id);

            // If "id" argument equals -1 then all records are returned,
            // otherwise a record with specific "id" is returned.
            string Read(int? id = -1);
            void DisposeDBObjects();

        }

        // -----------------------------------------------------------------
        //                      Unit of Work Interface.
        // -----------------------------------------------------------------
        public interface IUnitOfWork
        {
            public void ExecuteNonQuery(string query,
                         IDictionary<string, object> queryParams,
                         [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0);
            public object ExecuteScalar(string query,
                         IDictionary<string, object> queryParams,
                         [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0);
            string GetRecords(string query,
                              IDictionary<string, object> queryParams,
                              [CallerMemberName] string memberName = "",
                              [CallerFilePath] string sourceFilePath = "",
                              [CallerLineNumber] int sourceLineNumber = 0);
            void ReleaseDBObjects(bool isError = false);
        }
    }
}
