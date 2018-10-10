using System.Collections.Generic;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIProductType
    {
        //---------------------------------
        // Constructors
        //---------------------------------

        #region Construction

        public SIProductType()
        {
            this.id = ConcreteId;
            this.name = ConcreteName;
        }

        public SIProductType(int id)
        {
            if (id == ConcreteId)
            {
                this.id = ConcreteId;
                this.name = ConcreteName;
            }
            else if (id == AggregateId)
            {
                this.id = AggregateId;
                this.name = AggregateName;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException("id", string.Format("The valid Product Type Id range is [{0}-{1}].", MinValue.id, MaxValue.id));
            }

        }

        #endregion Construction

        //---------------------------------
        // Static
        //---------------------------------

        #region public static SIProductType Concrete

        public static SIProductType Concrete
        {
            get
            {
                return new SIProductType{Id = ConcreteId, Name = ConcreteName};
            }
        }

        #endregion public static SIProductType Concrete

        #region public static SIProductType Aggregate

        public static SIProductType Aggregate
        {
            get
            {
                return new SIProductType { Id = AggregateId, Name = AggregateName };
            }
        }

        #endregion public static SIProductType Aggregate

        #region public static List<SIProductType> ProductTypes

        public static List<SIProductType> ProductTypes
        {
            get
            {
                // Create the list
                List<SIProductType> productTypes = new List<SIProductType>();

                // Add the items
                productTypes.Add(Concrete);
                productTypes.Add(Aggregate);

                // Return 
                return productTypes;
            }
        }

        #endregion public static List<SIProductType> ProductTypes

        #region public static SIProductType MinValue

        public static SIProductType MinValue
        {
            get
            {
                return Concrete;
            }
        }

        #endregion public static SIProductType MinValue

        #region public static SIProductType MaxValue

        public static SIProductType MaxValue
        {
            get
            {
                return Aggregate;
            }
        }

        #endregion public static SIProductType MaxValue

        //---------------------------------
        // Properties
        //---------------------------------

        #region public int Id

        public int Id
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

        #endregion public int Id

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

        #endregion public string Name

        //---------------------------------
        // Constants
        //---------------------------------

        private const int ConcreteId = 1;
        private const string ConcreteName = "Concrete";
        private const int AggregateId = 2;
        private const string AggregateName = "Aggregate";

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected int id

        protected int id;

        #endregion protected int id

        #region protected string name

        protected string name;

        #endregion protected string name
    }
}
