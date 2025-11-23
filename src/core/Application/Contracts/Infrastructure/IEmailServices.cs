using Application.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Infrastructure
{
    public interface IEmailServices
    {
        Task<bool> SendEmailAsync(EmailMessage email, string token);
    }
}
