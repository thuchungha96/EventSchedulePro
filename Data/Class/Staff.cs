using System.ComponentModel.DataAnnotations;

namespace EventSchedulePro.Data.Class
{
    /// <summary>
    /// Staff Information
    /// </summary>
    public class Staff
    {
        /// <summary>
        /// Id with AUTO_INCREMENT
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Group single ID
        /// </summary>
        public int? GroupID { get; set; }

        /// <summary>
        /// Staff Role
        /// </summary>
        public string? RoleUser { get; set; }

        /// <summary>
        /// User 
        /// </summary>
        [Required]
        public string PasswordHash { get; set; }

        /// <summary>
        /// List Group Id
        /// </summary>
        public string? GroupIds { get; set; }

        /// <summary>
        /// List Group Names
        /// </summary>
        public string? GroupNames { get; set; }


    }
}
