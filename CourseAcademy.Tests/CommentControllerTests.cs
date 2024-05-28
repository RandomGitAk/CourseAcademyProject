using CourseAcademyProject.Controllers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CourseAcademy.Tests
{
    public class CommentControllerTests
    {
        [Fact]
        public void Comments_ReturnsViewResult()
        {
            // Arrange
            var mockComment = new Mock<IComment>();
            var commentList = new List<Comment>
            {
                new Comment { Id = 1, Content = "Comment 1", UserId = "1" },
                new Comment { Id = 2, Content = "Comment 2", UserId = "2" },
                new Comment { Id = 3, Content = "Comment 3", UserId = "3" }
            };
            var controller = new CommentController(mockComment.Object);

            var options = new QueryOptions();
            mockComment.Setup(c => c.GetAllCommentsWithCourse(options))
                          .Returns(new PagedList<Comment>(commentList.AsQueryable(), options));

            // Act
            var result = controller.Comments(options);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Comment>>(viewResult.Model);
            Assert.Equal(commentList, model);
        }

        [Fact]
        public async Task DeleteComment_ValidCommentId_ReturnsOkResult()
        {
            // Arrange
            var mockComment = new Mock<IComment>();
            var controller = new CommentController(mockComment.Object);
            var commentId = 1;
            var comment = new Comment { Id = commentId, Content = "Test comment" };
            mockComment.Setup(c => c.GetCommentByIdAsync(commentId)).ReturnsAsync(comment);
            mockComment.Setup(c => c.DeleteCommentAndReturnDeletsIdAsync(comment)).ReturnsAsync(new List<int> { commentId });

            // Act
            var result = await controller.DeleteComment(commentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedIds = Assert.IsType<List<int>>(okResult.Value);
            Assert.Single(deletedIds);
            Assert.Equal(commentId, deletedIds[0]);
        }

        [Fact]
        public async Task DeleteComment_InvalidCommentId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockComment = new Mock<IComment>();
            var controller = new CommentController(mockComment.Object);
            var commentId = 1;
            mockComment.Setup(c => c.GetCommentByIdAsync(commentId)).ReturnsAsync((Comment)null);

            // Act
            var result = await controller.DeleteComment(commentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
