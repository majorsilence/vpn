﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibLogic.Accounts
{
    public class CreateAccountInfo
    {

        public string BetaKey { get; set; }
        public string Email { get; set; }
        public string EmailConfirm { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

    }
}