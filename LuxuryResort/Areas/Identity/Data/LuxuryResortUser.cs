using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LuxuryResort.Areas.Identity.Data;

// Add profile data for application users by adding properties to the LuxuryResortUser class
public class LuxuryResortUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? FullName { get; set; }

}

