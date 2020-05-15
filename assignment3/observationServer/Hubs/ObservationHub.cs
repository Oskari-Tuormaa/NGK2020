using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace observationServer.Hubs
{
  public class ObservationHub : Hub
  {
    public async Task SendMessage(string user, string message)
    {
      await Clients.All.SendAsync("MessageReceivec", user, message);
    }
  }
}
