using AstroBoy.Models;

namespace AstroBoy.Services
{
    public class DashboardService
    {
        private readonly ItemService _itemService;
        private readonly StoreService _storeService;
        private readonly OwnerService _ownerService;

        public DashboardService(
            ItemService itemService,
            StoreService storeService,
            OwnerService userService)
        {
            _itemService = itemService;
            _storeService = storeService;
            _ownerService = userService;
        }

        public string GetTodaySummary()
        {
            var today = DateTime.Now.ToString("dddd, dd MMM yyyy");
            return $"Today is {today}";
        }

        public (string name, string stats) GetTopStore()
        {
            var stores = _storeService.GetAllStores();
            var items = _itemService.GetAllItems();

            var topStore = stores
                .Select(store => new
                {
                    Store = store,
                    ItemCount = items.Count(i => i.StoreId == store.StoreId)
                })
                .OrderByDescending(x => x.ItemCount)
                .FirstOrDefault();

            if (topStore == null)
                return ("-", "No data");

            return (
                topStore.Store.Name!,
                $"{topStore.ItemCount} items"
            );
        }

        public (string name, string stats) GetTopOwner()
        {
            var owners = _ownerService.GetAllOwners();
            var stores = _storeService.GetAllStores();
            var items = _itemService.GetAllItems();

            var topOwner = owners
                .Select(owner => new
                {
                    Owner = owner,
                    ItemCount = stores
                        .Where(s => s.OwnerId == owner.Id)
                        .Sum(s => items.Count(i => i.StoreId == s.StoreId))
                })
                .OrderByDescending(x => x.ItemCount)
                .FirstOrDefault();

            if (topOwner == null)
                return ("-", "No data");

            return (
                topOwner.Owner.Name,
                $"{topOwner.ItemCount} items"
            );
        }
    }
}