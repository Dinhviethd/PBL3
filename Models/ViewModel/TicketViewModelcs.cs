namespace PBL3.Models.ViewModel
{
    public class TicketViewModel
    {
        public string BienSoXe { get; set; }
        public string PackageName { get; set; } // Tên gói (1/3/6 tháng)
        public decimal Price { get; set; }     // Giá tiền
        public string HoTen { get; set; }     // Thông tin sinh viên
        public string MSSV { get; set; }
        public string Lop { get; set; }
    }
}