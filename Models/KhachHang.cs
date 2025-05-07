using System;
using System.Collections.Generic;

namespace WebBanGame.Models;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string TenKh { get; set; } = null!;

    public string Diachi { get; set; } = null!;

    public DateOnly Ngaysinh { get; set; }

    public int Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? CreateDate { get; set; }

    public string? TenTaiKhoan { get; set; }

    public string? MatKhau { get; set; }

    public decimal? SoDuTk { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
