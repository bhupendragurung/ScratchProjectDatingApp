namespace ScratchProjectDatingApp.Entity
{
    public class AppUser
    {
        /// <summary>
        /// for key id
        /// </summary>
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
