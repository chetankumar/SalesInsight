namespace RedHill.SalesInsight.DAL.DataTypes
{
    using System;

    public class SISavedProduct
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SISavedProduct()
        {
            this.result = SaveProductResult.Undefined;
            this.productId = null;
        }

        public SISavedProduct(SaveProductResult result)
            : base()
        {
            this.result = result;
        }

        public SISavedProduct(SaveProductResult result, int productId)
        {
            this.result = result;
            this.productId = productId;
        }

        #endregion Construction

        //---------------------------------
        // Properties
        //---------------------------------

        #region Result

        public SaveProductResult Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
            }
        }

        #endregion Result

        #region ProductId

        public int? ProductId
        {
            get
            {
                return this.productId;
            }
            set
            {
                this.productId = value;
            }
        }

        #endregion ProductId

        //---------------------------------
        // Fields
        //---------------------------------

        #region Fields

        private SaveProductResult result;
        private int? productId;

        #endregion Fields
    }
}
