namespace WebBanGame.Models
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public IEnumerable<SanPham> Games { get; set; }
    }
}

