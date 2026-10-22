
using System;

namespace CMS.Utility
{
    public class ExceptionHandler : Exception
    {
        #region Data Members

        private string message = "";
        private bool isError = false;
        private string errorSource = "";
        private Exception exception = new Exception();
        private static ExceptionHandler exceptionLog = null;

        #endregion

        #region Properties

        public static ExceptionHandler ExceptionLog
        {
            set { exceptionLog = value; }
        }

        public virtual void WirteLog(ExceptionHandler exceptionHandler)
        {

        }

        public string TargetMethod
        {
            get { return this.GetExceptionItem("METHOD"); }
        }

        public string TargetSource
        {
            get { return this.GetExceptionItem("SOURCE"); }
        }

        public string TargetAssembly
        {
            get { return this.GetExceptionItem("ASSEMBLY"); }
        }

        public override string Message
        {
            get { return this.ErrorMessage; }
        }

        public string ErrorMessage
        {
            get { return this.message; }
            set { this.message = value; }
        }

        public bool IsError
        {
            get { return this.isError; }
            set { this.isError = value; }
        }

        public string ErrorSource
        {
            get { return this.errorSource; }
            set { this.errorSource = value; }
        }

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; this.isError = true; }
        }

        #endregion

        #region Constructors

        public ExceptionHandler()
        {

        }

        private string GetExceptionItem(string type)
        {
            string value = "";

            if (exception != null)
            {
                if (type == "METHOD")
                {
                    value = exception.TargetSite.ToString();
                }
                else if (type == "SOURCE")
                {
                    value = exception.StackTrace;
                }
                else if (type == "ASSEMBLY")
                {
                    value = exception.TargetSite.Module.Name;
                }
            }

            return value;
        }

        public ExceptionHandler(Exception e, bool showMessage)
            : this(e, "", true, showMessage) { }
        public ExceptionHandler(string message, bool showMessage)
            : this(null, message, false, showMessage) { }
        public ExceptionHandler(string message, bool isError, bool showMessage)
            : this(null, message, isError, showMessage) { }

        public ExceptionHandler(Exception e, string message, bool isError, bool showMessage)
        {
            HandleException(e, message, isError, showMessage);
        }
        #endregion

        #region Methods

        public void Add(Exception e)
        {
            HandleException(e, "", true, false);
        }

        public void Add(Exception e, string message)
        {
            HandleException(e, message, false, true);
        }

        public void Add(Exception e, string message, bool isError)
        {
            HandleException(e, message, isError, false);
        }

        public void Add(string message)
        {
            HandleException(null, message, false, false);
        }

        public void Add(string message, bool isError)
        {
            HandleException(null, message, isError, false);
        }

        public void Add(string message, bool isError, bool showMessage)
        {
            HandleException(null, message, isError, showMessage);
        }

        private void HandleException(Exception e, string message, bool isError, bool showMessage)
        {
            if (e != null)
            {
                this.exception = e;
                this.message = e.Message;
                this.ErrorSource = e.Source + "\r\n\r\n" + e.StackTrace;
            }

            this.isError = isError;
            if (message != "") { this.message = message; }
            if (exceptionLog != null) { exceptionLog.WirteLog(this); }

            if (showMessage)
            {
                //MessageRender.ShowMessage(message + "\r\n\r\nSource::" + errorSource, isError);
                MessageRender.ShowMessage(message, isError);
            }
        }

        #endregion
    }
}
