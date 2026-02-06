//Thâm xử lý validation

using System.ComponentModel.DataAnnotations;
using dotnet05_api_base.Controllers;
using dotnet05_api_base.Models;

public class SanPhamInsertDTO
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [MaxLength(50, ErrorMessage = "Tên sản phẩm không được vượt quá 50 ký tự")]
    public string TenSanPham { get; set; } = "";
    [Required(ErrorMessage = "Giá bán không được để trống")]
    [Range(0, 100000000, ErrorMessage = "Giá bán phải là số dương")]
    public double GiaBan { get; set; }
    [MaxLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
    public string MoTa { get; set; } = "";
    [MaxLength(200, ErrorMessage = "Hình ảnh không được vượt quá 200 ký tự")]
    [Url(ErrorMessage = "Hình ảnh phải là một URL hợp lệ")]
    public string HinhAnh { get; set; } = "";
    [Required(ErrorMessage = "Id danh mục không được để trống")]

    public int IdDanhMuc { get; set; }
    public SanPhamInsertDTO()
    {
       
    }
}