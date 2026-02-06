using System.ComponentModel.DataAnnotations;

public class DanhMucSanPham
{
    [Key]
    public int Id { get; set; }
    public string TenDanhMuc { get; set; } = "";
    public IEnumerable<SanPham> SanPhams { get; set; }
    
}