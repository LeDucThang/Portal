using Portal.BE.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Helpers;
using Common;
using System.Reflection;
using Portal.Models;

namespace Portal.BE
{
    public class Setup
    {
        private readonly DataContext DataContext;
        public Setup(IConfiguration Configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
            DataContext = new DataContext(optionsBuilder.Options);
            DataContext.Database.Migrate();
            InitEnum();
            Init();
        }

        private void Init()
        {
            InitRoute();

            RoleDAO Admin = DataContext.Role
               .Where(r => r.Name == "ADMIN")
               .FirstOrDefault();
            if (Admin == null)
            {
                Admin = new RoleDAO
                {
                    Name = "ADMIN",
                };
                DataContext.Role.Add(Admin);
                DataContext.SaveChanges();
            }

            ApplicationUserDAO applicationUser = DataContext.ApplicationUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower())
                .FirstOrDefault();
            if (applicationUser == null)
            {
                applicationUser = new ApplicationUserDAO
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                    DisplayName = "Administrator",
                    Email = "",
                    Password = HashPassword(StaticParams.AdminPassword),
                    Phone = "",
                    UserStatusId = UserStatusEnum.ACTIVE.Id,
                    Username = "Administrator"
                };
                DataContext.ApplicationUser.Add(applicationUser);
                DataContext.SaveChanges();
            }

            UserRoleMappingDAO userRole = DataContext.UserRoleMapping
                .Where(ur => ur.RoleId == Admin.Id && ur.ApplicationUserId == applicationUser.Id)
                .FirstOrDefault();
            if (userRole == null)
            {
                userRole = new UserRoleMappingDAO
                {
                    ApplicationUserId = applicationUser.Id,
                    RoleId = Admin.Id,
                };
                DataContext.UserRoleMapping.Add(userRole);
                DataContext.SaveChanges();
            }
        }

        private void InitRoute()
        {
            List<Type> routeTypes = typeof(Setup).Assembly.GetTypes()
               .Where(x => typeof(Root).IsAssignableFrom(x) && x.IsClass)
               .ToList();

            List<ViewDAO> views = DataContext.View.ToList();
            views.ForEach(m => m.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                ViewDAO view = views.Where(m => m.Name == type.Name).FirstOrDefault();
                if (view == null)
                {
                    view = new ViewDAO
                    {
                        Name = type.Name,
                        IsDeleted = false,
                    };
                    views.Add(view);
                }
                else
                {
                    view.IsDeleted = false;
                }
            }
            DataContext.BulkMerge(views);
            views = DataContext.View.ToList();
            List<PageDAO> pages = DataContext.Page.OrderBy(p => p.Path).ToList();
            pages.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                ViewDAO view = views.Where(v => v.Name == type.Name).FirstOrDefault();
                var values = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToList();
                foreach (string value in values)
                {
                    PageDAO page = pages.Where(p => p.Path == value).FirstOrDefault();
                    if (page == null)
                    {
                        page = new PageDAO
                        {
                            Name = value,
                            Path = value,
                            IsDeleted = false,
                            ViewId = view.Id,
                        };
                        pages.Add(page);
                    }
                    else
                    {
                        page.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(pages);
            List<PermissionFieldDAO> permissionFields = DataContext.PermissionField.ToList();
            permissionFields.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                ViewDAO view = views.Where(v => v.Name == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, FieldType>))
                .Select(x => (Dictionary<string, FieldType>)x.GetValue(x))
                .FirstOrDefault();
                if (value == null)
                    continue;
                foreach (var pair in value)
                {
                    PermissionFieldDAO permissionField = permissionFields
                        .Where(p => p.ViewId == view.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (permissionField == null)
                    {
                        permissionField = new PermissionFieldDAO
                        {
                            ViewId = view.Id,
                            Name = pair.Key,
                            Type = pair.Value.ToString(),
                            IsDeleted = false,
                        };
                        permissionFields.Add(permissionField);
                    }
                    else
                    {
                        permissionField.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(permissionFields);
            string sql = DataContext.PermissionPageMapping.Where(ppm => ppm.Page != null && ppm.Page.IsDeleted).ToSql();
            DataContext.PermissionPageMapping.Where(ppm => ppm.Page != null && ppm.Page.IsDeleted).DeleteFromQuery();
            DataContext.Page.Where(p => p.IsDeleted || (p.View != null && p.View.IsDeleted)).DeleteFromQuery();
            DataContext.PermissionData.Where(pd => pd.PermissionField != null && pd.PermissionField.IsDeleted).DeleteFromQuery();
            DataContext.PermissionField.Where(pf => pf.IsDeleted || (pf.View != null && pf.View.IsDeleted)).DeleteFromQuery();
            DataContext.View.Where(v => v.IsDeleted).DeleteFromQuery();
        }

        private void InitEnum()
        {
            InitProviderEnum();
            InitUserStatusEnum();
            DataContext.SaveChanges();
        }
        private void InitProviderEnum()
        {
            List<ProviderDAO> providers = DataContext.Provider.ToList();
            if (!providers.Any(pt => pt.Id == ProviderEnum.SELF.Id))
            {
                providers.Add(new ProviderDAO
                {
                    Id = ProviderEnum.SELF.Id,
                    Name = ProviderEnum.SELF.Name,
                });
            }
            if (!providers.Any(pt => pt.Id == ProviderEnum.AD.Id))
            {
                providers.Add(new ProviderDAO
                {
                    Id = ProviderEnum.AD.Id,
                    Name = ProviderEnum.AD.Name,
                });
            }

            if (!providers.Any(pt => pt.Id == ProviderEnum.Microsoft.Id))
            {
                providers.Add(new ProviderDAO
                {
                    Id = ProviderEnum.Microsoft.Id,
                    Name = ProviderEnum.Microsoft.Name,
                });
            }
            if (!providers.Any(pt => pt.Id == ProviderEnum.GOOGLE.Id))
            {
                providers.Add(new ProviderDAO
                {
                    Id = ProviderEnum.GOOGLE.Id,
                    Name = ProviderEnum.GOOGLE.Name,
                });
            }
        }

        private void InitUserStatusEnum()
        {
            List<UserStatusDAO> statuses = DataContext.UserStatus.ToList();
            if (!statuses.Any(pt => pt.Id == UserStatusEnum.ACTIVE.Id))
            {
                DataContext.UserStatus.Add(new UserStatusDAO
                {
                    Id = UserStatusEnum.ACTIVE.Id,
                    Code = UserStatusEnum.ACTIVE.Code,
                    Name = UserStatusEnum.ACTIVE.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == UserStatusEnum.INACTIVE.Id))
            {
                DataContext.UserStatus.Add(new UserStatusDAO
                {
                    Id = UserStatusEnum.INACTIVE.Id,
                    Code = UserStatusEnum.INACTIVE.Code,
                    Name = UserStatusEnum.INACTIVE.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == UserStatusEnum.LOCKED.Id))
            {
                DataContext.UserStatus.Add(new UserStatusDAO
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
