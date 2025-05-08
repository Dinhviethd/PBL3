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
                            Id = staffUser.Id, 
                            HoTen = "Nhân viên",
                            Email = staffEmail,
                            DiaChi = "123 Đường ABC",
                            PhoneNumber = staffUser.PhoneNumber 
                        };
                        context.Staff.Add(staff);
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
                            Id = studentUser.Id, 
                            HoTen = "Sinh viên",
                            Email = studentEmail,
                            MSSV = "SV001",
                            Lop = "20TCLC_DT1",
                            DKyVe = false,
                            PhoneNumber = studentUser.PhoneNumber 
                        };
                        context.Student.Add(student);
                        await context.SaveChangesAsync();
                    }

                    // Seed VehicleInfo data if not present
                    if (!context.VehicleInfos.Any())
                    {
                        context.VehicleInfos.AddRange(
                            // Ô số 1
                            new VehicleInfo { BienSo = "43A-12345", IdChu = "001", TenChu = "Nguyễn Văn A", HetHan = "01/01/2025", NgayRa = "15/03/2024", OgiuXe = 1 },
                            new VehicleInfo { BienSo = "43A-23456", IdChu = "002", TenChu = "Trần Văn B", HetHan = "02/02/2025", NgayRa = "20/03/2024", OgiuXe = 1 },
                            new VehicleInfo { BienSo = "43A-34567", IdChu = "003", TenChu = "Lê Văn C", HetHan = "03/03/2025", NgayRa = "25/03/2024", OgiuXe = 1 },
                            // Ô số 2
                            new VehicleInfo { BienSo = "43B-45678", IdChu = "004", TenChu = "Phạm Văn D", HetHan = "04/04/2025", NgayRa = "10/03/2024", OgiuXe = 2 },
                            new VehicleInfo { BienSo = "43B-56789", IdChu = "005", TenChu = "Hoàng Văn E", HetHan = "05/05/2025", NgayRa = "12/03/2024", OgiuXe = 2 },
                            new VehicleInfo { BienSo = "43B-67890", IdChu = "006", TenChu = "Vũ Văn F", HetHan = "06/06/2025", NgayRa = "18/03/2024", OgiuXe = 2 },
                            new VehicleInfo { BienSo = "43B-78901", IdChu = "007", TenChu = "Đặng Văn G", HetHan = "07/07/2025", NgayRa = "22/03/2024", OgiuXe = 2 },
                            // Ô số 3
                            new VehicleInfo { BienSo = "43C-89012", IdChu = "008", TenChu = "Bùi Văn H", HetHan = "08/08/2025", NgayRa = "14/03/2024", OgiuXe = 3 },
                            new VehicleInfo { BienSo = "43C-90123", IdChu = "009", TenChu = "Đỗ Văn I", HetHan = "09/09/2025", NgayRa = "16/03/2024", OgiuXe = 3 },
                            // Ô số 4
                            new VehicleInfo { BienSo = "43D-01234", IdChu = "010", TenChu = "Ngô Văn J", HetHan = "10/10/2025", NgayRa = "11/03/2024", OgiuXe = 4 },
                            new VehicleInfo { BienSo = "43D-12345", IdChu = "011", TenChu = "Trịnh Văn K", HetHan = "11/11/2025", NgayRa = "13/03/2024", OgiuXe = 4 },
                            new VehicleInfo { BienSo = "43D-23456", IdChu = "012", TenChu = "Lương Văn L", HetHan = "12/12/2025", NgayRa = "17/03/2024", OgiuXe = 4 },
                            new VehicleInfo { BienSo = "43D-34567", IdChu = "013", TenChu = "Phan Văn M", HetHan = "13/01/2025", NgayRa = "19/03/2024", OgiuXe = 4 },
                            new VehicleInfo { BienSo = "43D-45678", IdChu = "014", TenChu = "Vương Văn N", HetHan = "14/02/2025", NgayRa = "21/03/2024", OgiuXe = 4 },
                            new VehicleInfo { BienSo = "43D-56789", IdChu = "015", TenChu = "Tạ Văn O", HetHan = "15/03/2025", NgayRa = "23/03/2024", OgiuXe = 4 },
                            new VehicleInfo { BienSo = "43D-67890", IdChu = "016", TenChu = "Chu Văn P", HetHan = "16/04/2025", NgayRa = "24/03/2024", OgiuXe = 4 },
                            new VehicleInfo { BienSo = "43D-78901", IdChu = "017", TenChu = "Hồ Văn Q", HetHan = "17/05/2025", NgayRa = "26/03/2024", OgiuXe = 4 },
                            // Ô số 5
                            new VehicleInfo { BienSo = "43E-89012", IdChu = "018", TenChu = "Đoàn Văn R", HetHan = "18/06/2025", NgayRa = "27/03/2024", OgiuXe = 5 },
                            new VehicleInfo { BienSo = "43E-90123", IdChu = "019", TenChu = "Lý Văn S", HetHan = "19/07/2025", NgayRa = "28/03/2024", OgiuXe = 5 },
                            new VehicleInfo { BienSo = "43E-01234", IdChu = "020", TenChu = "Mai Văn T", HetHan = "20/08/2025", NgayRa = "29/03/2024", OgiuXe = 5 },
                            new VehicleInfo { BienSo = "43E-12345", IdChu = "021", TenChu = "Hà Văn U", HetHan = "21/09/2025", NgayRa = "30/03/2024", OgiuXe = 5 },
                            new VehicleInfo { BienSo = "43E-23456", IdChu = "022", TenChu = "Lê Văn V", HetHan = "22/10/2025", NgayRa = "31/03/2024", OgiuXe = 5 },
                            new VehicleInfo { BienSo = "43E-34567", IdChu = "023", TenChu = "Trần Văn W", HetHan = "23/11/2025", NgayRa = "01/04/2024", OgiuXe = 5 },
                            new VehicleInfo { BienSo = "43E-45678", IdChu = "024", TenChu = "Nguyễn Văn X", HetHan = "24/12/2025", NgayRa = "02/04/2024", OgiuXe = 5 },
                            new VehicleInfo { BienSo = "43E-56789", IdChu = "025", TenChu = "Phạm Văn Y", HetHan = "25/01/2025", NgayRa = "03/04/2024", OgiuXe = 5 },
                            new VehicleInfo { BienSo = "43E-67890", IdChu = "026", TenChu = "Hoàng Văn Z", HetHan = "26/02/2025", NgayRa = "04/04/2024", OgiuXe = 5 },
                            // Ô số 6
                            new VehicleInfo { BienSo = "43F-78901", IdChu = "027", TenChu = "Vũ Văn AA", HetHan = "27/03/2025", NgayRa = "05/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-89012", IdChu = "028", TenChu = "Đặng Văn AB", HetHan = "28/04/2025", NgayRa = "06/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-90123", IdChu = "029", TenChu = "Bùi Văn AC", HetHan = "29/05/2025", NgayRa = "07/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-01234", IdChu = "030", TenChu = "Đỗ Văn AD", HetHan = "30/06/2025", NgayRa = "08/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-12345", IdChu = "031", TenChu = "Ngô Văn AE", HetHan = "01/07/2025", NgayRa = "09/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-23456", IdChu = "032", TenChu = "Trịnh Văn AF", HetHan = "02/08/2025", NgayRa = "10/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-34567", IdChu = "033", TenChu = "Lương Văn AG", HetHan = "03/09/2025", NgayRa = "11/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-45678", IdChu = "034", TenChu = "Phan Văn AH", HetHan = "04/10/2025", NgayRa = "12/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-56789", IdChu = "035", TenChu = "Vương Văn AI", HetHan = "05/11/2025", NgayRa = "13/04/2024", OgiuXe = 6 },
                            new VehicleInfo { BienSo = "43F-67890", IdChu = "036", TenChu = "Tạ Văn AJ", HetHan = "06/12/2025", NgayRa = "14/04/2024", OgiuXe = 6 },
                            // Ô số 7
                            new VehicleInfo { BienSo = "43G-78901", IdChu = "037", TenChu = "Chu Văn AK", HetHan = "07/01/2025", NgayRa = "15/04/2024", OgiuXe = 7 },
                            // Ô số 9
                            new VehicleInfo { BienSo = "43I-89012", IdChu = "038", TenChu = "Hồ Văn AL", HetHan = "08/02/2025", NgayRa = "16/04/2024", OgiuXe = 9 },
                            new VehicleInfo { BienSo = "43I-90123", IdChu = "039", TenChu = "Đoàn Văn AM", HetHan = "09/03/2025", NgayRa = "17/04/2024", OgiuXe = 9 },
                            new VehicleInfo { BienSo = "43I-01234", IdChu = "040", TenChu = "Lý Văn AN", HetHan = "10/04/2025", NgayRa = "18/04/2024", OgiuXe = 9 },
                            new VehicleInfo { BienSo = "43I-12345", IdChu = "041", TenChu = "Mai Văn AO", HetHan = "11/05/2025", NgayRa = "19/04/2024", OgiuXe = 9 },
                            // Ô số 10
                            new VehicleInfo { BienSo = "43K-23456", IdChu = "042", TenChu = "Hà Văn AP", HetHan = "12/06/2025", NgayRa = "20/04/2024", OgiuXe = 10 },
                            new VehicleInfo { BienSo = "43K-34567", IdChu = "043", TenChu = "Lê Văn AQ", HetHan = "13/07/2025", NgayRa = "21/04/2024", OgiuXe = 10 },
                            new VehicleInfo { BienSo = "43K-45678", IdChu = "044", TenChu = "Trần Văn AR", HetHan = "14/08/2025", NgayRa = "22/04/2024", OgiuXe = 10 },
                            new VehicleInfo { BienSo = "43K-56789", IdChu = "045", TenChu = "Nguyễn Văn AS", HetHan = "15/09/2025", NgayRa = "23/04/2024", OgiuXe = 10 },
                            new VehicleInfo { BienSo = "43K-67890", IdChu = "046", TenChu = "Phạm Văn AT", HetHan = "16/10/2025", NgayRa = "24/04/2024", OgiuXe = 10 }
                        );
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