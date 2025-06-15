// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace WebApplication3.Controllers;
//
// [ApiController]
// [Route("[controller]")]
// public class TestController : ControllerBase
// {
//
//     private readonly ILogger<TestController> _logger;
//
//     public TestController(ILogger<TestController> logger)
//     {
//         _logger = logger;
//     }
//
//     /*
//     [Route("/normal")]
//     [HttpGet]
//     public ResultDTO Get()
//     {
//         return new ResultDTO("Normal Route Accessed");
//     }
//
//
//     [HttpGet]
//     [Authorize]
//     [Route("/protected")]
//     public ResultDTO GetProtected()
//     {
//         return new ResultDTO("Protected Route Accessed");
//     }
//
//     [HttpGet]
//     [Authorize(Policy = "Admin")]
//     [Route("/admin")]
//     public ResultDTO GetAdmin()
//     {
//         return new ResultDTO("Admin Route Accessed");
//     }
//     */
// }