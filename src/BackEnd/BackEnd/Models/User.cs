using Autofac;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BackEnd.Models
{
    /// <summary>
    /// User can add new plugs to his active plugs, remove plugs and more
    /// </summary>
    public class User 
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public virtual List<Plug> Plugs { get; set; }

        public User()
        {
            Plugs = new List<Plug>();
        }

        public User(string name, string pass)
        {
            using (ILifetimeScope scope = Program.Container.BeginLifetimeScope()) if (scope.Resolve<SmartSwitchDbContext>().Users.Any(x => x.Username == name)) throw new UsernameAlreadyInUseException();
            Username = name;
            Password = pass;
            Plugs = new List<Plug>();
        }

    }
}