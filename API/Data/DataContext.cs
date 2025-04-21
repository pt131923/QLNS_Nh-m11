using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<AppDepartment> Department { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình cho AppDepartment với khóa chính
            builder.Entity<AppDepartment>()
                .HasKey(d => d.id) // Đặt id là khóa chính của AppDepartment
                .HasName("DepartmentId");

            builder.Entity<AppDepartment>()
                .Property(d => d.id)
                .ValueGeneratedOnAdd();

            builder.Entity<AppDepartment>()
                .Property(d => d.name)
                .IsRequired(); // Không để trống

            builder.Entity<AppDepartment>()
                .HasMany(d => d.Employee)
                .WithOne(e => e.Department)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa cascade

            // Cấu hình cho Employee với khóa chính
            builder.Entity<Employee>()
                .HasKey(e => e.id) // Đặt id là khóa chính của Employee
                .HasName("EmployeeId"); // Đặt tên cho khóa chính


            builder.Entity<Employee>()
                .Property(e => e.id)
                .ValueGeneratedOnAdd();

            builder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employee)
                .HasForeignKey(e => e.departmentId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa cascade
        }
    }
}
