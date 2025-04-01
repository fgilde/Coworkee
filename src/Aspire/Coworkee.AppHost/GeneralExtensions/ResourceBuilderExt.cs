namespace Coworkee.AppHost.GeneralExtensions;

internal static class ResourceBuilderExt
{
    
    public static IResourceBuilder<SqlServerServerResource> WithDataVolumeIf(this IResourceBuilder<SqlServerServerResource> builder, bool condition, string? name = null, bool isReadOnly = false)
    {
        if (condition)
            builder.WithDataVolume(name, isReadOnly);
        return builder;
    }
    public static IResourceBuilder<PostgresServerResource> WithDataVolumeIf(this IResourceBuilder<PostgresServerResource> builder, bool condition, string? name = null, bool isReadOnly = false)
    {
        if (condition)
            builder.WithDataVolume(name, isReadOnly);
        return builder;
    }

}