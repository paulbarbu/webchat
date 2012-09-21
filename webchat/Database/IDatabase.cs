using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webchat.Database {
    public interface IDatabase {
        void AddUser(IEnumerable<string> rooms, string nick);
        void DelUser(IEnumerable<string> rooms, string nick);
        void DelUserFromGlobalList(string nick);
        Dictionary<string, HashSet<string>> GetUsers();
        List<string> GetRooms();
        List<string> GetBackupRooms(string nick);
        List<string> GetRooms(string nick);
        void Backup();
        bool IsPopulated();
        bool IsUser(string nick);
    }
}
