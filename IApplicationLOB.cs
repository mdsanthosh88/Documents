using ApplicationDashboardServiceCommon.Model;
using ApplicationDashboardServiceCommon.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationDashboardServiceCommon.Interface
{
  public interface IApplicationLOB
    {
        Result<SchemaTypes>  AllApplicationLOB(int LOBID);
        Result<List<LOBSubCategories>> GetLOBSubCategories(string ApplicationName, int LOBID);
        Result<List<Application_Status>> GetApplicationName_Total();
        Result<List<Application_Status>> GetApplicationName_Dashboard_Filter(int LOBID);
        Result<List<ApplicationModel>> GetApplicationName_Filter(int LOBID, int Requester);

    }
}
