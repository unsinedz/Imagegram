using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Extensions;
using Imagegram.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiModels = Imagegram.Api.Models.Api;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository postRepository;
        private readonly IMapper mapper;

        public PostsController(IPostRepository postRepository, IMapper mapper)
        {
            this.postRepository = postRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ApiModels.Post>> PostAsync([Required, FromBody] ApiModels.PostInput postInput)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO: do smth with postInput.Image
            var post = await postRepository.CreateAsync(mapper.Map<EntityModels.Post>(postInput));
            post.CreatorId = User.GetId();
            return mapper.Map<ApiModels.Post>(post);
        }
    }
}