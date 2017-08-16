using Vecc.Base32;
using Vecc.Base32.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBase32(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IBase32, Base32>();

            return serviceCollection;
        }
    }
}
