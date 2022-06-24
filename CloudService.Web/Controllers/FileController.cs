using CloudService.Entities;
using CloudService.Impl.Services.Files;
using CloudService.Impl.Services.Folders;
using CloudService.Impl.Services.FrameService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CloudService.Web.Controllers
{
    [Authorize]
    [Route("file")]
    public class FileController : ApiController
    {
        private readonly IFilesService filesService;
        private readonly IFoldersService foldersService;
        private readonly IFrameService frameService;
        private readonly UserManager<ApplicationUser> userManager;

        public FileController(
            IFilesService filesService,
            IFoldersService foldersService,
            UserManager<ApplicationUser> userManager,
            IFrameService frameService
        )
        {
            this.filesService = filesService;
            this.foldersService = foldersService;
            this.userManager = userManager;
            this.frameService = frameService;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await IsCorrectAction(id, null))
            {
                var parentId = (await filesService.Get(id)).FolderId;

                await filesService.Delete(id);

                return Ok(await frameService.GetFrame(parentId, HttpContext.User.Identity.Name));
            }

            return Unauthorized();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(string id, string name)
        {
            if (await IsCorrectAction(id, null))
            {
                var parentId = (await filesService.Get(id)).FolderId;

                await filesService.Update(id, name);

                return Ok(await frameService.GetFrame(parentId, HttpContext.User.Identity.Name));
            }

            return Unauthorized();
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("upload/{parentId}")]
        public async Task<IActionResult> Upload(string parentId)
        {
            if (await IsCorrectAction(null, parentId))
            {
                try
                {
                    var formCollection = await Request.ReadFormAsync();
                    var file = formCollection.Files.First();

                    if (file.Length > 0)
                    {
                        await filesService.Create(parentId, file);

                        return Ok(await frameService.GetFrame(parentId, HttpContext.User.Identity.Name));
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex}");
                }
            }

            return Unauthorized();
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("download/{id}")]
        public async Task<IActionResult> Download(string id)
        {
            var result = await filesService.GetForDownload(id);

            return File(result.File, result.MimeType, result.Name);
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
