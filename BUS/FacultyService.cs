using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
            using (var context = new MyDbContext())
            {
                return context.Faculties.ToList();
            }
        }

        public Faculty FindById(int facultyId)
        {
            MyDbContext context = new MyDbContext();
            return context.Faculties.FirstOrDefault(f => f.FacultyID == facultyId);
        }

        public void Insert(Faculty faculty)
        {
            MyDbContext context = new MyDbContext();
            context.Faculties.Add(faculty);
            context.SaveChanges();
        }

        public void Update(Faculty faculty)
        {
            MyDbContext context = new MyDbContext();
            var existingFaculty = context.Faculties.FirstOrDefault(f => f.FacultyID == faculty.FacultyID);
            if (existingFaculty != null)
            {
                existingFaculty.FacultyName = faculty.FacultyName;
                context.SaveChanges();
            }
        }

        public void Delete(int facultyId)
        {
            MyDbContext context = new MyDbContext();
            var faculty = context.Faculties.FirstOrDefault(f => f.FacultyID == facultyId);
            if (faculty != null)
            {
                context.Faculties.Remove(faculty);
                context.SaveChanges();
            }
        }
        public List<Major> GetAllMajors()
        {
            using (var context = new MyDbContext())
            {
                return context.Majors.ToList();
            }
        }

        public List<Major> GetMajorsByFacultyId(int facultyId)
        {
            using (var context = new MyDbContext())
            {
                return context.Majors
                             .Where(m => m.FacultyID == facultyId)
                             .ToList();
            }
        }
    }
}