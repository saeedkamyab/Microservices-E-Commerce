using System.Diagnostics;

namespace Observability;

public static class MessagingActivitySource
{
    public const string Name = "ECommerce.Messaging";

    public static readonly ActivitySource Source =
        new(Name);
}
