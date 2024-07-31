using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        //private readonly IEmployeeRepository _employeeRepository;
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHostEnvironment _env;

        public EmployeeController(
            //IEmployeeRepository employeeRepository, 
            //IDepartmentRepository departmentRepository, 
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHostEnvironment env)
        {
            //_employeeRepository = employeeRepository;
            //_departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }


        public async Task<IActionResult> Index(string SearchValue)
        {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(SearchValue))
                employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
                //ViewData["Message"] = " Hello From View Bag";
                //ViewBag.Message = " Hello From View Bag";
            else 
                employees =  _unitOfWork.EmployeeRepository.GetEmployeesByName(SearchValue);


            var MappedEmployees = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);

            return View(MappedEmployees);
        }

		public async Task<IActionResult> Search(string SearchValue)
		{
			IEnumerable<Employee> employees;
			if (string.IsNullOrEmpty(SearchValue))
				employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
			//ViewData["Message"] = " Hello From View Bag";
			//ViewBag.Message = " Hello From View Bag";
			else
				employees =  _unitOfWork.EmployeeRepository.GetEmployeesByName(SearchValue);


			var MappedEmployees = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);

			return PartialView("EmployeeTablePartialView", MappedEmployees);
		}

		public IActionResult Create()
        {
            //ViewBag.Departments = _departmentRepository.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            if (ModelState.IsValid)
            {
                // Manual Mapping
                ///var MappedEmployee = new Employee()
                ///{
                ///    Name = employeeVM.Name,
                ///    Age = employeeVM.Age,
                ///    Address = employeeVM.Address,
                ///    PhoneNumber = employeeVM.PhoneNumber,
                ///    DepartmentId = employeeVM.DepartmentId
                ///};

                string FileName = DocumentSettings.UploadFile(employeeVM.Image, "Images");

                employeeVM.ImageName = FileName;

                var MappedEmployee= _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                await _unitOfWork.EmployeeRepository.AddAsync(MappedEmployee);

                await _unitOfWork.CompleteAsync(); // For Save Changes

                return RedirectToAction(nameof(Index));
            }
            return View(employeeVM);
        }

        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();

            //ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();

            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id.Value);

            if (employee == null)
                return NotFound();
            var MappedEmployee = _mapper.Map<Employee, EmployeeViewModel>(employee);

            return View(ViewName, MappedEmployee);
            
        } 

        public async Task<IActionResult> Edit(int? id)
        {
            //ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();
            return await Details(id, "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel employeeVM, [FromRoute] int id)
        {
            if (id != employeeVM.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "Images");

                    var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                    //var MappedEmployee = _mapper.Map<Employee>(employeeVM);

                    _unitOfWork.EmployeeRepository.Update(MappedEmployee);
                    await _unitOfWork.CompleteAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(employeeVM);
        }

        public async Task<IActionResult> Delete(int id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(EmployeeViewModel employeeVM, [FromRoute] int id)
        {
            if (id != employeeVM.Id)
                return BadRequest();

            try
            {
                var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                _unitOfWork.EmployeeRepository.Delete(MappedEmployee);

                var result = await _unitOfWork.CompleteAsync();

                if (result > 0 && employeeVM.ImageName is not null)
                {
                    DocumentSettings.DeleteFile(employeeVM.ImageName, "Images");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                //ModelState.AddModelError(string.Empty, ex.Message);
                //return View(employeeVM);

                if (_env.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    ModelState.AddModelError(string.Empty, "An Error has been occured during update");
                return View(employeeVM);

                ///var innerException = ex.InnerException as SqlException;
                ///if (innerException != null)
                ///{
                ///    // Log or handle specific SQL error
                ///    Console.WriteLine(innerException.Message);
                ///}
                ///throw;
            }
        }


    }
}
