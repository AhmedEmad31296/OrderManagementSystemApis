using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.IOC.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static void RegisterAppServices(IServiceCollection services, Assembly assembly)
        {
            // Find all types in the assembly that have names ending with "AppService"
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("AppService"))
                .ToList();

            // Register each service dynamically
            foreach (var serviceType in serviceTypes)
            {
                // Get the first interface that this service implements
                var interfaceType = serviceType.GetInterfaces().FirstOrDefault();

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, serviceType);  // Register with a scoped lifetime
                }
            }
        }
    }
}
