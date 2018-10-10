namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIViewProjectNoteDetails
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public ProjectNote ProjectNote

        public ProjectNote ProjectNote
        {
            get
            {
                return projectNote;
            }
            set
            {
                projectNote = value;
            }
        }

        #endregion

        #region public string Username

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected ProjectNote projectNote

        protected ProjectNote projectNote = null;

        #endregion

        #region protected string username

        protected string username = null;

        #endregion
    }
}
