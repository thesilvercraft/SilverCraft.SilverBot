using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Objects.Database.Classes;

namespace SilverBotDS.ProgramExtensions
{
    public class ModuleRegistrationService
    {
        public static object CreateInstance(Type t, IServiceProvider services)
        {
            var ti = t.GetTypeInfo();
            var constructors = ti.DeclaredConstructors
                .Where(xci => xci.IsPublic)
                .ToArray();

            if (constructors.Length != 1)
                throw new ArgumentException(
                    "Specified type does not contain a public constructor or contains more than one public constructor.");

            var constructor = constructors[0];
            var constructorArgs = constructor.GetParameters();
            var args = new object[constructorArgs.Length];

            if (constructorArgs.Length != 0 && services == null)
                throw new InvalidOperationException(
                    "Dependency collection needs to be specified for parameterized constructors.");

            // inject via constructor
            if (constructorArgs.Length != 0)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    args[i] = services.GetRequiredService(constructorArgs[i].ParameterType);
                }
            }

            var moduleInstance = Activator.CreateInstance(t, args);

            // inject into properties
            var props = t.GetRuntimeProperties().Where(xp =>
                xp.CanWrite && xp.SetMethod?.IsStatic == false && xp.SetMethod.IsPublic);
            foreach (var prop in props)
            {
                if (prop.GetCustomAttribute<DSharpPlus.CommandsNext.Attributes.DontInjectAttribute>() != null)
                    continue;

                var service = services.GetService(prop.PropertyType);
                if (service == null)
                    continue;

                prop.SetValue(moduleInstance, service);
            }

            // inject into fields
            var fields = t.GetRuntimeFields().Where(xf => !xf.IsInitOnly && !xf.IsStatic && xf.IsPublic);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<DSharpPlus.CommandsNext.Attributes.DontInjectAttribute>() != null)
                    continue;

                var service = services.GetService(field.FieldType);
                if (service == null)
                    continue;

                field.SetValue(moduleInstance, service);
            }

            return moduleInstance;
        }

        public static IEnumerable<string> GetMissingAssets(string[] required)
        {
            return required.Where(x => !Program.AssetPresent(x));
        }

        public async Task ProcessExternalServiceType(Type? module, ServiceCollection services)
        {
            if (module.GetInterfaces().Contains(typeof(IRequireAssets)))
            {
                var assets = (string[]?)module.GetProperty("RequiredAssets").GetValue(null);
                if (assets != null)
                {
                    var missingAssets = GetMissingAssets(assets);
                    if (missingAssets.Any())
                    {
                        Log.Information(
                            "Module {Module} might not work properly as its requirements weren't met, the asset/s {MissingAssets} is/are missing",
                            module, string.Join(',', missingAssets));
                        var autofix = module.GetMethod("AutoFixRequiredAssets");
                        if (autofix != null)
                        {
                            try
                            {
                                Log.Information(
                                    "Trying to autofix {Module}'s dependancies",
                                    module);
                                autofix.Invoke(null, null);
                            }
                            catch (Exception ex)
                            {
                                Log.Information(ex,
                                    "Failed to autofix {Module}'s dependancies Exception occured",
                                    module);
                            }
                        }
                    }
                }
            }
            if (!module.GetInterfaces().Contains(typeof(IService)))
            {
                return;
            }
            var thing = CreateInstance(module, services.BuildServiceProvider());
            await ((IService)thing).Start();
            services.AddSingleton(thing);
        }

        public async Task ProcessModuleType(Type? type, Config _config, CommandsNextExtension commands, SlashCommandsExtension slash)
        {
            if (type == null)
            {
                return;
            }

            if (type.GetInterfaces().Contains(typeof(IRequireAssets)))
            {
                var assets = (string[]?)type.GetProperty("RequiredAssets")?.GetValue(null);
                if (assets != null)
                {
                    var missingAssets = GetMissingAssets(assets).ToArray();
                    if (missingAssets.Length != 0)
                    {
                        Log.Information(
                            "Module {Module} might not work properly as its requirements weren't met, the asset/s {MissingAssets} is/are missing",
                            type.Name, string.Join(',', missingAssets));
                        var autofix = type.GetMethod("AutoFixRequiredAssets");
                        if (autofix != null)
                        {
                            try
                            {
                                Log.Information("Trying to autofix {Module}'s dependencies", type.Name);
                                autofix.Invoke(null, new object[] { missingAssets });
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex,
                                    "Failed to autofix {Module}'s dependencies Exception occured",
                                    type.Name);
                            }
                        }
                    }
                }
            }

            if (type.IsSubclassOf(typeof(IHaveExecutableRequirements)))
            {
                var n = (IHaveExecutableRequirements)Activator.CreateInstance(type)!;
                if (await n.ExecuteRequirements(_config))
                {
                    commands.RegisterCommands(type);
                }
                else
                {
                    Log.Information("Module {Module} won't be loaded as its requirements weren't met",
                        type.Name);
                }
            }
            else if (type.IsSubclassOf(typeof(ApplicationCommandModule)))
            {
                slash.RegisterCommands(type);
            }
            else if (type.IsSubclassOf(typeof(BaseCommandModule)))
            {
                commands.RegisterCommands(type);
            }
        }
    }
}