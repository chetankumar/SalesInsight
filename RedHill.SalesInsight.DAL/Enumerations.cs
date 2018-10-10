namespace RedHill.SalesInsight.DAL
{
    //---------------------------------
    // Enumerations
    //---------------------------------

    #region SaveQuoteResult

    public enum SaveQuoteResult : int
    {
        Undefined = -1,
        Success = 0,
        DuplicateAdditionalProductIdPlantId = 3,
        DuplicateConcreteProductIdPlantId = 4
    };

    #endregion SaveQuoteResult

    #region SaveProductResult

    public enum SaveProductResult : int
    {
        Undefined = -1,
        Success = 0,
        DuplicateCompanyIdProductCode = 1,
        DuplicateProductIdPlantId = 2
    };

    #endregion SaveProductResult

    #region SaveCustomerResult

    public enum SaveCustomerResult : int
    {
        Undefined = -1,
        Success = 0,
        DuplicateCustomerContactName = 1
    }

    #endregion SaveCustomerResult
}
