using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanGame.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIsActiveToNonNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountAdmin",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiKhoan = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Phone = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "varchar(24)", unicode: false, maxLength: 24, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreateDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AccountA__349DA586C498078D", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucSP",
                columns: table => new
                {
                    MaDM = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDM = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AnhDM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaDM = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DanhMucS__2725866E4825DDF8", x => x.MaDM);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKH = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Diachi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Ngaysinh = table.Column<DateOnly>(type: "date", nullable: false),
                    Phone = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TenTaiKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MatKhau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoDuTK = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__2725CF1E0BBCEEB3", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    MaSP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDM = table.Column<int>(type: "int", nullable: true),
                    TenSP = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AnhSP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoSP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaSP = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    BestSeller = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NgaySua = table.Column<DateOnly>(type: "date", nullable: false),
                    LinkDownGame = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotaSP = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SanPham__2725081C37D0281D", x => x.MaSP);
                    table.ForeignKey(
                        name: "FK__SanPham__MotaSP__3B75D760",
                        column: x => x.MaDM,
                        principalTable: "DanhMucSP",
                        principalColumn: "MaDM");
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThaiHuyDon = table.Column<bool>(type: "bit", nullable: false),
                    ThanhToan = table.Column<bool>(type: "bit", nullable: false),
                    NgayThanhToan = table.Column<DateOnly>(type: "date", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonHang__27258661FFFC2296", x => x.MaDH);
                    table.ForeignKey(
                        name: "FK_DonHang_KhachHang",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    MaCTDH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDH = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<int>(type: "int", nullable: false),
                    Ngaygiao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietD__1E4E40F05ACAC64D", x => x.MaCTDH);
                    table.ForeignKey(
                        name: "FK__ChiTietDo__Ngayg__4316F928",
                        column: x => x.MaDH,
                        principalTable: "DonHang",
                        principalColumn: "MaDH");
                    table.ForeignKey(
                        name: "FK__ChiTietDon__MaSP__440B1D61",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaDH",
                table: "ChiTietDonHang",
                column: "MaDH");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaSP",
                table: "ChiTietDonHang",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKH",
                table: "DonHang",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaDM",
                table: "SanPham",
                column: "MaDM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountAdmin");

            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "DanhMucSP");
        }
    }
}
