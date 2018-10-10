using RedHill.SalesInsight.AUJSIntegration.Setup.ClientProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Setup
{
    public class InitialSync
    {
        public void StartInitialSync(IInitialSyncManager initialSyncManager)
        {
            if (initialSyncManager == null)
                throw new ArgumentException("Sync Manager not provided");

            initialSyncManager.StartSync();
        }

        public static IInitialSyncManager FindProvider(string provider)
        {
            IInitialSyncManager sm = null;

            switch (provider)
            {
                case "Brannan":
                    sm = new BrannanSyncProvider(0);
                    break;
                default:
                    break;
            }

            return sm;
        }
    }
}
