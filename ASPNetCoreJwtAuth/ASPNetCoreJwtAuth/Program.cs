using ASPNetCoreJwtAuth.Data;
using ASPNetCoreJwtAuth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ASPNetCoreJwtAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Đăng ký context cơ sở dữ liệu để tương tác với database SQL Server.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            //Đăng ký dịch vụ JWTAuthService để quản lý việc tạo và xác thực token JWT.
            builder.Services.AddScoped<JWTAuthService>();
            //Đăng ký dịch vụ SignInManager để quản lý việc đăng nhập và đăng xuất người dùng.
            builder.Services.AddScoped<SignInManager>();

            var jwtTokenConfig = builder.Configuration.GetSection("jwt").Get<JwtTokenConfig>();
            //Đăng ký đối tượng JwtTokenConfig chứa các cấu hình JWT dưới dạng singleton.
            builder.Services.AddSingleton(jwtTokenConfig);

            //Đăng ký hệ thống xác thực sử dụng JWT Bearer và thiết lập các tham số xác thực token.
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true; // Yêu cầu sử dụng HTTPS cho các endpoint liên quan đến xác thực.
                x.SaveToken = true; //Lưu trữ token đã xác thực trong cookie trình duyệt.
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //Kiểm tra xem token có được phát hành bởi issuer hợp lệ hay không.
                    ValidIssuer = jwtTokenConfig.Issuer, //Giá trị issuer hợp lệ được lấy từ cấu hình.
                    ValidateAudience = true, // Kiểm tra xem token có dành cho audience hợp lệ hay không.
                    ValidAudience = jwtTokenConfig.Audience, //Giá trị audience hợp lệ được lấy từ cấu hình.
                    ValidateIssuerSigningKey = true, //Kiểm tra xem token có được ký bởi khóa bí mật hợp lệ hay không.
                    RequireExpirationTime = false, //Không bắt buộc token phải có thời hạn hết hạn.
                    ValidateLifetime = true, //Kiểm tra xem token có còn trong thời hạn hiệu lực hay không.
                    ClockSkew = TimeSpan.Zero, //Không cho phép sai lệch thời gian giữa máy chủ và client.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)) //hóa bí mật được sử dụng để xác thực chữ ký token.
                };
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
