using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebBanGame.Models
{
    public class SanPhamEditViewModel
    {
        public int MaSp { get; set; }
        public string TenSp { get; set; }
        public string AnhSp { get; set; }
        public int GiaSp { get; set; }
        public bool TrangThai { get; set; }
        public bool BestSeller { get; set; }
        public DateOnly CreateDate { get; set; }
        public DateOnly NgaySua { get; set; }
        public string LinkDownGame { get; set; }
        public string MotaSp { get; set; }

        // Chọn nhiều danh mục
        public List<int> SelectedDanhMucs { get; set; } = new List<int>();
        public List<SelectListItem> AllDanhMucs { get; set; } = new List<SelectListItem>();
    }
}
