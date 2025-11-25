using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project1.Models;

namespace Project1.Classes
{
    public static class DB
    {
        public static UserManagerDBEntities Context = new UserManagerDBEntities();
    }
}