using AutoMapper;
using ICode.API.Services.Interfaces;
using ICode.Common;
using ICode.Data.Entity;
using ICode.Models.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ICode.API.Controllers
{
    [ApiController]
    [Route("comments")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Comment>, IEnumerable<CommentBase>>(_commentService.GetAll()));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            Comment comment = _commentService.FindByID(id);
            if (comment == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Comment does not exist."
                });
            }
            return Ok(_mapper.Map<Comment, CommentBase>(comment));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, CommentUpdateDTO entity)
        {
            Comment comment = _commentService.FindByID(id);
            if (comment == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Comment does not exist."
                });
            }
            if (User.FindFirstValue(Constant.ID) == comment.UserID || User.FindFirstValue(Constant.ROLE) == "Admin")
            {
                await _commentService.Update(id, entity);
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            Comment comment = _commentService.FindByID(id);
            if (comment == null)
            {
                return NotFound(new ErrorResponse
                {
                    error = "Resource not found.",
                    detail = "Comment does not exist."
                });
            }
            if (User.FindFirstValue(Constant.ID) == comment.UserID || User.FindFirstValue(Constant.ROLE) == "Admin")
            {
                await _commentService.Remove(id);
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }
    }
}
