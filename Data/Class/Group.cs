using System.ComponentModel.DataAnnotations;

namespace EventSchedulePro.Data.Class
{
    /// <summary>
    /// Group Event
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Id with AUTO_INCREMENT
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Group Name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Group Detail, not required
        /// </summary>
        public string? Detail { get; set; }
    }
}
