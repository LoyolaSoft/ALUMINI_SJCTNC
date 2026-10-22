/***********************************************************************************************************
 * Created by       : Paul
 * Created On       : 08th Dec 2015
 *  
 * Modified by      : 
 * Modified On      : 
 * Modified Purpose : 
 * 
 * Reviewed By      : 
 * Reviewed On      : 
 * 
 * Purpose          : This is to handle the error handling with db manipulation - accessible throught the application
 ********************************************************************************************************************/
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace CMS.Utility
{
    public class ErrorHandler : IDisposable
    {

        #region Declaration
        MySqlConnection connection;
      //  UserProperty userinfo = new UserProperty();
        private string rowUniqueParmName = "";
        private bool getRowUniqueId = false;
        private const string PARAMETER_DELIMITER = "?";
        //private CurrentConnectionString =strin
        public string CurrentConnectionString { get; set; }
        public ErrorHandler()
        {
            connection = new MySqlConnection();
        }
        #endregion

        #region Connection
        /// <summary>
        /// This method helps in opening the MYSQL Connection
        /// </summary>
        /// <returns>Returning mysql connection</returns>
        /// <exception cref="MySQLException">Problem in opening sql connection</exception>
        /// <exception cref="InvalidOperationException">The connection is open already</exception>
        private MySqlConnection OpenConnection()
        {

            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Open();
            }
            catch (InvalidOperationException ex)
            {

                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError(ex);
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError(ex);
                }
            }

            return connection;
        }

        public void SetConnectionType()
        {
            string s = ConfigurationManager.ConnectionStrings["SMS"].ConnectionString;
            connection = new MySqlConnection(s);
        }

        /// <summary>
        /// This method helps in closing the connection
        /// </summary>
        private void CloseConnection()
        {
            if (connection != null)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        #endregion

        #region MySql Data Types
        /// <summary>Get the .Net Data Type and return the corresponding MySql Datatype </summary>
        /// <param name="fieldType">.Net DataType Enumerator</param>
        /// <returns>MySqlDataType</returns>
        private MySqlDbType GetMySQLFieldType(EnumCommand.DataType fieldType)
        {
            switch (fieldType)
            {
                case EnumCommand.DataType.Boolean:
                    { return MySqlDbType.Bit; }
                case EnumCommand.DataType.Byte:
                    { return MySqlDbType.Byte; }
                case EnumCommand.DataType.Char:
                    { return MySqlDbType.VarChar; }
                case EnumCommand.DataType.DateTime:
                    { return MySqlDbType.DateTime; }
                case EnumCommand.DataType.TimeSpan:
                    { return MySqlDbType.Timestamp; }
                case EnumCommand.DataType.Double:
                case EnumCommand.DataType.Decimal:
                case EnumCommand.DataType.Single:
                    { return MySqlDbType.Decimal; }
                case EnumCommand.DataType.Int16:
                case EnumCommand.DataType.UInt16:
                    { return MySqlDbType.Int16; }
                case EnumCommand.DataType.Int:
                case EnumCommand.DataType.Int32:
                case EnumCommand.DataType.UInt32:
                    { return MySqlDbType.Int32; }
                case EnumCommand.DataType.Int64:
                case EnumCommand.DataType.UInt64:
                    { return MySqlDbType.Int64; }
                case EnumCommand.DataType.ByteArray:
                    { return MySqlDbType.Blob; }
                case EnumCommand.DataType.Blob:
                    { return MySqlDbType.Blob; }
                case EnumCommand.DataType.Text:
                    { return MySqlDbType.Text; }
                case EnumCommand.DataType.Varchar:
                    { return MySqlDbType.VarChar; }
                default:
                    { return MySqlDbType.VarChar; }
            }
        }
        #endregion


        #region Execute Command

        //Execute the insert,update,delete queries
        //LAST_INSERT_ID if set to true bind last insert id in the result args
        public ResultArgs ExecuteCommand(string query, bool getLastInserID)
        {
            return ExecuteCommand(null, query, getLastInserID);
        }

        /// <summary>Execute the Insert,Update,Delete Query </summary>           
        /// <param name="dv">DataValue Object</param>
        /// <param name="query">The Insert,Update or Delete Query</param>
        /// <param name="getlastinsert_ID">LAST_INSERT_ID if set to true
        /// bind last insert id in the result args</param>
        /// <returns>Number of Rows Affected by the Query
        /// greater than 1 --> success
        /// 0 --> failure
        /// -1  --> table referred(delete mode)
        /// -2 --> record referred(delete mode)
        /// </returns>
        /// <exception name="ArgumentNullException"></exception>  
        /// <exception name="InvalidOperationException"></exception>  
        public ResultArgs ExecuteCommand(DataValue dv, string query, bool getlastinsert_ID)
        {
            ResultArgs result = new ResultArgs();
            result.Success = false;
            MySqlCommand Command = null;
            MySqlConnection Con = OpenConnection();
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    //throw new ArgumentNullException("Query is empty", "ExecuteCommand(DataValue dv, string query"); 
                    using (ErrorLog objlog = new ErrorLog())
                    {
                        objlog.WriteError("Error Handler", "ExecuteScalar(DataValue,string)", query, "Query is empty!");
                    }
                }
                else
                {
                    if (dv == null)
                        Command = new MySqlCommand(query, Con);
                    else
                        Command = SetMySqlCommand(dv, query, Con);
                    result.RowsAffected = Command.ExecuteNonQuery();
                    //get the last insert id if flag is set to true
                    if (getlastinsert_ID)
                    {
                        getRowUniqueId = getlastinsert_ID; //Set true to return the last insert id - not handled for stored procedure
                        SetRowUniqueIdentifierValue(result, Command);
                    }
                    result.Success = true;
                }
            }
            catch (InvalidOperationException ex)
            {
                //throw new InvalidOperationException("Problem in Execute Command " + iex.Message, iex);                
                result.RowsAffected = ex.Message.ToLower().Contains("foreign keys") ? -1 : ex.Message.ToLower().Contains("integrity constraint") ? -2 : 0;

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "ExecuteCommand(DataValue dv, string query)", ex.Message);
                }
            }
            catch (Exception ex)
            {

                result.RowsAffected = ex.Message.ToLower().Contains("foreign keys") ? -1 : ex.Message.ToLower().Contains("integrity constraint") ? -2 : 0;

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "ExecuteCommand(DataValue dv, string query)", ex.Message);
                }
            }
            finally
            {
                Command.Dispose();
                Command = null;
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// This method is to execute sql with image data.(member identity details are saveed through this method)
        /// </summary>
        /// <param name="dv">data value</param>
        /// <param name="query">query</param>
        /// <param name="imageByte">member photo</param>
        /// <param name="thumbByte">thumb impression</param>
        /// <param name="digitalByte">digital signature</param>
        /// <returns>1 --> success
        /// 0 or le 0 --> failure</returns>
        public int ExecuteByte(DataValue dv, string query, byte[] imageByte, byte[] thumbByte, byte[] digitalByte)
        {
            int RowsAffected = 0;
            MySqlCommand Command = null;
            MySqlConnection Con = OpenConnection();
            try
            {
                Command = SetMySqlCommand(dv, query, Con);
                //if (imageByte.Length>0)
                Command.Parameters.Add("?PHOTO", MySqlDbType.Blob, imageByte.Length, "PHOTO").Value = (object)imageByte;

                //if (thumbByte.Length>0)
                Command.Parameters.Add("?THUMB_IMPRESSION", MySqlDbType.Blob, thumbByte.Length, "THUMB_IMPRESSION").Value = (object)thumbByte;

                //if (digitalByte.Length>0)
                Command.Parameters.Add("?DIGITAL_SIGNATURE", MySqlDbType.Blob, digitalByte.Length, "DIGITAL_SIGNATURE").Value = (object)digitalByte;

                RowsAffected = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "ExecuteByte(DataValue dv, string query, byte[] imageByte, byte[] thumbByte, byte[] digitalByte)",ex.Message );
                }
            }
            finally
            {
                Command.Dispose();
                Command = null;
                CloseConnection();
            }
            //ServicedComponent.DisposeObject(this);
            return RowsAffected;
        }
        /// <summary>
        /// This method is to execute sql with image data.(nominee identity details are saveed through this method)
        /// </summary>
        /// <param name="dv">data value</param>
        /// <param name="query">query</param>
        /// <param name="digitalByte">digital signature</param>
        /// <returns>1 --> success
        /// 0 or le 0 --> failure</returns>
        public int ExecuteByte(DataValue dv, string query, byte[] digitalByte, int type)
        {
            int RowsAffected = 0;
            MySqlCommand Command = null;
            MySqlConnection Con = OpenConnection();
            try
            {
                Command = SetMySqlCommand(dv, query, Con);
                if (type > 0)
                {
                    //if (digitalByte.Length>0)
                    Command.Parameters.Add("?SIGNATURE", MySqlDbType.Blob, digitalByte.Length, "SIGNATURE").Value = (object)digitalByte;
                }
                else
                {
                    //if (digitalByte.Length > 0)
                    Command.Parameters.Add("?PHOTO", MySqlDbType.Blob, digitalByte.Length, "PHOTO").Value = (object)digitalByte;
                }

                RowsAffected = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "ExecuteByte(DataValue dv, string query, byte[] digitalByte)", ex.Message);
                }
            }
            finally
            {
                Command.Dispose();
                Command = null;
                CloseConnection();
            }
            //ServicedComponent.DisposeObject(this);
            return RowsAffected;
        }
        /// <summary>
        /// This method is to execute the logo image details
        /// </summary>
        /// <param name="dv">data value</param>
        /// <param name="query">query</param>
        /// <param name="imageByte">logo image in bytes</param>
        /// <returns>1 --> success
        /// 0 or le 0 --> failure</returns>
        public int ExecuteLogoByte(DataValue dv, string query, byte[] imageByte)
        {
            int RowsAffected = 0;
            MySqlCommand Command = null;
            MySqlConnection Con = OpenConnection();
            try
            {
                Command = SetMySqlCommand(dv, query, Con);
                if (imageByte != null)
                    Command.Parameters.Add("?LOGO", MySqlDbType.Blob, imageByte.Length, "LOGO").Value = (object)imageByte;
                RowsAffected = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "ExecuteLogoByte(DataValue dv, string query, byte[] imageByte)", ex.Message);
                }
            }
            finally
            {
                Command.Dispose();
                Command = null;
                CloseConnection();
            }
            //ServicedComponent.DisposeObject(this);
            return RowsAffected;
        }

        /// <summary>
        /// This mehtod is to save image byte
        /// </summary>
        /// <param name="dv">data values</param>
        /// <param name="query">query</param>
        /// <param name="imageByte">imageByte</param>
        /// <param name="fieldName">parametername</param>
        /// <returns>1 --> success
        /// 0 or le 0 --> failure</returns>
        public int ExecuteImageByte(DataValue dv, string query, byte[] imageByte, string fieldName)
        {
            int RowsAffected = 0;
            MySqlCommand Command = null;
            MySqlConnection Con = OpenConnection();
            try
            {
                Command = SetMySqlCommand(dv, query, Con);
                //if (imageByte.Length>0)
                Command.Parameters.Add("?" + fieldName, MySqlDbType.Blob, imageByte.Length, fieldName).Value = (object)imageByte;

                RowsAffected = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "ExecuteImageByte(DataValue dv, string query, byte[] imageByte)", ex.Message);
                }
            }
            finally
            {
                Command.Dispose();
                Command = null;
                CloseConnection();
            }
            return RowsAffected;
        }

        #endregion

        #region Execute Scalar

        //Execute Scalar functions
        public string ExecuteScalar(string query)
        {
            return ExecuteScalar(null, query);
        }

        /// <summary>Execute Scalar Query </summary>           
        /// <param name="dv">DataValue Object</param>
        /// <param name="query">The Select Query</param>
        /// <returns>If The Execute Scalar Returns the Null, the function will return
        /// the empty string otherwise the value</returns>
        /// <exception name="ArgumentNullException"></exception>  
        /// <exception name="InvalidOperationException"></exception>  
        public string ExecuteScalar(DataValue dv, string query)
        {
            string ScalarValue = string.Empty;
            MySqlCommand Command = null;
            MySqlConnection Con = OpenConnection();
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    // throw new ArgumentNullException("Query is empty!", "ExecuteScalar(DataValue dv, string query)"); 
                    using (ErrorLog objlog = new ErrorLog())
                    {
                        objlog.WriteError("Error Handler", "ExecuteScalar(DataValue,string)", query, "Query is empty!");
                    }
                }
                if (dv == null)
                    Command = new MySqlCommand(query, Con);
                else
                    Command = SetMySqlCommand(dv, query, Con);
                Object obj = Command.ExecuteScalar();
                if (obj != null)
                    ScalarValue = obj.ToString();
            }
            catch (InvalidOperationException ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "ExecuteScalar(DataValue dv, string query)", ex.Message);
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "ExecuteScalar(DataValue dv, string query)",ex.Message);
                }
            }
            finally
            {
                Command.Dispose();
                Command = null;
                CloseConnection();
            }
            return ScalarValue;
        }

        #endregion

        #region Fetch Data

        /// <summary>Execute the Select Query  </summary>           
        /// <param name="dv">DataValue Object</param>
        /// <param name="query">The Select Query</param>
        /// <returns>DataTable with the records</returns>
        /// <exception name="ArgumentNullException"></exception>  
        /// <exception name="InvalidOperationException"></exception>  
        public ResultArgs FetchData(string query, EnumCommand.DataSource dataSourceType, DataValue dv = null)
        {
            ResultArgs result = new ResultArgs();
            result.Success = true;
            MySqlDataAdapter Adapter;
            MySqlCommand Command = null;
            DataSet ds = new DataSet();
            MySqlConnection Con = OpenConnection();
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    //throw new ArgumentNullException("Query is empty", "FetchData(DataValue dv, string query)"); 
                    using (ErrorLog objlog = new ErrorLog())
                    {
                        result.Success = false;
                        objlog.WriteError("Error Handler", "ExecuteScalar(DataValue,string)", query, "Query is empty!");
                    }
                }
                else
                {

                    if (dv == null)
                        Command = new MySqlCommand(query, Con);
                    else
                        Command = SetMySqlCommand(dv, query, Con);

                    result.dataSource = null;
                    try
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            switch (dataSourceType)
                            {
                                case EnumCommand.DataSource.DataSet:
                                    {
                                        Adapter = new MySqlDataAdapter(Command);
                                        if (result.dataSource == null) result.dataSource = new DataSet();
                                        result.RowsAffected = Adapter.Fill(result.dataSource as DataSet);
                                        result.Success = true;
                                        break;
                                    }
                                case EnumCommand.DataSource.DataView:
                                    {
                                        Adapter = new MySqlDataAdapter(Command);
                                        if (result.dataSource == null) result.dataSource = new DataTable();
                                        result.RowsAffected = Adapter.Fill(result.dataSource as DataTable);
                                        result.dataSource = ((DataTable)result.dataSource).DefaultView;
                                        result.Success = true;
                                        break;
                                    }
                                case EnumCommand.DataSource.DataReader:
                                    {
                                        result.dataSource = Command.ExecuteReader();
                                        result.RowsAffected = ((MySqlDataReader)result.dataSource).RecordsAffected;
                                        result.Success = true;
                                        break;
                                    }
                                case EnumCommand.DataSource.Scalar:
                                    {
                                        result.dataSource = Command.ExecuteScalar();
                                        if (result.dataSource != null)
                                        {
                                            result.RowsAffected = 1;
                                            result.Success = true;
                                        }
                                        else
                                        {
                                            result.RowsAffected = 0;
                                            result.Success = true;
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        Adapter = new MySqlDataAdapter(Command);
                                        if (result.dataSource == null) result.dataSource = new DataTable();
                                        result.RowsAffected = Adapter.Fill(result.dataSource as DataTable);
                                        result.Success = true;
                                        break;
                                    }
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        //Update Exception
                        result.Exception = e;
                        result.Success = false;
                        //new ErrorLog().WriteError("MySQLDataHandler", "Fetch", e.Message, Command.CommandText, CommonMethods.GetExceptionLineNo(e));
                    }
                    finally
                    {
                        // CloseConnection(TransactionType.None);
                        CloseConnection();
                    }

                    //result.ShowMessage("Updated");
                    if (result.Success)
                    {

                        result.DataSource.Data = result.dataSource;
                    }
                    //Adapter = new MySqlDataAdapter(Command);
                    //Adapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ErrorMysqlHandler", "FetchData",ex.Message );
                }
            }
            finally
            {
                Command.Dispose();
                Command = null;
                CloseConnection();
            }
            return result;
        }
        #endregion

        #region Set Mysql Commnad with parameters

        /// <summary>Add the parameter name, value and data type to the MySql Command Object </summary>           
        /// <param name="dv">DataValue Object</param>
        /// <param name="query">The Insert,Update,Delete or Select Query </param>
        /// <returns>MySql Command</returns>
        private MySqlCommand SetMySqlCommand(DataValue dv, string query, MySqlConnection connection)
        {
            string Param = "";
            string FieldName = "";
            MySqlCommand Command = new MySqlCommand(query, connection);
            Command.CommandType = CommandType.Text;
            foreach (DataValueBase dvBase in dv)
            {
                FieldName = dvBase.FieldName;
                Param = GetParameter(query, FieldName);
                if (Param != string.Empty)
                {
                    MySqlParameter Parameter = new MySqlParameter(Param, GetMySQLFieldType(dvBase.FieldDataType));
                    if (dvBase.FieldValue != string.Empty)
                    {
                        Parameter.Value = dvBase.FieldValue;
                    }
                    else
                    {
                        if (dvBase.FieldDataType == EnumCommand.DataType.UInt64 || dvBase.FieldDataType == EnumCommand.DataType.Int ||
                           dvBase.FieldDataType == EnumCommand.DataType.UInt32 || dvBase.FieldDataType == EnumCommand.DataType.SByte ||
                            dvBase.FieldDataType == EnumCommand.DataType.Int32 || dvBase.FieldDataType == EnumCommand.DataType.Int16 ||
                             dvBase.FieldDataType == EnumCommand.DataType.Int64 || dvBase.FieldDataType == EnumCommand.DataType.Byte ||
                            dvBase.FieldDataType == EnumCommand.DataType.UInt16)
                        {
                            Parameter.Value = 0;
                        }
                        else if (dvBase.FieldDataType == EnumCommand.DataType.Decimal || dvBase.FieldDataType == EnumCommand.DataType.Double)
                        {
                            Parameter.Value = 0.0;
                        }
                        else if (dvBase.FieldDataType == EnumCommand.DataType.DateTime)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                        else if (dvBase.FieldDataType == EnumCommand.DataType.Date)
                        {
                            Parameter.Value = DBNull.Value;
                        }
                        else
                        {
                            Parameter.Value = "";
                        }
                    }
                    Command.Parameters.Add(Parameter);
                }


            }
            return Command;
        }

        /// <summary>Check a parameter is available in the query </summary>
        /// <param name="query">The Select,Insert,Update or Delete Query</param>
        /// <param name="fieldName">The Filed Name</param>
        /// <returns>if the query contains the field,it return the filed name other wise returns the empty</returns>
        private string GetParameter(string query, string fieldName)
        {
            string param = "?" + fieldName.ToUpper();
            param = query.Contains(param) ? param : "";
            return param;
        }
        #endregion

        #region Row Unique Identifier

        private void SetRowUniqueIdentifierValue(ResultArgs result, MySqlCommand mySqlCommand)
        {
            if (mySqlCommand.CommandType == CommandType.StoredProcedure)
            {
                MySqlParameterCollection mySqlParameterCollection = mySqlCommand.Parameters;
                string paramName = "";

                foreach (MySqlParameter mySqlParameter in mySqlParameterCollection)
                {
                    if (mySqlParameter.ParameterName == rowUniqueParmName)
                    {
                        paramName = RemoveParameterDelimiter(mySqlParameter.ParameterName);
                        result.RowUniqueIdCollection[paramName] = mySqlParameter.Value;
                        break;
                    }
                }
            }
            else
            {
                if (getRowUniqueId)
                {
                    string sQuery = "SELECT LAST_INSERT_ID()";
                    mySqlCommand.CommandText = sQuery;
                    mySqlCommand.CommandType = CommandType.Text;
                    result.RowUniqueId = mySqlCommand.ExecuteScalar().ToString();
                }
            }
        }

        private string RemoveParameterDelimiter(string parameterName)
        {
            string paramName = parameterName.Replace(PARAMETER_DELIMITER, "");
            return paramName;
        }

        private void ParseException(ResultArgs result, Exception e)
        {
            string errorMessage = e.Message;
            string message = errorMessage;

            int posStart = 0;
            int posEnd = 0;

            //Validation Message for Adding/Updating duplicate value
            posStart = errorMessage.ToLower().IndexOf("duplicate entry");

            if (posStart >= 0)
            {
                posStart = errorMessage.IndexOf("'");
                posEnd = errorMessage.LastIndexOf("'");

                if (posEnd >= posStart)
                {
                    message = "The Record is Available";//errorMessage.Substring(posStart, (posEnd - posStart + 1)) + " is available";
                    goto ENDLINE;
                }
            }

            //Validation Message for Adding/Updating duplicate value
            posStart = errorMessage.ToLower().IndexOf("cannot delete");

            if (posStart >= 0)
            {
                message = "Cannot Delete";
            }

            posStart = errorMessage.ToLower().IndexOf("Deadlock found");

            if (posStart >= 0)
            {
                result.IsDeadLock = true;
                message = "Other user is trying to save,try to save again.";
            }

        ENDLINE:
            ((ExceptionHandler)result.Exception).Add(e, message);
        }
        #endregion

        #region Dispose
        void IDisposable.Dispose()
        {
            GC.Collect();
        }
        #endregion

        #region Transaction Methods
        public void RollBackTransaction()
        {
            MySqlCommand objcmd = new MySqlCommand();
            objcmd.Transaction.Rollback();
        }

        public void CommitTransaction()
        {
            MySqlCommand objcmd = new MySqlCommand();
            objcmd.Transaction.Commit();
        }
        #endregion
    }
}
