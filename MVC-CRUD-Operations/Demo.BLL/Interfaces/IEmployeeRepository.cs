using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces
{
    public interface IEmployeeRepository  :IGenericRepository<Employee>
    {
        //IEnumerable<Employee> GetAll(); // This Is Signature
        //Employee GetById(int Id);
        //int Add(Employee employee); /// int  =>  is the count of rows Affected in DataBase 
        //int Update(Employee employee);
        //int Delete(Employee employee);

        IQueryable<Employee> GetEmployeesByAddress(string address);
        IQueryable<Employee> GetEmployeesByName(string SearchValue);


    }
}
