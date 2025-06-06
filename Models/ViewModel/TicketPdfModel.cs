namespace PBL3.Models.ViewModel
{
    public class TicketPdfModel
    {
        public string HoTen { get; set; }
        public string MSSV { get; set; }
        public string BienSoXe { get; set; }
        public DateTime NgayDangKy { get; set; }
        public DateTime NgayHetHan { get; set; }
        public decimal Price { get; set; }
        public string SlotName { get; set; }
        public string QRCode { get; set; } 
    }
}
