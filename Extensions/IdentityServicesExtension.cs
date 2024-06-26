using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class IdentityServicesExtension
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            //injects the service to DI container, using JWTbearer sets the default auth scheme to JWT based authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
    {
        //allows us to set parameters for validating the tokens recieved by app
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //this property indicated the issuer key needs to be validated , otherwise anyone with any JWT token can authenticate itslf
            ValidateIssuerSigningKey = true,
            //IssuerSigningKey : this property sets the key used to validate the token signature
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
        }
    }
}
