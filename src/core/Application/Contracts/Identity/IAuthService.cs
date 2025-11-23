using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Identity
{
    public interface IAuthService
    {

        string GetSessionUser();

        string CreateToken(User usuario, IList<string>? roles);

    }
}
