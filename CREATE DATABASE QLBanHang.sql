CREATE DATABASE QLBanHang
use QLBanHang

CREATE TABLE KhachHang(
    id int PRIMARY KEY IDENTITY(1,1),
    tenKhachHang NVARCHAR(255) NOT NULL,
    email NVARCHAR(255) NOT NULL,
    soDienThoai NVARCHAR(15) NULL,
    diaChi NVARCHAR(255) NULL
);

CREATE TABLE DonHang(
    id int PRIMARY KEY IDENTITY(1,1),
    khachHangId int NOT NULL,
    ngayDatHang DATETIME NOT NULL DEFAULT GETDATE(),
    trangThai NVARCHAR(50) NOT NULL DEFAULT 'Chua xu ly',
);

CREATE TABLE SanPham(
    id int PRIMARY KEY IDENTITY(1,1),
    tenSanPham NVARCHAR(255) NOT NULL,
    gia DECIMAL(10,2) NOT NULL,
    soLuongTon INT NOT NULL DEFAULT 0
);


CREATE TABLE ChiTietDonHang(
    idDonHang int,
    idSanPham int,
    soLuong int,
    ngayDat DATETIME DEFAULT GETDATE(),
    gia DECIMAL(10,2) NOT NULL
)

/* Add khoá chính  */
ALTER TABLE ChiTietDonHang
ADD CONSTRAINT PK_ChiTietDonHang PRIMARY KEY (idDonHang, idSanPham)

/*Add column số lượng*/
ALTER TABLE SanPham
ADD soLuong INT NOT NULL DEFAULT 0

/*Tạo ràng buộc khoá ngoại cho bảng chi tiết đơn hàng*/
ALTER TABLE ChiTietDonHang 
ADD CONSTRAINT FK_ChiTietDonHang_DonHang
FOREIGN KEY (idDonHang) REFERENCES DonHang(id)

/*Tạo ràng buộc khoá ngoại bảng sản phẩm*/
ALTER TABLE ChiTietDonHang
ADD CONSTRAINT FK_ChiTietDonHang_SanPham
FOREIGN KEY (idSanPham) REFERENCES SanPham(id)




/*TẠO RÀNG BUỘC TOÀN VẸN DỮ LIỆU KHOÁ NGOẠI*/
ALTER TABLE DonHang 
ADD CONSTRAINT FK_DonHang_KhachHang
FOREIGN KEY (khachHangId) REFERENCES KhachHang(id)
ON DELETE CASCADE 


/*XÓA RÀNG BUỘC TOÀN VẸN DỮ LIỆU KHOÁ NGOẠI*/
ALTER TABLE DonHang DROP CONSTRAINT FK_1


/*Sql command: insert (thêm dữ liệu vào table)*/

/*Câu lệnh thêm dữ liệu các trường cụ thể*/
INSERT into KhachHang (tenKhachHang, email, soDienThoai, diaChi) 
VALUES(N'Nguyễn Văn B','nguyenvanb@gmail.com','0123456789',N'Hà Nội'),
      (N'Trần Thị C','nguyenvanc@gmail.com','0987654321',N'Hồ Chí Minh')

INSERT into KhachHang (tenKhachHang, email, soDienThoai, diaChi) 
VALUES(N'Nguyễn Văn D','nguyenvand@gmail.com','0123452789',N'Hà Nội')

/*đọc dữ liệu của table*/
SELECT * from KhachHang
SELECT top 2 * from KhachHang
SELECT * from KhachHang
WHERE soDienThoai = '0123456789' OR diaChi = N'Hà Nội' /*lọc theo số điện thoại, có thể kết hợp and và or*/ 

/*select offset*/
SELECT * from KhachHang
ORDER BY id /*phải có order by khi dùng offset*/ 
OFFSET 10 * 2 ROWS FETCH NEXT 10 ROWS ONLY /*bỏ qua 2 dòng */

/*Cập nhật dữ liệu*/
UPDATE KhachHang
set tenKhachHang = N'Nguyễn Thị F'
WHERE id = 22

/*Xoá dữ liệu*/
DELETE from KhachHang
WHERE id = 23