using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerWeb.DTO
{
    public class ForgotPasswordDto
    {
        public string? EmailAddress { get; set; }
    }
}