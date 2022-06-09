using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationDashboardServiceCommon.Model
{
    public class ApplicationModel
    {


        [JsonProperty]
        public string ApplicationName { get; set; }
        [JsonProperty]
        public string Requester { get; set; }
        
        [JsonProperty]
        public int ApplicationNameCount { get; set; }

        [JsonProperty]
        public int RequesterCount { get; set; }

        [JsonProperty]
        public string ConfigurationItem { get; set; }
        [JsonProperty]
        public string TicketPriority { get; set; }

       
    }

    public class ApplicationModellist
    {
          public List<string> Data { get; set; }
    }

    public class SchemaTypes
    {


        [JsonProperty]
        public string Schema { get; set; }
        [JsonProperty]
        public List<string> Columns { get; set; }

        [JsonProperty]
        public List<string> DataTypes { get; set; }

        [JsonProperty]
        public List<Array> Data { get; set; }


    }


}
