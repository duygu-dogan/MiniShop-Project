using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniShop.Business.Abstract;
using MiniShop.Business.Concrete;
using MiniShop.Shared.ResponseViewModels;
using MiniShop.Shared.ViewModels;
using MiniShop.UI.Helpers;

namespace MiniShop.UI.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryManager;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryManager, IMapper mapper)
        {
            _categoryManager = categoryManager;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(bool id = false)
        {
            Response<List<CategoryViewModel>> categories = await _categoryManager.GetNonDeletedCategories(id);
            ViewBag.ShowDeleted = id;
            return View(categories.Data);
        }

        //Form sayfasını açacak controller
        [HttpGet]
        public IActionResult Create()
        {
            AddCategoryViewModel model = new AddCategoryViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(AddCategoryViewModel model)
        {
            if (ModelState.IsValid) 
            {
                model.Url = Jobs.GetUrl(model.Name);
                await _categoryManager.CreateAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public async Task<IActionResult> UpdateIsActive(int id)
        {
            await _categoryManager.UpdateIsActiveAsync(id);
            var category = await _categoryManager.GetByIdAsync(id);
            return Json(category.Data.IsActive);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        { 
            var category = await _categoryManager.GetByIdAsync(id);
            CategoryViewModel response = category.Data;
            EditCategoryViewModel model = _mapper.Map<EditCategoryViewModel>(response);
            return View(model);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var deletedCategory = await _categoryManager.GetByIdAsync(id);
            var categoryViewModel = deletedCategory.Data;
            DeleteCategoryViewModel model = _mapper.Map<DeleteCategoryViewModel>(categoryViewModel);

            return View(model);
        }
        public async Task<IActionResult> HardDelete(int id)
        {
           await _categoryManager.HardDeleteAsync(id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _categoryManager.SoftDeleteAsync(id);
            var caregoryViewModel = await _categoryManager.GetByIdAsync(id);
            return Redirect($"/Admin/Category/Index/{!caregoryViewModel.Data.IsDeleted}");
        }
    }
}
