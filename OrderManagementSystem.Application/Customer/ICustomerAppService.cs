using OrderManagementSystem.Application.Customer.Dto;
using OrderManagementSystem.Domain.Helpers;

namespace OrderManagementSystem.Application.Customer
{
    public interface ICustomerAppService
    {
        Task<DataTableFilteredDto<CustomerPagedDto>> GetPaged(FilterCustomerPagedInput input);
        Task<FullInfoCustomerDto> GetAsync(int id);
        Task<string> CreateAsync(CreateCustomerInput input);
        Task<int> CheckForNewOrder(CreateCustomerInput input);
        Task<string> UpdateAsync(UpdateCustomerInput input);
        Task<string> DeleteAsync(int id);
    }
}
