using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Models;
using System.Linq;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly MvcAppDbContext _dbContext;

        public EmployeeRepository(MvcAppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }


        public IQueryable<Employee> GetEmployeesByAddress(string address)
            => _dbContext.Employees.Where(E => E.Address.ToLower().Contains(address.ToLower()));

        public IQueryable<Employee> GetEmployeesByName(string SearchValue)
            => _dbContext.Employees.Where(E => E.Name.ToLower().Contains(SearchValue.ToLower()));
    }
}
