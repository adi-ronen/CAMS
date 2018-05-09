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
                List<UserAccess> acc = GetAccesses();
                if (_byDepartment) return acc;
                else return acc.OrderBy(e => e.User.UserId).ToList();
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
        private bool _byDepartment;

        public AccessViewModel(User user, UsersController controller, bool byDepartment)
        {
            _usersController = controller;
            _user = user;
            _byDepartment = byDepartment;
        }

        private List<UserAccess> GetAccesses()
        {
            List<UserAccess> accesses = new List<UserAccess>();
            //return access only for the departments that the user have full access to
            foreach (var dep in Departments)
            {
                foreach (var userDep in dep.UserDepartments)
                {
                    if (userDep.UserId != _user.UserId)
                    {
                        accesses.Add(new UserAccess(userDep.User, userDep.Department, userDep.AccessType));
                    }
                }

            }
            return accesses;

        }
        private List<Department> GetDepartments()
        {
            List<Department> dep = new List<Department>();
            //return only departments with full access

            foreach (var userDep in _user.UserDepartments.Where(e => e.AccessType == AccessType.Full))
            {
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