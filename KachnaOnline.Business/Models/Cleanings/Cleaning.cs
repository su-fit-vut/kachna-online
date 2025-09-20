namespace KachnaOnline.Business.Models.Cleanings
{
    /// <summary>
    /// A model representing a cleaning.
    /// </summary>
    public class Cleaning : NewCleaning
    {
        /// <summary>
        /// An ID of the cleaning.
        /// </summary>
        public int Id { get; set; }
        public string AssignedUsersNames { get; }
    }
}
