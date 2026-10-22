using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Utility
{
    public  class MessageResult
    {

        public  class ResponseFromSolutionInfi
        {
            public string status { get; set; }
            public  DataTable data { get; set; }
            public string message { get; set; }
        }
        public class sms
        {
            public string to { get; set; }
            public string msgid { get; set; }
            public string message { get; set; }
            public string CreditCount { get; set; }
            public string custom { get; set; }
            public string custom1 { get; set; }
            public string custom2 { get; set; }
            public string sender { get; set; }
        }
        public  class ComposeMessageList
        {
            public string message { get; set; }
            public List<sms> sms { get; set; }
            public int unicode { get; set; }
            public string sender { get; set; }
            public int flash { get; set; }
            public string dlrurl { get; set; }
            public List<listsms> listSMS { get; set; }

        }
        public class Responcesms
        {
            public string id { get; set; }
            public string mobile { get; set; }
            public string senttime { get; set; }
            public string dlrtime { get; set; }
            public string customid { get; set; }
            public string customid1 { get; set; }
            public string customid2 { get; set; }
            public string status { get; set; }
            public string sender { get; set; }
            public string provider { get; set; }
            public string location { get; set; }
        }
        public class ComposeResponcesms
        {
            public string status { get; set; }
            public List<Responcesms> data { get; set; }
            public string message { get; set; }
            public DovesoftResponce sms = new DovesoftResponce();
        }

        public class Dovesoftcompose
        {
            public ComposeResponcesms smslist = new ComposeResponcesms();
        }

        public class listsms
        {
            public string sms { get; set; }
            public string mobiles { get; set; }
            public string senderid { get; set; }
            public string clientSMSID { get; set; }
            public string accountusagetypeid { get; set; }
            public string class_id { get; set; }
            public string receipient_id { get; set; }
            public string entityid { get; set; }
            public string tempid { get; set; }
        }

        public class SMSRequestCompose
        {
            public List<listsms> listsms { get; set; }
            public string password { get; set; }
            public string user { get; set; }
        }

        public class DovesoftResponce
        {
            public string reason { get; set; }
            public string status { get; set; }
            public string messageid { get; set; }
            public string clientsmsid { get; set; }
            public string code { get; set; }
            public string mobileno { get; set; }
        }

        public class SMSResponse
        {
            public SMSlist smslist { get; set; }
        }
        public class SMSlist
        {
            public List<DovesoftResponce> sms { get; set; }

        }

        public class Sms
        {
            public string reason { get; set; }
            public string status { get; set; }
            public string messageid { get; set; }
            public string clientsmsid { get; set; }
            public string code { get; set; }
            public string mobileno { get; set; }

        }
        public class Smslist
        {
            public Sms sms { get; set; }

        }

        public class SMSSingleResponse
        {
            public Smslist smslist { get; set; }

        }
    }
}
