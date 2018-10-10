using System.Collections.Generic;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIStatusType
    {     
        //---------------------------------
        // Static
        //---------------------------------

        #region public static SIStatusType Archive

        public static SIStatusType Archive
        {
            get
            {
                return new SIStatusType{Id = 4, Name = "Archive"};
            }
        }

        #endregion

        #region public static SIStatusType LostBid

        public static SIStatusType LostBid
        {
            get
            {
                return new SIStatusType { Id = 3, Name = "LostBid" };
            }
        }

        #endregion

        #region public static SIStatusType Pending

        public static SIStatusType Pending
        {
            get
            {
                return new SIStatusType { Id = 5, Name = "Pending" };
            }
        }

        #endregion

        #region public static SIStatusType Pipeline

        public static SIStatusType Pipeline
        {
            get
            {
                return new SIStatusType { Id = 1, Name = "Pipeline" };
            }
        }

        #endregion

        #region public static SIStatusType Sold

        public static SIStatusType Sold
        {
            get
            {
                return new SIStatusType { Id = 2, Name = "Sold" };
            }
        }

        #endregion

        #region public static List<SIStatusType> StatusTypes

        public static List<SIStatusType> StatusTypes
        {
            get
            {
                // Create the list
                List<SIStatusType> statusTypes = new List<SIStatusType>();

                // Add the items
                statusTypes.Add(Archive);
                statusTypes.Add(LostBid);
                statusTypes.Add(Pipeline);
                statusTypes.Add(Pending);
                statusTypes.Add(Sold);

                // Return 
                return statusTypes;
            }
        }

        #endregion

        //---------------------------------
        // Properties
        //---------------------------------

        #region public short Id

        public short Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        #endregion

        #region public string Name

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected short id

        protected short id = 0;

        #endregion

        #region protected string name

        protected string name = null;

        #endregion
    }
}
