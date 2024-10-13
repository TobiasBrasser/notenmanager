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
            _database.CreateTableAsync<GradeInfo>().Wait();
            _database.CreateTableAsync<Subject>().Wait();
        }

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
                .Where(g => g.SubjectName == subjectName)
                .ToListAsync();
        }

        public async Task<double> GetAverageGradeBySubjectAsync(string subjectName)
        {
            var grades = await GetGradesBySubjectAsync(subjectName);
            if (grades.Count == 0)
            {
                return 0; 
            }

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

            return totalWeight > 0 ? totalGrades / totalWeight : 0; 
        }

        public async Task<List<YearModel>> GetYearsAsync()
        {
            return await _database.Table<YearModel>().ToListAsync();
        }


        public async Task AddYearAsync(YearModel year)
        {
            await _database.InsertAsync(year);
        }

        public async Task DeleteYearAsync(YearModel year)
        {
            await _database.DeleteAsync(year);
        }

        public async Task<List<GradeInfo>> GetGradesBySubjectAndYearAsync(string subjectName, string yearName)
        {
            var grades = new List<GradeInfo>();

                grades = await _database.Table<GradeInfo>()
                    .Where(g => g.SubjectName == subjectName && g.YearName == yearName)
                    .ToListAsync();
            

            return grades;
        }

        public async Task DeleteSubjectsByYearAsync(string yearName)
        {
                await _database.ExecuteAsync("DELETE FROM Subject WHERE YearName = ?", yearName);
        }

        public async Task DeleteGradesByYearAsync(string yearName)
        {
                await _database.ExecuteAsync("DELETE FROM GradeInfo WHERE YearName = ?", yearName);
        }

        public async Task<int> GetYearIdByNameAsync(string yearName)
        {
                var year = await _database.Table<YearModel>()
                    .FirstOrDefaultAsync(y => y.Name == yearName);
                return year?.Id ?? 0;
        }

        public async Task<List<Subject>> GetSubjectsByYearIdAsync(int yearId)
        {
                return await _database.Table<Subject>()
                    .Where(s => s.YearId == yearId) 
                    .ToListAsync();
        }

        public async Task<bool> SubjectExistsAsync(string subjectName, int yearId)
        {
            var existingSubject = await _database.Table<Subject>()
                .FirstOrDefaultAsync(s => s.Name == subjectName && s.YearId == yearId);
            return existingSubject != null; 
        }

        public async Task<bool> GradeExistsAsync(string title, string subjectName, string yearName)
        {
            var existingGrade = await _database.Table<GradeInfo>()
                .FirstOrDefaultAsync(g => g.Title == title && g.SubjectName == subjectName && g.YearName == yearName);
            return existingGrade != null;
        }


    }
}
