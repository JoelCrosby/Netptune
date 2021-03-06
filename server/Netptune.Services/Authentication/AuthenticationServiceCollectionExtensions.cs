using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Netptune.Core.Authentication;
using Netptune.Core.Authorization;
using Netptune.Core.Entities;

namespace Netptune.Services.Authentication
{
    public class NetptuneAuthenticationOptions
    {
        [Required]
        public string Issuer { get; set; }

        [Required]
        public string Audience { get; set; }

        [Required]
        public string SecurityKey { get; set; }

        [Required]
        public string GitHubClientId { get; set; }

        [Required]
        public string GitHubSecret { get; set; }
    }

    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddNeptuneAuthentication(this IServiceCollection services, Action<NetptuneAuthenticationOptions> action)
        {
            var authenticationOptions = ConfigureServices(services, action);

            services.AddHttpContextAccessor();

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<DbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

                .AddCookie(options =>
                {
                    options.LoginPath = "/app/auth/login";
                    options.LogoutPath = "/app/auth/login";
                })

                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = authenticationOptions.Issuer,
                        ValidAudience = authenticationOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationOptions.SecurityKey)),
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            if (!context.Request.Headers.TryGetValue(NetptuneClaims.Workspace, out var workspace))
                            {
                                return Task.CompletedTask;
                            }

                            var claims = new[] {new Claim(NetptuneClaims.Workspace, workspace)};
                            var identity = new ClaimsIdentity(claims);

                            context.Principal?.AddIdentity(identity);

                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            var accessTokenNotEmpty = !string.IsNullOrEmpty(accessToken);
                            var isHubPath = path.StartsWithSegments("/hubs");

                            if (accessTokenNotEmpty && isHubPath)
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },
                    };
                })

                .AddGitHub(options =>
                {
                    options.ClientId = authenticationOptions.GitHubClientId;
                    options.ClientSecret = authenticationOptions.GitHubSecret;
                    options.CallbackPath = "/api/auth/github-callback";
                    options.Scope.Add("read:user");
                    options.Scope.Add("urn:github:name");
                    options.Scope.Add("user:email");
                    options.SaveTokens = true;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Events.OnCreatingTicket = async context =>
                    {
                        var token = context.AccessToken;
                        using var client = new WebClient();

                        client.Headers.Add("Authorization", $"token {token}");
                        client.Headers.Add("User-Agent", "Netptune API");

                        var stream = await client.OpenReadTaskAsync("https://api.github.com/user");
                        var gitHubUser = await JsonSerializer.DeserializeAsync<GithubUserResponse>(stream);

                        if (gitHubUser is null) return;

                        context.Identity.AddClaim(new Claim("Provider-Picture-Url", gitHubUser.AvatarUrl.ToString()));
                    };
                });

            services.AddTransient<INetptuneAuthService, NetptuneAuthService>();

            return services;
        }

        private static NetptuneAuthenticationOptions ConfigureServices(
            IServiceCollection services, Action<NetptuneAuthenticationOptions> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            var options = new NetptuneAuthenticationOptions();

            action(options);

            services.Configure(action);

            return options;
        }
    }
}
