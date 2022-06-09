using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationDashboardServiceCommon.Model
{
  public  class Application_Status
    {
        [JsonProperty]
        public string ApplicationColor { get; set; }
        [JsonProperty]
        public int ApplicationCount { get; set; }
        [JsonProperty]
        public int RequesterCount { get; set; }

    }
}
