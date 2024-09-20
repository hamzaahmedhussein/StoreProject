namespace Application.DTOs
{
    public interface CurrentCustomerProfileDto
    {
        public string DisplayName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
