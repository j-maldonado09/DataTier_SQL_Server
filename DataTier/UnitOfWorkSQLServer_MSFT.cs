using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace DataTier
{
    public class UnitOfWorkSQLServer_MSFT : Interfaces.IUnitOfWork
    {
        private Tools _tools;
        private SqlConnection _sqlConnection;
        private SqlTransaction _sqlTransaction;
        private SqlCommand _sqlCommand;
        private bool _isTransactionActive = false;
        private bool _isError = false;
        private string _exceptionDetails;

        // -----------------------------------------------------------------
        // Constructor.
        // -----------------------------------------------------------------
        public UnitOfWorkSQLServer_MSFT(SqlConnection sqlConnection,
                                    Tools tools)
        {
            _tools = tools;
            _sqlConnection = sqlConnection;

            GetConnectionObject();
            GetCommandObject();
        }

        // -----------------------------------------------------------------
        // Get connection object.
        // -----------------------------------------------------------------
        //private async void GetConnectionObject()
        //{
        //    _sqlConnection.ConnectionString = await _tools.GetConnectionStringContent("DefaultConnection");
        //    _sqlConnection.Open();
        //}

        private void GetConnectionObject()
        {
            _sqlConnection.ConnectionString = _tools.GetConnectionStringContent("DefaultConnection");
            _sqlConnection.Open();
        }

        // -----------------------------------------------------------------
        // Get command object.
        // -----------------------------------------------------------------
        private void GetCommandObject()
        {
            _sqlCommand = _sqlConnection.CreateCommand();
        }

        // -----------------------------------------------------------------
        // Get transaction object.
        // -----------------------------------------------------------------
        private void GetTransactionObject()
        {
            if (!_isTransactionActive)
            {
                _sqlTransaction = _sqlConnection.BeginTransaction();
                _sqlCommand.Transaction = _sqlTransaction;
                _isTransactionActive = true;
            }
        }

        // --------------------------------------------------------------------------------
        // Executes an Insert, Update or Delete SQL query without returning any result.
        // --------------------------------------------------------------------------------
        public void ExecuteNonQuery(string query,
                         IDictionary<string, object> queryParams,
                         [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (_sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                _sqlConnection = new SqlConnection();
                GetConnectionObject();
                GetCommandObject();
            }

            GetTransactionObject();
            _sqlCommand.CommandText = query;
            _sqlCommand.Parameters.Clear();

            if (queryParams.Count > 0)
            {
                foreach (KeyValuePair<string, object> item in queryParams)
                {
                    SqlParameter sqlParameter = new SqlParameter
                    {
                        ParameterName = item.Key,
                        Value = item.Value
                    };
                    _sqlCommand.Parameters.Add(sqlParameter);
                }
            }

            try
            {
                if (_sqlCommand.ExecuteNonQuery() != 0)
                {
                }
                else
                {
                }
            }
            catch (Exception exception)
            {
                ThrowException(memberName, sourceFilePath, sourceLineNumber, exception.Message, exception.GetType().ToString(), exception.StackTrace);
            }
            finally
            {
            }
        }

        // ------------------------------------------------------------------------------------------
        // Executes an Insert, Update or Delete SQL query and returns a single value of type object.
        // ------------------------------------------------------------------------------------------
        public object ExecuteScalar(string query,
                         IDictionary<string, object> queryParams,
                         [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (_sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                _sqlConnection = new SqlConnection();
                GetConnectionObject();
                GetCommandObject();
            }

            object value = null;

            GetTransactionObject();
            _sqlCommand.CommandText = query;
            _sqlCommand.Parameters.Clear();

            if (queryParams.Count > 0)
            {
                foreach (KeyValuePair<string, object> item in queryParams)
                {
                    SqlParameter sqlParameter = new SqlParameter
                    {
                        ParameterName = item.Key,
                        Value = item.Value
                    };
                    _sqlCommand.Parameters.Add(sqlParameter);
                }
            }

            try
            {
                value = _sqlCommand.ExecuteScalar();
            }
            catch (Exception exception)
            {
                ThrowException(memberName, sourceFilePath, sourceLineNumber, exception.Message, exception.GetType().ToString(), exception.StackTrace);
            }
            finally
            {
            }

            return value;
        }

        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        public string GetRecords(string query,
                                 IDictionary<string, object> queryParams,
                                 [CallerMemberName] string memberName = "",
                                 [CallerFilePath] string sourceFilePath = "",
                                 [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (_sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                _sqlConnection = new SqlConnection();
                GetConnectionObject();
                GetCommandObject();
            }

            string jsonResult = "";
            StringBuilder jsonAux = new StringBuilder();
            //jsonAux.Append('[');
            SqlDataReader reader = null;
            _sqlCommand.CommandText = query;
            _sqlCommand.Parameters.Clear();

            if (queryParams.Count > 0)
            {
                foreach (KeyValuePair<string, object> item in queryParams)
                {
                    SqlParameter sqlParameter = new SqlParameter
                    {
                        ParameterName = item.Key,
                        Value = item.Value
                    };
                    _sqlCommand.Parameters.Add(sqlParameter);
                }
            }

            try
            {
                reader = _sqlCommand.ExecuteReader();
                
                while (reader.Read())
                {
                    //jsonAux.Append(reader[0] + ",");
                    jsonAux.Append(reader[0]);
                }

                //jsonAux.Append(']');
                jsonResult = jsonAux.ToString();

                //if (jsonResult.LastIndexOf(',') > -1)
                //    jsonResult = jsonResult.Remove(jsonResult.LastIndexOf(','), 1);
            }
            catch (Exception exception)
            {
                ThrowException(memberName, sourceFilePath, sourceLineNumber, exception.Message, exception.GetType().ToString(), exception.StackTrace);
            }
            finally
            {
                reader.Close();
            }

            if (jsonResult == "")
                jsonResult = "[]";

            return jsonResult;
        }

        // -----------------------------------------------------------------	
        // Dispose DB Objects.
        // -----------------------------------------------------------------
        public void ReleaseDBObjects(bool isErrror = false)
        {
            if (_sqlCommand != null)
                _sqlCommand.Dispose();

            if (_sqlTransaction != null)
            {
                if (_isTransactionActive)
                {
                    if (_isError)
                        _sqlTransaction.Rollback();
                    else
                        _sqlTransaction.Commit();

                    //_sqlTransaction.Dispose();
                    //_isTransactionActive = false;
                }
				_sqlTransaction.Dispose();
				_isTransactionActive = false;
            }

            if (_sqlConnection != null)
                _sqlConnection.Dispose();
        }

        // -----------------------------------------------------------------	
        // Throw an exception when an error is found.
        // -----------------------------------------------------------------
        private void ThrowException(string memberName,
                           string sourceFilePath,
                           int sourceLineNumber,
                           string exceptionMessage,
                           string exceptionType,
                           string exceptionStack)
        {
            _exceptionDetails = $"Member: {memberName} | File path: {sourceFilePath} | Line number: {sourceLineNumber}";
            _tools.AddSystemEvent(exceptionMessage, exceptionType, exceptionStack, _exceptionDetails);
            _isError = true;
            ReleaseDBObjects();
            throw new Exception(exceptionMessage);
        }
    }
}

