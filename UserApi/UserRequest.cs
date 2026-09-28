namespace UserApi
{
    public class UserRequest
    {
        public string Name { get; internal set; }
        public int Age { get; internal set; }
        public string City { get; internal set; }
        public string State { get; internal set; }
        public string Pincode { get; internal set; }
    }
}
