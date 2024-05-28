namespace CourseAcademyProject.Models.Email
{
    public class News
    {
        public int Id { get; set; }
        public string Title {  get; set; }
        public string BodyText {  get; set; }
        public DateTime PublicationDate { get; set; }
        public string UserId {  get; set; }
        public User User {  get; set; }
    }
}
