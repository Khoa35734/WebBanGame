using System;
using System.Collections.Generic;

namespace WebBanGame.Models
{
    public partial class SanPham
    {
        public int MaSp { get; set; }

        public int? MaDm { get; set; }

        public string TenSp { get; set; } = null!;

        public string AnhSp { get; set; } = null!;

        public string? VideoSp { get; set; }

        public int GiaSp { get; set; }

        public bool TrangThai { get; set; }

        public bool BestSeller { get; set; }

        public DateOnly CreateDate { get; set; }

        public DateOnly NgaySua { get; set; }

        public  string? LinkDownGame { get; set; }

        public  string MotaSp { get; set; } = null!;

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

        public virtual DanhMucSp? MaDmNavigation { get; set; }

        // New: Relation to multiple categories
      
    }
}