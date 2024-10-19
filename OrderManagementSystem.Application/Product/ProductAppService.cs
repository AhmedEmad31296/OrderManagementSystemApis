using Microsoft.EntityFrameworkCore;

using OrderManagementSystem.Application.Product.Dto;
using OrderManagementSystem.Domain.Exceptions;
using OrderManagementSystem.Domain.Helpers;
using OrderManagementSystem.Infrastructure.Data.Repositories;

using System.Linq.Dynamic.Core;

namespace OrderManagementSystem.Application.Product
{
    public class ProductAppService : IProductAppService
    {
        private readonly IRepository<Domain.Entities.Product> _ProductRepository;
        public ProductAppService(IRepository<Domain.Entities.Product> ProductRepository)
        {
            _ProductRepository = ProductRepository;
        }
        public async Task<DataTableFilteredDto<ProductPagedDto>> GetPaged(FilterProductPagedInput input)
        {
            IQueryable<Domain.Entities.Product> query = _ProductRepository.GetAll().AsNoTracking();
            ;
            int totalCount = await query.CountAsync();

            // Apply filtering products by name or filter by price range.
            query = query.Where(p =>
                                 (string.IsNullOrEmpty(input.SearchTerm) || p.Name.ToLower().Contains(input.SearchTerm.ToLower())) &&
                                 (!input.LowPrice.HasValue || p.Price >= input.LowPrice.Value) &&
                                 (!input.HighPrice.HasValue || p.Price <= input.HighPrice.Value));


            int recordsFiltered = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(input.SortColumn) && !string.IsNullOrEmpty(input.SortDirection))
            {
                query = query.OrderBy(string.Concat(input.SortColumn, " ", input.SortDirection));
            }

            // Pagination
            List<ProductPagedDto> products = await query
                .Select(p => new ProductPagedDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .Skip((input.Page - 1) * input.PageSize)
                .Take(input.PageSize)
                .ToListAsync();

            return new DataTableFilteredDto<ProductPagedDto>
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = totalCount,
                AaData = products,
                Draw = input.Draw
            };

        }
        public async Task<FullInfoProductDto> GetAsync(int id)
        {
            var product = await _ProductRepository.GetAll()
                                                    .AsNoTracking()
                                                    .Where(x => x.ProductId == id)
                                                    .FirstOrDefaultAsync();
            if (product == null) throw new KeyNotFoundException("Product Not Found");

            return new FullInfoProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }
        public async Task<string> CreateAsync(CreateProductInput input)
        {
            // Check if product already exists
            bool existingProduct = await _ProductRepository.GetAll()
                .Where(b => b.Name.Equals(input.Name))
                .AnyAsync();

            if (existingProduct)
                throw new ProductAlreadyExistsException();

            var product = new Domain.Entities.Product
            {
                Name = input.Name,
                Price = input.Price,
                StockQuantity = input.StockQuantity

            };

            await _ProductRepository.InsertAsync(product);

            return "Product Created Successfully";
        }
        public async Task<string> UpdateAsync(UpdateProductInput input)
        {
            // Retrieve the existing Product
            var product = await _ProductRepository.GetAll()
                                                .Where(x => x.ProductId == input.ProductId)
                                                .FirstOrDefaultAsync();
            if (product == null)
                throw new KeyNotFoundException("Product Not Found");

            // Update Product details
            product.Name = input.Name;
            product.Price = input.Price;
            product.StockQuantity = input.StockQuantity;

            // Update Product in the repository
            await _ProductRepository.UpdateAsync(product);

            return "Product Updated Successfully";
        }
        public async Task<string> DeleteAsync(int id)
        {
            // Retrieve the client from the repository
            var product = await _ProductRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            // Delete the client from the repository
            await _ProductRepository.DeleteAsync(id);

            return "Product Deleted successfully";

        }

    }
}
