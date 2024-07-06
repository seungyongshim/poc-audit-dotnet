using Api.Middlewares;

namespace Api.ExtensionMethods;

public static class WithActionExtension
{
    public static TBuilder AddActionDescription<TBuilder>(this TBuilder builder, EnumAction action, Action<EndpointFilterInvocationContext, ActionDetailContext>? actionDetail = null) where TBuilder : IEndpointConventionBuilder
    {
        actionDetail ??= (ctx, audit) =>        {        };

        builder.AddEndpointFilter(async (ctx, next) =>
        {
            var audit = ctx.HttpContext.RequestServices.GetRequiredService<Audit>();

            actionDetail.Invoke(ctx, audit.ActionDetail);
            audit.Action = action.ToString().ToLower();

            return await next(ctx);
        });

        return builder;
    }
}
