using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallNotificationService.Domain.Abstratctions.Interfaces
{
    public interface ISender
    {
        Task SendAsync<T>(T payload);
    }
}
