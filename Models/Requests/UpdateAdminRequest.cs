namespace DopamineDetoxAPI.Models.Requests
{
    public class UpdateAdminRequest
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
