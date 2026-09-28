using UserApi.DB.Models;

namespace UserApi.Models
{
    public class UserResponse
    {
        public UserResponse(int id, string name, int age, string city, string state, string pincode)
        {
            Id = id;
            Name = name;
            Age = age;
            City = city;
            State = state;
            Pincode = pincode;
        }
        public static UserResponse From(User u) => new(u.Id, u.Name, u.Age, u.City, u.State, u.Pincode);

        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string City { get; }
        public string State { get; }
        public string Pincode { get; }
    }
}
