using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.Product.Dto;
using OrderManagementSystem.Application.Product;
using OrderManagementSystem.Domain.Common;
using OrderManagementSystem.Domain.Exceptions;
using OrderManagementSystem.Domain.Helpers;

namespace OrderManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductAppService _productAppService;

        public ProductController(IProductAppService productAppService, ILogger<ProductController> logger)
        {
            _productAppService = productAppService;
            _logger = logger;
        }

        [HttpGet("GetPaged")]
        public async Task<ActionResult<ApiResponse<DataTableFilteredDto<ProductPagedDto>>>> GetPaged([FromQuery] FilterProductPagedInput input)
        {
            try
            {
                var result = await _productAppService.GetPaged(input);
                return Ok(new ApiResponse<DataTableFilteredDto<ProductPagedDto>>(200, "Paged products retrieved successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting paged products.");
                return StatusCode(500, new ApiResponse<object>(500, "Internal server error"));
            }
        }

        [HttpGet("Get")]
        public async Task<ActionResult<ApiResponse<FullInfoProductDto>>> GetAsync(int id)
        {
            try
            {
                var result = await _productAppService.GetAsync(id);
                if (result == null)
                    return NotFound(new ApiResponse<object>(404, "Product not found"));

                return Ok(new ApiResponse<FullInfoProductDto>(200, "Product retrieved successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting product with ID {id}.");
                return StatusCode(500, new ApiResponse<object>(500, "Internal server error"));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<string>>> CreateAsync([FromBody] CreateProductInput input)
        {
            try
            {
                var result = await _productAppService.CreateAsync(input);
                return Ok(new ApiResponse<string>(200, "Product created successfully", result));
            }
            catch (ProductAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Product already exists.");
                return BadRequest(new ApiResponse<object>(400, "Product already exists"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product.");
                return StatusCode(500, new ApiResponse<object>(500, "Internal server error"));
            }
        }

        [HttpPost("Update")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateAsync([FromBody] UpdateProductInput input)
        {
            try
            {
                var result = await _productAppService.UpdateAsync(input);
                return Ok(new ApiResponse<string>(200, "Product updated successfully", result));
            }
            catch (ProductAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Product already exists.");
                return BadRequest(new ApiResponse<object>(400, "Product already exists"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product.");
                return StatusCode(500, new ApiResponse<object>(500, "Internal server error"));
            }
        }

        [HttpPost("Delete/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
        {
            try
            {
                var result = await _productAppService.DeleteAsync(id);
                return Ok(new ApiResponse<string>(200, "Product deleted successfully", result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"Product with ID {id} not found.");
                return NotFound(new ApiResponse<object>(404, "Product not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting product with ID {id}.");
                return StatusCode(500, new ApiResponse<object>(500, "Internal server error"));
            }
        }
    }
}