using CAMS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAMS.Models
{
    public class AccessViewModel
    {
        public List<UserAccess> Accesses
        {
            get
            {
                return GetAccesses();
            }

        }

        public List<Department> Departments
        {
            get
            {
                return GetDepartments();
            }
        }



        private UsersController _usersController;
        private User _user;

        public AccessViewModel(User user, UsersController controller)
        {
            _usersController = controller;
            _user = user;
        }

        private List<UserAccess> GetAccesses()
        {
            List<UserAccess> accesses = new List<UserAccess>();
            foreach (var userDep in _user.UserDepartments)
            {
                if (userDep.UserId != _user.UserId)
                {
                    accesses.Add(new UserAccess(userDep.User, userDep.Department, userDep.AccessType));
                }
            }

            return accesses;
        }
        private List<Department> GetDepartments()
        {
            List<Department> dep = new List<Department>();
            foreach (var userDep in _user.UserDepartments)
            {
                if (userDep.AccessType == AccessType.Full)
                    dep.Add(userDep.Department);
            }
            return dep;
        }
    }

   

    public class UserAccess
    {
        public User User;
        public AccessType AccessType;
        public Department Department;

        public UserAccess(User user, Department dep, AccessType accType)
        {
            User = user;
            Department = dep;
            AccessType = accType;
        }
    }
}