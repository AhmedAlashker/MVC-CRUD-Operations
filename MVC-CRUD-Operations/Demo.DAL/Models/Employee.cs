using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Models
{
    //public enum Gender
    //{
    //    [EnumMember(Value ="Male")]
    //    Male = 1,
    //    [EnumMember(Value = "Female")]
    //    Female = 2
    //}

    //public enum EmpType
    //{
    //    FullTime = 1,
    //    PartTime =2
    //}

    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Address { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }

        #region new
        //public Gender Gender { get; set; }
        //public EmpType EmployeeType { get; set; }
        //public bool IsDeleted { get; set; }

        #endregion

        public DateTime Creationdate { get; set; } = DateTime.Now;
        public string ImageName { get; set; }
        // FK Optional => OnDelete : Restrict ==> will delete Department and NOT emloyees in this Department
        // FK Required => OnDelete : Cascade ==> will Delete Department and All emloyees in this Department
        [ForeignKey("Department")]
        public int? DepartmentId { get; set; } /* int? => Optional */

        [InverseProperty("Employees")]
        public Department Department { get; set; }




    }
}
