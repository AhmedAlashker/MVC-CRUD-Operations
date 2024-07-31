using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo.BLL.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        //private MvcAppDbContext dbContext;
        ///private readonly MvcAppDbContext _dbContext;
        ///
        ///public DepartmentRepository(MvcAppDbContext dbContext) // Ask CLR For Object From DbContext
        ///{
        ///    //dbContext = new MvcAppDbContext();  
        ///    _dbContext = dbContext;
        ///}
        ///public int Add(Department department)
        ///{
        ///    _dbContext.Add(department);
        ///    return _dbContext.SaveChanges();
        ///}
        ///
        ///public int Delete(Department department)
        ///{
        ///    _dbContext.Remove(department);
        ///    return _dbContext.SaveChanges();
        ///}
        ///
        ///public IEnumerable<Department> GetAll()
        ///    => _dbContext.Departments.ToList();
        ///
        ///public Department GetById(int Id)
        ///{
        ///    //var department = _dbContext.Departments.Local.Where(d => d.Id == Id).FirstOrDefault();
        ///    //if (department == null)
        ///    //department = _dbContext.Departments.Where(d => d.Id == Id).FirstOrDefault();
        ///    //return department;
        ///
        ///    return _dbContext.Departments.Find(Id);
        ///}
        ///
        ///public int Update(Department department)
        ///{
        ///    _dbContext.Update(department);
        ///    return _dbContext.SaveChanges();
        ///}
        ///

        //
        public DepartmentRepository(MvcAppDbContext dbContext):base(dbContext)
        {
            
        }

    }
}
