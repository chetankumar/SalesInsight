using System.Runtime.Serialization;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    [KnownType(typeof(Project))]
    [KnownType(typeof(ProjectSalesStaff))]
    [KnownType(typeof(ProjectCompetitor))]
    [KnownType(typeof(ProjectNote))]
    [KnownType(typeof(ProjectProjection))]
    [KnownType(typeof(Customer))]
    [KnownType(typeof(Competitor))]
    [KnownType(typeof(SalesStaff))]
    [KnownType(typeof(Contractor))]
    [KnownType(typeof(MarketSegment))]
    [KnownType(typeof(ProjectStatus))]
    [KnownType(typeof(District))]
    [KnownType(typeof(Region))]
    [KnownType(typeof(Plant))]
    [KnownType(typeof(Company))]
    [KnownType(typeof(ConcreteProduct))]
    [KnownType(typeof(ConcreteProductPlant))]
    [KnownType(typeof(AdditionalProduct))]
    [KnownType(typeof(AdditionalProductPlant))]
    public class SIOperation
    {
        //---------------------------------
        // Constructors
        //---------------------------------

        #region public SIOperation()

        public SIOperation()
        {
        }

        #endregion

        #region public SIOperation(SIOperationType type, object item)

        public SIOperation(SIOperationType type, object item)
        {
            this.type = type;
            this.item = item;
        }

        #endregion

        //---------------------------------
        // Properties
        //---------------------------------

        #region public SIOperationType Type

        public SIOperationType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        #endregion

        #region public object Item

        public object Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected SIBatchOperationType type

        protected SIOperationType type = SIOperationType.Add;

        #endregion

        #region protected object item

        protected object item = null;

        #endregion
    }
}
