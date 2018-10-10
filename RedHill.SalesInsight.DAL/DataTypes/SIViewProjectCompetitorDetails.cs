namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIViewProjectCompetitorDetails
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public ProjectCompetitor ProjectCompetitor

        public ProjectCompetitor ProjectCompetitor
        {
            get
            {
                return projectCompetitor;
            }
            set
            {
                projectCompetitor = value;
            }
        }

        #endregion

        #region public Competitor Competitor

        public Competitor Competitor
        {
            get
            {
                return competitor;
            }
            set
            {
                competitor = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected ProjectCompetitor projectCompetitor

        protected ProjectCompetitor projectCompetitor = null;

        #endregion

        #region protected Competitor competitor

        protected Competitor competitor = null;

        #endregion
    }
}
