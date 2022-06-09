using ApplicationDashboardServiceCommon.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System;

namespace StudnetHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationLOB _applicationLOB;
        ILogger<ApplicationController> _logger;
        public ApplicationController(IApplicationLOB applicationLOB, ILogger<ApplicationController> logger)
        {
            this._applicationLOB = applicationLOB;
            this._logger = logger;
        }

        [HttpGet]
        public IActionResult GetbyLOBID(int LOBID)
        {
            _logger.LogDebug("Index was called");
            dynamic response = _applicationLOB.AllApplicationLOB(Convert.ToInt32(LOBID));
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            { return StatusCode(200, response.Value); }
        }
        [HttpGet("GetSubCategories")]
        public IActionResult GetSubCategories(string ApplicationName,int LOBID)
        {
            _logger.LogDebug("Index was called");
            dynamic response = _applicationLOB.GetLOBSubCategories(ApplicationName, Convert.ToInt32(LOBID));
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            { return StatusCode(200, response.Value); }
        }

        [HttpGet("GetApplicationName_Categories")]
        public IActionResult GetApplicationName_Dashboard_Filter(int LOBID)
        {
            _logger.LogDebug("Index was called");
            dynamic response = _applicationLOB.GetApplicationName_Dashboard_Filter(Convert.ToInt32(LOBID));
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            { return StatusCode(200, response.Value); }
        }

        [HttpGet("GetApplicationName")]
        public IActionResult GetApplicationName_Filter( int LOBID, int Requester)
        {
            _logger.LogDebug("Index was called");
            dynamic response = _applicationLOB.GetApplicationName_Filter(Convert.ToInt32(LOBID), Convert.ToInt32(Requester));
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            { return StatusCode(200, response.Value); }
        }

        [HttpGet("GetApplicationNameSummary")]
        public IActionResult Get_ApplicationName_Total()
        {
            _logger.LogDebug("Index was called");
            dynamic response = _applicationLOB.GetApplicationName_Total();
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            { return StatusCode(200, response.Value); }
        }

        
    }
}
