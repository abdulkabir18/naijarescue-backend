using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Notifications
{
    public interface IResponderNotifier
    {
        Task NotifyNearbyRespondersAsync(Incident incident, double radiusInKm = 5.0);
    }
}
