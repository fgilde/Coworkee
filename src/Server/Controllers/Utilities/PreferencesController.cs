using System.Threading.Tasks;
using Coworkee.Server.Managers.Preferences;
using Coworkee.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coworkee.Server.Controllers.Utilities
{
    [ApiController]
    public class PreferencesController : BaseApiController<PreferencesController>
    {
        private readonly ServerPreferenceManager _serverPreferenceManager;

        public PreferencesController(ServerPreferenceManager serverPreferenceManager)
        {
            _serverPreferenceManager = serverPreferenceManager;
        }

        /// <summary>
        /// Change Language Preference
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Preferences.ChangeLanguage)]
        [HttpPost("changeLanguage")]
        public async Task<IActionResult> ChangeLanguageAsync(string languageCode)
        {
            var result = await _serverPreferenceManager.ChangeLanguageAsync(languageCode);
            return Ok(result);
        }

        //TODO - add actions
    }
}