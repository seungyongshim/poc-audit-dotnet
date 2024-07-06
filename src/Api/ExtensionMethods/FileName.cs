namespace Api.ExtensionMethods;

public static class FileName
{
    public static TBuilder WithAction<TBuilder>(this TBuilder builder, EnumAction action) where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (ctx, next) =>
        {
            return await next(ctx);
        });

        return builder;
    }
}

public enum EnumAction
{
    Search,
    Add,
    Modify,
    Delete,
    Upload,
    Download,
    On,
    Off,
    Execute,
    Cancel,
    Approve,
    Reject,
    Ban,
    Kick,
    Deploy,
}
