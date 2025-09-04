# Coworkee Modular Framework

The Coworkee Modular Framework is a lightweight, ABP-inspired framework for building modular .NET applications. It provides a simple yet powerful way to organize your application into discrete, reusable modules.

## Overview

The framework provides:
- **Automatic Module Discovery**: Modules are automatically discovered and loaded from assemblies
- **Dependency Management**: Modules can declare dependencies on other modules, ensuring proper load order
- **Service Registration**: Each module configures its own services and dependencies
- **Extensibility**: Easy to add new modules without changing existing code

## Core Components

### 1. Core Module (`Coworkee.Core`)

The foundation of the framework, providing:
- `IModuleLayerEntrance`: Interface for all modules
- `ModuleLayer`: Base class for modules
- `ModuleLoader`: Discovers and configures modules
- `ServiceCollectionExtensions`: Extension methods for DI registration

### 2. Module Structure

Each module implements `IModuleLayerEntrance` or inherits from `ModuleLayer`:

```csharp
public class ExampleModule : ModuleLayer
{
    public override string ModuleName => "Example";
    
    public override Type[] Dependencies => new[] { typeof(OtherModule) };
    
    public override IServiceCollection ConfigureServices(IServiceCollection services)
    {
        // Register module-specific services
        services.AddScoped<IExampleService, ExampleService>();
        return services;
    }
}
```

### 3. Application Layers

The framework organizes code into logical layers:

- **Domain** (`Coworkee.Domain`): Core business entities and interfaces
- **Application** (`Coworkee.Application`): Business logic and use cases  
- **Infrastructure** (`Coworkee.Infrastructure`): Data access and external services
- **Client.Blazor** (`Coworkee.Client.Blazor`): Blazor WebAssembly UI

## Quick Start

### 1. Basic Usage

```csharp
// Add the framework to your services
services.AddCoworkeeCore(
    typeof(DomainModule).Assembly,
    typeof(ApplicationModule).Assembly,
    typeof(InfrastructureModule).Assembly
);
```

### 2. Manual Module Registration

```csharp
// Register a specific module
services.AddModule<MyModule>();

// Register a module instance
services.AddModule(new MyModule());
```

### 3. Module Dependencies

Modules can declare dependencies to ensure proper load order:

```csharp
public class InfrastructureModule : ModuleLayer
{
    public override string ModuleName => "Infrastructure";
    
    // This module depends on Application module
    public override Type[] Dependencies => new[] { typeof(ApplicationModule) };
    
    public override IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return services.AddInfrastructure();
    }
}
```

## Example Application

See `example/Coworkee.Example` for a complete console application demonstrating the framework.

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddCoworkeeCore(
            typeof(DomainModule).Assembly,
            typeof(ApplicationModule).Assembly,
            typeof(InfrastructureModule).Assembly
        );
    })
    .Build();

await host.RunAsync();
```

## Module Loading Process

1. **Discovery**: Framework scans specified assemblies for `IModuleLayerEntrance` implementations
2. **Dependency Resolution**: Modules are sorted by their dependencies
3. **Service Configuration**: Each module's `ConfigureServices` method is called in order
4. **Registration**: All modules are registered as singletons in the DI container

## Best Practices

### Module Design
- Keep modules focused on a single responsibility
- Declare dependencies explicitly
- Use interfaces for cross-module communication
- Avoid circular dependencies

### Service Registration
- Register services in the appropriate module
- Use appropriate service lifetimes (Scoped, Transient, Singleton)
- Keep module initialization lightweight

### Dependency Management
- List dependencies in the `Dependencies` property
- The framework handles load order automatically
- Circular dependencies will be detected and logged

## Extending the Framework

### Creating Custom Modules

1. Create a class inheriting from `ModuleLayer`
2. Implement required properties and methods
3. Register your services in `ConfigureServices`

```csharp
public class MyCustomModule : ModuleLayer
{
    public override string ModuleName => "MyCustom";
    
    public override IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IMyService, MyService>();
        services.AddSingleton<IMyConfiguration, MyConfiguration>();
        return services;
    }
}
```

### Module Configuration

Modules can accept configuration through their constructor:

```csharp
public class ConfigurableModule : ModuleLayer
{
    private readonly MyConfiguration _config;
    
    public ConfigurableModule(MyConfiguration config)
    {
        _config = config;
    }
    
    public override IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_config);
        return services;
    }
}
```

## Migration Guide

### From Existing Coworkee Application

1. **Core Foundation**: The `Coworkee.Core` project provides the framework foundation
2. **Module Implementation**: Each layer now has a corresponding module:
   - `DomainModule` for domain services
   - `ApplicationModule` for application services
   - `InfrastructureModule` for infrastructure services
3. **Service Registration**: Replace direct DI registration with module-based registration
4. **Dependencies**: Explicitly declare module dependencies

### Benefits of Migration

- **Better Organization**: Clear separation of concerns through modules
- **Reusability**: Modules can be reused across different applications
- **Maintainability**: Easier to understand and maintain modular structure
- **Testability**: Modules can be tested in isolation
- **Extensibility**: New features can be added as new modules

## Troubleshooting

### Common Issues

1. **Circular Dependencies**: Check module dependencies for cycles
2. **Missing Dependencies**: Ensure all required modules are included
3. **Service Resolution**: Verify services are registered in the correct module
4. **Load Order**: Dependencies determine module load order

### Debugging

Enable logging to see module loading process:

```csharp
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
```

The framework will log:
- Discovered modules
- Module load order
- Dependency resolution
- Any errors during loading

## Roadmap

Future enhancements planned:
- Module versioning and compatibility checks
- Hot module reloading for development
- Module marketplace and package management
- Advanced dependency injection features
- Performance optimizations for large applications

## Contributing

The framework is designed to be extensible. Contributions are welcome for:
- New module types
- Enhanced dependency resolution
- Performance improvements
- Documentation and examples

---

*This framework provides a solid foundation for building modular, maintainable .NET applications while keeping complexity minimal.*