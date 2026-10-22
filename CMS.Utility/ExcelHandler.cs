using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Utility
{
    public class ExcelHandler
    {
        public ExcelHandler()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private static OleDbConnection m_connection;

        public static void OpenConnection(string sConnectionString)
        {
            try
            {
                string sconnection = sConnectionString;
                m_connection = new OleDbConnection(sconnection);
                m_connection.Open();
            }
            catch (Exception err)
            {
                throw new Exception("Can't open connection \r\n" + err.Message.ToString());
            }
        }

        public static void CloseConnection()
        {
            try
            {
                m_connection.Close();
            }
            catch (Exception err)
            {
                throw new Exception("Can't close the connection \r\n" + err.Message.ToString());
            }
        }


        /// <summary>
        ///  To execute the Sql Statement
        ///  Executing Insert/Update/Delete statements
        /// <param name="sSql">SQL Statement</param>
        /// </summary>
        public static void ExecuteCommand(string sSql, string sConnectionString)
        {
            try
            {
                OpenConnection(sConnectionString);
                OleDbCommand command = getCommand(sSql);
                command.ExecuteNonQuery();
                command = null;
                CloseConnection();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "   " + sSql);
            }
        }


        /// <summary>
        ///  To read single column value
        /// <param name="SQL">SQL Statement</param>
        /// </summary>
        public static object ExecuteScalar(string SQL, string sConnectionString)
        {

            try
            {
                OpenConnection(sConnectionString);
                OleDbCommand command = getCommand(SQL);
                object obj = command.ExecuteScalar();
                CloseConnection();
                return obj;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "   " + SQL);
            }
        }

        public static DataTable ExecuteTable(string sSQL, string TableName, string sConnectionString)
        {
            // sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sConnectionString + ";Extended Properties=Excel 8.0";
            sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sConnectionString + ";Extended Properties=Excel 8.0";

            try
            {
                DataTable dt = new DataTable(TableName);
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                OpenConnection(sConnectionString);
                OleDbCommand command = getCommand(sSQL);
                adapter.SelectCommand = command;
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.Fill(dt);
                command = null;

                adapter = null;
                CloseConnection();
                //reslt.ResultDataTable = dt;
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "   " + sSQL);
            }
        }
        public static DataTable ExecuteTable(string sSQL, string TableName, string sConnectionString, string filetype)
        {
            if (filetype == ".xls")

                sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sConnectionString + ";Extended Properties=Excel 8.0";
            else if (filetype == ".xlsx")

                sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sConnectionString + ";Extended Properties=Excel 8.0";

            try
            {
                DataTable dt = new DataTable(TableName);
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                OpenConnection(sConnectionString);
                OleDbCommand command = getCommand(sSQL);
                adapter.SelectCommand = command;
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.Fill(dt);
                command = null;

                adapter = null;
                CloseConnection();
                //reslt.ResultDataTable = dt;
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "   " + sSQL);
            }
        }


        public static String[] GetExcelSheetNames(string excelFile)
        {
            string sConnectionString = string.Empty;

            sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelFile + ";Extended Properties=Excel 8.0";
            //sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sConnectionString + ";Extended Properties=Excel 8.0";

            using (OleDbConnection excelConn = new OleDbConnection(sConnectionString))
            {

                DataTable dt = new DataTable();
                excelConn.Open();
                dt = excelConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    return null;
                }
                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;
                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }
                if (excelConn.State == ConnectionState.Open)
                    excelConn.Close();
                return excelSheets;
            }
        }

        public static OleDbCommand getCommand(string SQL)
        {
            OleDbCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = SQL;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
        }
    }
}
