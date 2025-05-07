using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PBL3.Models;

namespace PBL3.Data
{
    public class SeedData
    {
        public static async Task Initialize(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDBContext>();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Tạo các role nếu chưa tồn tại
                    string[] roleNames = { "Admin", "Staff", "Student" };
                    foreach (var roleName in roleNames)
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }

                    // Tạo tài khoản Admin
                    var adminEmail = "admin@school.com";
                    var adminUser = await userManager.FindByEmailAsync(adminEmail);
                    if (adminUser == null)
                    {
                        adminUser = new AppUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            HoTen = "Quản trị viên",
                            Role = "Admin",
                            EmailConfirmed = true,
                            PhoneNumber = "0123456789" 
                        };
                        await userManager.CreateAsync(adminUser, "Admin@123");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }

                    // Tạo tài khoản Staff
                    var staffEmail = "staff@school.com";
                    var staffUser = await userManager.FindByEmailAsync(staffEmail);
                    if (staffUser == null)
                    {
                        staffUser = new AppUser
                        {
                            UserName = staffEmail,
                            Email = staffEmail,
                            HoTen = "Nhân viên",
                            Role = "Staff",
                            EmailConfirmed = true,
                            PhoneNumber = "0987654321"
                        };
                        await userManager.CreateAsync(staffUser, "Staff@123");
                        await userManager.AddToRoleAsync(staffUser, "Staff");


                        var staff = new Staff
                        {
                            Id = Guid.NewGuid().ToString(), // Tạo ID ngẫu nhiên cho Staff
                            UserId = staffUser.Id, // Lưu ID của người dùng vào Staff
                            DiaChi = "123 Đường ABC",
                        };
                        context.Staffs.Add(staff);
                        await context.SaveChangesAsync();
                    }

                    // Tạo tài khoản Student
                    var studentEmail = "student@school.com";
                    var studentUser = await userManager.FindByEmailAsync(studentEmail);
                    if (studentUser == null)
                    {
                        studentUser = new AppUser
                        {
                            UserName = studentEmail,
                            Email = studentEmail,
                            HoTen = "Sinh viên",
                            Role = "Student",
                            EmailConfirmed = true,
                            PhoneNumber = "0123456789"
                        };
                        await userManager.CreateAsync(studentUser, "Student@123");
                        await userManager.AddToRoleAsync(studentUser, "Student");

                        var student = new Student
                        {
                            UserId = studentUser.Id, // Lưu ID của người dùng vào Student
                            MSSV = "SV001",
                            Lop = "20TCLC_DT1",
                            DKyVe = false,
                        };
                        context.Students.Add(student);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<SeedData>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
        }
    }
}