using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DACS403A
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            HienThiDuLieu();
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
        }
        string ketnoi = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\thaib\Downloads\DACS403A\DACS403A\DACS403A\Database1.mdf;Integrated Security=True";
        private void HienThiDuLieu()
        {
            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                // Truy vấn để lấy dữ liệu từ bảng NhanKhau
                string query = "SELECT * FROM NhanKhau";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Hiển thị dữ liệu trong DataGridView
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Update the ngaysinh_tb TextBox with the selected date
            ngaysinh_tb.Text = dateTimePicker1.Value.ToString("dd-MM-yyyy");
        }

        private void thembtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hoten_tb.Text) || string.IsNullOrWhiteSpace(cccd_tb.Text) ||
            string.IsNullOrWhiteSpace(quoctich_tb.Text) || string.IsNullOrWhiteSpace(ngaysinh_tb.Text))
            {
                MessageBox.Show("Họ tên, CCCD, Quốc tịch và Ngày sinh không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Kiểm tra mã nhân khẩu có tồn tại không
            if (IsMaNhanKhauExists(manhankhau_tb.Text))
            {
                MessageBox.Show("Mã nhân khẩu đã tồn tại. Vui lòng nhập mã khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra CCCD đã tồn tại
            if (cccd_tb != null && !string.IsNullOrWhiteSpace(cccd_tb.Text) && IsCCCDExists(cccd_tb.Text))
            {
                MessageBox.Show("CCCD đã tồn tại. Vui lòng nhập CCCD khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra số điện thoại đã tồn tại
            if (IsPhoneNumberExists(sdt_tb.Text))
            {
                MessageBox.Show("Số điện thoại đã tồn tại. Vui lòng nhập số điện thoại khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                // Chuẩn bị câu lệnh SQL INSERT
                string query = "INSERT INTO NhanKhau (IdNK, Hovaten, NgaySinh, NoiSinh, NguyenQuan, DanToc, NgheNghiep, NoiLamViec, CCCD, QuocTich, NoiThuongTru, SoDienThoai, TrinhDoHocVan, DiaChiThuongTru, GioiTinh) " +
                               "VALUES (@IdNK, @Hovaten, @NgaySinh, @NoiSinh, @NguyenQuan, @DanToc, @NgheNghiep, @NoiLamViec, @CCCD, @QuocTich, @NoiThuongTru, @SoDienThoai, @TrinhDoHocVan, @DiaChiThuongTru, @GioiTinh)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Truyền giá trị từ các controls vào các tham số trong câu lệnh SQL
                    command.Parameters.AddWithValue("@IdNK", manhankhau_tb.Text);
                    command.Parameters.AddWithValue("@Hovaten", hoten_tb.Text);
                    // Lấy giá trị từ DateTimePicker và chỉ lấy ngày/tháng/năm
                    DateTimePicker dateTimePicker = dateTimePicker1; // Thay "yourDateTimePickerControl" bằng tên thực tế của DateTimePicker
                    DateTime ngaySinh = dateTimePicker.Value;
                    command.Parameters.AddWithValue("@NgaySinh", ngaySinh.ToString("dd-MM-yyyy")); // Chuyển định dạng ngày/tháng/năm
                    command.Parameters.AddWithValue("@NoiSinh", noisinh_tb.Text);
                    command.Parameters.AddWithValue("@NguyenQuan", nguyenquan_tb.Text);
                    command.Parameters.AddWithValue("@DanToc", dantoc_tb.Text);
                    command.Parameters.AddWithValue("@NgheNghiep", nghenghiep_tb.Text);
                    command.Parameters.AddWithValue("@NoiLamViec", noilamviec_tb.Text);
                    command.Parameters.AddWithValue("@CCCD", cccd_tb.Text);
                    command.Parameters.AddWithValue("@QuocTich", quoctich_tb.Text);
                    command.Parameters.AddWithValue("@NoiThuongTru", noithuongtru_tb.Text);
                    command.Parameters.AddWithValue("@SoDienThoai", sdt_tb.Text);
                    command.Parameters.AddWithValue("@TrinhDoHocVan", trinhdohocvan_tb.Text);
                    command.Parameters.AddWithValue("@DiaChiThuongTru", diachithuongtru_tb.Text);
                    command.Parameters.AddWithValue("@GioiTinh", gioitinh_tb.Text);

                    // Thực thi câu lệnh INSERT
                    int rowsAffected = command.ExecuteNonQuery();

                    // Hiển thị thông báo thành công
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thêm dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Sau khi thêm dữ liệu, cập nhật DataGridView
                        HienThiDuLieu();
                    }
                    else
                    {
                        MessageBox.Show("Thêm dữ liệu không thành công!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void xoabtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa dữ liệu không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(ketnoi))
                {
                    connection.Open();

                    // Chuẩn bị câu lệnh SQL DELETE
                    string query = "DELETE FROM NhanKhau WHERE IdNK = @IdNK";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Truyền giá trị từ các controls vào các tham số trong câu lệnh SQL
                        command.Parameters.AddWithValue("@IdNK", manhankhau_tb.Text);

                        // Thực thi câu lệnh DELETE
                        int rowsAffected = command.ExecuteNonQuery();

                        // Hiển thị thông báo thành công hoặc thất bại
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Sau khi xóa dữ liệu, cập nhật DataGridView
                            HienThiDuLieu();
                        }
                        else
                        {
                            MessageBox.Show("Xóa dữ liệu không thành công!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void suabtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn sửa dữ liệu không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


            if (result == DialogResult.Yes)
            {
                if (string.IsNullOrWhiteSpace(hoten_tb.Text) || string.IsNullOrWhiteSpace(cccd_tb.Text) ||
         string.IsNullOrWhiteSpace(quoctich_tb.Text) || string.IsNullOrWhiteSpace(ngaysinh_tb.Text))
                {
                    MessageBox.Show("Họ tên, CCCD, Quốc tịch và Ngày sinh không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra mã nhân khẩu có tồn tại không (trừ trường hợp sửa chính bản thân)
                if (manhankhau_tb.Tag != null && manhankhau_tb.Text != manhankhau_tb.Tag.ToString() && IsMaNhanKhauExists(manhankhau_tb.Text))
                {
                    MessageBox.Show("Mã nhân khẩu đã tồn tại. Vui lòng nhập mã khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra CCCD đã tồn tại (trừ trường hợp sửa chính bản thân)
                if (cccd_tb != null && !string.IsNullOrWhiteSpace(cccd_tb.Text) && cccd_tb.Tag != null && cccd_tb.Text != cccd_tb.Tag.ToString() && IsCCCDExists(cccd_tb.Text))
                {
                    MessageBox.Show("CCCD đã tồn tại. Vui lòng nhập CCCD khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra CCCD đã tồn tại (trừ trường hợp sửa chính bản thân)
                if (cccd_tb != null && !string.IsNullOrWhiteSpace(cccd_tb.Text) && cccd_tb.Tag != null && cccd_tb.Text != cccd_tb.Tag.ToString() && IsCCCDExists(cccd_tb.Text))
                {
                    MessageBox.Show("CCCD đã tồn tại. Vui lòng nhập CCCD khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using (SqlConnection connection = new SqlConnection(ketnoi))
                {
                    connection.Open();

                    // Chuẩn bị câu lệnh SQL UPDATE
                    string query = "UPDATE NhanKhau SET Hovaten = @Hovaten, NgaySinh = @NgaySinh, NoiSinh = @NoiSinh, NguyenQuan = @NguyenQuan, DanToc = @DanToc, " +
                                   "NgheNghiep = @NgheNghiep, NoiLamViec = @NoiLamViec, CCCD = @CCCD, QuocTich = @QuocTich, NoiThuongTru = @NoiThuongTru, SoDienThoai = @SoDienThoai, " +
                                   "TrinhDoHocVan = @TrinhDoHocVan, DiaChiThuongTru = @DiaChiThuongTru, GioiTinh = @GioiTinh WHERE IdNK = @IdNK";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Truyền giá trị từ các controls vào các tham số trong câu lệnh SQL
                        command.Parameters.AddWithValue("@IdNK", manhankhau_tb.Text);
                        command.Parameters.AddWithValue("@Hovaten", hoten_tb.Text);

                        DateTimePicker dateTimePicker = dateTimePicker1;
                        DateTime ngaySinh = dateTimePicker.Value;
                        command.Parameters.AddWithValue("@NgaySinh", ngaySinh.ToString("yyyy-MM-dd"));

                        command.Parameters.AddWithValue("@NoiSinh", noisinh_tb.Text);
                        command.Parameters.AddWithValue("@NguyenQuan", nguyenquan_tb.Text);
                        command.Parameters.AddWithValue("@DanToc", dantoc_tb.Text);
                        command.Parameters.AddWithValue("@NgheNghiep", nghenghiep_tb.Text);
                        command.Parameters.AddWithValue("@NoiLamViec", noilamviec_tb.Text);
                        command.Parameters.AddWithValue("@CCCD", cccd_tb.Text);
                        command.Parameters.AddWithValue("@QuocTich", quoctich_tb.Text);
                        command.Parameters.AddWithValue("@NoiThuongTru", noithuongtru_tb.Text);
                        command.Parameters.AddWithValue("@SoDienThoai", sdt_tb.Text);
                        command.Parameters.AddWithValue("@TrinhDoHocVan", trinhdohocvan_tb.Text);
                        command.Parameters.AddWithValue("@DiaChiThuongTru", diachithuongtru_tb.Text); // Thay đổi tên cột này
                        command.Parameters.AddWithValue("@GioiTinh", gioitinh_tb.Text);



                        // Thực thi câu lệnh UPDATE
                        int rowsAffected = command.ExecuteNonQuery();

                        // Hiển thị thông báo thành công hoặc thất bại
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sửa dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Sau khi sửa dữ liệu, cập nhật DataGridView
                            HienThiDuLieu();
                        }
                        else
                        {
                            MessageBox.Show("Sửa dữ liệu không thành công!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Đảm bảo người dùng không nhấn vào header
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Hiển thị dữ liệu của dòng được chọn lên các controls
                manhankhau_tb.Text = row.Cells["IdNK"].Value.ToString();
                hoten_tb.Text = row.Cells["Hovaten"].Value.ToString();

                if (DateTime.TryParse(row.Cells["NgaySinh"].Value.ToString(), out DateTime ngaySinh))
                {
                    dateTimePicker1.Value = ngaySinh;
                    ngaysinh_tb.Text = ngaySinh.ToString("dd-MM-yyyy");
                }

                noisinh_tb.Text = row.Cells["NoiSinh"].Value.ToString();
                nguyenquan_tb.Text = row.Cells["NguyenQuan"].Value.ToString();
                dantoc_tb.Text = row.Cells["DanToc"].Value.ToString();
                nghenghiep_tb.Text = row.Cells["NgheNghiep"].Value.ToString();
                noilamviec_tb.Text = row.Cells["NoiLamViec"].Value.ToString();
                cccd_tb.Text = row.Cells["CCCD"].Value.ToString();
                quoctich_tb.Text = row.Cells["QuocTich"].Value.ToString();
                noithuongtru_tb.Text = row.Cells["NoiThuongTru"].Value.ToString();
                sdt_tb.Text = row.Cells["SoDienThoai"].Value.ToString();
                trinhdohocvan_tb.Text = row.Cells["TrinhDoHocVan"].Value.ToString();
                diachithuongtru_tb.Text = row.Cells["DiaChiThuongTru"].Value.ToString(); // Thay đổi tên cột này
                gioitinh_tb.Text = row.Cells["GioiTinh"].Value.ToString();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Hủy sự kiện đóng form nếu người dùng không đồng ý
            }
        }

        private void timkiem_tb_TextChanged(object sender, EventArgs e)
        {
            string searchText = timkiem_tb.Text.Trim();

            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                // Truy vấn để lấy dữ liệu từ bảng NhanKhau theo ID hoặc tên
                string query = "SELECT * FROM NhanKhau WHERE IdNK LIKE @SearchText OR Hovaten LIKE @SearchText";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Hiển thị dữ liệu trong DataGridView
                    dataGridView1.DataSource = dataTable;
                }
            }
        }
        private bool IsMaNhanKhauExists(string maNhanKhau)
        {
            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                // Kiểm tra xem mã nhân khẩu đã tồn tại trong cơ sở dữ liệu chưa
                string query = "SELECT COUNT(*) FROM NhanKhau WHERE IdNK = @MaNhanKhau";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaNhanKhau", maNhanKhau);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
        private bool IsCCCDExists(string cccd)
        {
            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                // Kiểm tra xem CCCD đã tồn tại trong cơ sở dữ liệu chưa
                string query = "SELECT COUNT(*) FROM NhanKhau WHERE CCCD = @CCCD";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CCCD", cccd);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        private bool IsPhoneNumberExists(string phoneNumber)
        {
            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                // Kiểm tra xem số điện thoại đã tồn tại trong cơ sở dữ liệu chưa
                string query = "SELECT COUNT(*) FROM NhanKhau WHERE SoDienThoai = @PhoneNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
        private void cccdkeypress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void sdtkeypress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        
    }
}
