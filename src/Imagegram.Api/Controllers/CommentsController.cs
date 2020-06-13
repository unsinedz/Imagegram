using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Extensions;
using Imagegram.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiModels = Imagegram.Api.Models.Api;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository commentRepository;
        private readonly IMapper mapper;

        public CommentsController(ICommentRepository commentRepository, IMapper mapper)
        {
            this.commentRepository = commentRepository;
            this.mapper = mapper;
        }

        [HttpPost("posts/{postId:required}/[controller]")]
        public async Task<ActionResult<ApiModels.Comment>> PostAsync([FromBody, Required] ApiModels.CommentInput commentInput, Guid postId)
        {
            var comment = mapper.Map<EntityModels.Comment>(commentInput);
            comment.CreatorId = User.GetId();
            comment.PostId = postId;

            var createdId = await commentRepository.CreateAsync(comment);
            return mapper.Map<ApiModels.Comment>(await commentRepository.GetAsync(createdId));
        }
    }
}