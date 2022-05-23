using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelWebApp.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel() {Username = "Devel", Password ="Devel"},
            new UserModel() {Username = "Byron", Password ="David"}
        };
    }
}
