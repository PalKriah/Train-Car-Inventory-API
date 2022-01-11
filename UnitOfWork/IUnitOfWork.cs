using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Train_Car_Inventory_App.Models;
using Train_Car_Inventory_App.Repository;

namespace Train_Car_Inventory_App.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : AbstractEntity;

        DbSet<TEntity> GetDbSet<TEntity>() where TEntity : AbstractEntity;

        int SaveChanges();

        Task<int> SaveChangesAsync();

        DbContext Context();
    }
}