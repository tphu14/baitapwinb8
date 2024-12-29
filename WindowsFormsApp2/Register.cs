using BUS;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Register : Form
    {
        private List<Student> students;
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();

        public Register(List<Student> studentList)
        {
            InitializeComponent();
            this.students = studentList;
            LoadData();
            
            // Thêm event handler cho sự kiện SelectedIndexChanged của cbbFal
            cbbFal.SelectedIndexChanged += cbbFal_SelectedIndexChanged;
            // Thêm event handler cho nút Save
            btnSave.Click += btnSave_Click;
        }

        private void cbbFal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbFal.SelectedValue != null && (int)cbbFal.SelectedValue != 0)
            {
                int facultyId = (int)cbbFal.SelectedValue;
                // Lấy danh sách chuyên ngành theo khoa
                var majors = facultyService.GetMajorsByFacultyId(facultyId);
                FillMajorCombobox(majors);
            }
            else
            {
                // Nếu không chọn khoa thì xóa dữ liệu combobox chuyên ngành
                cbbMajor.DataSource = null;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbbMajor.SelectedValue == null || (int)cbbMajor.SelectedValue == 0)
                {
                    MessageBox.Show("Vui lòng chọn chuyên ngành!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy MajorID đã chọn
                int majorId = (int)cbbMajor.SelectedValue;

                // Lưu thông tin cho các sinh viên được chọn
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Kiểm tra nếu checkbox được tích
                    if (row.Cells["Choose"].Value != null && 
                        (bool)row.Cells["Choose"].Value == true)
                    {
                        int studentId = Convert.ToInt32(row.Cells["StudentID"].Value);
                        
                        // Cập nhật chuyên ngành cho sinh viên
                        studentService.UpdateMajor(studentId, majorId);
                    }
                }

                MessageBox.Show("Cập nhật chuyên ngành thành công!", "Thông báo");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            // Load dữ liệu cho combobox Faculty
            var faculties = facultyService.GetAll();
            FillFacultyCombobox(faculties);

            // Hiển thị danh sách sinh viên
            BindGrid(students);
        }

        private void FillFacultyCombobox(List<Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty { FacultyID = 0, FacultyName = "-- Chọn khoa --" });
            this.cbbFal.DataSource = listFacultys;
            this.cbbFal.DisplayMember = "FacultyName";
            this.cbbFal.ValueMember = "FacultyID";
        }

        private void FillMajorCombobox(List<Major> listMajors)
        {
            listMajors.Insert(0, new Major { MajorID = 0, Name = "-- Chọn chuyên ngành --" });
            cbbMajor.DataSource = listMajors;
            cbbMajor.DisplayMember = "Name";
            cbbMajor.ValueMember = "MajorID";
        }

        private void BindGrid(List<Student> students)
        {
            dataGridView1.Rows.Clear();
            if (students != null)
            {
                foreach (var student in students)
                {
                    int index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[1].Value = student.StudentID;
                    dataGridView1.Rows[index].Cells[2].Value = student.FullName;
                    dataGridView1.Rows[index].Cells[3].Value = student.Faculty?.FacultyName;
                    dataGridView1.Rows[index].Cells[4].Value = student.Major?.Name;
                }
            }
        }
    }
}
