using AutoMapper;
using GP.APIs.DTOs;
using GP.APIs.Errors;
using GP.Core.Entites.OrderAggregate;
using GP.Core.Entities;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using GP.Core.Specifications;
using GP.Service.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace GP.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IRepository<Review> _Repo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ReviewController(IRepository<Review> Repo, UserManager<AppUser> userManager, IMapper mapper)
        {
            _Repo = Repo;
           _userManager = userManager;
            _mapper = mapper;
        }



        //Get all DR Reviews

        [HttpGet("GetAllDrReview")]

        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsForDr(string id)
        {

            var reviews =  _Repo.Get(includeProps: new Expression<Func<Review, object>>[]
    {
        reviews => reviews.Doctor // Include the UserCartItems
    }, R => R.DoctorId == id,true);
            var MappedReviews= _mapper.Map<IEnumerable<Review>, IEnumerable< ReviewDto>>(reviews);
            return Ok(MappedReviews);

        }
        //GET Review by id
        [HttpGet("{id}")]

        public IActionResult GetReviewById(int id)
        {
            var review = _Repo.GetOne(null, R => R.ReviewId == id, true);
           

            if (review == null)
            {
                return NotFound(new ApiResponse(404));
            }
            var MappedReview= _mapper.Map<Review, ReviewDto>(review);
            return Ok(MappedReview);
        }




        //delete Review
        [HttpDelete("{id}")]

        public IActionResult DeleteReviewById(int id)
        {
            var review = _Repo.GetOne(null, R => R.ReviewId == id, true);
            if (review == null)
            {
                return NotFound(new ApiResponse(404));
            }
            _Repo.Delete(review);
            _Repo.Commit();
            return Ok();
        }
        
        //Add
        [HttpPost("AddReview")]
        public IActionResult AddReview(int rating,string comment,string drId)
        {
            // Add new Review

            if (ModelState.IsValid)
            {
                var review = new Review
                {
                    UserId = _userManager.GetUserId(User),
                   DoctorId = drId,
                   Rating = rating,
                   Comment = comment,
                };
                _Repo.Create(review);
                _Repo.Commit();
                return Ok();
            }

            return BadRequest(new ApiResponse(400));
        }


    }
}
