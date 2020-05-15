using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace hubClient
{
  class Program
  {
    static void Main(string[] args)
    {
      using (var hubConnection = new HubConnection("https://localhost:5001/observationHub"))
      {
        IHubProxy observationHubProxy = hubConnection.CreateHubProxy("ObservationHub");
        hubConnection.Start().Wait();
      }
    }
  }
}
