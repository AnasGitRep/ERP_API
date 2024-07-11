using Erp.Base;
using Erp.Data;
using Erp.Dto;
using Erp.Helpers;
using Erp.IServices;
using Erp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Erp.Services
{
    public class AuthService : IAuthServices
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IConfiguration _config;
        public AuthService(ApplicationDbContext dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _config = configuration;
        }
        public async Task<ResponseModel<RegisterDto>> Register(RegisterDto dto)
        {
            ResponseModel<RegisterDto> response=new ResponseModel<RegisterDto>();

            try
            {
                if(dto != null) 
                {
                    if (await UserAlreadyExists(dto.Name))
                    {
                        response.Message = "User name already taken";
                        response.IsOk = false;
                        return response;
                    }


                    var HashKey = GetPasswordKeyHash(dto);

                    var user = new ApplicationUser();
                    user.Email = dto.Email;
                    user.PhoneNumber = dto.PhoneNumber;
                    user.Name=dto.Name;
                    user.Password = HashKey.Result.PasswordHash;
                    user.PasswordKey= HashKey.Result.PasswordKey;

                    await _dbcontext.Users.AddAsync(user);
                    await _dbcontext.SaveChangesAsync();

                    var UserImage = new UserImage()
                    {
                        ImageUrl = dto.ImageUrl,
                        UserId = user.Id
                    };

                    await _dbcontext.UserImages.AddAsync(UserImage);

                    var CheckAdminRole = await _dbcontext.SystemRolls.FirstOrDefaultAsync(x => x.Name!.Equals(Constant.Admin));
                    if (CheckAdminRole is null)
                    {
                        var CreateAdminRol = new SystemRolls();
                        CreateAdminRol.Name = Constant.Admin;
                        await _dbcontext.AddAsync(CreateAdminRol);
                        await _dbcontext.SaveChangesAsync();

                        var CreateUserRoll = new UserRolls();
                        CreateUserRoll.UserId = user.Id;
                        CreateUserRoll.RollId=CreateAdminRol.Id;
                        await _dbcontext.AddAsync(CreateUserRoll);
                        await _dbcontext.SaveChangesAsync();


                        response.IsOk = true;
                        response.Message = "User created";
                        return response;

                    }




                    var CheckUserRoll = await _dbcontext.SystemRolls.FirstOrDefaultAsync(x => x.Name!.Equals(Constant.User));
                    if (CheckUserRoll is null)
                    {
                        var CreateUserRol = new SystemRolls();
                        CreateUserRol.Name = Constant.User;
                        await _dbcontext.AddAsync(CreateUserRol);
                        await _dbcontext.SaveChangesAsync();

                        var CreateUserRoll = new UserRolls();
                        CreateUserRoll.UserId = user.Id;
                        CreateUserRoll.RollId = CreateUserRol.Id;
                        await _dbcontext.AddAsync(CreateUserRoll);
                        await _dbcontext.SaveChangesAsync();

                    }
                    else
                    {
                        var CreateUserRoll = new UserRolls
                        {
                            UserId = user.Id,
                            RollId = CheckUserRoll.Id
                        };
                        await _dbcontext.AddAsync(CreateUserRoll);
                        await _dbcontext.SaveChangesAsync();

                    }
                }
                await _dbcontext.SaveChangesAsync();
                response.IsOk = true;
                response.Message = "User created";
                return response;
            }
            catch (Exception ex) 
            {
                throw ex;
            }

        }


        public async Task<ResponseModel<LoginResponseDto>> Login(LoginDto dto)
        {
            ResponseModel<LoginResponseDto> response = new ResponseModel<LoginResponseDto>();

            try
            {
                var user = await _dbcontext.Users.Include(x=>x.UserImage).FirstOrDefaultAsync(x => x.Name == dto.Username);
                if(user == null)
                {
                    response.IsOk = false;
                    response.Message = "USER NOT FOUND";
                    return response;    

                }else if (!MatchPassword(dto.Password, user.Password, user.PasswordKey))
                {
                    response.IsOk = false;
                    response.Message = "Invalid userName or password";
                    return response;

                }
                else
                {
                    var GetUserRole = await FindUserRole(user.Id);
                    if (GetUserRole == null)
                    {
                        response.IsOk = false;
                        response.Message = "user roll not found";
                        return response;    
                    }

                    var GetUserRoleName = await FindRoleName(GetUserRole.RollId);
                    if (GetUserRoleName == null)
                    {
                        response.IsOk = false;
                        response.Message = "user roll not found";
                        return response;
                    }

                    
                    string JwtToken = GenerateToken(user, GetUserRoleName!.Name!);

                    var loginResponse = new LoginResponseDto();
                    
                    loginResponse.JwtToke = JwtToken;
                    
                   

                    response.IsOk= true;
                    response.Message = "login sucess";
                    response.Item = loginResponse;

                    return response;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<List<string>> GetUserPermissions(int userId)
        {
            var result=await _dbcontext.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.Permissions.Name)
                .ToListAsync();
            if(result.Count > 0)
            {
                return result;
            }

            return null;
        }
        public async Task<HashKey> GetPasswordKeyHash(RegisterDto User)
        {

            byte[] PasswordHash, PasswordKey;
            using (var hmac = new HMACSHA512())
            {
                PasswordKey = hmac.Key;
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(User.Password));
            }
            var HashKEY = new HashKey()
            {
                PasswordKey = PasswordKey,
                PasswordHash = PasswordHash
            };
            return HashKEY;
        }


        private bool MatchPassword(string passwordTxt, byte[] password, byte[] passwordKey)
        {
            byte[] PasswordHash, PasswordKey;
            using (var hmac = new HMACSHA512(passwordKey))
            {
                PasswordKey = hmac.Key;
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordTxt));
                for (int i = 0; i < PasswordHash.Length; i++)
                {
                    if (PasswordHash[i] != password[i])
                        return false;

                }
                return true;

            }
        }


    /*    JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                expires: DateTime.Now.AddMinutes(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)

            );

            return token;
        }*/

        private string GenerateToken(ApplicationUser appUser, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,appUser.Id.ToString()),
                new Claim(ClaimTypes.Name,appUser.Name!),
                new Claim(ClaimTypes.Email,appUser.Email!),
                new Claim(ClaimTypes.Role,role!),
                

            };
            if (!string.IsNullOrEmpty(appUser.UserImage?.ImageUrl))
            {
                userClaims.Add(new Claim(ClaimTypes.Uri, appUser.UserImage.ImageUrl));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:ValidIssuer"],
                audience: _config["Jwt:ValidAudience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> UserAlreadyExists(string UserName)
        {
            return await _dbcontext.Users.AnyAsync(x => x.Name == UserName);

        }




        

        private async Task<UserRolls> FindUserRole(int UserId) => await _dbcontext.UserRolls.FirstOrDefaultAsync(x => x.UserId == UserId);

        private async Task<SystemRolls> FindRoleName(int RoleId) => await _dbcontext.SystemRolls.FirstOrDefaultAsync(x => x.Id == RoleId);

        public async Task<bool> HasPermissionAsync(string permissionName,int userId)
        {
            var userPermissions = await _dbcontext.UserPermissions
                                         .Include(up => up.Permissions)
                                         .Where(up => up.UserId == userId)
                                         .ToListAsync();

            // Check if any of the user's permissions match the given permissionName
            return userPermissions.Any(up => up.Permissions.Name == permissionName);
        }
    }







}
