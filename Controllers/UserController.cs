using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Models.Requests;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly AppSettings _appSettings;
        private readonly ITopicService _topicService;
        private readonly ISubTopicService _subTopicService;
        private readonly IChannelService _channelService;
        private readonly INoteService _noteService;
        private readonly ITopSearchResultService _topSearchResultService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ILoggingService _loggingService;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IJwtService jwtService,IOptions<AppSettings> appSettings
, ITopicService topicService, ISubTopicService subTopicService, IChannelService channelService, INoteService noteService, ITopSearchResultService topSearchResultService, IMapper mapper, INotificationService notificationService, ILoggingService loggingService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _appSettings = appSettings.Value;
            _topicService = topicService;
            _subTopicService = subTopicService;
            _channelService = channelService;
            _noteService = noteService;
            _topSearchResultService = topSearchResultService;
            _mapper = mapper;
            _notificationService = notificationService;
            _loggingService = loggingService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto model, CancellationToken cancellationToken = default)
        {
            return Ok(model);
            //try
            //{
            //    if (!ModelState.IsValid || model == null)
            //    {
            //        return BadRequest(ModelState);
            //    }

            //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
            //    var result = await _userManager.CreateAsync(user, model.Password ?? "");
            //    if (result.Succeeded)
            //    {
            //        if (model.IsAdmin)
            //        {
            //            await _userManager.AddToRoleAsync(user, "Admin");
            //        }

            //        return Ok(new { message = "User registered successfully" });
            //    }
            //    return BadRequest(result.Errors);
            //}
            //catch (Exception e)
            //{
            //    await _loggingService.LogErrorAsync("500", e.Message, e.StackTrace);
            //    return Ok(new { message = e.Message });
            //}
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(model?.Email) || String.IsNullOrEmpty(model?.Password))
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (user != null)
                {
                    var token = _jwtService.GenerateJwtToken(user);
                    var userDto = user.ToApplicationUserDto(_mapper);
                    return Ok(new { Token = token, Message = "Login successful", User = userDto });
                }
            }
            return Unauthorized(new { User = (ApplicationUserDto)null, Response = "Unauthorized", ErrorCode = "401", Message = "Invalid username or password" });
        }

        [Authorize]
        [HttpGet("details/{username}")]
        public async Task<ActionResult<ApplicationUserDto>> GetUserDetails(string username, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Check if the current user has permission to view this user's details
            // This is a basic check. You might want to implement more sophisticated authorization logic
            if (User.Identity?.Name != username && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var userDto = user.ToApplicationUserDto(_mapper);

            return Ok(userDto);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto model, CancellationToken cancellationToken = default)
        {
            // Verify the Google token
            if (String.IsNullOrEmpty(model?.IdToken))
            {
                return BadRequest(new { message = "Missing Google Id Token" });
            }


            var payload = await _jwtService.VerifyGoogleToken(model.IdToken);
            if (payload == null)
            {
                return Unauthorized(new { User = (ApplicationUserDto)null, Response = "Unauthorized", ErrorCode = "401", Message = "Invalid Google email" });
            }

            // validate the payload.Email matches a custom property in the ApplicationUser (AspNetUsers table). This emails is the user's Google email
            // If the email is not found, return Unauthorized

            // Check if the user exists, if not, create a new user
            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = payload.Email, Email = payload.Email };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return Unauthorized(new { User = (ApplicationUserDto)null, Response = "Unauthorized", ErrorCode = "401", Message = "Invalid Google email" });
                }
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            // Generate JWT token
            var token = _jwtService.GenerateJwtToken(user);
            var userDto = user.ToApplicationUserDto(_mapper);
            return Ok(new { Token = token, Message = "Google login successful", User = userDto });
        }

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailRequest model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || model == null)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null || user.UserName != model.UserName || user.Email != model.OldEmail)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Email = model.NewEmail;
            user.NormalizedEmail = model.NewEmail;
            user.UserName = model.NewEmail;
            user.NormalizedUserName = model.NewEmail;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var userDto = user.ToApplicationUserDto(_mapper);
                return Ok(new { message = "Email changed successfully", User = userDto });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("is-admin")]
        public async Task<IActionResult> IsAdmin(UpdateAdminRequest model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || model == null || (String.IsNullOrEmpty(model.UserId) && String.IsNullOrEmpty(model.UserName)))
            {
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrEmpty(model.UserId))
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                if (model.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }

                var userDto = user.ToApplicationUserDto(_mapper);
                return Ok(new { message = "Profile updated successfully", User = userDto });
            }
            
            if(!String.IsNullOrEmpty(model.UserName))
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                if (model.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }

                var userDto = user.ToApplicationUserDto(_mapper);

                return Ok(new { message = "Profile updated successfully", User = userDto });
            }
            return BadRequest(ModelState);
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || (String.IsNullOrEmpty(model?.UserId)))
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null || ((!String.IsNullOrEmpty(model.UserName) && (user.UserName != model.UserName))))
            {
                return NotFound(new { message = "User not found" });
            }

            if(!String.IsNullOrEmpty(model.NewUserName))
                user.UserName = model.NewUserName;

            if(!String.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!String.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                //Use other is-admin method to update admin status
                //if (model.IsAdmin)
                //{
                //    await _userManager.AddToRoleAsync(user, "Admin");
                //}
                //else
                //{
                //    await _userManager.RemoveFromRoleAsync(user, "Admin");
                //}

                var userDto = user.ToApplicationUserDto(_mapper);

                return Ok(new { message = "Profile updated successfully", User = userDto });
            }

            return BadRequest(result.Errors);
        }



        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || model == null)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null || user.UserName != model.UserName)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { message = "Password changed successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordRequest model, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid || String.IsNullOrEmpty(model?.Email))
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return Ok(new { message = "If the email is registered, a password reset link will be sent." });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                if (!String.IsNullOrEmpty(token))
                {
                    string baseUrl = _appSettings?.DopamineDetoxUrl ?? "http://localhost:4200";
                    bool isSuccess = await _notificationService.SendPasswordResetEmail(user, token, baseUrl, cancellationToken);
                }

                return Ok(new { message = "If the email is registered, a password reset link will be sent." });
            }
            catch(Exception e)
            {
                return Ok(new {message = e.Message});
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(model?.Email) || String.IsNullOrEmpty(model?.Token) || String.IsNullOrEmpty(model?.NewPassword))
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest(new { message = "Invalid request" });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { message = "Password has been reset successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("delete/{username}")]
        public async Task<IActionResult> DeleteUser(string username, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Start a transaction
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Remove related entities
                var userId = user.Id;

                // Remove Topics
                await _topicService.DeleteTopicsByUserIdAsync(userId, cancellationToken);
                await _subTopicService.DeleteSubTopicsByUserIdAsync(userId, cancellationToken);
                await _channelService.DeleteChannelsByUserIdAsync(userId, cancellationToken);
                await _noteService.DeleteNotesByUserIdAsync(userId, cancellationToken);
                await _topSearchResultService.DeleteTopSearchResultsByUserIdAsync(userId, cancellationToken);

                // Remove the user
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Error deleting user");
                }

                // Commit the transaction if all operations succeed
                await transaction.CommitAsync(cancellationToken);

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                await transaction.RollbackAsync(cancellationToken);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }
    }

}