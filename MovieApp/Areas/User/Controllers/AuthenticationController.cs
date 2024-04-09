using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieApp.DTOs;
using MovieApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieApp.Areas.User.Controllers
{
    [Area("User")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [Route("Login")]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                TempData["error"] = "Already Logged In!!";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUser loginuser)
        {
            if (ModelState.IsValid)
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
            return View(loginuser);
            /*if(user!=null && await _userManager.CheckPasswordAsync(user,loginuser.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };
                var userRoles=await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role,role));
                }
                var jwtToken=GetToken(authClaims);
                ViewBag.Token = jwtToken;
                return View();
            }
            return Unauthorized();*/
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "Logged Out Successfully";
            return RedirectToAction("Index", "Home");
        }
        [Route("Register")]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                TempData["error"] = "Already Logged In!!";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [Route("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUser registeruser)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(registeruser.Email);
                if (userExist != null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(registeruser);
                }
                IdentityUser user = new()
                {
                    Email = registeruser.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registeruser.Username
                };
                var result = await _userManager.CreateAsync(user, registeruser.Password);
                if (!result.Succeeded)
                {   
                    TempData["error"] = result.Errors.FirstOrDefault()?.Description;
                    return View(registeruser);
                }
                await _userManager.AddToRoleAsync(user, "User");
                TempData["success"] = "User Registered Successfully!!";
                return RedirectToAction("Login");
            }
            return View(registeruser);
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
