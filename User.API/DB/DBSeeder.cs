using UserApi.DB.Models;

namespace UserApi.DB
{
    public class DBSeeder
    {
        public static void Seed(UserDbContext db)
        {
            if (db.Users.Any()) return;

            db.Users.AddRange(
                new User { Name = "AAA AAAA", Age = 29, City = "Melbourne", State = "VIC", Pincode = "3000" },
                new User { Name = "Test User1", Age = 34, City = "Melbourne", State = "VIC", Pincode = "3001" },
                new User { Name = "Test User2", Age = 41, City = "Sydney", State = "NSW", Pincode = "2031" });
            db.SaveChanges();
        }

    }
}
