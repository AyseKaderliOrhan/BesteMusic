using BesteMusic.Business.Dtos;
using BesteMusic.Business.Services;
using BesteMusic.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BesteMusic.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;
        private readonly IProductService _productService;
        public ProductController(ICategoryService categoryService, IWebHostEnvironment environment, IProductService productService)
        {
            _categoryService = categoryService;
            _environment = environment;
            _productService = productService;
        }

        public IActionResult List()
        {
            var productDtos = _productService.GetProducts();

            var viewModel = productDtos.Select(x => new ListProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                CategoryId= x.CategoryId,
                CategoryName = x.CategoryName,
                UnitInStock = x.UnitInStock,
                UnitPrice = x.UnitPrice,
                ImagePath = x.ImagePath
            }).ToList();

            return View(viewModel);        
        }

        [HttpGet]
        public IActionResult New()
        {
            ViewBag.Categories = _categoryService.GetCategories();
            return View("Form", new ProductFormViewModel());
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var editProductDto = _productService.GetProductById(id);

            var viewModel = new ProductFormViewModel()
            {
                Id = editProductDto.Id,
                Name = editProductDto.Name,
                Description = editProductDto.Description,
                UnitInStock = editProductDto.UnitInStock,
                UnitPrice = editProductDto.UnitPrice,
                CategoryId = editProductDto.CategoryId
            };

            // eski görseli ekranda görmek istiyorum, O yüzden ViewBag ile gönderip bir çerçeveye yerleştireceğim.
            ViewBag.ImagePath = editProductDto.ImagePath;
            ViewBag.Categories = _categoryService.GetCategories();
            return View("form", viewModel);
        }

        [HttpPost]
        public IActionResult Save(ProductFormViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                if (formData.CategoryId == 0)
                {
                    ViewBag.CatError = "Kategori Seçmek zorunludur.";
                }
                ViewBag.Categories = _categoryService.GetCategories();
                return View("Form", formData);
            }

            var newFileName = "";

            if (formData.File is not null)
            {
                var allowedFileTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/jfif" };

                var allowedFileExtensions = new string[] { ".jpeg", ".jpg", ".png", ".jfif" };

                var fileContentType = formData.File.ContentType; //dosyanın içeriği
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(formData.File.FileName); // uzantısız dosya ismi
                var fileExtension = Path.GetExtension(formData.File.FileName); // dosya uzantısı.

                if (!allowedFileTypes.Contains(fileContentType) ||
                    !allowedFileExtensions.Contains(fileExtension))
                {
                    ViewBag.FileError = "Dosya formatı veya içeriği hatalı";
                    
                    ViewBag.Categories = _categoryService.GetCategories();
                    return View("Form", formData);

                }

                newFileName = fileNameWithoutExtension + "-" + Guid.NewGuid() + fileExtension;

                var folderPath = Path.Combine("images", "products");

                var wwwrootFolderPath = Path.Combine(_environment.WebRootPath, folderPath);

                var wwwrootFilePath = Path.Combine(wwwrootFolderPath, newFileName);

                Directory.CreateDirectory(wwwrootFolderPath);


                using (var fileStream = new FileStream(wwwrootFilePath, FileMode.Create))
                {
                    formData.File.CopyTo(fileStream);
                }
            }

            if (formData.Id == 0) // Ekleme
            {

                var addProductDto = new AddProductDto()
                {
                    Name = formData.Name.Trim(),
                    Description = formData.Description,
                    UnitPrice = formData.UnitPrice,
                    UnitInStock = formData.UnitInStock,
                    CategoryId = formData.CategoryId,
                    ImagePath = newFileName
                };

                var response = _productService.AddProduct(addProductDto);

                if (response.IsSucceed)
                {
                    return RedirectToAction("List");

                }
                else
                {
                    ViewBag.ErrorMessage = response.Message;
                    ViewBag.Categories = _categoryService.GetCategories();
                    return View("Form", formData);

                }

            }
            else // Güncelleme
            {

                var editProductDto = new EditProductDto()
                {
                   Id = formData.Id,
                    Name = formData.Name,
                    Description = formData.Description,
                    UnitInStock = formData.UnitInStock,
                    UnitPrice = formData.UnitPrice,
                    CategoryId = formData.CategoryId
                };

                if (formData.File != null) // is not null
                    editProductDto.ImagePath = newFileName;


                _productService.EditProduct(editProductDto);

                return RedirectToAction("List");

            }           
        }

        public IActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);

            return RedirectToAction("List");
        }
    }
}
