using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Exceptions;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IPostRepository postRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;

        public CommentService(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
            this.accountRepository = accountRepository;
            this.mapper = mapper;
        }

        public async Task<ProjectionModels.Comment> CreateAsync(EntityModels.Comment comment)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));

            await ValidatePostIdAsync(comment.PostId);

            var createdId = await commentRepository.CreateAsync(comment);
            var commentTask = commentRepository.GetAsync(createdId);
            var accountsTask = accountRepository.GetAsync(comment.CreatorId);
            await Task.WhenAll(commentTask, accountsTask);

            var commentProjection = mapper.Map<ProjectionModels.Comment>(commentTask.Result);
            commentProjection.Creator = mapper.Map<ProjectionModels.Account>(accountsTask.Result.Single());
            return commentProjection;
        }

        public async Task<ICollection<ProjectionModels.Comment>> GetLatestByPostAsync(Guid postId, int? limit, long? previousCommentCursor)
        {
            await ValidatePostIdAsync(postId);

            var comments = await commentRepository.GetLatestByPostAsync(postId, limit, previousCommentCursor);
            var accounts = (await accountRepository.GetAsync(comments.Select(x => x.CreatorId).Distinct().ToArray()))
                .ToDictionary(x => x.Id);
            return comments.Select(x =>
            {
                var comment = mapper.Map<ProjectionModels.Comment>(x);
                comment.Creator = mapper.Map<ProjectionModels.Account>(accounts[x.CreatorId]);
                return comment;
            }).ToList();
        }

        private async Task ValidatePostIdAsync(Guid postId)
        {
            var existingPost = await postRepository.GetAsync(postId);
            if (existingPost == null)
                throw new NotFoundException("Post was not found.");
        }
    }
}