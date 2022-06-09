using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationDashboardServiceCommon.Model
{
    public class LOBSubCategories
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string ApplicationName { get; set; }
       
        [JsonProperty]
        public string Configuration_Item { get; set; }
        [JsonProperty]
        public string TicketPriority { get; set; }
        [JsonProperty]
        public string TicketNo { get; set; }
        [JsonProperty]
        public string TicketStatus { get; set; }
        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public int Requester { get; set; }

        [JsonProperty]
        public string Work_Notes { get; set; }
         
        [JsonProperty]
        public string Createdby { get; set; }
        [JsonProperty]
        public string CreatedDate { get; set; }

    }
}
