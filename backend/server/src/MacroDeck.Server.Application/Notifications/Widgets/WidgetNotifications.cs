using MacroDeck.Server.Domain.Entities;

namespace MacroDeck.Server.Application.Notifications.Widgets;

public record WidgetCreatedNotification(WidgetEntity Widget) : INotification;

public record WidgetUpdatedNotification(WidgetEntity Widget) : INotification;

public record WidgetDeletedNotification(WidgetEntity Widget) : INotification;
