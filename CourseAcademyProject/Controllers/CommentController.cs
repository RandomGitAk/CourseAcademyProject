using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseAcademyProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommentController : Controller
    {
        private readonly IComment _comments;
        public CommentController(IComment comments)
        {
            _comments = comments;
        }
        [Route("/panel/comments")]
        [HttpGet]
        public IActionResult Comments(QueryOptions options)
        {
            return View(_comments.GetAllCommentsWithCourse(options));
        }

        [Route("/panel/delete-comment")]
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var currentComment = await _comments.GetCommentByIdAsync(commentId);
            if (currentComment != null)
            {
              var idCommentsToDelete = await _comments.DeleteCommentAndReturnDeletsIdAsync(currentComment);
              return Ok(idCommentsToDelete);
            }
            return NotFound();
        }
    }
}
