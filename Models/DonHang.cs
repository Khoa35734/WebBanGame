using System;
using System.Collections.Generic;

namespace WebBanGame.Models;

public partial class DonHang
{
    public int MaDh { get; set; }

    public int MaKh { get; set; }

    public DateTime NgayTao { get; set; }

    public bool TrangThaiHuyDon { get; set; }

    public bool ThanhToan { get; set; }

    public DateTime NgayThanhToan { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
