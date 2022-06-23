using CloudService.Entities;
using CloudService.Impl.Services.Files;
using CloudService.Impl.Services.Folders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudService.Web.Controllers
{
    [Authorize]
    [Route("file")]
    public class FileController : ApiController
    {
        private readonly IFilesService filesService;
        private readonly IFoldersService foldersService;
        private readonly UserManager<ApplicationUser> userManager;

        public FileController(
            IFilesService filesService,
            IFoldersService foldersService,
            UserManager<ApplicationUser> userManager
        )
        {
            this.filesService = filesService;
            this.foldersService = foldersService;
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(string name, string parentId)
        {
            if (await IsCorrectAction(null, parentId))
            {
                await filesService.Create(name, parentId);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await IsCorrectAction(id, null))
            {
                await filesService.Delete(id);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(string id, string name)
        {
            if (await IsCorrectAction(id, null))
            {
                await filesService.Update(id, name);

                return Ok();
            }

            return Unauthorized();
        }

        private async Task<bool> IsCorrectAction(string id, string parentId)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (parentId != null)
            {
                var folder = await foldersService.Get(parentId);

                return user.Id == folder.Storage.UserId ? true : false;
            }
            else
            {
                var file = await filesService.Get(id);

                return user.Id == file.Storage.UserId ? true : false;
            }
        }
    }
}
