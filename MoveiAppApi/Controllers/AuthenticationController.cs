using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoveiAppApi.ApiResponses;
using MovieApp.DTOs;
using MovieApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieAppApi.Areas.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginUser loginuser)
        {
            /* if (ModelState.IsValid)
             {
                 var user = await _userManager.FindByEmailAsync(loginuser.Email);
                 if (user == null)
                 {
                     ModelState.AddModelError("Email", "Email doesn't exists.");
                     return View(loginuser);
                 }
                 var result = await _signInManager.PasswordSignInAsync(user, loginuser.Password, loginuser.RememberMe, lockoutOnFailure: false);
                 if (!result.Succeeded)
                 {
                     ModelState.AddModelError("Password", "Incorrect Password!");
                     return View(loginuser);
                 }
                 TempData["success"] = "Logged In Successfully";
                 if(User.IsInRole("Admin"))
                 {
                     return RedirectToAction("Index", "Home", new {area="Admin"});
                 }
                 return RedirectToAction("Index", "Home");
             }
             return View(loginuser);*/
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginuser.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, loginuser.Password))
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email,user.Email),
                        new Claim(ClaimTypes.Name,user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    };
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var jwtToken = GetToken(authClaims);
                    return Ok(jwtToken);
                }
                return Unauthorized();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "Logged Out Successfully";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterUser registeruser)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(registeruser.Email);
                ResponseMessage response = new ResponseMessage();
                if (userExist != null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return BadRequest(ModelState);
                }
                ApplicationUser user = new()
                {
                    Email = registeruser.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registeruser.Username
                };
                var result = await _userManager.CreateAsync(user, registeruser.Password);
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = result.Errors.FirstOrDefault()?.Description;
                    return BadRequest(response);
                }
                await _userManager.AddToRoleAsync(user, "User");
                response.Success = true;
                response.Message = "User Registered Successfully!!";
                return Ok(response);
            }
            return BadRequest(ModelState);
        }

        private string GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: authClaims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
             );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    }
}
