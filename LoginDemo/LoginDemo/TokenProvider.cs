using LoginDemo.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace LoginDemo
{
    public class TokenProvider
    {
        public string LoginUser(string UserID, string Password)
        {
            //Получаем юзера, чтобы убедиться есть ли он в нашей БД
            var user = UserList.SingleOrDefault(x => x.USERID == UserID);

            
            if (user == null)
                return null;

           //Если пользователь существует в БД, проверяем совпадение паролей
            if (Password == user.PASSWORD)
            {
                //Authentication successful
                //такой же Защитный ключ как и в sturtUp
                var key = Encoding.ASCII.GetBytes("IlnurNagibator911");
                //Создание токена
                var JWToken = new JwtSecurityToken(
                    issuer: "http://localhost:45092/",
                    audience: "http://localhost:45092/",
                    //получаем все Claimы пользователя(права доступа)
                    claims: GetUserClaims(user),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                    //HS256 для шифрования самого токена
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                );
                
                var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                return token;
            }
            else
            {
                return null;
            }
        }

        //Использование жестко закодированного списка сбора данных в качестве хранилища данных для демонстрационных целей
        public List<User> UserList = new List<User>
        {
            new User { USERID = "ilnur2500@email.com", PASSWORD = "test", EMAILID = "ilnur2500@email.com", FIRST_NAME = "Ilnur", LAST_NAME = "Mukhametov", PHONE = "356-735-2748", ACCESS_LEVEL = "Director"},
            new User { USERID = "Ilshat12@email.com", PASSWORD = "test", FIRST_NAME = "Ilshat", LAST_NAME = "Mukhamediarov", EMAILID = "Ilshat12@email.com", PHONE = "567-479-8537", ACCESS_LEVEL = "Supervisor" },
            new User { USERID = "Bulatsa@email.com", PASSWORD = "test", FIRST_NAME = "Bulat", LAST_NAME = "Muradimov", EMAILID = "Bulatsa@email.com", PHONE = "599-306-6010", ACCESS_LEVEL = "Analyst" },
            new User { USERID = "Ildarwar@email.com", PASSWORD = "test", FIRST_NAME = "Ildar", LAST_NAME = "Zalalyev", EMAILID = "Ildarwar@email.com", PHONE = "764-460-8610", ACCESS_LEVEL = "Analyst"}
        };

        
        private IEnumerable<Claim> GetUserClaims(User user)
        {
            IEnumerable<Claim> claims = new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.FIRST_NAME + " " + user.LAST_NAME),
                    new Claim("USERID", user.USERID),
                    
                    new Claim("EMAILID", user.EMAILID),
                    new Claim("PHONE", user.PHONE),
                    new Claim("ACCESS_LEVEL", user.ACCESS_LEVEL.ToUpper()),
                    };
            return claims;
        }
    }
}
