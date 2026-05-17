namespace ProjectManagement.Application.Common.Caching
{
    public interface IDashboardCache
    {
        bool TryGet(out DTOs.DashboardDto? dashboard);
        void Set(DTOs.DashboardDto dashboard);
        void Remove();
    }
}
