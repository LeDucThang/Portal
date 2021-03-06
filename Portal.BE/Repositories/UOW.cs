using Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Portal.Models;
using Portal.Repositories;

namespace Portal.Repositories
{
    public interface IUOW : IServiceScoped
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IApplicationUserRepository ApplicationUserRepository { get; }
        IPageRepository PageRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IPermissionDataRepository PermissionDataRepository { get; }
        IPermissionFieldRepository PermissionFieldRepository { get; }
        IProviderRepository ProviderRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISiteRepository SiteRepository { get; }
        IUserStatusRepository UserStatusRepository { get; }
        IViewRepository ViewRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IApplicationUserRepository ApplicationUserRepository { get; private set; }
        public IPageRepository PageRepository { get; private set; }
        public IPermissionRepository PermissionRepository { get; private set; }
        public IPermissionDataRepository PermissionDataRepository { get; private set; }
        public IPermissionFieldRepository PermissionFieldRepository { get; private set; }
        public IProviderRepository ProviderRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public ISiteRepository SiteRepository { get; private set; }
        public IUserStatusRepository UserStatusRepository { get; private set; }
        public IViewRepository ViewRepository { get; private set; }

        public UOW(DataContext DataContext)
        {
            this.DataContext = DataContext;

            ApplicationUserRepository = new ApplicationUserRepository(DataContext);
            PageRepository = new PageRepository(DataContext);
            PermissionRepository = new PermissionRepository(DataContext);
            PermissionDataRepository = new PermissionDataRepository(DataContext);
            PermissionFieldRepository = new PermissionFieldRepository(DataContext);
            ProviderRepository = new ProviderRepository(DataContext);
            RoleRepository = new RoleRepository(DataContext);
            SiteRepository = new SiteRepository(DataContext);
            UserStatusRepository = new UserStatusRepository(DataContext);
            ViewRepository = new ViewRepository(DataContext);
        }
        public async Task Begin()
        {
            await DataContext.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            DataContext.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            DataContext.Database.RollbackTransaction();
            return Task.CompletedTask;
        }
    }
}