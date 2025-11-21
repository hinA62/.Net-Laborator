using Microsoft.Extensions.Logging;

namespace ProductManagement.Logging;

public static class LoggingExtensions
{
    public static void LogProductCreationMetrics(
        this ILogger logger,
        ProductCreationMetrics metrics)
    {
        logger.Log(
            LogLevel.Information,
            new EventId(ProductLogEvents.ProductCreationCompleted, nameof(ProductLogEvents.ProductCreationCompleted)),
            "Product creation metrics: " +
            "OperationId={OperationId}, " +
            "Name={ProductName}, SKU={SKU}, Category={Category}, " +
            "ValidationDurationMs={ValidationMs}, " +
            "DatabaseSaveDurationMs={DbMs}, " +
            "TotalDurationMs={TotalMs}, " +
            "Success={Success}, ErrorReason={ErrorReason}",
            metrics.OperationId,
            metrics.ProductName,
            metrics.SKU,
            metrics.Category.ToString(),
            metrics.ValidationDuration.TotalMilliseconds,
            metrics.DatabaseSaveDuration.TotalMilliseconds,
            metrics.TotalDuration.TotalMilliseconds,
            metrics.Success,
            metrics.ErrorReason ?? "None"
        );
    }
}