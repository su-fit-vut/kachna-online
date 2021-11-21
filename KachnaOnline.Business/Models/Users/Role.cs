// Role.cs
// Author: František Nečas

namespace KachnaOnline.Business.Models.Users
{
    /// <summary>
    /// A model representing a single user role.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Unique ID of the role.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Textual name of the role.
        /// </summary>
        public string Name { get; set; }
    }
}
