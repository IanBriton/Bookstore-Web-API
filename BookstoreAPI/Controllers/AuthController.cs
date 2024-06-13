using BookstoreAPI.Dto;
using BookstoreAPI.Dto.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        //Route for seeding roles to the DB.
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            bool isUserExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
            bool isAdminExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isOwnerExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);

            if (isUserExists && isAdminExists && isOwnerExists)
                return Ok("Role Already Exists");

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

            return Ok("Role Seeding to the Database is successful.");
        }

        //Route for Registering users to the Database
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var isUserExists = await _userManager.FindByNameAsync(registerDto.UserName);
            if (isUserExists != null)
                return BadRequest("User already exists");

            IdentityUser newUser = new IdentityUser()
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createdUserResults = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!createdUserResults.Succeeded)
            {
                var errorString = "User creation failed because: ";
                foreach (var error in createdUserResults.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return BadRequest(errorString);
            }

            //Default Role to all the users
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);
            return Ok("User successfully created...");
        }

        //Route for logging in the user
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null) return Unauthorized("Incorrect credentials");

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect)
                return BadRequest("Incorrect Credentials");

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateNewJwtWebToken(authClaims);

            return Ok(token);
        }

        private string GenerateNewJwtWebToken(List<Claim> claims)
        {
            SymmetricSecurityKey authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

        //Route for adding roles to users
        [HttpPost]
        [Route("make-admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MakeAdmin([FromBody] UpdatedPermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)

                return BadRequest("Invalid User name!!!");
            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return Ok("User is now an Admin");

        }

        //Route for making user owner
        [HttpPost]
        [Route("make-owner")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MakeOwner([FromBody] UpdatedPermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
                return BadRequest("Invalid User name!!!");

            await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

            return Ok("User is now an Owner");

        }

        //Route for Revoking User Roles
        [HttpPost]
        [Route("revoke-roles")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RevokeRole([FromBody] UpdatedPermissionDto updatedPermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatedPermissionDto.UserName);
            if (user is null)
                return BadRequest("Invalid User name");

            var results = await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.ADMIN);

            if (!results.Succeeded)
                return StatusCode(500, "Failed to revoke Role");
            return Ok("Role revoking was successful. ");
        }
    }
}
