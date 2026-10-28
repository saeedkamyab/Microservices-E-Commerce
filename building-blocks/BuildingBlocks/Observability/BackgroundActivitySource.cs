using System.Diagnostics;

namespace Observability;

public static class BackgroundActivitySource
{
    public const string Name =
        "ECommerce.BackgroundProcessing";

    public static readonly ActivitySource Source =
        new(Name);
}
