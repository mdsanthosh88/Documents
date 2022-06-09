using ApplicationDashboardServiceCommon.Interface;
using ApplicationDashboardServiceCommon.Model;
using ApplicationDashboardServiceCommon.Result;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace StudentServices
{
    public  class UserServices
    {
        private readonly IUserRepository _UserRepository;
        public UserServices(IUserRepository userRepository,IConfiguration configuration)
        {
            _UserRepository = userRepository;
        }

        public List<User> GetByName(string Name)
        {
            return _UserRepository.GetByName(Name);
        }
        public Result<List<User>> GetById(int Id)
        {
            return _UserRepository.GetById(Id);
        }
        public Result<List<User>> GetAll()
        {
            return _UserRepository.GetAll();
        }
       
        public Result<bool> Update(User user)
        {
            return _UserRepository.Update(user);
        }
        public Result<bool> Delete(int Id)
        {
            return _UserRepository.Delete(Id);
        }

        public Result<bool> Insert(User user)
        {
            return _UserRepository.Insert(user);
        }
    }
}
