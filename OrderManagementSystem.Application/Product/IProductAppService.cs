using OrderManagementSystem.Application.Product.Dto;
using OrderManagementSystem.Domain.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Product
{
    public interface IProductAppService
    {
        Task<DataTableFilteredDto<ProductPagedDto>> GetPaged(FilterProductPagedInput input);
        Task<FullInfoProductDto> GetAsync(int id);
        Task<string> CreateAsync(CreateProductInput input);
        Task<string> UpdateAsync(UpdateProductInput input);
        Task<string> DeleteAsync(int id);
    }
}
