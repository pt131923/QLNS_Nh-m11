using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController(DataContext context, IEmployeeRepository employeeRepository, AutoMapper.IMapper mapper) : BaseApiController
    {
        private readonly DataContext _context = context;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly AutoMapper.IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _employeeRepository.GetEmployeeAsync();
            return Ok(employees);
        }

        [HttpGet("{id}", Name = "GetEmployeeById")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmployee(EmployeeUpdateDto employeeUpdateDto, int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            _mapper.Map(employeeUpdateDto, employee);

            _employeeRepository.Update(employee);

            if(await _employeeRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update employee");
        }

        [HttpPost("add-employee")]
public async Task<ActionResult<EmployeeDto>> AddEmployee(EmployeeDto employeeDto)
{
    if (await EmployeeExists(employeeDto.employeeName)) 
        return BadRequest("Employee Name is taken");

    var departmentExists = await _context.Department.AnyAsync(d => d.id == employeeDto.departmentId);
    if (!departmentExists)
        return BadRequest("Department does not exist");

    var employee = new Employee
    {
        employeeName = employeeDto.employeeName.ToLower(),
        departmentId = employeeDto.departmentId
    };

    _context.Employee.Add(employee);
    await _context.SaveChangesAsync();

    return new EmployeeDto
    {
        id = employee.id,
        employeeName = employee.employeeName,
        departmentId = employee.departmentId
    };
}

private async Task<bool> EmployeeExists(string employeeName)
{
    return await _context.Employee.AnyAsync(x => x.employeeName == employeeName.ToLower());
}

        [HttpDelete("delete-employee/{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            var employees = await _employeeRepository.GetEmployeeByIdAsync(id);

            if(employees == null) return NotFound();

            _employeeRepository.Delete(employees);

            if(await _employeeRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the employee");
        }

        [HttpGet("employees-with-departments")]
        public async Task<ActionResult<IEnumerable<EmployeeWithDepartmentDto>>> GetEmployeesWithDepartments()
        {
            var employees = await _context.Employee
                .Include(e => e.Department) // Thêm thông tin phòng ban
                .Select(e => new EmployeeWithDepartmentDto
                {
                    id = e.id,
                    employeeName = e.employeeName,
                    name = e.Department.name // Lấy tên phòng ban
                })
                .ToListAsync();

            return Ok(employees);
        }
    }
}