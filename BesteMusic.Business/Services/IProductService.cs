using BesteMusic.Business.Dtos;
using BesteMusic.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BesteMusic.Business.Services
{
    public interface IProductService
    {
        ServiceMessage AddProduct(AddProductDto addProductDto);

        List<ListProductDto> GetProducts();

        EditProductDto GetProductById(int id);

        void EditProduct(EditProductDto editProductDto);
        void DeleteProduct(int id);

        List<ListProductDto> GetProductsByCategoryId(int? categoryId = null);

        DetailProductDto GetProductDetail(int id);
    }
}
