using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Notenmanager
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            // Erstelle beide Tabellen für Fächer und Noten, falls sie nicht vorhanden sind
            _database.CreateTableAsync<GradeInfo>().Wait();
            _database.CreateTableAsync<Subject>().Wait();
        }

        // CRUD-Methoden für GradeInfo (Noten)

        public async Task<int> AddGradeAsync(GradeInfo grade)
        {
            return await _database.InsertAsync(grade);
        }

        public async Task<List<GradeInfo>> GetGradesAsync()
        {
            return await _database.Table<GradeInfo>().ToListAsync();
        }

        public async Task<int> UpdateGradeAsync(GradeInfo grade)
        {
            return await _database.UpdateAsync(grade);
        }

        public async Task<int> DeleteGradeAsync(GradeInfo grade)
        {
            return await _database.DeleteAsync(grade);
        }

        // CRUD-Methoden für Subject (Fächer)

        public async Task<int> AddSubjectAsync(Subject subject)
        {
            return await _database.InsertAsync(subject);
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            return await _database.Table<Subject>().ToListAsync();
        }

        public async Task<int> UpdateSubjectAsync(Subject subject)
        {
            return await _database.UpdateAsync(subject);
        }

        public async Task<int> DeleteSubjectAsync(Subject subject)
        {
            return await _database.DeleteAsync(subject);
        }

        public async Task<List<GradeInfo>> GetGradesBySubjectAsync(string subjectName)
        {
            return await _database.Table<GradeInfo>()
                .Where(g => g.SubjectName == subjectName) // Filtern nach Fach
                .ToListAsync();
        }

        public async Task<double> GetAverageGradeBySubjectAsync(string subjectName)
        {
            var grades = await GetGradesBySubjectAsync(subjectName);
            if (grades.Count == 0)
            {
                return 0; // Return 0 if no grades are found
            }

            // Calculate the average grade
            double totalGrades = 0;
            double totalWeight = 0;

            foreach (var grade in grades)
            {
                if (double.TryParse(grade.Grade, out double gradeValue) &&
                    double.TryParse(grade.Weight, out double weightValue))
                {
                    totalGrades += gradeValue * weightValue;
                    totalWeight += weightValue;
                }
            }

            return totalWeight > 0 ? totalGrades / totalWeight : 0; // Avoid division by zero
        }

    }
}
