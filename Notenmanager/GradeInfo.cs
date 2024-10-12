using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Notenmanager; 


namespace Notenmanager
{
    public class GradeInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Grade { get; set; }
        public string Weight { get; set; }
        public string SubjectName { get; set; }
    }

}
