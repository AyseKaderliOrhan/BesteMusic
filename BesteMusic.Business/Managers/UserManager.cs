using BesteMusic.Business.Dtos;
using BesteMusic.Business.Services;
using BesteMusic.Business.Types;
using BesteMusic.Data.Entities;
using BesteMusic.Data.Repositories;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BesteMusic.Business.Managers
{
    public class UserManager : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IDataProtector _dataprotector;

        public UserManager(IRepository<UserEntity> userRepository , IDataProtectionProvider dataProtectionProvider)
        {
            _userRepository = userRepository;
            _dataprotector = dataProtectionProvider.CreateProtector("security");
        }

        public ServiceMessage AddUser(AddUserDto addUserDto)
        {
            var hasMail = _userRepository.GetAll(x => x.Email.ToLower() == addUserDto.Email.ToLower()).ToList();

            if (hasMail.Any())  
            {
                return new ServiceMessage()
                { 
                    IsSucceed = false ,
                    Message = "Bu Eposta ile bir kullanıcı zaten mevcut."                
                };
            }

            var entity = new UserEntity()
            {
                Email = addUserDto.Email,
                FirstName = addUserDto.FirstName,
                LastName = addUserDto.LastName,
                Password = _dataprotector.Protect(addUserDto.Password),
                UserType = Data.Enums.UserTypeEnum.User,
            };
            _userRepository.Add(entity);

            return new ServiceMessage()
            {
                IsSucceed = true,
                Message = "Hesap başarıyla oluşturuldu."
            };
        }

        public UserInfoDto LoginUser(LoginDto loginDto)
        {
            var UserEntity = _userRepository.Get(x => x.Email == loginDto.Email);

            if (UserEntity is null)
            {
                return null;
            }

            var rawPassword = _dataprotector.Unprotect(UserEntity.Password);

            if (loginDto.Password == rawPassword)
            {
                return new UserInfoDto()
                {
                    Id = UserEntity.Id,
                    Email = UserEntity.Email,
                    FirstName = UserEntity.FirstName,
                    LastName = UserEntity.LastName,
                    UserType = UserEntity.UserType
                };
            }
            else
            {
                return null;
            }

           
        }
    }
}
