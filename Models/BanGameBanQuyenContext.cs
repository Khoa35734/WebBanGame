using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebBanGame.Models;

public partial class BanGameBanQuyenContext : DbContext
{
    public BanGameBanQuyenContext()
    {
    }

    public BanGameBanQuyenContext(DbContextOptions<BanGameBanQuyenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountAdmin> AccountAdmins { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhMucSp> DanhMucSps { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<NapTien> NapTiens { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=ADMIN-PC\\MSSQLSERVER02;Initial Catalog=BanGameBanQuyen;Persist Security Info=True;User ID=sa;Password=khoaphamby;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountAdmin>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__AccountA__349DA586C498078D");

            entity.ToTable("AccountAdmin");

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Email)
                .HasMaxLength(24)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TaiKhoan)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaCtdh).HasName("PK__ChiTietD__1E4E40F05ACAC64D");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.MaCtdh).HasColumnName("MaCTDH");
            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__Ngayg__4316F928");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDon__MaSP__440B1D61");
        });

        modelBuilder.Entity<DanhMucSp>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DanhMucS__2725866E4825DDF8");

            entity.ToTable("DanhMucSP");

            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.MoTaDm)
                .HasMaxLength(500)
                .HasColumnName("MoTaDM");
            entity.Property(e => e.TenDm)
                .HasMaxLength(200)
                .HasColumnName("TenDM");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDh).HasName("PK__DonHang__27258661FFFC2296");

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHang_KhachHang");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1E0BBCEEB3");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.Diachi).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.MatKhau).HasMaxLength(50);
            entity.Property(e => e.SoDuTk)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SoDuTK");
            entity.Property(e => e.TenKh)
                .HasMaxLength(200)
                .HasColumnName("TenKH");
            entity.Property(e => e.TenTaiKhoan).HasMaxLength(50);
        });

        modelBuilder.Entity<NapTien>(entity =>
        {
            entity.HasKey(e => e.IdNapTien);

            entity.ToTable("NapTien");

            entity.Property(e => e.BankTransactionId).HasMaxLength(50);
            entity.Property(e => e.NgayNap).HasColumnType("datetime");
            entity.Property(e => e.SoTienNap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.NapTiens)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_NapTien_KhachHang");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SanPham__2725081C37D0281D");

            entity.ToTable("SanPham");

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.AnhSp).HasColumnName("AnhSP");
            entity.Property(e => e.GiaSp).HasColumnName("GiaSP");
            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.MotaSp).HasColumnName("MotaSP");
            entity.Property(e => e.TenSp)
                .HasMaxLength(200)
                .HasColumnName("TenSP");

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDm)
                .HasConstraintName("FK__SanPham__MotaSP__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
