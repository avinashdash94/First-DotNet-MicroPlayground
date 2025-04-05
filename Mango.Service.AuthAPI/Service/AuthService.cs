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
        public AuthService(AppDbContext db, 
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) // these helper method injected automaticaly no need extra configuration.
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
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
