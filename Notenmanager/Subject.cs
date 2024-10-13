using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Notenmanager
{
    public class Subject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } 
        public string Name { get; set; }
        public int YearId { get; set; } 
        public string YearName { get; set; }
    }
}
