using InvestorCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestorCenter.Areas.Identity.Data;

// Add profile data for application users by adding properties to the InvestorCenterUser class
public class InvestorCenterUser : IdentityUser
{
    public decimal Balance { get; set; }
}

