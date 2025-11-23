using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Email
{
    public class EmailFluentSettings
    {
        public string? Email { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? BaseUrlClient { get; set; }
    }
}
