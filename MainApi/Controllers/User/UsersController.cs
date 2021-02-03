using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MainApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MainApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly JWTSettings _jwtsettings;
        private readonly IConfiguration configuration;
        private UserManager UsersManager;

        public UsersController(DatabaseContext context, IOptions<JWTSettings> jwtsettings, IConfiguration iConfig)
        {
            _context = context;
            _jwtsettings = jwtsettings.Value;
            configuration = iConfig;
            UsersManager = new UserManager(_context);
        }

        // GET: api/Users/5
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Users>>> GetUser()
        {
            List<Users> users = _context.Users.ToList();
            foreach (var user in users)
            {
                user.Password = "NaN";
            }
            return users;
            
        }


        //// GET: api/Users/5
        //[HttpGet("{id}")]
        //[Authorize]
        //public async Task<ActionResult<Users>> GetUser(int id)
        //{

        //    var Users = await _context.Users.FindAsync(id);
        //    Users.Password = string.Empty;
        //    if (Users == null)
        //    {
        //        return NotFound();
        //    }
        //    Users findedUsers = _context.Users.Where(User => User.Id == id).FirstOrDefault();
        //    Console.WriteLine("Users id: {0} loggedin!", findedUsers.Id);
        //    findedUsers.LastLogin = DateTime.Now;
        //    await _context.SaveChangesAsync();
        //    return Users;
        //}

        [HttpPost("UpdateProfile")]
        [Authorize]
        public async Task<ActionResult<Users>> UpdateProfile(int id)
        {

            var Users = await _context.Users.FindAsync(id);
            Users.Password = string.Empty;
            if (Users == null)
            {
                return NotFound();
            }
            Users findedUsers = _context.Users.Where(User => User.Id == id).FirstOrDefault();
            Console.WriteLine("Users id: {0} loggedin!", findedUsers.Id);
            findedUsers.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();
            return Users;
        }

        // POST: api/Userss  
        [HttpPost("Login")]
        public async Task<ActionResult<UsersWithToken>> Login([FromBody] LoginDetails loginDetails)
        {
            UsersWithToken UsersWithToken = null;
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                    return Unauthorized();

                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                Console.WriteLine(authenticationHeaderValue);
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");
                string emailAddress = credentials[0];
                string password = credentials[1];


                Users findedUsers = _context.Users.Where(Users => Users.Email == emailAddress).FirstOrDefault();

                if (findedUsers == null)
                {
                    return NotFound();
                }
                if (PasswordHelper.CheckPassword(password, findedUsers.Password))
                {
                    //generate refresh token
                    Tokens refreshToken = UsersManager.GenerateTokens();
                    findedUsers.Tokens.Add(refreshToken);
                    findedUsers.LastLogin = DateTime.Now;
                    await _context.SaveChangesAsync();
                    UsersWithToken = new UsersWithToken(findedUsers);
                    UsersWithToken.RefreshToken = refreshToken.Token;
                    UsersWithToken.AccessToken = UsersManager.GenerateAccessToken(findedUsers.Id, _jwtsettings);
                    await UsersManager.saveloginHistory(loginDetails, findedUsers);
                    //add Security
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return UsersWithToken;
        }
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<UsersWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            Users Users = UsersManager.GetUserFromAccessToken(refreshRequest.AccessToken, _jwtsettings);

            if (Users != null && UsersManager.ValidateTokens(Users, refreshRequest.RefreshToken))
            {
                UsersWithToken UsersWithToken = new UsersWithToken(Users);
                UsersWithToken.AccessToken = UsersManager.GenerateAccessToken(Users.Id, _jwtsettings);

                return UsersWithToken;
            }
            return null;
        }



        [HttpPost("Register")]
        public async Task<ActionResult<UsersWithToken>> Register([FromQuery] Users Users)
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                    return new UsersWithToken(new Users()) { Error = "No Authorization" };

                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                Console.WriteLine(authenticationHeaderValue);
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");
                string emailAddress = credentials[0];
                string password = credentials[1];
                Console.WriteLine("New Users: {0}", emailAddress);

                Users findedUsers = _context.Users.Where(User => User.Email == emailAddress).FirstOrDefault();
                if (findedUsers != null)
                {
                    return Conflict();
                }
                Users NewUsers = new Users();
                Users.Email = emailAddress;
                Users.Password = PasswordHelper.CreatePassword(password);
                _context.Users.Add(Users);
                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return Ok();
        }


    }
}