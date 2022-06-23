using CloudService.Entities;
using CloudService.Impl.Services.FrameService;
using CloudService.Impl.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudService.Web.Controllers
{
    [Authorize]
    [Route("storage")]
    public class StorageController : ApiController
    {
        private readonly IStorageService storageService;
        private readonly IFrameService frameService;
        private readonly UserManager<ApplicationUser> userManager;

        public StorageController(
            IStorageService storageService,
            UserManager<ApplicationUser> userManager,
            IFrameService frameService
        )
        {
            this.storageService = storageService;
            this.userManager = userManager;
            this.frameService = frameService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(string name)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            await storageService.Create(name, user.Id);

            return Ok(await frameService.GetFrame(null, user.UserName));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(string name)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            await storageService.Update(name, user.Id);

            return Ok(await frameService.GetFrame(null, user.UserName));
        }

        [HttpGet]
        [Route("{folderId}")]
        public async Task<IActionResult> Get(string folderId)
        {
            return Ok(await frameService.GetFrame(folderId, HttpContext.User.Identity.Name));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetRoot()
        {
            return Ok(await frameService.GetFrame(null, HttpContext.User.Identity.Name));
        }

        [HttpGet]
        [Route("links")]
        public async Task<IActionResult> GetLinks()
        {
            return Ok(await frameService.GetLinksFrame(HttpContext.User.Identity.Name));
        }
    }
}
