using CloudService.Entities;
using CloudService.Impl.Services.Storage;
using CloudService.Web.ViewModels.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CloudService.Web.Controllers
{
    [Authorize]
    [Route("storage")]
    public class StorageController : ApiController
    {
        private readonly IStorageService storageService;
        private readonly UserManager<ApplicationUser> userManager;

        public StorageController(
            IStorageService storageService,
            UserManager<ApplicationUser> userManager
        )
        {
            this.storageService = storageService;
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(string name)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var result = await storageService.Create(name, user.Id);

            return Ok(ToViewModel(result));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update(string name)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var result = await storageService.Update(name, user.Id);

            return Ok(ToViewModel(result));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var result = await storageService.Get(user.Id);

            return Ok(ToViewModel(result));
        }

        private StorageViewModel ToViewModel(Storage model)
        {
            var capacity = CalculateCapacity(model.Capacity);
            var usedCapacity = CalculateCapacity(model.UsedCapacity);

            var result = new StorageViewModel
            {
                Id = model.Id,
                Name = model.Name,
                Capacity = capacity.Item1,
                CapacityType = capacity.Item2,
                UsedCapacity = usedCapacity.Item1,
                UsedCapacityType = usedCapacity.Item2
            };

            return result;
        }

        private (double, string) CalculateCapacity(long bytes)
        {
            var counter = 0;
            double value = Convert.ToDouble(bytes);

            while(value >= 1000)
            {
                value /= 1000;
                counter++;
            }

            value = Math.Round(value, 1);

            string type = "";

            switch(counter)
            {
                case 0:
                    type = "Б";
                    break;
                case 1:
                    type = "КБ";
                    break;
                case 2:
                    type = "МБ";
                    break;
            }

            return (value, type);
        }
    }
}
