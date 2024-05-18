using System.ComponentModel.DataAnnotations;

namespace EventSchedulePro.Data.Class
{
    /// <summary>
    /// Schedule Event
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Id with AUTO_INCREMENT
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Event Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Date of Event
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        /// Course Name
        /// </summary>
        public string? Course { get; set; }

        /// <summary>
        /// Season Name
        /// </summary>
        public string? Season { get; set; }

        /// <summary>
        /// Group Id
        /// </summary>
        public string? GroupID { get; set; }

        /// <summary>
        /// Activity
        /// </summary>
        public string? Activity { get; set; }

        /// <summary>
        /// Staff
        /// </summary>
        public string? Staff { get; set; }

        /// <summary>
        /// Student Leader
        /// </summary>
        public string? StudentLeader { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// FromTime
        /// </summary>
        public string? FromTime { get; set; }

        /// <summary>
        /// ToTime
        /// </summary>
        public string? ToTime { get; set; }

        /// <summary>
        /// StaffNames
        /// </summary>
        public string? StaffNames { get; set; }

    }
}
