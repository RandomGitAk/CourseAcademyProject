namespace CourseAcademyProject.Models.Pages
{
    public class QueryOptions
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string OrderPropertyName { get; set; }
        public bool DescendingOrder { get; set; }
        public string SearchPropertyName { get; set; }
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? DifficultyLevel { get; set; }
        public bool? Price { get; set; }
        public bool? DateOfPublication { get; set; }
    }
}
