using ApplicationDashboardServiceCommon.Interface;
using ApplicationDashboardServiceCommon.Model;
using ApplicationDashboardServiceCommon.Result;
using ApplicationDashboardServiceRepository.DB;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ApplicationDashboardServiceRepository
{
    public class ApplicationLOBRepository: IApplicationLOB
    {
        private readonly SqlHelper _sqlHelper;
        public ApplicationLOBRepository(IConfiguration configuration) => _sqlHelper = new SqlHelper(configuration);
        public Result<SchemaTypes> AllApplicationLOB(int LOBID) => _sqlHelper.GetAllApplication(LOBID);
        public Result<List<Application_Status>> GetApplicationName_Dashboard_Filter(int LOBID) => _sqlHelper.GetApplicationName_Dashboard_Filter(LOBID);
        public Result<List<ApplicationModel>> GetApplicationName_Filter(int LOBID, int Requester) => _sqlHelper.GetApplicationName_Filter(LOBID, Requester);
        public Result<List<Application_Status>> GetApplicationName_Total() => _sqlHelper.GetAllApplication_Total();
        public Result<List<LOBSubCategories>> GetLOBSubCategories(string ApplicationName, int LOBID) => _sqlHelper.GetLOBSubCategories(ApplicationName, LOBID);
    }
}
