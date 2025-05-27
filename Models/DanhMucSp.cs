using System;
using System.Collections.Generic;

namespace WebBanGame.Models;

public partial class DanhMucSp
{
    public int MaDm { get; set; }

    public string TenDm { get; set; } = null!;

    public string? MoTaDm { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
