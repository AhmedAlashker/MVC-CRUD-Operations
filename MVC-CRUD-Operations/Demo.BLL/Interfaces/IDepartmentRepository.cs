using Demo.DAL.Models;
using System.Collections.Generic;

namespace Demo.BLL.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        //IEnumerable<Department> GetAll(); // This Is Signature
        //Department GetById(int Id);
        //int Add(Department department); /// int  =>  is the count of rows Affected in DataBase 
        //int Update(Department department);
        //int Delete(Department department);



    }
}
