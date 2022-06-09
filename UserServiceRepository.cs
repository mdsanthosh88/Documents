using ApplicationDashboardServiceCommon.Interface;
using ApplicationDashboardServiceCommon.Model;
using ApplicationDashboardServiceCommon.Result;
using ApplicationDashboardServiceRepository.DB;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ApplicationDashboardServiceRepository
{
    public class UserServiceRepository : IUserRepository
    {
        private readonly SqlHelper _sqlHelper;
       public UserServiceRepository(IConfiguration configuration)
        {
            _sqlHelper = new SqlHelper(configuration);
        }
        public Result<bool> Delete(User user)
        {
            throw new NotImplementedException();
        }

        public Result<List<User>> GetAll()
        {
            return _sqlHelper.GetAll();
        }

        public List<User> GetByName(string Name)
        {
            return _sqlHelper.GetUserName(Name);
        }

        public Result<List<User>> GetById(int Id)
        {
            return _sqlHelper.GetById(Id);
        }

        public Result<bool> Delete(int Id)
        {
            return _sqlHelper.DeleteUsers(Id);
        }
        public Result<bool> Insert(User user)
        {
            return _sqlHelper.UserInsert(user);
        }

        public Result<bool> Update(User user)
        {
            return _sqlHelper.UserUpdate(user);
        }
    }
}
