using RedHill.SalesInsight.AUJSIntegration.Data;
using RedHill.SalesInsight.AUJSIntegration.Model;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Logger;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.QuotationModels
{
    public class PushQuoteModel
    {
        public int CompanyId { get; set; }
        public string CustomerId { get; set; }
        public long QuotationId { get; set; }

        public PushQuoteModel(int companyId, long quotationId)
        {
            this.CompanyId = companyId;
            this.QuotationId = quotationId;
        }

        /// <summary>
        /// Generates the Quote Payload out of the quotation
        /// </summary>
        /// <param name="quotation">Load with Project, Project.Customer, SalesStaff, TaxCode, QuotationMixes, QuotationMix.StandardMix</param>
        /// <returns></returns>
        public QuotePayload GenerateQuotePayload(Quotation quotation)
        {
            QuotePayload qp = new QuotePayload();
            Project project = quotation.Project;
            Customer customer = quotation.Customer;

            //Customer DispatchID as the CustomerId for QuotePayload
            qp.CustomerId = customer.DispatchId;
            qp.Address1 = (project.Address ?? "");
            qp.Address2 = "";
            qp.City = (project.City ?? "");
            qp.State = (project.State ?? "");
            qp.County = "";
            qp.Country = "";
            qp.Zip = (project.ZipCode ?? "");
            qp.CustomerPO = "";
            qp.ConcreteSalesPersonId = Convert.ToInt64(quotation.SalesStaff.DispatchId);
            qp.SalesLimit = 0;

            if (quotation.PlantId > 0)
            {
                qp.ConcretePlantId = Convert.ToInt64(quotation.Plant.DispatchId);
            }

            if (quotation.AggregatePlantId > 0)
            {
                qp.AggregatePlantId = Convert.ToInt64(quotation.AggregatePlant.DispatchId);
                qp.AggregateSalesPersonId = qp.ConcreteSalesPersonId;
            }

            if (quotation.BlockPlantId > 0)
            {
                qp.BlockPlantId = Convert.ToInt64(quotation.BlockPlant.DispatchId);
                qp.BlockSalesPersonId = qp.ConcreteSalesPersonId;
            }

            qp.TaxScheduleId = Convert.ToInt64(quotation.TaxCode.DispatchId);
            qp.IsNonTaxable = false;
            qp.Notes = quotation.PrivateNotes ?? "";
            qp.TicketNotes = quotation.PublicNotes ?? "";
            qp.QuoteName = project.Name + "          " + string.Format("[{0}]", quotation.Id);

            string dateFormat = "yyyy-MM-dd";

            if (quotation.CreatedOn != null)
                qp.QuoteDate = quotation.CreatedOn.GetValueOrDefault().ToString(dateFormat);

            if (quotation.Project.StartDate != null)
            {
                qp.ConcreteStartDate = quotation.Project.StartDate.GetValueOrDefault().ToString(dateFormat);

                qp.AggregateStartDate = qp.BlockStartDate = qp.ConcreteStartDate;
            }

            if (quotation.QuoteExpirationDate != null)
            {
                qp.ConcreteEndDate = quotation.QuoteExpirationDate.GetValueOrDefault().ToString(dateFormat);

                qp.AggregateEndDate = qp.BlockEndDate = qp.ConcreteEndDate;
            }
            //Initialize the Products
            qp.Products = new List<Product>();

            //Add mixes to the quotation
            if (quotation.QuotationMixes != null)
            {
                Product product = null;
                foreach (var mix in quotation.QuotationMixes.Where(x => x.StandardMixId != null))
                {
                    product = new Product()
                    {
                        ProductId = mix.StandardMix.DispatchId,
                        Description = mix.QuotedDescription,
                        Quantity = mix.Volume.GetValueOrDefault(),
                        Price = Math.Round(mix.Price.GetValueOrDefault(), 2),
                        FreightRate = 0,
                        UsageTypeId = 1,
                        SystemTypeId = (int)ProductType.Concrete
                    };
                    qp.Products.Add(product);
                }
            }

            //Add Aggregate Products to the quotation payload
            if (quotation.QuotationAggregates != null)
            {
                Product product = null;
                foreach (var mix in quotation.QuotationAggregates.Where(x => x.AggregateProductId != null))
                {
                    product = new Product()
                    {
                        ProductId = mix.AggregateProduct.DispatchId,
                        Description = mix.QuotedDescription,
                        Quantity = mix.Volume.GetValueOrDefault(),
                        Price = Math.Round(mix.Price.GetValueOrDefault(), 2),
                        FreightRate = Math.Round(mix.Freight.GetValueOrDefault(), 2),
                        UsageTypeId = 1,
                        SystemTypeId = (int)ProductType.Aggregate
                    };
                    qp.Products.Add(product);
                }
            }

            //Add Block Products to the quotation payload
            if (quotation.QuotationBlocks != null)
            {
                Product product = null;
                foreach (var mix in quotation.QuotationBlocks.Where(x => x.BlockProductId != null))
                {
                    product = new Product()
                    {
                        ProductId = mix.BlockProduct.DispatchId,
                        Description = mix.QuotedDescription,
                        Quantity = mix.Volume.GetValueOrDefault(),
                        Price = Math.Round(mix.Price.GetValueOrDefault(), 2),
                        FreightRate = Math.Round(mix.Price.GetValueOrDefault(), 2),
                        UsageTypeId = 1,
                        SystemTypeId = (int)ProductType.Block
                    };
                    qp.Products.Add(product);
                }
            }

            //TODO: Push Quote level Addons
            List<QuotationAddon> quotationAddons = SIDAL.GetQuotationAddOns(quotation.Id);

            if (quotationAddons != null)
            {
                long districtId = SIDAL.GetDistrictIdForQuotation(quotation.Id); //Get the district from the project plant
                List<AddonPriceProjection> addOnPriceProjections = SIDAL.GetAddOnPriceProjections(districtId, DateTime.Now.Year, DateTime.Now.Month);
                Product product = null;
                foreach (var quoteAddOn in quotationAddons)
                {
                    product = new Product()
                    {
                        ProductId = quoteAddOn.Addon.DispatchId,
                        Description = quoteAddOn.Description,
                        //TODO: Quantity needs further discussion
                        Quantity = 1,
                        Price = addOnPriceProjections.Where(x => x.AddonId == quoteAddOn.AddonId)
                                                         .Select(x => x.Price)
                                                         .FirstOrDefault(),
                        FreightRate = 0,
                        UsageTypeId = 1,
                        SystemTypeId = 1
                    };
                    qp.Products.Add(product);
                }
            }

            //TODO: Push Quote Aggregate level Addons
            List<QuotationAggregateAddon> quoteAggregateAddons = SIDAL.GetQuotationAggregateAddon(quotation.Id);
            if (quoteAggregateAddons != null)
            {
                long aggDistrictId = SIDAL.GetDistrictIdForQuotation(quotation.Id, "aggregate"); //Get the district from the project plant
                if (aggDistrictId != 0)
                {
                    List<AddonPriceProjection> addOnPriceProjections = SIDAL.GetAddOnPriceProjections(aggDistrictId, DateTime.Now.Year, DateTime.Now.Month);
                    Product product = null;
                    int i = 0;
                    foreach (var aggAddOn in quoteAggregateAddons)
                    {
                        i++;

                        product = new Product()
                        {
                            ProductId = aggAddOn.Addon.DispatchId,
                            Description = aggAddOn.Description,
                            //TODO: Quantity needs further discussion
                            Quantity = 1,
                            Price = addOnPriceProjections.Where(x => x.AddonId == aggAddOn.AddonId)
                                                             .Select(x => x.Price)
                                                             .FirstOrDefault(),
                            FreightRate = 0,
                            UsageTypeId = 1,
                            SystemTypeId = 2
                        };
                        qp.Products.Add(product);

                    }
                }
            }
            //TODO: Push Quote Block level Addons
            List<QuotationBlockAddon> quoteBlockAddons = SIDAL.GetQuotationBlockAddon(quotation.Id);
            if (quoteBlockAddons != null)
            {
                long blockDistrictId = SIDAL.GetDistrictIdForQuotation(quotation.Id, "block"); //Get the district from the project plant
                if (blockDistrictId != 0)
                {
                    List<AddonPriceProjection> addOnPriceProjections = SIDAL.GetAddOnPriceProjections(blockDistrictId, DateTime.Now.Year, DateTime.Now.Month);
                    Product product = null;
                    foreach (var blockAddon in quoteBlockAddons)
                    {
                        product = new Product()
                        {
                            ProductId = blockAddon.Addon.DispatchId,
                            Description = blockAddon.Description,
                            //TODO: Quantity needs further discussion
                            Quantity = 1,
                            Price = addOnPriceProjections.Where(x => x.AddonId == blockAddon.AddonId)
                                                             .Select(x => x.Price)
                                                             .FirstOrDefault(),
                            FreightRate = 0,
                            UsageTypeId = 1,
                            SystemTypeId = 3
                        };
                        qp.Products.Add(product);
                    }
                }
            }

            return qp;
        }

        public bool PushQuote(string PushedBy)
        {
            bool success = false;
            try
            {
                Quotation quotation = SIDAL.FindQuotationWithAllRefs(this.QuotationId);

                //int sytemTypeIdForAddons = (int)ProductType.Concrete;

                //bool concreteEnabled = quotation.ConcreteEnabled.GetValueOrDefault(),
                //    aggEnabled = quotation.AggregateEnabled.GetValueOrDefault(),
                //    blockEnabled = quotation.BlockEnabled.GetValueOrDefault();

                //if (concreteEnabled && aggEnabled && blockEnabled)
                //{
                //    sytemTypeIdForAddons = (int)ProductType.Concrete;
                //}
                //else if (!concreteEnabled && aggEnabled && !blockEnabled)
                //{
                //    sytemTypeIdForAddons = (int)ProductType.Aggregate;
                //}
                //else if (!concreteEnabled && !aggEnabled && blockEnabled)
                //{
                //    sytemTypeIdForAddons = (int)ProductType.Block;
                //}

                SyncManager syncManager = new SyncManager(this.CompanyId,new FileLogger());

                success = syncManager.PushQuote(this.GenerateQuotePayload(quotation));

                if (success)
                {

                    SIDAL.UpdateQuotationLastPushedDate(quotation.Id, PushedBy);
                }
            }
            catch (Exception ex)
            {
            }
            return success;
        }
    }
}