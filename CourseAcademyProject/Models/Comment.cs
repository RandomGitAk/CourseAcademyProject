using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content {  get; set; }
        public DateTime DateOfPublication { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }

        public int CourseId {  get; set; }
        public Course Course { get; set; }
    }
}
