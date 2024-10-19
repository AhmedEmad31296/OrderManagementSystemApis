using OrderManagementSystem.Domain.Helpers;

namespace OrderManagementSystem.Application.Product.Dto
{
    public class FilterProductPagedInput : DataTableFilterInput
    {
        public decimal? LowPrice { get; set; }
        public decimal? HighPrice { get; set; }
    }
    public class ProductPagedDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
    public class FullInfoProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
    public class CreateProductInput
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
    public class UpdateProductInput
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
