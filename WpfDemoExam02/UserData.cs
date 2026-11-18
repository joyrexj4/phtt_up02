using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfDemoExam02.Models;

namespace WpfDemoExam02
{
    public static class UserData
    {
        private static int id;
        private static int roleId;

        private static string surname;
        private static string name;
        private static string patronymic;

        public static int Id
        {
            get { return id; }
            set { id = value; }
        }

        public static int RoleId
        {
            get { return roleId; }
            set { roleId = value; }
        }

        public static string Surname
        {
            get { return surname; }
            set { surname = value; }
        }
        public static string Name
        {
            get { return name; }
            set { name = value; }
        }
        public static string Patronymic
        {
            get { return patronymic; }
            set { patronymic = value; }
        }
        public static bool IsSignedIn()
        {
            if (id == 0 && roleId == 0)
                return false;
            else
                return true;
        }
        public static void ClearData()
        {
            id = 0;
            roleId = 0;
        }
        public static int GetRole()
        {
            return 0;
        }
    }
}
