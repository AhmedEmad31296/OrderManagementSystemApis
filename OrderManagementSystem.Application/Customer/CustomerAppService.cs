using Microsoft.EntityFrameworkCore;

using OrderManagementSystem.Application.Customer.Dto;
using OrderManagementSystem.Domain.Exceptions;
using OrderManagementSystem.Domain.Helpers;
using OrderManagementSystem.Infrastructure.Data.Repositories;

using System.Linq.Dynamic.Core;

namespace OrderManagementSystem.Application.Customer
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly IRepository<Domain.Entities.Customer> _CustomerRepository;
        public CustomerAppService(IRepository<Domain.Entities.Customer> customerRepository)
        {
            _CustomerRepository = customerRepository;
        }
        public async Task<DataTableFilteredDto<CustomerPagedDto>> GetPaged(FilterCustomerPagedInput input)
        {
            IQueryable<Domain.Entities.Customer> query = _CustomerRepository.GetAll().AsNoTracking();
            ;
            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(input.SearchTerm))
            {
                query = query.Where(b => b.FullName.ToLower().Contains(input.SearchTerm.ToLower()));
            }

            int recordsFiltered = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(input.SortColumn) && !string.IsNullOrEmpty(input.SortDirection))
            {
                query = query.OrderBy(string.Concat(input.SortColumn, " ", input.SortDirection));
            }

            // Pagination
            List<CustomerPagedDto> customers = await query
                .Select(b => new CustomerPagedDto
                {
                    CustomerId = b.CustomerId,
                    PhoneNumber = b.PhoneNumber,
                    DateOfBirth = b.DateOfBirth,
                    FullName = b.FullName,
                    Email = b.Email
                })
                .Skip((input.Page - 1) * input.PageSize)
                .Take(input.PageSize)
                .ToListAsync();

            return new DataTableFilteredDto<CustomerPagedDto>
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = totalCount,
                AaData = customers,
                Draw = input.Draw
            };

        }
        public async Task<FullInfoCustomerDto> GetAsync(int id)
        {
            var customer = await _CustomerRepository.GetAll()
                                                    .AsNoTracking()
                                                    .Where(x => x.CustomerId == id)
                                                    .FirstOrDefaultAsync();
            if (customer == null) throw new KeyNotFoundException("Customer Not Found");

            return new FullInfoCustomerDto
            {
                CustomerId = customer.CustomerId,
                DateOfBirth = customer.DateOfBirth,
                Email = customer.Email,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber
            };
        }
        public async Task<string> CreateAsync(CreateCustomerInput input)
        {
            // Check if email already exists
            bool existingEmail = await _CustomerRepository.GetAll()
                .Where(b => b.Email.Equals(input.Email))
                .AnyAsync();

            if (existingEmail)
                throw new EmailAlreadyExistsException();

            // Check if mobile number already exists
            bool existingPhone = await _CustomerRepository.GetAll()
                .Where(b => b.PhoneNumber.Equals(input.PhoneNumber))
                .AnyAsync();

            if (existingPhone)
                throw new MobileNumberAlreadyExistsException();


            var Customer = new Domain.Entities.Customer
            {
                Email = input.Email,
                PhoneNumber = input.PhoneNumber,
                FullName = input.FullName,
                DateOfBirth = input.DateOfBirth,
            };

            await _CustomerRepository.InsertAsync(Customer);

            return "Customer Created Successfully";
        }
        public async Task<int> CheckForNewOrder(CreateCustomerInput input)
        {
            var existingCustomer = await _CustomerRepository.GetAll()
                .Where(b => b.Email.Equals(input.Email) || b.PhoneNumber.Equals(input.PhoneNumber))
                .FirstOrDefaultAsync();

            if (existingCustomer == null)
            {
                var Customer = new Domain.Entities.Customer
                {
                    Email = input.Email,
                    PhoneNumber = input.PhoneNumber,
                    FullName = input.FullName,
                    DateOfBirth = input.DateOfBirth,
                };
                await _CustomerRepository.InsertAsync(Customer);

                var newCustomer = await _CustomerRepository.GetAll()
                .Where(b => b.Email.Equals(input.Email) || b.PhoneNumber.Equals(input.PhoneNumber))
                .FirstOrDefaultAsync();
                return newCustomer.CustomerId;
            }

            return existingCustomer.CustomerId;
        }
        public async Task<string> UpdateAsync(UpdateCustomerInput input)
        {
            // Retrieve the existing Customer
            var customer = await _CustomerRepository.GetAll()
                                                .Where(x => x.CustomerId == input.CustomerId)
                                                .FirstOrDefaultAsync();
            if (customer == null)
                throw new KeyNotFoundException("Customer Not Found");

            // Check for existing email 
            var emailExists = await _CustomerRepository.GetAll()
                .AnyAsync(b => b.CustomerId != input.CustomerId && b.Email == input.Email);

            if (emailExists)
                throw new EmailAlreadyExistsException();

            // Check for existing mobile number
            var mobileExists = await _CustomerRepository.GetAll()
                .AnyAsync(b => b.CustomerId != input.CustomerId && b.PhoneNumber == input.PhoneNumber);

            if (mobileExists)
                throw new MobileNumberAlreadyExistsException();


            // Update Customer details
            customer.Email = input.Email;
            customer.FullName = input.FullName;
            customer.PhoneNumber = input.PhoneNumber;
            customer.DateOfBirth = input.DateOfBirth;

            // Update Customer in the repository
            await _CustomerRepository.UpdateAsync(customer);

            return "Customer Updated Successfully";
        }
        public async Task<string> DeleteAsync(int id)
        {
            // Retrieve the client from the repository
            var customer = await _CustomerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new KeyNotFoundException("Customer not found");

            // Delete the client from the repository
            await _CustomerRepository.DeleteAsync(id);

            return "Customer Deleted successfully";

        }

    }
}
