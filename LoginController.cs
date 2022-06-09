using ApplicationDashboardServiceCommon.Helper;
using ApplicationDashboardServiceCommon.Interface;
using ApplicationDashboardServiceCommon.Model;
using ApplicationDashboardServiceRepository.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StudentServices;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudnetHost.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors("MyPolicy")]
    public class LoginController : Controller
    {

        private readonly UserServices _userServices;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        ILogger<LoginController> _logger;
        public LoginController(IUserRepository UserRepository, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, ILogger<LoginController> logger, IConfiguration configuration)
        {
            _userServices = new UserServices(UserRepository, configuration);
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            this._logger = logger;
        }

        // POST api/auth/login
        [HttpPost("Tokens")]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
            if (identity == null)
            {
                return StatusCode(400,"Invalid username or password.");
            }
            var jwt = await Helper.Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

            return new OkObjectResult(jwt);
        }


        [HttpPost("Create")]
        [AllowAnonymous]
        public IActionResult Insert([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            dynamic response =_userServices.Insert(user);
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            { return StatusCode(200, response.Value); }


        }

        [HttpPut]
        [AllowAnonymous]
        public IActionResult Update([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            dynamic response = _userServices.Update(user);
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            { return StatusCode(200, response.Value); }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll ()
        {
            _logger.LogWarning("GetAll");
               dynamic response = _userServices.GetAll();
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            {
                _logger.LogInformation("success");
                return StatusCode(200, response.Value); }


        }



        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogWarning("GetAll");
            dynamic response = _userServices.GetById(id);
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            {
                _logger.LogInformation("success");
                return StatusCode(200, response.Value);
            }


        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogWarning("GetAll");
            dynamic response = _userServices.Delete(id);
            if (!response.IsValid)
            {
                return StatusCode(400, response.ErrorMessage);
            }
            else
            {
                _logger.LogInformation("success");
                return StatusCode(200, response.Value);
            }


        }




        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            List<User> userToVerify = _userServices.GetByName(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);
            var students = (from c in userToVerify where c.Password == CryptographyHelper.Encrypt(password) select c).ToList();
            // check the credentials
            if (students.Count > 0)
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(students));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
        
    }
}
