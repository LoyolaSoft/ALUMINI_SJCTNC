using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;
using CMS.Utility;

namespace CMS.SQL.Test
{
    public static class TestSQL
    {
        public static string GetStaffGroupSQL(TestSQLCommands sEnumCommand)
        {
            string query = "";


            switch (sEnumCommand)
            {
                case TestSQLCommands.Add:
                    {
                        query = "insert into "+ 
                            Common.SQLTables.tblTest
                            +" (TESTCOL1)VALUE(?TESTCOL1 )";
                        break;
                    }
            }
            return query;
        }
    }
}
