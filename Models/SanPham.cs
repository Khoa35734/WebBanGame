﻿using System;
using System.Collections.Generic;

namespace WebBanGame.Models;

public partial class SanPham
{
    public int MaSp { get; set; }

    public string TenSp { get; set; } = null!;

    public string AnhSp { get; set; } = null!;

    public int GiaSp { get; set; }

    public bool TrangThai { get; set; }

    public bool BestSeller { get; set; }

    public DateOnly CreateDate { get; set; }

    public DateOnly NgaySua { get; set; }

    public string? LinkDownGame { get; set; }

    public string MotaSp { get; set; } = null!;

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<DanhMucSp> DanhMucs { get; set; } = new List<DanhMucSp>();
}
