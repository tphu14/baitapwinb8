using BUS;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();



        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty());
            this.cbbFaculty.DataSource = listFacultys;
            this.cbbFaculty.DisplayMember = "FacultyName";
            this.cbbFaculty.ValueMember = "FacultyID";
        }
        private void FillMajorCombobox(List<Major> listMajors)
        {
            listMajors.Insert(0, new Major { MajorID = 0, Name = "" });
            cbbMajor.DataSource = listMajors;
            cbbMajor.DisplayMember = "Name";
            cbbMajor.ValueMember = "MajorID";
        }
        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dataGridView1.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dataGridView1.Rows[index].Cells[4].Value = item.Major.Name + "";

                // Hiển thị đường dẫn ảnh vào cột mới
                dataGridView1.Rows[index].Cells[5].Value = item.Avatar; // Cột thứ 6 (index 5) sẽ chứa đường dẫn ảnh

                ShowAvatar(item.Avatar); // Hiển thị avatar vào PictureBox
            }
        }

        private void ShowAvatar(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                pictureBox1.Image = null; // Nếu không có ảnh, đặt ảnh null
            }
            else
            {
                // Lấy đường dẫn thư mục gốc của ứng dụng
                string parentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Tạo đường dẫn đến thư mục Images và tên ảnh
                string imagePath = Path.Combine(parentDirectory, "Images", imageName);

                // Kiểm tra xem file ảnh có tồn tại không
                if (File.Exists(imagePath))
                {
                    pictureBox1.Image = Image.FromFile(imagePath); // Hiển thị ảnh lên PictureBox
                    pictureBox1.Refresh();
                }
                else
                {
                    MessageBox.Show("Ảnh không tồn tại tại: " + imagePath);
                }
            }
        }
        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

      
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dataGridView1);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                var listMajors = facultyService.GetAllMajors(); // Lấy danh sách chuyên ngành
                if (listMajors != null)
                    FillMajorCombobox(listMajors);
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
                dataGridView1.CellClick += dataGridView1_CellClick_1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu nhấp vào dòng hợp lệ (không phải header)
            if (e.RowIndex >= 0)
            {
                // Lấy thông tin từ các cột trong dòng đã chọn
                var studentID = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(); // Cột 0 là StudentID
                var fullName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(); // Cột 1 là FullName
                var facultyName = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString(); // Cột 2 là FacultyName
                var averageScore = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString(); // Cột 3 là AverageScore
                //var majorName = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString(); // Cột 4 là MajorName
                var avatar = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString(); // Cột 5 là Avatar

                // Gán các giá trị vào các ô nhập tương ứng
                txtID.Text = studentID;           // Gán StudentID vào TextBox txtID
                txtName.Text = fullName;          // Gán FullName vào TextBox txtName
                txtScore.Text = averageScore;    // Gán AverageScore vào TextBox txtDiemTB

                // Gán vào ComboBox cbbKhoa
                foreach (var item in cbbFaculty.Items)
                {
                    if (item is Faculty faculty && faculty.FacultyName == facultyName)
                    {
                        cbbFaculty.SelectedItem = item; // Chọn khoa đúng trong ComboBox
                        break;
                    }
                }

                // Gán vào ComboBox hoặc các TextBox khác nếu có
                //cbbMajor.Text = majorName;     // Nếu có TextBox cho MajorName
                ShowAvatar(avatar);              // Hiển thị ảnh vào PictureBox (nếu có)
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở hộp thoại để chọn ảnh
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                // Kiểm tra nếu người dùng chọn ảnh
                string imagePath = null;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lấy đường dẫn ảnh được chọn
                    imagePath = openFileDialog.FileName;

                    // Lưu ảnh vào thư mục Images
                    string fileName = Path.GetFileName(imagePath); // Lấy tên file ảnh
                    string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName); // Đường dẫn mới
                    File.Copy(imagePath, destinationPath, true); // Sao chép ảnh vào thư mục Images
                }

                // Tạo đối tượng student với thông tin từ giao diện
                var student = new Student
                {
                    StudentID = int.Parse(txtID.Text),
                    FullName = txtName.Text,
                    AverageScore = float.Parse(txtScore.Text),
                    FacultyID = (int)cbbFaculty.SelectedValue,
                    //MajorID = (int?)cbbMajor.SelectedValue,
                    Avatar = Path.GetFileName(imagePath) // Lưu tên file ảnh vào Avatar
                };

                // Gọi phương thức thêm sinh viên vào cơ sở dữ liệu
                studentService.Add(student); // Gọi phương thức thêm

                MessageBox.Show("Thêm sinh viên thành công!");

                // Làm mới DataGridView
                BindGrid(studentService.GetAll());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm sinh viên. Lỗi: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở hộp thoại để chọn ảnh
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                // Kiểm tra nếu người dùng chọn ảnh
                string imagePath = null;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lấy đường dẫn ảnh được chọn
                    imagePath = openFileDialog.FileName;

                    // Lưu ảnh vào thư mục Images
                    string fileName = Path.GetFileName(imagePath); // Lấy tên file ảnh
                    string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName); // Đường dẫn mới
                    File.Copy(imagePath, destinationPath, true); // Sao chép ảnh vào thư mục Images
                }

                // Tạo đối tượng student với thông tin từ giao diện
                var student = new Student
                {
                    StudentID = int.Parse(txtID.Text),
                    FullName = txtName.Text,
                    AverageScore = float.Parse(txtScore.Text),
                    FacultyID = (int)cbbFaculty.SelectedValue,
                    //MajorID = (int?)cbbMajor.SelectedValue,
                    Avatar = Path.GetFileName(imagePath) // Lưu tên file ảnh vào Avatar
                };

                // Gọi phương thức thêm sinh viên vào cơ sở dữ liệu
                studentService.Add(student); // Gọi phương thức thêm

                MessageBox.Show("Thêm sinh viên thành công!");

                // Làm mới DataGridView
                BindGrid(studentService.GetAll());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm sinh viên. Lỗi: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem đã chọn sinh viên để xóa chưa
                if (string.IsNullOrEmpty(txtID.Text))
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị hộp thoại xác nhận
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Thực hiện xóa sinh viên
                    int studentID = int.Parse(txtID.Text);
                    studentService.Delete(studentID);

                    // Làm mới DataGridView
                    BindGrid(studentService.GetAll());

                    // Xóa thông tin trên các controls
                    txtID.Text = "";
                    txtName.Text = "";
                    txtScore.Text = "";
                    cbbFaculty.SelectedIndex = 0;
                    //cbbMajor.SelectedIndex = 0;
                    pictureBox1.Image = null;

                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // Lấy danh sách sinh viên chưa đăng ký chuyên ngành
                var studentsWithoutMajor = studentService.GetStudentsWithoutMajor();

                // Tạo instance của form Register và truyền danh sách sinh viên
                Register registerForm = new Register(studentsWithoutMajor);
                registerForm.ShowDialog();

                // Sau khi form Register đóng, bỏ chọn checkbox
                checkBox1.Checked = false;
            }
        }
    }
}
