using Mango.Service.AuthAPI.Models;
using Mango.Service.AuthAPI.Models.Dto;
using Mango.Service.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Data;
using Microsoft.AspNetCore.Identity;

namespace Mango.Service.AuthAPI.Service
{
    public class AuthService: IAuthService
    {
        private readonly AppDbContext _db;
        //UserManager is used to manage user accounts, including creating, deleting, and updating user information. ApplicationUser is the custom user class that extends IdentityUser.
        public readonly UserManager<ApplicationUser> _userManager; 
        public readonly RoleManager<IdentityRole> _roleManager; //RoleManager<IdentityRole> handels the roles in the application by using inbuild function of Identity.
        public readonly IJwtTokenGenerator _jwtTokenGenerator; //this is used to generate JWT token for the user after login. This is injected in the constructor of this class.
        public AuthService(AppDbContext db, 
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, // these helper method injected automaticaly no need extra configuration.
            IJwtTokenGenerator jwtTokenGenerator) 
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if(user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult()) //_rol
                {
                    //Create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                //Assigned the role to the user
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u=> u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool isvalid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null|| isvalid == false) {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //if user  was found, Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(user);
            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };

            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password); //_userManager.CreateAsync automaticaly create the user in Db and hash the password.
                if(result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registrationRequestDto.Email);
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description; //result.Errors is a list of errors that occured while creating the user by identity.
                }

            }
            catch(Exception ex)
            {
               
            }
            return "Error Encountered";
        }

       
    }    
}
