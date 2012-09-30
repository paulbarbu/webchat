using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webchat.Database {
    /// <summary>
    /// The interface that defines what a database class should do
    /// </summary>
    public interface IDatabase {
        /// <summary>
        /// Add a user to the daatbase
        /// </summary>
        /// <param name="rooms">Which rooms the user joined</param>
        /// <param name="nick">The user's nickname</param>
        void AddUser(IEnumerable<string> rooms, string nick);

        /// <summary>
        /// Delete a user from the database in case he leaves
        /// </summary>
        /// <param name="rooms">The rooms the user left</param>
        /// <param name="nick">The user's nickname</param>
        void DelUser(IEnumerable<string> rooms, string nick);

        /// <summary>
        /// Delete a user from the global user list
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        void DelUserFromGlobalList(string nick);

        /// <summary>
        /// Get the users currently connected
        /// </summary>
        /// <returns>Returns a Dictionary&lt;string, HashSet&lt;string&gt;&gt;
        /// of rooms as keys and users as values for the HastSet&lt;string&gt;</returns>
        Dictionary<string, HashSet<string>> GetUsers();

        /// <summary>
        /// Get all the rooms currently populated
        /// </summary>
        /// <returns>Returns a List&lt;string&gt; of rooms</returns>
        List<string> GetRooms();

        /// <summary>
        /// Get all the rooms currently joined by a user
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        /// <returns>Returns a List&lt;string&gt; of rooms</returns>
        List<string> GetRooms(string nick);

        /// <summary>
        /// Get the rooms the user was connected to before the PING
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        /// <returns>Returns a List&lt;string&gt; of rooms</returns>
        List<string> GetBackupRooms(string nick);

        /// <summary>
        /// Do a backup of the database before sending a PING
        /// </summary>
        void Backup();

        /// <summary>
        /// Check if there are users connected
        /// </summary>
        /// <returns>Returns true if there is at least one user, else returns false</returns>
        bool IsPopulated();

        /// <summary>
        /// Check if a user is connected on at least a room
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        /// <returns>Return true if the user is connected, else false</returns>
        bool IsUser(string nick);
    }
}
