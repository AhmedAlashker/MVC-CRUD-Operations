using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MvcAppDbContext _dbContext;

        public IEmployeeRepository EmployeeRepository { get ; set; }
        public IDepartmentRepository DepartmentRepository { get; set; }
        public UnitOfWork(MvcAppDbContext dbContext) // Ask CLR For Object From DbContext
        {
            EmployeeRepository = new EmployeeRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);
            _dbContext = dbContext;
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
