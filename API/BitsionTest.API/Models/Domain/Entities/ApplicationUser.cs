﻿using Microsoft.AspNetCore.Identity;

namespace BitsionTest.API.Models.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
