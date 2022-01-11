using Train_Car_Inventory_App.UnitOfWork;

namespace Train_Car_Inventory_App.Services
{
    public abstract class AbstractService
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected AbstractService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}