using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using PBL3.Models;

namespace PBL3.Data
{
    public static class SeedData
    {
        public static async Task Initialize(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                // Tạo các role
                string[] roleNames = { "Admin", "Staff", "Student" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Tạo tài khoản Admin mặc định
                var adminEmail = "admin@school.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        HoTen = "Quản trị viên",
                        EmailConfirmed = true,
                        PhoneNumber = "0123456789"
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // Tạo tài khoản Staff mặc định
                var staffEmail = "staff@school.com";
                var staffUser = await userManager.FindByEmailAsync(staffEmail);
                if (staffUser == null)
                {
                    staffUser = new Staff
                    {
                        UserName = staffEmail,
                        Email = staffEmail,
                        HoTen = "Nhân viên",
                        EmailConfirmed = true,
                        PhoneNumber = "0987654321",
                        DiaChi = "123 Đường ABC"
                    };
                    var result = await userManager.CreateAsync(staffUser, "Staff@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(staffUser, "Staff");
                    }
                }

                // Tạo tài khoản Student mặc định
                var studentEmail = "student@school.com";
                var studentUser = await userManager.FindByEmailAsync(studentEmail);
                if (studentUser == null)
                {
                    studentUser = new Student
                    {
                        UserName = studentEmail,
                        Email = studentEmail,
                        HoTen = "Sinh viên",
                        EmailConfirmed = true,
                        PhoneNumber = "0123456789",
                        MSSV = "SV001",
                        Lop = "20TCLC_DT1"
                    };
                    var result = await userManager.CreateAsync(studentUser, "Student@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(studentUser, "Student");
                    }
                }
            }
        }

    }
}