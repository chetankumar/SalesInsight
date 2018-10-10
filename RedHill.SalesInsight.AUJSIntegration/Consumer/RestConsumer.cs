using RedHill.SalesInsight.AUJSIntegration.Model;
using RestSharp;
using System.Collections.Generic;
using RedHill.SalesInsight.Logger;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.AUJSIntegration.Consumer
{
    public class RestConsumer
    {
        public string APIBaseURL { get; set; }
        public ILogger Logger { get; set; }

        public RestConsumer()
        {
            this.Logger = new  FileLogger();
        }

        private string GetURL(string baseUrl, string resource)
        {
            return string.Format("{0}/{1}", baseUrl, resource);
        }

        /// <summary>
        /// Gets the response from the API by posting the Payload, using the HTTP POST method
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="payloadWrapper"></param>
        /// <returns></returns>
        public string GetResponse(string resource, PayloadWrapper payloadWrapper = null)
        {
            RestClient client = new RestClient(APIBaseURL);

            RestRequest request = new RestRequest(resource);
            request.Method = Method.POST;

            if (payloadWrapper != null && payloadWrapper.Payload != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Content-type", "application/json");
                request.AddJsonBody(new
                {
                    Payload = new
                    {
                        ClientId = payloadWrapper.ReadPayloadValue("ClientId"),
                        Response = payloadWrapper.ReadPayloadValue("Response")
                    }
                });
                //request.AddJsonBody(payloadWrapper.ToJsonString());
            }

            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        /// <summary>
        /// Gets the response from the API by posting the Payload, using the HTTP POST method
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="payloadObject"></param>
        /// <returns></returns>
        public string GetResponse(string resource, object payloadObject, bool addPayload = true)
        {
            RestClient client = new RestClient(APIBaseURL);

            RestRequest request = new RestRequest(resource);

            request.Method = Method.POST;
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer.ContentType = "application/json; charset=UTF-8";
            if (addPayload)
            {
                request.AddJsonBody(new
                {
                    Payload = payloadObject
                });
            }
            else
            {
                request.JsonSerializer.ContentType = "application/json";

                request.AddJsonBody(payloadObject);
            }

            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        public string GetResponse(string resource, string authToken, object payloadObject)
        {
            RestClient client = new RestClient(APIBaseURL);
            RestRequest request = new RestRequest(resource);

            request.Method = Method.POST;
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer.ContentType = "application/json; charset=UTF-8";
            request.AddJsonBody(new
            {
                AuthToken = authToken,
                Payload = payloadObject
            });
            IRestResponse response = client.Execute(request);
            Logger.LogInfo("### RESPONSE ### ===>"+ response.Content + "### REQUEST ### ===>"+ request.Parameters[0].ToString(),"PayloadResponse");
            return response.Content;
        }

        public string GetResponse(string resource, Dictionary<string, string> queryParams)
        {
            RestClient client = new RestClient(APIBaseURL);

            RestRequest request = new RestRequest(resource);

            request.Method = Method.GET;

            if (queryParams != null && queryParams.Count > 0)
            {
                foreach (var qP in queryParams)
                    request.AddParameter(qP.Key, qP.Value);
            }

            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        /// <summary>
        /// Gets response from the API by passing the provided parameters and http method
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="parameters"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public IRestResponse GetResponse(string resource, Dictionary<string, string> parameters = null, Method? method = null)
        {
            RestClient client = new RestClient(APIBaseURL);

            RestRequest request = new RestRequest(resource);
            request.Method = method.GetValueOrDefault();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var par in parameters)
                    request.AddParameter(par.Key, par.Value);
            }
            return client.Execute(request);
        }
    }
}
