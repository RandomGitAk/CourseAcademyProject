using System.Collections;
using CourseAcademyProject.Models.Test;

namespace CourseAcademyProject.ViewModels.Test
{
    public class CompletedAndIncompletedTestsViewModel
    {
        public List<Models.Test.Test> CompletedTests {  get; set; }
        public List<Models.Test.Test> IncompletedTests { get; set; }
        public string TestName { get; set; }
        public int CourseId { get; set; }
    }
}
