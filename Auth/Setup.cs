using Auth.Enums;
using Auth.Helpers;
using Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Auth
{
    public class Setup
    {
        private readonly DataContext DataContext;
        public Setup(IConfiguration Configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
            DataContext = new DataContext(optionsBuilder.Options);
            InitEnum();
            Init();
        }

        private void Init()
        {
            Provider provider = DataContext.Provider.Where(p => p.TypeId == ProviderTypeEnum.SELF.Id).FirstOrDefault();
            if(provider == null)
            {
                provider = new Provider
                {
                    Name = "Local",
                    IsDefault = true,
                    TypeId = ProviderTypeEnum.SELF.Id,
                    Value = null,
                };
                DataContext.Provider.Add(provider);
                DataContext.SaveChanges();
            }

            Role Admin = DataContext.Role
                .Where(r => r.Code == "ADMIN")
                .FirstOrDefault();
            if (Admin == null)
            {
                Admin = new Role
                {
                    Code = "ADMIN",
                };
                DataContext.Role.Add(Admin);
                DataContext.SaveChanges();
            }

            ApplicationUser applicationUser = DataContext.ApplicationUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower())
                .FirstOrDefault();
            if (applicationUser == null)
            {
                applicationUser = new ApplicationUser
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                    DisplayName = "Administrator",
                    Email = "",
                    Password = HashPassword(StaticParams.AdminPassword),
                    Phone = "",
                    RetryTime = 0,
                    StatusId = UserStatusEnum.ACTIVE.Id,
                    ProviderId = provider.Id,
                    Username = "Administrator"
                };
                DataContext.ApplicationUser.Add(applicationUser);
                DataContext.SaveChanges();
            }

            UserRoleMapping userRole = DataContext.UserRoleMapping
                .Where(ur => ur.RoleId == Admin.Id && ur.ApplicationUserId == applicationUser.Id)
                .FirstOrDefault();
            if (userRole == null)
            {
                userRole = new UserRoleMapping
                {
                    ApplicationUserId = applicationUser.Id,
                    RoleId = Admin.Id,
                };
                DataContext.UserRoleMapping.Add(userRole);
                DataContext.SaveChanges();
            }
        }

        private void InitEnum()
        {
            InitProviderTypeEnum();
            InitUserStatusEnum();
            DataContext.SaveChanges();
        }
        private void InitProviderTypeEnum()
        {
            List<ProviderType> providerTypes = DataContext.ProviderType.ToList();
            if (!providerTypes.Any(pt => pt.Id == ProviderTypeEnum.SELF.Id))
            {
                DataContext.ProviderType.Add(new ProviderType
                {
                    Id = ProviderTypeEnum.SELF.Id,
                    Code = ProviderTypeEnum.SELF.Code,
                    Name = ProviderTypeEnum.SELF.Name,
                });
            }
            if (!providerTypes.Any(pt => pt.Id == ProviderTypeEnum.AD.Id))
            {
                DataContext.ProviderType.Add(new ProviderType
                {
                    Id = ProviderTypeEnum.AD.Id,
                    Code = ProviderTypeEnum.AD.Code,
                    Name = ProviderTypeEnum.AD.Name,
                });
            }

            if (!providerTypes.Any(pt => pt.Id == ProviderTypeEnum.ADFS.Id))
            {
                DataContext.ProviderType.Add(new ProviderType
                {
                    Id = ProviderTypeEnum.ADFS.Id,
                    Code = ProviderTypeEnum.ADFS.Code,
                    Name = ProviderTypeEnum.ADFS.Name,
                });
            }
            if (!providerTypes.Any(pt => pt.Id == ProviderTypeEnum.GOOGLE.Id))
            {
                DataContext.ProviderType.Add(new ProviderType
                {
                    Id = ProviderTypeEnum.GOOGLE.Id,
                    Code = ProviderTypeEnum.GOOGLE.Code,
                    Name = ProviderTypeEnum.GOOGLE.Name,
                });
            }
        }

        private void InitUserStatusEnum()
        {
            List<UserStatus> statuses = DataContext.UserStatus.ToList();
            if (!statuses.Any(pt => pt.Id == UserStatusEnum.ACTIVE.Id))
            {
                DataContext.UserStatus.Add(new UserStatus
                {
                    Id = UserStatusEnum.ACTIVE.Id,
                    Code = UserStatusEnum.ACTIVE.Code,
                    Name = UserStatusEnum.ACTIVE.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == UserStatusEnum.INACTIVE.Id))
            {
                DataContext.UserStatus.Add(new UserStatus
                {
                    Id = UserStatusEnum.INACTIVE.Id,
                    Code = UserStatusEnum.INACTIVE.Code,
                    Name = UserStatusEnum.INACTIVE.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == UserStatusEnum.LOCKED.Id))
            {
                DataContext.UserStatus.Add(new UserStatus
                {
                    Id = UserStatusEnum.LOCKED.Id,
                    Code = UserStatusEnum.LOCKED.Code,
                    Name = UserStatusEnum.LOCKED.Name,
                });
            }
        }            

        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}
