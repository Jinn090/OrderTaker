﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OrderTaker.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public User()
    {
    }

    public User(string userName) : base(userName)
    {
    }

    [PersonalData]
    public string FirstName { get; set; }
    [PersonalData]
    public string LastName { get; set; }

}

