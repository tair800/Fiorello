using Fiorella.ViewModels;

namespace Fiorella.Services.Interfaces
{
    public interface IBasketService
    {
        int GetBasketCount();
        List<BasketVM> GetBasketList();
    }
}
