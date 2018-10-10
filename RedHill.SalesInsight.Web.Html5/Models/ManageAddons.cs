using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ManageAddons
    {
        public SIUser User { get; set; }
        public String CompanyName { get; set; }
        public String SelectMode { get; set; }
        public bool ShowInactives { get; set; }
        public String PriceMode { get; set; }
        public String SearchTerm { get; set; }
        public long? SelectedAddonId { get; set; }
        public AddonView SelectedAddon { get; set; }
        public bool ShowEditModal { get; set; }
        public long ExpandUOMId { get; set; }
        public decimal SavePrice { get; set; }
        public int SelectedDistrict { get; set; }
        public long SaveUOMId { get; set; }
        public int FutureMonth { get; set; }
        public int ResetExpandUOM { get; set; }

        public List<AddonProjectionView> DistrictProjections { get; set; }
        public String MonthYear { get; set; }
         
        public Company Company {
            get
            {
               return SIDAL.GetCompany();
            }
                
         }
        public SelectList CurrentUOMSelection
        {
            get
            {
                if (SelectedAddon != null)
                {
                    if (PriceMode == "QUOTE")
                    {
                        return SelectedAddon.ChooseQuoteUOM;
                    }
                    else if (PriceMode == "MIX")
                    {
                        return SelectedAddon.ChooseMixUOM;
                    }
                }
                return null;
            }
        }

        public SelectList ChooseTargetUOM
        {
            get
            {
                if (SelectedAddon != null)
                {
                    if (PriceMode == "QUOTE")
                    {
                        return new SelectList(SIDAL.GetUOMS(SelectedAddon.QuoteUOM), "Id", "Name", ExpandUOMId);
                    }
                    else if (PriceMode == "MIX")
                    {
                        return new SelectList(SIDAL.GetUOMS(SelectedAddon.MixUOM), "Id", "Name", ExpandUOMId);
                    }
                }
                return null;
            }
        }

        public SelectList SaveTargetUOM
        {
            get
            {
                if (SelectedAddon != null)
                {
                    if (PriceMode == "QUOTE")
                    {
                        return new SelectList(SIDAL.GetUOMS(SelectedAddon.QuoteUOM), "Id", "Name", ExpandUOMId);
                    }
                    else if (PriceMode == "MIX")
                    {
                        return new SelectList(SIDAL.GetUOMS(SelectedAddon.MixUOM), "Id", "Name", ExpandUOMId);
                    }
                }
                return null;
            }
        }

        public List<Addon> Addons
        {
            get
            {
                return SIDAL.GetAddons(ShowInactives, SearchTerm,null);
            }
        }

        public ManageAddons()
        {
            this.PriceMode = "QUOTE";
            this.ProjectionDate = DateTime.Today;
        }

        public ManageAddons(long? id)
        {
            this.PriceMode = "QUOTE";
            this.ProjectionDate = DateTime.Today;
            this.SelectedAddonId = id;
            LoadValues();
        }

        public void LoadValues()
        {
            if (SelectedAddonId.HasValue && SelectedAddonId.Value > 0 && SelectMode == "EDIT")
            {
                SelectedAddon = new AddonView(SIDAL.FindAddon(SelectedAddonId.Value));
                ShowEditModal = true;
            }
            else if (SelectedAddonId.HasValue && SelectedAddonId.Value > 0 && SelectMode == "EXPAND")
            {
                if (User != null)
                {
                    SelectedAddon = new AddonView(SIDAL.FindAddon(SelectedAddonId.Value));
                    List<District> districts = SIDAL.GetDistricts(User.UserId).ToList();
                    DistrictProjections = new List<AddonProjectionView>();

                    // For transitions between accordion, we need to reset the selected target UOM. For example,
                    // If the first addon has only "NA" and the new addon has "CY, Lts" then first the UOM needs to
                    // be cleared, else the new addon will open with the target UOM "NA"
                    if (ResetExpandUOM>0)
                    {
                        ExpandUOMId = 0;
                        ResetExpandUOM = 0;
                    }

                    if (ExpandUOMId == 0)
                    {
                        if (PriceMode == "QUOTE" && SelectedAddon.QuoteUOM != null)
                        {
                            ExpandUOMId = SelectedAddon.QuoteUOM.Value;
                        }
                        else if (PriceMode == "MIX" && SelectedAddon.MixUOM != null)
                        {
                            ExpandUOMId = SelectedAddon.MixUOM.Value;
                        }
                    }
                    foreach (District d in districts)
                    {
                        AddonProjectionView view = new AddonProjectionView(SelectedAddonId.Value, d, ProjectionDate.Value,PriceMode,ExpandUOMId);
                        DistrictProjections.Add(view);
                    }
                }

            }
            else
            {
                this.SelectedAddon = new AddonView();
            }
        }

        public DateTime? ProjectionDate
        {
            get
            {
                if (MonthYear != null && !MonthYear.Trim().Equals(""))
                {
                    try
                    {
                        return DateTime.ParseExact("1 " + MonthYear, "d MMM, yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return DateTime.Now;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                    MonthYear = value.Value.ToString("MMM, yyyy");
            }
        }

        public void UpdatePrice()
        {
            if (SelectedDistrict > 0 && SelectedAddonId.GetValueOrDefault(0) > 0)
            {
                SIDAL.UpdateAddonProjection(SelectedAddonId.GetValueOrDefault(), SelectedDistrict, ProjectionDate.Value.AddMonths(FutureMonth), SavePrice, SaveUOMId,PriceMode);
                SelectedDistrict = 0;
                SaveUOMId = 0;
                SavePrice = 0;
                FutureMonth = 0;
            }
        }
    }
}