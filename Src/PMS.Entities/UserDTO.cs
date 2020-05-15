namespace PMS.Entities
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PictureName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public string PswSalt { get; set; }
    }
}