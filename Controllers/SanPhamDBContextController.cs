using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using dotnet05_api_base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
//using dotnet05_api_base.Models;

namespace dotnet05_api_base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamDBContextController : ControllerBase
    {
        private readonly QuanLySanPhamContext _context;

        public SanPhamDBContextController(QuanLySanPhamContext context)
        {
            _context = context;
        }
        [HttpGet("layDanhSachSanPham")]

        public async Task<IActionResult> LayDanhSachSanPham()
        {
            //2 Cách truy vấn 
            //Cách 1: Sử dụng LINQ
            var lstSP = _context.SanPhams.Include(item => item.danhMucSanPham).Select(item => new
            {
                maSanPham = item.Id,
                tenSanPham = item.TenSanPham,
                giaBan = item.GiaBan,
                moTa = item.MoTa,
                hinhAnh = item.HinhAnh,
                idDanhMuc = item.IdDanhMuc,
                tenDanhMuc = item.danhMucSanPham.TenDanhMuc
            });
            return Ok(lstSP);
        }
        [HttpGet("layDanhSachSanPhamRaw")]
        public async Task<IActionResult> LayDanhSachSanPhamRaw()
        {
            //Cách 2: Sử dụng câu lệnh SQL
            var lstSP = _context.Database.SqlQueryRaw<SanPhamDTO>("SELECT SanPhams.Id as MaSanPham,tenSanPham,giaBan,moTa,hinhAnh,idDanhMuc, tenDanhMuc FROM SanPhams, DanhMucSanPhams WHERE SanPhams.idDanhMuc = DanhMucSanPhams.Id").ToList();
            return Ok(lstSP);
        }


        //Ví dụ có tham số vào câu query sql raw
        [HttpGet("timKiemSanPhamPhanTrang")]
        public async Task<IActionResult> TimKiemSanPhamPhanTrang([FromQuery] string tuKhoa = "", [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
        {

            SqlParameter tuKhoaParam = new SqlParameter("@tuKhoa", SqlDbType.NVarChar, 50) { Value = @$"%{tuKhoa}%" };
            SqlParameter pageIndexParam = new SqlParameter("@pageIndex", SqlDbType.Int) { Value = pageIndex };
            SqlParameter pageSizeParam = new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize };


            string sqlQueryRaw = @$"SELECT SanPhams.Id as MaSanPham,tenSanPham,giaBan,moTa,hinhAnh,idDanhMuc, tenDanhMuc FROM SanPhams, DanhMucSanPhams WHERE SanPhams.idDanhMuc = DanhMucSanPhams.Id AND tenSanPham LIKE @tuKhoa ORDER BY SanPhams.Id OFFSET (@pageIndex - 1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY";


            var lstSanPham = _context.Database.SqlQueryRaw<SanPhamDTO>(sqlQueryRaw, tuKhoaParam, pageIndexParam, pageSizeParam).ToList();


            return Ok(lstSanPham);
        }

        //Ví dụ có tham số vào câu query sql linq
        [HttpGet("timKiemSanPhamPhanTrangLinQ")]
        public async Task<IActionResult> TimKiemSanPhamPhanTrangLinQ([FromQuery] string tuKhoa = "", [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
        {
            //dbset -> điều kiện lọc -> sắp xếp -> phân trang -> select
            var lstSPTimKiem = _context.SanPhams.Include(item => item.danhMucSanPham).Where(item => item.TenSanPham.Contains(tuKhoa)).OrderBy(item => item.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(item => new
            {
                maSanPham = item.Id,
                tenSanPham = item.TenSanPham,
                giaBan = item.GiaBan,
                moTa = item.MoTa,
                hinhAnh = item.HinhAnh,
                idDanhMuc = item.IdDanhMuc,
                tenDanhMuc = item.danhMucSanPham.TenDanhMuc
            });
            return Ok(lstSPTimKiem);
        }

        [HttpPost("ThemSanPhamLinQ")]
        public async Task<IActionResult> ThemSanPhamLinQ([FromBody] SanPhamInsertDTO sanPhamInsertDTO)
        {

            //Kiểm tra mã danh mục có tổn tại hay không
            var danhMuc = await _context.DanhMucSanPhams.SingleAsync(item => item.Id == sanPhamInsertDTO.IdDanhMuc);
            if (danhMuc == null)
            {
                return BadRequest(new { message = "Danh mục sản phẩm không tồn tại!" });
            }

            //Khởi tạo model của bản sanPham
            SanPham sanPhamInsert = new SanPham();
            sanPhamInsert.TenSanPham = sanPhamInsertDTO.TenSanPham;
            sanPhamInsert.GiaBan = sanPhamInsertDTO.GiaBan;
            sanPhamInsert.MoTa = sanPhamInsertDTO.MoTa;
            sanPhamInsert.HinhAnh = sanPhamInsertDTO.HinhAnh;
            sanPhamInsert.IdDanhMuc = sanPhamInsertDTO.IdDanhMuc;
            //Sử dụng linq để thêm
            _context.SanPhams.Add(sanPhamInsert);
            _context.SaveChanges(); //tương ứng với lệnh _contxt.database.commit() trong sqlAlchemy
            return StatusCode(201, new { message = "Thêm sản phẩm thành công!", data = sanPhamInsertDTO });
        }

        [HttpPost("ThemSanPhamRaw")]
        public async Task<IActionResult> ThemSanPhamRaw([FromBody] SanPhamInsertDTO sanPhamInsertDTO)
        {
            try
            {
                var danhMucSP = await _context.DanhMucSanPhams.SingleAsync(item => item.Id == sanPhamInsertDTO.IdDanhMuc);
                if (danhMucSP == null)
                {
                    return BadRequest(new { message = "Danh mục sản phẩm không tồn tại!" });
                }
                await _context.Database.BeginTransactionAsync();
                //Sử dụng câu lệnh sql raw để thêm
                string sqlInsert = "INSERT INTO SanPhams (tenSanPham, giaBan, moTa, hinhAnh, idDanhMuc) VALUES (@tenSanPham, @giaBan, @moTa, @hinhAnh, @idDanhMuc)";

                SqlParameter tenSanPhamParam = new SqlParameter("@tenSanPham", SqlDbType.NVarChar, 50) { Value = sanPhamInsertDTO.TenSanPham };
                SqlParameter giaBanParam = new SqlParameter("@giaBan", SqlDbType.Float) { Value = sanPhamInsertDTO.GiaBan };
                SqlParameter moTaParam = new SqlParameter("@moTa", SqlDbType.NVarChar, 200) { Value = sanPhamInsertDTO.MoTa };
                SqlParameter hinhAnhParam = new SqlParameter("@hinhAnh", SqlDbType.NVarChar, 200) { Value = sanPhamInsertDTO.HinhAnh };
                SqlParameter idDanhMucParam = new SqlParameter("@idDanhMuc", SqlDbType.Int) { Value = sanPhamInsertDTO.IdDanhMuc };
                int rowsAffected = _context.Database.ExecuteSqlRaw(sqlInsert, tenSanPhamParam, giaBanParam, moTaParam, hinhAnhParam, idDanhMucParam);
                if (rowsAffected > 0)
                {
                    await _context.Database.CommitTransactionAsync();

                    return StatusCode(201, new { message = "Thêm sản phẩm thành công!", data = sanPhamInsertDTO });
                }
                else
                {
                    return BadRequest(new { message = "Thêm sản phẩm thất bại!" });
                }
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                return BadRequest(new { message = "Thêm sản phẩm thất bại!", error = ex.Message });
            }
        }
        [HttpDelete("XoaSanPhamRaw/{id}")]
        public async Task<IActionResult> XoaSanPham([FromRoute] int id)
        {
            //Kiểm tra mã sản phẩm có tồn tại trong bảng sản phâm hay không
            var sp = await _context.SanPhams.SingleOrDefaultAsync(item => item.Id == id);
            if(sp == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại!" });
            }

            try
            {
                SqlParameter idParam = new SqlParameter("@idSanPham", SqlDbType.Int) { Value = id };

                await _context.Database.BeginTransactionAsync();
                string sqlRaw = "DELETE FROM SanPhams WHERE Id = @idSanPham"; //Nghiệp vụ xoá sản phẩm thật (và thường không áp dụng trong thực tế)
                int rowsAffected = _context.Database.ExecuteSqlRaw(sqlRaw, idParam);

            }catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                return BadRequest(new { message = "Xoá sản phẩm thất bại!", error = ex.Message });
            }




            return Ok(new { message = "Xoá sản phẩm thành công!" } );
        }
    }





}
