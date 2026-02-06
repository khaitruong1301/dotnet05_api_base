//ef-context
using Microsoft.EntityFrameworkCore;

namespace dotnet05_api_base.Models
{
    public class QuanLySanPhamContext : DbContext
    {
        public QuanLySanPhamContext() { }
        public QuanLySanPhamContext(DbContextOptions<QuanLySanPhamContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=QuanLySanPham;User Id=sa;Password=Cybersoft123@; TrustServerCertificate=True");
        }

        public DbSet<SanPham> SanPhams { get; set; }

        public DbSet<DanhMucSanPham> DanhMucSanPhams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SanPham>()
                .HasOne(s => s.danhMucSanPham)
                .WithMany(dm => dm.SanPhams)
                .HasForeignKey(s => s.IdDanhMuc);
            
            modelBuilder.Entity<DanhMucSanPham>()
                .HasMany(dm => dm.SanPhams)
                .WithOne(s => s.danhMucSanPham)
                .HasForeignKey(s => s.IdDanhMuc);
        }

    }
}