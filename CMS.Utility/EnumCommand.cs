using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Utility
{
    public class EnumCommand
    {
        public enum DataType
        {
            Boolean,
            Byte,
            Char,
            Date,
            DateTime,
            Decimal,
            Double,
            Int,
            Int16,
            Int32,
            Int64,
            SByte,
            Single,
            String,
            TimeSpan,
            UInt16,
            UInt32,
            UInt64,
            ByteArray,
            Varchar,
            None,
            Memo,
            Blob,
            Text
        }
        public enum SQLType
        {
            SQLStatic,
            SQLDynamic,
            SQLStoredProcedure
        }
        /// <summary>
        /// These are linked with table mailsetting
        /// string - Mailcode
        /// int - mailsettingId
        /// </summary>
        public enum MailCode
        {
            InviteFriends = 1,
            ForgotPassword = 3,
            PoductMessage=2,
        }
       
        public enum RequestType
        {
            Topup = 1,
            Withdraw = 2,
        }
        public enum DataSource
        {
            DataSet,
            DataReader,
            DataTable,
            DataView,
            Scalar,
            
        }
        public enum MenuCode
        {
           
        }

        /// <summary>
        /// This contains the features which are categorized as page 
        /// This code should be mathced with rights_feature table pagecode field
        /// </summary>
        public enum PageCode
        {
           
        }
        public enum UserRights
        {
            Read = 1,
            Write = 2,
            Deny = 3
        }

        public enum ImageCategory
        {
            
        }
        public enum Role
        {
           
        }
        public enum TransactionCategory
        {
            
        }
       
       
       
    }
}
