using Microsoft.AspNetCore.Mvc;

using OrderManagementSystem.Application.Customer;
using OrderManagementSystem.Application.Customer.Dto;
using OrderManagementSystem.Domain.Common;
using OrderManagementSystem.Domain.Exceptions;
using OrderManagementSystem.Domain.Helpers;

namespace OrderManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerAppService _customerAppService;

        public CustomerController(ICustomerAppService customerAppService, ILogger<CustomerController> logger)
        {
            _customerAppService = customerAppService;
            _logger = logger;
        }

        [HttpGet("GetPaged")]
        public async Task<ActionResult<ApiResponse<DataTableFilteredDto<CustomerPagedDto>>>> GetPaged([FromQuery] FilterCustomerPagedInput input)
        {
            try
            {
                var result = await _customerAppService.GetPaged(input);
                return Ok(new ApiResponse<DataTableFilteredDto<CustomerPagedDto>>(200, "Paged customers retrieved successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged customers");
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }

        [HttpGet("Get")]
        public async Task<ActionResult<ApiResponse<FullInfoCustomerDto>>> GetAsync(int id)
        {
            try
            {
                var result = await _customerAppService.GetAsync(id);
                if (result == null) return NotFound(new ApiResponse<string>(400, "Customer not found"));

                return Ok(new ApiResponse<FullInfoCustomerDto>(200, "Customer retrieved successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting customer with ID {id}");
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<string>>> CreateAsync([FromBody] CreateCustomerInput input)
        {
            try
            {
                var result = await _customerAppService.CreateAsync(input);
                return Ok(new ApiResponse<string>(200, "Customer created successfully", result));
            }
            catch (EmailAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Email already exists");
                return BadRequest(new ApiResponse<string>(400, "Email already exists"));
            }
            catch (MobileNumberAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Mobile number already exists");
                return BadRequest(new ApiResponse<string>(400, "Mobile number already exists"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating customer");
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }

        [HttpPost("Update")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateAsync([FromBody] UpdateCustomerInput input)
        {
            try
            {
                var result = await _customerAppService.UpdateAsync(input);
                return Ok(new ApiResponse<string>(200, "Customer updated successfully", result));
            }
            catch (EmailAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Email already exists");
                return BadRequest(new ApiResponse<string>(400, "Email already exists"));
            }
            catch (MobileNumberAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Mobile number already exists");
                return BadRequest(new ApiResponse<string>(400, "Mobile number already exists"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating customer");
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }

        [HttpPost("Delete/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
        {
            try
            {
                var result = await _customerAppService.DeleteAsync(id);
                return Ok(new ApiResponse<string>(200, "Customer deleted successfully", result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"Customer with ID {id} not found");
                return NotFound(new ApiResponse<string>(400, "Customer not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting customer with ID {id}");
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }
    }
}
