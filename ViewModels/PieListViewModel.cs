using BethanysPieShop.Models;

namespace BethanysPieShop.ViewModels
{
    public class PieListViewModel
    {
        public IEnumerable<Pie> Pies { get; set; } = new List<Pie>();
        public string CurrentCategory { get; set; } = string.Empty;
    }
}
