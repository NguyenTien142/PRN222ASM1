# Hướng dẫn Upload File Vehicle Import

## Vấn đề thường gặp và cách khắc phục

### 1. Project bị dừng khi upload file

**Nguyên nhân có thể:**
- File quá lớn (>5MB)
- File bị hỏng hoặc không đúng định dạng
- Lỗi mạng hoặc timeout
- Memory overflow

**Cách khắc phục:**
1. **Kiểm tra kích thước file**: Đảm bảo file < 5MB
2. **Kiểm tra định dạng**: Chỉ hỗ trợ .csv, .xlsx, .xls
3. **Thử file nhỏ hơn**: Chia file lớn thành nhiều file nhỏ
4. **Reload trang**: Refresh browser và thử lại

### 2. Cách sử dụng chức năng Import

1. **Tải template**: Click "Download Template" để tải file mẫu
2. **Chuẩn bị data**: Sử dụng format như trong template
3. **Upload file**: Click "Choose File" hoặc kéo thả file
4. **Bắt đầu import**: Click "Start Import"

### 3. Format file CSV chuẩn

```csv
CategoryId,Model,Color,Price,ManufactureDate,Version,Quantity,Image
1,Tesla Model 3,Black,45000,2023-01-15,Standard,5,/images/tesla-model3-black.jpg
```

**Các cột bắt buộc:**
- CategoryId: ID danh mục (số nguyên)
- Model: Tên model xe
- Color: Màu sắc
- Price: Giá (số)
- ManufactureDate: Ngày sản xuất (YYYY-MM-DD)
- Version: Phiên bản
- Quantity: Số lượng (số nguyên)
- Image: Đường dẫn hình ảnh

### 4. Các cải tiến đã thực hiện

✅ **Logging chi tiết**: Thêm log để theo dõi quá trình upload
✅ **Xử lý lỗi tốt hơn**: Phân loại và hiển thị lỗi cụ thể
✅ **Timeout protection**: Tự động hủy request sau 2 phút
✅ **Memory management**: Dọn dẹp bộ nhớ sau upload
✅ **File validation**: Kiểm tra file trước khi upload
✅ **Progress tracking**: Hiển thị tiến trình upload
✅ **Error recovery**: Cho phép thử lại sau lỗi

### 5. Troubleshooting

**Nếu vẫn bị lỗi:**

1. **Mở Developer Tools (F12)**
2. **Xem Console tab** để kiểm tra lỗi
3. **Kiểm tra Network tab** để xem request/response
4. **Thử với file template** trước

**Lỗi thường gặp:**
- `File too large`: File > 5MB
- `Invalid format`: File không đúng định dạng
- `Timeout`: Upload quá lâu
- `Network error`: Lỗi kết nối

### 6. Tips để upload thành công

✅ **Sử dụng file < 1MB** để đảm bảo ổn định
✅ **Kiểm tra data** trước khi upload
✅ **Đóng các tab khác** để giải phóng memory
✅ **Thử upload từng ít record** thay vì upload hàng nghìn record
✅ **Đảm bảo internet ổn định**