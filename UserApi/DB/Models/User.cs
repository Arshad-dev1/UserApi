namespace UserApi.DB.Models
{
    public class User
    {
        public string Name { get;  set; }
        public int Age { get;  set; }
        public string City { get;  set; }
        public string State { get;  set; }
        public string Pincode { get;  set; }
        public int Id { get; internal set; }
    }
}
