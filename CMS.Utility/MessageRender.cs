using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Utility
{
    public class MessageRender
    {
        private static ResultArgs returnResult;

        public static ResultArgs Result
        {
            get { return returnResult; }
            set { returnResult = value; }
        }

        public static string GetMessage(ResultArgs resultArgs)
        {
            string msg = "";

            if (!resultArgs.Success)
            {
                ExceptionHandler e = resultArgs.Exception as ExceptionHandler;
                msg = e.ErrorMessage;

                if (e.IsError)
                {
                    msg += "\r\n\r\nSource::" + e.ErrorSource;
                }
            }

            return msg;
        }

        public static void ShowMessage(ResultArgs resultArgs)
        {
            ShowMessage(resultArgs, false);
        }

        public static void ShowMessage(ResultArgs resultArgs, bool isError)
        {
            returnResult = resultArgs;
            ((ExceptionHandler)returnResult.Exception).IsError = isError;
            ShowError();
        }

        public static void ShowMessage(Exception e)
        {
            returnResult = new ResultArgs();
            returnResult.Exception = e;

            if (!returnResult.IsShowExceptionMessage)
            {
                ShowError();
            }
        }

        public static void ShowMessage(string message)
        {
            ShowMessage(message, false);
        }

        public static void ShowMessage(string message, bool isError)
        {
            returnResult = new ResultArgs();
            returnResult.Message = message;
            ((ExceptionHandler)returnResult.Exception).IsError = isError;
            ShowError();
        }

        private static void ShowError()
        {
            if (returnResult != null && returnResult.Success == false && returnResult.Message != "")
            {
                //MessageBox.Show(returnResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //XtraMessageBox.Show(returnResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}