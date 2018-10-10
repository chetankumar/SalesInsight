using System;

namespace RedHill.SalesInsight.API.Exceptions
{
    public class SIException : Exception
    {
        //---------------------------------
        // Constructors
        //---------------------------------

        #region public SIException()

        public SIException()
        {
        }

        #endregion

        #region public SIException(bool log, string message) : base(message)

        public SIException(bool log, string message) : base(message)
        {
            this.log = log;
            this.showMessage = message;
        }

        #endregion

        #region public SIException(bool log, string message, string showMessage) : base(message)

        public SIException(bool log, string message, string showMessage)
            : base(message)
        {
            this.log = log;
            this.showMessage = showMessage;
        }

        #endregion      

        //---------------------------------
        // Properties
        //---------------------------------

        #region public string ShowMessage

        public string ShowMessage
        {
            get
            {
                return showMessage;
            }
            set
            {
                showMessage = value;
            }
        }

        #endregion

        #region public bool Log

        public bool Log
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
            }
        }

        #endregion
        
        //---------------------------------
        // Fields
        //---------------------------------

        #region protected string showMessage

        protected string showMessage = null;

        #endregion

        #region protected bool log

        protected bool log = false;

        #endregion
    }
}
