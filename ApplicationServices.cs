using ApplicationDashboardServiceCommon.Interface;
using ApplicationDashboardServiceCommon.Model;
using ApplicationDashboardServiceCommon.Result;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace StudentServices
{
    public class ApplicationServices
    {
        private readonly IApplicationLOB _applicationLOB;
        private IConfiguration _Configuration { get; set; }
        public ApplicationServices(IApplicationLOB applicationLOB, IConfiguration configuration)
        {
            _applicationLOB = applicationLOB;
            _Configuration = configuration;
        }

        public Result<SchemaTypes> AllApplication(int LOBID) => _applicationLOB.AllApplicationLOB(LOBID);

        public Result<List<LOBSubCategories>> GetLOBSubCategories(string  ApplicationName, int LOBID) => _applicationLOB.GetLOBSubCategories(ApplicationName, LOBID);
        public Result<List<Application_Status>> GetApplicationName_Dashboard_Filter(int LOBID) => _applicationLOB.GetApplicationName_Dashboard_Filter(LOBID);
        public Result<List<ApplicationModel>> GetApplicationName_Filter(int LOBID, int Requester) => _applicationLOB.GetApplicationName_Filter(LOBID, Requester);
        public Result<List<Application_Status>> GetApplicationName_Total() => _applicationLOB.GetApplicationName_Total();
    }
}
