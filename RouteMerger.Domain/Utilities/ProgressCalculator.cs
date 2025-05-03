namespace RouteMerger.Domain.Utilities;

public static class ProgressCalculator
{
    public static decimal CalculateProgress(long bytesRead, long totalBytes)
    {
        if (totalBytes == 0) return 0;
        return Math.Round((decimal)bytesRead / totalBytes * 100, 2);
    }
}