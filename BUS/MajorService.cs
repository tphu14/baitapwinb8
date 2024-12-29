using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyID)
        {
            MyDbContext context = new MyDbContext();
            return context.Majors.Where(p => p.FacultyID == facultyID).ToList();
        }
    }
}