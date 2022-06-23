﻿using CloudService.Entities;
using CloudService.Impl.Services.Folders;
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

        public FoldersController(
            IFoldersService foldersService,
            UserManager<ApplicationUser> userManager
        )
        {
            this.foldersService = foldersService;
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(string name, string parentId)
        {
            if (await IsCorrectAction(parentId))
            {
                await foldersService.Create(name, parentId);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await IsCorrectAction(id))
            {
                await foldersService.Delete(id);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(string id, string name)
        {
            if (await IsCorrectAction(id))
            {
                await foldersService.Update(id, name);

                return Ok();
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
