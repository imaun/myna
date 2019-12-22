using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Myna.Web.Core.Configuration {
    public static class ServiceCollectionExtensions {

        public static void AddMvcServices(this IServiceCollection services) {
            services.AddMvcCore()
                .AddAuthorization()
                .AddFormatterMappings();
        }


        public static void AddJwtAuthentication(this IServiceCollection services) {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                var secretkey = Encoding.UTF8.GetBytes("secretkey");
                var encryptionkey = Encoding.UTF8.GetBytes("encryptkey");

                var validationParameters = new TokenValidationParameters {
                    ClockSkew = TimeSpan.Zero, // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = "filan",

                    ValidateIssuer = true, //default : false
                    ValidIssuer = "issuer",

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents {
                    OnAuthenticationFailed = context => {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("Authentication failed.", context.Exception);

                        //if (context.Exception != null)
                        //    throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context => {
                        //var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                        //var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() != true)
                            context.Fail("This token has no claims.");

                        //var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        //if (!securityStamp.HasValue())
                        //    context.Fail("This token has no secuirty stamp");

                        //Find user and token from database and perform your custom validation
                        //var userId = claimsIdentity.GetUserId<int>();
                        //var user = await userRepository.GetByIdAsync(context.HttpContext.RequestAborted, userId);

                        //if (user.SecurityStamp != Guid.Parse(securityStamp))
                        //    context.Fail("Token secuirty stamp is not valid.");

                        //var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                        //if (validatedUser == null)
                        //    context.Fail("Token secuirty stamp is not valid.");

                        //if (!user.IsActive)
                        //    context.Fail("User is not active.");

                        //await userRepository.UpdateLastLoginDateAsync(user, context.HttpContext.RequestAborted);
                    },
                    //OnChallenge = context => {
                    //    //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                    //    //logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);

                    //    //if (context.AuthenticateFailure != null)
                    //    //    throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                    //    //throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);

                    //    //return Task.CompletedTask;
                    //}
                };
            });
        }


        public static void AddCustomApiVersions(this IServiceCollection services) {
            services.AddApiVersioning(_ => {
                _.AssumeDefaultVersionWhenUnspecified = true;
                _.DefaultApiVersion = new ApiVersion(1, 0);
                _.ApiVersionReader = new UrlSegmentApiVersionReader();
            }); 
        }

    }
}
