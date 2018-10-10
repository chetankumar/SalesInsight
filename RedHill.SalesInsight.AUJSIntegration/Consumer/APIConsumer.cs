using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.AUJSIntegration.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using RedHill.SalesInsight.DAL.Utilities;

namespace RedHill.SalesInsight.AUJSIntegration.Consumer
{
    public class APIConsumer : RestConsumer
    {
        private List<Addon> AddOns { get; set; }
        private List<BlockProduct> BlockProducts { get; set; }
        private List<AggregateProduct> AggregateProducts { get; set; }

        public string Nonce { get; set; }
        public bool LoginSuccessful { get; set; }
        public string AuthToken { get; set; }

        public APIConsumer(string baseUrl, string clientId, string clientKey)
        {
            this.APIBaseURL = baseUrl;
            Logon(clientId, clientKey);
        }

        private string GetNonce(string clientId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("ClientId", clientId);

            PayloadWrapper response = PayloadWrapper.FromJson(GetResponse("nonce", parameters));

            if (response != null)
                this.Nonce = response.ReadPayloadValue("Nonce");

            return this.Nonce;
        }

        /// <summary>
        /// Log in using the given client id and nonce. If nonce is not provided it'll try to generate the nonce
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        private void Logon(string clientId, string clientKey, string nonce = null)
        {
            if (clientId == null || clientId.Trim().Length == 0)
                throw new ArgumentException("Invalid ClientId provided");

            if (clientKey == null || clientKey.Trim().Length == 0)
                throw new ArgumentException("Invalid ClientKey provided");

            //If nonce is not provided get the nonce
            if (nonce == null || nonce.Trim().Length == 0)
                nonce = GetNonce(clientId);

            if (nonce == null || nonce.Trim().Length == 0)
            {
                throw new ArgumentException("Could not generate nonce");
            }

            //Get the logon Token
            string logonToken = GetLogonToken(clientId, clientKey, nonce);

            this.LoginSuccessful = !(this.AuthToken == null || this.AuthToken.Trim().Length == 0);
        }

        private string GetLogonToken(string clientId, string clientKey, string nonce)
        {
            string responseHash = GenerateResponseHash(clientKey, nonce);

            var requestPayload = new
            {
                ClientId = clientId,
                Response = responseHash
            };

            var payloadResponse = PayloadWrapper.FromJson(GetResponse("logon", requestPayload));

            if (payloadResponse != null)
                this.AuthToken = payloadResponse.ReadPayloadValue("AuthToken");

            return this.AuthToken;
        }

        private string GenerateResponseHash(string clientKey, string nonce)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientKey)))
            {
                if (nonce == null || nonce.Trim().Length == 0)
                    return null;
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(nonce)));
            }
        }

        public T ProcessResponse<T>(IRestResponse response, Func<JToken, T> onSuccess, Func<string, T> onFailure)
        {
            string responseText = response.Content;

            if (responseText == null || responseText.Trim().Length == 0)
                return default(T);

            JObject responseObj = JObject.Parse(responseText);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JToken payload = responseObj["Payload"];
                if (payload != null)
                {
                    return onSuccess(payload);
                }
            }

            JToken errorObj = responseObj["Error"];
            string statusCode = errorObj != null ? errorObj.Value<string>("Code") : response.StatusCode.ToString();
            string errorMessage = errorObj != null ? errorObj.Value<string>("Message") : responseText;
            return onFailure(string.Format("{0} ({1})", errorMessage, statusCode));
        }

        public Dictionary<Type, object> GetLookupItems(long? timeStamp = null)
        {
            var requestPayload = new Dictionary<string, string>();

            requestPayload.Add("AuthToken", AuthToken);

            if (timeStamp != null && timeStamp.HasValue)
                requestPayload.Add("TimeStamp", timeStamp.Value.ToString());

            var response = GetResponse("lookups", requestPayload, method: Method.GET);

            var lookUpItems = new Dictionary<Type, object>();

            #region MarketSegments

            //Parse Market Segments
            var marketSegments = ProcessResponse<List<MarketSegment>>(response, payload =>
            {
                List<MarketSegment> marketSegmentList = new List<MarketSegment>();
                MarketSegment marketSegment = null;

                //FileUtils.SaveJson(payload["JobTypes"].ToString(), "Brannan_MarketSegment" + FileUtils.GetTimeStampForFile(), "Brannan");

                foreach (var c in (JArray)payload["JobTypes"])
                {
                    marketSegment = new MarketSegment();
                    marketSegment.DispatchId = c.Value<string>("Id");
                    marketSegment.Name = c.Value<string>("Description");

                    marketSegmentList.Add(marketSegment);
                }
                return marketSegmentList;
            },
            err =>
            {
                //TODO: Log err here
                return null;
            });

            //Add Plants to lookup item list
            lookUpItems.Add(typeof(MarketSegment), marketSegments);

            #endregion

            #region Plants

            //Parse Plants
            var plants = ProcessResponse<List<Plant>>(response, payload =>
            {
                List<Plant> plantList = new List<Plant>();
                Plant plant = null;

                //FileUtils.SaveJson(payload["Plants"].ToString(), "Brannan_Plants" + FileUtils.GetTimeStampForFile(), "Brannan");

                foreach (var c in (JArray)payload["Plants"])
                {
                    plant = new Plant()
                    {
                        DispatchId = c.Value<string>("Id"),
                        Name = c.Value<string>("Name"),
                        Latitude = c.Value<string>("Latitude"),
                        Longitude = c.Value<string>("Longitude"),
                        Active = !c.Value<bool>("Inactive"),
                        ProductTypeId = c.Value<int>("SystemTypeId")
                    };

                    //string systemTypeId = c.Value<string>("SystemTypeId");

                    ////As per the requirement only import plants with System Type ID = 1
                    //if ("1".Equals(systemTypeId))
                    plantList.Add(plant);
                }
                return plantList;
            },
            err =>
            {
                //TODO: Log err here
                return null;
            });

            //Add Plants to lookup item list
            lookUpItems.Add(typeof(Plant), plants);

            #endregion

            #region RawMaterial Types

            //Parse Plants
            var rawMaterialTypes = ProcessResponse<List<RawMaterialType>>(response, payload =>
            {
                List<RawMaterialType> rawMaterialTypeList = new List<RawMaterialType>();
                RawMaterialType rawMaterialType = null;

                //FileUtils.SaveJson(payload["RawMaterialTypes"].ToString(), "Brannan_RawMaterialTypes" + FileUtils.GetTimeStampForFile(), "Brannan");

                foreach (var c in (JArray)payload["RawMaterialTypes"])
                {
                    rawMaterialType = new RawMaterialType();
                    rawMaterialType.DispatchId = c.Value<string>("Id");
                    rawMaterialType.Name = c.Value<string>("Description");

                    rawMaterialTypeList.Add(rawMaterialType);
                }
                return rawMaterialTypeList;
            },
            err =>
            {
                //TODO: Log err here
                return null;
            });

            //Add Plants to lookup item list
            lookUpItems.Add(typeof(RawMaterialType), rawMaterialTypes);

            #endregion

            #region Status Types

            //Parse Status Types
            var statusTypes = ProcessResponse<List<ProjectStatus>>(response, payload =>
            {
                List<ProjectStatus> statusTypeList = new List<ProjectStatus>();
                ProjectStatus projectStatus = null;

                //FileUtils.SaveJson(payload["StatusTypes"].ToString(), "Brannan_StatusTypes" + FileUtils.GetTimeStampForFile(), "Brannan");

                foreach (var c in (JArray)payload["StatusTypes"])
                {
                    projectStatus = new ProjectStatus();
                    projectStatus.DispatchId = c.Value<string>("Id");
                    projectStatus.Name = c.Value<string>("Description");

                    statusTypeList.Add(projectStatus);
                }
                return statusTypeList;
            },
            err =>
            {
                //TODO: Log err here
                return null;
            });

            //Add Plants to lookup item list
            lookUpItems.Add(typeof(ProjectStatus), statusTypes);

            #endregion

            #region Tax Schedule

            //Parse Status Types
            var taxSchedules = ProcessResponse<List<TaxCode>>(response, payload =>
            {
                List<TaxCode> taxScheduleList = new List<TaxCode>();
                TaxCode taxCode = null;

                //FileUtils.SaveJson(payload["TaxSchedule"].ToString(), "Brannan_TaxSchedule" + FileUtils.GetTimeStampForFile(), "Brannan");

                foreach (var c in (JArray)payload["TaxSchedule"])
                {
                    taxCode = new TaxCode();
                    taxCode.DispatchId = c.Value<string>("Id");
                    taxCode.Description = c.Value<string>("Description");

                    //Add Unique dispatch id to code, since code is not coming from the API
                    taxCode.Code = taxCode.DispatchId;

                    taxScheduleList.Add(taxCode);
                }
                return taxScheduleList;
            },
            err =>
            {
                //TODO: Log err here
                return null;
            });

            //Add Plants to lookup item list
            lookUpItems.Add(typeof(TaxCode), taxSchedules);

            #endregion

            #region Unit Of Measures

            //Parse Status Types
            var uomTypes = ProcessResponse<List<Uom>>(response, payload =>
            {
                List<Uom> utomTypeList = new List<Uom>();
                Uom uom = null;

                //FileUtils.SaveJson(payload["UnitOfMeasureTypes"].ToString(), "Brannan_UnitOfMeasureTypes" + FileUtils.GetTimeStampForFile(), "Brannan");

                foreach (var c in (JArray)payload["UnitOfMeasureTypes"])
                {
                    uom = new Uom();
                    uom.DispatchId = c.Value<string>("Id");
                    uom.Name = c.Value<string>("Description");

                    uom.Category = uom.Name;
                    uom.BaseConversion = 0;
                    uom.Priority = 1;
                    uom.Active = true;

                    utomTypeList.Add(uom);
                }
                return utomTypeList;
            },
            err =>
            {
                //TODO: Log err here
                return null;
            });

            //Add Plants to lookup item list
            lookUpItems.Add(typeof(Uom), uomTypes);

            #endregion

            return lookUpItems;
        }

        public List<Customer> GetCustomers(long? timeStamp = null)
        {
            var requestPayload = new Dictionary<string, string>();

            requestPayload.Add("AuthToken", AuthToken);

            if (timeStamp != null && timeStamp.HasValue)
                requestPayload.Add("TimeStamp", timeStamp.Value.ToString());

            var response = GetResponse("customers", requestPayload, method: Method.GET);

            //FileUtils.SaveJson(response.Content.ToString(), "Brannan_Customers" + FileUtils.GetTimeStampForFile(), "Brannan");

            List<Customer> customers = new List<Customer>();
            var customerList = ProcessResponse<List<Customer>>(response, payload =>
            {
                Customer cust = null;
                foreach (var c in (JArray)payload["Customers"])
                {
                    cust = new Customer()
                    {
                        Name = c.Value<string>("Name"),
                        CustomerNumber = c.Value<string>("Number"),
                        DispatchId = c.Value<string>("Id"),
                        Address1 = c.Value<string>("Address1"),
                        Address2 = c.Value<string>("Address2"),
                        City = c.Value<string>("City"),
                        State = c.Value<string>("State"),
                        Zip = c.Value<string>("Zip"),
                        Source = AppSettings.APISource,
                        Active = !c.Value<bool>("Inactive"),
                        APIActiveStatus = !c.Value<bool>("Inactive"),
                        PurchaseConcrete = c.Value<bool>("PurchaseConcrete"),
                        PurchaseAggregate = c.Value<bool>("PurchaseAggregate"),
                        PurchaseBlock = c.Value<bool>("PurchaseBlock")
                    };
                    customers.Add(cust);
                }
                return customers;
            },
            err =>
            {
                //TODO: Log err here
                return null;
            });

            return customerList;
        }

        public List<SalesStaff> GetSalesPeople(long? timeStamp = null)
        {
            var requestPayload = new Dictionary<string, string>();

            requestPayload.Add("AuthToken", AuthToken);

            if (timeStamp != null && timeStamp.HasValue)
                requestPayload.Add("TimeStamp", timeStamp.Value.ToString());

            var response = GetResponse("salespeople", requestPayload, method: Method.GET);

            //FileUtils.SaveJson(response.Content.ToString(), "Brannan_SalesPeople" + FileUtils.GetTimeStampForFile(), "Brannan");

            List<SalesStaff> staffList = new List<SalesStaff>();
            var staffMemberList = ProcessResponse<List<SalesStaff>>(response, payload =>
            {
                SalesStaff staff = null;
                foreach (var c in (JArray)payload["SalesPeople"])
                {
                    staff = new SalesStaff()
                    {
                        DispatchId = c.Value<string>("Id"),
                        Name = c.Value<string>("Name"),
                        Phone = c.Value<string>("Phone"),
                        Email = c.Value<string>("Email"),
                        Active = !c.Value<bool>("Inactive")
                    };
                    staffList.Add(staff);
                }
                return staffList;
            },
            err =>
            {
                //TODO: Log err here
                return null;
            });

            return staffMemberList;
        }

        public List<RawMaterial> GetRawMaterials(long? timeStamp = null)
        {
            var requestPayload = new Dictionary<string, string>();

            requestPayload.Add("AuthToken", AuthToken);

            if (timeStamp != null && timeStamp.HasValue)
                requestPayload.Add("TimeStamp", timeStamp.Value.ToString());

            var response = GetResponse("rawmaterials", requestPayload, method: Method.GET);

            //FileUtils.SaveJson(response.Content.ToString(), "Brannan_RawMaterial" + FileUtils.GetTimeStampForFile(), "Brannan");

            var rawMaterials = ProcessResponse<List<RawMaterial>>(response, payload =>
            {
                List<RawMaterial> rawMaterialList = new List<RawMaterial>();
                RawMaterial rawMaterial = null;
                RawMaterialCostProjection rmCostProjection = null;
                foreach (var c in (JArray)payload["RawMaterials"])
                {
                    rawMaterial = new RawMaterial();
                    rawMaterial.DispatchId = c.Value<string>("Id");
                    //rawMaterial.UomDispatchId = c.Value<string>("UomId");
                    rawMaterial.MaterialCode = c.Value<string>("Number");
                    rawMaterial.Description = c.Value<string>("Description");
                    rawMaterial.Active = !c.Value<bool>("Inactive");
                    rawMaterial.RawMaterialTypeDispatchId = c.Value<long>("RawMaterialTypeId");

                    //Initialize the cost projections list
                    rawMaterial.CostProjections = new List<RawMaterialCostProjection>();

                    //Get the Raw material Projections here
                    foreach (var p in (JArray)c["Plants"])
                    {
                        rmCostProjection = new RawMaterialCostProjection();
                        //cost as specified in requirement
                        rmCostProjection.Cost = p.Value<decimal>("CurrentCost") + p.Value<decimal>("HaulCost");
                        rmCostProjection.PlantDispatchId = p.Value<string>("Id");
                        rmCostProjection.UomDispatchId = p.Value<string>("UomId");

                        rawMaterial.CostProjections.Add(rmCostProjection);
                    }

                    rawMaterialList.Add(rawMaterial);
                }
                return rawMaterialList;
            },
            err =>
            {
                //TODO: Log error 
                return null;
            });

            return rawMaterials;
        }

        public List<StandardMixConstituent> GetProducts(long? timeStamp = null)
        {
            var requestPayload = new Dictionary<string, string>();

            requestPayload.Add("AuthToken", AuthToken);

            if (timeStamp != null && timeStamp.HasValue)
                requestPayload.Add("TimeStamp", timeStamp.Value.ToString());

            var response = GetResponse("products", requestPayload, method: Method.GET);

            #region Mix Constituents

            var mixConstituents = ProcessResponse<List<StandardMixConstituent>>(response, payload =>
            {
                List<StandardMixConstituent> mixConstituentList = new List<StandardMixConstituent>();
                StandardMixConstituent standardMixConstituent = null;

                var rawMaterials = SIDAL.GetRawMaterials(true);
                var uoms = SIDAL.GetUOMS();

                //FileUtils.SaveJson(response.Content.ToString(), "Brannan_StandardConstituents" + FileUtils.GetTimeStampForFile(), "Brannan");

                foreach (var c in (JArray)payload["Concrete"])
                {
                    //Create the StandardMix
                    StandardMix standardMix = new StandardMix();
                    standardMix.DispatchId = c.Value<string>("Id");
                    standardMix.Number = c.Value<string>("Number");
                    standardMix.Description = c.Value<string>("Description");
                    standardMix.SalesDesc = standardMix.Description;
                    standardMix.Active = !c.Value<bool>("Inactive");

                    Int32.TryParse(c.Value<string>("PSI"), out int psi);
                    standardMix.PSI = psi;
                    standardMix.Slump = c.Value<string>("Slump");
                    standardMix.Air = c.Value<string>("Air");
                    standardMix.MD1 = c.Value<string>("WaterToCementRatio");
                    standardMix.MD2 = c.Value<string>("SackEquivalent");
                    standardMix.MD3 = c.Value<string>("AggregateSize");
                    standardMix.MD4 = c.Value<string>("CementType");

                    //create mix formulations
                    var plants = c["Plants"];
                    if (plants != null)
                    {
                        foreach (var p in plants)
                        {
                            Plant plant = SIDAL.FindDuplicateByDispatchId<Plant>(new Plant() { DispatchId = p.Value<string>("Id") });

                            //if plant not found just continue 
                            if (plant == null)
                                continue;

                            MixFormulation mixFormulation = new MixFormulation();
                            mixFormulation.PlantId = plant.PlantId;
                            mixFormulation.StandardMix = standardMix;

                            var materials = p["Materials"];
                            foreach (var mat in materials)
                            {
                                standardMixConstituent = new StandardMixConstituent();
                                standardMixConstituent.MixFormulation = mixFormulation;

                                standardMixConstituent.RawMaterialId = rawMaterials.Where(x => x.DispatchId == mat.Value<string>("Id"))
                                                                                   .Select(x => x.Id)
                                                                                   .FirstOrDefault();
                                standardMixConstituent.UomId = uoms.Where(x => x.DispatchId == mat.Value<string>("UomId"))
                                                                   .Select(x => x.Id)
                                                                   .FirstOrDefault();
                                standardMixConstituent.Quantity = mat.Value<double>("Quantity");

                                mixConstituentList.Add(standardMixConstituent);
                            }
                        }
                    }
                }
                return mixConstituentList;
            },
            err =>
            {
                //TODO: Log error 
                return null;
            });

            #endregion

            #region AddOns

            this.AddOns = ProcessResponse(response, payload =>
            {
                List<Addon> addOns = new List<Addon>();

                Addon addOn = null;
                AddonPriceProjection addOnPriceProjection = null;
                foreach (var o in (JArray)payload["Other"])
                {
                    addOn = new Addon()
                    {
                        DispatchId = o.Value<string>("Id"),
                        Code = o.Value<string>("Number"),
                        Description = o.Value<string>("Description"),
                        UomDispatchId = o.Value<string>("UomId"),
                        Active = !o.Value<bool>("Inactive"),
                        PriceProjections = new List<AddonPriceProjection>()
                    };

                    addOnPriceProjection = new AddonPriceProjection()
                    {
                        Price = o.Value<decimal>("Price")
                    };

                    addOn.PriceProjections.Add(addOnPriceProjection);

                    addOns.Add(addOn);
                }

                return addOns;
            },
            err =>
            {
                //TODO: Log error 
                return null;
            });

            #endregion

            #region Aggregate Products

            this.AggregateProducts = ProcessResponse<List<AggregateProduct>>(response, payload =>
            {
                List<AggregateProduct> aggProducts = new List<AggregateProduct>();
                var aggResponse = (JArray)payload["Aggregate"];
                AggregateProduct aggProduct = null;
                AggregateProductPriceProjection aggProductPriceProjection = null;
                if (aggResponse != null)
                {
                foreach (var bR in aggResponse)
                {
                    //Create the Block Product
                    aggProduct = new AggregateProduct()
                    {
                        DispatchId = bR.Value<string>("Id"),
                        Code = bR.Value<string>("Number"),
                        Description = bR.Value<string>("Description"),
                        UomDispatchId = bR.Value<string>("UomId"),
                        Active = !bR.Value<bool>("Inactive")
                    };

                    var plants = bR["Plants"];
                    if (plants != null)
                    {
                        aggProduct.AggregateProductPriceProjections = new System.Data.Linq.EntitySet<AggregateProductPriceProjection>();


                        //Change date should be set to the first day of given month
                        DateTime changeDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);

                        foreach (var p in plants)
                        {
                            Plant plant = SIDAL.FindDuplicateByDispatchId<Plant>(new Plant() { DispatchId = p.Value<string>("Id") });

                            //if plant not found just continue 
                            if (plant == null)
                                continue;

                            aggProductPriceProjection = new AggregateProductPriceProjection()
                            {
                                PlantId = plant.PlantId,
                                Price = bR.Value<decimal?>("Price"),
                                Active = true,
                                AggregateProduct = aggProduct,
                                ChangeDate = changeDate,
                                CreatedAt = DateTime.Now
                            };

                            aggProduct.AggregateProductPriceProjections.Add(aggProductPriceProjection);
                        }
                    }

                    aggProducts.Add(aggProduct);
                }
                }
                return aggProducts;
            },
            err =>
            {
                return null;
            });

            #endregion

            #region Block Products

            this.BlockProducts = ProcessResponse<List<BlockProduct>>(response, payload =>
            {
                List<BlockProduct> blockProducts = new List<BlockProduct>();
                var blockResponse = (JArray)payload["Block"];
                BlockProduct blockProduct = null;
                BlockProductPriceProjection blockProductPriceProjection = null;
                if (blockResponse != null)
                {
                    foreach (var bR in blockResponse)
                    {
                        //Create the Block Product
                        blockProduct = new BlockProduct()
                        {
                            DispatchId = bR.Value<string>("Id"),
                            Code = bR.Value<string>("Number"),
                            Description = bR.Value<string>("Description"),
                            UomDispatchId = bR.Value<string>("UomId"),
                            Active = !bR.Value<bool>("Inactive")
                        };

                        var plants = bR["Plants"];
                        if (plants != null)
                        {
                            blockProduct.BlockProductPriceProjections = new System.Data.Linq.EntitySet<BlockProductPriceProjection>();

                            DateTime changeDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);

                            foreach (var p in plants)
                            {
                                Plant plant = SIDAL.FindDuplicateByDispatchId<Plant>(new Plant() { DispatchId = p.Value<string>("Id") });

                                //if plant not found just continue 
                                if (plant == null)
                                    continue;

                                blockProductPriceProjection = new BlockProductPriceProjection()
                                {
                                    PlantId = plant.PlantId,
                                    Price = bR.Value<decimal?>("Price"),
                                    Active = true,
                                    BlockProduct = blockProduct,
                                    ChangeDate = changeDate,
                                    CreatedAt = DateTime.Now
                                };

                                blockProduct.BlockProductPriceProjections.Add(blockProductPriceProjection);
                            }
                        }

                        blockProducts.Add(blockProduct);
                    }
                }
                return blockProducts;
            },
            err =>
            {
                return null;
            });

            #endregion

            return mixConstituents;
        }

        public List<Addon> GetAddOns(long? timeStamp = null)
        {
            //Check if addOns exist in Memory, if not fetch 'em from the API
            if (this.AddOns == null)
                this.GetProducts(timeStamp);
            return this.AddOns;
        }

        public List<BlockProduct> GetBlockProducts(long? timeStamp = null)
        {
            if (this.BlockProducts == null)
                this.GetProducts(timeStamp);
            return this.BlockProducts;
        }

        public List<AggregateProduct> GetAggregateProducts(long? timeStamp = null)
        {
            if (this.AggregateProducts == null)
                this.GetProducts(timeStamp);
            return this.AggregateProducts;
        }

        public bool PushQuote(QuotePayload quotePayload)
        {
            bool success = false;
            try
            {
                //var requestPayload = new JObject();
                //requestPayload.Add("AuthToken", this.AuthToken);
                //requestPayload.Add("Payload", JObject.FromObject(quotePayload));
                var payloadResponse = PayloadWrapper.FromJson(GetResponse("addquote", this.AuthToken, quotePayload));
                success = payloadResponse.ReadPayloadValue<bool>("Success");
            }
            catch (Exception ex)
            {
            }
            return success;
        }

        public bool PushRawQuote(object quotePayload)
        {
            bool success = false;
            try
            {
                var payloadResponse = PayloadWrapper.FromJson(GetResponse("addquote", this.AuthToken, quotePayload));

                success = payloadResponse.ReadPayloadValue<bool>("Success");
            }
            catch (Exception ex)
            {
                //TODO: Log exception here
            }
            return success;
        }
    }
}
