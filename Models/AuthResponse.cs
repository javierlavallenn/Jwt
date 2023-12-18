namespace Application.Models
{
    public class AuthResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreateAt { get; set; }
        public string Token { get; set; }
    }
}
