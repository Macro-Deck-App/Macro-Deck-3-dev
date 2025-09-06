using MacroDeck.Server.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;

namespace MacroDeck.Server.Hubs;

public class SystemNotificationHub : Hub<ISystemNotificationClient>;
