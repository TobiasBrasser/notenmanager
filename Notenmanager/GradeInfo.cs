using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notenmanager
{
    public class GradeInfo
    {
        public string Title { get; set; }
        public string Grade { get; set; }
        public string Weight { get; set; }

        public GradeInfo(string title, string grade, string weight)
        {
            Title = title;
            Grade = grade;
            Weight = weight;
        }
    }

}
