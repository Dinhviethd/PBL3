using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using PBL3.Models;
using PBL3.Data;

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
                var context = services.GetRequiredService<AppDBContext>();

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
                //var staffEmail = "staff@school.com";
                //var staffUser = await userManager.FindByEmailAsync(staffEmail);
                //if (staffUser == null)
                //{
                //    staffUser = new Staff
                //    {
                //        UserName = staffEmail,
                //        Email = staffEmail,
                //        HoTen = "Nhân viên",
                //        EmailConfirmed = true,
                //        PhoneNumber = "0987654321",
                //        DiaChi = "123 Đường ABC"
                //    };
                //    var result = await userManager.CreateAsync(staffUser, "Staff@123");
                //    if (result.Succeeded)
                //    {
                //        await userManager.AddToRoleAsync(staffUser, "Staff");
                //    }
                //}

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

                // Tạo 5 sinh viên và vé tương ứng
                for (int i = 1; i <= 5; i++)
                {
                    var newStudentEmail = $"student{i}@school.com";
                    var newStudentUser = await userManager.FindByEmailAsync(newStudentEmail);
                    if (newStudentUser == null)
                    {
                        newStudentUser = new Student
                        {
                            UserName = newStudentEmail,
                            Email = newStudentEmail,
                            HoTen = $"Sinh viên {i}",
                            EmailConfirmed = true,
                            PhoneNumber = $"012345678{i}",
                            MSSV = $"SV00{i}",
                            Lop = $"20TCLC_DT{i}"
                        };
                        var result = await userManager.CreateAsync(newStudentUser, $"Student{i}@123");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(newStudentUser, "Student");

                            // Tạo vé cho sinh viên này
                            var ticket = new Ticket
                            {
                                BienSoXe = $"29A-123.{i}",
                                NgayDangKy = DateTime.Now.AddDays(-i),
                                NgayHetHan = DateTime.Now.AddMonths(6).AddDays(-i),
                                Price = 100000 * i,
                                StudentId = newStudentUser.Id,
                                ParkingSlotId = null
                            };
                            context.Tickets.Add(ticket);
                        }
                    }
                }

                // Tạo 10 ParkingSlot
                if (!context.ParkingSlots.Any())
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        var parkingSlot = new ParkingSlot
                        {
                            SlotName = $"A{i:00}",
                            CurrentCount = 0,
                            MaxCapacity = 10
                        };
                        context.ParkingSlots.Add(parkingSlot);
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}