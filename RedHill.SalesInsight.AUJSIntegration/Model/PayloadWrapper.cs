using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration.Model
{
    public class PayloadWrapper
    {
        public Dictionary<string, string> Payload { get; set; }

        public PayloadWrapper()
        {
            this.Payload = new Dictionary<string, string>();
        }

        public string ReadPayloadValue(string key)
        {
            if (this.Payload == null || this.Payload.Count == 0 || this.Payload.ContainsKey(key) == false)
                return null;
            return this.Payload[key];
        }

        public T ReadPayloadValue<T>(string key)
        {
            return (T)Convert.ChangeType(ReadPayloadValue(key), typeof(T));
        }

        public static PayloadWrapper FromJson(string json)
        {
            var obj = JsonConvert.DeserializeObject<PayloadWrapper>(json);
            return obj;
        }

        #region To JSON

        public JObject ToJson()
        {
            JObject mainObj = new JObject();
            JObject obj = null;
            if (this.Payload != null && this.Payload.Count > 0)
            {
                obj = new JObject();
                foreach (var item in this.Payload)
                    obj.Add(item.Key, (JToken)item.Value);
            }

            mainObj.Add("Payload", obj);

            return mainObj;
        }

        public string ToJsonString()
        {
            var obj = ToJson();
            return obj == null ? "" : obj.ToString();
        }

        #endregion
    }
}
