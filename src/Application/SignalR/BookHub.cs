using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SignalR;

public class BookHub:Hub
{
    public async Task SendBookPageImage(string connectionId, byte[] imageData)
    {
        await Clients.Client(connectionId).SendAsync(Strings.ReceiverBook, imageData);
    }
}
