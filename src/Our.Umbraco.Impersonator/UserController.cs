using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Security;
using Umbraco.Cms.Web.Common.Attributes;

namespace Our.Umbraco.Impersonator
{
    [PluginController("Impersonator")]
    public class UserController : UmbracoAuthorizedApiController
    {
        private const string IMPERSONATOR_USER_ID = "Impersonator.User.Id";

        private readonly IUserService _userService;
        private readonly IUmbracoMapper _umbracoMapper;
        private readonly IBackOfficeSecurityAccessor _backofficeSecurityAccessor;
        private readonly IBackOfficeSignInManager _signInManager;

        public UserController(
            IUserService userService,
            IUmbracoMapper umbracoMapper,
            IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
            IBackOfficeSignInManager signInManager
            )
        {
            _userService = userService;
            _umbracoMapper = umbracoMapper;
            _backofficeSecurityAccessor = backOfficeSecurityAccessor;
            _signInManager = signInManager;
        }


        private ImpersonatedUserId GetImpersonatingUserId()
        {
            var impersonatedUserIdString = HttpContext.Session.GetString(IMPERSONATOR_USER_ID);
            if (impersonatedUserIdString == null) return null;

            var impersonatedUserId = JsonSerializer.Deserialize<ImpersonatedUserId>(impersonatedUserIdString);

            if (impersonatedUserId == null) return null;

            if (impersonatedUserId.SessionId != HttpContext.Session.Id)
            {
                HttpContext.Session.Remove(IMPERSONATOR_USER_ID);
                return null;
            }

            return impersonatedUserId;
        }

        [HttpGet]
        public string GetImpersonatingUserHash()
        {
            ImpersonatedUserId impersonatingUserId = GetImpersonatingUserId();
            if (impersonatingUserId == null)
            {
                return null;
            }
            var userById = _userService.GetUserById(impersonatingUserId.UserId);
            if (userById != null)
            {
                return _umbracoMapper.Map<UserBasic>(userById)?.EmailHash;
            }
            return null;
        }

        [HttpPost]
        public async Task<string> EndImpersonation()
        {
            if (_backofficeSecurityAccessor.BackOfficeSecurity?.CurrentUser == null)
            {
                return "notSignedIn";
            }
            ImpersonatedUserId impersonatingUserId = GetImpersonatingUserId();
            if (impersonatingUserId == null)
            {
                return "success";
            }
            var userById = _userService.GetUserById(impersonatingUserId.ImpersonatingUserId);
            if (userById != null)
            {
                var user = _umbracoMapper.Map<BackOfficeIdentityUser>(userById);
                HttpContext.Session.Remove(IMPERSONATOR_USER_ID);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, false);

                return "success";
            }
            return "userNotFound";
        }

        [HttpPost]
        public string Impersonate(int id)
        {
            var currentUser = _backofficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser == null)
            {
                return "notSignedIn";
            }
            if (currentUser.AllowedSections.Contains(Constants.Applications.Users))
            {
                if (id > 0)
                {
                    var userById = _userService.GetUserById(id);
                    if (userById != null)
                    {
                        var user = _umbracoMapper.Map<BackOfficeIdentityUser>(userById);
                        HttpContext.Session.Remove(IMPERSONATOR_USER_ID);
                        _signInManager.SignOutAsync();
                        _signInManager.SignInAsync(user, false);
                        HttpContext.Session.SetString(IMPERSONATOR_USER_ID, JsonSerializer.Serialize(new ImpersonatedUserId(id, currentUser.Id, HttpContext.Session.Id)));
                        return "success";
                    }
                    return "userNotFound";
                }
                return "invalidUserId";
            }
            return "notAdministrator";
        }
    }
}
