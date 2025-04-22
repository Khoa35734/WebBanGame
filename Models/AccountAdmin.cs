using System;
using System.Collections.Generic;

namespace WebBanGame.Models;

public partial class AccountAdmin
{
    public int AccountId { get; set; }

    public string TaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int Phone { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly CreateDate { get; set; }
}
