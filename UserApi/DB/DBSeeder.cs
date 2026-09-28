using UserApi.DB.Models;

namespace UserApi.DB
{
    public class DBSeeder
    {
        public static void Seed(UserDbContext db)
        {
            if (db.Users.Any()) return;

            db.Users.AddRange(
                new User { Name = "Asha Rao", Age = 29, City = "Bengaluru", State = "Karnataka", Pincode = "560001" },
                new User { Name = "Rahul Mehta", Age = 34, City = "Mumbai", State = "Maharashtra", Pincode = "400001" },
                new User { Name = "Priya Nair", Age = 41, City = "Kochi", State = "Kerala", Pincode = "682001" });
            db.SaveChanges();
        }

    }
}
