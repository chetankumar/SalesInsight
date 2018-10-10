namespace RedHill.SalesInsight.DAL
{
    using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Reflection;
    using RedHill.SalesInsight.DAL.DataTypes;
    using System;

    public partial class SalesInsightDataContext
    {
        [Function(Name = "dbo.GetForecastVersusPlan")]
        public ISingleResult<SIForecastVersusPlan> GetForecastVersusPlan([Parameter(Name = "UserId", DbType = "UniqueIdentifier")] System.Nullable<System.Guid> userId, [Parameter(Name = "DistrictIds", DbType = "VarChar(MAX)")] string districtIds, [Parameter(Name = "PlantIds", DbType = "VarChar(MAX)")] string plantIds, [Parameter(Name = "CurrentMonth", DbType = "DATETIME")]  DateTime? currentMonth, [Parameter(Name = "RecordDelimiter", DbType = "VarChar(3)")] string recordDelimiter, [Parameter(Name = "ValueDelimiter", DbType = "VarChar(3)")] string valueDelimiter)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId, districtIds, plantIds, currentMonth, recordDelimiter, valueDelimiter);
            return ((ISingleResult<SIForecastVersusPlan>)(result.ReturnValue));
        }

        [Function(Name = "dbo.GetPlantProjections")]
        public ISingleResult<SIPlantProjections> GetPlantProjections([Parameter(Name = "UserId", DbType = "UniqueIdentifier")] System.Nullable<System.Guid> userId, [Parameter(Name = "DistrictIds", DbType = "VarChar(MAX)")] string districtIds, [Parameter(Name = "PlantIds", DbType = "VarChar(MAX)")] string plantIds, [Parameter(Name = "CurrentMonth", DbType = "DATETIME")] DateTime? currentMonth, [Parameter(Name = "RecordDelimiter", DbType = "VarChar(3)")] string recordDelimiter, [Parameter(Name = "ValueDelimiter", DbType = "VarChar(3)")] string valueDelimiter)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId, districtIds, plantIds, currentMonth, recordDelimiter, valueDelimiter);
            return ((ISingleResult<SIPlantProjections>)(result.ReturnValue));
        }

        [Function(Name = "dbo.GetProjectSuccessMarketShareSummary")]
        public ISingleResult<SIProjectSuccessMarketShareSummary> GetProjectSuccessMarketShareSummary([Parameter(Name = "UserId", DbType = "UniqueIdentifier")] System.Nullable<System.Guid> userId, [Parameter(Name = "RegionIds", DbType = "VarChar(MAX)")] string regionIds, [Parameter(Name = "DistrictIds", DbType = "VarChar(MAX)")] string districtIds, [Parameter(Name = "PlantIds", DbType = "VarChar(MAX)")] string plantIds, [Parameter(Name = "SalesStaffIds", DbType = "VarChar(MAX)")] string salesStaffIds, [Parameter(Name = "BidDateFrom", DbType = "DateTime")] System.Nullable<System.DateTime> bidDateFrom, [Parameter(Name = "BidDateTo", DbType = "DateTime")] System.Nullable<System.DateTime> bidDateTo, [Parameter(Name = "StartDateFrom", DbType = "DateTime")] System.Nullable<System.DateTime> startDateFrom, [Parameter(Name = "StartDateTo", DbType = "DateTime")] System.Nullable<System.DateTime> startDateTo, [Parameter(Name = "WLDateFrom", DbType = "DateTime")] System.Nullable<System.DateTime> wlDateFrom, [Parameter(Name = "WLDateTo", DbType = "DateTime")] System.Nullable<System.DateTime> wlDateTo, [Parameter(Name = "RecordDelimiter", DbType = "VarChar(3)")] string recordDelimiter, [Parameter(Name = "ValueDelimiter", DbType = "VarChar(3)")] string valueDelimiter)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId, regionIds, districtIds, plantIds, salesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo, wlDateFrom, wlDateTo, recordDelimiter, valueDelimiter);
            return ((ISingleResult<SIProjectSuccessMarketShareSummary>)(result.ReturnValue));
        }

        [Function(Name = "dbo.GetProjectSuccessSalesStaffSummary")]
        public ISingleResult<SIProjectSuccessSalesStaffSummary> GetProjectSuccessSalesStaffSummary([Parameter(Name = "UserId", DbType = "UniqueIdentifier")] System.Nullable<System.Guid> userId, [Parameter(Name = "RegionIds", DbType = "VarChar(MAX)")] string regionIds, [Parameter(Name = "DistrictIds", DbType = "VarChar(MAX)")] string districtIds, [Parameter(Name = "PlantIds", DbType = "VarChar(MAX)")] string plantIds, [Parameter(Name = "SalesStaffIds", DbType = "VarChar(MAX)")] string salesStaffIds, [Parameter(Name = "BidDateFrom", DbType = "DateTime")] System.Nullable<System.DateTime> bidDateFrom, [Parameter(Name = "BidDateTo", DbType = "DateTime")] System.Nullable<System.DateTime> bidDateTo, [Parameter(Name = "StartDateFrom", DbType = "DateTime")] System.Nullable<System.DateTime> startDateFrom, [Parameter(Name = "StartDateTo", DbType = "DateTime")] System.Nullable<System.DateTime> startDateTo,[Parameter(Name = "WLDateFrom", DbType = "DateTime")] System.Nullable<System.DateTime> wlDateFrom, [Parameter(Name = "WLDateTo", DbType = "DateTime")] System.Nullable<System.DateTime> wlDateTo, [Parameter(Name = "RecordDelimiter", DbType = "VarChar(3)")] string recordDelimiter, [Parameter(Name = "ValueDelimiter", DbType = "VarChar(3)")] string valueDelimiter)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId, regionIds, districtIds, plantIds, salesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo, wlDateFrom, wlDateTo, recordDelimiter, valueDelimiter);
            return ((ISingleResult<SIProjectSuccessSalesStaffSummary>)(result.ReturnValue));
        }
    }
}
