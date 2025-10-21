namespace Bagchi_Admin_Backend.Models
{
    public class shaydtos
    {
    }

    public class ShayariDto
    {
        public int ShayariId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string ShayariText { get; set; }
        public string Author { get; set; }
        public int LikesCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }


    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        
    }

}
