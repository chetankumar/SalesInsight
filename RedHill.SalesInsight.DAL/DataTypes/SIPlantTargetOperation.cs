using System;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIPlantTargetOperation
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public SIPlantTargetOperationType OperationType

        public SIPlantTargetOperationType OperationType
        {
            get
            {
                return operationType;
            }
            set
            {
                operationType = value;
            }
        }

        #endregion

        #region public SIPlantTargetColumnType ColumnType

        public SIPlantTargetColumnType ColumnType
        {
            get
            {
                return columnType;
            }
            set
            {
                columnType = value;
            }
        }

        #endregion

        #region public int PlantBudgetID

        public int PlantBudgetID
        {
            get
            {
                return plantBudgetID;
            }
            set
            {
                plantBudgetID = value;
            }
        }

        #endregion

        #region public int PlantID

        public int PlantID
        {
            get
            {
                return plantID;
            }
            set
            {
                plantID = value;
            }
        }

        #endregion

        #region public string Value

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        #endregion

        #region public int Budget

        public int Budget
        {
            get
            {
                return budget;
            }
            set
            {
                budget = value;
            }
        }

        #endregion

        #region public int Trucks

        public int Trucks
        {
            get
            {
                return trucks;
            }
            set
            {
                trucks = value;
            }
        }

        #endregion

        #region public DateTime BudgetDate

        public DateTime BudgetDate
        {
            get
            {
                return budgetDate;
            }
            set
            {
                budgetDate = value;
            }
        }

        #endregion

        #region public int PlantBudgetMarketSegmentID

        public int PlantBudgetMarketSegmentID
        {
            get
            {
                return plantBudgetMarketSegmentID;
            }
            set
            {
                plantBudgetMarketSegmentID = value;
            }
        }

        #endregion

        #region public int MarketSegmentID

        public int MarketSegmentID
        {
            get
            {
                return marketSegmentID;
            }
            set
            {
                marketSegmentID = value;
            }
        }

        #endregion

        #region public int PlantBudgetSalesStaffID

        public int PlantBudgetSalesStaffID
        {
            get
            {
                return plantBudgetSalesStaffID;
            }
            set
            {
                plantBudgetSalesStaffID = value;
            }
        }

        #endregion

        #region public int SalesStaffID

        public int SalesStaffID
        {
            get
            {
                return salesStaffID;
            }
            set
            {
                salesStaffID = value;
            }
        }

        #endregion

        #region public short? Percentage

        public short? Percentage
        {
            get
            {
                return percentage;
            }
            set
            {
                percentage = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected SIPlantTargetOperationType operationType

        protected SIPlantTargetOperationType operationType = SIPlantTargetOperationType.PlantBudget;

        #endregion

        #region protected SIPlantTargetColumnType columnType

        protected SIPlantTargetColumnType columnType = SIPlantTargetColumnType.Plant;

        #endregion

        #region protected int plantBudgetID

        protected int plantBudgetID = int.MinValue;

        #endregion

        #region protected int plantID

        protected int plantID = int.MinValue;

        #endregion

        #region protected string value

        protected string value = null;

        #endregion

        #region protected int budget

        protected int budget = int.MinValue;

        #endregion

        #region protected int trucks

        protected int trucks = int.MinValue;

        #endregion

        #region protected DateTime budgetDate

        protected DateTime budgetDate = DateTime.MinValue;

        #endregion

        #region protected int plantBudgetMarketSegmentID

        protected int plantBudgetMarketSegmentID = int.MinValue;

        #endregion

        #region protected int marketSegmentID

        protected int marketSegmentID = int.MinValue;

        #endregion

        #region protected int plantBudgetSalesStaffID

        protected int plantBudgetSalesStaffID = int.MinValue;

        #endregion

        #region protected int salesStaffID

        protected int salesStaffID = int.MinValue;

        #endregion

        #region protected short percentage

        protected short? percentage = short.MinValue;

        #endregion
    }
}
