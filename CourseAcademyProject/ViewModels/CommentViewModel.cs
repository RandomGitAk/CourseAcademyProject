namespace CourseAcademyProject.ViewModels
{
    public class CommentViewModel
    {
        public string Content { get; set; }
        public string UserName {  get; set; }
        public int CourseId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
