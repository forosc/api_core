using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        [StringLength(50)]
        public string? name { get; set; }
        [StringLength(50)]
        public string? lastName { get; set; }
        [StringLength(20)]
        public string? phoneNumber { get; set; }
        [StringLength(500)]
        public string? avatarUrl { get; set; }
        public bool isActive { get; set; }
    }
}
