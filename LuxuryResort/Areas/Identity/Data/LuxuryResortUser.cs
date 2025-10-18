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

    [PersonalData]
    [Column(TypeName = "date")]
    public DateTime? DateOfBirth { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(500)")]
    public string? Address { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? Nationality { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string? PassportNumber { get; set; }

    [Column(TypeName = "nvarchar(100)")]
    public string? PreferredRoomType { get; set; }

    [Column(TypeName = "nvarchar(1000)")]
    public string? SpecialRequests { get; set; }

    public bool MarketingConsent { get; set; } = false;

    public bool NewsletterSubscription { get; set; } = false;
}

