using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting the process V2");

            Console.WriteLine("Refreshing Mix Ash Percentages");
            SIDAL.RefreshAllMixFormulationCalculations();
            
            Console.WriteLine("Refreshing Mix Formulation Costs");

            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(0));   Console.WriteLine("00 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(1));   Console.WriteLine("01 Refreshing Mix Formulation Costs");
            
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-1));  Console.WriteLine("-01 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-2));  Console.WriteLine("-02 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-3));  Console.WriteLine("-03 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-4));  Console.WriteLine("-04 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-5));  Console.WriteLine("-05 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-6));  Console.WriteLine("-06 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-7));  Console.WriteLine("-07 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-8));  Console.WriteLine("-08 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-9));  Console.WriteLine("-09 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-10)); Console.WriteLine("-10 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(-11)); Console.WriteLine("-11 Refreshing Mix Formulation Costs");

            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(2));   Console.WriteLine("02 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(3));   Console.WriteLine("03 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(4));   Console.WriteLine("04 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(5));   Console.WriteLine("05 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(6));   Console.WriteLine("06 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(7));   Console.WriteLine("07 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(8));   Console.WriteLine("08 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(9));   Console.WriteLine("09 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(10));  Console.WriteLine("10 Refreshing Mix Formulation Costs");
            SIDAL.RefreshAllMixFormulationCosts(DateTime.Today.AddMonths(11));  Console.WriteLine("11 Refreshing Mix Formulation Costs");
            
            Console.WriteLine("Process End");
        }
    }
}
