using CloudService.Entities;
using CloudService.Impl.Services.Folders;
using CloudService.Impl.Services.FrameService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudService.Web.Controllers
{
    [Authorize]
    [Route("folder")]
    public class FoldersController : ApiController
    {
        private readonly IFoldersService foldersService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFrameService frameService;

        public FoldersController(
            IFoldersService foldersService,
            UserManager<ApplicationUser> userManager,
            IFrameService frameService
        )
        {
            this.foldersService = foldersService;
            this.userManager = userManager;
            this.frameService = frameService;
        }

        [HttpPost]
        [Route("{parentId}")]
        public async Task<IActionResult> Create(string parentId, string name)
        {
            if (await IsCorrectAction(parentId))
            {
                await foldersService.Create(name, parentId);

                return Ok(await frameService.GetFrame(parentId, HttpContext.User.Identity.Name));
            }

            return Unauthorized();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await IsCorrectAction(id))
            {
                var parentId = (await foldersService.Get(id)).FolderId;

                await foldersService.Delete(id);

                return Ok(await frameService.GetFrame(parentId, HttpContext.User.Identity.Name));
            }

            return Unauthorized();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(string id, string name)
        {
            if (await IsCorrectAction(id))
            {
                var parentId = (await foldersService.Get(id)).FolderId;

                await foldersService.Update(id, name);

                return Ok(await frameService.GetFrame(parentId, HttpContext.User.Identity.Name));
            }

            return Unauthorized();
        }

        private async Task<bool> IsCorrectAction(string id)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var folder = await foldersService.Get(id);

            return user.Id == folder.Storage.UserId ? true : false;
        }
    }
}
