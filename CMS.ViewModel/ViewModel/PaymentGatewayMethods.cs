using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class PaymentGatewayMethods
    {
        public static bool validateDdateExpire(string sDate)
        {
            //  DateTime dateTime = Convert.ToDateTime(sDate, "");

            DateTime dt = Convert.ToDateTime(sDate);
            //dt.ToString("yyyy-MM-ddTHH:mm:ss:zzz")

            // dt.ToString("dd-MM-yyyy HH:mm:ss")
            if (dt > DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateDateExpire(string sDate)
        {
            DateTime dt = Convert.ToDateTime(sDate);

            // Check if it's within the next 20 minutes
            TimeSpan difference = dt - DateTime.Now;

            if (difference.TotalMinutes <= 20 && difference.TotalMinutes > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string CFDateFormatToDDMMYYYY(string sDate)
        {
            if (string.IsNullOrEmpty(sDate))
            {
                DateTime dt = Convert.ToDateTime(sDate);
                return dt.ToString("dd-MM-yyyy HH:mm:ss");
            }
            else
            {
                return string.Empty;
            }
            //  DateTime dateTime = Convert.ToDateTime(sDate, "");

            /// DateTime dt = Convert.ToDateTime(sDate);
            //dt.ToString("yyyy-MM-ddTHH:mm:ss:zzz")

            // dt.ToString("dd-MM-yyyy HH:mm:ss")
        }

        public static List<OrderSplitep> BindVendorSplits(List<FEE_STUDENT_ACCOUNT> studentFeeAccount)
        {
            List<OrderSplitep> orderSplits = new List<OrderSplitep>();
            var studentAccountByBankId = studentFeeAccount.GroupBy(o => o.BANK_ACCOUNT_ID);
            foreach (var item in studentAccountByBankId)
            {
                orderSplits.Add(new OrderSplitep
                {
                    amount = Convert.ToDecimal(studentFeeAccount.Where(o => o.BANK_ACCOUNT_ID == item.FirstOrDefault().BANK_ACCOUNT_ID).Sum(x => decimal.Parse(x.BALANCE)).ToString()),
                    //percentage = Convert.ToDecimal(0.0),
                    vendor_id = item.FirstOrDefault().ACCOUNT_LABEL
                });

            }
            return orderSplits;
        }
    }
}
