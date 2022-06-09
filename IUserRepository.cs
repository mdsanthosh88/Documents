using ApplicationDashboardServiceCommon.Model;
using ApplicationDashboardServiceCommon.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationDashboardServiceCommon.Interface
{
    public interface IUserRepository
    {

        Result<List<User>>GetAll();
        List<User> GetByName(string Name);
        Result<List<User>> GetById(int Id);
        Result<bool> Insert(User user);
        Result<bool> Update(User user);
        Result<bool> Delete(int Id);

    }
}

