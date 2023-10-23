using BesteMusic.Business.Dtos;
using BesteMusic.Business.Services;
using BesteMusic.Business.Types;
using BesteMusic.Data.Entities;
using BesteMusic.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BesteMusic.Business.Managers
{
    public class ProductManager : IProductService
    {
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly ICategoryService _categoryService;
        public ProductManager(IRepository<ProductEntity> productRepository, ICategoryService categoryService)
        {
            _productRepository = productRepository;
            _categoryService = categoryService;
        }

        public ServiceMessage AddProduct(AddProductDto addProductDto)
        {
            var hasProduct = _productRepository.GetAll(x => x.Name.ToLower() == addProductDto.Name.ToLower()).ToList();

            if (hasProduct.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu isimde bir ürün zaten mevcut."
                };
            }

            var productEntity = new ProductEntity()
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                UnitInStock = addProductDto.UnitInStock,
                UnitPrice = addProductDto.UnitPrice,
                CategoryId = addProductDto.CategoryId,
                ImagePath = addProductDto.ImagePath
            };

            _productRepository.Add(productEntity);

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        public void DeleteProduct(int id)
        {
            _productRepository.Delete(id);
        }

        public void EditProduct(EditProductDto editProductDto)
        {
            var productEntity = _productRepository.GetById(editProductDto.Id); 

            productEntity.Name = editProductDto.Name;
            productEntity.Description = editProductDto.Description;
            productEntity.UnitPrice = editProductDto.UnitPrice;
            productEntity.UnitInStock = editProductDto.UnitInStock;
            productEntity.CategoryId = editProductDto.CategoryId;

            if (editProductDto.ImagePath is not null)
            {
                productEntity.ImagePath = editProductDto.ImagePath;
            }
            // Bu If'i yazmasam, editProductDto aracılığı ile View'den gelen null olan imagePath bilgisi, veritabanımdaki görsel bilgisi üzerine yazılır, böylelikle elimde olan görseli silmiş olurum. Bunu istemiyorum !

            _productRepository.Update(productEntity);
        }

        public EditProductDto GetProductById(int id)
        {
            var productEntity = _productRepository.GetById(id);

            var editProductDto = new EditProductDto()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                UnitInStock = productEntity.UnitInStock,
                UnitPrice = productEntity.UnitPrice,
                CategoryId = productEntity.CategoryId,
                ImagePath = productEntity.ImagePath
            };

            return editProductDto;
        }

        public DetailProductDto GetProductDetail(int id)
        {

            var productEntity = _productRepository.GetById(id);

            var productDto = new DetailProductDto()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                UnitPrice = productEntity.UnitPrice,
                ImagePath = productEntity.ImagePath,
                CategoryId = productEntity.CategoryId,
                UnitInStock = productEntity.UnitInStock
            };

            productDto.CategoryName = _categoryService.GetCategoryName(productDto.CategoryId);

            return productDto;
        }

        public List<ListProductDto> GetProducts()
        {
            var productEntities = _productRepository.GetAll().OrderBy(x => x.Category.Name).ThenBy(x => x.Name);

            var productDtoList = productEntities.Select(x => new ListProductDto
            {
                Id = x.Id,
                Name = x.Name,
                UnitInStock = x.UnitInStock,
                UnitPrice = x.UnitPrice,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                ImagePath = x.ImagePath
            }).ToList();

            return productDtoList;
        }

        public List<ListProductDto> GetProductsByCategoryId(int? categoryId = null)
        {
            if (categoryId.HasValue) // is not null -- != null
            {
                var productEntites = _productRepository.GetAll(x => x.CategoryId == categoryId).OrderBy(x => x.Name);
                // gönderdiğim id değeri ile, categoryId verisi eşleşen bütün ürünleri isimlerine göre sıralayarak getir.


                var productDtos = productEntites.Select(x => new ListProductDto
                {
                    Id = x.Id,
                    Name = x.Name,                   
                    UnitInStock = x.UnitInStock,
                    UnitPrice = x.UnitPrice,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    ImagePath = x.ImagePath
                }).ToList();

                return productDtos;

            }
            else
            {
                return GetProducts();
            }
        }
    }
}
