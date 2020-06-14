using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        private readonly ICommentService commentService;
        private readonly IMapper mapper;

        public CommentsController(ICommentService commentService, IMapper mapper)
        {
            this.commentService = commentService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Creates a comment to a post.
        /// </summary>
        /// <param name="commentInput">The comment input data.</param>
        /// <param name="postId">The post ID.</param>
        /// <returns>An instance of created comment.</returns>
        [HttpPost("posts/{postId:required}/[controller]")]
        public async Task<ApiModels.Comment> PostAsync([FromBody, Required] ApiModels.CommentInput commentInput, Guid postId)
        {
            var comment = mapper.Map<EntityModels.Comment>(commentInput);
            comment.CreatorId = User.GetId();
            comment.PostId = postId;

            var createdComment = await commentService.CreateAsync(comment);
            return mapper.Map<ApiModels.Comment>(createdComment);
        }

        /// <summary>
        /// Get list of comments of the specified post.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <param name="limit">The amount of comments to return.</param>
        /// <param name="previousCommentCursor">The cursor to the previous comment.</param>
        /// <returns>
        /// A list of comments, which belong to the post with ID specified in <paramref name="postId" />,
        /// that contains maximum <paramref name="limit" /> comments, that were created before the comment,
        /// whose <paramref name="previousCommentCursor" /> was specified.
        /// </returns>
        [HttpGet("posts/{postId:required}/[controller]")]
        public async Task<ICollection<ApiModels.Comment>> GetAsync(Guid postId, int? limit, long? previousCommentCursor)
        {
            var comments = await commentService.GetLatestByPostAsync(postId, limit, previousCommentCursor);
            return comments.Select(x => mapper.Map<ApiModels.Comment>(x)).ToList();
        }
    }
}