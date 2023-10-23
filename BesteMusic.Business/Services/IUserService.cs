using BesteMusic.Business.Dtos;
using BesteMusic.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BesteMusic.Business.Services
{
    public interface IUserService
    {
        ServiceMessage AddUser(AddUserDto addUserDto);

        UserInfoDto LoginUser(LoginDto loginDto);
    }
}
