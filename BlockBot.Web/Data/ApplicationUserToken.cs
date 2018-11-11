﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlockBot.Web.Data
{
    public class ApplicationUserToken : IdentityUserToken<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
