using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL.Models;
using RedHill.SalesInsight.DAL.Models.POCO;
using RedHill.SalesInsight.DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using Utils;
/**
 * A much neater and structured SIDAL file extending the legacy SIDAL.cs we have come to love and hate.
 * */
namespace RedHill.SalesInsight.DAL
{
    public partial class SIDAL
    {
        #region RawMaterial Types
        public static void UpdateRawMaterialType(RawMaterialType entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.RawMaterialTypes.InsertOnSubmit(entity);
                }
                else
                {
                    context.RawMaterialTypes.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
        }

        public static List<RawMaterialType> GetRawMaterialTypes(bool showInactives = false)
        {
            using (var context = GetContext())
            {
                var query = context.RawMaterialTypes.AsQueryable();
                if (!showInactives)
                {
                    query = query.Where(x => x.Active.GetValueOrDefault() == true);
                }
                return query.ToList();
            }
        }

        public static RawMaterialType FindRawMaterialType(long id)
        {
            using (var context = GetContext())
            {
                return context.RawMaterialTypes.First(x => x.Id == id);
            }
        }

        #endregion

        #region FSK Prices
        public static void UpdateFSKPrice(FSKPrice entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.FSKPrices.InsertOnSubmit(entity);
                }
                else
                {
                    context.FSKPrices.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
        }

        public static List<FSKPrice> GetFSKPrices(bool showInactives = false)
        {
            using (var context = GetContext())
            {
                var query = context.FSKPrices.AsQueryable();
                if (!showInactives)
                {
                    query = query.Where(x => x.Active == true);
                }
                return query.ToList();
            }
        }

        public static FSKPrice FindFSKPrice(long id)
        {
            using (var context = GetContext())
            {
                return context.FSKPrices.First(x => x.Id == id);
            }
        }

        public static void DeleteFSKPrice(long id)
        {
            using (var context = GetContext())
            {
                var o = context.FSKPrices.First(x => x.Id == id);
                if (o.Plants.Count == 0)
                {
                    context.FSKPrices.DeleteOnSubmit(o);
                }
                context.SubmitChanges();
            }
        }

        public static decimal Update5skPrice(long id, long? fskPriceId)
        {
            using (var context = GetContext())
            {
                Quotation q = context.Quotations.FirstOrDefault(x => x.Id == id);
                if (q != null)
                {
                    q.FskPriceId = fskPriceId;
                    if (fskPriceId != null)
                    {
                        FSKPrice p = context.FSKPrices.FirstOrDefault(x => x.Id == fskPriceId.GetValueOrDefault());
                        q.FskBasePrice = p.BasePrice;
                    }
                }
                context.SubmitChanges();
                if (q.ProjectId != null)
                    UpdateProjectSackPrice(q.ProjectId.GetValueOrDefault());
                return q.FskBasePrice.GetValueOrDefault(0);
            }
        }

        public static decimal Update5skPrice(long id, decimal fskPrice)
        {
            using (var context = GetContext())
            {
                Quotation q = context.Quotations.FirstOrDefault(x => x.Id == id);
                if (q != null)
                {
                    q.FskBasePrice = fskPrice;
                }
                context.SubmitChanges();
                return q.FskBasePrice.GetValueOrDefault(0);
            }
        }

        public static void UpdateProjectSackPrice(long id)
        {
            using (var context = GetContext())
            {
                Project p = context.Projects.FirstOrDefault(x => x.ProjectId == id);
                if (p.Quotations == null || p.Quotations.Count() == 0)
                {
                    p.SackPrice = 0;
                    context.SubmitChanges();
                    return;
                }
                else
                {
                    decimal totalPrice = 0;
                    decimal totalVolume = 0;

                    foreach (var q in p.Quotations)
                    {
                        if (q.Awarded.GetValueOrDefault())
                        {
                            p.SackPrice = q.FskBasePrice.GetValueOrDefault();
                            context.SubmitChanges();
                            return;
                        }
                        totalPrice += q.FskBasePrice.GetValueOrDefault() * Convert.ToDecimal(q.TotalVolume.GetValueOrDefault(1));
                        totalVolume += Convert.ToDecimal(q.TotalVolume.GetValueOrDefault(1));
                    }
                    if (totalVolume != 0)
                    {
                        p.SackPrice = Decimal.Round(totalPrice / totalVolume, 2);
                        context.SubmitChanges();
                    }
                }
            }
        }
        #endregion

        #region Raw Material
        public static void UpdateRawMaterial(RawMaterial entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.RawMaterials.InsertOnSubmit(entity);
                }
                else
                {
                    context.RawMaterials.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
        }

        public static List<RawMaterial> GetRawMaterials(bool showInactives = false)
        {
            using (var context = GetContext())
            {
                DataLoadOptions opts = new DataLoadOptions();
                opts.LoadWith<RawMaterial>(x => x.RawMaterialType);
                context.LoadOptions = opts;
                if (!showInactives)
                {
                    return context.RawMaterials.Where(x => x.Active == true).OrderBy(x => x.RawMaterialType.Name).ThenBy(x => x.MaterialCode).ToList();
                }
                return context.RawMaterials.ToList();
            }
        }

        public static List<RawMaterial> GetNonZeroRawMaterials(long plantId)
        {
            using (var context = GetContext())
            {
                DataLoadOptions opts = new DataLoadOptions();
                opts.LoadWith<RawMaterial>(x => x.RawMaterialType);
                context.LoadOptions = opts;

                var query = context.RawMaterials.Join(context.RawMaterialCostProjections, r => r.Id, c => c.RawMaterialId, (r, c) => new { RawMaterial = r, CostProjection = c }).
                    Where(x => x.RawMaterial.Active == true).
                    Where(x => x.CostProjection.ChangeDate <= DateTime.Today).
                    Where(x => x.CostProjection.PlantId == plantId).
                    Where(x => x.CostProjection.Cost > 0);
                return query.Select(x => x.RawMaterial).Distinct().OrderBy(x => x.RawMaterialType.Name).ThenBy(x => x.MaterialCode).ToList();
            }
        }

        public static List<RawMaterial> GetRawMaterials(bool showInactives, string searchTerm)
        {
            using (var context = GetContext())
            {
                DataLoadOptions opts = new DataLoadOptions();
                opts.LoadWith<RawMaterial>(x => x.RawMaterialType);
                context.LoadOptions = opts;
                var query = context.RawMaterials.AsQueryable();
                if (!showInactives)
                    query = query.Where(x => x.Active == true);
                if (searchTerm != null)
                    query = query.Where(x => x.Description.Contains(searchTerm) || x.MaterialCode.Contains(searchTerm) || x.MeasurementType.Contains(searchTerm));
                return query.ToList();
            }
        }

        public static List<RawMaterialCostProjection> GetLatestRawMaterialCostProjections()
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<RawMaterialCostProjection>(x => x.RawMaterial);
            opts.LoadWith<RawMaterialCostProjection>(x => x.Plant);
            opts.LoadWith<RawMaterialCostProjection>(x => x.Uom);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                var query = context.RawMaterialCostProjections.Where(x => x.Plant.Active.GetValueOrDefault() == true).Where(x => x.RawMaterial.Active == true).ToList();
                var finalList = query.GroupBy(x => new { x.RawMaterial, x.Plant })
                        .Select(x => new
                        {
                            Plant = x.Key.Plant,
                            RawMaterial = x.Key.RawMaterial,
                            LatestProjection = x.OrderByDescending(y => y.ChangeDate).FirstOrDefault()
                        }).Where(x => x.LatestProjection != null).Select(x => new RawMaterialCostProjection
                        {
                            Plant = x.Plant,
                            PlantId = x.Plant.PlantId,
                            RawMaterial = x.RawMaterial,
                            RawMaterialId = x.RawMaterial.Id,
                            Cost = x.LatestProjection.Cost,
                            ChangeDate = x.LatestProjection.ChangeDate,
                            Uom = x.LatestProjection.Uom,
                            UomId = x.LatestProjection.UomId,
                        }).OrderBy(x => x.RawMaterial.MaterialCode).ThenBy(x => x.Plant.Name).ToList();
                return finalList;
            }
        }

        public static Decimal FindRawMaterialPrice(long rawMaterialId, int plantId, DateTime currentMonth, int future, out bool isActual, long? targetUOM = null)
        {
            DateTime actualDate = currentMonth.AddMonths(future);
            using (var context = GetContext())
            {
                RawMaterialCostProjection proj = context.RawMaterialCostProjections.Where(x => x.RawMaterialId == rawMaterialId).Where(x => x.PlantId == plantId).
                    Where(x => x.ChangeDate <= actualDate).OrderByDescending(x => x.ChangeDate).FirstOrDefault();

                isActual = context.RawMaterialCostProjections
                                  .Where(x => x.RawMaterialId == rawMaterialId)
                                  .Where(x => x.PlantId == plantId)
                                  .Where(x => x.ChangeDate == actualDate).Count() > 0;


                if (proj == null)
                {
                    return 0;
                }
                else
                {
                    if (targetUOM == null)
                    {
                        return proj.Cost;
                    }
                    else
                    {
                        Uom target = context.Uoms.Where(x => x.Id == targetUOM).FirstOrDefault();
                        if (target != null)
                        {
                            return proj.Cost * (decimal)target.BaseConversion / (decimal)proj.Uom.BaseConversion;
                        }
                        else
                        {
                            return proj.Cost;
                        }
                    }
                }
            }
        }

        public static RawMaterial FindRawMaterial(long id)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<RawMaterial>(x => x.RawMaterialType);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.RawMaterials.FirstOrDefault(x => x.Id == id);
            }
        }

        public static RawMaterial FindRawMaterialByCode(object value)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<RawMaterial>(x => x.RawMaterialType);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.RawMaterials.FirstOrDefault(x => x.MaterialCode == value && x.Active == true);
            }
        }

        public static void UpdateRawMaterialCostProjection(long rawMaterialId, int plantId, DateTime dateTime, decimal cost, long SaveUOMId)
        {
            using (var context = GetContext())
            {
                RawMaterialCostProjection projection = context.RawMaterialCostProjections.
                    Where(x => x.RawMaterialId == rawMaterialId).
                    Where(x => x.PlantId == plantId).
                    Where(x => x.ChangeDate == dateTime).FirstOrDefault();

                if (projection != null)
                {
                    projection.Cost = cost;
                    projection.UomId = SaveUOMId;
                }
                else
                {
                    projection = new RawMaterialCostProjection();
                    projection.RawMaterialId = rawMaterialId;
                    projection.PlantId = plantId;
                    projection.ChangeDate = dateTime;
                    projection.Cost = cost;
                    projection.UomId = SaveUOMId;
                    context.RawMaterialCostProjections.InsertOnSubmit(projection);
                }
                UpdateMixFormulationCostForRawMaterialCostProjection(projection.Id);
                context.SubmitChanges();
            }
        }

        public static string ScrubRawMaterialProjection(string materialCode, string plantName, string unitCost, string costUom, string date)
        {
            using (var context = GetContext())
            {
                if (materialCode == null)
                    return "Raw material code is empty";
                if (plantName == null)
                    return "Plant is empty";
                if (unitCost == null)
                    return "Cost is empty";
                if (costUom == null)
                    return "Cost UOM is empty";
                if (date == null)
                    return "Date needs to be present";

                RawMaterial raw = context.RawMaterials.FirstOrDefault(x => x.MaterialCode == materialCode && x.Active == true);
                if (raw == null)
                {
                    return "Raw material not found";
                }

                Plant Plant = context.Plants.FirstOrDefault(x => x.Name == plantName && x.Active == true);
                if (Plant == null)
                {
                    return "Plant was not found";
                }

                decimal Cost = 0;
                Decimal.TryParse(unitCost, out Cost);
                if (Cost == 0)
                {
                    return "Cost cannot be 0";
                }

                Uom CostUom = context.Uoms.FirstOrDefault(x => x.Name == costUom);
                if (CostUom == null)
                {
                    return "The unit of measure is incorrect or unavailable";
                }

                if (raw.MeasurementType != CostUom.Category)
                {
                    return "The UOM is not valid for " + raw.MeasurementType + " based Raw Materials";
                }


                DateTime dateTime = DateTime.Today;

                try
                {
                    dateTime = DateTime.ParseExact(date, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    dateTime = dateTime.AddDays((-1 * dateTime.Day) + 1);
                }
                catch (Exception)
                {
                    return "Date is an incorrect format or empty. It should be 'M/d/yyyy hh:mm:ss tt' format, and not empty";
                }

                RawMaterialCostProjection projection = context.RawMaterialCostProjections.
                    Where(x => x.RawMaterialId == raw.Id).
                    Where(x => x.PlantId == Plant.PlantId).
                    Where(x => x.ChangeDate == dateTime).FirstOrDefault();

                if (projection != null)
                {
                    projection.Cost = Cost;
                    projection.UomId = CostUom.Id;
                }
                else
                {
                    projection = new RawMaterialCostProjection();
                    projection.RawMaterialId = raw.Id;
                    projection.PlantId = Plant.PlantId;
                    projection.ChangeDate = dateTime;
                    projection.Cost = Cost;
                    projection.UomId = CostUom.Id;
                    context.RawMaterialCostProjections.InsertOnSubmit(projection);
                }
                UpdateMixFormulationCostForRawMaterialCostProjection(projection.Id);
                context.SubmitChanges();
            }
            return null;
        }

        #endregion

        #region Addons

        public static void UpdateAddon(Addon entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.Addons.InsertOnSubmit(entity);
                }
                else
                {
                    context.Addons.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
        }

        public static List<Addon> GetAddons(bool showInactives, string searchTerm, string mode)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Addon>(x => x.MixUom);
            opts.LoadWith<Addon>(x => x.QuoteUom);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                var query = context.Addons.AsQueryable();
                if (!showInactives)
                {
                    query = query.Where(x => x.Active.GetValueOrDefault() == true);
                }
                if (mode != null)
                {
                    if (mode == "Mix")
                    {
                        query.Where(x => x.MixUom.MixAddonProduct.GetValueOrDefault() == true ||
                                            x.MixUom.MixAddonFee.GetValueOrDefault() == true ||
                                            x.MixUom.MixAddonService.GetValueOrDefault() == true);
                    }
                    if (mode == "Quote")
                    {
                        query.Where(x => x.MixUom.QuoteAddonFee.GetValueOrDefault() == true ||
                                            x.MixUom.QuoteAddonProduct.GetValueOrDefault() == true ||
                                            x.MixUom.QuoteAddonService.GetValueOrDefault() == true);
                    }
                }
                if (searchTerm != null)
                {
                    query = query.Where(x => x.Description.Contains(searchTerm) || x.Code.Contains(searchTerm) || x.AddonType.Contains(searchTerm));
                }
                return query.OrderBy(x => x.AddonType).ThenBy(x => x.Code).ToList();
            }
        }

        public static List<Addon> GetActiveAddons(string mode, int plantId, DateTime? pricingMonth = null)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Addon>(x => x.MixUom);
            opts.LoadWith<Addon>(x => x.QuoteUom);
            using (var context = GetContext())
            {
                if (pricingMonth == null || DateTime.MinValue == pricingMonth)
                    pricingMonth = DateUtils.GetFirstOf(DateTime.Today);

                context.LoadOptions = opts;
                var plant = context.Plants.Where(x => x.PlantId == plantId).FirstOrDefault();
                var query = context.AddonPriceProjections.Where(x => x.ChangeDate <= pricingMonth)
                    .Where(x => x.Price > 0)
                    .Where(x => x.PriceMode == mode.ToUpper())
                    .Where(x => x.District.DistrictId == plant.DistrictId).AsQueryable();
                query = query.Where(x => x.Addon.Active.GetValueOrDefault() == true);
                var list = query.Select(x => x.Addon).Distinct().ToList();
                if (mode != null)
                {
                    if (mode == "Mix")
                    {
                        list = list.Where(x => x.MixUom.MixAddonProduct.GetValueOrDefault() == true ||
                                            x.MixUom.MixAddonFee.GetValueOrDefault() == true ||
                                            x.MixUom.MixAddonService.GetValueOrDefault() == true).ToList();
                    }
                    if (mode == "Quote")
                    {
                        list = list.Where(x => x.MixUom.QuoteAddonFee.GetValueOrDefault() == true ||
                                            x.MixUom.QuoteAddonProduct.GetValueOrDefault() == true ||
                                            x.MixUom.QuoteAddonService.GetValueOrDefault() == true).ToList();
                    }
                }
                return list.OrderBy(x => x.AddonType).ThenBy(x => x.Code).ToList();
            }
        }


        public static Addon FindAddon(long id)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Addon>(x => x.QuoteUom);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.Addons.First(x => x.Id == id);
            }
        }

        public static List<AddonPriceProjection> GetCurrentAddonQuoteCosts(int districtId, DateTime? costEffectiveDate)
        {
            using (var context = GetContext())
            {
                DateTime currentCostEffectiveDate = DateTime.Today;
                if (costEffectiveDate != null)
                {
                    currentCostEffectiveDate = costEffectiveDate.GetValueOrDefault();
                }

                currentCostEffectiveDate = currentCostEffectiveDate.AddDays(-1 * currentCostEffectiveDate.Day);
                currentCostEffectiveDate = currentCostEffectiveDate.AddMonths(1);
                List<AddonPriceProjection> apps = context.AddonPriceProjections.
                                            Where(x => x.PriceMode == "QUOTE").
                                            Where(x => x.DistrictId == districtId).
                                            Where(x => x.ChangeDate <= currentCostEffectiveDate).
                                            OrderByDescending(x => x.ChangeDate).ToList();
                return apps;
            }
        }

        public static List<AddonPriceProjection> GetLatestAddonCostProjections()
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<AddonPriceProjection>(x => x.Addon);
            opts.LoadWith<AddonPriceProjection>(x => x.District);
            opts.LoadWith<AddonPriceProjection>(x => x.Uom);

            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                var query = context.AddonPriceProjections.Where(x => x.District.Active.GetValueOrDefault() == true).Where(x => x.Addon.Active == true).ToList();
                var finalList = query.GroupBy(x => new { x.Addon, x.District })
                        .Select(x => new
                        {
                            District = x.Key.District,
                            Addon = x.Key.Addon,
                            LatestProjection = x.OrderByDescending(y => y.ChangeDate).FirstOrDefault()
                        }).Where(x => x.LatestProjection != null).Select(x => new AddonPriceProjection
                        {
                            District = x.District,
                            DistrictId = x.District.DistrictId,
                            Addon = x.Addon,
                            AddonId = x.Addon.Id,
                            Price = x.LatestProjection.Price,
                            ChangeDate = x.LatestProjection.ChangeDate,
                            PriceMode = x.LatestProjection.PriceMode,
                            Uom = x.LatestProjection.Uom,
                            UomId = x.LatestProjection.UomId,
                        }).ToList();
                return finalList.OrderBy(x => x.Addon.Code).ThenBy(x => x.District.Name).ToList();
            }
        }

        public static Decimal FindPrice(long addOnId, int districtId, DateTime currentMonth, int future, string priceMode, out bool isActual, long? targetUOM = null)
        {
            DateTime actualDate = currentMonth.AddMonths(future);
            decimal price = 0;
            using (var context = GetContext())
            {
                AddonPriceProjection proj = context.AddonPriceProjections.Where(x => x.AddonId == addOnId).Where(x => x.DistrictId == districtId).
                    Where(x => x.ChangeDate <= actualDate).OrderByDescending(x => x.ChangeDate).Where(x => x.PriceMode == priceMode).FirstOrDefault();

                isActual = context.AddonPriceProjections
                                  .Where(x => x.AddonId == addOnId)
                                  .Where(x => x.DistrictId == districtId)
                                  .Where(x => x.PriceMode == priceMode)
                                  .Where(x => x.ChangeDate == actualDate).Count() > 0;

                if (proj == null)
                {
                    price = 0;
                }
                else
                {
                    if (targetUOM == null)
                    {
                        price = proj.Price;
                    }
                    else
                    {
                        Uom target = context.Uoms.Where(x => x.Id == targetUOM).FirstOrDefault();
                        if (target != null)
                        {
                            var baseConversion = (decimal)target.BaseConversion / (decimal)proj.Uom.BaseConversion;
                            if (baseConversion != 0)
                            {
                                price = proj.Price * baseConversion;
                            }
                            else
                            {
                                price = proj.Price;
                            }
                        }
                        else
                        {
                            price = proj.Price;
                        }
                    }
                }
                return price;
            }
        }

        public static void UpdateAddonProjection(long addonId, int districtId, DateTime dateTime, decimal SavePrice, long SaveUOMId, String priceMode)
        {
            using (var context = GetContext())
            {
                AddonPriceProjection existing = context.AddonPriceProjections.Where(x => x.AddonId == addonId).Where(x => x.DistrictId == districtId).Where(x => x.ChangeDate == dateTime).Where(x => x.PriceMode == priceMode).FirstOrDefault();
                if (existing != null)
                {
                    existing.Price = SavePrice;
                    existing.UomId = SaveUOMId;
                }
                else
                {
                    AddonPriceProjection proj = new AddonPriceProjection();
                    proj.AddonId = addonId;
                    proj.DistrictId = districtId;
                    proj.ChangeDate = dateTime;
                    proj.PriceMode = priceMode;
                    proj.Price = SavePrice;
                    proj.UomId = SaveUOMId;
                    context.AddonPriceProjections.InsertOnSubmit(proj);
                }
                context.SubmitChanges();
            }
        }


        public static string ScrubAddonProjection(string addonCode, string district, string unitCost, string costUom, string priceMode, string date)
        {
            using (var context = GetContext())
            {
                if (addonCode == null)
                    return "Addon code is empty";
                if (district == null)
                    return "District is empty";
                if (unitCost == null)
                    return "Cost is empty";
                if (costUom == null)
                    return "Cost UOM is empty";
                if (date == null)
                    return "Date needs to be present";

                Addon raw = context.Addons.FirstOrDefault(x => x.Code == addonCode && x.Active == true);
                if (raw == null)
                {
                    return "Addon not found";
                }

                District District = context.Districts.FirstOrDefault(x => x.Name == district && x.Active == true);
                if (District == null)
                {
                    return "District was not found";
                }

                decimal Cost = 0;
                Decimal.TryParse(unitCost, out Cost);
                if (Cost == 0)
                {
                    return "Cost cannot be 0";
                }

                Uom CostUom = context.Uoms.FirstOrDefault(x => x.Name == costUom);
                if (CostUom == null)
                {
                    return "The unit of measure is incorrect on unavailable";
                }

                DateTime dateTime = DateTime.Today;

                try
                {
                    dateTime = DateTime.ParseExact(date, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    dateTime = dateTime.AddDays((-1 * dateTime.Day) + 1);
                }
                catch (Exception)
                {
                    return "Date is an incorrect format or empty. It should be 'M/d/yyyy hh:mm:ss tt' format, and not empty";
                }

                if (priceMode != "MIX" && priceMode != "QUOTE")
                    return "Price mode must be MIX or QUOTE";

                AddonPriceProjection existing = context.AddonPriceProjections.
                    Where(x => x.AddonId == raw.Id).
                    Where(x => x.DistrictId == District.DistrictId).
                    Where(x => x.PriceMode == priceMode).
                    Where(x => x.ChangeDate == dateTime).FirstOrDefault();

                if (existing != null)
                {
                    existing.Price = Cost;
                    existing.UomId = CostUom.Id;
                }
                else
                {
                    AddonPriceProjection proj = new AddonPriceProjection();
                    proj.AddonId = raw.Id;
                    proj.DistrictId = District.DistrictId;
                    proj.ChangeDate = dateTime;
                    proj.PriceMode = priceMode;
                    proj.Price = Cost;
                    proj.UomId = CostUom.Id;
                    context.AddonPriceProjections.InsertOnSubmit(proj);
                }
                context.SubmitChanges();
            }
            return null;
        }

        public static Addon FindAddonByCode(string value)
        {
            using (var context = GetContext())
            {
                return context.Addons.FirstOrDefault(x => x.Code == value && x.Active == true);
            }
        }
        #endregion

        #region Unit of Measurement
        public static List<Uom> GetUOMS(long? siblingUOM = null, bool showInactive = false)
        {
            using (var context = GetContext())
            {
                if (siblingUOM != null && siblingUOM.Value > 0)
                {
                    String category = context.Uoms.First(x => x.Id == siblingUOM).Category;
                    return context.Uoms.Where(x => x.Category == category).Where(x => x.Active == !showInactive).OrderBy(x => x.Priority).ToList();
                }
                else
                {
                    return context.Uoms.OrderBy(x => x.Category).Where(x => x.Active == !showInactive).OrderBy(x => x.Priority).ToList();
                }
            }
        }

        public static List<Uom> GetAddonUOMS(string addonType, string mode)
        {
            using (var context = GetContext())
            {
                if (mode == "quote")
                {
                    if (addonType == "Product")
                    {
                        return context.Uoms.Where(x => x.QuoteAddonProduct == true).OrderBy(x => x.Category).ThenBy(x => x.Priority).ToList();
                    }
                    if (addonType == "Fees")
                    {
                        return context.Uoms.Where(x => x.QuoteAddonFee == true).OrderBy(x => x.Category).ThenBy(x => x.Priority).ToList();
                    }
                    if (addonType == "Service Charge")
                    {
                        return context.Uoms.Where(x => x.QuoteAddonService == true).OrderBy(x => x.Category).ThenBy(x => x.Priority).ToList();
                    }
                }
                if (mode == "mix")
                {
                    if (addonType == "Product")
                    {
                        return context.Uoms.Where(x => x.MixAddonProduct == true).OrderBy(x => x.Category).ThenBy(x => x.Priority).ToList();
                    }
                    if (addonType == "Fees")
                    {
                        return context.Uoms.Where(x => x.MixAddonFee == true).OrderBy(x => x.Category).ThenBy(x => x.Priority).ToList();
                    }
                    if (addonType == "Service Charge")
                    {
                        return context.Uoms.Where(x => x.MixAddonService == true).OrderBy(x => x.Category).ThenBy(x => x.Priority).ToList();
                    }
                }
                return new List<Uom>();
            }
        }

        public static List<Uom> GetUOMSByType(string category, bool? priority2Order = null)
        {
            using (var context = GetContext())
            {
                var query = context.Uoms.Where(x => x.Category == category).Where(x => x.Active == true);

                if (priority2Order != null && (true).Equals(priority2Order))
                    query = query.OrderBy(x => x.Priority2);
                else
                    query = query.OrderBy(x => x.Priority);

                return query.ToList();
            }
        }

        public static Uom FindUOM(long? uomId)
        {
            using (var context = GetContext())
            {
                if (uomId == null)
                    return null;
                return context.Uoms.FirstOrDefault(x => x.Id == uomId);
            }
        }
        #endregion

        #region Global Settings / Misc Settings

        public static GlobalSetting GetGlobalSettings()
        {
            using (var context = GetContext())
            {
                GlobalSetting setting = context.GlobalSettings.First();
                if (setting == null)
                {
                    setting = new GlobalSetting();
                    context.GlobalSettings.InsertOnSubmit(setting);
                }
                context.SubmitChanges();
                return setting;
            }
        }

        public static CompanySetting FindOrCreateCompanySettings()
        {
            using (var context = GetContext())
            {
                CompanySetting setting = context.CompanySettings.FirstOrDefault();

                if (setting == null)
                {
                    setting = new CompanySetting();
                    setting.EnableAPI = false;
                    setting.APIEndPoint = "";
                    setting.ClientKey = "";
                    setting.ClientId = "";
                    setting.CreatedAt = DateTime.Now;
                    setting.UpdatedAt = DateTime.Now;

                    context.CompanySettings.InsertOnSubmit(setting);
                    context.SubmitChanges();
                }
                return setting;
            }
        }
        public static void UpdateGlobalSetting(GlobalSetting globalSetting)
        {
            using (var context = GetContext())
            {
                context.GlobalSettings.Attach(globalSetting);
                context.Refresh(RefreshMode.KeepCurrentValues, globalSetting);
                context.SubmitChanges();
            }
        }

        public static void UpdateCompanySetting(CompanySetting companySetting)
        {
            using (var context = GetContext())
            {
                context.CompanySettings.Attach(companySetting);
                context.Refresh(RefreshMode.KeepCurrentValues, companySetting);
                context.SubmitChanges();
            }
        }

        #region Tax Codes

        public static List<TaxCode> GetTaxCodes()
        {
            using (var context = GetContext())
            {
                return context.TaxCodes.OrderBy(x => x.Code).ThenBy(x => x.Description).ToList();
            }
        }

        public static TaxCode FindTaxCode(long id)
        {
            using (var context = GetContext())
            {
                return context.TaxCodes.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public static bool DeleteTaxCode(long id)
        {
            using (var context = GetContext())
            {
                TaxCode tc = context.TaxCodes.Where(x => x.Id == id).FirstOrDefault();
                try
                {
                    context.TaxCodes.DeleteOnSubmit(tc);
                    context.SubmitChanges();
                    return true;
                }
                catch (Exception ex) { return false; }
            }
        }

        public static void AddUpdateTaxCode(TaxCode tc)
        {
            using (var context = GetContext())
            {
                if (tc.Id > 0)
                {
                    context.TaxCodes.Attach(tc);
                    context.Refresh(RefreshMode.KeepCurrentValues, tc);
                }
                else
                {
                    context.TaxCodes.InsertOnSubmit(tc);
                }
                context.SubmitChanges();
            }
        }

        #endregion

        #endregion

        #region Standard Mix

        public static List<StandardMix> GetStandardMixes(out int count, bool showInactives = false, string searchTerm = null, int? page = null, int? rowCount = null)
        {
            using (var context = GetContext())
            {
                var query = context.StandardMixes.AsQueryable();
                if (!showInactives)
                    query = query.Where(x => x.Active == true);
                if (searchTerm != null)
                {
                    query = query.Where(x => x.Number.Contains(searchTerm) || x.Description.Contains(searchTerm) || x.SalesDesc.Contains(searchTerm));
                }
                count = query.Count();
                if (page != null && rowCount != null)
                {
                    query = query.Skip(page.Value * rowCount.Value).Take(rowCount.Value);
                }
                return query.ToList();
            }
        }

        public static string ScrubMixDesigns(string number, string mixDescription, string SalesDescription, string PSI, string Slump, string Air, string UDF1, string UDF2, string UDF3, string UDF4, string userName)
        {
            using (var context = GetContext())
            {
                GlobalSetting setting = GetGlobalSettings();

                if (number == null)
                    return "Number is empty";
                if (mixDescription == null)
                    return "Mix Description is empty";
                if (SalesDescription == null)
                    return "Sales Description is empty";
                if (PSI == null)
                    return "PSI is empty";
                if (Slump == null)
                    return "Slump is empty";
                if (Air == null)
                    return "Air is empty";
                if (UDF1 == null)
                    return setting.MD1 + " is empty";
                if (UDF2 == null)
                    return setting.MD2 + " is empty";
                if (UDF3 == null)
                    return setting.MD3 + " is empty";
                if (UDF4 == null)
                    return setting.MD4 + " is empty";

                StandardMix mix = context.StandardMixes.FirstOrDefault(x => x.Number == number && x.Active == true);
                if (mix == null)
                {
                    mix = new StandardMix();
                    context.StandardMixes.InsertOnSubmit(mix);
                }

                mix.Number = number;
                mix.SalesDesc = SalesDescription;
                mix.Description = mixDescription;
                int psi = 0;
                Int32.TryParse(PSI, out psi);
                mix.PSI = psi;
                mix.Slump = Slump;
                mix.Air = Air;
                mix.MD1 = UDF1;
                mix.MD2 = UDF2;
                mix.MD3 = UDF3;
                mix.MD4 = UDF4;
                mix.Active = true;
                mix.UpdatedBy = userName;
                mix.UpdatedOn = DateTime.Now;
                context.SubmitChanges();
            }
            return null;
        }

        public static List<SIStandardMixPlant> GetStandardMixPlants(Guid userId, out int count, bool showInactives = false, bool noFormulations = false, string searchTerm = null, int? page = null, int? rowCount = null, string sortColumn = "MixNum", string sortDirection = "asc")
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<MixFormulation>(x => x.Plant);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                IQueryable<SIStandardMixPlant> query = null;
                if (noFormulations)
                {
                    query = from s in context.StandardMixes
                            join m in context.MixFormulations on s.Id equals m.StandardMixId
                            into smf
                            from m in smf.DefaultIfEmpty()
                            join f in context.MixFormulationCostProjections on m.Id equals f.MixFormulationId
                            into str
                            from x in str.DefaultIfEmpty()
                            select new SIStandardMixPlant { Mix = s, Formulation = m, Projection = x };
                }
                else
                {
                    query = from s in context.StandardMixes
                            join m in context.MixFormulations on s.Id equals m.StandardMixId
                            join f in context.MixFormulationCostProjections on m.Id equals f.MixFormulationId
                            into str
                            from x in str.DefaultIfEmpty()
                            select new SIStandardMixPlant { Mix = s, Formulation = m, Projection = x };
                }

                if (!noFormulations)
                    query = query.Where(x => x.Projection.AsOfDate == DateUtils.GetFirstOf(DateTime.Today));
                else
                    query = query.Where(x => x.Projection == null || x.Projection.AsOfDate == DateUtils.GetFirstOf(DateTime.Today));

                if (!showInactives)
                {
                    query = query.Where(x => x.Mix.Active == true);

                    if (!noFormulations)
                    {
                        query = query.Where(x => x.Formulation.Plant.Active == true);
                    }
                }
                if (searchTerm != null)
                {
                    query = query.Where(x => x.Mix.Number.Contains(searchTerm) || x.Mix.Description.Contains(searchTerm) || x.Mix.PSI.ToString().Contains(searchTerm) || x.Mix.Air.Contains(searchTerm) || x.Mix.Slump.Contains(searchTerm) || x.Mix.MD1.Contains(searchTerm) || x.Mix.MD2.Contains(searchTerm) || x.Mix.MD3.Contains(searchTerm) || x.Mix.MD4.Contains(searchTerm) || x.Mix.SalesDesc.Contains(searchTerm) || x.Formulation.Plant.Name.Contains(searchTerm));
                }
                //if (zeroCostOnly)
                //{
                //    query = query.Where(x => x.Projection.Cost == 0);
                //}
                #region Sorting
                if (sortColumn == "MixNum")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.Number);
                    else
                        query = query.OrderByDescending(x => x.Mix.Number);
                }
                if (sortColumn == "MixDesc")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.Description);
                    else
                        query = query.OrderByDescending(x => x.Mix.Description);
                }
                if (sortColumn == "SalesDesc")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.SalesDesc);
                    else
                        query = query.OrderByDescending(x => x.Mix.SalesDesc);
                }
                if (sortColumn == "PSI")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.PSI);
                    else
                        query = query.OrderByDescending(x => x.Mix.PSI);
                }
                if (sortColumn == "Air")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.Air);
                    else
                        query = query.OrderByDescending(x => x.Mix.Air);
                }
                if (sortColumn == "Slump")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.Slump);
                    else
                        query = query.OrderByDescending(x => x.Mix.Slump);
                }
                if (sortColumn == "Plant")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Formulation.Plant.Name);
                    else
                        query = query.OrderByDescending(x => x.Formulation.Plant.Name);
                }
                if (sortColumn == "AshPercentage")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Formulation.AshPercentage);
                    else
                        query = query.OrderByDescending(x => x.Formulation.AshPercentage);
                }
                if (sortColumn == "FineAggPercentage")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Formulation.FineAggPercentage);
                    else
                        query = query.OrderByDescending(x => x.Formulation.FineAggPercentage);
                }
                if (sortColumn == "Sacks")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Formulation.Sacks);
                    else
                        query = query.OrderByDescending(x => x.Formulation.Sacks);
                }
                if (sortColumn == "MD1")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.MD1);
                    else
                        query = query.OrderByDescending(x => x.Mix.MD1);
                }
                if (sortColumn == "MD2")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.MD2);
                    else
                        query = query.OrderByDescending(x => x.Mix.MD2);
                }
                if (sortColumn == "MD3")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.MD3);
                    else
                        query = query.OrderByDescending(x => x.Mix.MD3);
                }
                if (sortColumn == "MD4")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.MD4);
                    else
                        query = query.OrderByDescending(x => x.Mix.MD4);
                }
                if (sortColumn == "Active")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Mix.Active);
                    else
                        query = query.OrderByDescending(x => x.Mix.Active);
                }
                if (sortColumn == "Cost")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Projection.Cost);
                    else
                        query = query.OrderByDescending(x => x.Projection.Cost);
                }
                #endregion

                count = query.Count();
                if (page != null && rowCount != null)
                {
                    query = query.Skip(page.Value * rowCount.Value).Take(rowCount.Value);
                }
                return query.ToList();
            }
        }

        public static StandardMix FindStandardMix(long id)
        {
            using (var context = GetContext())
            {
                return context.StandardMixes.FirstOrDefault(x => x.Id == id);
            }
        }

        public static List<SearchMixFormulationsResult> SearchMixFormulations(
            int plantId, DateTime asOfDate,
            int[] psi, string[] airs, string[] slumps,
            double[] ashes, double[] fineAggs, double[] sacks,
            string[] md1s, string[] md2s, string[] md3s, string[] md4s,
            long[] rawMaterialIncluded, long[] rawMaterialExcluded)
        {
            using (var context = GetContext())
            {
                var results = context.SearchMixFormulations(
                    plantId,
                    StringUtils.Commatize(airs),
                    StringUtils.Commatize(slumps),
                    StringUtils.Commatize(psi),
                    StringUtils.Commatize(md1s),
                    StringUtils.Commatize(md2s),
                    StringUtils.Commatize(md3s),
                    StringUtils.Commatize(md4s),
                    StringUtils.Commatize(ashes),
                    StringUtils.Commatize(fineAggs),
                    StringUtils.Commatize(sacks),
                    StringUtils.Commatize(rawMaterialIncluded),
                    StringUtils.Commatize(rawMaterialExcluded),
                    asOfDate
                );
                return results.ToList();
            }
        }

        public static void UpdateStandardMix(StandardMix entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.StandardMixes.InsertOnSubmit(entity);
                }
                else
                {
                    context.StandardMixes.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
        }

        public static void SetMixFormulationAuditTrail(long formulationId, string updatedBy, DateTime updatedOn)
        {
            using (var context = GetContext())
            {
                MixFormulation formulation = context.MixFormulations.FirstOrDefault(x => x.Id == formulationId);
                if (formulation != null)
                {
                    formulation.UpdatedBy = updatedBy;
                    formulation.UpdatedOn = updatedOn;
                }
                context.SubmitChanges();
            }
        }

        #endregion

        #region Mix Formulations

        public static List<RawMaterial> GetCommonRawMaterialsForFomulations(List<long> formulationIds)
        {
            using (var context = GetContext())
            {
                DataLoadOptions dlo = new DataLoadOptions();
                dlo.LoadWith<StandardMixConstituent>(x => x.RawMaterial);
                context.LoadOptions = dlo;
                var standardMixConstituents = context.StandardMixConstituents.ToList();

                return standardMixConstituents.Where(x => formulationIds.Contains(x.MixFormulationId))
                                              .Select(x => x.RawMaterial).Distinct().ToList();
                //return context.StandardMixConstituents.Where(x => formulationIds.Contains(x.MixFormulationId)).Select(x => x.RawMaterial).Distinct().ToList();
            }
        }

        public static MixFormulation FindFormulation(int plantId, long standardMixId)
        {
            using (var context = GetContext())
            {
                return context.MixFormulations.Where(x => x.PlantId == plantId).Where(x => x.StandardMixId == standardMixId).FirstOrDefault();
            }
        }

        public static MixFormulation FindFormulation(long id)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<MixFormulation>(x => x.StandardMix);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.MixFormulations.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public static void UpdateMixFormulation(MixFormulation entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.MixFormulations.InsertOnSubmit(entity);
                }
                else
                {
                    context.MixFormulations.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
        }

        public static void CopyFormulation(long standardMixId, int sourcePlantId, int targetPlantId)
        {
            using (var context = GetContext())
            {
                MixFormulation m = context.MixFormulations.Where(x => x.PlantId == sourcePlantId).Where(x => x.StandardMixId == standardMixId).FirstOrDefault();
                MixFormulation old = context.MixFormulations.Where(x => x.PlantId == targetPlantId).Where(x => x.StandardMixId == standardMixId).FirstOrDefault();
                if (old != null)
                {
                    context.StandardMixConstituents.DeleteAllOnSubmit(old.StandardMixConstituents);
                    context.MixFormulationCostProjections.DeleteAllOnSubmit(old.MixFormulationCostProjections);
                    context.SubmitChanges();

                    context.MixFormulations.DeleteOnSubmit(old);
                    context.SubmitChanges();
                }

                if (m != null)
                {
                    MixFormulation newMix = new MixFormulation();
                    newMix.StandardMixId = standardMixId;
                    newMix.PlantId = targetPlantId;
                    newMix.Description = m.Description;
                    newMix.Number = m.Number;
                    context.MixFormulations.InsertOnSubmit(newMix);
                    context.SubmitChanges();

                    foreach (StandardMixConstituent c in m.StandardMixConstituents)
                    {
                        StandardMixConstituent newConst = new StandardMixConstituent();
                        newConst.MixFormulationId = newMix.Id;
                        newConst.RawMaterialId = c.RawMaterialId;
                        newConst.PerCementWeight = c.PerCementWeight;
                        newConst.Quantity = c.Quantity;
                        newConst.UomId = c.UomId;
                        context.StandardMixConstituents.InsertOnSubmit(newConst);
                        context.SubmitChanges();
                    }

                    UpdateMixFormulationCalculatedFields(newMix.Id);
                    UpdateMixFormulationCostProjections(newMix.Id);
                }
            }
        }

        public static void DeleteMixFormulation(int plantId, long standardMixId)
        {
            using (var context = GetContext())
            {
                MixFormulation mixFormulation = context.MixFormulations.Where(x => x.PlantId == plantId).Where(x => x.StandardMixId == standardMixId).FirstOrDefault();
                if (mixFormulation != null)
                {
                    var MFCP = context.MixFormulationCostProjections.Where(x => x.MixFormulationId == mixFormulation.Id);
                    if (MFCP != null)
                    {
                        context.MixFormulationCostProjections.DeleteAllOnSubmit(MFCP);
                    }
                    var standardMixConstituents = context.StandardMixConstituents.Where(x => x.MixFormulationId == mixFormulation.Id);
                    if (standardMixConstituents != null)
                    {
                        context.StandardMixConstituents.DeleteAllOnSubmit(standardMixConstituents);
                    }
                    context.MixFormulations.DeleteOnSubmit(mixFormulation);
                }
                context.SubmitChanges();
            }
        }

        public static void DeleteMixFormulationAndConstituents(int plantId, long standardMixId)
        {
            using (var context = GetContext())
            {
                var standardMixes = context.StandardMixConstituents
                                         .Where(x => x.MixFormulation.PlantId == plantId)
                                         .Where(x => x.MixFormulation.StandardMixId == standardMixId);

                context.StandardMixConstituents.DeleteAllOnSubmit(standardMixes);
                context.SubmitChanges();

                var mixFormulations = context.MixFormulations
                                             .Where(x => x.PlantId == plantId)
                                             .Where(x => x.StandardMixId == standardMixId);

                context.MixFormulations.DeleteAllOnSubmit(mixFormulations);
                context.SubmitChanges();
            }
        }

        public static MixFormulation FindOrCreateFormulation(int plantId, long standardMixId)
        {
            using (var context = GetContext())
            {
                MixFormulation formulation = context.MixFormulations.Where(x => x.PlantId == plantId).Where(x => x.StandardMixId == standardMixId).FirstOrDefault();
                if (formulation == null)
                {
                    formulation = new MixFormulation();
                    formulation.PlantId = plantId;
                    formulation.StandardMixId = standardMixId;
                    context.MixFormulations.InsertOnSubmit(formulation);
                    context.SubmitChanges();
                }
                return formulation;
            }
        }

        public static void DeleteConstituents(string plant, string mixNumber)
        {
            using (var context = GetContext())
            {
                StandardMix mix = context.StandardMixes.FirstOrDefault(x => x.Number == mixNumber && x.Active == true);
                if (mix == null)
                    return;

                Plant p = context.Plants.FirstOrDefault(x => x.Name == plant);
                if (p == null)
                    return;

                var formulation = context.MixFormulations.Where(x => x.PlantId == p.PlantId).Where(x => x.StandardMixId == mix.Id).FirstOrDefault();
                if (formulation != null)
                {
                    context.StandardMixConstituents.DeleteAllOnSubmit(formulation.StandardMixConstituents);
                    context.SubmitChanges();
                }
            }
        }

        public static string ScrubMixFormulations(string mixNumber, string formulaNumber, string description, string plant, string materialCode, string uom, bool perCw, string qty, string userName, List<long> deletedMixes)
        {
            using (var context = GetContext())
            {

                if (mixNumber == null)
                    return "Mix Number is empty";
                if (formulaNumber == null)
                    return "Formula number is empty";
                if (plant == null)
                    return "Plant is empty";
                if (materialCode == null)
                    return "Material Code is empty";
                if (uom == null)
                    return "UOM needs to be present";
                if (qty == null)
                    return "Qty needs to be present";

                StandardMix mix = context.StandardMixes.FirstOrDefault(x => x.Number == mixNumber && x.Active == true);
                if (mix == null)
                    return "Mix not found with the number specified";

                Plant p = context.Plants.FirstOrDefault(x => x.Name == plant);
                if (p == null)
                    return "Plant not found with the name specified";

                RawMaterial raw = context.RawMaterials.FirstOrDefault(x => x.MaterialCode == materialCode && x.Active == true);
                if (raw == null)
                    return "No Raw Material found with the code";

                Uom unit = context.Uoms.FirstOrDefault(x => x.Name == uom);
                if (unit == null)
                    return "No UOM Found";

                double quantity = 0;
                Double.TryParse(qty, out quantity);

                if (quantity == 0)
                    return "Quantity cannot be 0";


                MixFormulation formulation = FindOrCreateFormulation(p.PlantId, mix.Id);

                formulation = context.MixFormulations.Where(x => x.Id == formulation.Id).FirstOrDefault();
                formulation.Number = formulaNumber;
                formulation.Description = description;
                formulation.UpdatedBy = userName;
                formulation.UpdatedOn = DateTime.Now;
                if (!deletedMixes.Contains(formulation.Id))
                {
                    context.StandardMixConstituents.DeleteAllOnSubmit(formulation.StandardMixConstituents);
                    context.SubmitChanges();
                    deletedMixes.Add(formulation.Id);
                }

                StandardMixConstituent c = context.StandardMixConstituents.Where(x => x.RawMaterialId == raw.Id).Where(x => x.MixFormulationId == formulation.Id).FirstOrDefault();
                if (c == null)
                {
                    c = new StandardMixConstituent();
                    c.MixFormulationId = formulation.Id;
                    c.RawMaterialId = raw.Id;
                    context.StandardMixConstituents.InsertOnSubmit(c);
                }
                c.Quantity = quantity;
                c.UomId = unit.Id;
                c.PerCementWeight = perCw;
                context.SubmitChanges();
            }
            return null;
        }

        public static bool CheckConstituentValidity(long standardMixId, int plantId)
        {
            using (var context = GetContext())
            {
                MixFormulation formulation = context.MixFormulations.Where(x => x.PlantId == plantId).Where(x => x.StandardMixId == standardMixId).FirstOrDefault();
                if (formulation != null)
                {
                    List<SIQuotationMixIngredient> quoteMixIngredients = new List<SIQuotationMixIngredient>();
                    DateTime CostEffectiveDate = DateTime.Today;
                    CostEffectiveDate = CostEffectiveDate.AddDays(-1 * CostEffectiveDate.Day).AddMonths(1);
                    foreach (StandardMixConstituent ingredient in formulation.StandardMixConstituents)
                    {
                        SIQuotationMixIngredient six = new SIQuotationMixIngredient(ingredient);
                        RawMaterial r = ingredient.RawMaterial;
                        RawMaterialCostProjection projection = context.RawMaterialCostProjections
                                                                        .Where(x => x.RawMaterialId == r.Id)
                                                                        .Where(x => x.PlantId == plantId)
                                                                        .Where(x => x.ChangeDate < CostEffectiveDate)
                                                                        .OrderByDescending(x => x.ChangeDate)
                                                                        .FirstOrDefault();
                        if (projection == null || projection.Cost == 0)
                        {
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        // Update the Ash, Fine Agg, and Sack Percentages
        public static void UpdateMixFormulationCalculatedFields(long mixFormulationId)
        {
            using (var context = GetContext())
            {
                MixFormulation mf = context.MixFormulations.FirstOrDefault(x => x.Id == mixFormulationId);
                SIMixFormulationCalculations calculations = new SIMixFormulationCalculations(mf);
                mf.AshPercentage = Convert.ToDecimal(calculations.TotalAshPercentage);
                mf.FineAggPercentage = Convert.ToDecimal(calculations.TotalFineAggPercentage);
                mf.Sacks = Convert.ToDecimal(calculations.TotalSacks);
                context.SubmitChanges();
            }
        }

        public static void RefreshAllMixFormulationCalculations()
        {
            using (var context = GetContext())
            {
                foreach (var mf in context.MixFormulations)
                {
                    UpdateMixFormulationCalculatedFields(mf.Id);
                }
            }
        }

        // Returns a datastructure containing the Rawmaterial and cost for the standard mix and plant.
        public static List<SIMixIngredientPriceSheetRow> GetMixPricingSheet(long standardMixId, int plantId, DateTime? CostEffectiveDate = null)
        {
            using (var context = GetContext())
            {
                MixFormulation formulation = context.MixFormulations.Where(x => x.PlantId == plantId).Where(x => x.StandardMixId == standardMixId).FirstOrDefault();
                if (formulation != null)
                {
                    List<SIMixIngredientPriceSheetRow> quoteMixIngredients = new List<SIMixIngredientPriceSheetRow>();
                    if (CostEffectiveDate == null)
                        CostEffectiveDate = DateTime.Today;

                    CostEffectiveDate = DateUtils.GetFirstOf(CostEffectiveDate.Value); // Get the first of the month.
                    CostEffectiveDate = CostEffectiveDate.Value.AddMonths(1); // Next month's first date.
                    foreach (StandardMixConstituent ingredient in formulation.StandardMixConstituents)
                    {
                        SIMixIngredientPriceSheetRow six = new SIMixIngredientPriceSheetRow(ingredient);
                        RawMaterialCostProjection projection = context.RawMaterialCostProjections
                                                                        .Where(x => x.RawMaterialId == six.RawMaterialId)
                                                                        .Where(x => x.PlantId == plantId)
                                                                        .Where(x => x.ChangeDate < CostEffectiveDate)
                                                                        .OrderByDescending(x => x.ChangeDate)
                                                                        .FirstOrDefault();
                        if (projection != null && projection.Cost > 0)
                        {
                            six.Cost = projection.Cost;
                            six.CostUom = projection.Uom;
                            quoteMixIngredients.Add(six);
                        }
                    }
                    return quoteMixIngredients;
                }
                return null;
            }
        }

        // This method updates the mix formulation cost projection for a given date.
        public static MixFormulationCostProjection RefreshMixFormulationCosts(long mixFormulationId, DateTime asOfDate)
        {
            using (var context = GetContext())
            {
                MixFormulation mf = context.MixFormulations.Where(x => x.Id == mixFormulationId).FirstOrDefault();
                decimal cost = CalculateCurrentMixCost(mf.StandardMixId, mf.PlantId, asOfDate);
                asOfDate = DateUtils.GetFirstOf(asOfDate.Month, asOfDate.Year);

                var projections = context.MixFormulationCostProjections.Where(x => x.MixFormulationId == mixFormulationId).Where(x => x.AsOfDate == asOfDate).ToList();

                if (projections.Count() != 0)
                {
                    foreach (var item in projections)
                    {
                        item.Cost = cost;
                        context.SubmitChanges();
                    }
                }
                else
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        var projection = new MixFormulationCostProjection();
                        projection.Cost = cost;
                        projection.AsOfDate = asOfDate.AddMonths(i);
                        projection.MixFormulationId = mixFormulationId;
                        context.MixFormulationCostProjections.InsertOnSubmit(projection);
                        context.SubmitChanges();
                    }
                }
                return projections.FirstOrDefault();
            }
        }

        // This method updates all the mix formulations for a given date.
        public static void RefreshAllMixFormulationCosts(DateTime asOfDate)
        {
            using (var context = GetContext())
            {
                foreach (var mf in context.MixFormulations)
                {
                    RefreshMixFormulationCosts(mf.Id, asOfDate);
                }
            }
        }

        // This method Updates all mixes associated with a rawmaterial and plant for the given date.
        public static void UpdateMixFormulationCostForRawMaterialCostProjection(long rawMaterialCostProjectionId)
        {
            using (var context = GetContext())
            {
                RawMaterialCostProjection proj = context.RawMaterialCostProjections.Where(x => x.Id == rawMaterialCostProjectionId).FirstOrDefault();
                if (proj != null)
                {
                    var mixes = context.StandardMixConstituents.Where(x => x.MixFormulation.PlantId == proj.PlantId).Where(x => x.RawMaterialId == proj.RawMaterialId).Select(x => x.MixFormulationId);
                    foreach (var mixId in mixes)
                    {
                        RefreshMixFormulationCosts(mixId, proj.ChangeDate);
                    }
                }
            }
        }

        public static decimal GetMixFormulationCostForDate(long mixFormulationId, DateTime? asOfDate = null)
        {
            if (asOfDate == null)
            {
                asOfDate = DateUtils.GetFirstOf(DateTime.Today.Month, DateTime.Today.Year);
            }
            else
            {
                asOfDate = DateUtils.GetFirstOf(asOfDate.Value.Month, asOfDate.Value.Year);
            }
            using (var context = GetContext())
            {
                var projection = context.MixFormulationCostProjections.Where(x => x.AsOfDate == asOfDate).Where(x => x.MixFormulationId == mixFormulationId).FirstOrDefault();
                if (projection == null)
                {
                    projection = RefreshMixFormulationCosts(mixFormulationId, asOfDate.Value);
                }
                return projection.Cost;
            }
        }





        /*This method Updates all the projections for a mixFormulation after changes to it's constituents.*/
        public static void UpdateMixFormulationCostProjections(long mixFormulationId)
        {
            using (var context = GetContext())
            {
                MixFormulation mix = context.MixFormulations.Where(x => x.Id == mixFormulationId).FirstOrDefault();
                if (mixFormulationId != null)
                {
                    if (mix.MixFormulationCostProjections.Any())
                    {
                        foreach (var proj in mix.MixFormulationCostProjections)
                        {
                            RefreshMixFormulationCosts(proj.MixFormulationId, proj.AsOfDate);
                        }
                    }
                    else
                    {
                        RefreshMixFormulationCosts(mix.Id, DateTime.Today);
                    }
                }
            }
        }

        public static List<MixFormulation> GetAllMixFormulations()
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<MixFormulation>(x => x.StandardMix);
            opts.LoadWith<MixFormulation>(x => x.Plant);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.MixFormulations.Where(x => x.StandardMix.Active.GetValueOrDefault() == true).Where(x => x.Plant.Active.GetValueOrDefault() == true).ToList();
            }
        }

        // This method does the actual calculation on the mix cost based on the raw material prices and composition.
        public static decimal CalculateCurrentMixCost(long standardMixId, int plantId, DateTime? asOfDate = null)
        {
            using (var context = GetContext())
            {
                var quoteMixIngredients = GetQuoteMixIngredientsWithCosts(standardMixId, plantId, asOfDate);
                if (quoteMixIngredients != null && quoteMixIngredients.Where(x => x.Cost == 0).Count() == 0)
                {
                    return SIQuotationMixIngredient.CalculateStandardMixTotal(quoteMixIngredients);
                }
                return 0;
            }
        }

        public static List<SIQuotationMixIngredient> GetQuoteMixIngredientsWithCosts(long standardMixId, int plantId, DateTime? asOfDate)
        {
            List<SIQuotationMixIngredient> quoteMixIngredients = new List<SIQuotationMixIngredient>();
            DateTime CostEffectiveDate = DateTime.Today;
            if (asOfDate != null)
            {
                CostEffectiveDate = asOfDate.Value;
            }

            CostEffectiveDate = DateUtils.GetFirstOf(CostEffectiveDate).AddMonths(1);

            using (var context = GetContext())
            {
                MixFormulation formulation = context.MixFormulations.Where(x => x.PlantId == plantId).Where(x => x.StandardMixId == standardMixId).FirstOrDefault();
                if (formulation == null)
                    return null;

                foreach (StandardMixConstituent ingredient in formulation.StandardMixConstituents)
                {
                    SIQuotationMixIngredient six = new SIQuotationMixIngredient(ingredient);
                    RawMaterial r = ingredient.RawMaterial;
                    RawMaterialCostProjection projection = context.RawMaterialCostProjections
                                                                    .Where(x => x.RawMaterialId == r.Id)
                                                                    .Where(x => x.PlantId == plantId)
                                                                    .Where(x => x.ChangeDate < CostEffectiveDate)
                                                                    .OrderByDescending(x => x.ChangeDate)
                                                                    .FirstOrDefault();
                    if (projection != null && projection.Cost > 0)
                    {
                        six.Cost = projection.Cost;
                        six.CostUOM = projection.Uom;
                    }
                    else
                    {
                        six.Cost = 0;
                    }
                    quoteMixIngredients.Add(six);
                }
                return quoteMixIngredients;
            }
        }

        public static List<int> GetPSIValues()
        {
            using (var context = GetContext())
            {
                return context.StandardMixes.Where(x => x.Active == true).Select(x => x.PSI.GetValueOrDefault()).Distinct().OrderBy(x => x).ToList();
            }
        }

        public static List<string> GetAirValues()
        {
            using (var context = GetContext())
            {
                return context.StandardMixes.Where(x => x.Active == true).Select(x => x.Air).Distinct().OrderBy(x => x).ToList();
            }
        }

        public static List<string> GetSlumpValues()
        {
            using (var context = GetContext())
            {
                return context.StandardMixes.Where(x => x.Active == true).Select(x => x.Slump).Distinct().OrderBy(x => x).ToList();
            }
        }


        public static List<string> GetMixCustom1Values()
        {
            using (var context = GetContext())
            {
                return context.StandardMixes.Where(x => x.Active == true).Select(x => x.MD1).Distinct().OrderBy(x => x).ToList();
            }
        }

        public static List<string> GetMixCustom2Values()
        {
            using (var context = GetContext())
            {
                return context.StandardMixes.Where(x => x.Active == true).Select(x => x.MD2).Distinct().OrderBy(x => x).ToList();
            }
        }

        public static List<string> GetMixCustom3Values()
        {
            using (var context = GetContext())
            {
                return context.StandardMixes.Where(x => x.Active == true).Select(x => x.MD3).Distinct().OrderBy(x => x).ToList();
            }
        }

        public static List<string> GetMixCustom4Values()
        {
            using (var context = GetContext())
            {
                return context.StandardMixes.Where(x => x.Active == true).Select(x => x.MD4).Distinct().OrderBy(x => x).ToList();
            }
        }

        public static void UpdateMixFormulationsScrubbedStatus(bool isScrubbed)
        {
            using (var context = GetContext())
            {
                var mixFormulations = context.MixFormulations.ToList();

                if (mixFormulations != null)
                {
                    foreach (var item in mixFormulations)
                    {
                        item.IsScrubbed = isScrubbed;
                    }
                }
                context.SubmitChanges();
            }
        }

        public static void UpdateMixFormulationScrubbedStatus(long mixFormulationId, bool isScrubbed)
        {
            using (var context = GetContext())
            {
                var mixFormulation = context.MixFormulations.FirstOrDefault(x => x.Id == mixFormulationId);

                if (mixFormulation != null)
                {
                    mixFormulation.IsScrubbed = isScrubbed;
                }
                context.SubmitChanges();
            }
        }

        public static void DeleteUnScrubbedMixFormulations()
        {
            using (var context = GetContext())
            {
                var mixFormulationCostProjections = context.MixFormulationCostProjections
                                                           .Where(x => x.MixFormulation.IsScrubbed == false);

                if (mixFormulationCostProjections != null)
                {
                    context.MixFormulationCostProjections.DeleteAllOnSubmit(mixFormulationCostProjections);
                    context.SubmitChanges();
                }

                var standarMixConstituents = context.StandardMixConstituents
                                                   .Where(x => x.MixFormulation.IsScrubbed == false);
                context.StandardMixConstituents.DeleteAllOnSubmit(standarMixConstituents);
                context.SubmitChanges();

                var mixFormulations = context.MixFormulations
                                             .Where(x => x.IsScrubbed == false);
                context.MixFormulations.DeleteAllOnSubmit(mixFormulations);
                context.SubmitChanges();
            }
        }

        #endregion

        #region Mix Constituents

        public static StandardMixConstituent FindMixConstituent(long id)
        {
            using (var context = GetContext())
            {
                return context.StandardMixConstituents.FirstOrDefault(x => x.Id == id);
            }
        }

        public static List<StandardMixConstituent> GetMixConstituents(long? mixFormulationId = null)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<StandardMixConstituent>(x => x.RawMaterial);
            opts.LoadWith<RawMaterial>(x => x.RawMaterialType);
            opts.LoadWith<StandardMixConstituent>(x => x.Uom);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                var query = context.StandardMixConstituents.AsQueryable();

                if (mixFormulationId > 0)
                {
                    query = query.Where(x => x.MixFormulationId == mixFormulationId);
                }
                return query.ToList();
            }
        }

        public static void UpdateStandardMixConstituent(StandardMixConstituent entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.StandardMixConstituents.InsertOnSubmit(entity);
                }
                else
                {
                    context.StandardMixConstituents.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
                UpdateMixFormulationCostProjections(entity.MixFormulationId);
            }
        }

        public static void DeleteMixConstituent(long id)
        {
            using (var context = GetContext())
            {
                StandardMixConstituent c = context.StandardMixConstituents.FirstOrDefault(x => x.Id == id);
                if (c != null)
                {
                    context.StandardMixConstituents.DeleteOnSubmit(c);
                    context.SubmitChanges();
                    UpdateMixFormulationCostProjections(c.MixFormulationId);
                }
            }
        }

        public static void DeleteMixConstituents(long mixFormulationId, long? rawMaterialId = null)
        {
            using (var db = GetContext())
            {
                var mixConstituents = db.StandardMixConstituents
                                        .Where(x => x.MixFormulationId == mixFormulationId);

                if (rawMaterialId > 0)
                    mixConstituents = mixConstituents.Where(x => x.RawMaterialId == rawMaterialId);

                db.StandardMixConstituents.DeleteAllOnSubmit(mixConstituents);
                db.SubmitChanges();
            }
        }

        #endregion

        #region Quotations

        //public static List<Quotation> GetQuotations(Guid userId, int[] districtIds, int[] plantIds, int[] salesStaffIds, bool showInactives, string searchTerm, int pageNum, int numRows, string sortColumn, string sortDirection, out int count)
        //{
        //    using (var context = GetContext())
        //    {
        //        context.LoadOptions = GetQuotationLoadOptions();
        //        var queryCount = GetQuotationsQuery(userId, districtIds, plantIds, salesStaffIds, showInactives, searchTerm, sortColumn, sortDirection, context);
        //        var queryPage = GetQuotationsQuery(userId, districtIds, plantIds, salesStaffIds, showInactives, searchTerm, sortColumn, sortDirection, context);
        //        try
        //        {
        //            IOrderedQueryable<Quotation> finalRes = null;
        //            if (sortColumn == "Customer")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.Customer.Name);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Customer.Name);
        //            }

        //            if (sortColumn == "Id" || sortColumn == null)
        //            {
        //                if (sortDirection == "asc" || sortDirection == null)
        //                    finalRes = queryPage.OrderBy(x => x.Id);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Id);
        //            }

        //            if (sortColumn == "Status")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.Status);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Status);
        //            }
        //            if (sortColumn == "Awarded")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.Awarded);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Awarded);
        //            }
        //            if (sortColumn == "Project")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.Project.Name);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Project.Name);
        //            }

        //            if (sortColumn == "QuoteDate")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.QuoteDate);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.QuoteDate);
        //            }
        //            if (sortColumn == "AcceptanceExpirationDate")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.AcceptanceExpirationDate);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.AcceptanceExpirationDate);
        //            }
        //            if (sortColumn == "QuoteExpirationDate")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.QuoteExpirationDate);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.QuoteExpirationDate);
        //            }
        //            if (sortColumn == "MarketSegment")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.Project.MarketSegment.Name);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Project.MarketSegment.Name);
        //            }
        //            if (sortColumn == "Plant")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.Plant.Name);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Plant.Name);
        //            }
        //            if (sortColumn == "SalesStaff")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.SalesStaff.Name);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.SalesStaff.Name);
        //            }
        //            if (sortColumn == "Volume")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.TotalVolume);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.TotalVolume);
        //            }
        //            if (sortColumn == "Revenue")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.TotalRevenue);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.TotalRevenue);
        //            }
        //            if (sortColumn == "Price")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.AvgSellingPrice);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.AvgSellingPrice);
        //            }
        //            if (sortColumn == "Spread")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.Spread);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Spread);
        //            }
        //            if (sortColumn == "Profit")
        //            {
        //                if (sortDirection == "asc")
        //                    finalRes = queryPage.OrderBy(x => x.Profit);
        //                else
        //                    finalRes = queryPage.OrderByDescending(x => x.Profit);
        //            }
        //            count = queryCount.Count();
        //            return finalRes.Skip(pageNum).Take(numRows).ToList();
        //        }
        //        catch
        //        {

        //            count = 10;
        //            return null;
        //        }

        //    }
        //}

        //private static IQueryable<Quotation> GetQuotationsQuery(Guid userId, int[] districtIds, int[] plantIds, int[] salesStaffIds, bool showInactives, string searchTerm, string sortColumn, string sortDirection, SalesInsightDataContext context)
        //{
        //    var query = Enumerable.Empty<Quotation>().AsQueryable();
        //    if (showInactives != true)
        //    {
        //        query = context.ExecuteQuery<Quotation>("Select * from quotation where id in(" +
        //                                "select Id from quotation where ProjectId not in(" +
        //                                "select ProjectId from(select COUNT(projectId) as TotalProject,ProjectId from Quotation group by projectId) as tmp " +
        //                                "where TotalProject>1) and Active=1 " +
        //                                "Union " +
        //                                "Select Id from quotation where projectId in(" +
        //                                "Select qot.ProjectId from quotation qot inner join(" +
        //                                "select ProjectId from(select COUNT(projectId) as TotalProject,ProjectId from Quotation group by projectId) as tmp " +
        //                                "where TotalProject>1) tmp on qot.ProjectId=tmp.ProjectId where qot.Awarded=1 group by qot.ProjectId) and active=1 and awarded=1 " +
        //                                "union " +
        //                                "Select Id from Quotation where projectId in(" +
        //                                "select ProjectId from(select COUNT(projectId) as TotalProject,ProjectId from Quotation group by projectId) as tmp " +
        //                                "where TotalProject>1 and projectid not in (Select qot.ProjectId from quotation qot inner join(" +
        //                                "select ProjectId from(select COUNT(projectId) as TotalProject,ProjectId from Quotation group by projectId) as tmp " +
        //                                "where TotalProject>1) tmp on qot.ProjectId=tmp.ProjectId where qot.Awarded=1)) and active=1)").AsQueryable();
        //    }
        //    else
        //    {
        //        query = context.Quotations.AsQueryable();
        //    }

        //    if (districtIds == null || districtIds.Count() == 0)
        //    {
        //        districtIds = GetDistricts(userId).Select(x => x.DistrictId).ToArray();
        //    }
        //    query = query.Where(x => districtIds.Contains(x.Plant.DistrictId));
        //    if (plantIds != null && plantIds.Count() > 0)
        //    {
        //        query = query.Where(x => plantIds.Contains(x.PlantId.GetValueOrDefault()));
        //    }
        //    if (salesStaffIds != null && salesStaffIds.Count() > 0)
        //    {
        //        query = query.Where(x => salesStaffIds.Contains(x.Project.ProjectSalesStaffs.FirstOrDefault().SalesStaffId));
        //    }
        //    if (showInactives != true)
        //    {
        //        query = query.Where(x => x.Active == true).
        //            Where(x => x.QuoteExpirationDate != null).
        //            Where(x => x.QuoteExpirationDate.Value >= DateTime.Today).
        //            Where(x => x.Project.ProjectStatus.StatusType != SIStatusType.LostBid.Id);
        //    }
        //    if (searchTerm != null)
        //    {
        //        query = query.Where(x => x.Project.Name.Contains(searchTerm) || x.Id.ToString() == searchTerm || x.Project.MarketSegment.Name.Contains(searchTerm) || x.Project.ProjectSalesStaffs.First().SalesStaff.Name.Contains(searchTerm) || x.Customer.Name.Contains(searchTerm) || x.Plant.Name.Contains(searchTerm));
        //    }

        //    //if (sortColumn == "Id" || sortColumn == null)
        //    //{
        //    //    if (sortDirection == "asc" || sortDirection == null)
        //    //        query = query.OrderBy(x => x.Id);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.Id);
        //    //}
        //    //if (sortColumn == "Status")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.Status);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.Status);
        //    //}
        //    //if (sortColumn == "Awarded")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.Awarded);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.Awarded);
        //    //}
        //    //if (sortColumn == "Project")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.Project.Name);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.Project.Name);
        //    //}
        //    //if (sortColumn == "Customer")
        //    //{
        //    //    //if (sortDirection == "asc")
        //    //    //    query = query.OrderBy(x => x.Customer.Name);
        //    //    //else
        //    //    //    query = query.OrderByDescending(x => x.Customer.Name);
        //    //}
        //    //if (sortColumn == "QuoteDate")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.QuoteDate);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.QuoteDate);
        //    //}
        //    //if (sortColumn == "AcceptanceExpirationDate")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.AcceptanceExpirationDate);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.AcceptanceExpirationDate);
        //    //}
        //    //if (sortColumn == "QuoteExpirationDate")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.QuoteExpirationDate);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.QuoteExpirationDate);
        //    //}
        //    //if (sortColumn == "MarketSegment")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.Project.MarketSegment.Name);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.Project.MarketSegment.Name);
        //    //}
        //    //if (sortColumn == "Plant")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.Plant.Name);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.Plant.Name);
        //    //}
        //    //if (sortColumn == "SalesStaff")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.SalesStaff.Name);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.SalesStaff.Name);
        //    //}
        //    //if (sortColumn == "Volume")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.TotalVolume);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.TotalVolume);
        //    //}
        //    //if (sortColumn == "Revenue")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.TotalRevenue);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.TotalRevenue);
        //    //}
        //    //if (sortColumn == "Price")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.AvgSellingPrice);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.AvgSellingPrice);
        //    //}
        //    //if (sortColumn == "Spread")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.Spread);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.Spread);
        //    //}
        //    //if (sortColumn == "Profit")
        //    //{
        //    //    if (sortDirection == "asc")
        //    //        query = query.OrderBy(x => x.Profit);
        //    //    else
        //    //        query = query.OrderByDescending(x => x.Profit);
        //    //}
        //    return query;
        //}

        public static List<Quotation> GetQuotations(Guid userId, int[] districtIds, int[] plantIds, int[] salesStaffIds, bool showInactives, string searchTerm, int pageNum, int numRows, string sortColumn, string sortDirection, out int count)
        {
            using (var context = GetContext())
            {
                context.LoadOptions = GetQuotationLoadOptions();
                var query = context.Quotations.AsQueryable();
                if (districtIds == null || districtIds.Count() == 0)
                {
                    districtIds = GetDistricts(userId).Select(x => x.DistrictId).ToArray();
                }
                query = query.Where(x => districtIds.Contains(x.Plant.DistrictId));
                if (plantIds != null && plantIds.Count() > 0)
                {
                    query = query.Where(x => plantIds.Contains(x.PlantId.GetValueOrDefault()));
                }
                if (salesStaffIds != null && salesStaffIds.Count() > 0)
                {
                    // query = query.Where(x => salesStaffIds.Contains(x.Project.ProjectSalesStaffs.FirstOrDefault().SalesStaffId));
                    query = query.Where(x => salesStaffIds.Contains(x.SalesStaffId.GetValueOrDefault()));

                }
                if (showInactives != true)
                {
                    query = query.Where(x => x.Active == true)
                                 .Where(x => x.QuoteExpirationDate != null)
                                 .Where(x => x.QuoteExpirationDate.Value >= DateTime.Today)
                                 .Where(x => x.Project.ProjectStatus.StatusType != SIStatusType.LostBid.Id)
                                 .Where(x => x.Awarded == true || x.Awarded == null);
                }

                if (searchTerm != null && searchTerm.Trim() != "")
                {
                    query = query.Where(x => x.Project.Name.Contains(searchTerm) || x.Id.ToString() == searchTerm || x.Project.MarketSegment.Name.Contains(searchTerm) || x.Project.ProjectSalesStaffs.First().SalesStaff.Name.Contains(searchTerm) || x.Customer.Name.Contains(searchTerm) || x.Plant.Name.Contains(searchTerm));
                }
                count = query.Count();
                if (sortColumn == "Id" || sortColumn == null)
                {
                    if (sortDirection == "asc" || sortDirection == null)
                        query = query.OrderBy(x => x.Id);
                    else
                        query = query.OrderByDescending(x => x.Id);
                }
                if (sortColumn == "Status")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Status);
                    else
                        query = query.OrderByDescending(x => x.Status);
                }
                if (sortColumn == "Awarded")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Awarded);
                    else
                        query = query.OrderByDescending(x => x.Awarded);
                }
                if (sortColumn == "Project")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Project.Name);
                    else
                        query = query.OrderByDescending(x => x.Project.Name);
                }
                if (sortColumn == "Customer")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Customer.Name);
                    else
                        query = query.OrderByDescending(x => x.Customer.Name);
                }
                if (sortColumn == "QuoteDate")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.QuoteDate);
                    else
                        query = query.OrderByDescending(x => x.QuoteDate);
                }
                if (sortColumn == "AcceptanceExpirationDate")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.AcceptanceExpirationDate);
                    else
                        query = query.OrderByDescending(x => x.AcceptanceExpirationDate);
                }
                if (sortColumn == "QuoteExpirationDate")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.QuoteExpirationDate);
                    else
                        query = query.OrderByDescending(x => x.QuoteExpirationDate);
                }
                if (sortColumn == "MarketSegment")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Project.MarketSegment.Name);
                    else
                        query = query.OrderByDescending(x => x.Project.MarketSegment.Name);
                }
                if (sortColumn == "Plant")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Plant.Name);
                    else
                        query = query.OrderByDescending(x => x.Plant.Name);
                }
                if (sortColumn == "SalesStaff")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.SalesStaff.Name);
                    else
                        query = query.OrderByDescending(x => x.SalesStaff.Name);
                }
                if (sortColumn == "Volume")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.TotalVolume);
                    else
                        query = query.OrderByDescending(x => x.TotalVolume);
                }
                if (sortColumn == "Revenue")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.TotalRevenue);
                    else
                        query = query.OrderByDescending(x => x.TotalRevenue);
                }
                if (sortColumn == "Price")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.AvgSellingPrice);
                    else
                        query = query.OrderByDescending(x => x.AvgSellingPrice);
                }
                if (sortColumn == "Spread")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Spread);
                    else
                        query = query.OrderByDescending(x => x.Spread);
                }
                if (sortColumn == "Profit")
                {
                    if (sortDirection == "asc")
                        query = query.OrderBy(x => x.Profit);
                    else
                        query = query.OrderByDescending(x => x.Profit);
                }
                return query.Skip(pageNum).Take(numRows).ToList();
            }
        }

        //public static List<Quotation> GetQuotationList(Guid userId, int[] districtIds, int[] plantIds, int[] salesStaffIds, bool showInactives, string searchTerm, int pageNum, int numRows, string sortColumn, string sortDirection, out int count)
        //{
        //    using (var context = GetContext())
        //    {
        //        //context.LoadOptions = GetQuotationLoadOptions();
        //        var query = context.Quotations.AsQueryable();
        //        if (districtIds == null || districtIds.Count() == 0)
        //        {
        //            districtIds = GetDistricts(userId).Select(x => x.DistrictId).ToArray();
        //        }
        //        query = query.Where(x => districtIds.Contains(x.Plant.DistrictId));
        //        if (plantIds != null && plantIds.Count() > 0)
        //        {
        //            query = query.Where(x => plantIds.Contains(x.PlantId.GetValueOrDefault()));
        //        }
        //        if (salesStaffIds != null && salesStaffIds.Count() > 0)
        //        {
        //            // query = query.Where(x => salesStaffIds.Contains(x.Project.ProjectSalesStaffs.FirstOrDefault().SalesStaffId));
        //            query = query.Where(x => salesStaffIds.Contains(x.SalesStaffId.GetValueOrDefault()));

        //        }
        //        if (showInactives != true)
        //        {
        //            query = query.Where(x => x.Active == true)
        //                         .Where(x => x.QuoteExpirationDate != null)
        //                         .Where(x => x.QuoteExpirationDate.Value >= DateTime.Today)
        //                         .Where(x => x.Project.ProjectStatus.StatusType != SIStatusType.LostBid.Id)
        //                         .Where(x => x.Awarded == true || x.Awarded == null);
        //        }

        //        if (searchTerm != null && searchTerm.Trim() != "")
        //        {
        //            query = query.Where(x => x.ProjectName.Contains(searchTerm) || x.Id.ToString() == searchTerm || x.MarketSegmentName.Contains(searchTerm) || x.SalesStaffName.Contains(searchTerm) || x.CustomerName.Contains(searchTerm) || x.PlantName.Contains(searchTerm));
        //        }
        //        count = query.Count();
        //        if (sortColumn == "Id" || sortColumn == null)
        //        {
        //            if (sortDirection == "asc" || sortDirection == null)
        //                query = query.OrderBy(x => x.Id);
        //            else
        //                query = query.OrderByDescending(x => x.Id);
        //        }
        //        if (sortColumn == "Status")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.Status);
        //            else
        //                query = query.OrderByDescending(x => x.Status);
        //        }
        //        if (sortColumn == "Awarded")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.Awarded);
        //            else
        //                query = query.OrderByDescending(x => x.Awarded);
        //        }
        //        if (sortColumn == "Project")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.ProjectName);
        //            else
        //                query = query.OrderByDescending(x => x.ProjectName);
        //        }
        //        if (sortColumn == "Customer")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.CustomerName);
        //            else
        //                query = query.OrderByDescending(x => x.CustomerName);
        //        }
        //        if (sortColumn == "QuoteDate")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.QuoteDate);
        //            else
        //                query = query.OrderByDescending(x => x.QuoteDate);
        //        }
        //        if (sortColumn == "AcceptanceExpirationDate")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.AcceptanceExpirationDate);
        //            else
        //                query = query.OrderByDescending(x => x.AcceptanceExpirationDate);
        //        }
        //        if (sortColumn == "QuoteExpirationDate")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.QuoteExpirationDate);
        //            else
        //                query = query.OrderByDescending(x => x.QuoteExpirationDate);
        //        }
        //        if (sortColumn == "MarketSegment")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.MarketSegmentName);
        //            else
        //                query = query.OrderByDescending(x => x.MarketSegmentName);
        //        }
        //        if (sortColumn == "Plant")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.PlantName);
        //            else
        //                query = query.OrderByDescending(x => x.PlantName);
        //        }
        //        if (sortColumn == "SalesStaff")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.SalesStaffName);
        //            else
        //                query = query.OrderByDescending(x => x.SalesStaffName);
        //        }
        //        if (sortColumn == "Volume")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.TotalVolume);
        //            else
        //                query = query.OrderByDescending(x => x.TotalVolume);
        //        }
        //        if (sortColumn == "Revenue")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.TotalRevenue);
        //            else
        //                query = query.OrderByDescending(x => x.TotalRevenue);
        //        }
        //        if (sortColumn == "Price")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.AvgSellingPrice);
        //            else
        //                query = query.OrderByDescending(x => x.AvgSellingPrice);
        //        }
        //        if (sortColumn == "Spread")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.Spread);
        //            else
        //                query = query.OrderByDescending(x => x.Spread);
        //        }
        //        if (sortColumn == "Profit")
        //        {
        //            if (sortDirection == "asc")
        //                query = query.OrderBy(x => x.Profit);
        //            else
        //                query = query.OrderByDescending(x => x.Profit);
        //        }
        //        return query.Skip(pageNum).Take(numRows).ToList();
        //    }
        //}

        public static void CopyQuotation(long toQuotationId, long fromQuotationId, Guid createdBy)
        {
            using (var context = GetContext())
            {
                Quotation q1 = context.Quotations.FirstOrDefault(x => x.Id == toQuotationId);
                Quotation q2 = context.Quotations.FirstOrDefault(x => x.Id == fromQuotationId);

                q1.PlantId = q2.PlantId;
                q1.QuoteDate = q2.QuoteDate;
                q1.AcceptanceExpirationDate = q2.AcceptanceExpirationDate;
                q1.QuoteExpirationDate = q2.QuoteExpirationDate;
                q1.PrivateNotes = q2.PrivateNotes;
                q1.PublicNotes = q2.PublicNotes;

                q1.PriceIncrease1 = q2.PriceIncrease1;
                q1.PriceIncrease2 = q2.PriceIncrease2;
                q1.PriceIncrease3 = q2.PriceIncrease3;

                q1.FskPriceId = q2.FskPriceId;
                q1.FskBasePrice = q2.FskBasePrice;

                q1.PriceIncreaseAmount1 = q2.PriceIncreaseAmount1;
                q1.PriceIncreaseAmount2 = q2.PriceIncreaseAmount2;
                q1.PriceIncreaseAmount3 = q2.PriceIncreaseAmount3;

                q1.Active = q2.Active;
                q1.CreatedOn = DateTime.Today;
                q1.UserId = createdBy;
                q1.SalesStaffId = q2.SalesStaffId;
                q1.AggregateEnabled = q2.AggregateEnabled;
                q1.ConcreteEnabled = q2.ConcreteEnabled;
                q1.BlockEnabled = q2.BlockEnabled;
                q1.AggregatePlantId = q2.AggregatePlantId;
                q1.BlockPlantId = q2.BlockPlantId;
                q1.TermsAndConditions = q2.TermsAndConditions;
                q1.Disclaimers = q2.Disclaimers;
                q1.Disclosures = q2.Disclosures;
                q1.AdjustMixPrice = q2.AdjustMixPrice;
                q1.BiddingDate = q2.BiddingDate;
                q1.IncludeAsLettingDate = q2.IncludeAsLettingDate;
                q1.CustomerNumberOnPDF = q2.CustomerNumberOnPDF;

                if (q2.PricingMonth != null)
                {
                    var copyPricingMonth = q2.PricingMonth.GetValueOrDefault();
                    if (q2.PricingMonth > DateTime.Now)
                        q1.PricingMonth = q2.PricingMonth;
                    else
                    {
                        q1.PricingMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    }
                }

                context.SubmitChanges();

                foreach (QuotationAddon addon in q2.QuotationAddons)
                {
                    QuotationAddon newAddon = new QuotationAddon();
                    newAddon.AddonId = addon.AddonId;
                    newAddon.Description = addon.Description;
                    newAddon.Price = addon.Price;
                    newAddon.IsIncludeTable = addon.IsIncludeTable;
                    newAddon.QuotationId = q1.Id;

                    context.QuotationAddons.InsertOnSubmit(newAddon);
                    context.SubmitChanges();
                }

                foreach (QuotationAggregate quoteAgg in q2.QuotationAggregates)
                {
                    QuotationAggregate newQuoteAgg = new QuotationAggregate();
                    newQuoteAgg.QuotationId = q1.Id;
                    newQuoteAgg.AggregateProductId = quoteAgg.AggregateProductId;
                    newQuoteAgg.QuotedDescription = quoteAgg.QuotedDescription;
                    newQuoteAgg.Volume = quoteAgg.Volume;
                    newQuoteAgg.Price = quoteAgg.Price;
                    newQuoteAgg.PublicNotes = quoteAgg.PublicNotes;
                    newQuoteAgg.Freight = quoteAgg.Freight;
                    newQuoteAgg.TotalPrice = quoteAgg.TotalPrice;
                    newQuoteAgg.TotalRevenue = quoteAgg.TotalRevenue;

                    context.QuotationAggregates.InsertOnSubmit(newQuoteAgg);
                    context.SubmitChanges();
                }

                foreach (QuotationBlock quoteBlock in q2.QuotationBlocks)
                {
                    QuotationBlock newQuoteBlock = new QuotationBlock();
                    newQuoteBlock.QuotationId = q1.Id;
                    newQuoteBlock.BlockProductId = quoteBlock.BlockProductId;
                    newQuoteBlock.QuotedDescription = quoteBlock.QuotedDescription;
                    newQuoteBlock.Volume = quoteBlock.Volume;
                    newQuoteBlock.Price = quoteBlock.Price;
                    newQuoteBlock.PublicNotes = quoteBlock.PublicNotes;
                    newQuoteBlock.Freight = quoteBlock.Freight;
                    newQuoteBlock.TotalPrice = quoteBlock.TotalPrice;
                    newQuoteBlock.TotalRevenue = quoteBlock.TotalRevenue;

                    context.QuotationBlocks.InsertOnSubmit(newQuoteBlock);
                    context.SubmitChanges();
                }

                foreach (QuotationMix mix in q2.QuotationMixes.Where(x => x.StandardMixId != null))
                {
                    QuotationMix newMix = new QuotationMix();
                    newMix.StandardMixId = mix.StandardMixId;
                    newMix.QuotationId = q1.Id;
                    newMix.QuotedDescription = mix.QuotedDescription;
                    newMix.PublicNotes = mix.PublicNotes;
                    newMix.PrivateNotes = mix.PrivateNotes;
                    newMix.Price = mix.Price;
                    newMix.AvgLoad = mix.AvgLoad;
                    newMix.Position = mix.Position;
                    newMix.Unload = mix.Unload;
                    newMix.Volume = mix.Volume;
                    newMix.MixCost = mix.MixCost;
                    context.QuotationMixes.InsertOnSubmit(newMix);
                    context.SubmitChanges();

                    foreach (MixLevelAddon mixLevelAddon in mix.MixLevelAddons)
                    {
                        MixLevelAddon newAddon = new MixLevelAddon();
                        newAddon.AddonId = mixLevelAddon.AddonId;
                        newAddon.Quantity = mixLevelAddon.Quantity;
                        newAddon.QuotationMixId = newMix.Id;
                        newAddon.Cost = mixLevelAddon.Cost;
                        context.MixLevelAddons.InsertOnSubmit(newAddon);
                        context.SubmitChanges();
                    }
                    UpdateQuotationStandardMixCalculations(newMix.Id);
                }

                foreach (QuotationMix mix in q2.QuotationMixes.Where(x => x.StandardMixId == null))
                {
                    QuotationMix newMix = new QuotationMix();
                    newMix.QuotationId = q1.Id;
                    newMix.CustomMixId = mix.CustomMixId;
                    newMix.QuotedDescription = mix.QuotedDescription;
                    newMix.PublicNotes = mix.PublicNotes;
                    newMix.PrivateNotes = mix.PrivateNotes;
                    newMix.Price = mix.Price;
                    newMix.AvgLoad = mix.AvgLoad;
                    newMix.Position = mix.Position;
                    newMix.Unload = mix.Unload;
                    newMix.Volume = mix.Volume;
                    context.QuotationMixes.InsertOnSubmit(newMix);
                    context.SubmitChanges();

                    foreach (CustomMixConstituent constituent in mix.CustomMixConstituents)
                    {
                        CustomMixConstituent newConstituent = new CustomMixConstituent();
                        newConstituent.Quantity = constituent.Quantity;
                        newConstituent.AddonId = constituent.AddonId;
                        newConstituent.RawMaterialId = constituent.RawMaterialId;
                        newConstituent.Description = constituent.Description;
                        newConstituent.IsCementitious = constituent.IsCementitious;
                        newConstituent.PerCementWeight = constituent.PerCementWeight;
                        newConstituent.Quantity = constituent.Quantity;
                        newConstituent.QuantityUomId = constituent.QuantityUomId;
                        newConstituent.Cost = constituent.Cost;
                        newConstituent.CostUomId = constituent.CostUomId;
                        newConstituent.QuotationMixId = newMix.Id;
                        context.CustomMixConstituents.InsertOnSubmit(newConstituent);
                        context.SubmitChanges();
                    }
                    UpdateQuotationCustomMixCalculations(newMix.Id);
                }

                if (q2.QuotationFormSettings != null)
                {
                    foreach (QuotationFormSetting qfs in q2.QuotationFormSettings)
                    {
                        QuotationFormSetting quoteFormSetting = new QuotationFormSetting();
                        quoteFormSetting.QuoteId = q1.Id;
                        quoteFormSetting.PriceInclude = qfs.PriceInclude;
                        quoteFormSetting.PriceSequence = qfs.PriceSequence;
                        quoteFormSetting.QuantityInclude = qfs.QuantityInclude;
                        quoteFormSetting.QuantitySequence = qfs.QuantitySequence;
                        quoteFormSetting.MixIdInclude = qfs.MixIdInclude;
                        quoteFormSetting.MixIdSequence = qfs.MixIdSequence;
                        quoteFormSetting.DescriptionInclude = qfs.DescriptionInclude;
                        quoteFormSetting.DescriptionSequence = qfs.DescriptionSequence;
                        quoteFormSetting.PsiInclude = qfs.PsiInclude;
                        quoteFormSetting.PsiSequence = qfs.PsiSequence;
                        quoteFormSetting.PublicCommentsInclude = qfs.PublicCommentsInclude;
                        quoteFormSetting.PublicCommentsSequence = qfs.PublicCommentsSequence;
                        quoteFormSetting.SlumpInclude = qfs.SlumpInclude;
                        quoteFormSetting.SlumpSequence = qfs.SlumpSequence;
                        quoteFormSetting.AirInclude = qfs.AirInclude;
                        quoteFormSetting.AirSequence = qfs.AirSequence;
                        quoteFormSetting.AshInclude = qfs.AshInclude;
                        quoteFormSetting.AshSequence = qfs.AshSequence;
                        quoteFormSetting.FineAggInclude = qfs.FineAggInclude;
                        quoteFormSetting.FineAggSequence = qfs.FineAggSequence;
                        quoteFormSetting.SacksInclude = qfs.SacksInclude;
                        quoteFormSetting.SacksSequence = qfs.SacksSequence;
                        quoteFormSetting.MD1Include = qfs.MD1Include;
                        quoteFormSetting.MD1Sequence = qfs.MD1Sequence;
                        quoteFormSetting.MD2Include = qfs.MD2Include;
                        quoteFormSetting.MD2Sequence = qfs.MD2Sequence;
                        quoteFormSetting.MD3Include = qfs.MD3Include;
                        quoteFormSetting.MD3Sequence = qfs.MD3Sequence;
                        quoteFormSetting.MD4Include = qfs.MD4Include;
                        quoteFormSetting.MD4Sequence = qfs.MD4Sequence;
                        context.QuotationFormSettings.InsertOnSubmit(quoteFormSetting);
                        context.SubmitChanges();
                    }
                }

                //if (q2.QuoteQcRequirements != null)
                //{
                //    foreach (var quoteQc in q2.QuoteQcRequirements)
                //    {
                //        QuoteQcRequirement quoteQcReq = new QuoteQcRequirement();
                //        quoteQcReq.QuoteId = q1.Id;
                //        quoteQcReq.QCRequirementId = quoteQc.QCRequirementId;
                //        context.QuoteQcRequirements.InsertOnSubmit(quoteQcReq);
                //        context.SubmitChanges();
                //    } 
                //}
            }
        }

        public static List<Quotation> GetQuotationsForProject(int projectId)
        {

            using (var context = GetContext())
            {
                context.LoadOptions = GetQuotationLoadOptions();
                return context.Quotations.Where(x => x.ProjectId == projectId).ToList();
            }
        }
        public static CustomerContact GetQuotationContact(long quotationId)
        {
            using (var context = GetContext())
            {
                CustomerContact cc = context.Quotations.Where(x => x.Id == quotationId).Select(x => x.CustomerContact).FirstOrDefault();
                return cc;
            }
        }

        private static DataLoadOptions GetQuotationLoadOptions()
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Quotation>(x => x.Plant);
            opts.LoadWith<Quotation>(x => x.AggregatePlant);
            opts.LoadWith<Quotation>(x => x.BlockPlant);
            opts.LoadWith<Quotation>(x => x.Customer);
            opts.LoadWith<Quotation>(x => x.CustomerContact);
            opts.LoadWith<Quotation>(x => x.Project);
            opts.LoadWith<Quotation>(x => x.SalesStaff);
            opts.LoadWith<Project>(x => x.ProjectSalesStaffs);
            opts.LoadWith<Project>(x => x.Customer);
            return opts;
        }

        public static Quotation FindQuotation(long id)
        {
            using (var context = GetContext())
            {
                context.LoadOptions = GetQuotationLoadOptions();
                return context.Quotations.FirstOrDefault(x => x.Id == id);
            }
        }

        public static Quotation FindQuotationWithAllRefs(long id)
        {
            using (var context = GetContext())
            {
                DataLoadOptions opts = GetQuotationLoadOptions();
                opts.LoadWith<Quotation>(x => x.QuotationMixes);
                opts.LoadWith<Quotation>(x => x.QuotationAggregates);
                opts.LoadWith<Quotation>(x => x.QuotationBlocks);
                opts.LoadWith<QuotationAggregate>(x => x.AggregateProduct);
                opts.LoadWith<QuotationBlock>(x => x.BlockProduct);
                opts.LoadWith<QuotationMix>(x => x.StandardMix);
                opts.LoadWith<Quotation>(x => x.TaxCode);

                context.LoadOptions = opts;

                return context.Quotations.FirstOrDefault(x => x.Id == id);
            }
        }

        public static List<Quotation> GetQuotations(long projectId)
        {
            using (var context = GetContext())
            {
                context.LoadOptions = GetQuotationLoadOptions();
                var query = context.Quotations.Where(x => x.ProjectId == projectId);
                return query.ToList();
            }
        }

        public static void UpdateQuotation(Quotation entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    SIRoleAccess access = SIDAL.GetAccessRules(SIDAL.GetUser(entity.UserId).Role);
                    if (access.Enable5skPricing)
                    {
                        FSKPrice p = null;
                        if (entity.PlantId != null)
                        {
                            p = context.Plants.Where(x => x.PlantId == entity.PlantId).FirstOrDefault().FSKPrice;
                        }
                        if (p == null)
                        {
                            p = context.FSKPrices.FirstOrDefault();
                        }
                        if (p != null)
                        {
                            entity.FskPriceId = p.Id;
                            entity.FskBasePrice = p.BasePrice;
                        }
                    }
                    context.Quotations.InsertOnSubmit(entity);
                }
                else
                {
                    context.Quotations.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
                if (entity.ProjectId != null)
                {
                    UpdateProjectPlantVolume(entity.ProjectId.Value, entity.Id);
                    UpdateProjectSackPrice(entity.ProjectId.Value);
                }
            }
        }

        public static bool DeleteQuotation(long id)
        {
            int projectId = 0;
            using (var context = GetContext())
            {
                Quotation c = context.Quotations.FirstOrDefault(x => x.Id == id);
                projectId = c.ProjectId.GetValueOrDefault();
                if (c != null)
                {
                    context.QuotationMixes.DeleteAllOnSubmit(c.QuotationMixes);
                    context.QuoteAuditLogs.DeleteAllOnSubmit(c.QuoteAuditLogs);
                    context.QuotationAddons.DeleteAllOnSubmit(c.QuotationAddons);
                    context.QuotationAggregates.DeleteAllOnSubmit(c.QuotationAggregates);
                    context.QuotationAggregateAddons.DeleteAllOnSubmit(c.QuotationAggregateAddons);
                    context.QuotationBlocks.DeleteAllOnSubmit(c.QuotationBlocks);
                    context.QuotationBlockAddons.DeleteAllOnSubmit(c.QuotationBlockAddons);
                    context.Notifications.DeleteAllOnSubmit(c.Notifications);
                    context.QuotationFormSettings.DeleteAllOnSubmit(c.QuotationFormSettings);
                    context.Quotations.DeleteOnSubmit(c);
                    context.SubmitChanges();
                }
                UpdateProjectPlantVolume(c.ProjectId.Value);
            }
            if (projectId != 0)
                UpdateProjectSackPrice(projectId);
            return true;
        }

        public static void UpdateQuotationCustomer(long quotationId, int customerId)
        {
            using (var context = GetContext())
            {
                Quotation c = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
                c.CustomerId = customerId;
                context.SubmitChanges();
            }
        }

        public static void UpdateQuotationLastPushedDate(long quotationId, string PushedBy)
        {
            using (var context = GetContext())
            {
                Quotation c = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
                c.LastPushedAt = DateTime.Now;
                c.PushedBy = PushedBy;
                context.SubmitChanges();
            }
        }
        public static void UpdateQuotationCustomerContact(long quotationId, int customerContactId)
        {
            using (var context = GetContext())
            {
                Quotation c = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
                c.CustomerContactId = customerContactId;
                context.SubmitChanges();
            }
        }

        public static void UpdateQuotationProject(long quotationId, int projectId)
        {
            using (var context = GetContext())
            {
                Quotation q = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
                q.ProjectId = projectId;
                context.SubmitChanges();
            }
        }

        public static long UpdateQuotationBasicInfo(long quotationId, Guid userId, int plantId, DateTime pricingMonth, int customerId, int customerContactId, long taxCodeId, string taxExemptReason, int salesStaffId,bool customerNumberOnPDF)
        {
            using (var context = GetContext())
            {
                Quotation q = null;
                if (quotationId > 0)
                {
                    q = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
                }
                else
                {
                    q = new Quotation();
                    q.UserId = userId;
                    q.CreatedOn = DateTime.Today;
                    context.Quotations.InsertOnSubmit(q);
                }
                if (salesStaffId > 0)
                    q.SalesStaffId = salesStaffId;
                if (plantId > 0)
                    q.PlantId = plantId;
                if (customerContactId > 0)
                    q.CustomerContactId = customerContactId;
                if (customerId > 0)
                    q.CustomerId = customerId;
                if (taxCodeId > 0)
                    q.TaxCodeId = taxCodeId;
                //if (backupPlantId > 0)
                //    q.BackupPlantId = backupPlantId;
                
                // Update Fsk Price while Quote Updating
                FSKPrice p = null;
                if (q.PlantId != null)
                {
                    p = context.Plants.Where(x => x.PlantId == q.PlantId).FirstOrDefault().FSKPrice;
                }
                if (p == null)
                {
                    p = context.FSKPrices.FirstOrDefault();
                }
                // The below code should not be running when Quotation is Updated
                //if (p != null)
                //{
                //    q.FskPriceId = p.Id;
                //    q.FskBasePrice = p.BasePrice;
                //}

                q.TaxExemptReason = taxExemptReason;
                q.PricingMonth = pricingMonth;
                q.CustomerNumberOnPDF = customerNumberOnPDF;
                context.SubmitChanges();
                return q.Id;
            }
        }

        public static void UpdateQuotationDetails(long quotationId, bool Active,
                        DateTime? quoteDate, DateTime? acceptance, DateTime? quoteExpiration,
                        DateTime? dateChange1, decimal change1,
                        DateTime? dateChange2, decimal change2,
                        DateTime? dateChange3, decimal change3,
                        string privateNotes, string publicNotes,
                        string disclaimer, string disclosure,
                        string termsAndCondition, DateTime? biddingDate,
                        bool includeAsLettingDate, int projectId)
        {
            using (var context = GetContext())
            {
                Quotation quotation = context.Quotations.First(x => x.Id == quotationId);
                if (quotation != null)
                {
                    quotation.Active = Active;
                    District d = SIDAL.GetDistrict(SIDAL.GetPlant(quotation.PlantId).DistrictId);
                    if (quoteDate == null)
                    {
                        quoteDate = quotation.CreatedOn;
                    }
                    if (acceptance == null)
                    {
                        acceptance = quoteDate.Value.AddDays(d.AcceptanceExpiration.GetValueOrDefault(30));
                    }
                    if (quoteExpiration == null)
                    {
                        quoteExpiration = quoteDate.Value.AddDays(d.QuoteExpiration.GetValueOrDefault(60));
                    }

                    quotation.QuoteDate = quoteDate;
                    quotation.AcceptanceExpirationDate = acceptance;
                    quotation.QuoteExpirationDate = quoteExpiration;

                    quotation.PriceIncrease1 = dateChange1;
                    quotation.PriceIncreaseAmount1 = change1;

                    quotation.PriceIncrease2 = dateChange2;
                    quotation.PriceIncreaseAmount2 = change2;

                    quotation.PriceIncrease3 = dateChange3;
                    quotation.PriceIncreaseAmount3 = change3;

                    quotation.PrivateNotes = privateNotes;
                    quotation.PublicNotes = publicNotes;

                    quotation.Disclaimers = disclaimer;
                    quotation.Disclosures = disclosure;
                    quotation.TermsAndConditions = termsAndCondition;
                    quotation.BiddingDate = biddingDate;
                    quotation.IncludeAsLettingDate = includeAsLettingDate;

                    context.SubmitChanges();
                }

                if (quotation.BiddingDate != null)
                {
                    updateProjectBidDate(quotation.BiddingDate.GetValueOrDefault(), projectId);
                }
            }
        }

        public static void updateProjectBidDate(DateTime bidDate, int projectId)
        {
            using (var db = GetContext())
            {
                var project = db.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                project.BidDate = bidDate;
                db.SubmitChanges();

                var projectQuotationList = db.Quotations.Where(x => x.ProjectId == projectId && x.Status == null).ToList();
                foreach (var quote in projectQuotationList)
                {
                    quote.BiddingDate = bidDate;
                    db.SubmitChanges();
                }
            }
        }

        public static List<Addon> GetAddonsForQuote(long quotationId)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<QuotationAddon>(x => x.Addon);
            opts.LoadWith<Addon>(x => x.QuoteUom);
            opts.LoadWith<Addon>(x => x.MixUom);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.QuotationAddons.Where(x => x.QuotationId == quotationId).Select(x => x.Addon).ToList();
            }
        }

        public static void UpdateQuotationAddons(long quotationId, long[] addonIds)
        {
            using (var context = GetContext())
            {
                Quotation q = context.Quotations.Where(x => x.Id == quotationId).FirstOrDefault();
                var quotations = context.QuotationAddons.Where(x => x.QuotationId == quotationId);
                context.QuotationAddons.DeleteAllOnSubmit(quotations);
                context.SubmitChanges();
                if (addonIds == null)
                    return;

                foreach (long id in addonIds)
                {
                    QuotationAddon a = new QuotationAddon();
                    a.QuotationId = quotationId;
                    a.AddonId = id;
                    context.QuotationAddons.InsertOnSubmit(a);
                }
                context.SubmitChanges();
                UpdateProjectPriceSpreadProfit(q.ProjectId.GetValueOrDefault());
            }
        }

        public static void AddQuotationAddOn(QuotationAddon qaon)
        {
            using (var context = GetContext())
            {
                if (qaon.Id == 0)
                    context.QuotationAddons.InsertOnSubmit(qaon);
                else
                {
                    context.QuotationAddons.Attach(qaon);
                    context.Refresh(RefreshMode.KeepCurrentValues, qaon);
                }
                context.SubmitChanges();
            }
        }
        public static void DeleteQuotationAddon(long quoteAddonId)
        {
            using (var context = GetContext())
            {
                QuotationAddon qaon = context.QuotationAddons.FirstOrDefault(x => x.Id == quoteAddonId);

                if (qaon != null)
                {
                    context.QuotationAddons.DeleteOnSubmit(qaon);
                    context.SubmitChanges();
                }
            }
        }
        public static string GetAddonsUomName(long addonId)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Addon>(x => x.QuoteUom);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.Addons.Where(x => x.Id == addonId).Select(x => x.QuoteUom.Name).FirstOrDefault();
            }
        }
        public static QuotationAddon GetQuotationAddon(long quoteId, long addonId)
        {
            QuotationAddon qaon = new QuotationAddon();
            using (var context = GetContext())
            {
                qaon = context.QuotationAddons.Where(x => x.QuotationId == quoteId && x.AddonId == addonId).FirstOrDefault();
                if (qaon != null)
                    return qaon;
                else
                    return null;
            }
        }
        public static void UpdateQuotationAddonFields(QuotationAddon qaon)
        {
            QuotationAddon quoteAon = new QuotationAddon();
            using (var context = GetContext())
            {
                quoteAon = context.QuotationAddons.Where(x => x.Id == qaon.Id).FirstOrDefault();
                quoteAon.Description = qaon.Description;
                quoteAon.Price = qaon.Price;
                quoteAon.IsIncludeTable = qaon.IsIncludeTable;
                context.SubmitChanges();
            }
        }
        public static QuotationAddon GetQuoatationAddonIdWise(long Id)
        {

            using (var context = GetContext())
            {
                return context.QuotationAddons.Where(x => x.Id == Id).FirstOrDefault();
            }
        }
        public static List<QuotationAddon> GetQuoatationAddonQuotationIdWise(long quoteId, bool isIncludeTable = false)
        {
            using (var context = GetContext())
            {
                var qaon = context.QuotationAddons.Where(x => x.QuotationId == quoteId).AsQueryable();
                if (qaon != null)
                {
                    if (isIncludeTable)
                        qaon = qaon.Where(x => x.IsIncludeTable == true);
                }
                return qaon.ToList();
            }
        }
        public static decimal FindCurrentAddonQuoteCost(long addonId, int districtId, DateTime? costEffectiveDate = null)
        {
            using (var context = GetContext())
            {
                DateTime currentCostEffectiveDate = DateTime.Today;
                if (costEffectiveDate != null)
                {
                    currentCostEffectiveDate = costEffectiveDate.GetValueOrDefault();
                }

                currentCostEffectiveDate = currentCostEffectiveDate.AddDays(-1 * currentCostEffectiveDate.Day);
                currentCostEffectiveDate = currentCostEffectiveDate.AddMonths(1);
                List<AddonPriceProjection> apps = context.AddonPriceProjections.Where(x => x.AddonId == addonId).
                                            Where(x => x.PriceMode == "QUOTE").
                                            Where(x => x.DistrictId == districtId).
                                            Where(x => x.ChangeDate <= currentCostEffectiveDate).
                                            OrderByDescending(x => x.ChangeDate).ToList();
                AddonPriceProjection app = apps.FirstOrDefault();

                if (app == null)
                {
                    return 0;
                }
                else
                {
                    return app.Price;
                }
            }
        }

        public static void UpdateQuotationAddonTable()
        {
            using (var context = GetContext())
            {
                QuotationAddon qaon;
                var quoteAddons = context.QuotationAddons.Where(x => x.Description == null && x.Price == null);
                foreach (var quoteAddon in quoteAddons)
                {
                    qaon = new QuotationAddon();
                    qaon.Id = quoteAddon.Id;
                    qaon.AddonId = quoteAddon.AddonId;
                    qaon.QuotationId = quoteAddon.QuotationId;
                    //Addon Description
                    Addon aon = new Addon();
                    aon = context.Addons.First(x => x.Id == quoteAddon.AddonId);
                    qaon.Description = aon.Description;
                    // Addon Price
                    Quotation qot = new Quotation();
                    qot = context.Quotations.First(x => x.Id == quoteAddon.QuotationId);
                    qaon.Price = SIDAL.FindCurrentAddonQuoteCost(quoteAddon.AddonId, SIDAL.GetPlant(qot.PlantId).DistrictId, qot.PricingMonth != null ? qot.PricingMonth : DateTime.Today);

                    qaon.IsIncludeTable = quoteAddon.IsIncludeTable;
                    AddQuotationAddOn(qaon);
                }
            }
        }
        public static decimal? FindAddonMixCost(long addonId, int districtId, DateTime? asOfDate = null)
        {
            using (var context = GetContext())
            {
                if (asOfDate == null)
                    asOfDate = DateUtils.GetFirstOf(DateTime.Today);
                else
                    asOfDate = DateUtils.GetFirstOf(asOfDate.Value);

                asOfDate = asOfDate.Value.AddMonths(1);
                AddonPriceProjection app = context.AddonPriceProjections.Where(x => x.AddonId == addonId).
                                            Where(x => x.PriceMode == "MIX").
                                            Where(x => x.DistrictId == districtId).
                                            Where(x => x.ChangeDate <= asOfDate).
                                            OrderByDescending(x => x.ChangeDate).
                                            FirstOrDefault();
                if (app == null)
                {
                    return 0;
                }
                else
                {
                    return app.Price;
                }
            }
        }
        public static AddonPriceProjection FindAddonMixPriceProjection(long addonId, int districtId, DateTime? asOfDate = null)
        {
            using (var context = GetContext())
            {
                if (asOfDate == null)
                    asOfDate = DateUtils.GetFirstOf(DateTime.Today);
                else
                    asOfDate = DateUtils.GetFirstOf(asOfDate.Value);

                asOfDate = asOfDate.Value.AddMonths(1);
                AddonPriceProjection app = context.AddonPriceProjections.Where(x => x.AddonId == addonId).
                                            Where(x => x.PriceMode == "MIX").
                                            Where(x => x.DistrictId == districtId).
                                            Where(x => x.ChangeDate <= asOfDate).
                                            OrderByDescending(x => x.ChangeDate).
                                            FirstOrDefault();
                return app;
            }
        }

        public static RawMaterialCostProjection FindRawMaterialCost(long rawMaterialId, long plantId, DateTime? pricingMonth = null)
        {
            using (var context = GetContext())
            {
                if (pricingMonth == null)
                    pricingMonth = DateUtils.GetFirstOf(DateTime.Today);
                else
                    pricingMonth = DateUtils.GetFirstOf(pricingMonth.Value);

                pricingMonth = pricingMonth.Value.AddMonths(1);
                RawMaterialCostProjection projection = context.RawMaterialCostProjections
                                                                        .Where(x => x.RawMaterialId == rawMaterialId)
                                                                        .Where(x => x.PlantId == plantId)
                                                                        .Where(x => x.ChangeDate < pricingMonth)
                                                                        .OrderByDescending(x => x.ChangeDate)
                                                                        .FirstOrDefault();
                return projection;
            }
        }

        public static List<QuotationMix> GetQuotationMixes(long QuotationId)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<QuotationMix>(x => x.StandardMix);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.QuotationMixes.Where(x => x.QuotationId == QuotationId).ToList();
            }
        }

        public static void UpdateQuoteMixPosition(long id, int position)
        {
            using (var context = GetContext())
            {
                var q = context.QuotationMixes.FirstOrDefault(x => x.Id == id);
                q.Position = position;
                context.SubmitChanges();
            }
        }
        public static double GetQuotationMixVolume(long id)
        {
            using (var context = GetContext())
            {
                var mix = context.QuotationMixes.FirstOrDefault(x => x.Id == id);
                return mix.Volume.GetValueOrDefault();
            }
        }

        public static void UpdateQuotationCalculations(long quotationId)
        {
            int? projectId = 0;
            using (var context = GetContext())
            {
                Quotation q = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
                projectId = q.ProjectId;
                q.TotalVolume = q.QuotationMixes.Sum(x => x.Volume.GetValueOrDefault(0));
                q.TotalRevenue = q.QuotationMixes.Sum(x => x.Price.GetValueOrDefault(0) * Convert.ToDecimal(x.Volume.GetValueOrDefault(0)));
                q.TotalProfit = q.QuotationMixes.Sum(x => x.Profit.GetValueOrDefault(0) * Convert.ToDecimal(x.Volume.GetValueOrDefault(0)));
                if (q.TotalVolume != 0)
                {
                    q.AvgLoad = q.QuotationMixes.Sum(x => x.AvgLoad.GetValueOrDefault(0) * x.Volume.GetValueOrDefault(0)) / q.TotalVolume;
                    q.AvgUnload = q.QuotationMixes.Sum(x => x.Unload.GetValueOrDefault(0) * x.Volume.GetValueOrDefault(0)) / q.TotalVolume;
                    q.AvgSellingPrice = q.TotalRevenue / Convert.ToDecimal(q.TotalVolume);
                    var spread = q.QuotationMixes.Sum(x => x.Spread.GetValueOrDefault(0) * Convert.ToDecimal(x.Volume.GetValueOrDefault(0))) / Convert.ToDecimal(q.TotalVolume);
                    q.Spread = Convert.ToDecimal(spread.ToString("#.##"));
                    q.Contribution = q.QuotationMixes.Sum(x => x.Contribution.GetValueOrDefault(0) * Convert.ToDecimal(x.Volume.GetValueOrDefault(0))) / Convert.ToDecimal(q.TotalVolume);
                    q.Profit = q.QuotationMixes.Sum(x => x.Profit.GetValueOrDefault(0) * Convert.ToDecimal(x.Volume.GetValueOrDefault(0))) / Convert.ToDecimal(q.TotalVolume);
                    q.CYDHr = q.QuotationMixes.Sum(x => x.CydHour.GetValueOrDefault(0) * x.Volume.GetValueOrDefault(0)) / q.TotalVolume;
                }
                else
                {
                    q.AvgLoad = null;
                    q.AvgUnload = null;
                    q.AvgSellingPrice = null;
                    q.Spread = null;
                    q.Contribution = null;
                    q.Profit = null;
                    q.CYDHr = null;
                }
                context.SubmitChanges();
            }
            UpdateProjectPlantVolume(Convert.ToInt32(projectId), quotationId);
        }



        public static void CleanUpQuotesForReport()
        {
            using (var context = GetContext())
            {
                context.LoadOptions = GetQuotationLoadOptions();

                var projects = context.Projects.Where(x => x.Quotations.Count() > 0);

                if (projects != null)
                {
                    foreach (var project in projects)
                    {
                        if (project == null)
                            continue;

                        //Get Awarded quotes for this project
                        var awardedQuotes = project.Quotations.Where(x => x.Awarded == true);

                        if (awardedQuotes.Count() == 0)
                        {
                            //Set all quotes to null
                            foreach (var quote in project.Quotations)
                            {
                                quote.Awarded = null;
                            }
                        }
                        else
                        {
                            List<long> awardedQuoteIds = awardedQuotes.Select(x => x.Id).ToList();
                            foreach (var quote in project.Quotations)
                            {
                                if (awardedQuoteIds != null && !awardedQuoteIds.Contains(quote.Id))
                                {
                                    quote.Awarded = false;
                                }
                            }
                        }
                    }
                    context.SubmitChanges();
                }
            }
        }

        public static void AwardQuote(long quoteId)
        {
            using (var context = GetContext())
            {
                var q = context.Quotations.FirstOrDefault(x => x.Id == quoteId);
                if (q.Project != null)
                {
                    q.Project.Price = q.AvgSellingPrice;
                    q.Project.Spread = q.Spread;
                    q.Project.Profit = q.Profit;
                    q.Project.Volume = Convert.ToInt32(q.TotalVolume.GetValueOrDefault(0));
                    q.Project.CustomerId = q.CustomerId;
                }
                q.Awarded = true;
                context.SubmitChanges();
                UpdateProjectPriceSpreadProfit(q.ProjectId);
                if (q.ProjectId != null)
                    UpdateProjectSackPrice(q.ProjectId.GetValueOrDefault());
            }
        }
        public static void AwardQuotes(int projectId, List<long> quoteIds)
        {
            if (quoteIds.Count() > 0)
            {
                DataLoadOptions dlo = new DataLoadOptions();
                dlo.LoadWith<Project>(x => x.Quotations);
                using (var context = GetContext())
                {
                    context.LoadOptions = dlo;
                    var proj = context.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                    var projQuotations = proj.Quotations.ToList();
                    foreach (var item in projQuotations)
                    {
                        var q = new Quotation();
                        q = context.Quotations.FirstOrDefault(x => x.Id == item.Id);

                        if (quoteIds.Contains(item.Id))
                            q.Awarded = true;
                        else
                            q.Awarded = null;
                        context.SubmitChanges();
                    }
                }
                UpdateProjectPlantVolume(Convert.ToInt32(projectId));
            }
            UpdateProjectSackPrice(projectId);

        }
        public static void UnAwardQuote(long quoteId)
        {
            using (var context = GetContext())
            {
                var q = context.Quotations.FirstOrDefault(x => x.Id == quoteId);
                q.Awarded = null;

                context.SubmitChanges();
                UpdateProjectPlantVolume(Convert.ToInt32(q.ProjectId));
                if (q.ProjectId != null)
                    UpdateProjectSackPrice(q.ProjectId.GetValueOrDefault());
            }
        }

        public static QuoteAuditLog AddQuoteAuditLog(long quoteId, string message, string username, bool? exemptionApplies = false, string key = null)
        {
            using (var context = GetContext())
            {
                var entity = new QuoteAuditLog();
                var quotation = context.Quotations.FirstOrDefault(x => x.Id == quoteId);

                if (quotation != null)
                {
                    //Exemption condition
                    var quoteUser = context.aspnet_Users.FirstOrDefault(x => x.UserId == quotation.UserId);
                    bool createLog = true;
                    if (exemptionApplies.Value)
                    {
                        createLog = (quoteUser.UserName != username || quotation.CreatedOn.GetValueOrDefault().Date != DateTime.Today);

                        if (createLog)
                        {
                            var qaLogDate = context.QuoteAuditLogs.Where(x => x.QuotationId == quoteId && x.Text.ToString().ToLower() == message.ToLower()).GroupBy(x => x.Text.ToString()).Select(x => x.Max(log => log.CreatedOn)).FirstOrDefault();
                            if (qaLogDate != null)
                            {
                                if (!(qaLogDate <= DateTime.Now.AddMinutes(-10)))
                                    createLog = false;
                            }
                        }
                    }
                    if (createLog)
                    {
                        entity.QuotationId = quoteId;
                        entity.Text = message;
                        entity.Username = username;
                        entity.CreatedOn = DateTime.Now;
                        entity.ReferenceLink = key;
                        context.QuoteAuditLogs.InsertOnSubmit(entity);
                        context.SubmitChanges();
                    }
                }
                return entity;
            }
        }

        public static List<QuoteAuditLog> GetQuoteAuditLogs(long quotationId, int? skip = null)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<QuoteAuditLog>(x => x.Quotation);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                var query = context.QuoteAuditLogs.Where(x => x.QuotationId == quotationId).OrderByDescending(x => x.CreatedOn);

                //if (skip > 0)
                //    query = query.Skip(skip.GetValueOrDefault());

                return query.ToList();
            }
        }

        // Update Project Entry Form PDF Generated Date on Quotation
        public static void ProjectEntryFormPDFGeneratedDate(long quotationId)
        {
            using (var context = GetContext())
            {
                Quotation q = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
                q.PDFGeneratedDate = DateTime.Now;
                context.SubmitChanges();
            }
        }

        // Check Quote exist against project's customer
        public static bool CheckQuoteExistAgainstProjectCustomer(long projectId, long customerId)
        {
            using (var context = GetContext())
            {
                return context.Quotations.Where(x => x.ProjectId == projectId && x.CustomerId == customerId).Count() > 0;
            }
        }

        public static bool UpdateQuotationAdjustMixPrice(double? adjustMixPrice, long quoteId)
        {
            using (var context = GetContext())
            {
                try
                {
                    Quotation q = context.Quotations.FirstOrDefault(x => x.Id == quoteId);
                    q.AdjustMixPrice = adjustMixPrice;
                    context.SubmitChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

        }
        #endregion

        #region QuotationStandardMix
        public static List<StandardMix> SearchStandardMix(int plantId, int? PSI, string Slump, string Air, string MD1, string MD2, string MD3, string MD4)
        {
            using (var context = GetContext())
            {
                var query = context.StandardMixes.AsQueryable();
                //Below line changed by Shail (API returns PSI with 0 in it)
                if (PSI != null)
                    query = query.Where(x => x.PSI == PSI);
                if (Slump != null)
                    query = query.Where(x => x.Slump == Slump);
                if (Air != null)
                    query = query.Where(x => x.Air == Air);
                if (MD1 != null)
                    query = query.Where(x => x.MD1 == MD1);
                if (MD2 != null)
                    query = query.Where(x => x.MD2 == MD2);
                if (MD3 != null)
                    query = query.Where(x => x.MD3 == MD3);
                if (MD4 != null)
                    query = query.Where(x => x.MD4 == MD4);

                query = query.Join(context.MixFormulations,
                                    mix => mix.Id,
                                    formulation => formulation.StandardMixId,
                                    (mix, formulation) => new { Mix = mix, Formulation = formulation }).Where(x => x.Formulation.PlantId == plantId).Select(x => x.Mix);

                query = query.Where(x => x.Active == true).OrderBy(x => x.Number).ThenBy(x => x.Description);

                return query.ToList();
            }
        }

        public static List<StandardMix> GetValidStandardMixes(int plantId, DateTime asOfDate)
        {
            using (var context = GetContext())
            {
                //var query = context.MixFormulationCostProjections.AsQueryable();

                //query = query.Where(x => x.MixFormulation.PlantId == plantId)
                //             .Where(x => x.Cost > 0)
                //             .Where(x => x.AsOfDate <= asOfDate);

                //var query2 = query.Select(x => x.MixFormulation.StandardMix)
                //            .Where(x => x.Active == true)
                //            .Distinct()
                //            .OrderBy(x => x.Number)
                //            .ThenBy(x => x.Description);

                //return query2.ToList();

                var query = context.MixFormulationCostProjections.AsQueryable();

                DateTime actualAsOfDate = DateUtils.GetFirstOf(asOfDate);

                var validationQuery = query.Where(x => x.MixFormulation.PlantId == plantId).Where(x => x.AsOfDate == actualAsOfDate).Where(x => x.Cost > 0);
                if (validationQuery.Count() == 0)
                {
                    DateTime? maxAsOfDate = query.Where(x => x.MixFormulation.PlantId == plantId)
                                           .Where(x => x.Cost > 0)
                                           .Max(x => (DateTime?)x.AsOfDate);
                    asOfDate = DateUtils.GetFirstOf(maxAsOfDate ?? DateTime.Today);

                    query = query.Where(x => x.MixFormulation.PlantId == plantId).Where(x => x.AsOfDate == actualAsOfDate).Where(x => x.Cost > 0);
                }
                else
                {
                    query = validationQuery;
                }

                var queryFinal = query.Select(x => x.MixFormulation.StandardMix);

                //// Below line changed by Shail (API returns PSI with 0 in it)
                //var query2 = query.Where(x => x.MixFormulation.PlantId == plantId).Where(x => x.AsOfDate == asOfDate).Where(x => x.Cost > 0).Select(x => x.MixFormulation.StandardMix);

                queryFinal = queryFinal.Where(x => x.Active == true).OrderBy(x => x.Number).ThenBy(x => x.Description);

                return queryFinal.ToList();
            }
        }

        public static bool CheckDuplicateMixNumber(long entityId, string number)
        {
            using (var context = GetContext())
            {
                var existing = context.StandardMixes.Where(x => x.Number == number).FirstOrDefault();
                if (existing == null)
                {
                    return false;
                }
                else
                {
                    if (existing.Id == entityId)
                        return false;
                    else
                        return true;
                }
            }
        }

        public static decimal CalculateQuotationStandardMixCost(long quoteMixId)
        {
            long standardMixID = 0;
            int plantId = 0;
            DateTime? pricingMonth = null;
            using (var context = GetContext())
            {
                QuotationMix qm = context.QuotationMixes.FirstOrDefault(x => x.Id == quoteMixId);
                if (qm == null)
                    return 0;

                Quotation q = qm.Quotation;
                StandardMix mix = qm.StandardMix;
                standardMixID = mix.Id;
                Plant plant = q.Plant;
                if (plant == null)
                    return 0;
                pricingMonth = q.PricingMonth;
                plantId = plant.PlantId;
            }
            return CalculateCurrentMixCost(standardMixID, plantId, pricingMonth);
        }

        public static void UpdateQuotationMix(QuotationMix entity)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.QuotationMixes.InsertOnSubmit(entity);
                    entity.Position = context.QuotationMixes.Where(x => x.QuotationId == entity.QuotationId).Count();
                }
                else
                {
                    context.QuotationMixes.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
        }

        public static void UpdateQuotationMix(long mixId, long? standardMixId, double volume, decimal price, int unload, double averageLoad)
        {
            int projectId = 0;
            using (var context = GetContext())
            {
                QuotationMix qm = context.QuotationMixes.FirstOrDefault(x => x.Id == mixId);
                if (standardMixId != null)
                {
                    qm.StandardMixId = standardMixId;
                    context.SubmitChanges();
                    qm.MixCost = CalculateQuotationStandardMixCost(qm.Id);
                }
                projectId = qm.Quotation.ProjectId.GetValueOrDefault();
                qm.Volume = volume;
                qm.Price = price;
                qm.Unload = unload;
                qm.AvgLoad = averageLoad;
                context.SubmitChanges();
            }
            if (projectId != 0)
            {
                UpdateProjectSackPrice(projectId);
            }
        }

        public static string ResetDefaultQuoteMixDescription(long quoteMixId)
        {
            using (var context = GetContext())
            {
                QuotationMix qm = context.QuotationMixes.FirstOrDefault(x => x.Id == quoteMixId);
                string quote = "";
                if (qm.StandardMix != null)
                {
                    quote = qm.StandardMix.SalesDesc;
                }

                quote = quote + ", " + String.Join(", ", qm.MixLevelAddons.Select(x => x.Addon.Code).ToArray());
                qm.QuotedDescription = quote;
                context.SubmitChanges();
                return quote;
            }
        }

        public static string GenerateDefaultQuoteMixDescription(long quoteMixId, long standarMixId)
        {
            using (var context = GetContext())
            {
                string quote = "";
                if (standarMixId != 0)
                {
                    StandardMix mix = context.StandardMixes.FirstOrDefault(x => x.Id == standarMixId);
                    quote = mix.SalesDesc;
                }
                if (quoteMixId != 0)
                {
                    QuotationMix qm = context.QuotationMixes.FirstOrDefault(x => x.Id == quoteMixId);
                    if (standarMixId == 0)
                    {
                        quote = qm.StandardMix.SalesDesc;
                    }
                    quote = quote + ", " + String.Join(", ", qm.MixLevelAddons.Select(x => x.Addon.Code).ToArray());
                    return quote;
                }
                return quote;
            }
        }

        public static QuotationMix FindQuotationMix(long id)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<QuotationMix>(x => x.Quotation);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.QuotationMixes.FirstOrDefault(x => x.Id == id);
            }
        }

        public static decimal FindMixVariableCost(long id, out double perHourRate)
        {
            using (var context = GetContext())
            {
                QuotationMix qm = context.QuotationMixes.FirstOrDefault(x => x.Id == id);
                if (qm != null)
                {
                    int numberOfTrips = (int)Math.Ceiling(qm.Volume.GetValueOrDefault() / qm.AvgLoad.GetValueOrDefault());
                    Plant plant = qm.Quotation.Plant;
                    Project project = qm.Quotation.Project;
                    int ticketedTime = plant.TicketMinutes.GetValueOrDefault(0) + plant.LoadMinutes.GetValueOrDefault(0) + plant.TemperMinutes.GetValueOrDefault(0);
                    ticketedTime = ticketedTime + project.ToJobMinutes.GetValueOrDefault(0) + project.WaitOnJob.GetValueOrDefault(0) + project.WashMinutes.GetValueOrDefault(0) + project.ReturnMinutes.GetValueOrDefault(0);
                    ticketedTime = ticketedTime + qm.Unload.GetValueOrDefault(0);
                    double roundTripTime = (double)(ticketedTime / plant.Utilization.GetValueOrDefault(1));
                    perHourRate = roundTripTime > 0 ? (qm.AvgLoad.GetValueOrDefault(0) / roundTripTime * 60.0) : 0;
                    double totalTimeSpent = numberOfTrips * roundTripTime;
                    decimal deliveryCost = plant.VariableCostPerMin.GetValueOrDefault(0) * Convert.ToDecimal(totalTimeSpent);
                    return deliveryCost / Convert.ToDecimal(qm.Volume.GetValueOrDefault(0));
                }
                perHourRate = 0;
                return 0;
            }
        }

        public static decimal FindMixVariableCost(QuotationMix mix, int plantId, decimal toJob, decimal waitOnJob, decimal washMinutes, decimal returnMinutes, out double perHourRate)
        {
            int numberOfTrips = (int)Math.Ceiling(mix.Volume.GetValueOrDefault() / mix.AvgLoad.GetValueOrDefault());
            Plant plant = SIDAL.GetPlant(plantId);
            decimal ticketedTime = plant.TicketMinutes.GetValueOrDefault(0) + plant.LoadMinutes.GetValueOrDefault(0) + plant.TemperMinutes.GetValueOrDefault(0);
            ticketedTime = ticketedTime + toJob + waitOnJob + washMinutes + returnMinutes;
            ticketedTime = ticketedTime + mix.Unload.GetValueOrDefault(0);
            double roundTripTime = (double)(ticketedTime / plant.Utilization.GetValueOrDefault(1));
            perHourRate = roundTripTime > 0 ? (mix.AvgLoad.GetValueOrDefault(0) / roundTripTime * 60.0) : 0;
            double totalTimeSpent = numberOfTrips * roundTripTime;
            decimal deliveryCost = plant.VariableCostPerMin.GetValueOrDefault(0) * Convert.ToDecimal(totalTimeSpent);
            return deliveryCost / Convert.ToDecimal(mix.Volume.GetValueOrDefault(0));
        }

        //public static decimal FindMixVariableCost(QuotationMix mix, int plantId, int toJob, int waitOnJob, int washMinutes, int returnMinutes, out double perHourRate)
        //{
        //    int numberOfTrips = (int)Math.Ceiling(mix.Volume.GetValueOrDefault() / mix.AvgLoad.GetValueOrDefault());
        //    Plant plant = SIDAL.GetPlant(plantId);
        //    int ticketedTime = plant.TicketMinutes.GetValueOrDefault(0) + plant.LoadMinutes.GetValueOrDefault(0) + plant.TemperMinutes.GetValueOrDefault(0);
        //    ticketedTime = ticketedTime + toJob + waitOnJob + washMinutes + returnMinutes;
        //    ticketedTime = ticketedTime + mix.Unload.GetValueOrDefault(0);
        //    double roundTripTime = (double)(ticketedTime / plant.Utilization.GetValueOrDefault(1));
        //    perHourRate = roundTripTime > 0 ? (mix.AvgLoad.GetValueOrDefault(0) / roundTripTime * 60.0) : 0;
        //    double totalTimeSpent = numberOfTrips * roundTripTime;
        //    decimal deliveryCost = plant.VariableCostPerMin.GetValueOrDefault(0) * Convert.ToDecimal(totalTimeSpent);
        //    return deliveryCost / Convert.ToDecimal(mix.Volume.GetValueOrDefault(0));
        //}

        public static void UpdateQuoteMixDescription(long id, string description)
        {
            using (var context = GetContext())
            {
                QuotationMix mix = context.QuotationMixes.FirstOrDefault(x => x.Id == id);
                if (mix != null)
                {
                    mix.QuotedDescription = description;
                }
                context.SubmitChanges();
            }
        }

        public static void UpdateQuoteMixPublicNotes(long id, string publicNotes)
        {
            using (var context = GetContext())
            {
                QuotationMix mix = context.QuotationMixes.FirstOrDefault(x => x.Id == id);
                if (mix != null)
                {
                    mix.PublicNotes = publicNotes;
                }
                context.SubmitChanges();
            }
        }

        public static void UpdateQuotationStandardMixCalculations(long quotationMixId, bool quotationSpread = false)
        {
            using (var context = GetContext())
            {
                QuotationMix mix = context.QuotationMixes.FirstOrDefault(x => x.Id == quotationMixId);
                if (mix != null)
                {
                    mix.AddonCost = mix.MixLevelAddons.Sum(x => x.Cost.GetValueOrDefault() * (decimal)x.Quantity.GetValueOrDefault());
                    if (quotationSpread)
                    {
                        mix.Spread = FindQuotationSpread(mix.QuotationId.GetValueOrDefault());
                    }
                    else
                    {
                        mix.Spread = mix.Price.GetValueOrDefault(0) - mix.MixCost.GetValueOrDefault(0) - mix.AddonCost.GetValueOrDefault(0);
                    }
                    mix.Price = mix.Spread.GetValueOrDefault(0) + mix.MixCost.GetValueOrDefault(0) + mix.AddonCost.GetValueOrDefault(0);
                    double cydPerHour = 0;
                    decimal variableDeliveryCost = SIDAL.FindMixVariableCost(mix.Id, out cydPerHour);
                    decimal fixedCosts = SIDAL.FindPlantFixedCosts(mix.Quotation.PlantId.GetValueOrDefault());
                    mix.CydHour = cydPerHour;
                    mix.Contribution = mix.Spread.GetValueOrDefault(0) - variableDeliveryCost;
                    mix.Profit = mix.Contribution - fixedCosts;
                    context.SubmitChanges();
                }
                UpdateQuotationCalculations(mix.QuotationId.GetValueOrDefault());
            }
        }

        public static QuotationMix CalculateQuotationStandardMixProperties(QuotationMix mix, int plantId, decimal toJob, decimal waitOnJob, decimal washMinutes, decimal returnMinutes)
        {
            using (var context = GetContext())
            {
                if (mix != null)
                {
                    mix.AddonCost = 0;
                    mix.Spread = mix.Price.GetValueOrDefault(0) - mix.MixCost.GetValueOrDefault(0);
                    double cydPerHour = 0;
                    decimal variableDeliveryCost = SIDAL.FindMixVariableCost(mix, plantId, toJob, waitOnJob, washMinutes, returnMinutes, out cydPerHour);
                    decimal fixedCosts = SIDAL.FindPlantFixedCosts(plantId);
                    mix.CydHour = cydPerHour;
                    mix.Contribution = mix.Spread.GetValueOrDefault(0) - variableDeliveryCost;
                    mix.Profit = mix.Contribution - fixedCosts;
                    return mix;
                }
                return null;
            }
        }

        #endregion

        #region MixLevelAddon

        private static DataLoadOptions GetMixLevelAddonLoadOptions()
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<MixLevelAddon>(x => x.Addon);
            return opts;
        }

        public static MixLevelAddon FindMixLevelAddon(long id)
        {
            using (var context = GetContext())
            {
                context.LoadOptions = GetMixLevelAddonLoadOptions();
                return context.MixLevelAddons.FirstOrDefault(x => x.Id == id);
            }
        }

        public static List<MixLevelAddon> GetMixLevelAddons(long quotationMixId)
        {
            using (var context = GetContext())
            {
                context.LoadOptions = GetMixLevelAddonLoadOptions();
                return context.MixLevelAddons.Where(x => x.QuotationMixId == quotationMixId).ToList();
            }
        }

        public static void UpdateMixLevelAddon(MixLevelAddon mixAddon)
        {
            using (var context = GetContext())
            {
                if (mixAddon.Id == 0)
                {
                    context.MixLevelAddons.InsertOnSubmit(mixAddon);
                }
                else
                {
                    context.MixLevelAddons.Attach(mixAddon);
                    context.Refresh(RefreshMode.KeepCurrentValues, mixAddon);
                }
                context.SubmitChanges();
            }
        }

        public static void DeleteMixLevelAddon(long id)
        {
            using (var context = GetContext())
            {
                MixLevelAddon entity = context.MixLevelAddons.FirstOrDefault(x => x.Id == id);
                context.MixLevelAddons.DeleteOnSubmit(entity);
                context.SubmitChanges();
            }
        }

        #endregion

        #region CustomMixConstituents

        public static CustomMixConstituent FindCustomMixConstituent(long id)
        {
            using (var context = GetContext())
            {
                return context.CustomMixConstituents.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public static void UpdateCustomMixConstituent(CustomMixConstituent entity, bool withQuotationSpread = false)
        {
            using (var context = GetContext())
            {
                if (entity.Id == 0)
                {
                    context.CustomMixConstituents.InsertOnSubmit(entity);
                }
                else
                {
                    context.CustomMixConstituents.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
                UpdateQuotationCustomMixCalculations(entity.QuotationMixId, withQuotationSpread);
            }
        }

        public static decimal UpdateCustomMixDefaultPrice(long quotationMixId)
        {
            using (var context = GetContext())
            {
                decimal price = 0;
                var quotationMix = context.QuotationMixes.FirstOrDefault(x => x.Id == quotationMixId);

                if (quotationMix != null)
                {
                    price = quotationMix.Spread.GetValueOrDefault() + quotationMix.AddonCost.GetValueOrDefault() + quotationMix.MixCost.GetValueOrDefault();

                    quotationMix.Price = price;

                    context.SubmitChanges();
                }

                return price;
            }
        }

        public static List<CustomMixConstituent> GetCustomMixConstituents(long mixId)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<CustomMixConstituent>(x => x.CostUom);
            opts.LoadWith<CustomMixConstituent>(x => x.QuantityUom);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.CustomMixConstituents.Where(x => x.QuotationMixId == mixId).ToList();
            }
        }

        public static void DeleteCustomMixConstituent(long id)
        {
            using (var context = GetContext())
            {
                var query = context.CustomMixConstituents.Where(x => x.Id == id).First();
                context.CustomMixConstituents.DeleteOnSubmit(query);
                context.SubmitChanges();
                UpdateQuotationCustomMixCalculations(query.QuotationMixId, true);
            }
        }

        public static void UpdateQuotationCustomMixCalculations(long quotationMixId, bool withQuotationSpread = false)
        {
            using (var context = GetContext())
            {
                QuotationMix mix = context.QuotationMixes.Where(x => x.Id == quotationMixId).FirstOrDefault();
                if (mix != null)
                {
                    var query = context.CustomMixConstituents.Where(x => x.QuotationMixId == quotationMixId);
                    mix.AddonCost = 0;
                    foreach (CustomMixConstituent constituent in query.Where(x => x.AddonId != null))
                    {
                        mix.AddonCost += Convert.ToDecimal(constituent.Quantity) * constituent.Cost.GetValueOrDefault(0);
                    }
                    List<SIQuotationMixIngredient> constituents = new List<SIQuotationMixIngredient>();
                    foreach (CustomMixConstituent constituent in query.Where(x => x.AddonId == null))
                    {
                        SIQuotationMixIngredient ingredient = new SIQuotationMixIngredient(constituent);
                        constituents.Add(ingredient);
                    }
                    mix.MixCost = SIQuotationMixIngredient.CalculateStandardMixTotal(constituents);
                    if (withQuotationSpread)
                    {
                        mix.Spread = FindQuotationSpread(mix.QuotationId.GetValueOrDefault());
                    }
                    else
                    {
                        mix.Spread = mix.Price - mix.MixCost - mix.AddonCost;
                    }
                    double cydHour = 0;
                    decimal variableDeliveryCost = SIDAL.FindMixVariableCost(mix.Id, out cydHour);
                    decimal fixedCosts = SIDAL.FindPlantFixedCosts(mix.Quotation.PlantId.GetValueOrDefault());
                    mix.Contribution = mix.Spread - variableDeliveryCost;
                    mix.Profit = mix.Contribution - fixedCosts;
                    mix.CydHour = cydHour;
                    context.SubmitChanges();
                }
                UpdateQuotationCalculations(mix.QuotationId.GetValueOrDefault());
            }
        }
        public static decimal FindQuotationSpread(long quoteId)
        {
            using (var context = GetContext())
            {
                Quotation quote = context.Quotations.Where(x => x.Id == quoteId).FirstOrDefault();

                Plant configPlant = SIDAL.GetPlant(quote.PlantId);
                District configDistrict = SIDAL.GetDistrict(configPlant.DistrictId);
                decimal spread;
                DistrictMarketSegment dms = SIDAL.FindDistrictMarketSegment(quote.Project.MarketSegmentId.GetValueOrDefault(), configDistrict.DistrictId);
                if (dms != null)
                {
                    spread = dms.Spread.GetValueOrDefault();
                }
                else
                {
                    spread = 0;
                }
                return spread;
            }
        }
        public static CustomMixConstituent QuickAddRawMaterialCustomMixConstituent(long quotationMixId, double? quantity = 1, bool? perCementWeight = false)
        {
            using (var context = GetContext())
            {
                CustomMixConstituent cMixConst = new CustomMixConstituent();
                cMixConst.QuotationMixId = quotationMixId;

                QuotationMix qMix = SIDAL.FindQuotationMix(quotationMixId);
                Quotation q = SIDAL.FindQuotation(qMix.QuotationId.GetValueOrDefault());
                Plant p = SIDAL.GetPlant(q.PlantId);

                var rawMaterials = GetNonZeroRawMaterials(q.PlantId.GetValueOrDefault());

                RawMaterial rawMaterial = rawMaterials.FirstOrDefault();

                cMixConst.RawMaterialId = rawMaterial.Id;
                Uom uom = context.Uoms.FirstOrDefault();

                if (uom != null)
                    cMixConst.QuantityUomId = uom.Id;
                cMixConst.Quantity = quantity.GetValueOrDefault(1);
                cMixConst.PerCementWeight = perCementWeight;

                cMixConst.Description = rawMaterial.MaterialCode + " - " + rawMaterial.Description;
                RawMaterialCostProjection projection = SIDAL.FindRawMaterialCost(rawMaterial.Id, p.PlantId);

                cMixConst.IsCementitious = rawMaterial.RawMaterialType.IsCementitious;

                if (projection != null && projection.Cost > 0)
                {
                    cMixConst.Cost = projection.Cost;
                    cMixConst.CostUomId = projection.UomId;
                }
                else
                {
                    cMixConst.Cost = 0;
                    if (uom != null)
                        cMixConst.CostUomId = uom.Id;
                }
                SIDAL.UpdateCustomMixConstituent(cMixConst, true);
                return cMixConst;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quotationMixId"></param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static CustomMixConstituent QuickAddAddOnCustomMixConstituent(long quotationMixId)
        {
            using (var context = GetContext())
            {
                QuotationMix qMix = context.QuotationMixes.FirstOrDefault(x => x.Id == quotationMixId);
                Quotation q = SIDAL.FindQuotation(qMix.QuotationId.GetValueOrDefault());
                District d = SIDAL.GetDistrict(SIDAL.GetPlant(q.PlantId).DistrictId);

                var addons = GetActiveAddons("Mix", q.PlantId.GetValueOrDefault(), q.PricingMonth).Where(x => x.MixUom.Name != "N.A.");
                Addon addon = addons.FirstOrDefault();

                CustomMixConstituent entity = new CustomMixConstituent();
                entity.QuotationMixId = quotationMixId;
                entity.AddonId = addon.Id;
                entity.Description = addon.Code + " - " + addon.Description;
                entity.Quantity = 1;
                var priceProjection = SIDAL.FindAddonMixPriceProjection(addon.Id, d.DistrictId, q.PricingMonth);
                if (priceProjection != null)
                {
                    entity.Cost = priceProjection.Price;
                    entity.CostUomId = priceProjection.UomId;
                }
                else
                {
                    entity.Cost = 0;
                }

                if (entity.Cost > 0)
                {
                    SIDAL.UpdateCustomMixConstituent(entity, true);
                }
                else
                {
                    throw new ApplicationException("Addons with 0 cost cannot be added");
                }
                return entity;
            }
        }

        public static CustomMixConstituent QuickAddNonStandardConstMixConstituent(long quotationMixId)
        {
            using (var context = GetContext())
            {
                QuotationMix qMix = context.QuotationMixes.FirstOrDefault(x => x.Id == quotationMixId);
                Quotation q = SIDAL.FindQuotation(qMix.QuotationId.GetValueOrDefault());
                District d = SIDAL.GetDistrict(SIDAL.GetPlant(q.PlantId).DistrictId);

                CustomMixConstituent entity = new CustomMixConstituent();

                entity.QuotationMixId = quotationMixId;
                entity.Description = "Non Standard Constituent";
                entity.IsCementitious = false;
                entity.Quantity = 1;
                entity.QuantityUomId = GetUOMS().Where(x => x.Category == "Weight" || x.Category == "Volume").OrderBy(x => x.Category).ThenBy(x => x.Priority2).Select(x => x.Id).FirstOrDefault();
                entity.Cost = 1;
                entity.CostUomId = GetUOMS().Where(x => x.Category == "Weight" || x.Category == "Volume").OrderBy(x => x.Category).ThenBy(x => x.Priority).Select(x => x.Id).FirstOrDefault();
                entity.PerCementWeight = false;
                if (entity.Cost > 0)
                {
                    SIDAL.UpdateCustomMixConstituent(entity, true);
                }
                return entity;
            }
        }

        #endregion

        #region Quotation Approval
        public static List<SIUser> GetQuoteApprovalManagers(long quotationId, bool usersWithinMarginLimits = false)
        {
            List<SIUser> allUsers = new List<SIUser>();
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                DataLoadOptions opts = new DataLoadOptions();
                opts.LoadWith<Quotation>(x => x.Plant);
                opts.LoadWith<Plant>(x => x.District);

                context.LoadOptions = opts;

                Quotation quotation = context.Quotations.Where(x => x.Id == quotationId).FirstOrDefault();
                if (quotation.PlantId == null)
                {
                    return new List<SIUser>();
                }
                District quoteDistrict = quotation.Plant.District;
                double volume = quotation.TotalVolume.GetValueOrDefault(0);
                var query = from du in context.DistrictUsers
                            join qr in context.QuotationRecipents on du.UserId equals qr.UserId
                            where du.DistrictId == quoteDistrict.DistrictId
                            && (qr.QuotationLimit >= volume)
                            select du.UserId;

                foreach (Guid userId in query)
                {
                    MembershipUser user = Membership.GetUser(userId);
                    SIUser getUser = new SIUser
                    {
                        UserId = (Guid)user.ProviderUserKey,
                        Username = user.UserName,
                        Email = user.Email,
                        Active = user.IsApproved,
                        Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                        Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                        Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault()
                    };
                    if (getUser.Active)
                    {
                        if (usersWithinMarginLimits)
                        {
                            if (GetMarginLimitViolations(quotationId, getUser.Role).Count == 0)
                            {
                                allUsers.Add(getUser);
                            }

                        }
                        else
                        {
                            allUsers.Add(getUser);
                        }
                    }
                }
                return allUsers;
            }
        }

        public static double GetUserQuoteApprovalLimit(Guid userId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                var userQuotationLimit = (Double)context.QuotationRecipents.Where(x => x.UserId == userId).Select(x => x.QuotationLimit).FirstOrDefault().GetValueOrDefault(0);
                return userQuotationLimit;
            }
        }

        public static bool CheckUserQuoteApprovalLimit(Guid userId, long quotationId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Quotation quotation = context.Quotations.Where(x => x.Id == quotationId).FirstOrDefault();
                double volume = quotation.TotalVolume.GetValueOrDefault(0);
                var userQuotationLimit = (Double)context.QuotationRecipents.Where(x => x.UserId == userId).Select(x => x.QuotationLimit).FirstOrDefault().GetValueOrDefault(0);
                if (userQuotationLimit >= volume)
                    return true;
                else
                    return false;
            }
        }
        public static List<SIUser> GetQuoteApprovalUser(long quotationId)
        {
            List<SIUser> allUsers = new List<SIUser>();
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Quotation quotation = context.Quotations.Where(x => x.Id == quotationId).FirstOrDefault();
                if (quotation.PlantId == null)
                {
                    return new List<SIUser>();
                }
                District quoteDistrict = quotation.Plant.District;
                double volume = quotation.TotalVolume.GetValueOrDefault(0);

                var query = from du in context.DistrictUsers
                            join qr in context.QuotationRecipents on du.UserId equals qr.UserId
                            where du.DistrictId == quoteDistrict.DistrictId
                            && (qr.QuotationLimit > volume) && du.UserId != quotation.ApprovedBy
                            select du.UserId;

                foreach (Guid userId in query)
                {
                    MembershipUser user = Membership.GetUser(userId);
                    SIUser getUser = new SIUser
                    {
                        UserId = (Guid)user.ProviderUserKey,
                        Username = user.UserName,
                        Email = user.Email,
                        Active = user.IsApproved,
                        Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                        Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                        Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault()
                    };
                    if (getUser.Active)
                    {
                        allUsers.Add(getUser);
                    }
                }
                return allUsers;
            }
        }
        public static List<SIUser> GetProjectEntryRecipients(long districtId)
        {
            List<SIUser> allUsers = new List<SIUser>();
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {

                var query = from du in context.DistrictUsers
                            join qr in context.QuotationRecipents on du.UserId equals qr.UserId
                            where du.DistrictId == districtId
                            && (qr.QuotationAccess == true)
                            select du.UserId;

                foreach (Guid userId in query)
                {
                    MembershipUser user = Membership.GetUser(userId);
                    SIUser getUser = new SIUser
                    {
                        UserId = (Guid)user.ProviderUserKey,
                        Username = user.UserName,
                        Email = user.Email,
                        Active = user.IsApproved,
                        Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                        Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                        Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault()
                    };
                    if (getUser.Active)
                    {
                        allUsers.Add(getUser);
                    }
                }
                return allUsers;
            }
        }
        public static void QueueQuotationForApproval(string sender, long quotationId, string[] uids, string message, string subject)
        {
            //long conversationId = CreateFindConversationForQuotation(quotationId);
            //AddUsersToConversation(conversationId, uids.ToList());
            //CreateSystemMessage(conversationId, sender+" requested approval for this quote");
            Guid senderId = GetUserIdFromUserName(sender);
            if (senderId != Guid.Empty)
                //CreateMessage(conversationId, message, senderId, uids.ToList());

                using (var context = GetContext())
                {
                    foreach (string uid in uids)
                    {
                        Guid userId = new Guid(uid);
                        Notification n = context.Notifications.
                                            Where(x => x.UserId == userId).
                                            Where(x => x.QuotationId == quotationId).
                                            Where(x => x.NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_QUOTATION_APPROVAL).FirstOrDefault();
                        // Notification is not present already, only then create a new one. Else ignore.
                        if (n == null)
                        {
                            n = new Notification();
                            n.UserId = userId;
                            n.CreatedByUser = sender;
                            n.QuotationId = quotationId;
                            n.NotificationType = SINotificationStatuses.NOTIFICATION_TYPE_QUOTATION_APPROVAL;
                            n.MessageSubject = subject;
                            n.MessageText = message;
                            n.CreatedOn = DateTime.Now;
                            n.IsRead = false;
                            n.Status = SINotificationStatuses.APPROVAL_STATUS_NEW;
                            context.Notifications.InsertOnSubmit(n);
                        }
                    }
                    context.SubmitChanges();
                }
        }
        public static List<SIUser> GetUsersForUserIds(string[] userIds)
        {
            using (var context = GetContext())
            {
                List<SIUser> users = new List<SIUser>();
                foreach (string uid in userIds)
                {
                    Guid userId = new Guid(uid);
                    MembershipUser user = Membership.GetUser(userId);
                    SIUser getUser = new SIUser
                    {
                        UserId = (Guid)user.ProviderUserKey,
                        Username = user.UserName,
                        Email = user.Email,
                        Active = user.IsApproved,
                        Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                        Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                        Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault()
                    };
                    users.Add(getUser);
                }
                return users;
            }
        }
        public static void SendProjectEntryNotifications(string sender, long quotationId, string[] uids, string message, string subject)
        {
            //long conversationId = CreateFindConversationForQuotation(quotationId);
            //AddUsersToConversation(conversationId, uids.ToList());
            //CreateSystemMessage(conversationId, sender + " sent project entry form for this quote");
            Guid senderId = GetUserIdFromUserName(sender);
            if (senderId != Guid.Empty)
                //CreateMessage(conversationId, message, senderId, uids.ToList());

                using (var context = GetContext())
                {
                    foreach (string uid in uids)
                    {
                        Guid userId = new Guid(uid);
                        Notification n = context.Notifications.
                                            Where(x => x.UserId == userId).
                                            Where(x => x.QuotationId == quotationId).
                                            Where(x => x.NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_PROJECT_ENTRY).FirstOrDefault();
                        // Notification is not present already, only then create a new one. Else ignore.
                        if (n == null)
                        {
                            n = new Notification();
                            n.UserId = userId;
                            n.CreatedByUser = sender;
                            n.QuotationId = quotationId;
                            n.NotificationType = SINotificationStatuses.NOTIFICATION_TYPE_PROJECT_ENTRY;
                            n.MessageSubject = subject;
                            n.MessageText = message;
                            n.CreatedOn = DateTime.Now;
                            n.IsRead = false;
                            n.Status = SINotificationStatuses.APPROVAL_STATUS_NEW;
                            context.Notifications.InsertOnSubmit(n);
                        }
                    }
                    context.SubmitChanges();

                    Quotation Quote = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
                    if (Quote != null)
                        Quote.ProjectFormNotificationDate = DateTime.Now;
                    context.SubmitChanges();
                }
        }
        public static void SendCommentNotification(string sender, long quotationId, string[] uids, string message, string subject)
        {
            //long conversationId = CreateFindConversationForQuotation(quotationId);
            //AddUsersToConversation(conversationId, uids.ToList());
            Guid senderId = GetUserIdFromUserName(sender);
            if (senderId != Guid.Empty)
                if (!String.IsNullOrEmpty(message))
                    //CreateMessage(conversationId, message, senderId, uids.ToList());

                    using (var context = GetContext())
                    {
                        foreach (string uid in uids)
                        {
                            Guid userId = new Guid(uid);
                            Notification n = new Notification();
                            n.UserId = userId;
                            n.CreatedByUser = sender;
                            n.QuotationId = quotationId;
                            n.NotificationType = SINotificationStatuses.NOTIFICATION_TYPE_COMMENT;
                            n.MessageSubject = subject;
                            n.MessageText = message;
                            n.CreatedOn = DateTime.Now;
                            n.IsRead = false;
                            n.Status = SINotificationStatuses.APPROVAL_STATUS_NEW;
                            context.Notifications.InsertOnSubmit(n);

                        }
                        context.SubmitChanges();
                    }
        }
        public static void SendApprovalNotification(string sender, long quotationId, string[] uids, string message, string subject)
        {
            //long conversationId = CreateFindConversationForQuotation(quotationId);
            //CreateSystemMessage(conversationId, sender + " approved this quote.");
            //AddUsersToConversation(conversationId, uids.ToList());
            Guid senderId = GetUserIdFromUserName(sender);
            if (senderId != Guid.Empty)
                //if (!String.IsNullOrEmpty(message))
                //CreateMessage(conversationId, message, senderId, uids.ToList());

                using (var context = GetContext())
                {
                    foreach (string uid in uids)
                    {
                        Guid userId = new Guid(uid);
                        Notification n = context.Notifications.
                                            Where(x => x.UserId == userId).
                                            Where(x => x.QuotationId == quotationId).
                                            Where(x => x.NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_APPROVED).FirstOrDefault();
                        // Notification is not present already, only then create a new one. Else ignore.
                        if (n == null)
                        {
                            n = new Notification();
                            n.UserId = userId;
                            n.CreatedByUser = sender;
                            n.QuotationId = quotationId;
                            n.NotificationType = SINotificationStatuses.NOTIFICATION_TYPE_APPROVED;
                            n.MessageSubject = subject;
                            n.MessageText = message;
                            n.CreatedOn = DateTime.Now;
                            n.IsRead = false;
                            n.Status = SINotificationStatuses.APPROVAL_STATUS_APPROVED;
                            context.Notifications.InsertOnSubmit(n);
                        }
                    }
                    context.SubmitChanges();
                }
        }

        public static void SendQuotationEditingNotification(string sender, long quotationId, string[] uids, string message, string subject)
        {

            Guid senderId = GetUserIdFromUserName(sender);
            if (senderId != Guid.Empty)
                if (!String.IsNullOrEmpty(message))
                    //CreateMessage(conversationId, message, senderId, uids.ToList());

                    using (var context = GetContext())
                    {
                        foreach (string uid in uids)
                        {
                            Guid userId = new Guid(uid);
                            Notification n = context.Notifications.
                                                Where(x => x.UserId == userId).
                                                Where(x => x.QuotationId == quotationId).
                                                Where(x => x.NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_ENABLE_QUOTATION_EDIT).FirstOrDefault();
                            // Notification is not present already, only then create a new one. Else ignore.
                            if (n == null)
                            {
                                n = new Notification();
                                n.UserId = userId;
                                n.CreatedByUser = sender;
                                n.QuotationId = quotationId;
                                n.NotificationType = SINotificationStatuses.NOTIFICATION_TYPE_ENABLE_QUOTATION_EDIT;
                                n.MessageSubject = subject;
                                n.MessageText = message;
                                n.CreatedOn = DateTime.Now;
                                n.IsRead = false;
                                n.Status = SINotificationStatuses.QUOTATION_EDITING_STATUS_ENABLED;
                                context.Notifications.InsertOnSubmit(n);
                            }
                        }
                        context.SubmitChanges();
                    }
        }

        public static bool CheckApprovalAccess(long id, Guid guid)
        {
            using (var context = GetContext())
            {
                List<SIUser> approvalManagers = GetQuoteApprovalManagers(id);
                return (approvalManagers.Select(x => x.UserId).Contains(guid));
            }
        }
        public static void ApproveQuote(long id, SIUser u)
        {
            using (var context = GetContext())
            {
                Quotation q = context.Quotations.FirstOrDefault(x => x.Id == id);
                q.Status = "APPROVED";
                q.ApprovalDate = DateTime.Now;
                q.ApprovedBy = u.UserId;
                context.SubmitChanges();
                var notifications = context.Notifications.Where(x => x.QuotationId == id).Where(x => x.NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_QUOTATION_APPROVAL);
                context.Notifications.DeleteAllOnSubmit(notifications);
                context.SubmitChanges();
            }
        }
        #endregion

        #region Quotation Editing Enable
        public static void EnableEditQuote(long id, Guid userId, bool enableEdit = true, bool withoutNotification = true)
        {
            using (var context = GetContext())
            {
                Quotation q = context.Quotations.FirstOrDefault(x => x.Id == id);
                q.EnableEdit = enableEdit;
                if (enableEdit)
                {
                    q.EditingEnabledBy = userId;
                    q.EditingEnabledDate = DateTime.Now;
                }
                context.SubmitChanges();
                if (withoutNotification == true)
                {
                    var notifications = context.Notifications.Where(x => x.QuotationId == id).Where(x => x.NotificationType == SINotificationStatuses.NOTIFICATION_TYPE_ENABLE_QUOTATION_EDIT);
                    context.Notifications.DeleteAllOnSubmit(notifications);
                    context.SubmitChanges();
                }
            }
        }
        #endregion

        #region Notifications
        public static List<Notification> GetLatestNotifications(Guid userId, int count, out int unread)
        {
            using (var context = GetContext())
            {
                var query = context.Notifications.Where(x => x.UserId == userId);
                unread = query.Count();
                query = query.OrderByDescending(x => x.CreatedOn);
                return query.Skip(0).Take(count).ToList();
            }
        }
        public static void MarkNotificationRead(string type, long quotationId, Guid userId)
        {
            using (var context = GetContext())
            {
                var query = context.Notifications.Where(x => x.UserId == userId).Where(x => x.NotificationType == type).Where(x => x.QuotationId == quotationId).FirstOrDefault();
                if (query != null)
                {
                    query.IsRead = true;
                    context.SubmitChanges();
                }
            }
        }
        public static List<Notification> GetNotifications(Guid userId, string notificationType, int[] districtIds, int[] plantIds, int[] salesStaffIds, string searchTerm, int pageNum, int numRows, out int count)
        {
            using (var context = GetContext())
            {
                var query = context.Notifications.Where(x => x.UserId == userId);
                if (notificationType != null)
                {
                    query = query.Where(x => x.NotificationType == notificationType);
                }
                if (districtIds != null && districtIds.Count() > 0)
                {
                    query = query.Where(x => districtIds.Contains(x.Quotation.Plant.DistrictId));
                }
                if (plantIds != null && plantIds.Count() > 0)
                {
                    query = query.Where(x => plantIds.Contains(x.Quotation.PlantId.GetValueOrDefault()));
                }
                if (salesStaffIds != null && salesStaffIds.Count() > 0)
                {
                    query = query.Where(x => salesStaffIds.Contains(x.Quotation.Project.ProjectSalesStaffs.FirstOrDefault().SalesStaffId));
                }
                if (searchTerm != null)
                {
                    query = query.Where(x => x.MessageSubject.Contains(searchTerm) || x.MessageText.Contains(searchTerm) || x.CreatedByUser.Contains(searchTerm));
                }
                count = query.Count();
                return query.Skip(pageNum * numRows).Take(numRows).ToList();
            }
        }
        public static void DeleteNotification(long id)
        {
            using (var context = GetContext())
            {
                var query = context.Notifications.Where(x => x.Id == id).FirstOrDefault();
                if (query != null)
                    context.Notifications.DeleteOnSubmit(query);
                context.SubmitChanges();
            }
        }
        #endregion

        #region Conversations
        //public static long CreateFindConversationForQuotation(long quotationId)
        //{
        //    using (var context = GetContext())
        //    {
        //        Conversation c = context.Conversations.Where(x => x.QuotationId == quotationId).FirstOrDefault();
        //        if (c == null)
        //        {
        //            c = new Conversation();
        //            c.QuotationId = quotationId;
        //            c.CreatedOn = DateTime.Now;
        //            c.UpdatedOn = DateTime.Now;
        //            context.Conversations.InsertOnSubmit(c);
        //            context.SubmitChanges();
        //        }
        //        return c.Id;
        //    }
        //}
        //public static long CreateFindConversationForProject(int projectId)
        //{
        //    using (var context = GetContext())
        //    {
        //        Conversation c = context.Conversations.Where(x => x.ProjectId == projectId).First();
        //        if (c == null)
        //        {
        //            c = new Conversation();
        //            c.ProjectId = projectId;
        //            c.CreatedOn = DateTime.Now;
        //            c.UpdatedOn = DateTime.Now;
        //            context.Conversations.InsertOnSubmit(c);
        //            context.SubmitChanges();
        //        }
        //        return c.Id;
        //    }
        //}
        //public static void AddUsersToConversation(long conversationId, List<string> userIds)
        //{
        //    using (var context = GetContext())
        //    {
        //        foreach (string u in userIds)
        //        {
        //            Guid userId = new Guid(u);
        //            if (context.ConversationParticipants.FirstOrDefault(x => x.ConversationId == conversationId && x.UserId == userId) == null)
        //            {
        //                ConversationParticipant cp = new ConversationParticipant();
        //                cp.ConversationId = conversationId ;
        //                cp.UserId = userId;
        //                context.ConversationParticipants.InsertOnSubmit(cp);
        //                context.SubmitChanges();
        //            }
        //        }
        //    }
        //}
        //public static void CreateMessage(long conversationId, string text, Guid user, List<string> mentions)
        //{
        //    using (var context = GetContext())
        //    {
        //        Conversation c = context.Conversations.FirstOrDefault(x => x.Id == conversationId);
        //        if (c != null)
        //        {
        //            ConversationMessage cm = new ConversationMessage();
        //            cm.UserId = user;
        //            cm.Mode = "USER";
        //            cm.ConversationId = conversationId;
        //            cm.Text = text;
        //            cm.CreatedOn = DateTime.Now;
        //            context.ConversationMessages.InsertOnSubmit(cm);
        //            context.SubmitChanges();

        //            foreach (string mention in mentions)
        //            {
        //                MessageMention mm = new MessageMention();
        //                mm.UserId = new Guid(mention);
        //                SIUser siuser = GetUser(mm.UserId);
        //                mm.UserName = siuser.Username;
        //                mm.MessageId = cm.Id;
        //                context.MessageMentions.InsertOnSubmit(mm);
        //                context.SubmitChanges();
        //            }
        //            c.MessageId = cm.Id;
        //            context.SubmitChanges();
        //        }
        //    }
        //}
        //public static void CreateSystemMessage(long conversationId, string text)
        //{
        //    using (var context = GetContext())
        //    {
        //        ConversationMessage cm = new ConversationMessage();
        //        cm.Mode = "SYSTEM";
        //        cm.ConversationId = conversationId;
        //        cm.Text = text;
        //        context.ConversationMessages.InsertOnSubmit(cm);
        //        context.SubmitChanges();
        //    }
        //}
        //public static List<Conversation> GetConversationsForUser(Guid userId, int[] selectedDistricts, string searchTerm, DateTime startDate, DateTime endDate, int currentPage, int numRows)
        //{
        //    using (var context = GetContext())
        //    {
        //        var query = context.ConversationParticipants.Where(x => x.UserId == userId).Select(x => x.Conversation);
        //        if (selectedDistricts != null)
        //        {
        //            query = query.Where(x =>
        //                (x.QuotationId != null && selectedDistricts.Contains(x.Quotation.Plant.DistrictId)) ||
        //                (x.ProjectId != null && selectedDistricts.Contains(x.Project.Plant.DistrictId))
        //            );
        //        }
        //        if (String.IsNullOrEmpty(searchTerm))
        //        {
        //            query = query.Where(x=>
        //                (x.QuotationId != null && x.Quotation.Project.Name.Contains(searchTerm) ||
        //                (x.ProjectId != null && x.Project.Name.Contains(searchTerm)))
        //            );
        //        }
        //        query = query.Skip(currentPage * numRows).Take(numRows);
        //        query = query.OrderBy(x => x.UpdatedOn);
        //        return query.ToList();
        //    }
        //}
        #endregion

        #region Targets
        public static void ExecutePlantTargetOperation(SIPlantTargetOperation operation)
        {
            // Plant budget id
            int plantBudgetID = operation.PlantBudgetID;

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // If we dont have a PlantBudget yet

                //var plantBudgets = (from n in context.PlantBudgets 
                //                    where n.PlantId == operation.PlantID 
                //                    && n.BudgetDate == operation.BudgetDate 
                //                    select n);
                var plantBudgets = context.PlantBudgets
                                          .Where(x => x.PlantId == operation.PlantID)
                                          .Where(x => x.BudgetDate == operation.BudgetDate);


                if (plantBudgets.Count() == 0)
                {
                    // Create it
                    PlantBudget plantBudget = new PlantBudget
                    {
                        PlantId = operation.PlantID,
                        BudgetDate = operation.BudgetDate,
                        Budget = operation.Budget == int.MinValue ? 0 : operation.Budget,
                        Trucks = operation.Trucks == int.MinValue ? 0 : operation.Trucks
                    };

                    // Insert it
                    context.PlantBudgets.InsertOnSubmit(plantBudget);

                    // Execute the changes
                    context.SubmitChanges();

                    // Get the id
                    plantBudgetID = plantBudget.PlantBudgetId;
                }
                else
                {

                    PlantBudget plantBudget = plantBudgets.AsEnumerable().First();
                    plantBudgetID = plantBudget.PlantBudgetId;

                    // If they want to update the budget
                    if (operation.OperationType == SIPlantTargetOperationType.PlantBudget)
                    {
                        // Get the current plant budget


                        // If we got it
                        if (plantBudget != null)
                        {
                            // Set the values
                            plantBudget.Budget = operation.Budget == int.MinValue ? plantBudget.Budget : operation.Budget;
                            plantBudget.Trucks = operation.Trucks == int.MinValue ? plantBudget.Trucks : operation.Trucks;

                            // Execute the changes
                            context.SubmitChanges();
                        }
                    }
                }

                // If they are saving a salesstaff
                if (operation.OperationType == SIPlantTargetOperationType.PlantBudgetSalesSaff)
                {
                    var SalesStaffs = (from n in context.PlantBudgetSalesStaffs where n.SalesStaffId == operation.SalesStaffID && n.PlantBudgetId == plantBudgetID select n);

                    // If they need to add this
                    if (SalesStaffs.Count() == 0)
                    {
                        // Create it
                        PlantBudgetSalesStaff plantBudgetSalesStaff = new PlantBudgetSalesStaff
                        {
                            PlantBudgetId = plantBudgetID,
                            SalesStaffId = operation.SalesStaffID,
                            Percentage = operation.Percentage
                        };

                        // Insert it
                        context.PlantBudgetSalesStaffs.InsertOnSubmit(plantBudgetSalesStaff);

                        // Execute the changes
                        context.SubmitChanges();
                    }
                    else
                    {
                        // Get the current plant budget
                        PlantBudgetSalesStaff plantBudgetSalesStaff = SalesStaffs.AsEnumerable().First();

                        // If we got it
                        if (plantBudgetSalesStaff != null)
                        {
                            // Set the values
                            plantBudgetSalesStaff.Percentage = operation.Percentage == null || operation.Percentage == short.MinValue ? plantBudgetSalesStaff.Percentage : operation.Percentage;

                            // Execute the changes
                            context.SubmitChanges();
                        }
                    }
                }
                else if (operation.OperationType == SIPlantTargetOperationType.PlantBudgetMarketSegment)
                {

                    var MarketSegments = (from n in context.PlantBudgetMarketSegments where n.MarketSegmentId == operation.MarketSegmentID && n.PlantBudgetId == plantBudgetID select n);

                    // If they need to add this
                    if (MarketSegments.Count() == 0)
                    {
                        // Create it
                        PlantBudgetMarketSegment plantBudgetMarketSegment = new PlantBudgetMarketSegment
                        {
                            PlantBudgetId = plantBudgetID,
                            MarketSegmentId = operation.MarketSegmentID,
                            Percentage = operation.Percentage
                        };

                        // Insert it
                        context.PlantBudgetMarketSegments.InsertOnSubmit(plantBudgetMarketSegment);

                        // Execute the changes
                        context.SubmitChanges();
                    }
                    else
                    {
                        // Get the current plant budget
                        PlantBudgetMarketSegment plantBudgetMarketSegment = MarketSegments.AsEnumerable().First();

                        // If we got it
                        if (plantBudgetMarketSegment != null)
                        {
                            // Set the values
                            plantBudgetMarketSegment.Percentage = operation.Percentage == null || operation.Percentage == short.MinValue ? plantBudgetMarketSegment.Percentage : operation.Percentage;

                            // Execute the changes
                            context.SubmitChanges();
                        }
                    }
                }
            }
        }
        public static List<SIPlantTargetRow> GetAllPlantTargets(Guid userId, DateTime month)
        {
            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                DateTime monthBegin = new DateTime(month.Year, month.Month, 1, 0, 0, 0);
                DateTime monthEnd = monthBegin.AddMonths(1).Subtract(new TimeSpan(0, 0, 1));

                var results =
                (
                    from d in context.DistrictUsers
                    join p in context.Plants on d.DistrictId equals p.DistrictId // All the plants
                    from b in
                        (from b in context.PlantBudgets where p.PlantId == b.PlantId && (b.BudgetDate >= monthBegin && b.BudgetDate <= monthEnd) select b).DefaultIfEmpty()
                    where d.UserId == userId && p.Active == true
                    orderby p.Name ascending
                    select new
                    {
                        Plant = p,
                        PlantBudget = b,

                        PlantTargetSalesStaffs =
                        (
                            from f in context.DistrictUsers
                            join s in context.DistrictSalesStaffs on f.DistrictId equals s.DistrictId
                            join ss in context.SalesStaffs on s.SalesStaffId equals ss.SalesStaffId
                            from t in
                                (from t in context.PlantBudgetSalesStaffs where s.SalesStaffId == t.SalesStaffId && t.PlantBudgetId == b.PlantBudgetId select t).DefaultIfEmpty()
                            where f.DistrictId == p.DistrictId && ss.Active == true
                            select new
                            {
                                SalesStaff = s.SalesStaff,
                                PlantBudgetSalesStaff = t
                            }
                        ).Distinct().OrderBy(f => f.SalesStaff.Name),

                        PlantTargetMarketSegments =
                        (
                            from f in context.CompanyUsers
                            join s in context.MarketSegments on f.CompanyId equals s.CompanyId
                            from t in
                                (from t in context.PlantBudgetMarketSegments where s.MarketSegmentId == t.MarketSegmentId && t.PlantBudgetId == b.PlantBudgetId select t).DefaultIfEmpty()
                            where f.UserId == userId && s.Active == true
                            select new
                            {
                                MarketSegment = s,
                                PlantBudgetMarketSegment = t
                            }
                        ).Distinct().OrderBy(f => f.MarketSegment.Name)
                    }
                ).ToList();

                // Create the list
                List<SIPlantTargetRow> rows = new List<SIPlantTargetRow>();

                //---------------------------------
                // Plant Names
                //---------------------------------

                // Add all of the plants
                foreach (var result in results)
                {
                    // Create the item
                    SIPlantTargetRow row = new SIPlantTargetRow();

                    // Add the item
                    row.Columns["Plants"] = new SIPlantTargetColumn
                    {
                        ColumnType = SIPlantTargetColumnType.Plant,
                        PlantBudgetID = result.PlantBudget == null ? int.MinValue : result.PlantBudget.PlantBudgetId,
                        PlantID = result.Plant.PlantId,
                        BudgetDate = monthBegin,
                        Value = result.Plant.Name
                    };

                    // Add the item
                    row.Columns["Budget"] = new SIPlantTargetColumn
                    {
                        ColumnType = SIPlantTargetColumnType.Budget,
                        PlantBudgetID = result.PlantBudget == null ? int.MinValue : result.PlantBudget.PlantBudgetId,
                        PlantID = result.Plant.PlantId,
                        BudgetDate = monthBegin,
                        Value = result.PlantBudget == null ? "0" : result.PlantBudget.Budget.GetValueOrDefault().ToString("N0"),
                    };

                    // Add the item
                    row.Columns["Trucks"] = new SIPlantTargetColumn
                    {
                        ColumnType = SIPlantTargetColumnType.Trucks,
                        PlantBudgetID = result.PlantBudget == null ? int.MinValue : result.PlantBudget.PlantBudgetId,
                        PlantID = result.Plant.PlantId,
                        BudgetDate = monthBegin,
                        Value = result.PlantBudget == null ? "0" : result.PlantBudget.Trucks.GetValueOrDefault().ToString("N0")
                    };

                    // Pivot the sales staff
                    foreach (var salesStaff in result.PlantTargetSalesStaffs)
                    {
                        // Add the item
                        row.Columns[salesStaff.SalesStaff.Name] = new SIPlantTargetColumn
                        {
                            Tag = salesStaff.SalesStaff.Name,
                            ColumnType = SIPlantTargetColumnType.SalesStaff,
                            PlantBudgetID = result.PlantBudget == null ? int.MinValue : result.PlantBudget.PlantBudgetId,
                            PlantID = result.Plant.PlantId,
                            BudgetDate = monthBegin,
                            Value = salesStaff.PlantBudgetSalesStaff == null ? "0" : string.Format("{0}", salesStaff.PlantBudgetSalesStaff.Percentage),
                            SalesStaffID = salesStaff.SalesStaff.SalesStaffId,
                            PlantBudgetSalesStaffID = salesStaff.PlantBudgetSalesStaff == null ? int.MinValue : salesStaff.PlantBudgetSalesStaff.PlantBudgetSalesStaffId
                        };
                    }

                    // Pivot the market segments
                    foreach (var marketSegment in result.PlantTargetMarketSegments)
                    {
                        // Add the item
                        row.Columns[marketSegment.MarketSegment.Name] = new SIPlantTargetColumn
                        {
                            Tag = marketSegment.MarketSegment.Name,
                            ColumnType = SIPlantTargetColumnType.MarketSegment,
                            PlantBudgetID = result.PlantBudget == null ? int.MinValue : result.PlantBudget.PlantBudgetId,
                            PlantID = result.Plant.PlantId,
                            BudgetDate = monthBegin,
                            Value = marketSegment.PlantBudgetMarketSegment == null ? "0" : string.Format("{0}", marketSegment.PlantBudgetMarketSegment.Percentage),
                            MarketSegmentID = marketSegment.MarketSegment.MarketSegmentId,
                            PlantBudgetMarketSegmentID = marketSegment.PlantBudgetMarketSegment == null ? int.MinValue : marketSegment.PlantBudgetMarketSegment.PlantBudgetMarketSegmentId,
                        };
                    }

                    // Add the values to this row
                    rows.Add(row);
                }

                // Return the plant targets
                return rows;
            }
        }
        public static List<PlantBudget> GetPlantBudgets(int[] districtIds, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                var query = from p in context.PlantBudgets
                            join d in context.Districts on p.Plant.DistrictId equals d.DistrictId
                            where districtIds.Contains(d.DistrictId)
                            && p.Plant.Active == true
                            && p.BudgetDate >= startDate
                            && p.BudgetDate <= endDate
                            select p;
                return query.ToList();
            }
        }
        public static List<PlantBudget> GetPlantBudgets(int plantId, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                var query = from p in context.PlantBudgets
                            where p.PlantId == plantId
                            && p.BudgetDate >= startDate
                            && p.BudgetDate <= endDate
                            select p;
                return query.ToList();
            }
        }
        public static PlantBudget GetPlantBudgets(string plantCode, DateTime reporDate, Plant plant = null)
        {
            using (var context = GetContext())
            {
                if (plant != null)
                {
                    var query = from p in plant.PlantBudgets
                                where p.Plant.DispatchId == plantCode
                                      && p.BudgetDate.Month == reporDate.Month
                                      && p.BudgetDate.Year == reporDate.Year
                                select p;
                    return query.FirstOrDefault();
                }
                else
                {
                    var query = from p in context.PlantBudgets
                                where p.Plant.DispatchId == plantCode
                                      && p.BudgetDate.Month == reporDate.Month
                                      && p.BudgetDate.Year == reporDate.Year
                                select p;
                    return query.FirstOrDefault();
                }

            }
        }
        public static PlantBudget GetPlantBudgets(int plantId, DateTime reportDate)
        {
            using (var context = GetContext())
            {
                var query = from p in context.PlantBudgets
                            where p.PlantId == plantId
                                  && p.BudgetDate.Month == reportDate.Month
                                  && p.BudgetDate.Year == reportDate.Year
                            select p;
                return query.FirstOrDefault();
            }
        }

        public static PlantBudget GetPlantBudget(int plantId, int month, int year)
        {
            using (var context = GetContext())
            {
                var query = from p in context.PlantBudgets
                            where p.PlantId == plantId
                                  && p.BudgetDate.Month == month
                                  && p.BudgetDate.Year == year
                            select p;
                return query.FirstOrDefault();
            }
        }
        public static List<PlantFinancialBudget> GetPlantFinancialBudgets(int plantId, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                var query = from p in context.PlantFinancialBudgets
                            where p.Plant.PlantId == plantId
                            && p.Plant.Active == true
                            && (p.Month >= startDate.Month && p.Year >= startDate.Year)
                            && (p.Month <= endDate.Month && p.Year <= startDate.Year)
                            select p;
                return query.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
            }
        }
        public static PlantFinancialBudget GetPlantFinancialBudgets(int plantId, int month, int year)
        {
            using (var context = GetContext())
            {
                var query = from p in context.PlantFinancialBudgets
                            where p.Plant.PlantId == plantId
                            && p.Plant.Active == true
                            && (p.Month == month && p.Year == year)
                            select p;
                return query.FirstOrDefault();
            }
        }
        public static PlantFinancialBudget GetPlantFinancialBudgets(string plantCode, DateTime reportDate, Plant plant = null)
        {
            using (var context = GetContext())
            {
                if (plant != null)
                {
                    var query = from p in plant.PlantFinancialBudgets
                                where p.Plant.DispatchId == plantCode
                                && (p.Month == reportDate.Month && p.Year == reportDate.Year)
                                select p;
                    return query.FirstOrDefault();
                }
                else
                {
                    var query = from p in context.PlantFinancialBudgets
                                where p.Plant.DispatchId == plantCode
                                && (p.Month == reportDate.Month && p.Year == reportDate.Year)
                                select p;
                    return query.FirstOrDefault();
                }

            }
        }
        public static void UpdateMonthlyProductivityTarget(int plantId, int year, int month, string metric, float value)
        {
            using (var context = GetContext())
            {
                DateTime budgetDate = DateUtils.GetFirstOf(month + 1, year);
                PlantBudget b = new PlantBudget();
                var query = context.PlantBudgets.Where(x => x.PlantId == plantId).Where(x => x.BudgetDate == budgetDate);
                if (query.Count() == 0)
                {
                    b.PlantId = plantId;
                    b.BudgetDate = budgetDate;
                    context.PlantBudgets.InsertOnSubmit(b);
                }
                else
                {
                    b = query.First();
                }

                if (metric == "Ticketing") { b.Ticketing = value; }
                if (metric == "Loading") { b.Loading = value; }
                if (metric == "Tempering") { b.Tempering = value; }
                if (metric == "ToJob") { b.ToJob = value; }
                if (metric == "Wait") { b.Wait = value; }
                if (metric == "Unload") { b.Unload = value; }
                if (metric == "Wash") { b.Wash = value; }
                if (metric == "FromJob") { b.FromJob = value; }
                if (metric == "CYDHr") { b.CydHr = value; }
                if (metric == "AvgLoad") { b.AvgLoad = value; }
                if (metric == "Volume") { b.Budget = value; }
                if (metric == "Trucks") { b.Trucks = value; }
                if (metric == "StartUp") { b.StartUp = value; }
                if (metric == "Shutdown") { b.Shutdown = value; }
                if (metric == "InYard") { b.InYard = value; }
                if (metric == "FirstLoadOnTimePercent") { b.FirstLoadOnTimePercent = value; }
                if (metric == "TrucksPercentOperable") { b.TrucksPercentOperable = value; }
                if (metric == "Accidents") { b.Accidents = value; }
                if (metric == "PlantInterruptions") { b.PlantInterruptions = value; }
                if (metric == "TrucksDown") { b.TrucksDown = value; }
                if (metric == "BatchTolerance") { b.BatchTolerance = value; }

                context.SubmitChanges();
            }
        }
        public static void UpdateMonthlyFinancialTarget(int plantId, int year, int month, string metric, double value)
        {
            using (var context = GetContext())
            {
                PlantFinancialBudget b = new PlantFinancialBudget();
                var query = context.PlantFinancialBudgets.Where(x => x.PlantId == plantId).Where(x => x.Month == month).Where(x => x.Year == year);
                if (query.Count() == 0)
                {
                    b.PlantId = plantId;
                    b.Month = month;
                    b.Year = year;
                    context.PlantFinancialBudgets.InsertOnSubmit(b);
                }
                else
                {
                    b = query.First();
                }

                if (metric == "Revenue") { b.Revenue = Convert.ToDecimal(value); }
                if (metric == "Material Cost") { b.MaterialCost = Convert.ToDecimal(value); }
                if (metric == "Delivery Variable") { b.DeliveryVariable = value; }
                if (metric == "Plant Variable") { b.PlantVariable = value; }
                if (metric == "Delivery Fixed") { b.DeliveryFixed = value; }
                if (metric == "Plant Fixed") { b.PlantFixed = value; }
                if (metric == "SG&A") { b.SGA = value; }
                if (metric == "Profit") { b.Profit = Convert.ToDecimal(value); }

                context.SubmitChanges();
            }
        }
        public static string ScrubMonthlyFinancialBudget(string budgetYear, string budgetMonth, string plantName, string revenue, string materialCost, string deliveryVariable, string plantVariable, string deliveryFixed, string plantFixed, string sga, string profit)
        {
            using (var context = GetContext())
            {
                int year = 0;
                int month = 0;
                try
                {
                    year = Int32.Parse(budgetYear);
                }
                catch (Exception ex)
                {
                    return "The Budget year is not in correct format";
                }
                try
                {
                    month = Int32.Parse(budgetMonth);
                }
                catch (Exception ex)
                {
                    return "The Budget month is not in correct format";
                }
                if (year <= 1970)
                {
                    return "The Budget year is invalid";
                }
                if (month < 1 || month > 12)
                {
                    return "The Budget month is invalid";
                }
                if (plantName == null || plantName.Trim().Equals(""))
                {
                    return "The plant name cannot be empty";
                }
                Plant p = context.Plants.FirstOrDefault(x => x.Name == plantName);
                if (p == null)
                {
                    return "The specified plant does not exist";
                }

                PlantFinancialBudget budget = context.PlantFinancialBudgets.Where(x => x.PlantId == p.PlantId).Where(x => x.Year == year).Where(x => x.Month == month).FirstOrDefault();
                if (budget == null)
                {
                    budget = new PlantFinancialBudget();
                    budget.PlantId = p.PlantId;
                    budget.Year = year;
                    budget.Month = month;
                    context.PlantFinancialBudgets.InsertOnSubmit(budget);
                }

                try { budget.Revenue = Decimal.Parse(revenue); }
                catch (Exception ex) { budget.Revenue = null; }
                try { budget.MaterialCost = Decimal.Parse(materialCost); }
                catch (Exception ex) { budget.MaterialCost = null; }
                try { budget.DeliveryFixed = Double.Parse(deliveryFixed); }
                catch (Exception ex) { budget.DeliveryFixed = null; }
                try { budget.DeliveryVariable = Double.Parse(deliveryVariable); }
                catch (Exception ex) { budget.DeliveryVariable = null; }
                try { budget.PlantVariable = Double.Parse(plantVariable); }
                catch (Exception ex) { budget.PlantVariable = null; }
                try { budget.PlantFixed = Double.Parse(plantFixed); }
                catch (Exception ex) { budget.PlantFixed = null; }
                try { budget.SGA = Double.Parse(sga); }
                catch (Exception ex) { budget.SGA = null; }
                try { budget.Profit = Decimal.Parse(profit); }
                catch (Exception ex) { budget.Profit = null; }

                context.SubmitChanges();
                return null;
            }
        }
        #endregion

        #region MetricIndicators
        public static List<MetricIndicatorAllowance> GetAllMetricIndicator()
        {
            List<MetricIndicatorAllowance> merticIndicatorList = new List<MetricIndicatorAllowance>();
            using (var context = GetContext())
            {
                merticIndicatorList = context.MetricIndicatorAllowances.ToList();
            }
            return merticIndicatorList;
        }

        public static MetricIndicatorAllowance GetMetricIndicator(string metricName)
        {
            using (var context = GetContext())
            {
                if (metricName == "Variable")
                {
                    var plantVariable = context.MetricIndicatorAllowances.Where(x => x.Metric == "Plant Variable").FirstOrDefault();
                    var deliveryVariable = context.MetricIndicatorAllowances.Where(x => x.Metric == "Delivery Variable").FirstOrDefault();
                    if (plantVariable != null || deliveryVariable != null)
                    {
                        var variable = new MetricIndicatorAllowance();
                        variable.Ok = plantVariable.Ok + deliveryVariable.Ok;
                        variable.Caution = plantVariable.Caution + deliveryVariable.Caution;
                        variable.LessIsBetter = true;
                        return variable;
                    }
                }
                if (metricName == "OutGate")
                {
                    var ticketing = context.MetricIndicatorAllowances.Where(x => x.Metric == "Ticketing").FirstOrDefault();
                    var tempering = context.MetricIndicatorAllowances.Where(x => x.Metric == "Tempering").FirstOrDefault();
                    var loading = context.MetricIndicatorAllowances.Where(x => x.Metric == "Loading").FirstOrDefault();
                    if (ticketing != null && tempering != null && loading != null)
                    {
                        var variable = new MetricIndicatorAllowance();
                        variable.Ok = ticketing.Ok + tempering.Ok + loading.Ok;
                        variable.Caution = ticketing.Caution + tempering.Caution + loading.Caution;
                        variable.LessIsBetter = true;
                        return variable;
                    }
                }
                if (metricName == "Fixed")
                {
                    var plantFixed = context.MetricIndicatorAllowances.Where(x => x.Metric == "Plant Fixed").FirstOrDefault();
                    var deliveryFixed = context.MetricIndicatorAllowances.Where(x => x.Metric == "Delivery Fixed").FirstOrDefault();
                    if (plantFixed != null || deliveryFixed != null)
                    {
                        var fixedMetric = new MetricIndicatorAllowance();
                        fixedMetric.Ok = plantFixed.Ok + deliveryFixed.Ok;
                        fixedMetric.Caution = plantFixed.Caution + deliveryFixed.Caution;
                        fixedMetric.LessIsBetter = true;
                        return fixedMetric;
                    }
                }
                return context.MetricIndicatorAllowances.Where(x => x.Metric == metricName).FirstOrDefault();
            }
        }
        public static MetricIndicatorAllowance GetMetricIndicator(string metricName, List<MetricIndicatorAllowance> metricIndicator)
        {
            using (var context = GetContext())
            {
                if (metricName == "Variable")
                {
                    var plantVariable = metricIndicator.Where(x => x.Metric == "Plant Variable").FirstOrDefault();
                    var deliveryVariable = metricIndicator.Where(x => x.Metric == "Delivery Variable").FirstOrDefault();
                    if (plantVariable != null || deliveryVariable != null)
                    {
                        var variable = new MetricIndicatorAllowance();
                        variable.Ok = plantVariable.Ok + deliveryVariable.Ok;
                        variable.Caution = plantVariable.Caution + deliveryVariable.Caution;
                        variable.LessIsBetter = true;
                        return variable;
                    }
                }
                if (metricName == "OutGate")
                {
                    var ticketing = metricIndicator.Where(x => x.Metric == "Ticketing").FirstOrDefault();
                    var tempering = metricIndicator.Where(x => x.Metric == "Tempering").FirstOrDefault();
                    var loading = metricIndicator.Where(x => x.Metric == "Loading").FirstOrDefault();
                    if (ticketing != null && tempering != null && loading != null)
                    {
                        var variable = new MetricIndicatorAllowance();
                        variable.Ok = ticketing.Ok + tempering.Ok + loading.Ok;
                        variable.Caution = ticketing.Caution + tempering.Caution + loading.Caution;
                        variable.LessIsBetter = true;
                        return variable;
                    }
                }
                if (metricName == "Fixed")
                {
                    var plantFixed = metricIndicator.Where(x => x.Metric == "Plant Fixed").FirstOrDefault();
                    var deliveryFixed = metricIndicator.Where(x => x.Metric == "Delivery Fixed").FirstOrDefault();
                    if (plantFixed != null || deliveryFixed != null)
                    {
                        var fixedMetric = new MetricIndicatorAllowance();
                        fixedMetric.Ok = plantFixed.Ok + deliveryFixed.Ok;
                        fixedMetric.Caution = plantFixed.Caution + deliveryFixed.Caution;
                        fixedMetric.LessIsBetter = true;
                        return fixedMetric;
                    }
                }
                return metricIndicator.Where(x => x.Metric == metricName).FirstOrDefault();
            }
        }
        public static void UpdateMetricIndicator(string metricName, float value, string attribute)
        {
            using (var context = GetContext())
            {
                MetricIndicatorAllowance obj = context.MetricIndicatorAllowances.Where(x => x.Metric == metricName).FirstOrDefault();
                if (obj == null)
                {
                    obj = new MetricIndicatorAllowance();
                    obj.Metric = metricName;
                    context.MetricIndicatorAllowances.InsertOnSubmit(obj);
                }

                if (attribute == "OK")
                {
                    obj.Ok = value;
                }
                if (attribute == "CAUTION")
                {
                    obj.Caution = value;
                }
                if (attribute == "LESS_IS_BETTER")
                {
                    obj.LessIsBetter = value == 1;
                }

                context.SubmitChanges();

            }
        }
        public static TargetIndicatorAllowance GetTargetIndicator(string metricName)
        {
            using (var context = GetContext())
            {
                return context.TargetIndicatorAllowances.Where(x => x.Metric == metricName).FirstOrDefault();
            }
        }
        public static void UpdateTargetIndicator(string metricName, float value, string attribute)
        {
            using (var context = GetContext())
            {
                TargetIndicatorAllowance obj = context.TargetIndicatorAllowances.Where(x => x.Metric == metricName).FirstOrDefault();
                if (obj == null)
                {
                    obj = new TargetIndicatorAllowance();
                    obj.Metric = metricName;
                    context.TargetIndicatorAllowances.InsertOnSubmit(obj);
                }

                if (attribute == "TARGET")
                {
                    obj.Target = value;
                }
                if (attribute == "OK")
                {
                    obj.Ok = value;
                }
                if (attribute == "CAUTION")
                {
                    obj.Caution = value;
                }
                if (attribute == "LESS_IS_BETTER")
                {
                    obj.LessIsBetter = value == 1;
                }

                context.SubmitChanges();

            }
        }
        #endregion

        #region Customer Productivity
        public static string ScrubCustomerProductivity(string ticketDate, string orderCode, string customerCode, string ticketCode, string isFOB, string plantCode, string truckCode, string quantity, string ticketing, string loadTemper, string toJob, string wait, string unload, string wash, string fromJob, string segmentId, out CustomerProductivity newRecord)
        {
            using (var context = GetContext())
            {
                newRecord = new CustomerProductivity();
                try { newRecord.ReportDate = DateTime.ParseExact(ticketDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); }
                catch (Exception) { return "The Ticket Date is not in the correct format, empty, or is un-parseable. It needs to be yyyy-MM-dd"; }

                // Indentation on assignment lines is for readability only. There is no nesting in the if block.
                if (orderCode == null || orderCode.Trim().Equals("")) { return "The order code cannot be empty"; }
                newRecord.OrderCode = orderCode;
                if (customerCode == null || customerCode.Trim().Equals("")) { return "The customer code cannot be empty"; }
                newRecord.CustomerNumber = customerCode;
                if (ticketCode == null || ticketCode.Trim().Equals("")) { return "The ticket code cannot be empty"; }
                newRecord.TicketCode = ticketCode;
                if (isFOB == null || isFOB.Trim().Equals("")) { return "The FOB cannot be empty"; }
                newRecord.IsFOB = isFOB.ToLower() == "true";
                if (plantCode == null || plantCode.Trim().Equals("")) { return "The plant code cannot be empty"; }
                newRecord.PlantDispatchCode = plantCode;
                if (segmentId == null || segmentId.Trim().Equals("")) { return "The Segment ID cannot be empty"; }
                newRecord.SegmentId = segmentId;
                // No check for the truck code.
                newRecord.TruckCode = truckCode;

                Customer c = context.Customers.FirstOrDefault(x => x.CustomerNumber == customerCode);
                if (c == null) { return "The specified customer does not exist. Check the customer code again."; } else { newRecord.CustomerName = c.Name; }


                Plant p = context.Plants.FirstOrDefault(x => x.DispatchId == plantCode);
                if (p == null) { return "The specified plant does not exist. Check the plant code again."; }
                newRecord.PlantDispatchCode = plantCode;

                MarketSegment m = context.MarketSegments.FirstOrDefault(x => x.DispatchId == segmentId);
                if (m == null) { return "The specified market segment does not exist. Check the segment id again."; }
                newRecord.SegmentId = segmentId;

                try { newRecord.Quantity = Double.Parse(quantity); }
                catch (Exception) { return "Quantity cannot be empty and must be a number"; }
                try { newRecord.Ticketing = Double.Parse(ticketing); }
                catch (Exception) { return "Ticketing Mins cannot be empty and must be a number"; }
                try { newRecord.Wait = Double.Parse(wait); }
                catch (Exception) { return "Wait Mins cannot be empty and must be a number"; }
                try { newRecord.LoadTemper = Double.Parse(loadTemper); }
                catch (Exception) { return "Load and Temper Mins cannot be empty and must be a number"; }
                try { newRecord.ToJob = Double.Parse(toJob); }
                catch (Exception) { return "To Job Mins cannot be empty and must be a number"; }
                try { newRecord.Wait = Double.Parse(wait); }
                catch (Exception) { return "Wait Mins cannot be empty and must be a number"; }
                try { newRecord.Unload = Double.Parse(unload); }
                catch (Exception) { return "Pour Mins cannot be empty and must be a number"; }
                try { newRecord.Wash = Double.Parse(wash); }
                catch (Exception) { return "Wash Mins cannot be empty and must be a number"; }
                try { newRecord.FromJob = Double.Parse(fromJob); }
                catch (Exception) { return "To Plant Mins cannot be empty and must be a number"; }

                return null;
            }
        }
        public static void RefreshCustomerProductivities(List<CustomerProductivity> newData)
        {
            using (var context = GetContext())
            {
                var toDelete = context.CustomerProductivities.Where(x => x.ReportDate >= newData.Min(y => y.ReportDate)).Where(x => x.ReportDate <= newData.Max(y => y.ReportDate)).ToList();
                if (toDelete.Count() > 0)
                {
                    context.CustomerProductivities.DeleteAllOnSubmit(toDelete);
                }
                context.SubmitChanges();
                foreach (var data in newData)
                {
                    context.CustomerProductivities.InsertOnSubmit(data);
                }
                context.SubmitChanges();
            }
        }
        public static List<CustomerProductivity> GetCustomerProductivityData(string sortBy, string sortDirection, int pageNum, int rowCount, DateTime startDate, DateTime endDate, out int recordCount)
        {
            using (var context = GetContext())
            {
                var query = context.CustomerProductivities.AsEnumerable();
                query = query.Where(x => x.ReportDate >= startDate).Where(x => x.ReportDate <= endDate);
                if (sortDirection == "asc")
                {
                    switch (sortBy)
                    {
                        case "ReportDate":
                            query = query.OrderBy(x => x.ReportDate);
                            break;
                        case "OrderCode":
                            query = query.OrderBy(x => x.OrderCode);
                            break;
                        case "CustomerName":
                            query = query.OrderBy(x => x.CustomerName);
                            break;
                        case "CustomerNumber":
                            query = query.OrderBy(x => x.CustomerNumber);
                            break;
                        case "TicketCode":
                            query = query.OrderBy(x => x.TicketCode);
                            break;
                        case "IsFOB":
                            query = query.OrderBy(x => x.IsFOB);
                            break;
                        case "PlantDispatchCode":
                            query = query.OrderBy(x => x.PlantDispatchCode);
                            break;
                        case "TruckCode":
                            query = query.OrderBy(x => x.TruckCode);
                            break;
                        case "Quantity":
                            query = query.OrderBy(x => x.Quantity);
                            break;
                        case "Ticketing":
                            query = query.OrderBy(x => x.Ticketing);
                            break;
                        case "LoadTemper":
                            query = query.OrderBy(x => x.LoadTemper);
                            break;
                        case "ToJob":
                            query = query.OrderBy(x => x.ToJob);
                            break;
                        case "Wait":
                            query = query.OrderBy(x => x.Wait);
                            break;
                        case "Unload":
                            query = query.OrderBy(x => x.Unload);
                            break;
                        case "Wash":
                            query = query.OrderBy(x => x.Wash);
                            break;
                        case "FromJob":
                            query = query.OrderBy(x => x.FromJob);
                            break;
                        case "SegmentId":
                            query = query.OrderBy(x => x.SegmentId);
                            break;
                    }
                }
                if (sortDirection == "desc")
                {
                    switch (sortBy)
                    {
                        case "ReportDate":
                            query = query.OrderByDescending(x => x.ReportDate);
                            break;
                        case "OrderCode":
                            query = query.OrderByDescending(x => x.OrderCode);
                            break;
                        case "CustomerName":
                            query = query.OrderByDescending(x => x.CustomerName);
                            break;
                        case "CustomerNumber":
                            query = query.OrderByDescending(x => x.CustomerNumber);
                            break;
                        case "TicketCode":
                            query = query.OrderByDescending(x => x.TicketCode);
                            break;
                        case "IsFOB":
                            query = query.OrderByDescending(x => x.IsFOB);
                            break;
                        case "PlantDispatchCode":
                            query = query.OrderByDescending(x => x.PlantDispatchCode);
                            break;
                        case "TruckCode":
                            query = query.OrderByDescending(x => x.TruckCode);
                            break;
                        case "Quantity":
                            query = query.OrderByDescending(x => x.Quantity);
                            break;
                        case "Ticketing":
                            query = query.OrderByDescending(x => x.Ticketing);
                            break;
                        case "LoadTemper":
                            query = query.OrderByDescending(x => x.LoadTemper);
                            break;
                        case "ToJob":
                            query = query.OrderByDescending(x => x.ToJob);
                            break;
                        case "Wait":
                            query = query.OrderByDescending(x => x.Wait);
                            break;
                        case "Unload":
                            query = query.OrderByDescending(x => x.Unload);
                            break;
                        case "Wash":
                            query = query.OrderByDescending(x => x.Wash);
                            break;
                        case "FromJob":
                            query = query.OrderByDescending(x => x.FromJob);
                            break;
                        case "SegmentId":
                            query = query.OrderByDescending(x => x.SegmentId);
                            break;
                    }
                }
                recordCount = query.Count();
                query = query.Skip((pageNum - 1) * rowCount).Take(rowCount);
                return query.ToList();
            }
        }
        public static List<CustomerProductivity> GetCustomerProductivityReport(string[] customerIds, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                return context.CustomerProductivities
                    .Where(x => customerIds.Contains(x.CustomerNumber))
                    .Where(x => x.ReportDate >= startDate)
                    .Where(x => x.ReportDate <= endDate).ToList();
            }
        }
        public static List<CustomerProductivity> GetDistrictProductivityReport(int[] districtIds, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                var plantIds = context.Plants.Where(x => districtIds.Contains(x.DistrictId)).Where(x => x.DispatchId != null).Select(x => x.DispatchId).ToList();
                return context.CustomerProductivities
                    .Where(x => plantIds.Contains(x.PlantDispatchCode))
                    .Where(x => x.ReportDate >= startDate)
                    .Where(x => x.ReportDate <= endDate).ToList();
            }
        }
        #endregion

        #region Customer Profitability
        public static string ScrubCustomerProfitablity(string reportDate, string customerCode, string customerName, string segmentId, string revenue, string materialCost, string plantCode, out CustomerProfitability newRecord)
        {
            using (var context = GetContext())
            {
                newRecord = new CustomerProfitability();
                try { newRecord.ReportDate = DateTime.ParseExact(reportDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); }
                catch (Exception) { return "The Date is not in the correct format, empty, or is un-parseable. It needs to be yyyy-MM-dd"; }

                // Indentation on assignment lines is for readability only. There is no nesting in the if block.


                Customer c = context.Customers.FirstOrDefault(x => x.CustomerNumber == customerCode);
                if (c == null) { return "The specified customer does not exist. Check the customer code again."; } else { newRecord.CustomerName = customerName; newRecord.CustomerNumber = customerCode; }

                MarketSegment m = context.MarketSegments.FirstOrDefault(x => x.DispatchId == segmentId);
                if (m == null) { return "The specified market segment does not exist. Check the segment code again."; }
                newRecord.SegmentId = segmentId;

                Plant p = context.Plants.FirstOrDefault(x => x.DispatchId == plantCode);
                if (p == null) { return "The specified plant does not exist. Check the plant code and try again"; }
                newRecord.PlantCode = plantCode;

                try { newRecord.Revenue = Decimal.Parse(revenue); }
                catch (Exception) { return "Quantity cannot be empty and must be a number"; }
                try { newRecord.MaterialCost = Decimal.Parse(materialCost); }
                catch (Exception) { return "Ticketing Mins cannot be empty and must be a number"; }

                return null;
            }
        }
        public static void RefreshCustomerProfitabilities(List<CustomerProfitability> newData)
        {
            using (var context = GetContext())
            {
                var toDelete = context.CustomerProfitabilities.Where(x => x.ReportDate >= newData.Min(y => y.ReportDate)).Where(x => x.ReportDate <= newData.Max(y => y.ReportDate)).ToList();
                if (toDelete.Count() > 0)
                {
                    context.CustomerProfitabilities.DeleteAllOnSubmit(toDelete);
                }
                context.SubmitChanges();
                foreach (var data in newData)
                {
                    context.CustomerProfitabilities.InsertOnSubmit(data);
                }
                context.SubmitChanges();
            }
        }
        public static List<CustomerProfitability> GetCustomerProfitabilityData(string sortBy, string sortDirection, int pageNum, int rowCount, DateTime startDate, DateTime endDate, out int recordCount)
        {
            using (var context = GetContext())
            {
                var query = context.CustomerProfitabilities.AsEnumerable();
                query = query.Where(x => x.ReportDate >= startDate).Where(x => x.ReportDate <= endDate);
                if (sortDirection == "asc")
                {
                    switch (sortBy)
                    {
                        case "ReportDate":
                            query = query.OrderBy(x => x.ReportDate);
                            break;
                        case "CustomerName":
                            query = query.OrderBy(x => x.CustomerName);
                            break;
                        case "CustomerNumber":
                            query = query.OrderBy(x => x.CustomerNumber);
                            break;
                        case "SegmentId":
                            query = query.OrderBy(x => x.SegmentId);
                            break;
                        case "PlantCode":
                            query = query.OrderBy(x => x.PlantCode);
                            break;
                        case "Revenue":
                            query = query.OrderBy(x => x.Revenue);
                            break;
                        case "MaterialCost":
                            query = query.OrderBy(x => x.MaterialCost);
                            break;
                    }
                }
                else
                {
                    switch (sortBy)
                    {
                        case "ReportDate":
                            query = query.OrderByDescending(x => x.ReportDate);
                            break;
                        case "CustomerName":
                            query = query.OrderByDescending(x => x.CustomerName);
                            break;
                        case "CustomerNumber":
                            query = query.OrderByDescending(x => x.CustomerNumber);
                            break;
                        case "SegmentId":
                            query = query.OrderByDescending(x => x.SegmentId);
                            break;
                        case "PlantCode":
                            query = query.OrderByDescending(x => x.PlantCode);
                            break;
                        case "Revenue":
                            query = query.OrderByDescending(x => x.Revenue);
                            break;
                        case "MaterialCost":
                            query = query.OrderByDescending(x => x.MaterialCost);
                            break;
                    }
                }
                recordCount = query.Count();
                query = query.Skip((pageNum - 1) * rowCount).Take(rowCount);
                return query.ToList();
            }
        }
        public static List<CustomerProfitability> GetCustomerProfitabilityReport(string[] customerNums, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                return context.CustomerProfitabilities
                    .Where(x => customerNums.Contains(x.CustomerNumber))
                    .Where(x => x.ReportDate >= startDate)
                    .Where(x => x.ReportDate <= endDate).ToList();
            }
        }
        public static List<CustomerProfitability> GetDistrictProfitabilityReport(int[] districtIds, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                var plantIds = context.Plants.Where(x => districtIds.Contains(x.DistrictId)).Where(x => x.DispatchId != null).Select(x => x.DispatchId).ToList();
                return context.CustomerProfitabilities
                    .Where(x => plantIds.Contains(x.PlantCode))
                    .Where(x => x.ReportDate >= startDate)
                    .Where(x => x.ReportDate <= endDate).ToList();
            }
        }
        #endregion

        #region Customer Order Changes
        public static string ScrubCustomerOrderChanges(string reportDate, string orderCode, string customerCode, string productId, string volume, out CustomerOrderChange newRecord)
        {
            using (var context = GetContext())
            {
                newRecord = new CustomerOrderChange();
                try { newRecord.ReportDate = DateTime.ParseExact(reportDate, "yyyy-MM-dd H:m", CultureInfo.InvariantCulture); }
                catch (Exception) { return "The Date Time is not in the correct format, empty, or is un-parseable. It needs to be yyyy-MM-dd h:m"; }

                // Indentation on assignment lines is for readability only. There is no nesting in the if block.
                if (orderCode == null || orderCode.Trim().Equals("")) { return "The order Id cannot be empty"; }
                newRecord.OrderId = orderCode;
                if (productId == null || productId.Trim().Equals("")) { return "The product Id cannot be empty"; }
                newRecord.ConcreteProductId = productId;

                Customer c = context.Customers.FirstOrDefault(x => x.CustomerNumber == customerCode);
                if (c == null) { return "The specified customer does not exist. Check the customer code again."; } else { newRecord.CustomerName = c.Name; newRecord.CustomerNumber = customerCode; }

                try { newRecord.ConcreteProductVolume = Double.Parse(volume); }
                catch (Exception) { return "Concrete Product Volume cannot be empty and must be a number"; }

                return null;
            }
        }
        public static void RefreshCustomerOrderChanges(List<CustomerOrderChange> newData)
        {
            using (var context = GetContext())
            {
                var toDelete = context.CustomerOrderChanges.Where(x => x.ReportDate >= newData.Min(y => y.ReportDate)).Where(x => x.ReportDate <= newData.Max(y => y.ReportDate)).ToList();
                if (toDelete.Count() > 0)
                {
                    context.CustomerOrderChanges.DeleteAllOnSubmit(toDelete);
                }
                context.SubmitChanges();

                var orderIds = newData.Select(x => x.OrderId).Distinct();
                foreach (string orderId in orderIds)
                {
                    var orderHistory = newData.Where(x => x.OrderId == orderId).OrderBy(x => x.ReportDate);
                    double previousVolume = orderHistory.First().ConcreteProductVolume;
                    foreach (var order in orderHistory)
                    {
                        order.VolumeChange = order.ConcreteProductVolume - previousVolume;
                        previousVolume = order.ConcreteProductVolume;
                    }
                }

                foreach (var data in newData)
                {
                    context.CustomerOrderChanges.InsertOnSubmit(data);
                }

                context.SubmitChanges();
            }
        }
        public static List<CustomerOrderChange> GetCustomerOrderChangeData(string sortBy, string sortDirection, int pageNum, int rowCount, DateTime startDate, DateTime endDate, out int recordCount)
        {
            using (var context = GetContext())
            {
                var query = context.CustomerOrderChanges.AsEnumerable();
                query = query.Where(x => x.ReportDate >= startDate).Where(x => x.ReportDate <= endDate);
                if (sortDirection == "asc")
                {
                    switch (sortBy)
                    {
                        case "ReportDate":
                            query = query.OrderBy(x => x.ReportDate);
                            break;
                        case "CustomerNumber":
                            query = query.OrderBy(x => x.CustomerNumber);
                            break;
                        case "OrderId":
                            query = query.OrderBy(x => x.OrderId);
                            break;
                        case "ConcreteProductId":
                            query = query.OrderBy(x => x.ConcreteProductId);
                            break;
                        case "ConcreteProductVolume":
                            query = query.OrderBy(x => x.ConcreteProductVolume);
                            break;
                    }
                }
                else
                {
                    switch (sortBy)
                    {
                        case "ReportDate":
                            query = query.OrderByDescending(x => x.ReportDate);
                            break;
                        case "CustomerNumber":
                            query = query.OrderByDescending(x => x.CustomerNumber);
                            break;
                        case "OrderId":
                            query = query.OrderByDescending(x => x.OrderId);
                            break;
                        case "ConcreteProductId":
                            query = query.OrderByDescending(x => x.ConcreteProductId);
                            break;
                        case "ConcreteProductVolume":
                            query = query.OrderByDescending(x => x.ConcreteProductVolume);
                            break;
                    }
                }
                recordCount = query.Count();
                query = query.Skip((pageNum - 1) * rowCount).Take(rowCount);
                return query.ToList();
            }
        }
        public static List<CustomerOrderChange> GetCustomerOrderChangeReport(string[] customerIds, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                return context.CustomerOrderChanges
                    .Where(x => customerIds.Contains(x.CustomerNumber))
                    .Where(x => x.ReportDate >= startDate).Where(x => x.ReportDate <= endDate).ToList();
            }
        }
        #endregion

        #region Customer Aging
        public static string ScrubCustomerAging(string reportDate, string customerCode, string customerName, string balance, string current, string over1mo, string over2mo, string over3mo, string over4mo, string dso, out CustomerAging newRecord)
        {
            using (var context = GetContext())
            {
                newRecord = new CustomerAging();
                try { newRecord.ReportDate = DateTime.ParseExact(reportDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); }
                catch (Exception) { return "The As of Date is not in the correct format, empty, or is un-parseable. It needs to be yyyy-MM-dd"; }


                Customer c = context.Customers.FirstOrDefault(x => x.CustomerNumber == customerCode);
                if (c == null) { return "The specified customer does not exist. Check the customer code again."; } else { newRecord.CustomerName = customerName; newRecord.CustomerNumber = customerCode; }

                try { newRecord.Balance = Decimal.Parse(balance); }
                catch (Exception) { return "Balance cannot be empty and must be a number"; }
                try { newRecord.CurrentAmount = Decimal.Parse(current); }
                catch (Exception) { return "Current cannot be empty and must be a number"; }
                try { newRecord.Over1Month = Decimal.Parse(over1mo); }
                catch (Exception) { return "Over 1 month cannot be empty and must be a number"; }
                try { newRecord.Over2Month = Decimal.Parse(over2mo); }
                catch (Exception) { return "Over 2 month cannot be empty and must be a number"; }
                try { newRecord.Over3Month = Decimal.Parse(over3mo); }
                catch (Exception) { return "Over 3 month cannot be empty and must be a number"; }
                try { newRecord.Over4Month = Decimal.Parse(over4mo); }
                catch (Exception) { return "Over 4 month cannot be empty and must be a number"; }
                try { newRecord.DSO = Decimal.Parse(dso); }
                catch (Exception) { return "DSO cannot be empty and must be a number"; }

                return null;
            }
        }
        public static void RefreshCustomerAging(List<CustomerAging> newData)
        {
            using (var context = GetContext())
            {
                var toDelete = context.CustomerAgings.Where(x => x.ReportDate >= newData.Min(y => y.ReportDate)).Where(x => x.ReportDate <= newData.Max(y => y.ReportDate)).ToList();
                if (toDelete.Count() > 0)
                {
                    context.CustomerAgings.DeleteAllOnSubmit(toDelete);
                }
                context.SubmitChanges();
                foreach (var data in newData)
                {
                    context.CustomerAgings.InsertOnSubmit(data);
                }
                context.SubmitChanges();
            }
        }
        public static List<CustomerAging> GetCustomerAgingData(string sortBy, string sortDirection, int pageNum, int rowCount, DateTime startDate, DateTime endDate, out int recordCount)
        {
            using (var context = GetContext())
            {
                var query = context.CustomerAgings.AsEnumerable();
                query = query.Where(x => x.ReportDate >= startDate).Where(x => x.ReportDate <= endDate);
                if (sortDirection == "asc")
                {
                    switch (sortBy)
                    {
                        case "ReportDate":
                            query = query.OrderBy(x => x.ReportDate);
                            break;
                        case "CustomerName":
                            query = query.OrderBy(x => x.CustomerName);
                            break;
                        case "CustomerNumber":
                            query = query.OrderBy(x => x.CustomerNumber);
                            break;
                        case "Balance":
                            query = query.OrderBy(x => x.Balance);
                            break;
                        case "CurrentAmount":
                            query = query.OrderBy(x => x.CurrentAmount);
                            break;
                        case "Over1Month":
                            query = query.OrderBy(x => x.Over1Month);
                            break;
                        case "Over2Month":
                            query = query.OrderBy(x => x.Over2Month);
                            break;
                        case "Over3Month":
                            query = query.OrderBy(x => x.Over3Month);
                            break;
                        case "Over4Month":
                            query = query.OrderBy(x => x.Over4Month);
                            break;
                        case "DSO":
                            query = query.OrderBy(x => x.DSO);
                            break;
                    }
                }
                else
                {
                    switch (sortBy)
                    {
                        case "ReportDate":
                            query = query.OrderByDescending(x => x.ReportDate);
                            break;
                        case "CustomerName":
                            query = query.OrderByDescending(x => x.CustomerName);
                            break;
                        case "CustomerNumber":
                            query = query.OrderByDescending(x => x.CustomerNumber);
                            break;
                        case "Balance":
                            query = query.OrderByDescending(x => x.Balance);
                            break;
                        case "CurrentAmount":
                            query = query.OrderByDescending(x => x.CurrentAmount);
                            break;
                        case "Over1Month":
                            query = query.OrderByDescending(x => x.Over1Month);
                            break;
                        case "Over2Month":
                            query = query.OrderByDescending(x => x.Over2Month);
                            break;
                        case "Over3Month":
                            query = query.OrderByDescending(x => x.Over3Month);
                            break;
                        case "Over4Month":
                            query = query.OrderByDescending(x => x.Over4Month);
                            break;
                        case "DSO":
                            query = query.OrderByDescending(x => x.DSO);
                            break;
                    }
                }
                recordCount = query.Count();
                query = query.Skip((pageNum - 1) * rowCount).Take(rowCount);
                return query.ToList();
            }
        }
        public static List<CustomerAging> GetCustomerAgingReport(string[] customerIds, DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                CustomerAging lastAging = context.CustomerAgings.Where(x => x.ReportDate <= endDate).Where(x => x.ReportDate >= startDate).OrderByDescending(x => x.ReportDate).FirstOrDefault();
                if (lastAging == null)
                {
                    return null;
                }
                else
                {
                    return context.CustomerAgings
                        .Where(x => customerIds.Contains(x.CustomerNumber))
                        .Where(x => x.ReportDate == lastAging.ReportDate).ToList();
                }
            }
        }
        #endregion

        private static SalesInsightDataContext GetContext()
        {
            SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString);
            return context;
        }

        public static List<SyncHistory> GetLastSyncDates()
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            using (var context = GetContext())
            {
                var query = context.APIFetchHistories
                              .Where(x => x.LastImportDate != null)
                              .GroupBy(history => history.EntityType)
                              .Select(h => new SyncHistory
                              {
                                  Entity = h.Key,
                                  LastImportDate = String.Format("{0:MM/dd/yyyy hh:mm tt ' (GMT)'}", localZone.ToUniversalTime(h.Max(a => a.LastImportDate.GetValueOrDefault())))
                              });

                return query.ToList();
            }
        }

        #region Project

        //If Project Customer Is Null Update Project CustomerId with matching Quote CustomerId
        //public static void UpdateProjectCustomerWithQuoteCustomer(long quotationId)
        //{
        //    using (var context = GetContext())
        //    {
        //        Quotation quote = context.Quotations.FirstOrDefault(x => x.Id == quotationId);
        //        int? quoteCustomerId = quote.CustomerId;
        //        if (quoteCustomerId != null)
        //        {
        //            Project proj = context.Projects.Where(x => x.ProjectId == quote.ProjectId).FirstOrDefault();
        //            if (proj.CustomerId == null)
        //            {
        //                proj.CustomerId = quoteCustomerId;
        //            }
        //            context.SubmitChanges();
        //        }
        //    }
        //}

        // Update project BidDate with Quotation Quotedate if null
        public static void UpdateBidDateIfNull(int? projectId)
        {
            using (var context = GetContext())
            {
                Quotation quote = context.Quotations.FirstOrDefault(x => x.ProjectId == projectId);
                if (quote != null)
                {
                    Project proj = context.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                    if (proj.BidDate == null)
                        proj.BidDate = quote.QuoteDate;
                }
                context.SubmitChanges();
            }
        }
        public static void UpdateProjectNoteKey(string key, int id)
        {
            using (var db = GetContext())
            {
                if (id > 0 && !string.IsNullOrEmpty(key))
                {
                    var dbObj = db.ProjectNotes.Where(x => x.ProjectNoteId == id).FirstOrDefault();
                    if (dbObj != null)
                    {
                        dbObj.FileKey = key;
                    }
                    db.SubmitChanges();
                }
            }
        }
        // Add Update Project Note
        public static int AddUpdateProjectNotes(ProjectNote entity)
        {
            using (var context = GetContext())
            {
                if (entity.ProjectNoteId == 0)
                {
                    context.ProjectNotes.InsertOnSubmit(entity);
                }
                else
                {
                    context.ProjectNotes.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
            return entity.ProjectNoteId;
        }
        //Prject Bidder
        public static ProjectBidder GetProjectBidder(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<ProjectBidder>(c => c.Customer);
                context.LoadOptions = options;
                return context.ProjectBidders.Where(p => p.Id == id).FirstOrDefault();
            }
        }
        public static void AddUpdateProjectBidder(ProjectBidder projectBidder)
        {
            using (var context = GetContext())
            {
                if (projectBidder.Id == 0)
                {
                    projectBidder.CreatedTime = DateTime.Now;
                    context.ProjectBidders.InsertOnSubmit(projectBidder);
                }
                else
                {
                    ProjectBidder projBidder = context.ProjectBidders.Where(x => x.Id == projectBidder.Id).FirstOrDefault();
                    projBidder.LastEditedTime = DateTime.Now;
                    projBidder.Notes = projectBidder.Notes;
                    projBidder.CustomerId = projectBidder.CustomerId;
                    projBidder.ProjectId = projectBidder.ProjectId;
                    projectBidder.UserId = projectBidder.UserId;
                }
                context.SubmitChanges();
            }
        }
        public static void DeleteProjectBidder(int projectBidderId)
        {
            using (var context = GetContext())
            {
                ProjectBidder bidder = context.ProjectBidders.Where(p => p.Id == projectBidderId).FirstOrDefault();
                if (bidder != null)
                {
                    context.ProjectBidders.DeleteOnSubmit(bidder);
                    context.SubmitChanges();
                }

            }
        }
        //When a quote is approved, check to see if Project.Mix is null...
        //...IF yes THEN populate with Quote Description of mix with highest quoted volume.
        //...IF no THEN leave as-is.
        public static void PopulateProjectMixWithHighestQuotedVolume(int projectId, long quoteId)
        {
            using (var context = GetContext())
            {
                var isProjectMixNotExist = context.Projects.Where(x => x.ProjectId == projectId).Select(x => x.Mix).First() == null ? true : false;
                if (isProjectMixNotExist)
                {
                    List<QuotationMix> qMixes = context.QuotationMixes.Where(x => x.QuotationId == quoteId).OrderByDescending(x => x.Volume).ToList();
                    if (qMixes != null)
                    {
                        Project proj = context.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                        proj.Mix = qMixes.Select(x => x.QuotedDescription).FirstOrDefault();
                        context.SubmitChanges();
                    }
                }
            }
        }
        public static bool CheckAwardedQuoteExist(int projectId)
        {
            using (var context = GetContext())
            {
                var totalQuote = context.Quotations.Where(x => x.ProjectId == projectId && x.Awarded == true).ToList();
                bool IsQuoteExist = false;

                if (totalQuote.Count() > 0)
                    IsQuoteExist = true;

                return IsQuoteExist;
            }
        }

        public static void UpdateProjectPriceSpreadProfit(int? projectId)
        {
            if (projectId != null)
            {
                DataLoadOptions opts = new DataLoadOptions();
                opts.LoadWith<Project>(x => x.Quotations);
                opts.LoadWith<Project>(x => x.ProjectPlants);
                using (var context = GetContext())
                {
                    context.LoadOptions = opts;
                    var proj = context.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                    var projectQuotes = proj.Quotations.Where(x => x.Awarded.GetValueOrDefault(false)).ToList();
                    if (projectQuotes.Count() == 0)
                        projectQuotes = proj.Quotations.ToList();
                    else
                        proj.CustomerId = projectQuotes.Where(x => x.Awarded == true).OrderByDescending(x => x.TotalVolume).Select(x => x.CustomerId).FirstOrDefault();

                    var quotesVolume = Convert.ToDecimal(projectQuotes.Sum(x => x.TotalVolume));
                    proj.Volume = proj.ProjectPlants.Sum(x => x.Volume);
                    proj.ConcretePlantId = proj.ProjectPlants.OrderByDescending(x => x.Volume).Select(x => x.PlantId).FirstOrDefault();
                    if (projectQuotes.Count() > 0)
                    {
                        if (quotesVolume > 0)
                        {
                            proj.Price = projectQuotes.Sum(x => x.AvgSellingPrice.GetValueOrDefault(0) * Convert.ToDecimal(x.TotalVolume)) / quotesVolume;
                            proj.Spread = projectQuotes.Sum(x => x.Spread.GetValueOrDefault(0) * Convert.ToDecimal(x.TotalVolume)) / quotesVolume;
                            proj.Profit = projectQuotes.Sum(x => x.Profit.GetValueOrDefault(0) * Convert.ToDecimal(x.TotalVolume)) / quotesVolume;
                            proj.Contribution = projectQuotes.Sum(x => x.Contribution.GetValueOrDefault(0) * Convert.ToDecimal(x.TotalVolume)) / quotesVolume;
                        }

                        decimal quoteAggregateCalc = 0;
                        decimal quoteAggPriceCalc = 0;
                        decimal quoteBlockPriceCalc = 0;
                        double quoteAggregateTotalQtyCalc = 0;
                        decimal quoteBlockCalc = 0;
                        double quoteBlockTotalQtyCalc = 0;
                        foreach (var quote in projectQuotes)
                        {
                            //Quotation Aggregate
                            var quoteAggregateList = context.QuotationAggregates.Where(x => x.QuotationId == quote.Id).ToList();
                            if (quoteAggregateList != null)
                            {
                                var quoteTotalQty = quoteAggregateList.Sum(x => x.Volume.GetValueOrDefault());
                                quoteAggregateTotalQtyCalc = quoteTotalQty + quoteAggregateTotalQtyCalc;
                                var quoteAggTotalFreight = quoteAggregateList.Sum(x => x.Freight);
                                quoteAggregateCalc = (Convert.ToDecimal(quoteTotalQty) * quoteAggTotalFreight.GetValueOrDefault()) + quoteAggregateCalc;

                                var quoteAggTotalPrice = quoteAggregateList.Sum(x => x.Price);
                                quoteAggPriceCalc = (Convert.ToDecimal(quoteTotalQty) * quoteAggTotalPrice.GetValueOrDefault()) + quoteAggPriceCalc;
                            }

                            // Quotation Block
                            var quoteBlockList = context.QuotationBlocks.Where(x => x.QuotationId == quote.Id).ToList();
                            if (quoteBlockList != null)
                            {
                                var quoteBlockTotalQty = quoteBlockList.Sum(x => x.Volume.GetValueOrDefault());
                                quoteBlockTotalQtyCalc = quoteBlockTotalQty + quoteBlockTotalQtyCalc;
                                var quoteBlockTotalFreight = quoteBlockList.Sum(x => x.Freight);
                                quoteBlockCalc = (Convert.ToDecimal(quoteBlockTotalQty) * quoteBlockTotalFreight.GetValueOrDefault()) + quoteBlockCalc;

                                var quoteBlockTotalPrice = quoteBlockList.Sum(x => x.Price);
                                quoteBlockPriceCalc = (Convert.ToDecimal(quoteBlockTotalQty) * quoteBlockTotalPrice.GetValueOrDefault()) + quoteBlockPriceCalc;
                            }
                        }
                        if (quoteAggregateTotalQtyCalc > 0)
                        {
                            proj.AggregateFreight = quoteAggregateCalc / (decimal)quoteAggregateTotalQtyCalc;
                            proj.AggProductPrice = quoteAggPriceCalc / (decimal)quoteAggregateTotalQtyCalc;

                        }
                        if (quoteBlockTotalQtyCalc > 0)
                        {
                            proj.BlockFreight = quoteBlockCalc / (decimal)quoteBlockTotalQtyCalc;
                            proj.BlockProductPrice = quoteBlockPriceCalc / (decimal)quoteBlockTotalQtyCalc;

                        }
                    }
                    context.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// NOTE : To be used only when needed to update all Project's Contribution field.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="quoteId"></param>
        public static string ProjectsContributionUpdateScript()
        {
            string updateStatus = "Project's Contribution Updated Successfully";
            using (var db = GetContext())
            {
                try
                {
                    DataLoadOptions opts = new DataLoadOptions();
                    opts.LoadWith<Project>(x => x.Quotations);
                    db.LoadOptions = opts;
                    var proj = db.Projects.ToList();
                    foreach (var item in proj)
                    {
                        var awardedQuotes = db.Quotations.Where(x => x.Awarded == true && x.ProjectId == item.ProjectId).ToList();
                        if (awardedQuotes.Count() == 0)
                            awardedQuotes = item.Quotations.ToList();

                        if (awardedQuotes.Count() > 0)
                        {
                            var awardedQuotesVolume = Convert.ToDecimal(awardedQuotes.Sum(x => x.TotalVolume));
                            if (awardedQuotesVolume > 0)
                            {
                                var projectContribution = awardedQuotes.Sum(x => x.Contribution.GetValueOrDefault(0) * Convert.ToDecimal(x.TotalVolume)) / awardedQuotesVolume;

                                Project project = item;
                                project.Contribution = projectContribution;
                                db.SubmitChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    updateStatus = "Project's Contribution Updation Failed  ERROR :: " + ex;
                }

            }
            return updateStatus;
        }

        public static string UpdateQuotationBiddingDateFromProject()
        {
            string status = "Quotation Bidding Date Updated Successfully!!";

            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Project>(x => x.Quotations);
            using (var context = GetContext())
            {
                try
                {
                    context.LoadOptions = opts;
                    var projectList = context.Projects.ToList();
                    foreach (var project in projectList)
                    {
                        var quoteList = project.Quotations.ToList();
                        foreach (var quote in quoteList)
                        {
                            quote.BiddingDate = project.BidDate;
                            context.SubmitChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = "Quotation Bid Date updation FAILED ERROR :: " + ex;
                }
            }
            return status;
        }

        public void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }

        public static void UpdateProjectPlantVolume(long projectId, long quoteId = 0)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Project>(x => x.Quotations);
            opts.LoadWith<Project>(x => x.ProjectPlants);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                var proj = context.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                var awardedQuote = proj.Quotations.Where(x => x.Awarded.GetValueOrDefault(false)).ToList();
                if (quoteId != 0 && awardedQuote.Count() == 0)
                    awardedQuote = context.Quotations.Where(x => x.Id == quoteId).ToList();

                var projPlant = proj.ProjectPlants.ToList();
                if (projPlant.Count() > 0)
                {
                    var projPlantIds = projPlant.Select(x => x.PlantId).ToList();

                    var totalProjectPlantVolume = projPlant.Sum(x => x.Volume);
                    var totalAwardedQuotesVolume = awardedQuote.Sum(x => x.TotalVolume);
                    if (totalProjectPlantVolume < totalAwardedQuotesVolume)
                    {
                        var awardedQuotations = awardedQuote.GroupBy(x => x.PlantId).Select(quote => new { plantId = quote.Key, newTotalVolume = quote.Sum(x => x.TotalVolume) }).ToList();
                        foreach (var awQuote in awardedQuotations)
                        {
                            ProjectPlant newProjectPlant = new ProjectPlant();

                            if (projPlantIds.Contains(awQuote.plantId.GetValueOrDefault()))
                            {
                                var pPlant = context.ProjectPlants.Where(x => x.ProjectId == projectId && x.PlantId == awQuote.plantId).FirstOrDefault();
                                pPlant.Volume = Convert.ToInt32(awQuote.newTotalVolume);
                            }
                            else
                            {
                                newProjectPlant.ProjectId = Convert.ToInt32(projectId);
                                newProjectPlant.PlantId = Convert.ToInt32(awQuote.plantId);
                                newProjectPlant.Volume = Convert.ToInt32(awQuote.newTotalVolume);
                                context.ProjectPlants.InsertOnSubmit(newProjectPlant);
                            }
                            context.SubmitChanges();
                        }
                    }
                    UpdateProjectPriceSpreadProfit(Convert.ToInt32(projectId));
                }
            }
        }

        #endregion

        public static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        #region BrannanSyncLogic

        public static List<MarketSegment> GetMarketSegmentWithNullsDID(int companyId)
        {
            using (var db = GetContext())
            {
                if (companyId > 0)
                {
                    return db.MarketSegments.Where(x => x.CompanyId == companyId && x.DispatchId == null).ToList();
                }
                else
                    return null;
            }
        }

        public static void UpdateMarketSegmentsDispatchId(List<MarketSegment> dbMarketSegments)
        {
            using (var db = GetContext())
            {
                foreach (var item in dbMarketSegments)
                {
                    if (item != null)
                    {
                        var dbObj = db.MarketSegments.Where(x => x.MarketSegmentId == item.MarketSegmentId && x.DispatchId == null).FirstOrDefault();
                        if (dbObj != null)
                        {
                            dbObj.DispatchId = item.DispatchId;
                        }
                    }
                }
                db.SubmitChanges();
            }
        }

        public static void UpdateDispatchIdsForPlants(List<Plant> plantList)
        {
            using (var db = GetContext())
            {
                foreach (var item in plantList)
                {
                    if (item != null)
                    {
                        var dbObj = db.Plants.Where(x => x.PlantId == item.PlantId).FirstOrDefault();
                        if (dbObj != null)
                        {
                            dbObj.DispatchId = item.DispatchId;
                        }
                    }
                }
                db.SubmitChanges();
            }
        }

        public static List<RawMaterialType> GetAllRawMaterialTypeWithNullDID()
        {
            using (var db = GetContext())
            {
                return db.RawMaterialTypes.Where(x => x.DispatchId == null).ToList();

            }
        }

        public static void UpdateRawMaterialsDispatchId(List<RawMaterialType> dbRawMaterialsToBeUpdated)
        {
            using (var db = GetContext())
            {
                if (dbRawMaterialsToBeUpdated.Any())
                {
                    foreach (var item in dbRawMaterialsToBeUpdated)
                    {
                        var dbObj = db.RawMaterialTypes.Where(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase) && x.DispatchId == null).FirstOrDefault();
                        if (dbObj != null)
                        {
                            dbObj.DispatchId = item.DispatchId;
                        }
                    }
                }
                db.SubmitChanges();
            }
        }

        public static List<ProjectStatus> GetAllProjectStatusWithNullDID(int companyId)
        {
            using (var db = GetContext())
            {
                if (companyId > 0)
                {
                    return db.ProjectStatus.Where(x => x.CompanyId == companyId && x.DispatchId == null).ToList();
                }
                else
                    return null;
            }
        }

        public static void UpdateProjectStatusesDispatchId(List<ProjectStatus> dbUpdateList)
        {
            using (var db = GetContext())
            {
                if (dbUpdateList.Any())
                {
                    foreach (var item in dbUpdateList)
                    {
                        var dbObj = db.ProjectStatus.Where(x => x.Name == item.Name && x.DispatchId == null && x.ProjectStatusId == item.ProjectStatusId).FirstOrDefault();
                        if (dbObj != null)
                        {
                            dbObj.DispatchId = item.DispatchId;
                        }
                    }
                    db.SubmitChanges();
                }
            }
        }

        public static List<TaxCode> GetTaxCodesWithNullDID()
        {
            using (var db = GetContext())
            {
                return db.TaxCodes.Where(x => x.DispatchId == null).ToList();
            }
        }

        public static void UpdateTaxCodesDispatchId(List<TaxCode> dbList)
        {
            using (var db = GetContext())
            {
                if (dbList.Any())
                {
                    foreach (var item in dbList)
                    {
                        var dbObj = db.TaxCodes.Where(x => x.Code == item.Code && x.DispatchId == null).FirstOrDefault();
                        if (dbObj != null)
                        {
                            dbObj.DispatchId = item.DispatchId;
                        }
                    }
                    db.SubmitChanges();
                }
            }
        }

        public static List<Uom> GetUOMWithNullsDispatchId()
        {
            using (var db = GetContext())
            {
                return db.Uoms.Where(x => x.DispatchId == null).ToList();
            }
        }

        public static void UpdateUOMForDispatchId(List<Uom> dbList)
        {
            using (var db = GetContext())
            {
                if (dbList.Any())
                {
                    foreach (var item in dbList)
                    {
                        var dbObj = db.Uoms.Where(x => x.Name.Equals(item.Name) && x.DispatchId == null).FirstOrDefault();
                        if (dbObj != null)
                        {
                            dbObj.DispatchId = item.DispatchId;
                        }
                    }

                    db.SubmitChanges();
                }
            }
        }

        public static List<Customer> GetCustomersWithNullsDID()
        {
            using (var db = GetContext())
            {
                return db.Customers.Where(x => x.DispatchId == null).ToList();
            }
        }

        public static List<StandardMix> GetStandardMixWithNullsDID()
        {
            using (var db = GetContext())
            {
                return db.StandardMixes.Where(x => x.DispatchId == null).ToList();
            }
        }

        public static void UpdateCustomersForDispatchId(List<Customer> dbList)
        {
            using (var db = GetContext())
            {
                foreach (var item in dbList)
                {
                    var dbObj = db.Customers.Where(x => x.CustomerNumber == item.CustomerNumber && x.DispatchId == null).FirstOrDefault();
                    if (dbObj != null)
                    {
                        dbObj.DispatchId = item.DispatchId;
                    }
                }

                db.SubmitChanges();
            }
        }

        public static List<RawMaterial> GetRawMaterialsWithNullsDID()
        {
            using (var db = GetContext())
            {
                return db.RawMaterials.Where(x => x.DispatchId == null).ToList();
            }
        }

        public static void UpdateRawMaterialsForDispatchId(List<RawMaterial> dbList)
        {
            using (var db = GetContext())
            {
                foreach (var item in dbList)
                {
                    var dbObj = db.RawMaterials.Where(x => x.MaterialCode != null && x.MaterialCode.ToLower() == item.MaterialCode.ToLower() && x.DispatchId == null).FirstOrDefault();
                    if (dbObj != null)
                    {
                        dbObj.DispatchId = item.DispatchId;
                    }
                }

                db.SubmitChanges();
            }
        }

        public static List<SalesStaff> GetSalesStaffWithNullDID(int companyId)
        {
            using (var db = GetContext())
            {
                if (companyId > 0)
                    return db.SalesStaffs.Where(x => x.CompanyId == companyId).ToList();
                else
                    return null;
            }
        }

        public static void UpdateStandardMixes(List<StandardMix> updateList)
        {
            using (var db = GetContext())
            {
                if (updateList.Any())
                {
                    foreach (var item in updateList)
                    {
                        if (item != null)
                        {
                            var dbEntity = db.StandardMixes.Where(x => x.Number.ToLower() == item.Number.ToLower()).FirstOrDefault();

                            if (dbEntity != null)
                            {
                                dbEntity.DispatchId = item.DispatchId;
                            }
                        }
                    }

                    db.SubmitChanges();
                }
            }
        }

        #endregion

        #region Quotation Form Setting
        public static QuotationFormSetting GetQuotationFormSetting(long quoteId)
        {
            using (var context = GetContext())
            {
                return context.QuotationFormSettings.Where(x => x.QuoteId == quoteId).FirstOrDefault();
            }
        }

        public static void UpdateQuoteFormSetting(QuotationFormSetting qfs)
        {
            using (var context = GetContext())
            {
                if (qfs.Id == 0)
                {
                    context.QuotationFormSettings.InsertOnSubmit(qfs);
                }
                else
                {
                    context.QuotationFormSettings.Attach(qfs);
                    context.Refresh(RefreshMode.KeepCurrentValues, qfs);
                }
                context.SubmitChanges();
            }
        }
        #endregion

        #region Quote Margin Limits

        public static List<string> GetMarginLimitViolations(long quotationId, string roleName)
        {
            List<string> limitViolations = new List<string>();
            using (var context = GetContext())
            {
                Quotation quote = context.Quotations.FirstOrDefault(x => x.Id == quotationId);

                if (quote != null)
                {
                    var rules = context.RoleAccesses.FirstOrDefault(x => x.RoleName == roleName);

                    if (rules != null)
                    {
                        if (quote.Spread < rules.MinSpread)
                        {
                            limitViolations.Add("Spread violates the lower limit");
                        }
                        if (quote.Spread > rules.MaxSpread)
                        {
                            limitViolations.Add("Spread violates the upper limit");
                        }
                        if (quote.Contribution < rules.MinContribution)
                        {
                            limitViolations.Add("Contribution violates the lower limit");
                        }
                        if (quote.Contribution > rules.MaxContribution)
                        {
                            limitViolations.Add("Contribution violates the upper limit");
                        }
                        if (quote.Profit < rules.MinProfit)
                        {
                            limitViolations.Add("Profit violates the lower limit");
                        }
                        if (quote.Profit > rules.MaxProfit)
                        {
                            limitViolations.Add("Profit violates the upper limit");
                        }
                    }
                }
            }
            return limitViolations;
        }

        #endregion

        public static List<Customer> GetCustomers(List<long> payloadList)
        {
            using (var context = GetContext())
            {
                var custs = context.Customers.ToList();

                var companySettings = context.CompanySettings.FirstOrDefault();

                if (companySettings != null && companySettings.EnableAPI.GetValueOrDefault())
                {
                    if (custs.Where(x => !string.IsNullOrEmpty(x.DispatchId)).Count() > 0)
                    {
                        throw new ApplicationException("Cannot merge when more than one synced customers are selected", new InvalidOperationException("More than one synced customer is selected"));
                    }
                }

                custs = custs.Where(x => payloadList.Contains(x.CustomerId)).ToList();

                return custs;
            }
        }

        public static bool MergeDuplicateCustomers(int keepCustomerId, List<int> customersToDelete)
        {
            using (var context = GetContext())
            {
                if (customersToDelete.Contains(keepCustomerId))
                    customersToDelete.Remove(keepCustomerId);
                bool success = true;
                try
                {

                    #region Customer Contacts

                    var customerContacts = context.CustomerContacts.Where(x => customersToDelete.Contains(x.CustomerId));
                    var keepCustomerContacts = context.CustomerContacts.Where(x => x.CustomerId == keepCustomerId).ToList();

                    foreach (var item in customerContacts)
                    {
                        int suffix = (keepCustomerContacts.Count(x => x.Name == item.Name) + 1) / 1;

                        item.Name = item.Name + (suffix == 0 ? "" : " (" + suffix + ")");
                        item.CustomerId = keepCustomerId;
                    }

                    context.SubmitChanges();

                    #endregion

                    #region District Customers

                    var districtCustomers = context.DistrictCustomers.Where(x => customersToDelete.Contains(x.CustomerId));

                    foreach (var item in districtCustomers)
                    {
                        item.CustomerId = keepCustomerId;
                    }

                    context.SubmitChanges();

                    #endregion

                    #region Projects

                    var projects = context.Projects.Where(x => customersToDelete.Contains(x.CustomerId.GetValueOrDefault()));

                    foreach (var item in projects)
                    {
                        item.CustomerId = keepCustomerId;
                    }

                    context.SubmitChanges();

                    #endregion

                    #region Project Bidders

                    var projectBidders = context.ProjectBidders.Where(x => customersToDelete.Contains(x.CustomerId));

                    foreach (var item in projectBidders)
                    {
                        item.CustomerId = keepCustomerId;
                    }

                    context.SubmitChanges();

                    #endregion

                    #region Quotations

                    var quotations = context.Quotations.Where(x => customersToDelete.Contains(x.CustomerId.GetValueOrDefault()));

                    foreach (var item in quotations)
                    {
                        item.CustomerId = keepCustomerId;
                    }

                    context.SubmitChanges();

                    #endregion
                }
                catch (Exception ex)
                {
                    success = false;
                }
                if (success)
                {
                    var dbCustomersToDelete = context.Customers.Where(x => customersToDelete.Contains(x.CustomerId)).ToList();

                    CustomerArchive customer = null;
                    foreach (var item in dbCustomersToDelete)
                    {
                        customer = new CustomerArchive();
                        customer.CustomerId = item.CustomerId;
                        customer.Name = item.Name;
                        customer.Address1 = item.Address1;
                        customer.Address2 = item.Address2;
                        customer.Address3 = item.Address3;
                        customer.City = item.City;
                        customer.State = item.State;
                        customer.Zip = item.Zip;
                        customer.Active = item.Active;
                        customer.CompanyId = item.CompanyId;
                        customer.CustomerNumber = item.CustomerNumber;
                        customer.DispatchId = item.DispatchId;
                        customer.Source = item.Source;

                        context.CustomerArchives.InsertOnSubmit(customer);

                        context.Customers.DeleteOnSubmit(item);
                    }
                    context.SubmitChanges();
                }

                return success;
            }
        }

        public static List<CustomerBasic> GetDuplicateCustomers(bool showInactives)
        {
            using (var db = GetContext())
            {
                var query = db.Customers.AsQueryable();

                if (!showInactives)
                {
                    query = query.Where(x => x.Active == true);
                }
                var customers = query.ToList();

                var ignoreNumbers = db.CustomerMergeJunkWords.Where(x => x.MappedField == "Number").Where(x => x.Active != false).Select(x => x.Word).ToList();

                var junkWords = db.CustomerMergeJunkWords.Where(x => x.MappedField == "Name").Where(x => x.Active != false).Select(x => x.Word).ToList();

                string pattern = "";
                if (junkWords != null)
                {
                    for (var i = 0; i < junkWords.Count; i++)
                    {
                        pattern += string.Format(@"\b{0}\b", junkWords[i]);

                        if (i != (junkWords.Count - 1))
                        {
                            pattern += "|";
                        }
                    }
                }
                var dynamicRegex = new Regex(pattern, RegexOptions.IgnoreCase);

                var duplicatesByName = customers.GroupBy(c => dynamicRegex.Replace(c.Name, "").Replace(" ", "").ToLower().Trim())
                                           .Where(g => g.Count() > 1)
                                           .SelectMany(g => g.Skip(0)).ToList();

                var duplicatesByNumber = customers.Where(x => !duplicatesByName.Any(y => y.CustomerId == x.CustomerId))
                                            .Where(x => x.CustomerNumber != null && x.CustomerNumber.Trim() != "" && !ignoreNumbers.Contains(x.CustomerNumber.Trim().ToLower()))
                                            .GroupBy(s => s.CustomerNumber.Trim().ToLower())
                                            .Where(g => g.Count() > 1)
                                            .SelectMany(g => g.Skip(0)).ToList();

                List<CustomerBasic> dupNames = new List<CustomerBasic>();

                foreach (var item in duplicatesByName)
                {
                    dupNames.Add(new CustomerBasic { CustomerId = item.CustomerId, Number = item.CustomerNumber, Name = item.Name, Address1 = item.Address1, Address2 = item.Address2, Address3 = item.Address3, Active = item.Active.GetValueOrDefault(), City = item.City, State = item.State, Zip = item.Zip, Group = dynamicRegex.Replace(item.Name, "").Replace(" ", "").ToLower().Trim(), GroupHandle = "Name" });
                }

                List<CustomerBasic> dupNumbers = new List<CustomerBasic>();
                foreach (var item in duplicatesByNumber)
                {
                    dupNumbers.Add(new CustomerBasic { CustomerId = item.CustomerId, Number = item.CustomerNumber, Name = item.Name, Address1 = item.Address1, Address2 = item.Address2, Address3 = item.Address3, Active = item.Active.GetValueOrDefault(), City = item.City, State = item.State, Zip = item.Zip, Group = item.CustomerNumber.Trim().ToLower(), GroupHandle = "Number" });
                }

                return dupNames.Concat(dupNumbers).ToList();
            }
        }

        #region UserLogin

        public static aspnet_User FindUserByName(string username)
        {

            using (var context = GetContext())
            {
                return context.aspnet_Users.Where(x => x.UserName == username).FirstOrDefault();
            }
        }

        public static aspnet_Membership FindByEmail(string email)
        {
            using (var context = GetContext())
            {
                var user = context.aspnet_Memberships.Where(x => x.Email == email).FirstOrDefault();
                if (user != null)
                    return user;
                else
                    return null;
            }
        }

        public static aspnet_User FindUserByEmail(string email)
        {
            aspnet_User user = new aspnet_User();
            using (var context = GetContext())
            {
                var membershipUser = context.aspnet_Memberships.Where(x => x.Email == email).FirstOrDefault();
                if (membershipUser != null)
                {
                    var userId = membershipUser.UserId;
                    user = context.aspnet_Users.Where(x => x.UserId == userId).FirstOrDefault();
                }
            }
            return user;
        }

        /// <summary>
        /// Check  Max User Limit Exceeds
        /// </summary>
        /// <returns>bool</returns>
        public static bool CheckMaxUserLimitExceeds()
        {
            bool userLimitExceedStatus = false;
            using (var context = GetContext())
            {
                var maxUserLimit = context.CompanySettings.Select(x => x.MaxUsers).FirstOrDefault();
                if (maxUserLimit != null)
                {
                    List<Guid> activeUsersList = context.aspnet_Memberships.Where(x => x.IsApproved == true).Select(x => x.UserId).ToList();
                    List<Guid> maxExemptUserList = context.CompanyUsers.Where(x => x.IsMaxUsersExempt == true).Select(x => x.UserId).ToList();
                    var totalActiveUsers = activeUsersList.Count();
                    var totalExemptUsers = activeUsersList.Intersect(maxExemptUserList).Count();
                    var totalActiveNonExemptUsers = totalActiveUsers - totalExemptUsers;
                    if (maxUserLimit <= totalActiveNonExemptUsers)
                    {
                        userLimitExceedStatus = true;
                    }
                }
            }
            return userLimitExceedStatus;
        }
        /// <summary>
        /// Check Max Sales Staff Limit Exceeds
        /// </summary>
        /// <returns>Boolean Value</returns>
        public static bool CheckMaxSalesStaffLimitExceeds()
        {
            bool salesStaffLimitExceedStatus = false;
            using (var context = GetContext())
            {
                var maxSalesStaffLimit = context.CompanySettings.Select(x => x.MaxSalesStaff).FirstOrDefault();
                if (maxSalesStaffLimit != null)
                {
                    var currentActiveSalesStaff = context.SalesStaffs.Where(x => x.Active == true).Count();
                    if (maxSalesStaffLimit <= currentActiveSalesStaff)
                    {
                        salesStaffLimitExceedStatus = true;
                    }
                }
            }
            return salesStaffLimitExceedStatus;
        }
        public static List<aspnet_Membership> GetAllUsers()
        {
            using (var context = GetContext())
            {
                return context.aspnet_Memberships.ToList();
            }
        }

        public static aspnet_Membership FindByUserId(Guid userid)
        {
            using (var context = GetContext())
            {
                return context.aspnet_Memberships.Where(x => x.UserId == userid).FirstOrDefault();
            }
        }

        public static SISuperUserSettings FindSuperUserSettings()
        {
            using (var context = GetContext())
            {
                var suSetting = context.SuperUserSettings.FirstOrDefault();

                if (suSetting != null)
                {
                    return new SISuperUserSettings(suSetting);
                }
                return new SISuperUserSettings();
            }
        }

        public static SuperUserSetting FindUserSettings()
        {
            using (var context = new SalesInsightDataContext())
            {
                return context.SuperUserSettings.FirstOrDefault();
            }
        }

        public static void ChangePassword(string username, string password, string email)
        {

            var m = SIDAL.FindByEmail(email);
            try
            {
                //Membership.Provider.ChangePassword(username, m.Password, password);
                MembershipUser u = Membership.GetUser(username);
                u.ChangePassword(u.ResetPassword(), password);
            }
            catch (Exception e)
            {

            }

        }

        public static aspnet_Membership VerifyUserOldPassword(string password)
        {
            using (var context = GetContext())
            {
                return context.aspnet_Memberships.Where(x => x.Password == password).FirstOrDefault();
            }
        }

        public static void ResetPassword(string username, string password)
        {
            try
            {
                MembershipUser u = Membership.GetUser(username);
                u.ChangePassword(u.ResetPassword(), password);
            }
            catch (Exception e)
            {

            }
        }

        public static void ExpirePassword(Guid userid, int days)
        {
            using (var context = GetContext())
            {
                var users = context.aspnet_Memberships.ToList();
                foreach (var user in users)
                {
                    if (user.UserId != userid)
                    {
                        user.LastPasswordChangedDate = DateTime.Now.AddDays(-days);
                    }
                    context.SubmitChanges();
                }
            }
        }

        public static void UpdateSuperUserSettings(SuperUserSetting setting)
        {
            using (var context = GetContext())
            {
                if (setting.Id > 0)
                {
                    context.SuperUserSettings.Attach(setting);
                    context.Refresh(RefreshMode.KeepCurrentValues, setting);

                }
                context.SubmitChanges();
            }
        }

        public static bool CanUsePassword(string username, string password)
        {
            string hashedPassword = PasswordUtility.OneWayHash(password);

            List<string> passwordList = null;

            using (var context = GetContext())
            {

                int passwordHistoryLimit = context.SuperUserSettings.FirstOrDefault().PasswordHistoryLimit;

                if (passwordHistoryLimit == 0)
                    return true;
                else
                {
                    passwordList = context.UserPasswordHistories.Where(x => x.UserName == username).
                                        OrderByDescending(x => x.Id).
                                        Skip(0).
                                        Take(passwordHistoryLimit).
                                        Select(x => x.Password).
                                        ToList();
                }
            }

            if (passwordList != null && passwordList.Count() > 0)
            {
                return !passwordList.Contains(hashedPassword);
            }
            else
            {
                return true;
            }
        }

        public static void SaveUpdatePasswordHistory(string userName, string password, int maxAge)
        {
            using (var context = GetContext())
            {
                UserPasswordHistory userPasswordHistory = new UserPasswordHistory();
                userPasswordHistory.UserName = userName;
                userPasswordHistory.Password = PasswordUtility.OneWayHash(password);
                context.UserPasswordHistories.InsertOnSubmit(userPasswordHistory);
                context.SubmitChanges();
            }

        }

        public static void SaveUserPasswordToken(string userName, string token)
        {
            using (var context = GetContext())
            {
                UserPasswordVerification userPasswordToken = new UserPasswordVerification();

                userPasswordToken.Name = userName;
                userPasswordToken.PasswordVerificationToken = token;
                userPasswordToken.PasswordVerificationTokenExpirationDate = DateTime.Now.AddDays(2);
                context.UserPasswordVerifications.InsertOnSubmit(userPasswordToken);
                context.SubmitChanges();
            }

        }

        public static void UpdateUserPasswordToken(string username, string token)
        {
            using (var context = GetContext())
            {

                var usertoken = context.UserPasswordVerifications.Where(x => x.Name == username && x.PasswordVerificationToken == token).FirstOrDefault();
                usertoken.IsUsed = true;

                context.SubmitChanges();
            }
        }

        public static bool ValidatePasswordToken(string username, string token)
        {
            using (var context = GetContext())
            {
                var usertoken = context.UserPasswordVerifications.Where(x => x.Name == username && x.PasswordVerificationToken == token).FirstOrDefault();
                if (usertoken != null)
                {
                    if (usertoken.PasswordVerificationTokenExpirationDate > DateTime.Now && usertoken.IsUsed == null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region User Support

        public static List<SupportCategory> GetSupportCategories()
        {
            using (var context = GetContext())
            {
                return context.SupportCategories.Where(x => x.Active != false).ToList();
            }
        }

        public static List<string> GetSupportRecipients(int supportCategoryId)
        {
            using (var context = GetContext())
            {
                //Either get the recipients for the category or get the default Recipient
                return context.SupportRequestRoutings
                              .Where(x => x.SupportCategoryId == supportCategoryId || x.SupportRecipient.IsDefault.GetValueOrDefault() == true)
                              .Select(x => x.SupportRecipient.Email).ToList();

            }
        }

        public static void AddSupportRequest(SupportRequest supportRequest, List<SupportRequestAttachment> attachments)
        {
            using (var context = GetContext())
            {
                if (attachments != null)
                {
                    supportRequest.RequestId = "SR" + (context.SupportRequests.Count() + 1).ToString();
                    context.SupportRequests.InsertOnSubmit(supportRequest);
                    foreach (var item in attachments)
                    {
                        supportRequest.SupportRequestAttachments.Add(item);
                    }
                }

                context.SubmitChanges();
            }
        }

        public static string GetSupportCategory(int categoryId)
        {
            using (var context = GetContext())
            {
                return context.SupportCategories.Where(x => x.Id == categoryId).Select(x => x.Name).FirstOrDefault();
            }
        }

        public static SupportRequest GetSupportRequest(Guid id)
        {
            using (var context = GetContext())
            {
                DataLoadOptions o = new DataLoadOptions();
                o.LoadWith<SupportRequest>(x => x.aspnet_User);
                o.LoadWith<SupportRequest>(x => x.SupportCategory);
                context.LoadOptions = o;

                return context.SupportRequests.FirstOrDefault(x => x.Id == id);
            }
        }

        #endregion

        public static string GetCompanyNameByUserId(Guid userId)
        {
            using (var db = GetContext())
            {
                return db.CompanyUsers.Where(x => x.UserId == userId).Select(x => x.Company.Name).FirstOrDefault();
            }
        }

        public static Company GetCompany()
        {
            using (var db = GetContext())
            {
                return db.Companies.FirstOrDefault();
            }
        }

        public static CompanyUser GetCompanyUser(Guid userId)
        {
            using (var db = GetContext())
            {
                return db.CompanyUsers.Where(x => x.UserId == userId).FirstOrDefault();
            }
        }
        public static void BulkInsertList(string tableName, DataTable data)
        {
            using (var connection = new SqlConnection(SIDALConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = tableName;// "dbo.TicketDetails";
                    try
                    {
                        bulkCopy.WriteToServer(data);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        connection.Close();
                    }
                }

                transaction.Commit();
            }
        }

        #region Threaded Chat

        /// <summary>
        /// Gets the chat conversation id based on the project and quote combination, if no combination found, creates a new Chat conversation with Chat Subscriptions
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="quoteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Guid GetChatConversationId(int projectId, long? quoteId = null, Guid? userId = null)
        {
            using (var db = GetContext())
            {
                Guid chatConversationId = Guid.Empty;
                var chatConversation = db.ChatConversations.Where(x => x.ProjectId == projectId);

                if (quoteId != null)
                    chatConversation = chatConversation.Where(x => x.QuoteId == quoteId);
                else
                    chatConversation = chatConversation.Where(x => x.QuoteId == null);

                chatConversationId = chatConversation.Select(x => x.Id).FirstOrDefault();

                //if no existing ChatConversation is there create new ChatConversation for the Project and Quote combination
                if (chatConversationId == Guid.Empty)
                {
                    var newChatConversation = new ChatConversation();
                    newChatConversation.ProjectId = projectId;
                    newChatConversation.QuoteId = quoteId;
                    newChatConversation.Id = Guid.NewGuid();
                    newChatConversation.CreatedByUserId = userId;
                    newChatConversation.CreatedAt = DateTime.Now;

                    db.ChatConversations.InsertOnSubmit(newChatConversation);
                    db.SubmitChanges();

                    chatConversationId = newChatConversation.Id;

                    //Create Chat Subscriptions
                    AddChatSubscriptions(chatConversationId, userId);
                }

                return chatConversationId;
            }
        }

        #region Add Chat Subscribers

        public static void AddChatSubscriptions(Guid chatConversationId, Guid? userId = null)
        {
            using (var db = GetContext())
            {
                var chatConversation = db.ChatConversations.FirstOrDefault(x => x.Id == chatConversationId);
                var exChatSubscriptions = db.ChatSubscriptions.Where(x => x.ChatConversationId == chatConversationId).ToList();

                if (chatConversation != null)
                {
                    //Add Chat Subscribers
                    //1. Logged in user
                    if (userId != null && userId.GetValueOrDefault() != Guid.Empty)
                    {
                        //Add if not already exists
                        if (exChatSubscriptions.Count(x => x.UserId == userId.GetValueOrDefault()) == 0)
                        {
                            ChatSubscription chatSub = new ChatSubscription();

                            chatSub.UserId = userId.GetValueOrDefault();
                            chatSub.ChatConversationId = chatConversationId;
                            chatSub.CanBeRemoved = false;
                            chatSub.CreatedAt = DateTime.Now;

                            db.ChatSubscriptions.InsertOnSubmit(chatSub);
                            db.SubmitChanges();
                        }
                    }

                    //Check if it's Project Specific thread
                    if (chatConversation.QuoteId == null || chatConversation.QuoteId < 0)
                    {

                    }
                    else //Check if it's Quote specific thread
                    {
                        var quote = db.Quotations.FirstOrDefault(x => x.Id == chatConversation.QuoteId.GetValueOrDefault());

                        if (quote != null)
                        {
                            //Check if user already has subscription
                            if (exChatSubscriptions.Count(x => x.UserId == quote.UserId) == 0)
                            {
                                ChatSubscription chatSub = new ChatSubscription();

                                chatSub.UserId = quote.UserId;
                                chatSub.ChatConversationId = chatConversationId;
                                chatSub.CanBeRemoved = false;

                                db.ChatSubscriptions.InsertOnSubmit(chatSub);
                                db.SubmitChanges();
                            }

                            if ("APPROVED".Equals(quote.Status) && quote.ApprovedBy != null)
                            {

                                if (exChatSubscriptions.Count(x => x.UserId == quote.ApprovedBy) == 0)
                                {
                                    ChatSubscription chatSub = new ChatSubscription();

                                    chatSub.UserId = quote.ApprovedBy.GetValueOrDefault();
                                    chatSub.ChatConversationId = chatConversationId;
                                    chatSub.CanBeRemoved = false;

                                    db.ChatSubscriptions.InsertOnSubmit(chatSub);
                                    db.SubmitChanges();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        public static List<ChatMessageModel> GetChatMessages(Guid chatConversationId, Guid? lastId = null)
        {
            using (var context = GetContext())
            {
                var messages = context.ChatMessages.Where(x => x.ChatConversationId == chatConversationId);

                if (lastId != null)
                {
                    var lastMessageTime = context.ChatMessages
                                                 .Where(x => x.ChatConversationId == chatConversationId)
                                                 .Where(x => x.Id == lastId)
                                                 .Select(x => x.CreatedAt)
                                                 .FirstOrDefault();
                    messages = messages.Where(x => x.CreatedAt > lastMessageTime);
                }

                return messages.Select(x => new ChatMessageModel
                {
                    Id = x.Id,
                    UserName = x.CompanyUser.aspnet_User.UserName,
                    CompanyUserId = x.CompanyUserId,
                    Message = x.Message,
                    ChatConversationId = x.ChatConversationId,
                    CreatedAt = x.CreatedAt.GetValueOrDefault()
                }).OrderBy(x => x.CreatedAt).ToList();
            }
        }

        public static ChatMessageModel AddChatMessage(Guid chatConversationId, Guid userId, string message)
        {
            using (var context = GetContext())
            {
                var chatMessage = new ChatMessage();

                chatMessage.CompanyUserId = context.CompanyUsers.Where(x => x.UserId == userId).Select(x => x.CompanyUserId).FirstOrDefault();
                chatMessage.Id = Guid.NewGuid();
                chatMessage.ChatConversationId = chatConversationId;
                chatMessage.CreatedAt = DateTime.Now;
                chatMessage.Message = message;

                context.ChatMessages.InsertOnSubmit(chatMessage);
                context.SubmitChanges();

                return new ChatMessageModel(chatMessage, context.aspnet_Users.Where(x => x.UserId == userId).Select(x => x.UserName).FirstOrDefault());
            }
        }

        public static List<ChatSubscriber> GetChatSubscribers(Guid conversationId)
        {
            using (var db = GetContext())
            {
                return db.ChatSubscriptions
                         .Where(x => x.ChatConversationId == conversationId)
                         .Select(x => new ChatSubscriber
                         {
                             Id = x.Id,
                             UserId = x.UserId,
                             Name = db.CompanyUsers.Where(y => y.UserId == x.UserId).Select(cu => cu.Name).FirstOrDefault() ?? x.aspnet_Membership.aspnet_User.UserName,
                             Email = x.aspnet_Membership.Email,
                             CanBeRemoved = x.CanBeRemoved.GetValueOrDefault()
                         }).ToList();
            }
        }

        public static void SendChatNotification(Guid chatConversationId, Guid userId, string message)
        {
            using (var db = GetContext())
            {
                var chatSubscriptions = db.ChatSubscriptions.Where(x => x.ChatConversationId == chatConversationId).ToList();

                if (chatSubscriptions != null)
                {
                    ChatNotification chatNotif = null;
                    foreach (var chatSub in chatSubscriptions)
                    {
                        if (chatSub.UserId == userId)
                            continue;

                        chatNotif = new ChatNotification();
                        chatNotif.Id = Guid.NewGuid();
                        chatNotif.ChatConversationId = chatConversationId;
                        chatNotif.CreatedAt = DateTime.Now;
                        chatNotif.IsSeen = false;
                        chatNotif.Message = message;
                        chatNotif.MessageByUserId = userId;
                        chatNotif.UserId = chatSub.UserId;

                        db.ChatNotifications.InsertOnSubmit(chatNotif);
                        db.SubmitChanges();
                    }
                }
            }
        }

        public static ChatNotificationModel GetChatNotifications(Guid userId, DateTime? dateTimeFrom = null)
        {
            using (var db = GetContext())
            {
                //var users = GetUsers();

                var notifQuery = db.ChatNotifications
                                   .Where(x => x.UserId == userId);

                if (dateTimeFrom != null)
                {
                    notifQuery = notifQuery.Where(x => x.CreatedAt > dateTimeFrom.GetValueOrDefault());
                }

                var c = new ChatNotificationModel
                {
                    ChatNotifications = notifQuery.Select(ch => new ChatNotificationItem()
                    {
                        Id = ch.Id,
                        ConversationId = ch.ChatConversationId,
                        UserId = ch.UserId,
                        MessageByUserId = ch.MessageByUserId,
                        UserName = db.aspnet_Users.Where(u => u.UserId == ch.MessageByUserId).Select(user => user.UserName).FirstOrDefault(),
                        Message = ch.Message,
                        ProjectId = ch.ChatConversation.ProjectId,
                        QuoteId = ch.ChatConversation.QuoteId,
                        CreatedAt = ch.CreatedAt.GetValueOrDefault(),
                        IsSeen = ch.IsSeen.GetValueOrDefault(),
                        SeenAt = ch.SeenAt
                    })
                                                            .OrderBy(o => o.CreatedAt)
                                                            .ToList(),
                    UnseenCount = db.ChatNotifications.Count(x => x.UserId == userId && x.IsSeen == false)
                };
                return c;
            }
        }

        public static void AddChatSubscriptions(Guid chatConversationId, List<Guid> users)
        {
            using (var context = GetContext())
            {
                if (users != null)
                {
                    var chatSubscriptions = context.ChatSubscriptions.Where(x => x.ChatConversationId == chatConversationId).Select(x => x.UserId);
                    ChatSubscription cSub = null;
                    foreach (var u in users)
                    {
                        if (!chatSubscriptions.Contains(u))
                        {
                            cSub = new ChatSubscription();
                            cSub.ChatConversationId = chatConversationId;
                            cSub.UserId = u;
                            cSub.CanBeRemoved = true;
                            cSub.CreatedAt = DateTime.Now;

                            context.ChatSubscriptions.InsertOnSubmit(cSub);
                            context.SubmitChanges();
                        }
                    }
                }
            }
        }

        public static void RemoveChatSubscriptions(List<long> subscriptions)
        {
            using (var context = GetContext())
            {
                if (subscriptions != null)
                {
                    foreach (var u in subscriptions)
                    {
                        var csToDelete = context.ChatSubscriptions
                                                .FirstOrDefault(x => x.Id == u);

                        if (csToDelete != null)
                        {
                            context.ChatSubscriptions.DeleteOnSubmit(csToDelete);
                            context.SubmitChanges();
                        }
                    }
                }
            }
        }

        public static List<SIUser> GetUsersFromSameDistrict(Guid userId)
        {
            using (var context = GetContext())
            {
                var usersDistricts = context.DistrictUsers.Where(x => x.UserId == userId).Select(x => x.DistrictId).ToList();
                var users = context.DistrictUsers
                                   .Where(x => usersDistricts.Contains(x.DistrictId))
                                   .Select(x => x.aspnet_Membership.aspnet_User);
                //TODO: Threaded Chat - Add Logic to fetch users matching the district for userId
                return users.Select(x => new SIUser
                {
                    UserId = x.UserId,
                    Name = x.CompanyUsers.FirstOrDefault().Name,
                    Username = x.UserName,
                    Email = x.aspnet_Membership.Email
                })
                .Distinct()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Username)
                .ToList();
            }
        }

        /// <summary>
        /// Accepts a list of notifications to be deleted and deletes them from the database
        /// </summary>
        /// <param name="notificationsToDelete">List of Guid of notifications</param>
        public static void DeleteChatNotifications(List<Guid> notificationsToDelete)
        {
            if (notificationsToDelete != null && notificationsToDelete.Count > 0)
            {
                using (var context = GetContext())
                {
                    foreach (var notificationId in notificationsToDelete)
                    {
                        var chatNotification = context.ChatNotifications.FirstOrDefault(x => x.Id == notificationId);

                        if (chatNotification != null)
                        {
                            context.ChatNotifications.DeleteOnSubmit(chatNotification);
                            context.SubmitChanges();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Toggles the Chat Notificaition setting to enable or disable the notifications
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        public static void ToggleNotifications(Guid userId, bool enabled)
        {
            using (var context = GetContext())
            {
                var companyUser = context.CompanyUsers.FirstOrDefault(x => x.UserId == userId);

                if (companyUser != null)
                {
                    companyUser.ReceiveNotifications = enabled;
                    context.SubmitChanges();
                }

                if (!enabled)
                {
                    var subscriptions = context.ChatSubscriptions.Where(x => x.UserId == userId);

                    string userName = ((companyUser != null && companyUser.Name != null && companyUser.Name != "") ? companyUser.Name : companyUser.aspnet_User.UserName);

                    ChatMessage chatMessage = null;
                    foreach (var subscription in subscriptions)
                    {
                        chatMessage = new ChatMessage();
                        chatMessage.ChatConversationId = subscription.ChatConversationId;
                        chatMessage.CompanyUserId = null;
                        chatMessage.Id = Guid.NewGuid();
                        chatMessage.Message = string.Format("{0} has left the chat", userName);
                        chatMessage.CreatedAt = DateTime.Now;

                        context.ChatMessages.InsertOnSubmit(chatMessage);
                        context.SubmitChanges();

                        context.ChatSubscriptions.DeleteOnSubmit(subscription);
                        context.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the user has enabled the notification setting or not
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool ChatNotificationsEnabled(string userId)
        {
            using (var context = GetContext())
            {
                bool? notifEnabled = context.CompanyUsers.Where(x => x.UserId.ToString() == userId).Select(x => x.ReceiveNotifications).FirstOrDefault();

                return notifEnabled.GetValueOrDefault();
            }
        }

        public static void UpdateChatNotificationReadStatus(List<Guid> notificationId)
        {
            using (var context = GetContext())
            {
                foreach (var Id in notificationId)
                {
                    var chatNot = context.ChatNotifications.Where(x => x.Id == Id).FirstOrDefault();
                    chatNot.IsSeen = true;
                    context.SubmitChanges();
                }
            }
        }

        #endregion

        public static bool IsApiEnabled()
        {
            using (var db = GetContext())
            {
                var cs = db.CompanySettings.FirstOrDefault();

                return cs != null ? cs.EnableAPI.GetValueOrDefault() : false;
            }
        }

        public static void UpdateDefaultQuoteProducts(Guid userId, string[] defaultProductIds)
        {
            using (var db = GetContext())
            {
                var existingDefaults = db.DefaultQuoteProducts.Where(x => x.UserId == userId);

                if (existingDefaults != null)
                {
                    db.DefaultQuoteProducts.DeleteAllOnSubmit(existingDefaults);
                    db.SubmitChanges();
                }

                if (defaultProductIds != null)
                {
                    DefaultQuoteProduct defQuoteProd = null;
                    foreach (var item in defaultProductIds)
                    {
                        defQuoteProd = new DefaultQuoteProduct();
                        defQuoteProd.UserId = userId;
                        defQuoteProd.ProductTypeId = Convert.ToInt32(item);
                        defQuoteProd.CreatedAt = DateTime.Now;

                        db.DefaultQuoteProducts.InsertOnSubmit(defQuoteProd);
                    }

                    db.SubmitChanges();
                }
            }
        }

        public static List<DefaultQuoteProduct> GetDefaultQuoteProducts(string userId)
        {
            using (var db = GetContext())
            {
                return db.DefaultQuoteProducts.Where(x => x.UserId.GetValueOrDefault().ToString() == userId).ToList();
            }
        }

        public static Uom FindUOMByDispatchId(string dispatchId)
        {
            using (var db = GetContext())
            {
                return db.Uoms.FirstOrDefault(x => x.DispatchId == dispatchId);
            }
        }

        #region Block and Agg Products

        public static void AddUpdateAggregateProduct(AggregateProduct aggProduct)
        {
            using (var context = GetContext())
            {
                if (aggProduct.Id == 0)
                {
                    context.AggregateProducts.InsertOnSubmit(aggProduct);
                }
                else
                {
                    context.AggregateProducts.Attach(aggProduct);
                    context.Refresh(RefreshMode.KeepCurrentValues, aggProduct);
                }
                context.SubmitChanges();
            }
        }

        public static void AddUpdateBlockProduct(BlockProduct blockProduct)
        {
            using (var context = GetContext())
            {
                if (blockProduct.Id == 0)
                {
                    context.BlockProducts.InsertOnSubmit(blockProduct);
                }
                else
                {
                    context.BlockProducts.Attach(blockProduct);
                    context.Refresh(RefreshMode.KeepCurrentValues, blockProduct);
                }
                context.SubmitChanges();
            }
        }

        public static AggregateProduct FindAggregateProduct(long productId)
        {
            using (var context = GetContext())
            {
                return context.AggregateProducts.Where(x => x.Id == productId).FirstOrDefault();
            }
        }

        public static List<BlockProduct> GetBlockProducts(bool active = false)
        {
            using (var db = GetContext())
            {
                var blockProductList = db.BlockProducts.ToList();
                if (active)
                {
                    blockProductList = blockProductList.Where(x => x.Active == true).ToList();
                }
                return blockProductList;
            }
        }

        public static List<AggregateProduct> GetAggregateProducts(bool active = false)
        {
            using (var db = GetContext())
            {
                var aggProductList = db.AggregateProducts.ToList();
                if (active)
                {
                    aggProductList = aggProductList.Where(x => x.Active == true).ToList();
                }
                return aggProductList;
            }
        }

        public static List<AggregateProductPriceProjection> GetAllAggregateProductPriceProjections()
        {
            using (var context = GetContext())
            {
                return context.AggregateProductPriceProjections.ToList();
            }
        }

        public static List<BlockProductPriceProjection> GetAllBlockProductPriceProjections()
        {
            using (var context = GetContext())
            {
                return context.BlockProductPriceProjections.ToList();
            }
        }

        /// <summary>
        /// Add Quotation Aggregate
        /// </summary>
        /// <param name="quoteAgg"></param>
        public static void AddQuotationAggregate(QuotationAggregate quoteAgg)
        {
            using (var context = GetContext())
            {
                if (quoteAgg.Id == 0)
                    context.QuotationAggregates.InsertOnSubmit(quoteAgg);
                else
                {
                    context.QuotationAggregates.Attach(quoteAgg);
                    context.Refresh(RefreshMode.KeepCurrentValues, quoteAgg);
                }
                context.SubmitChanges();
            }
        }

        public static List<QuotationAggregate> GetQuotationAggregate(long quoteId)
        {
            using (var db = GetContext())
            {
                return db.QuotationAggregates.Where(x => x.QuotationId == quoteId).ToList();
            }
        }

        public static void UpdateQuotationAggregateFields(QuotationAggregate quoteAgg)
        {
            QuotationAggregate quoteAggregate = new QuotationAggregate();
            using (var context = GetContext())
            {
                quoteAggregate = context.QuotationAggregates.Where(x => x.Id == quoteAgg.Id).FirstOrDefault();
                quoteAggregate.Volume = quoteAgg.Volume;
                quoteAggregate.QuotedDescription = quoteAgg.QuotedDescription;
                quoteAggregate.Price = quoteAgg.Price;
                quoteAggregate.Freight = quoteAgg.Freight;
                quoteAggregate.PublicNotes = quoteAgg.PublicNotes;
                quoteAggregate.TotalPrice = quoteAgg.TotalPrice;
                quoteAggregate.TotalRevenue = quoteAgg.TotalRevenue;
                context.SubmitChanges();
            }
        }

        public static List<QuotationBlock> GetQuotationBlock(long quoteId)
        {
            using (var db = GetContext())
            {
                return db.QuotationBlocks.Where(x => x.QuotationId == quoteId).ToList();
            }
        }

        public static BlockProduct FindBlockProduct(long productId)
        {
            using (var context = GetContext())
            {
                return context.BlockProducts.Where(x => x.Id == productId).FirstOrDefault();
            }
        }

        public static void AddQuotationBlock(QuotationBlock quoteBlock)
        {
            using (var context = GetContext())
            {
                if (quoteBlock.Id == 0)
                    context.QuotationBlocks.InsertOnSubmit(quoteBlock);
                else
                {
                    context.QuotationBlocks.Attach(quoteBlock);
                    context.Refresh(RefreshMode.KeepCurrentValues, quoteBlock);
                }
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Update Quotation Block Fields
        /// </summary>
        /// <param name="quoteBlock"></param>
        public static void UpdateQuotationBlockFields(QuotationBlock quoteBlock)
        {
            QuotationBlock quotationBlock = new QuotationBlock();
            using (var context = GetContext())
            {
                quotationBlock = context.QuotationBlocks.Where(x => x.Id == quoteBlock.Id).FirstOrDefault();
                quotationBlock.Volume = quoteBlock.Volume;
                quotationBlock.QuotedDescription = quoteBlock.QuotedDescription;
                quotationBlock.Price = quoteBlock.Price;
                quotationBlock.Freight = quoteBlock.Freight;
                quotationBlock.PublicNotes = quoteBlock.PublicNotes;
                quotationBlock.TotalPrice = quoteBlock.TotalPrice;
                quotationBlock.TotalRevenue = quoteBlock.TotalRevenue;
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Update Quotation ConcreteEnabled, AggregateEnabled, BlockEnabled
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="quoteId"></param>
        public static void UpdateQuoteProductType(string productType, long quoteId)
        {
            using (var db = GetContext())
            {
                var quote = db.Quotations.Where(x => x.Id == quoteId).FirstOrDefault();
                if (quote != null)
                {
                    switch (productType)
                    {
                        case "concrete":
                            quote.ConcreteEnabled = !quote.ConcreteEnabled.GetValueOrDefault(false);
                            break;
                        case "aggregate":
                            quote.AggregateEnabled = !quote.AggregateEnabled.GetValueOrDefault(false);
                            break;
                        case "block":
                            quote.BlockEnabled = !quote.BlockEnabled.GetValueOrDefault(false);
                            break;
                        default:
                            break;
                    }
                    db.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Delete Quotation Aggregate
        /// </summary>
        /// <param name="quoteAggregateId"></param>
        public static void DeleteQuotationAggregate(long quoteAggregateId)
        {
            using (var context = GetContext())
            {
                QuotationAggregate quoteAgg = context.QuotationAggregates.FirstOrDefault(x => x.Id == quoteAggregateId);

                if (quoteAgg != null)
                {
                    context.QuotationAggregates.DeleteOnSubmit(quoteAgg);
                    context.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Delete Quotation Block
        /// </summary>
        /// <param name="quoteAggregateId"></param>
        public static void DeleteQuotationBlock(long quoteBlockId)
        {
            using (var context = GetContext())
            {
                QuotationBlock quoteBlock = context.QuotationBlocks.FirstOrDefault(x => x.Id == quoteBlockId);
                if (quoteBlock != null)
                {
                    context.QuotationBlocks.DeleteOnSubmit(quoteBlock);
                    context.SubmitChanges();
                }
            }
        }

        public static List<QuotationAggregate> GetQuotationAggregateProducts(long quoteId)
        {
            using (var context = GetContext())
            {
                return context.QuotationAggregates.Where(x => x.QuotationId == quoteId).ToList();
            }
        }

        public static List<QuotationBlock> GetQuotationBlockProducts(long quoteId)
        {
            using (var context = GetContext())
            {
                return context.QuotationBlocks.Where(x => x.QuotationId == quoteId).ToList();
            }
        }

        /// <summary>
        /// Update Quotation Aggregate PlantId as well as fill QuotationAggregateAddon based on plant's District
        /// </summary>
        /// <param name="plantId"></param>
        /// <param name="quoteId"></param>
        /// <returns>List<QuotationAggregateAddon></returns>
        public static List<QuotationAggregateAddon> UpdateQuotationAggregatePlant(int plantId, long quoteId)
        {
            List<QuotationAggregateAddon> aggAddonList = new List<QuotationAggregateAddon>();
            using (var db = GetContext())
            {
                try
                {
                    var quote = db.Quotations.FirstOrDefault(x => x.Id == quoteId);
                    if (quote != null)
                    {
                        quote.AggregatePlantId = plantId;
                        if (plantId == 0)
                        {
                            quote.AggregatePlantId = null;
                        }
                    }
                    db.SubmitChanges();
                    if (plantId != 0)
                    {
                        var districtId = db.Plants.Where(x => x.PlantId == plantId).Select(x => x.DistrictId).FirstOrDefault();
                        var addonIds = db.DistrictAddonDefaults.Where(x => x.DistrictId == districtId && x.IsAggregateDefault == true).ToList();

                        foreach (var item in addonIds)
                        {
                            var quoteAggAddon = db.QuotationAggregateAddons.Where(x => x.AddonId == item.AddonId && x.QuotationId == quoteId).FirstOrDefault();
                            QuotationAggregateAddon aggAddon = new QuotationAggregateAddon();
                            if (quoteAggAddon == null)
                            {
                                var addon = db.Addons.Where(x => x.Id == item.AddonId).FirstOrDefault();
                                var price = SIDAL.FindCurrentAddonQuoteCost(addon.Id, districtId, quote.PricingMonth);
                                if (price != 0)
                                {
                                    aggAddon.AddonId = addon.Id;
                                    aggAddon.Description = addon.Description;
                                    aggAddon.QuotationId = quoteId;
                                    aggAddon.Price = price;
                                    db.QuotationAggregateAddons.InsertOnSubmit(aggAddon);
                                    db.SubmitChanges();
                                    aggAddonList.Add(aggAddon);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                return aggAddonList;
            }
        }

        /// <summary>
        /// Update Quotation Block Plant
        /// </summary>
        /// <param name="plantId"></param>
        /// <param name="quoteId"></param>
        /// <returns>List<QuotationBlockAddon></returns>
        public static List<QuotationBlockAddon> UpdateQuotationBlockPlant(int plantId, long quoteId)
        {
            List<QuotationBlockAddon> blockAddonList = new List<QuotationBlockAddon>();
            using (var db = GetContext())
            {
                try
                {
                    var quote = db.Quotations.FirstOrDefault(x => x.Id == quoteId);
                    if (quote != null)
                    {
                        quote.BlockPlantId = plantId;
                        if (plantId == 0)
                        {
                            quote.BlockPlantId = null;
                        }

                    }
                    db.SubmitChanges();
                    if (plantId != 0)
                    {
                        var districtId = db.Plants.Where(x => x.PlantId == plantId).Select(x => x.DistrictId).FirstOrDefault();
                        var addonIds = db.DistrictAddonDefaults.Where(x => x.DistrictId == districtId && x.IsBlockDefault == true).ToList();

                        foreach (var item in addonIds)
                        {
                            var quoteBlockAddon = db.QuotationBlockAddons.Where(x => x.AddonId == item.AddonId && x.QuotationId == quoteId).FirstOrDefault();
                            QuotationBlockAddon blockAddon = new QuotationBlockAddon();
                            if (quoteBlockAddon == null)
                            {
                                var addon = db.Addons.Where(x => x.Id == item.AddonId).FirstOrDefault();
                                var price = SIDAL.FindCurrentAddonQuoteCost(addon.Id, districtId, quote.PricingMonth);
                                if (price != 0)
                                {
                                    blockAddon.AddonId = addon.Id;
                                    blockAddon.Description = addon.Description;
                                    blockAddon.QuotationId = quoteId;
                                    blockAddon.Price = price;
                                    db.QuotationBlockAddons.InsertOnSubmit(blockAddon);
                                    db.SubmitChanges();
                                    blockAddonList.Add(blockAddon);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                return blockAddonList;
            }
        }

        /// <summary>
        /// Update Aggregate Product Plant Price
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="plantId"></param>
        /// <returns></returns>
        public static decimal UpdateAggregateProductPlantPrice(long productId, long quoteId)
        {
            decimal plantPrice = 0;

            using (var db = GetContext())
            {
                var plantId = db.Quotations.Where(x => x.Id == quoteId).Select(x => x.AggregatePlantId).FirstOrDefault();
                if (plantId != 0)
                {
                    var aggPriceProjection = db.AggregateProductPriceProjections.Where(x => x.AggregateProductId == productId).ToList();
                    if (aggPriceProjection != null)
                    {
                        var aggPP = aggPriceProjection.Where(x => x.PlantId == plantId & x.ChangeDate == DateUtils.GetFirstOf(DateTime.Now)).FirstOrDefault();
                        if (aggPP != null)
                        {
                            plantPrice = aggPP.Price.GetValueOrDefault(0);
                        }
                        else
                        {
                            var maxProductPrice = aggPriceProjection.OrderByDescending(x => x.Price).FirstOrDefault();
                            if (maxProductPrice != null)
                            {
                                plantPrice = maxProductPrice.Price.GetValueOrDefault(0);
                            }
                        }
                    }
                }
            }
            return plantPrice;
        }

        /// <summary>
        /// Update Block Product Plant Price
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="plantId"></param>
        /// <returns></returns>
        public static decimal UpdateBlockProductPlantPrice(long productId, long quoteId)
        {
            decimal plantPrice = 0;

            using (var db = GetContext())
            {
                var plantId = db.Quotations.Where(x => x.Id == quoteId).Select(x => x.BlockPlantId).FirstOrDefault();
                if (plantId != 0)
                {
                    var blockPriceProjection = db.BlockProductPriceProjections.Where(x => x.BlockProductId == productId).ToList();
                    if (blockPriceProjection != null)
                    {
                        var blockPP = blockPriceProjection.Where(x => x.PlantId == plantId & x.ChangeDate == DateUtils.GetFirstOf(DateTime.Now)).FirstOrDefault();
                        if (blockPP != null)
                        {
                            plantPrice = blockPP.Price.GetValueOrDefault(0);
                        }
                        else
                        {
                            var maxProductPrice = blockPriceProjection.OrderByDescending(x => x.Price).FirstOrDefault();
                            if (maxProductPrice != null)
                            {
                                plantPrice = maxProductPrice.Price.GetValueOrDefault(0);
                            }
                        }
                    }
                }
            }
            return plantPrice;
        }

        public static string GetQuotationPlantsName(long quoteId)
        {
            using (var db = GetContext())
            {
                DataLoadOptions opts = new DataLoadOptions();
                opts.LoadWith<Quotation>(x => x.Plant);
                opts.LoadWith<Quotation>(x => x.AggregatePlant);
                opts.LoadWith<Quotation>(x => x.BlockPlant);
                opts.LoadWith<Quotation>(x => x.Customer);

                db.LoadOptions = opts;

                var quote = db.Quotations.FirstOrDefault(x => x.Id == quoteId);
                var company = db.Companies.FirstOrDefault();

                var concreteEnabled = quote.Customer.PurchaseConcrete.GetValueOrDefault() & quote.ConcreteEnabled.GetValueOrDefault();
                var aggregateEnabled = company.EnableAggregate.GetValueOrDefault() & quote.AggregateEnabled.GetValueOrDefault() & quote.Customer.PurchaseAggregate.GetValueOrDefault();
                var blockEnabled = company.EnableBlock.GetValueOrDefault() & quote.BlockEnabled.GetValueOrDefault() & quote.Customer.PurchaseBlock.GetValueOrDefault();

                return GetFormattedPlantNameForQuote(quote, concreteEnabled, aggregateEnabled, blockEnabled);
            }
        }

        public static string GetFormattedPlantNameForQuote(Quotation quote, bool concreteEnabled, bool aggregateEnabled, bool blockEnabled)
        {
            List<string> plants = new List<string>();
            if (quote != null)
            {
                if (concreteEnabled && quote.ConcreteEnabled.GetValueOrDefault() && quote.Plant != null)
                {
                    plants.Add(quote.Plant.Name);
                }

                if (aggregateEnabled && quote.AggregateEnabled.GetValueOrDefault() && quote.AggregatePlant != null)
                {
                    plants.Add(quote.AggregatePlant.Name);
                }

                if (blockEnabled && quote.BlockEnabled.GetValueOrDefault() && quote.BlockPlant != null)
                {
                    plants.Add(quote.BlockPlant.Name);
                }
            }

            return String.Join(",", plants.ToArray());
        }

        public static QuotationAggregate ChangeQuotationAggregateProduct(long quoteId, long newProductId, long oldProductId)
        {
            QuotationAggregate quoteAgg = new QuotationAggregate();
            using (var db = GetContext())
            {
                quoteAgg = db.QuotationAggregates.FirstOrDefault(x => x.QuotationId == quoteId && x.AggregateProductId == oldProductId);
                if (quoteAgg != null)
                {
                    var aggProduct = db.AggregateProducts.FirstOrDefault(x => x.Id == newProductId);
                    quoteAgg.AggregateProductId = aggProduct.Id;
                    quoteAgg.QuotedDescription = aggProduct.Description;
                    quoteAgg.Price = SIDAL.UpdateAggregateProductPlantPrice(aggProduct.Id, quoteId);
                    quoteAgg.TotalPrice = quoteAgg.Freight + quoteAgg.Price;
                    quoteAgg.TotalRevenue = quoteAgg.TotalPrice * (decimal)quoteAgg.Volume;
                }
                db.SubmitChanges();
            }
            return quoteAgg;
        }

        public static QuotationBlock ChangeQuotationBlockProduct(long quoteId, long newProductId, long oldProductId)
        {
            QuotationBlock quoteBlock = new QuotationBlock();
            using (var db = GetContext())
            {
                quoteBlock = db.QuotationBlocks.FirstOrDefault(x => x.QuotationId == quoteId && x.BlockProductId == oldProductId);
                if (quoteBlock != null)
                {
                    var blockProduct = db.BlockProducts.FirstOrDefault(x => x.Id == newProductId);
                    quoteBlock.BlockProductId = blockProduct.Id;
                    quoteBlock.QuotedDescription = blockProduct.Description;
                    quoteBlock.Price = SIDAL.UpdateBlockProductPlantPrice(blockProduct.Id, quoteId);
                    quoteBlock.TotalPrice = quoteBlock.Freight + quoteBlock.Price;
                    quoteBlock.TotalRevenue = quoteBlock.TotalPrice * (decimal)quoteBlock.Volume;
                }
                db.SubmitChanges();
            }
            return quoteBlock;
        }

        public static int GetQuotationConcretePlantId(int plantId, int projectId)
        {
            int concretePlantId = 0;
            using (var db = GetContext())
            {
                var plant = db.Plants.Where(x => x.PlantId == plantId).FirstOrDefault();
                if (plant.ProductTypeId != (int)ProductType.Concrete)
                {
                    var projectPlantIdsList = db.ProjectPlants.Where(x => x.ProjectId == projectId).Select(x => x.PlantId).ToList();
                    List<int> districtIdsList = new List<int>();
                    foreach (var item in projectPlantIdsList)
                    {
                        var projectPlant = db.Plants.Where(x => x.PlantId == item).FirstOrDefault();
                        districtIdsList.Add(projectPlant.DistrictId);

                        if (projectPlant.ProductTypeId == (int)ProductType.Concrete)
                        {
                            concretePlantId = projectPlant.PlantId;
                        }
                    }
                    if (concretePlantId == 0)
                    {
                        foreach (var item in districtIdsList)
                        {
                            var plants = db.Plants.Where(x => x.DistrictId == item && x.ProductTypeId == (int)ProductType.Concrete).ToList();
                            if (plants != null)
                            {
                                concretePlantId = plants.Select(x => x.PlantId).FirstOrDefault();
                            }
                        }
                    }
                }
                else
                {
                    concretePlantId = plantId;
                }
            }
            return concretePlantId;
        }

        public static string GetPlantProductType(int plantId)
        {
            var plantProductType = GetCompany().DeliveryQtyUomPlural;
            var productTypeId = GetPlant(plantId).ProductTypeId;
            if (productTypeId == (int)ProductType.Aggregate)
            {
                plantProductType = "Tons";
            }
            else if (productTypeId == (int)ProductType.Block)
            {
                plantProductType = "Units";
            }
            return plantProductType;
        }

        public static List<QuotationAggregateAddon> GetQuotationAggregateAddon(long quoteId, bool isIncludeTable = false)
        {
            List<QuotationAggregateAddon> quoteAggAddons = new List<QuotationAggregateAddon>();

            using (var db = GetContext())
            {
                DataLoadOptions loadOption = new DataLoadOptions();
                loadOption.LoadWith<QuotationAggregateAddon>(x => x.Addon);
                db.LoadOptions = loadOption;

                quoteAggAddons = db.QuotationAggregateAddons.Where(x => x.QuotationId == quoteId).ToList();
                if (isIncludeTable)
                {
                    quoteAggAddons = quoteAggAddons.Where(x => x.IsIncludeTable == true).ToList();
                }
            }
            return quoteAggAddons;
        }

        public static List<QuotationBlockAddon> GetQuotationBlockAddon(long quoteId, bool isIncludeTable = false)
        {
            List<QuotationBlockAddon> quoteBlockAddons = new List<QuotationBlockAddon>();
            using (var db = GetContext())
            {
                DataLoadOptions loadOption = new DataLoadOptions();
                loadOption.LoadWith<QuotationBlockAddon>(x => x.Addon);
                db.LoadOptions = loadOption;

                quoteBlockAddons = db.QuotationBlockAddons.Where(x => x.QuotationId == quoteId).ToList();
                if (isIncludeTable)
                {
                    quoteBlockAddons = quoteBlockAddons.Where(x => x.IsIncludeTable == true).ToList();
                }
            }
            return quoteBlockAddons;
        }

        public static void UpdateQuotationAggregateAddonFields(QuotationAggregateAddon qaon)
        {
            QuotationAggregateAddon quoteAon = new QuotationAggregateAddon();
            using (var context = GetContext())
            {
                quoteAon = context.QuotationAggregateAddons.Where(x => x.Id == qaon.Id).FirstOrDefault();
                quoteAon.Description = qaon.Description;
                quoteAon.Price = qaon.Price;
                quoteAon.IsIncludeTable = qaon.IsIncludeTable;
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Update Quotation Block Addon Fields
        /// </summary>
        /// <param name="qaon"></param>
        public static void UpdateQuotationBlockAddonFields(QuotationBlockAddon qaon)
        {
            QuotationBlockAddon quoteAon = new QuotationBlockAddon();
            using (var context = GetContext())
            {
                quoteAon = context.QuotationBlockAddons.Where(x => x.Id == qaon.Id).FirstOrDefault();
                quoteAon.Description = qaon.Description;
                quoteAon.Price = qaon.Price;
                quoteAon.IsIncludeTable = qaon.IsIncludeTable;
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Add Quotation Aggregate AddOn
        /// </summary>
        /// <param name="qaon"></param>
        public static void AddQuotationAggregateAddOn(QuotationAggregateAddon qaon)
        {
            using (var context = GetContext())
            {
                if (qaon.Id == 0)
                    context.QuotationAggregateAddons.InsertOnSubmit(qaon);
                else
                {
                    context.QuotationAggregateAddons.Attach(qaon);
                    context.Refresh(RefreshMode.KeepCurrentValues, qaon);
                }
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Add Quotation Block AddOn
        /// </summary>
        /// <param name="qaon"></param>
        public static void AddQuotationBlockAddOn(QuotationBlockAddon qaon)
        {
            using (var context = GetContext())
            {
                if (qaon.Id == 0)
                    context.QuotationBlockAddons.InsertOnSubmit(qaon);
                else
                {
                    context.QuotationBlockAddons.Attach(qaon);
                    context.Refresh(RefreshMode.KeepCurrentValues, qaon);
                }
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// /Get the list of District Addon Default
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns>List<DistrictAddonDefault></returns>
        public static List<DistrictAddonDefault> GetAllDistrictAggregateAddons(long quoteID)
        {
            List<DistrictAddonDefault> defaultAggAddons = new List<DistrictAddonDefault>();
            List<DistrictAddonDefault> aggAddons = new List<DistrictAddonDefault>();
            using (var db = GetContext())
            {
                var quote = db.Quotations.Where(x => x.Id == quoteID).FirstOrDefault();
                if (quote.AggregatePlantId == null)
                {
                    defaultAggAddons = null;
                }
                else
                {
                    var districtId = db.Plants.Where(x => x.PlantId == quote.AggregatePlantId).Select(x => x.DistrictId).FirstOrDefault();
                    defaultAggAddons = db.DistrictAddonDefaults.Where(x => x.DistrictId == districtId).ToList();

                    foreach (var item in defaultAggAddons)
                    {
                        var activeAddons = db.Addons.Where(x => x.Id == item.AddonId && x.Active == true).FirstOrDefault();
                        if (activeAddons != null)
                        {
                            var price = SIDAL.FindCurrentAddonQuoteCost(item.AddonId.GetValueOrDefault(), districtId, quote.PricingMonth);
                            if (price != 0)
                            {
                                aggAddons.Add(item);
                            }
                        }
                    }
                }
            }
            return aggAddons;
        }

        /// <summary>
        /// Get All District Block Addons
        /// </summary>
        /// <param name="quoteID"></param>
        /// <returns> List<DistrictAddonDefault></returns>
        public static List<DistrictAddonDefault> GetAllDistrictBlockAddons(long quoteID)
        {
            List<DistrictAddonDefault> defaultBlockAddons = new List<DistrictAddonDefault>();
            List<DistrictAddonDefault> blockAddons = new List<DistrictAddonDefault>();
            using (var db = GetContext())
            {
                var quote = db.Quotations.Where(x => x.Id == quoteID).FirstOrDefault();
                if (quote.BlockPlantId == null)
                {
                    defaultBlockAddons = null;
                }
                else
                {
                    var districtId = db.Plants.Where(x => x.PlantId == quote.BlockPlantId).Select(x => x.DistrictId).FirstOrDefault();
                    defaultBlockAddons = db.DistrictAddonDefaults.Where(x => x.DistrictId == districtId).ToList();
                    foreach (var item in defaultBlockAddons)
                    {
                        var activeAddons = db.Addons.Where(x => x.Id == item.AddonId && x.Active == true).FirstOrDefault();
                        if (activeAddons != null)
                        {
                            var price = SIDAL.FindCurrentAddonQuoteCost(item.AddonId.GetValueOrDefault(), districtId, quote.PricingMonth);
                            if (price != 0)
                            {
                                blockAddons.Add(item);
                            }
                        }
                    }
                }
            }
            return blockAddons;
        }

        /// <summary>
        /// Delete Quotation Aggregate Addon
        /// </summary>
        /// <param name="quoteAggregateId"></param>
        public static void DeleteQuotationAggregateAddon(long quoteAggregateId)
        {
            using (var context = GetContext())
            {
                QuotationAggregateAddon quoteAgg = context.QuotationAggregateAddons.FirstOrDefault(x => x.Id == quoteAggregateId);

                if (quoteAgg != null)
                {
                    context.QuotationAggregateAddons.DeleteOnSubmit(quoteAgg);
                    context.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Delete Quotation Block Addon
        /// </summary>
        /// <param name="quoteBlockId"></param>
        public static void DeleteQuotationBlockAddon(long quoteBlockId)
        {
            using (var context = GetContext())
            {
                QuotationBlockAddon quoteBlock = context.QuotationBlockAddons.FirstOrDefault(x => x.Id == quoteBlockId);

                if (quoteBlock != null)
                {
                    context.QuotationBlockAddons.DeleteOnSubmit(quoteBlock);
                    context.SubmitChanges();
                }
            }
        }
        #endregion

        ///// <summary>
        ///// NOTE::Temprory Logger to be used under Senior developer prescription.
        ///// </summary>
        ///// <param name="message"></param>
        //public static void ErrorLogger(string message)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    sb.Append(message);
        //    System.IO.File.WriteAllText("C:\\Logs\\" + "log.txt", sb.ToString());
        //}

        #region QC Requirement
        public static List<QC_Requirement> GetQCRequirementList()
        {
            List<QC_Requirement> qcReqList = new List<QC_Requirement>();
            using (var db = GetContext())
            {
                qcReqList = db.QC_Requirements.Where(x => x.Active == true).ToList();
            }
            return qcReqList;
        }

        public static List<long> GetProjectQCRequirementList(int projectId)
        {
            List<long> projectQC = new List<long>();
            using (var db = GetContext())
            {
                projectQC = db.ProjectQCRequirements.Where(x => x.ProjectId == projectId).Select(x => x.QCRequirementId).ToList();
            }
            return projectQC;
        }


        //public static List<long> GetQuoteQCRequirementList(long quoteId)
        //{
        //    List<long> quoteQC = new List<long>();
        //    using (var db = GetContext())
        //    {
        //        quoteQC = db.QuoteQcRequirements.Where(x => x.QuoteId == quoteId).Select(x => x.QCRequirementId).ToList();
        //    }
        //    return quoteQC;
        //}

        public static void UpdateProjectQCRequirement(int projectId, List<long> qcReqId)
        {
            using (var db = GetContext())
            {
                var projQCReq = db.ProjectQCRequirements.Where(x => x.ProjectId == projectId);
                db.ProjectQCRequirements.DeleteAllOnSubmit(projQCReq);
                if (qcReqId != null && qcReqId.Count > 0)
                {
                    foreach (var item in qcReqId)
                    {
                        ProjectQCRequirement projQcReq = new ProjectQCRequirement();

                        projQcReq.ProjectId = projectId;
                        projQcReq.QCRequirementId = item;

                        db.ProjectQCRequirements.InsertOnSubmit(projQcReq);
                    }
                }
                db.SubmitChanges();
            }
        }

        //public static void UpdateQuoteQCRequirement(long quoteId, List<long> qcReqId)
        //{
        //    using (var db = GetContext())
        //    {
        //        var projQCReq = db.QuoteQcRequirements.Where(x => x.QuoteId == quoteId);
        //        db.QuoteQcRequirements.DeleteAllOnSubmit(projQCReq);
        //        if (qcReqId != null && qcReqId.Count > 0)
        //        {
        //            foreach (var item in qcReqId)
        //            {
        //                QuoteQcRequirement projQcReq = new QuoteQcRequirement();

        //                projQcReq.QuoteId = quoteId;
        //                projQcReq.QCRequirementId = item;

        //                db.QuoteQcRequirements.InsertOnSubmit(projQcReq);
        //            }
        //        }
        //        db.SubmitChanges();
        //    }
        //}
        #endregion

        /// <summary>
        /// Get Addon Sort Order
        /// </summary>
        /// <param name="addonId"></param>
        /// <returns></returns>
        public static decimal? GetAddonSortOrder(long addonId)
        {
            decimal? sortOrder = 0;
            using (var db = GetContext())
            {
                sortOrder = db.Addons.Where(x => x.Id == addonId).First().Sort;
            }
            return sortOrder;
        }

        public static string GetProjectQCRequirementNameList(int projectId)
        {
            var qcRequirementList = GetQCRequirementList();
            var projectQCRequirementList = GetProjectQCRequirementList(projectId);
            var qcReqFormatNameList = "";

            if (projectQCRequirementList.Count == 1)
            {
                qcReqFormatNameList = qcRequirementList.Where(x => projectQCRequirementList.Contains(x.Id)).Select(x => x.Name).First();
            }
            else
            {
                var qcReqNameList = qcRequirementList.Where(x => projectQCRequirementList.Contains(x.Id)).Select(x => x.Name).ToArray();
                var totalQcReq = qcReqNameList.Length;

                for (int i = 0; i < totalQcReq; i++)
                {
                    qcReqFormatNameList += i + 1 + "; " + qcReqNameList[i].ToString() + " ";
                }
            }
            return qcReqFormatNameList;
        }

        //public static string GetQuoteQCRequirementNameList(long quoteId)
        //{
        //    var qcRequirementList = GetQCRequirementList();
        //    var quoteQCRequirementList = GetQuoteQCRequirementList(quoteId);
        //    var qcReqFormatNameList = "";

        //    if (quoteQCRequirementList.Count == 1)
        //    {
        //        qcReqFormatNameList = qcRequirementList.Where(x => quoteQCRequirementList.Contains(x.Id)).Select(x => x.Name).First();
        //    }
        //    else
        //    {
        //        var qcReqNameList = qcRequirementList.Where(x => quoteQCRequirementList.Contains(x.Id)).Select(x => x.Name).ToArray();
        //        var totalQcReq = qcReqNameList.Length;

        //        for (int i = 0; i < totalQcReq; i++)
        //        {
        //            qcReqFormatNameList += i + 1 + "; " + qcReqNameList[i].ToString() + " ";
        //        }
        //    }
        //    return qcReqFormatNameList;
        //}
		      #region Ticket Detail

        public static void AddUpdateTicketDetail(TicketDetail ticketDetail)
        {
            using (var context = GetContext())
            {
                if (ticketDetail.Id == 0)
                {
                    TicketDetail obj = context.TicketDetails.FirstOrDefault(x => x.TicketId == ticketDetail.TicketId);
                    if (obj != null)
                    {
                        obj.BatchmanDescription = ticketDetail.BatchmanDescription;
                        obj.BatchmanNumber = ticketDetail.BatchmanNumber;
                        obj.CustomerCity = ticketDetail.CustomerCity;
                        obj.CustomerDescription = ticketDetail.CustomerDescription;
                        obj.CustomerNumber = ticketDetail.CustomerNumber;
                        obj.CustomerSegmentDesc = ticketDetail.CustomerSegmentDesc;
                        obj.CustomerSegmentNumber = ticketDetail.CustomerSegmentNumber;
                        obj.CustomerZip = ticketDetail.CustomerZip;
                        obj.DeliveredVolume = ticketDetail.DeliveredVolume;
                        obj.DeliveryAddress = ticketDetail.DeliveryAddress;
                        obj.DriveHomePlantNumber = ticketDetail.DriveHomePlantNumber;
                        obj.DriverDescription = ticketDetail.DriverDescription;
                        obj.DriverNumber = ticketDetail.DriverNumber;
                        obj.DriverType = ticketDetail.DriverType;
                        obj.FOB = ticketDetail.FOB;
                        obj.JobDescription = ticketDetail.JobDescription;
                        obj.JobNumber = ticketDetail.JobNumber;
                        obj.JobSegmentDescription = ticketDetail.JobSegmentDescription;
                        obj.JobSegmentNumber = ticketDetail.JobSegmentNumber;
                        obj.MaterialCost = ticketDetail.MaterialCost;
                        obj.PlantDescription = ticketDetail.PlantDescription;
                        obj.PlantNumber = ticketDetail.PlantNumber;
                        obj.SalesPersonDescription = ticketDetail.SalesPersonDescription;
                        obj.SalesPersonNumber = ticketDetail.SalesPersonNumber;
                        obj.TicketDate = ticketDetail.TicketDate;
                        obj.TicketId = ticketDetail.TicketId;
                        obj.TicketNumber = ticketDetail.TicketNumber;
                        obj.TimeArriveJob = ticketDetail.TimeArriveJob;
                        obj.TimeArrivePlant = ticketDetail.TimeArrivePlant;
                        obj.TimeBeginLoad = ticketDetail.TimeBeginLoad;
                        obj.TimeBeginUnload = ticketDetail.TimeBeginUnload;
                        obj.TimeDueOnJob = ticketDetail.TimeDueOnJob;
                        obj.TimeEndLoad = ticketDetail.TimeEndLoad;
                        obj.TimeEndUnload = ticketDetail.TimeEndUnload;
                        obj.TimeLeaveJob = ticketDetail.TimeLeaveJob;
                        obj.TimeLeavePlant = ticketDetail.TimeLeavePlant;
                        obj.TimeTicketed = ticketDetail.TimeTicketed;
                        obj.TotalRevenue = ticketDetail.TotalRevenue;
                        obj.TruckHomePlantNumber = ticketDetail.TruckHomePlantNumber;
                        obj.TruckNumber = ticketDetail.TruckNumber;
                        obj.TruckType = ticketDetail.TruckType;
                        obj.VoidStatus = ticketDetail.VoidStatus;
                    }
                    else
                    {
                        context.TicketDetails.InsertOnSubmit(ticketDetail);
                    }
                }
                else
                {
                    TicketDetail obj = context.TicketDetails.Where(x => x.Id == ticketDetail.Id).FirstOrDefault();

                    obj.BatchmanDescription = ticketDetail.BatchmanDescription;
                    obj.BatchmanNumber = ticketDetail.BatchmanNumber;
                    obj.CustomerCity = ticketDetail.CustomerCity;
                    obj.CustomerDescription = ticketDetail.CustomerDescription;
                    obj.CustomerNumber = ticketDetail.CustomerNumber;
                    obj.CustomerSegmentDesc = ticketDetail.CustomerSegmentDesc;
                    obj.CustomerSegmentNumber = ticketDetail.CustomerSegmentNumber;
                    obj.CustomerZip = ticketDetail.CustomerZip;
                    obj.DeliveredVolume = ticketDetail.DeliveredVolume;
                    obj.DeliveryAddress = ticketDetail.DeliveryAddress;
                    obj.DriveHomePlantNumber = ticketDetail.DriveHomePlantNumber;
                    obj.DriverDescription = ticketDetail.DriverDescription;
                    obj.DriverNumber = ticketDetail.DriverNumber;
                    obj.DriverType = ticketDetail.DriverType;
                    obj.FOB = ticketDetail.FOB;
                    obj.JobDescription = ticketDetail.JobDescription;
                    obj.JobNumber = ticketDetail.JobNumber;
                    obj.JobSegmentDescription = ticketDetail.JobSegmentDescription;
                    obj.JobSegmentNumber = ticketDetail.JobSegmentNumber;
                    obj.MaterialCost = ticketDetail.MaterialCost;
                    obj.PlantDescription = ticketDetail.PlantDescription;
                    obj.PlantNumber = ticketDetail.PlantNumber;
                    obj.SalesPersonDescription = ticketDetail.SalesPersonDescription;
                    obj.SalesPersonNumber = ticketDetail.SalesPersonNumber;
                    obj.TicketDate = ticketDetail.TicketDate;
                    obj.TicketId = ticketDetail.TicketId;
                    obj.TicketNumber = ticketDetail.TicketNumber;
                    obj.TimeArriveJob = ticketDetail.TimeArriveJob;
                    obj.TimeArrivePlant = ticketDetail.TimeArrivePlant;
                    obj.TimeBeginLoad = ticketDetail.TimeBeginLoad;
                    obj.TimeBeginUnload = ticketDetail.TimeBeginUnload;
                    obj.TimeDueOnJob = ticketDetail.TimeDueOnJob;
                    obj.TimeEndLoad = ticketDetail.TimeEndLoad;
                    obj.TimeEndUnload = ticketDetail.TimeEndUnload;
                    obj.TimeLeaveJob = ticketDetail.TimeLeaveJob;
                    obj.TimeLeavePlant = ticketDetail.TimeLeavePlant;
                    obj.TimeTicketed = ticketDetail.TimeTicketed;
                    obj.TotalRevenue = ticketDetail.TotalRevenue;
                    obj.TruckHomePlantNumber = ticketDetail.TruckHomePlantNumber;
                    obj.TruckNumber = ticketDetail.TruckNumber;
                    obj.TruckType = ticketDetail.TruckType;
                    obj.VoidStatus = ticketDetail.VoidStatus;
                }
                context.SubmitChanges();
            }
        }

        //public static void AddUpdateTicketDetailList(List<TicketDetail> ticketDetailList)
        //{
        //    using (var context = GetContext())
        //    {
        //        foreach (var ticketDetail in ticketDetailList)
        //        {
        //            if (ticketDetail.Id == 0)
        //            {
        //                TicketDetail obj = context.TicketDetails.FirstOrDefault(x => x.TicketId == ticketDetail.TicketId);
        //                if (obj != null)
        //                {
        //                    SetTicketDetailsToUpdate(ticketDetail, obj);
        //                }
        //                else
        //                {
        //                    context.TicketDetails.InsertOnSubmit(ticketDetail);
        //                }
        //            }
        //            else
        //            {
        //                TicketDetail obj = context.TicketDetails.FirstOrDefault(x => x.Id == ticketDetail.Id);
        //                SetTicketDetailsToUpdate(ticketDetail, obj);
        //            }
        //        }
        //        context.SubmitChanges();
        //    }
        //}
        public static void AddUpdateTicketDetailList(List<TicketDetail> ticketDetailList)
        {
            BulkInsertList("dbo.TicketDetails", ticketDetailList.AsDataTable());
        }

        private static void SetTicketDetailsToUpdate(TicketDetail ticketDetail, TicketDetail obj)
        {
            obj.BatchmanDescription = ticketDetail.BatchmanDescription;
            obj.BatchmanNumber = ticketDetail.BatchmanNumber;
            obj.CustomerCity = ticketDetail.CustomerCity;
            obj.CustomerDescription = ticketDetail.CustomerDescription;
            obj.CustomerNumber = ticketDetail.CustomerNumber;
            obj.CustomerSegmentDesc = ticketDetail.CustomerSegmentDesc;
            obj.CustomerSegmentNumber = ticketDetail.CustomerSegmentNumber;
            obj.CustomerZip = ticketDetail.CustomerZip;
            obj.DeliveredVolume = ticketDetail.DeliveredVolume;
            obj.DeliveryAddress = ticketDetail.DeliveryAddress;
            obj.DriveHomePlantNumber = ticketDetail.DriveHomePlantNumber;
            obj.DriverDescription = ticketDetail.DriverDescription;
            obj.DriverNumber = ticketDetail.DriverNumber;
            obj.DriverType = ticketDetail.DriverType;
            obj.FOB = ticketDetail.FOB;
            obj.JobDescription = ticketDetail.JobDescription;
            obj.JobNumber = ticketDetail.JobNumber;
            obj.JobSegmentDescription = ticketDetail.JobSegmentDescription;
            obj.JobSegmentNumber = ticketDetail.JobSegmentNumber;
            obj.MaterialCost = ticketDetail.MaterialCost;
            obj.PlantDescription = ticketDetail.PlantDescription;
            obj.PlantNumber = ticketDetail.PlantNumber;
            obj.SalesPersonDescription = ticketDetail.SalesPersonDescription;
            obj.SalesPersonNumber = ticketDetail.SalesPersonNumber;
            obj.TicketDate = ticketDetail.TicketDate;
            obj.TicketId = ticketDetail.TicketId;
            obj.TicketNumber = ticketDetail.TicketNumber;
            obj.TimeArriveJob = ticketDetail.TimeArriveJob;
            obj.TimeArrivePlant = ticketDetail.TimeArrivePlant;
            obj.TimeBeginLoad = ticketDetail.TimeBeginLoad;
            obj.TimeBeginUnload = ticketDetail.TimeBeginUnload;
            obj.TimeDueOnJob = ticketDetail.TimeDueOnJob;
            obj.TimeEndLoad = ticketDetail.TimeEndLoad;
            obj.TimeEndUnload = ticketDetail.TimeEndUnload;
            obj.TimeLeaveJob = ticketDetail.TimeLeaveJob;
            obj.TimeLeavePlant = ticketDetail.TimeLeavePlant;
            obj.TimeTicketed = ticketDetail.TimeTicketed;
            obj.TotalRevenue = ticketDetail.TotalRevenue;
            obj.TruckHomePlantNumber = ticketDetail.TruckHomePlantNumber;
            obj.TruckNumber = ticketDetail.TruckNumber;
            obj.TruckType = ticketDetail.TruckType;
            obj.VoidStatus = ticketDetail.VoidStatus;
        }

        public static List<TicketDetail> GetTicketsWithinDateRange(DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                return context.TicketDetails.Where(x => x.TicketDate >= startDate && x.TicketDate <= endDate).ToList();
            }
        }
        public static List<TicketDetail> GetTicketsByDriver(string driverNumber)
        {
            using (var context = GetContext())
            {
                return context.TicketDetails.Where(x => x.DriverNumber == driverNumber).OrderBy(x => x.TimeTicketed).ToList();
            }
        }

        /// <summary>
        /// Get Ticket details pending for Update in Mongo DB. 
        /// If Rows COunt Paramenter is 0 then method will return all rows else only first RowsCount rows
        /// like of RowsCount is 50 then it will return first 50 records else if rows count is 0 it will return all rows
        /// </summary>
        /// <returns></returns>
        public static List<TicketDetail> GetMongoPendingTickets(int rowsCount)
        {
            using (var context = GetContext())
            {
                var query = context.TicketDetails.Where(x => x.IsMongoUpdated == false && x.IsScrubbed && x.IsProcessed).OrderBy(x => x.DriverNumber).ThenBy(x => x.TicketDate).ThenBy(x => x.TimeTicketed);

                if (rowsCount <= 0)
                    return query.ToList();

                return query.Take(rowsCount).ToList();
            }
        }

        /// <summary>
        /// Get Daily Plant Summary pending for Update in Mongo DB. 
        /// If Rows COunt Paramenter is 0 then method will return all rows else only first RowsCount rows
        /// like of RowsCount is 50 then it will return first 50 records else if rows count is 0 it will return all rows
        /// </summary>
        /// <returns></returns>
        public static List<DailyPlantSummary> GetMongoPendingDailyPlantSummary(int rowsCount)
        {
            using (var context = GetContext())
            {
                var query = context.DailyPlantSummaries.Where(x => x.IsMongoUpdated == false).OrderBy(x => x.DayDateTime);

                if (rowsCount <= 0)
                    return query.ToList();

                return query.Take(rowsCount).ToList();
            }
        }

        public static bool UpdateTicketIsMongoUpdated(long ticketId)
        {
            using (var context = GetContext())
            {
                var obj = context.TicketDetails.FirstOrDefault(x => x.Id == ticketId);
                if (obj != null)
                    obj.IsMongoUpdated = true;

                context.SubmitChanges();
                return true;
            }
        }
        public static bool UpdateDailyPlantSummaryIsMongoUpdated(long dailyPlantSummaryId)
        {
            using (var context = GetContext())
            {
                var obj = context.DailyPlantSummaries.FirstOrDefault(x => x.Id == dailyPlantSummaryId);
                if (obj != null)
                    obj.IsMongoUpdated = true;

                context.SubmitChanges();
                return true;
            }
        }
        public static bool UpdateDriverLoginTimeIsMongoUpdated(long driverLoginTimeId)
        {
            using (var context = GetContext())
            {
                var obj = context.DriverLoginTimes.FirstOrDefault(x => x.Id == driverLoginTimeId);
                if (obj != null)
                    obj.IsMongoUpdated = true;

                context.SubmitChanges();
                return true;
            }
        }
        #endregion

        #region Driver Detail

        public static void AddUpdateDriverDetail(DriverDetail driverDetail)
        {
            if (driverDetail.DriverNumber == null)
                return;
            using (var context = GetContext())
            {
                if (driverDetail.Id == 0)
                {
                    DriverDetail obj = context.DriverDetails.FirstOrDefault(x => x.DriverNumber == driverDetail.DriverNumber);
                    if (obj != null)
                    {
                        obj.DriverName = driverDetail.DriverName;
                        obj.DriverNumber = driverDetail.DriverNumber;
                    }
                    else
                    {
                        context.DriverDetails.InsertOnSubmit(driverDetail);
                    }
                }
                else
                {
                    DriverDetail obj = context.DriverDetails.FirstOrDefault(x => x.Id == driverDetail.Id);

                    obj.DriverName = driverDetail.DriverName;
                    obj.DriverNumber = driverDetail.DriverNumber;
                }
                context.SubmitChanges();
            }
        }
        public static void AddUpdateDriverDetailList(List<DriverDetail> driverDetailList)
        {

            driverDetailList = driverDetailList.Distinct(new DriverDetailComparer()).ToList();
            BulkInsertList("dbo.DriverDetails", driverDetailList.AsDataTable());
        }

        public static DriverDetail GetDriverDetail(string driverNumber)
        {
            using (var context = GetContext())
            {
                return context.DriverDetails.FirstOrDefault(x => x.DriverNumber == driverNumber);
            }
        }
        #endregion

        #region Driver Login Time Detail

        public static void AddUpdateDriverLoginTime(DriverLoginTime driverLoginTime)
        {
            using (var context = GetContext())
            {
                if (driverLoginTime.Id == 0)
                {
                    DriverLoginTime obj = context.DriverLoginTimes.FirstOrDefault(x => x.DriverNumber == driverLoginTime.DriverNumber && x.LoginDate == driverLoginTime.LoginDate);
                    if (obj != null)
                    {
                        obj.LoginDate = driverLoginTime.LoginDate;
                        obj.DriverNumber = driverLoginTime.DriverNumber;
                        obj.PunchInTime = driverLoginTime.PunchInTime;
                        obj.PunchOutTime = driverLoginTime.PunchOutTime;
                        obj.TotalTime = driverLoginTime.TotalTime;
                    }
                    else
                    {
                        context.DriverLoginTimes.InsertOnSubmit(driverLoginTime);
                    }
                }
                else
                {
                    DriverLoginTime obj = context.DriverLoginTimes.FirstOrDefault(x => x.Id == driverLoginTime.Id);

                    obj.LoginDate = driverLoginTime.LoginDate;
                    obj.DriverNumber = driverLoginTime.DriverNumber;
                    obj.PunchInTime = driverLoginTime.PunchInTime;
                    obj.PunchOutTime = driverLoginTime.PunchOutTime;
                    obj.TotalTime = driverLoginTime.TotalTime;
                }
                context.SubmitChanges();
            }
        }

        public static void AddUpdateDriverLoginTimeList(List<DriverLoginTime> driverDetailList)
        {
            BulkInsertList("dbo.DriverLoginTimes", driverDetailList.AsDataTable());
        }
        public static DriverLoginTime GetDriverLoginDetail(string driverNumber, DateTime date)
        {
            using (var context = GetContext())
            {
                return context.DriverLoginTimes.FirstOrDefault(x => x.DriverNumber == driverNumber && x.LoginDate == date);
            }
        }
        public static DriverLoginTime GetDriverLoginTime(long id)
        {
            using (var context = GetContext())
            {
                return context.DriverLoginTimes.FirstOrDefault(x => x.Id == id);
            }
        }
        public static List<DriverLoginTime> GetDriverLoginsWithinDates(DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                return context.DriverLoginTimes.Where(x => x.LoginDate >= startDate && x.LoginDate <= endDate && x.IsMongoUpdated == false).ToList();
            }
        }
        #endregion

        #region Daily Plant Summary

        public static void AddUpdateDailyPlantSummary(DailyPlantSummary dailyPlantSummary)
        {
            using (var context = GetContext())
            {
                if (dailyPlantSummary.Id == 0)
                {
                    DailyPlantSummary obj = null;
                    if (dailyPlantSummary.RefId != null)
                        obj = context.DailyPlantSummaries.FirstOrDefault(x => x.RefId == dailyPlantSummary.RefId);
                    if (obj != null)
                    {
                        obj.DayDateTime = dailyPlantSummary.DayDateTime;
                        obj.PlantId = dailyPlantSummary.PlantId;
                        obj.ProducedVolume = dailyPlantSummary.ProducedVolume;
                        obj.TrucksAssigned = dailyPlantSummary.TrucksAssigned;
                        obj.TruckAvailable = dailyPlantSummary.TruckAvailable;
                        obj.DriversOnPayroll = dailyPlantSummary.DriversOnPayroll;
                        obj.DriversAvailable = dailyPlantSummary.DriversAvailable;
                        obj.PlantInterruptions = dailyPlantSummary.PlantInterruptions;
                        obj.BadOrRejectedLoads = dailyPlantSummary.BadOrRejectedLoads;
                        obj.Accidents = dailyPlantSummary.Accidents;
                        obj.TotalLoads = dailyPlantSummary.TotalLoads;
                        obj.TotalOrders = dailyPlantSummary.TotalOrders;
                        obj.FirstLoadOnTimePercent = dailyPlantSummary.FirstLoadOnTimePercent;
                        obj.DriverDeliveredVolume = dailyPlantSummary.DriverDeliveredVolume;
                        obj.ScheduledVolume = dailyPlantSummary.ScheduledVolume;
                        obj.ScheduledTrucks = dailyPlantSummary.ScheduledTrucks;
                        obj.TotalClockHours = dailyPlantSummary.TotalClockHours;
                        obj.DriversUtilized = dailyPlantSummary.DriversUtilized;
                        obj.AverageLoadSize = dailyPlantSummary.AverageLoadSize;
                        obj.StartUpTime = dailyPlantSummary.StartUpTime;
                        obj.ShutdownTime = dailyPlantSummary.ShutdownTime;
                        obj.InYardTime = dailyPlantSummary.InYardTime;
                        obj.TicketTime = dailyPlantSummary.TicketTime;
                        obj.LoadTime = dailyPlantSummary.LoadTime;
                        obj.TemperingTime = dailyPlantSummary.TemperingTime;
                        obj.ToJobTime = dailyPlantSummary.ToJobTime;
                        obj.WaitOnJobTime = dailyPlantSummary.WaitOnJobTime;
                        obj.PourTime = dailyPlantSummary.PourTime;
                        obj.WashOnJobTime = dailyPlantSummary.WashOnJobTime;
                        obj.FromJobTime = dailyPlantSummary.FromJobTime;
                        obj.TruckBreakdowns = dailyPlantSummary.TruckBreakdowns;
                        obj.NonDeliveryHours = dailyPlantSummary.NonDeliveryHours;
                        obj.RefId = dailyPlantSummary.RefId;
                    }
                    else
                    {
                        context.DailyPlantSummaries.InsertOnSubmit(dailyPlantSummary);
                    }
                }
                else
                {
                    DailyPlantSummary obj = context.DailyPlantSummaries.Where(x => x.Id == dailyPlantSummary.Id).FirstOrDefault();

                    obj.DayDateTime = dailyPlantSummary.DayDateTime;
                    obj.PlantId = dailyPlantSummary.PlantId;
                    obj.ProducedVolume = dailyPlantSummary.ProducedVolume;
                    obj.TrucksAssigned = dailyPlantSummary.TrucksAssigned;
                    obj.TruckAvailable = dailyPlantSummary.TruckAvailable;
                    obj.DriversOnPayroll = dailyPlantSummary.DriversOnPayroll;
                    obj.DriversAvailable = dailyPlantSummary.DriversAvailable;
                    obj.PlantInterruptions = dailyPlantSummary.PlantInterruptions;
                    obj.BadOrRejectedLoads = dailyPlantSummary.BadOrRejectedLoads;
                    obj.Accidents = dailyPlantSummary.Accidents;
                    obj.TotalLoads = dailyPlantSummary.TotalLoads;
                    obj.TotalOrders = dailyPlantSummary.TotalOrders;
                    obj.FirstLoadOnTimePercent = dailyPlantSummary.FirstLoadOnTimePercent;
                    obj.DriverDeliveredVolume = dailyPlantSummary.DriverDeliveredVolume;
                    obj.ScheduledVolume = dailyPlantSummary.ScheduledVolume;
                    obj.ScheduledTrucks = dailyPlantSummary.ScheduledTrucks;
                    obj.TotalClockHours = dailyPlantSummary.TotalClockHours;
                    obj.DriversUtilized = dailyPlantSummary.DriversUtilized;
                    obj.AverageLoadSize = dailyPlantSummary.AverageLoadSize;
                    obj.StartUpTime = dailyPlantSummary.StartUpTime;
                    obj.ShutdownTime = dailyPlantSummary.ShutdownTime;
                    obj.InYardTime = dailyPlantSummary.InYardTime;
                    obj.TicketTime = dailyPlantSummary.TicketTime;
                    obj.LoadTime = dailyPlantSummary.LoadTime;
                    obj.TemperingTime = dailyPlantSummary.TemperingTime;
                    obj.ToJobTime = dailyPlantSummary.ToJobTime;
                    obj.WaitOnJobTime = dailyPlantSummary.WaitOnJobTime;
                    obj.PourTime = dailyPlantSummary.PourTime;
                    obj.WashOnJobTime = dailyPlantSummary.WashOnJobTime;
                    obj.FromJobTime = dailyPlantSummary.FromJobTime;
                    obj.TruckBreakdowns = dailyPlantSummary.TruckBreakdowns;
                    obj.NonDeliveryHours = dailyPlantSummary.NonDeliveryHours;
                    obj.RefId = dailyPlantSummary.RefId;
                }
                context.SubmitChanges();
            }
        }
        public static void AddUpdateDailyPlantSummaryList(List<DailyPlantSummary> ticketDetailList)
        {
            BulkInsertList("dbo.DailyPlantSummary", ticketDetailList.AsDataTable());
        }

        #endregion

        #region Weekday and Work day Distributions
        public static WeekDayDistribution FindOrCreateWeeklyDistribution(int? districtId = null)
        {
            using (var context = GetContext())
            {
                var dist = context.WeekDayDistributions.FirstOrDefault();

                if (districtId != null)
                {
                    dist = context.WeekDayDistributions.Where(x => x.DistrictId == districtId.GetValueOrDefault()).FirstOrDefault();
                }
                if (dist == null)
                {
                    dist = new WeekDayDistribution();
                    dist.Monday = 100;
                    dist.Tuesday = 100;
                    dist.Wednesday = 100;
                    dist.Thursday = 100;
                    dist.Friday = 100;
                    dist.Saturday = 50;
                    dist.Sunday = 0;
                    dist.DistrictId = districtId.GetValueOrDefault();
                    context.WeekDayDistributions.InsertOnSubmit(dist);
                    context.SubmitChanges();
                }
                return dist;
            }
        }

        public static List<WorkDayException> GetExceptions(int? districtId = null)
        {
            using (var context = GetContext())
            {
                if (districtId != null)
                {
                    return context.WorkDayExceptions.Where(x => x.DistrictId == districtId).OrderByDescending(x => x.ExceptionDate).ToList();
                }
                else
                {
                    return context.WorkDayExceptions.Where(x => x.DistrictId == 0).OrderByDescending(x => x.ExceptionDate).ToList();
                }
            }
        }

        public static void AddException(DateTime date, string description, double distribution, int? districtId = null)
        {
            using (var context = GetContext())
            {
                var exception = new WorkDayException();
                if (districtId != null)
                {
                    exception = context.WorkDayExceptions.Where(x => x.ExceptionDate == date && x.DistrictId == districtId).FirstOrDefault();
                }
                else
                {
                    exception = context.WorkDayExceptions.Where(x => x.ExceptionDate == date && x.DistrictId == 0).FirstOrDefault();
                }
                if (exception == null)
                {
                    exception = new WorkDayException();
                    exception.ExceptionDate = date;
                    exception.DistrictId = districtId.GetValueOrDefault();
                    context.WorkDayExceptions.InsertOnSubmit(exception);
                }
                exception.Distribution = distribution;
                exception.Description = description;
                context.SubmitChanges();
                var wdd = context.WorkDayDistributions.Where(x => x.WorkDay == date).FirstOrDefault();
                if (wdd == null)
                {
                    wdd = new WorkDayDistribution();
                    wdd.WorkDay = date;
                    context.WorkDayDistributions.InsertOnSubmit(wdd);
                }
                wdd.Distribution = distribution;
                context.SubmitChanges();
            }
        }

        public static void DeleteException(DateTime date)
        {
            using (var context = GetContext())
            {
                var exception = context.WorkDayExceptions.Where(x => x.ExceptionDate == date).FirstOrDefault();
                if (exception != null)
                {
                    context.WorkDayExceptions.DeleteOnSubmit(exception);
                    context.Refresh(RefreshMode.KeepCurrentValues, exception);
                }
                context.SubmitChanges();
                var wdd = context.WorkDayDistributions.Where(x => x.WorkDay == date).FirstOrDefault();
                if (wdd != null)
                {
                    wdd.Distribution = FindDefaultDistribution(date);
                }
                context.SubmitChanges();
            }
        }

        public static double FindDefaultDistribution(DateTime current)
        {
            WeekDayDistribution dist = FindOrCreateWeeklyDistribution();
            using (var context = GetContext())
            {
                if (current.DayOfWeek == DayOfWeek.Monday)
                {
                    return dist.Monday;
                }
                if (current.DayOfWeek == DayOfWeek.Tuesday)
                {
                    return dist.Tuesday;
                }
                if (current.DayOfWeek == DayOfWeek.Wednesday)
                {
                    return dist.Wednesday;
                }
                if (current.DayOfWeek == DayOfWeek.Thursday)
                {
                    return dist.Thursday;
                }
                if (current.DayOfWeek == DayOfWeek.Friday)
                {
                    return dist.Friday;
                }
                if (current.DayOfWeek == DayOfWeek.Saturday)
                {
                    return dist.Saturday;
                }
                if (current.DayOfWeek == DayOfWeek.Sunday)
                {
                    return dist.Sunday;
                }
            }
            return 0;
        }

        public static void ApplyExceptions()
        {
            using (var context = GetContext())
            {
                foreach (WorkDayException exception in context.WorkDayExceptions.ToList())
                {
                    WorkDayDistribution wdd = context.WorkDayDistributions.Where(x => x.WorkDay == exception.ExceptionDate).FirstOrDefault();
                    if (wdd == null)
                    {
                        wdd = new WorkDayDistribution();
                        wdd.WorkDay = exception.ExceptionDate;
                        context.WorkDayDistributions.InsertOnSubmit(wdd);
                    }
                    wdd.Distribution = exception.Distribution;
                    context.SubmitChanges();
                }
            }
        }

        public static void UpdateWeekDayDistribution(int monday, int tuesday, int wednesday, int thursday, int friday, int saturday, int sunday, int? districtId = null)
        {
            using (var context = GetContext())
            {
                WeekDayDistribution dist = context.WeekDayDistributions.Where(x => x.DistrictId == districtId).FirstOrDefault();
                if (dist == null)
                {
                    dist = new WeekDayDistribution();
                    context.WeekDayDistributions.InsertOnSubmit(dist);
                }
                dist.Monday = monday;
                dist.Tuesday = tuesday;
                dist.Wednesday = wednesday;
                dist.Thursday = thursday;
                dist.Friday = friday;
                dist.Saturday = saturday;
                dist.Sunday = sunday;
                dist.DistrictId = districtId.GetValueOrDefault();
                context.SubmitChanges();

                DateTime current = DateTime.Today;
                current = DateUtils.GetFirstOf(1, current.Year);
                current = current.AddYears(-3);

                for (int i = 0; i < 5000; i++)
                {
                    WorkDayDistribution wdd = context.WorkDayDistributions.Where(x => x.WorkDay == current).FirstOrDefault();
                    if (wdd == null)
                    {
                        wdd = new WorkDayDistribution();
                        wdd.WorkDay = current;
                        context.WorkDayDistributions.InsertOnSubmit(wdd);
                    }
                    if (current.DayOfWeek == DayOfWeek.Monday)
                    {
                        wdd.Distribution = monday;
                    }
                    if (current.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        wdd.Distribution = tuesday;
                    }
                    if (current.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        wdd.Distribution = wednesday;
                    }
                    if (current.DayOfWeek == DayOfWeek.Thursday)
                    {
                        wdd.Distribution = thursday;
                    }
                    if (current.DayOfWeek == DayOfWeek.Friday)
                    {
                        wdd.Distribution = friday;
                    }
                    if (current.DayOfWeek == DayOfWeek.Saturday)
                    {
                        wdd.Distribution = saturday;
                    }
                    if (current.DayOfWeek == DayOfWeek.Sunday)
                    {
                        wdd.Distribution = sunday;
                    }
                    current = current.AddDays(1);
                }
                context.SubmitChanges();
            }
        }

        public static WorkDayException FindWorkDayException(int id)
        {
            using (var context = GetContext())
            {
                return context.WorkDayExceptions.FirstOrDefault(x => x.Id == id);
            }
        }

        public static double GetTotalWorkDays(DateTime startDate, DateTime endDate)
        {
            using (var context = GetContext())
            {
                return context.WorkDayDistributions.Where(x => x.WorkDay > startDate).Where(x => x.WorkDay <= endDate).Sum(x => x.Distribution);
            }
        }

        public static double GetWorkDayDistribution(DateTime date)
        {
            using (var context = GetContext())
            {
                return context.WorkDayDistributions.FirstOrDefault(x => x.WorkDay == date).Distribution;
            }
        }
        #endregion

        #region Widget Setting 
        public static List<WidgetSetting> GetUserWidgetSettingsList(Guid userId, long dashBoardId = 0)
        {
            using (var db = GetContext())
            {
                var boardSettingId = GetUserDashboardSetting(userId, dashBoardId).Id;
                return db.WidgetSettings.Where(x => x.DashboardId == boardSettingId).OrderBy(x => x.Position).ToList(); ;
            }
        }

        public static List<WidgetSetting> WidgetSettings = null;

        public static WidgetSetting GetWidgetSettings(long widgetId)
        {
            var widgetSetting = new WidgetSetting();
            using (var db = GetContext())
            {
                if (WidgetSettings == null)
                {
                    WidgetSettings = db.WidgetSettings.ToList();
                }

                if (WidgetSettings != null)
                {
                    widgetSetting = WidgetSettings.FirstOrDefault(x => x.WidgetId == widgetId);
                    return widgetSetting;
                }
                widgetSetting = db.WidgetSettings.Where(x => x.WidgetId == widgetId).FirstOrDefault();
            }
            return widgetSetting;
        }

        public static long AddUpdateWidget(WidgetSetting widSet)
        {
            long widgetSettingId = 0;
            if (widSet != null)
            {
                using (var db = GetContext())
                {
                    if (widSet.WidgetId == 0)
                    {
                        db.WidgetSettings.InsertOnSubmit(widSet);
                    }
                    else
                    {
                        db.WidgetSettings.Attach(widSet);
                        db.Refresh(RefreshMode.KeepCurrentValues, widSet);
                    }
                    db.SubmitChanges();
                    widgetSettingId = widSet.WidgetId;

                    //Update the static variable
                    if (WidgetSettings != null)
                    {
                        WidgetSettings = db.WidgetSettings.ToList();
                    }
                }
            }
            return widgetSettingId;
        }
        /// <summary>
        /// Deleting the Single Widget from a dashboard
        /// </summary>
        /// <param name="widgetId"></param>
        /// <returns>Delete Status(bool)</returns>
        public static bool DeleteDashboardWidget(long widgetId)
        {
            var deleteStatus = false;
            using (var db = GetContext())
            {
                var widget = db.WidgetSettings.Where(x => x.WidgetId == widgetId).FirstOrDefault();
                if (widget != null)
                {
                    db.WidgetSettings.DeleteOnSubmit(widget);
                    db.Refresh(RefreshMode.KeepCurrentValues, widget);
                }
                db.SubmitChanges();
                deleteStatus = true;
            }
            return deleteStatus;
        }

        public static void UpdateWidgetOrder(long dashboardId, Dictionary<long, int> widgetOrder)
        {
            using (var context = GetContext())
            {
                var widgets = context.WidgetSettings.Where(x => x.DashboardId == dashboardId);

                if (widgetOrder != null && widgets != null)
                {
                    foreach (var wid in widgets)
                    {
                        if (widgetOrder.ContainsKey(wid.WidgetId))
                        {
                            wid.Position = widgetOrder[wid.WidgetId];
                        }
                    }

                    context.SubmitChanges();
                }
            }
        }

        #endregion

        #region Dashboard Settings
        //public static DashboardSetting GetUserDashboardSetting(Guid userId, long dashBoardId = 0, UserDashboardSetting userDashSetting = null)
        //{
        //    DashboardSetting dashboardSetting = null;
        //    using (var db = GetContext())
        //    {
        //        if (dashBoardId == 0)
        //        {
        //            dashboardSetting = db.DashboardSettings.Where(x => x.UserId == userId && x.Default == true).FirstOrDefault();
        //        }
        //        else
        //        {
        //            dashboardSetting = db.DashboardSettings.Where(x => x.Id == dashBoardId).FirstOrDefault();
        //            if (dashboardSetting != null)
        //            {
        //                if (dashboardSetting.AccessType != "Standard" && dashboardSetting.UserId != userId)
        //                {
        //                    dashboardSetting = null;
        //                }
        //            }
        //        }
        //    }
        //    return dashboardSetting;
        //}

        public static DashboardSetting GetUserDashboardSetting(Guid userId, long dashBoardId = 0, UserDashboardSetting userDashSetting = null)
        {
            DashboardSetting dashboardSetting = null;
            using (var db = GetContext())
            {
                if (dashBoardId == 0 && userDashSetting == null)
                {
                    dashboardSetting = db.DashboardSettings.Where(x => x.AccessType == "Standard").FirstOrDefault();
                }
                else
                {
                    var boardId = dashBoardId;
                    if (boardId == 0)
                    {
                        boardId = userDashSetting.DashboardId;
                    }
                    dashboardSetting = db.DashboardSettings.Where(x => x.Id == boardId).FirstOrDefault();
                    //if (dashboardSetting != null)
                    //{
                    //    if (dashboardSetting.AccessType != "Standard" && dashboardSetting.UserId != userId)
                    //    {
                    //        dashboardSetting = null;
                    //    }
                    //}
                }
            }
            return dashboardSetting;
        }

        public static UserDashboardSetting GetDashboardSetting(Guid userId, long dashBoardId = 0)
        {
           UserDashboardSetting UserdashboardSetting = null;
            using (var db = GetContext())
            {
                if (dashBoardId == 0)
                {
                    UserdashboardSetting = db.UserDashboardSettings.Where(x => x.UserId == userId && x.IsDefault == true).FirstOrDefault();

                }
                else
                {
                    UserdashboardSetting = db.UserDashboardSettings.Where(x => x.DashboardId == dashBoardId && x.UserId == userId).FirstOrDefault();
                }

            }
            return UserdashboardSetting;
        }

        /// <summary>
        /// Detete Dashboard
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <returns>Delete Status</returns>
        public static bool DeleteDashboard(long dashboardId, Guid userId)
        {
            var deleteStatus = false;
            using (var db = GetContext())
            {
                var dashboard = db.DashboardSettings.Where(x => x.Id == dashboardId).FirstOrDefault();
                if (dashboard != null)
                {
                    db.DashboardSettings.DeleteOnSubmit(dashboard);
                }
                var dashboardFilters = db.DashboardFilters.Where(x => x.DashboardId == dashboardId).ToList();
                if (dashboardFilters.Count() != 0)
                {
                    db.DashboardFilters.DeleteAllOnSubmit(dashboardFilters);
                }
                db.SubmitChanges();

                var dashboardList = db.UserDashboardSettings.Where(x => x.UserId == userId).ToList();
                if (dashboardList.Count() != 0)
                {
                    var dashbaordCount = dashboardList.Where(x => x.IsDefault == true).ToList();
                    if (dashbaordCount.Count() == 0)
                    {
                        var defautDashboard = dashboardList.FirstOrDefault();
                        defautDashboard.IsDefault = true;
                        db.SubmitChanges();
                    }
                }
                deleteStatus = true;
            }
            return deleteStatus;
        }
        /// <summary>
        /// Add New Dashboard
        /// </summary>
        /// <param name="dashboardSetting"></param>
        /// <returns>DashboardId</returns>
        public static long AddDashBoard(DashboardSetting dashboardSetting)
        {
            long dashboardId = 0;
            using (var db = GetContext())
            {
                db.DashboardSettings.InsertOnSubmit(dashboardSetting);
                db.SubmitChanges();
                dashboardId = dashboardSetting.Id;
            }
            return dashboardId;
        }

        public static bool AddUserDashboardSetting(UserDashboardSetting userDashSetting)
        {
            bool addStatus = true;
            try
            {
                UpdateDashBoardSettings(userDashSetting.IsDefault.GetValueOrDefault(), userDashSetting.MaxColumn, userDashSetting.IsFavourite.GetValueOrDefault(), userDashSetting.DashboardId, userDashSetting.UserId);
            }
            catch (Exception)
            {
                addStatus =  false;
            }
            return addStatus;
        }

        public static bool ChangeDashboardAccessType(long dashboardId, string accessType)
        {
            bool updateStatus = true;
            try
            {
                using (var db = GetContext())
                {
                    var dashboardSetting = db.DashboardSettings.Where(x => x.Id == dashboardId).FirstOrDefault();
                    if (dashboardSetting != null)
                    {
                        dashboardSetting.AccessType = accessType;
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return updateStatus;
        }

        public static bool ChangeDashboardStartDate(long dashboardId, DateTime startDate)
        {
            bool updateStatus = true;
            try
            {
                using (var db = GetContext())
                {
                    var dashboardSetting = db.DashboardSettings.Where(x => x.Id == dashboardId).FirstOrDefault();
                    if (dashboardSetting != null)
                    {
                        dashboardSetting.StartDate = startDate;
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return updateStatus;
        }

        public static bool ChangeDashboardName(long dashboardId, string dashboardName)
        {
            bool updateStatus = true;
            try
            {
                using (var db = GetContext())
                {
                    var dashboardSetting = db.DashboardSettings.Where(x => x.Id == dashboardId).FirstOrDefault();
                    if (dashboardSetting != null)
                    {
                        dashboardSetting.Name = dashboardName;
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return updateStatus;
        }

        

        public static long EditDashBoard(DashboardSetting dashboardSetting)
        {
            long dashboardId = 0;
            using (var db = GetContext())
            {
                var dashboardData = db.DashboardSettings.Where(x => x.Id == dashboardSetting.Id).FirstOrDefault();
                dashboardData.Id = dashboardSetting.Id;
                dashboardData.Name = dashboardSetting.Name;
                dashboardData.AccessType = dashboardSetting.AccessType;
                dashboardData.Favorite = dashboardSetting.Favorite;
                dashboardData.Default = dashboardSetting.Default;
                dashboardData.UserId = dashboardSetting.UserId;
                dashboardData.CreatedAt = dashboardSetting.CreatedAt;
                dashboardData.MaxColumn = dashboardSetting.MaxColumn;
                db.SubmitChanges();
                dashboardId = dashboardSetting.Id;
            }
            return dashboardId;
        }

        public static bool UpdateDashBoardSettings(bool isDefault, int maxColumn, bool isFavorite, long dashboardId, Guid userId)
        {
            bool updateStatus = true;
            try
            {
                using (var db = GetContext())
                {
                    var userBoardSetting = db.UserDashboardSettings.Where(x => x.UserId == userId).ToList();
                    if (userBoardSetting.Count > 0)
                    {
                        var dashboard = userBoardSetting.Where(x => x.DashboardId == dashboardId).FirstOrDefault();
                        if (dashboard != null)
                        {
                            foreach (var item in userBoardSetting)
                            {
                                if (item.DashboardId == dashboardId)
                                {
                                    item.IsFavourite = isFavorite;
                                    item.MaxColumn = maxColumn;
                                    item.IsDefault = isDefault;
                                }
                                else if (isDefault)
                                {
                                    item.IsDefault = false;
                                }
                                db.SubmitChanges();
                            }

                        }
                        else
                        {
                            if (isDefault)
                            {
                                foreach (var item in userBoardSetting)
                                {
                                    item.IsDefault = false;
                                    db.SubmitChanges();
                                }
                            }
                            UserDashboardSetting dbSetting = new UserDashboardSetting();
                            dbSetting.DashboardId = dashboardId;
                            dbSetting.UserId = userId;
                            dbSetting.IsFavourite = isFavorite;
                            dbSetting.MaxColumn = maxColumn;
                            dbSetting.IsDefault = isDefault;
                            db.UserDashboardSettings.InsertOnSubmit(dbSetting);
                            db.SubmitChanges();
                        }
                        if (!isDefault)
                        {
                            var totalDefaultBoard = userBoardSetting.Where(x => x.IsDefault == true && x.Id != dashboardId).Count();
                            if (totalDefaultBoard == 0)
                            {
                                var boardSetting = userBoardSetting.FirstOrDefault();
                                boardSetting.IsDefault = true;
                                db.SubmitChanges();
                            }
                        }
                    }
                    else
                    {
                        UserDashboardSetting dbSetting = new UserDashboardSetting();
                        dbSetting.DashboardId = dashboardId;
                        dbSetting.UserId = userId;
                        dbSetting.IsFavourite = isFavorite;
                        dbSetting.MaxColumn = maxColumn;
                        dbSetting.IsDefault = true;
                        db.UserDashboardSettings.InsertOnSubmit(dbSetting);
                        db.SubmitChanges();
                    }
                    //var boardSetting = db.DashboardSettings.Where(x => x.UserId == userId).ToList();
                    //if (boardSetting != null)
                    //{
                    //    foreach (var item in boardSetting)
                    //    {
                    //        if (item.Id == dashboardId)
                    //        {
                    //            item.Favorite = isFavorite;
                    //            item.MaxColumn = maxColumn;
                    //            item.Default = isDefault;
                    //            item.Name = dashboardName;
                    //            item.StartDate = startdate;
                    //        }
                    //        else if (isDefault)
                    //        {
                    //            item.Default = false;
                    //        }
                    //        db.SubmitChanges();
                    //    }
                    //    if (!isDefault)
                    //    {
                    //        var totalDefaultBoard = boardSetting.Where(x => x.Default == true && x.Id != dashboardId).Count();
                    //        if (totalDefaultBoard == 0)
                    //        {
                    //            var dashboard = boardSetting.FirstOrDefault();
                    //            dashboard.Default = true;
                    //            db.SubmitChanges();
                    //        }
                    //    }
                    //}

                }
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return updateStatus;
        }
        /// <summary>
        ///  To get the list of Dashboard according to the AccesType of the Dashboard usually used for filling the drop down in the Dashboard page       
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accessType"></param>
        /// <returns>Dictionary<string, List<DashboardSetting>></returns>
        public static Dictionary<string, List<DashboardSetting>> GetDashboardSettingGroupWise(Guid userId, string accessType = "")
        {
            List<DashboardSetting> dashboardList;
            var dashboardListWithGroup = new Dictionary<string, List<DashboardSetting>>();

            using (var db = GetContext())
            {
                var dashboardUserWise = db.DashboardSettings.Where(x => x.UserId == userId || x.AccessType == "Standard").ToList();
                if (dashboardUserWise.Count() != 0)
                {
                    var favouriteDashboard = db.UserDashboardSettings.Where(x => x.IsFavourite == true && x.UserId == userId).ToList();
                    dashboardList = new List<DashboardSetting>();
                    foreach (var reportItem in favouriteDashboard)
                    {
                        var dash = dashboardUserWise.Where(x => x.Id == reportItem.DashboardId).FirstOrDefault();
                        dashboardList.Add(dash);
                    }
                    if (dashboardList.Count() == 0)
                    {
                        dashboardList.Add(null);
                    }
                    dashboardListWithGroup.Add("Favourite", dashboardList);

                    var dashboardWithGroup = dashboardUserWise.GroupBy(x => new { x.AccessType }).ToList();
                    foreach (var item in dashboardWithGroup)
                    {
                        var dashboardGroupList = dashboardUserWise.Where(x => x.AccessType == item.Key.AccessType.ToString()).ToList();
                        dashboardList = new List<DashboardSetting>();
                        foreach (var reportItem in dashboardGroupList)
                        {
                            dashboardList.Add(reportItem);
                        }
                        if (dashboardList.Count() == 0)
                        {
                            dashboardList.Add(null);
                        }
                        dashboardListWithGroup.Add(item.Key.AccessType.ToString(), dashboardList);
                    }
                }
                else
                {
                    dashboardListWithGroup = null;
                }
            }
            return dashboardListWithGroup;
        }
        /// <summary>
        /// Update Dashboard filters while applying and  saving filters
        /// </summary>
        /// <param name="dashboardFilters"></param>
        /// <param name="dashboardId"></param>
        /// <returns>return Update status</returns>
        public static bool UpdateDashboardFilterSettings(Dictionary<string, List<long>> dashboardFilters, long dashboardId)
        {
            using (var db = GetContext())
            {
                var allDashboardFilters = db.DashboardFilters.Where(x => x.DashboardId == dashboardId).ToList();
                db.DashboardFilters.DeleteAllOnSubmit(allDashboardFilters);
                db.SubmitChanges();

                foreach (var entityType in dashboardFilters)
                {
                    if (entityType.Value != null)
                    {
                        foreach (var entityId in entityType.Value)
                        {
                            DashboardFilter filterSetting = new DashboardFilter();
                            filterSetting.DashboardId = dashboardId;
                            filterSetting.EntityType = entityType.Key;
                            filterSetting.EntityRefId = entityId;
                            filterSetting.CreatedAt = DateTime.Now;

                            db.DashboardFilters.InsertOnSubmit(filterSetting);
                            db.SubmitChanges();
                        }
                    }
                }
            }
            return false;
        }


        public static List<DashboardFilter> GetFiltersForDashboardByWidgetId(long widgetId)
        {
            using (var context = GetContext())
            {
                var dashboardId = context.WidgetSettings.Where(x => x.WidgetId == widgetId).Select(x => x.DashboardId).FirstOrDefault();

                return context.DashboardFilters.Where(x => x.DashboardId == dashboardId).ToList();
            }
        }

        public static List<DashboardFilter> GetDashboardFilterSetting(long dashboardId)
        {
            using (var db = GetContext())
            {
                return db.DashboardFilters.Where(x => x.DashboardId == dashboardId).ToList();
            }
        }

        public static DashboardSetting GetDashboardSetting(long dashboardId)
        {
            using (var db = GetContext())
            {
                return db.DashboardSettings.Where(x => x.Id == dashboardId).FirstOrDefault();
            }
        }
        #endregion

        #region Report Settings

        public static ReportSetting GetUserReport(Guid userId, string reportType, long reportId = 0,UserReportSetting userReportSetting =null)
        {
            ReportSetting reportSetting = null;
            using (var db = GetContext())
            {
                if (reportId == 0 && userReportSetting == null)
                {
                    reportSetting = db.ReportSettings.Where(x => x.Type == reportType && x.AccessType == "Standard").FirstOrDefault();
                }
                else
                {
                    var reportSettingId = reportId;
                    if (reportId == 0)
                    {
                        reportSettingId = userReportSetting.ReportId;
                    }
                    reportSetting = db.ReportSettings.Where(x => x.Id == reportSettingId && x.Type == reportType ).FirstOrDefault();
                }
                //var reports = db.ReportSettings.Where(x => x.UserId == userId && x.Type == reportType || (x.Type == reportType && x.AccessType == "Standard")).ToList();
                //if (reportId == 0)
                //{
                //    return reports.Where(x => x.IsDefault == true).FirstOrDefault();
                //}
                //else
                //{
                //    return reports.Where(x => x.Id == reportId).FirstOrDefault();
                //}
            }
            return reportSetting;
        }

        public static UserReportSetting GetReportSetting(Guid userId, string reportType, long reportId = 0)
        {
            UserReportSetting userReportSetting = null;
            using (var db = GetContext())
            {
                if (reportId == 0)
                {
                    userReportSetting = db.UserReportSettings.Where(x => x.UserId == userId && x.Type ==reportType && x.IsDefault == true).FirstOrDefault();

                }
                else
                {
                    userReportSetting = db.UserReportSettings.Where(x => x.ReportId == reportId && x.Type == reportType && x.UserId == userId).FirstOrDefault();
                }
            }
            return userReportSetting;
        }

        public static bool ChangeDashboardAccessType(string type, string accessType, long reportId)
        {
            bool updateStatus = true;
            try
            {
                using (var db = GetContext())
                {
                    var reportSetting = db.ReportSettings.Where(x => x.Id == reportId && x.Type == type).FirstOrDefault();
                    if (reportSetting != null)
                    {
                        reportSetting.AccessType = accessType;
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return updateStatus;
        }

        /// <summary>
        ///To get the list of Reports according to the AccesType of the Report usually used to fill the drop down in the Report page (ex. Goal Analysis,Trend Analysis..etc)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reportType"></param>
        /// <param name="accessType"></param>
        /// <returns>Dictionary<string, List<Report>></returns>
        public static Dictionary<string, List<ReportSetting>> GetSavedReports(Guid userId, string reportType, string accessType = "")
        {
            List<ReportSetting> reportsList;
            var reportsListWithGroup = new Dictionary<string, List<ReportSetting>>();

            using (var db = GetContext())
            {
                var reportUserWise = db.ReportSettings.Where(x => x.UserId == userId && x.Type == reportType || (x.Type == reportType && x.AccessType == "Standard")).ToList();
                if (reportUserWise.Count() != 0)
                {
                    //Favourite
                    var favouriteReports = db.UserReportSettings.Where(x =>x.UserId == userId && x.Type == reportType && x.IsFavourite == true).ToList();
                    reportsList = new List<ReportSetting>();
                    foreach (var reportItem in favouriteReports)
                    {
                        var report = reportUserWise.Where(x => x.Id == reportItem.ReportId).FirstOrDefault();
                        reportsList.Add(report);
                    }
                    if (reportsList.Count() == 0)
                    {
                        reportsList.Add(null);
                    }
                    reportsListWithGroup.Add("Favourite", reportsList);

                    var reportsWithGroup = reportUserWise.GroupBy(x => new { x.AccessType }).ToList();
                    foreach (var item in reportsWithGroup)
                    {
                        var reportsGroupList = reportUserWise.Where(x => x.AccessType == item.Key.AccessType.ToString()).ToList();
                        reportsList = new List<ReportSetting>();
                        foreach (var reportItem in reportsGroupList)
                        {
                            reportsList.Add(reportItem);
                        }
                        if (reportsList.Count() == 0)
                        {
                            reportsList.Add(null);
                        }
                        reportsListWithGroup.Add(item.Key.AccessType.ToString(), reportsList);
                    }
                }
                else
                {
                    reportsListWithGroup = null;
                }
            }
            return reportsListWithGroup;
        }

        /// <summary>
        /// Add reports for different reports like Goal_Analysis,Trend Analysis,..etc
        /// </summary>
        /// <param name="report"></param>
        /// <returns>last added ReportId</returns>
        public static long AddReport(ReportSetting report)
        {
            long reportId = 0;
            using (var db = GetContext())
            {
                db.ReportSettings.InsertOnSubmit(report);
                db.SubmitChanges();
                reportId = report.Id;
            }
            return reportId;
        }

        public static bool AddUserReportSetting(UserReportSetting userSetting)
        {
            bool updateStatus = true;
            try
            {
                UpdateReportSettings(userSetting.IsDefault.GetValueOrDefault(), userSetting.IsFavourite.GetValueOrDefault(), userSetting.Type, userSetting.ReportId, userSetting.UserId);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return updateStatus;
        }

        public static long EditReport(ReportSetting report)
        {

            using (var db = GetContext())
            {
                var reportData = db.ReportSettings.Where(x => x.Id == report.Id).FirstOrDefault();
                reportData.Id = report.Id;
                reportData.IsDefault = report.IsDefault;
                reportData.AccessType = report.AccessType;
                reportData.ReportName = report.ReportName;
                reportData.Type = report.Type;
                reportData.UserId = report.UserId;
                db.SubmitChanges();

            }
            return report.Id;
        }

        public static List<MetricDefinition> GetAllMetricDefinition(bool ignoreInternal = false, bool sortAlphabetically = true)
        {
            List<MetricDefinition> metricDefinition = new List<MetricDefinition>();
            using (var db = GetContext())
            {
                var query = db.MetricDefinitions.AsQueryable();

                if (ignoreInternal)
                {
                    query = query.Where(x => x.IsInternal == null || x.IsInternal == false);
                }

                if (sortAlphabetically)
                {
                    query = query.OrderBy(x => x.DisplayName).ThenBy(x => x.MetricName);
                }

                metricDefinition = query.ToList();
            }
            return metricDefinition;
        }

        /// <summary>
        /// Delete Report
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns>Delete status(bool)</returns>

        public static bool DeleteReport(long reportId, string reportType, Guid userId)
        {
            var deleteStatus = false;
            using (var db = GetContext())
            {
                var reportSettingFilter = db.ReportFilterSettings.Where(x => x.ReportId == reportId).ToList();
                if (reportSettingFilter.Count() != 0)
                {
                    db.ReportFilterSettings.DeleteAllOnSubmit(reportSettingFilter);
                }
                var reportRowConfig = db.ReportRowConfigs.Where(x => x.ReportId == reportId).ToList();
                if (reportRowConfig.Count() != 0)
                {
                    db.ReportRowConfigs.DeleteAllOnSubmit(reportRowConfig);
                }
                var reportColumnConfig = db.ReportColumnConfigs.Where(x => x.ReportId == reportId).ToList();
                if (reportColumnConfig.Count() != 0)
                {
                    db.ReportColumnConfigs.DeleteAllOnSubmit(reportColumnConfig);
                }
                var trendAnalysisReportConfigs = db.TrendAnalysisReportConfigs.Where(x => x.ReportSettingId == reportId).ToList();
                if (trendAnalysisReportConfigs.Count() != 0)
                {
                    db.TrendAnalysisReportConfigs.DeleteAllOnSubmit(trendAnalysisReportConfigs);
                }
                var drillinReportConfigs = db.DrillinReportConfigs.Where(x => x.ReportId == reportId).ToList();
                if (drillinReportConfigs.Count() != 0)
                {
                    db.DrillinReportConfigs.DeleteAllOnSubmit(drillinReportConfigs);
                }
                var reportSetting = db.ReportSettings.Where(x => x.Id == reportId).FirstOrDefault();
                if (reportSetting != null)
                {
                    db.ReportSettings.DeleteOnSubmit(reportSetting);
                    deleteStatus = true;
                }
                db.SubmitChanges();


                //if there is no default report left then mark first report as default
                var reportSettingList = db.UserReportSettings.Where(x => x.UserId == userId && x.Type == reportType).ToList();
                if (reportSettingList.Count > 0)
                {
                var totalDefaultReport = reportSettingList.Where(x => x.IsDefault == true).Count();
                if (totalDefaultReport == 0)
                {
                    var firstReport = reportSettingList.FirstOrDefault();
                    if (firstReport != null)
                    {
                        firstReport.IsDefault = true;
                        db.SubmitChanges();
                    }
                }
            }
            }
            return deleteStatus;
        }

        public static bool ChangeReportName(long reportId, string reportType, string reportName)
        {
            bool updateStatus = true;
            try
            {
                using (var db = GetContext())
                {
                    var reportSetting = db.ReportSettings.Where(x => x.Id == reportId && x.Type == reportType).FirstOrDefault();
                    if (reportSetting != null)
                    {
                        reportSetting.ReportName = reportName;
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return updateStatus;
        }


        /// <summary>
        /// To add report favourite ,default and ReportName
        /// </summary>
        /// <param name="isDefault"></param>
        /// <param name="isFavorite"></param>
        /// <param name="reportType"></param>
        /// <param name="reportName"></param>
        /// <param name="reportId"></param>
        /// <param name="userId"></param>
        /// <returns>status of the update</returns>

        public static bool UpdateReportSettings(bool isDefault, bool isFavorite, string reportType, long reportId, Guid userId)
        {
            bool updateStatus = true;
            try
            {
                using (var db = GetContext())
                {
                    var userReportList = db.UserReportSettings.Where(x => x.UserId == userId && x.Type == reportType).ToList();
                    if (userReportList.Count > 0)
                    {
                        var report = userReportList.Where(x => x.ReportId == reportId).FirstOrDefault();
                        if (report != null)
                        {
                            foreach (var item in userReportList)
                            {
                                if (item.ReportId == reportId)
                                {
                                    item.IsDefault = isDefault;
                                    item.IsFavourite = isFavorite;
                                }
                                else if(isDefault)
                                {
                                    item.IsDefault = false;
                                }
                                db.SubmitChanges();
                            }
                        }
                        else
                        {
                            if (isDefault)
                            {
                                foreach (var item in userReportList)
                                {
                                    item.IsDefault = false;
                                    db.SubmitChanges();
                                }
                            }
                            UserReportSetting userSetting = new UserReportSetting();
                            userSetting.ReportId = reportId;
                            userSetting.UserId = userId;
                            userSetting.Type = reportType;
                            userSetting.IsDefault = isDefault;
                            userSetting.IsFavourite = isFavorite;
                            db.UserReportSettings.InsertOnSubmit(userSetting);
                            db.SubmitChanges();
                        }
                        if (!isDefault)
                        {
                            var totalDefaultReport = userReportList.Where(x => x.IsDefault == true && x.ReportId != reportId).Count();
                            if(totalDefaultReport == 0)
                            {
                                var reportSetting = userReportList.FirstOrDefault();
                                reportSetting.IsDefault = true;
                                db.SubmitChanges();
                            }
                        }
                    }
                    else
                    {
                        UserReportSetting userSetting = new UserReportSetting();
                        userSetting.ReportId = reportId;
                        userSetting.UserId = userId;
                        userSetting.Type = reportType;
                        userSetting.IsDefault = true;
                        userSetting.IsFavourite = isFavorite;
                        db.UserReportSettings.InsertOnSubmit(userSetting);
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return updateStatus;

            //bool updateStatus = true;
            //try
            //{
            //    using (var db = GetContext())
            //    {
            //        var report = db.ReportSettings.Where(x => x.UserId == userId && x.Type == reportType).ToList();
            //        if (report != null)
            //        {
            //            foreach (var item in report)
            //            {
            //                if (item.Id == reportId)
            //                {
            //                    item.IsFavourite = isFavorite;
            //                    item.ReportName = reportName;
            //                    item.IsDefault = isDefault;
            //                }
            //                else if (isDefault)
            //                {
            //                    item.IsDefault = false;
            //                }
            //                db.SubmitChanges();
            //            }
            //            if (!isDefault)
            //            {
            //                var totalDefaultReport = report.Where(x => x.IsDefault == true && x.Id != reportId).Count();
            //                if (totalDefaultReport == 0)
            //                {
            //                    var firstReport = report.FirstOrDefault();
            //                    firstReport.IsDefault = true;
            //                    db.SubmitChanges();
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    updateStatus = false;
            //}
            //return updateStatus;
        }

        #endregion

        #region Custom Metric 
        public static bool AddUpdateCustomMetric(DateTime day, int plantId, string metric, string metricValue)
        {
            bool success = false;
            using (var context = GetContext())
            {
                //Check if existing entry is there
                var customMetric = context.DailyPlantSummaries.FirstOrDefault(x => x.DayDateTime == day && x.PlantId == plantId);
                object propertyVal = metricValue;
                if (customMetric == null)
                {
                    //Add entry if does not exist
                    customMetric = new DailyPlantSummary();
                    customMetric.DayDateTime = day;
                    customMetric.PlantId = plantId;

                    System.Reflection.PropertyInfo propInfo = customMetric.GetType().GetProperty(metric);

                    Type propertyType = propInfo.PropertyType;

                    //Convert.ChangeType does not handle conversion to nullable types 
                    var targetType = ReflectionUtils.IsNullableType(propInfo.PropertyType) ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;

                    //Returns an System.Object with the specified System.Type and whose value is equivalent to the specified object.
                    propertyVal = Convert.ChangeType(propertyVal, targetType);

                    if (propInfo != null)
                    {
                        propInfo.SetValue(customMetric, propertyVal);
                    }

                    context.DailyPlantSummaries.InsertOnSubmit(customMetric);
                }
                else
                {
                    //Update entry and field 
                    System.Reflection.PropertyInfo propInfo = customMetric.GetType().GetProperty(metric);

                    Type propertyType = propInfo.PropertyType;

                    var targetType = ReflectionUtils.IsNullableType(propInfo.PropertyType) ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;

                    propertyVal = Convert.ChangeType(propertyVal, targetType);

                    if (propInfo != null)
                    {
                        propInfo.SetValue(customMetric, propertyVal);
                    }
                    context.Refresh(RefreshMode.KeepCurrentValues, customMetric);
                }
                context.SubmitChanges();
                success = true;
            }
            return success;
        }

        public static DailyPlantSummary FindDailyPlantSummary(DateTime day, int plantId)
        {
            using (var context = GetContext())
            {
                var dailyPlantSummary = context.DailyPlantSummaries
                                               .FirstOrDefault(x => x.DayDateTime == day && x.PlantId == plantId);

                return dailyPlantSummary;
            }
        }

        #endregion

        #region Report Configuration 

        public static void AddReportRowConfig(ReportConfiguration item)
        {
            using (var context = GetContext())
            {
                ReportRowConfig reportRowConfig = new ReportRowConfig();
                reportRowConfig.ReportId = item.ReportId;
                reportRowConfig.DisplayName = item.ColumnName;
                reportRowConfig.MetricDefinitionId = item.ColumnId;
                reportRowConfig.Position = item.Order;
                reportRowConfig.CreatedAt = DateTime.Now;
                reportRowConfig.IsVarianceColumn = item.IsVarianceColumn;
                context.ReportRowConfigs.InsertOnSubmit(reportRowConfig);
                context.SubmitChanges();
            }
        }

        public static void AddReportColumnConfig(ReportConfiguration item)
        {
            using (var context = GetContext())
            {
                ReportColumnConfig reportColumnConfig = new ReportColumnConfig();
                reportColumnConfig.ReportId = item.ReportId;
                reportColumnConfig.MetricDefinitionId = item.ColumnId;
                reportColumnConfig.DisplayName = item.DisplayName;
                reportColumnConfig.ColumnName = item.ColumnName;
                reportColumnConfig.Position = item.Order;
                reportColumnConfig.EntityName = item.EntityName;
                reportColumnConfig.EntityRefId = item.EntityRefId.GetValueOrDefault();
                reportColumnConfig.CreatedAt = DateTime.Now;

                context.ReportColumnConfigs.InsertOnSubmit(reportColumnConfig);
                context.SubmitChanges();
            }
        }

        public static void UpdateSepratorStatus(ReportConfiguration item)
        {
            using (var context = GetContext())
            {
                ReportRowConfig reportRowConfig = new ReportRowConfig();
                reportRowConfig = context.ReportRowConfigs.Where(x => x.ReportId == item.ReportId && x.MetricDefinitionId == item.ColumnId).FirstOrDefault();
                if (reportRowConfig != null)
                {
                    reportRowConfig.IsHorizontalSeperator = true;

                    // Update horizontal row for Row config
                    context.Refresh(RefreshMode.KeepCurrentValues, reportRowConfig);
                    context.SubmitChanges();
                }
            }
        }

        public static void UpdateThresholdValues(ReportConfiguration item)
        {
            using (var context = GetContext())
            {
                ReportRowConfig reportRowConfig = new ReportRowConfig();
                reportRowConfig = context.ReportRowConfigs.Where(x => x.ReportId == item.ReportId && x.MetricDefinitionId == item.ColumnId).FirstOrDefault();
                if (reportRowConfig != null)
                {
                    reportRowConfig.ShowActionIcons = item.ShowActionIcons;
                    reportRowConfig.OkLimit = item.OkLimit;
                    //reportRowConfig.CautionLimit = item.CautionLimit;
                    reportRowConfig.WarningLimit = item.WarningLimit;
                    reportRowConfig.ComparisonMetricId = item.ComparisonMetricId;
                    // Update horizontal row for Row config
                    context.Refresh(RefreshMode.KeepCurrentValues, reportRowConfig);
                    context.SubmitChanges();
                }
            }
        }

        public static void DeleteReportConfiguration(long reportId)
        {
            using (var context = GetContext())
            {
                context.ReportColumnConfigs.DeleteAllOnSubmit(context.ReportColumnConfigs.Where(x => x.ReportId == reportId));
                context.ReportRowConfigs.DeleteAllOnSubmit(context.ReportRowConfigs.Where(x => x.ReportId == reportId));
                context.SubmitChanges();
            }
        }

        public static void DeleteDrillInConfiguration(long reportId)
        {
            using (var context = GetContext())
            {
                context.ReportColumnConfigs.DeleteAllOnSubmit(context.ReportColumnConfigs.Where(x => x.ReportId == reportId));
                context.DrillinReportConfigs.DeleteAllOnSubmit(context.DrillinReportConfigs.Where(x => x.ReportId == reportId));
                context.SpecialReportConfigs.DeleteAllOnSubmit(context.SpecialReportConfigs.Where(x => x.ReportId == reportId));
                context.SubmitChanges();
            }
        }

        public static void DeleteDrillInCustomConfiguration(long reportId)
        {
            using (var context = GetContext())
            {
                context.SpecialReportConfigs.DeleteAllOnSubmit(context.SpecialReportConfigs.Where(x => x.ReportId == reportId));

                context.SubmitChanges();
            }
        }

        public static List<ReportRowConfig> GetReportRowConfig(long reportId)
        {
            using (var db = GetContext())
            {
                return db.ReportRowConfigs.Where(x => x.ReportId == reportId).ToList();
            }
        }

        public static List<ReportColumnConfig> GetReportColumnConfig(long reportId)
        {
            using (var db = GetContext())
            {
                return db.ReportColumnConfigs.Where(x => x.ReportId == reportId).ToList();
            }
        }

        public static DrillinReportConfig GetDrillInReportConfig(long reportId)
        {
            using (var context = GetContext())
            {
                return context.DrillinReportConfigs.Where(x => x.ReportId == reportId).FirstOrDefault();
            }
        }

        public static SpecialReportConfig GetDrillInCustomConfig(long reportId)
        {
            using (var context = GetContext())
            {
                return context.SpecialReportConfigs.Where(x => x.ReportId == reportId).FirstOrDefault();
            }
        }

        public static void AddDrillinReportRowConfig(ReportConfiguration item)
        {
            using (var context = GetContext())
            {
                DrillinReportConfig drillInReportConfig = new DrillinReportConfig();
                drillInReportConfig.ReportId = item.ReportId;
                drillInReportConfig.DimensionName = item.DimensionName;
                drillInReportConfig.DimensionDisplayTitle = item.DimensionTitle != null ? item.DimensionTitle : item.DimensionName;
                context.DrillinReportConfigs.InsertOnSubmit(drillInReportConfig);
                context.SubmitChanges();
            }
        }

        public static void AddDrillinCustomReportConfig(ReportConfiguration item)
        {
            using (var context = GetContext())
            {
                SpecialReportConfig drillInCustomReportConfig = new SpecialReportConfig();
                drillInCustomReportConfig.ReportId = item.ReportId;
                drillInCustomReportConfig.CustomFilterDimension = item.CustomFilterDimension;
                drillInCustomReportConfig.SortDirection = item.SortDirection;
                drillInCustomReportConfig.SortCount = item.SortCount;
                drillInCustomReportConfig.SortType = item.SortType;
                context.SpecialReportConfigs.InsertOnSubmit(drillInCustomReportConfig);
                context.SubmitChanges();
            }
        }

        public static TrendAnalysisReportConfig GetTrendReportConfig(long reportId)
        {
            using (var context = GetContext())
            {
                return context.TrendAnalysisReportConfigs.FirstOrDefault(x => x.ReportSettingId == reportId);
            }
        }

        public static void AddTrendAnalysisConfiguration(long reportId, int metricDefinationId, int? targetDefintionId, double upperControlLimit, double lowerControlLimit, bool omitPeriodsWithNoData, bool isScallingAutoFit)
        {
            using (var context = GetContext())
            {
                TrendAnalysisReportConfig trendAnalysisReport = new TrendAnalysisReportConfig();
                trendAnalysisReport.ReportSettingId = reportId;
                trendAnalysisReport.MetricDefinitionId = metricDefinationId;
                trendAnalysisReport.TargetMetricDefinitionId = targetDefintionId.GetValueOrDefault();
                trendAnalysisReport.UpperControlLimit = upperControlLimit;
                trendAnalysisReport.LowerControlLimit = lowerControlLimit;
                trendAnalysisReport.OmitPeriodsWithNoData = omitPeriodsWithNoData;
                trendAnalysisReport.IsScallingAutoFit = isScallingAutoFit;

                context.TrendAnalysisReportConfigs.InsertOnSubmit(trendAnalysisReport);
                context.SubmitChanges();

            }
        }

        public static void DeleteTrendAnalysisConfiguration(long reportId)
        {
            using (var context = GetContext())
            {
                TrendAnalysisReportConfig trendAnalysisReport = context.TrendAnalysisReportConfigs.Where(x => x.ReportSettingId == reportId).FirstOrDefault();
                if (trendAnalysisReport != null)
                {
                    context.TrendAnalysisReportConfigs.DeleteOnSubmit(trendAnalysisReport);
                    context.SubmitChanges();
                }

            }
        }

        #endregion

        #region Get Entity List 
        public static List<ReportDimensionConfig> GetEntityList(string dimensionName)
        {
            IQueryable<ReportDimensionConfig> query = null;
            using (var context = GetContext())
            {
                switch (dimensionName)
                {
                    case "PlantName":
                        query = from s in context.Plants
                                select new ReportDimensionConfig { Id = s.PlantId, Name = s.Name };
                        return query.ToList();
                    case "CustomerName":
                        query = from s in context.Customers
                                select new ReportDimensionConfig { Id = s.CustomerId, Name = s.Name };
                        return query.ToList();
                    case "CustomerSegmentId":
                        query = from s in context.MarketSegments
                                select new ReportDimensionConfig { Id = s.MarketSegmentId, Name = s.Name };
                        return query.ToList();
                    case "SalesStaffName":
                        query = from s in context.SalesStaffs
                                select new ReportDimensionConfig { Id = s.SalesStaffId, Name = s.Name };
                        return query.ToList();
                    case "DistrictName":
                        query = from s in context.Districts
                                select new ReportDimensionConfig { Id = s.DistrictId, Name = s.Name };
                        return query.ToList();
                    case "RegionName":
                        query = from s in context.Regions
                                select new ReportDimensionConfig { Id = s.RegionId, Name = s.Name };
                        return query.ToList();
                    default:
                        return query.ToList();
                }
            }
        }
        public static long GetEntityIdByName(string dimensionName, string metricName)
        {
            using (var context = GetContext())
            {
                switch (metricName)
                {
                    case "PlantName":
                        return context.Plants.FirstOrDefault(x => x.Name.Contains(dimensionName.ToString().Trim())).PlantId;
                    case "CustomerName":
                        return context.Customers.FirstOrDefault(x => x.Name.Contains(dimensionName.ToString().Trim())).CustomerId;
                    case "RegionName":
                        return context.Regions.FirstOrDefault(x => x.Name.Contains(dimensionName.ToString().Trim())).RegionId;
                    case "DistrictName":
                        return context.Districts.FirstOrDefault(x => x.Name.Contains(dimensionName.ToString().Trim())).DistrictId;
                    case "SalesStaffName":
                        return context.SalesStaffs.FirstOrDefault(x => x.Name.Contains(dimensionName.ToString().Trim())).SalesStaffId;
                    case "CustomerSegmentId":
                        return context.MarketSegments.FirstOrDefault(x => x.Name.Contains(dimensionName.ToString().Trim())).MarketSegmentId;
                    case "DriverNumber":
                        return context.DriverDetails.FirstOrDefault(x => x.DriverNumber.Contains(dimensionName.ToString().Trim())).Id;
                    default:
                        return 0;
                }
            }
        }
        #endregion

        #region Report Filter Setting
        public static List<District> GetDistrictFilterSetting(Guid userId)
        {
            using (var db = GetContext())
            {
                var data = from d in db.Districts
                           join du in db.DistrictUsers on d.DistrictId equals du.DistrictId
                           join p in db.Plants on du.DistrictId equals p.DistrictId
                           join ds in db.DistrictSalesStaffs on d.DistrictId equals ds.DistrictId
                           join s in db.SalesStaffs on ds.SalesStaffId equals s.SalesStaffId
                           where du.UserId == userId
                           select d;
                return data.Distinct().ToList();
            }
        }

        public static List<ReportFilterSetting> GetReportFilterSetting(long reportId)
        {
            using (var db = GetContext())
            {
                return db.ReportFilterSettings.Where(x => x.ReportId == reportId).ToList();
            }
        }

        public static ReportSetting GetReportById(long reportId)
        {
            using (var db = GetContext())
            {
                return db.ReportSettings.Where(x => x.Id == reportId).FirstOrDefault();
            }
        }

        public static bool UpdateReportFilterSettings(Dictionary<string, List<long>> reportFilters, long reportId, DateTime? startDate, DateTime? endDate, bool persistFilter = false)
        {
            using (var db = GetContext())
            {
                var allReportFilter = db.ReportFilterSettings.Where(x => x.ReportId == reportId).ToList();
                db.ReportFilterSettings.DeleteAllOnSubmit(allReportFilter);
                db.SubmitChanges();

                foreach (var entityType in reportFilters)
                {
                    if (entityType.Value != null)
                    {
                        foreach (var entityId in entityType.Value)
                        {
                            ReportFilterSetting filterSetting = new ReportFilterSetting();
                            filterSetting.ReportId = reportId;
                            filterSetting.EntityType = entityType.Key;
                            filterSetting.EntityRefId = entityId;
                            filterSetting.CreatedAt = DateTime.Now;

                            db.ReportFilterSettings.InsertOnSubmit(filterSetting);
                            db.SubmitChanges();
                        }
                    }
                }
                var reportSetting = db.ReportSettings.Where(x => x.Id == reportId).FirstOrDefault();
                reportSetting.StartDate = startDate;
                reportSetting.EndDate = endDate;
                db.SubmitChanges();
            }
            return false;
        }
        #endregion 
    }
}
