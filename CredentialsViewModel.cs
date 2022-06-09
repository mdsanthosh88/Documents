using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationDashboardServiceCommon.Model
{
    public class CredentialsViewModel
    {
        [JsonProperty]
        [Required(ErrorMessage = "Username cannot be empty")]
        public string UserName { get; set; }
        [JsonProperty]
        [Required(ErrorMessage = "Password cannot be empty")]
        [StringLength(50, ErrorMessage = "Password must be between 6 and 12 characters", MinimumLength = 6)]
        public string Password { get; set; }
    }

    
}
