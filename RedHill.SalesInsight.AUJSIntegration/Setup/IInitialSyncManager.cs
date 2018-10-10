using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Setup
{
    public interface IInitialSyncManager
    {
        #region Methods for Lookups
        
        void SyncMarketSegments();
        void SyncPlants(); 
        void SyncRawMaterialTypes();
        void SyncStatusTypes();
        void SyncTaxSchedules();
        void SyncUOMs();

        #endregion

        void SyncRawMaterials();
        void SyncSalesStaff();
        void SyncCustomers();
        void SyncProducts();

        void StartSync();
    }
}
