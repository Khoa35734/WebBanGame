using System;
using System.Collections.Generic;

namespace WebBanGame.Models;

public partial class NapTien
{
    public int IdNapTien { get; set; }

    public int? IdUser { get; set; }

    public decimal? SoTienNap { get; set; }

    public string? BankTransactionId { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayNap { get; set; }

    public virtual KhachHang? IdUserNavigation { get; set; }
}
