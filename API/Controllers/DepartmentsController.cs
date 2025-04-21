using System.Security.Claims;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace API.Controllers
{
    public class DepartmentsController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly AutoMapper.IMapper _mapper;

        public DepartmentsController(DataContext context, IDepartmentRepository departmentRepository, AutoMapper.IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var departments = await _departmentRepository.GetDepartmentAsync();
            return Ok(departments);
        }

        [HttpGet("{id}", Name = "GetDepartmentById")]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
        {
            var department = await _departmentRepository.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDepartment(int id, [FromBody] DepartmentUpdateDto departmentUpdateDto)
        {
            var department = await _departmentRepository.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            _mapper.Map(departmentUpdateDto, department);

            _departmentRepository.Update(department);

            if (await _departmentRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update Department");
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> AddDepartment([FromBody] DepartmentDto departmentDto)
        {
            if (await DepartmentExists(departmentDto.name)) return BadRequest("Department Name is taken");

            var department = _mapper.Map<AppDepartment>(departmentDto);

            _context.Department.Add(department);
            await _context.SaveChangesAsync();

            // Return the newly created department with its generated ID
            return CreatedAtRoute("GetDepartmentById", new { id = department.id }, _mapper.Map<DepartmentDto>(department));
        }

        private async Task<bool> DepartmentExists(string name)
        {
            return await _context.Department.AnyAsync(x => x.name.ToLower() == name.ToLower());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            var department = await _departmentRepository.GetDepartmentByIdAsync(id);

            if (department == null) return NotFound();

            _departmentRepository.Delete(department);

            if (await _departmentRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the Department");
        }
    }
}



