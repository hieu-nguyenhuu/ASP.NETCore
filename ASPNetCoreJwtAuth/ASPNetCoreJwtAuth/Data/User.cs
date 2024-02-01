using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreJwtAuth.Data
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.HasData(
                new User { Id = 1, Email = "test@gmail.com", Password = "1234" },
                new User { Id = 2, Email = "abc@gmail.con", Password = "1234" }
            );
        }
    }

}