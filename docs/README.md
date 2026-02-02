<p align="center">
    <img src="logo.png" width="100" height="100">
</p>

# Moravian Star

**A comprehensive .NET library that helps you focus on writing business code rather than technical code.**

Moravian Star is a library built on top of [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) and [Hot Chocolate](https://chillicream.com/docs/hotchocolate) that reduces boilerplate code in your WebAPI, GraphQL, console or other type of applications. It provides a robust foundation for SQL database operations, CRUD workflows, and common application functionalities.

[![NuGet](https://img.shields.io/nuget/v/MoravianStar.svg)](https://www.nuget.org/packages/MoravianStar)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/lneshev/MoravianStar/blob/main/LICENSE)

> **📚 Demo Repository:** Check out [Moravian Star - Demo](https://github.com/lneshev/MoravianStar-Demo) for comprehensive examples and usage patterns.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Setup](#setup)
- [Core Concepts](#core-concepts)
  - [The Repository Pattern](#the-repository-pattern)
  - [Lifecycle Hooks](#lifecycle-hooks)
  - [Entity to Model Mapping (and vice versa)](#entity-to-model-mapping-and-vice-versa)
- [Detailed Usage](#detailed-usage)
  - [Working with SQL Data](#working-with-sql-data)
  - [WebAPI Features](#webapi-features)
  - [GraphQL Features](#graphql-features)
  - [Additional Features](#additional-features)
- [The Big Picture](#the-big-picture)
  - [The Problem](#the-problem)
  - [The Solution](#the-solution)
  - [Key Benefits](#key-benefits)
  - [Example: Complete User Management](#example-complete-user-management)
- [License](#license)
- [Contributing](#contributing)
- [Support](#support)

## Overview

Moravian Star addresses a common problem in enterprise applications: developers spend too much time writing repetitive technical code instead of focusing on business logic. By providing pre-built, customizable workflows for common operations, Moravian Star lets you:

- ✅ Reduce boilerplate code
- ✅ Maintain consistent patterns across your application
- ✅ Plug in custom business logic at specific lifecycle points
- ✅ Build type-safe, maintainable applications faster

**Supported .NET Versions:** .NET 6.0, 7.0, 8.0, 9.0 and 10.0

## Key Features

### 🎯 Core Package (`MoravianStar`)

**SQL & Data Access:**
- **Customizable CRUD workflows** with lifecycle hooks for entities and models
- **Dynamic filtering and sorting** with type-safe query building
- **Pagination support** with total count retrieval in a single query
- **Entity-to-Model transformations** (projections) and vice versa
- **Extended transaction management** with commit/rollback event handlers
- **Multi-database support** for working with multiple DbContexts

**Infrastructure:**
- **Service Locator pattern** for dependency injection in customizable flows
- **Rich extension methods** for strings, collections, DateTime, enums, and more
- **Custom exceptions** with automatic HTTP status code mapping
- **Enum translation** with resource file integration

### 🌐 WebAPI Package (`MoravianStar.WebAPI`)

- **`EntityRestController<>`** - Abstract base controller with complete CRUD operations
- **`EnumsController`** - Automatic enum exposure with translation support
- **Transaction attributes** - Execute HTTP requests in SQL transactions
- **Customizable global exception handling** with standardized error responses
- **Request logging integration** via [ElmahCore](https://github.com/ElmahCore/ElmahCore)
- **Swagger enhancements** - Hide internal endpoints, custom filters
- **Model binders & converters** - Auto-trim strings, unified endpoint naming and others

### 🔷 GraphQL Package (`MoravianStar.GraphQL`)

- **`[UseServiceLocator]`** - Initialize Moravian Star in GraphQL resolvers
- **`[UseTransaction]`** - Execute mutations in SQL transactions
- **Error filters** - Standardized error handling and logging
- **ElmahCore integration** - Automatic exception logging for GraphQL operations


## Installation

Moravian Star is available on [NuGet](https://www.nuget.org/packages?q=MoravianStar). Choose the package that fits your project type:

### For ASP.NET Core WebAPI Projects

```bash
dotnet add package MoravianStar.WebAPI
```

This package includes the core `MoravianStar` package as a dependency.

### For GraphQL Projects (Hot Chocolate)

```bash
dotnet add package MoravianStar.GraphQL
```

This package includes the core `MoravianStar` package as a dependency.

### For Console Applications or Core Features Only

```bash
dotnet add package MoravianStar
```

## Setup

### WebAPI Setup:

**1. Configure the middleware** in your `Startup.cs` or `Program.cs`:

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        //...
    }

    // Initialize Moravian Star middleware with the default settings
    app.UseMoravianStar(env);
    
    // OR

    // Optional: Initialize Moravian Star middleware with different settings
    app.UseMoravianStar(env, () =>
    {
        Settings.DefaultDbContextType = typeof(MyDbContext);
        Settings.AssemblyForEnums = typeof(UserStatus).Assembly;
        Settings.StringResourceTypeForEnums = typeof(Strings);
    });

    // ... other middlewares
}
```
**2. Register database transaction services** for each DbContext:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IDbTransaction<MyDbContext>, DbTransaction<MyDbContext>>();
}
```
**3. (Optional, but recommended) Hide non-invokable endpoints from Swagger:** If you use Swagger, in ConfigureServices method, add the following filter:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        options.DocumentFilter<HideInDocsFilter>();
    });
}
```
This will hide from the Swagger documentation every endpoint that is marked with `[NonInvokableAttribute]` (exists in `MoravianStar.WebAPI.Attributes`). There are a lot of endpoints (actions) in the abstract `EntityRestController` in `MoravianStar.WebAPI`, so it is a very good idea to hide them.

### GraphQL Setup:
For a WebAPI project, that is configured to use HotChocolate (GraphQL) and the WebAPI controllers are not used at all, there is no need to setup anything in Startup.cs (or Program.cs). There are certain attributes that should be applied in certain places, but this is explained in [Detailed Usage -> GraphQL features](#graphql-features).

### Console Application Setup (or similar)
```csharp
// 1. Initialize your DI container
var services = new ServiceCollection();
services.AddDbContext<MyDbContext>(options => /* ... */);
services.AddScoped<IDbTransaction<MyDbContext>, DbTransaction<MyDbContext>>();
var serviceProvider = services.BuildServiceProvider();

// 2. Create a scope and initialize ServiceLocator
using (var scope = serviceProvider.CreateScope())
{
    var scopedServices = scope.ServiceProvider;
    new ServiceLocator(() => scopedServices);
}

// 3. Use Moravian Star features
var users = (await Persistence.ForEntity<User>().ReadAsync<UserFilter>()).Items;
```

## Core Concepts

### The Repository Pattern

Moravian Star internally uses a repository pattern with two main components:

- **`EntityRepository`** - Works directly with EF Core entities
- **`ModelRepository`** - Works with presentation models (DTOs) and handles entity-model transformations

Access these repositories through the static `Persistence` class:

```csharp
// Working with entities
var users = (await Persistence.ForEntity<User>().ReadAsync<UserFilter>()).Items;

// Working with models (DTOs)
var userModels = (await Persistence.ForModel<UserModel, User>().ReadAsync<UserFilter>()).Items;

// Specify DbContext explicitly
var users = (await Persistence.ForDbContext<MyDbContext>().ForEntity<User>().ReadAsync<UserFilter>()).Items;
```

### Lifecycle Hooks

Inject custom business logic at specific points in the CRUD lifecycle by implementing these interfaces and registering them as services in the DI container:

**For Save Operations:**
- `IGetOriginalEntity<TEntity>` - Retrieve the original entity before modifications
- `IEntityFilling<TEntity>` - Fill entity properties before validation
- `IEntityValidating<TEntity>` - Validate entity before saving (pre-validation)
- `IEntityValidated<TEntity>` - Validate entity after filling (post-validation)
- `IEntitySaving<TEntity>` - Execute logic before saving to database
- `IEntitySaved<TEntity>` - Execute logic after successful save

**For Delete Operations:**
- `IEntityDeleting<TEntity>` - Execute logic before deletion
- `IEntityDeleted<TEntity>` - Execute logic after successful deletion

**For Transactions:**
- `IDbTransaction.Committing` event - Execute before transaction commit
- `IDbTransaction.Committed` event - Execute after successful commit

### Entity to Model Mapping (and vice versa)

Create a mapping service to define how entities transform to models and vice versa:

```csharp
public class UserModelMappingService : ModelsMappingService<UserModel, User>
{
    // Define which fields to read from database
    public override Expression<Func<User, IProjectionBase>> Project()
    {
        return entity => new UserModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email
        };
    }

    // Define how to map model to entity for saving
    public override async Task<List<EntityModelPair<User, UserModel>>> ToEntities(
        List<EntityModelPair<User, UserModel>> pairs)
    {
        pairs = await base.ToEntities(pairs);

        foreach (var pair in pairs)
        {
            pair.Entity.Name = pair.Model.Name;
            pair.Entity.Email = pair.Model.Email;
        }

        return pairs;
    }
}

// Register in DI container
services.AddTransient<IModelsMappingService<UserModel, User>, UserModelMappingService>();
```

## Detailed Usage

### Working with SQL data

**Prerequisites for all examples below:**
- `MoravianStar` project is referenced
- `MyDbContext` - Your EF Core DbContext
- `User` entity - class inheriting `EntityBase<int>` and is included in `MyDbContext`
- `UserFilter` class inheriting `FilterSorterBase<User>`
- `UserProjection` class inheriting `ProjectionBase<int>`
- `UserModel` class inheriting `ModelBase<int>`
- `UserModelMappingService` class inheriting `ModelsMappingService<UserModel, User>`
- `UserModelMappingService` registered in DI container
    ```csharp
    services.AddTransient<IModelsMappingService<UserModel, User>, UserModelMappingService>();
    ```
    *If you work with models, you have to create a service that implements interface `IModelsMappingService` into which you have to specify how the transformations between model, entity and vice-versa should happen. There is an abstract class - `ModelsMappingService`, that contains default logics, so it is recommended to be inherited as a starting point. These services should be registered in the DI container.

#### Reading data
```csharp
// Reads all users from the DB
var users = await Persistence.ForEntity<User>().ReadAsync<UserFilter>();

// Reads all users from the DB, with name "John" (requires UserFilter to have property "NameEquals" and "Filter" method to be overridden and filled with a logic that specifies how property "NameEquals" should work)
var users = (await Persistence.ForEntity<User>().ReadAsync<UserFilter>(
    new UserFilter { NameEquals = "John" }
)).Items;

// Reads all users from the DB and sorts them by name descending (requires method "Sort" in UserFilter to be overriden and filled with a logic that specifies how the sorting should work)
var users = (await Persistence.ForEntity<User>().ReadAsync<UserFilter>(
    sorts: new[] { new Sort { Field = "Name", Dir = SortDirection.Desc } }
)).Items;

// Reads 10 users from the DB, skipping the first 10. Remark: since no sorting is specified, the final result might be different each time, so always combine the paging with the sorting
var users = (await Persistence.ForEntity<User>().ReadAsync<UserFilter>(
    page: new Page { PageSize = 10, PageNumber = 2 }
)).Items;

// Reads all users from the DB, as well as all roles for each user (eager loading). For more info see: https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager
var users = (await Persistence.ForEntity<User>().ReadAsync<UserFilter>(
    includes: x => x.Include(y => y.Roles)
)).Items;

// Reads all users from the DB, with trackable option set to false (default is "true"). For more info see: https://learn.microsoft.com/en-us/ef/core/querying/tracking
var users = (await Persistence.ForEntity<User>().ReadAsync<UserFilter>(trackable: false)).Items;

// Reads all users from the DB, together with their total count. In this case, two DB requests are initiated, so this option is "false" by default. This option respects the filtering, but not the paging.
var users = await Persistence.ForEntity<User>().ReadAsync<UserFilter>(getTotalCount: true);
var users = result.Items;
var totalUsersCount = users.TotalCount;

// Reads only the IDs and the statuses of all users from the DB
var userIds = (await Persistence.ForEntity<UserEntity>().ReadAsync<UserFilter, UserProjection>(projection: x => new UserProjection() { Id = x.Id, Status = x.Status })).Items;

// Generates a query that is about to read all users from the database, without executing it. Remark: all examples above, given for reading entities, are also applicable for this method
var usersQuery = Persistence.ForEntity<User>().ReadQuery<UserFilter>();

// Reads all users from the DB and transforms them into UserModels. Remark: all examples above, given for reading entities, are also applicable for reading models
var userModels = (await Persistence.ForModel<UserModel, User>().ReadAsync<UserFilter>()).Items;
```

#### Dynamic filtering and sorting
To use the read functionality described above, you need to create a class that derives class `FilterSorterBase`. This abstract base class allows you to specify how the filtering and sorting should happen for each individual entity in one central place.
- Filtering
    ```csharp
    public class UserFilter : FilterSorterBase<User>
    {
        public string NameEquals { get; set; }

        public override IQueryable<User> Filter<TDbContext>(IQueryable<User> query, IEntityRepository<User, TDbContext> repository)
        {
            query = base.Filter(query, repository);

            var mainCriteria = PredicateBuilder.New<User>(x => true);

            if (!string.IsNullOrEmpty(NameEquals))
            {
                mainCriteria = mainCriteria.And(x => x.Name == NameEquals);
            }

            return query.Where(mainCriteria);
        }
    }
    ```
    - This approach allows you to make a dynamic filtering oven an entity, and in combination with the usage of models, this filtering can also be used from a web api or a client app.
    - Class `PredicateBuilder` is part of the library [LINQKit](https://github.com/scottksmith95/LINQKit) and allows you easily to create predicates. It is not necessary to use it. This is just an example of how you can implement the filtering for an entity. You have your own freedom to implement it as you wish.
    - By using methods `ReadQuery` you may create subqueries and use them in the main criteria:
        ```csharp
        if (CarBrandId.HasValue)
        {
            var userIds = Persistence.ForEntity<Car>().ReadQuery(new CarFilter() { BrandId = CarBrandId }).Select(x => x.UserId).Distinct();
            mainCriteria = mainCriteria.And(x => userIds.Contains(x.Id));
        }
        ```
- Sorting
    ```csharp
    public class UserFilter : FilterSorterBase<User>
    {
        public override List<(Expression<Func<User, object>> expression, SortDirection direction)> Sort<TDbContext>(IEnumerable<Sort> sorts, IEntityRepository<User, TDbContext> repository)
        {
            var result = base.Sort(sorts, repository);

            foreach (var sort in sorts)
            {
                if (sort.Field.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.Name, sort.Dir));
                }
            }

            return result;
        }
    }
    ```
    - This approach allows you to make a dynamic sorting over an entity, and in combination with the usage of models, this sorting can also be used from a web api or a client app.
    - This is just an example of how you can implement the sorting for an entity. You have your own freedom to implement it as you wish.

#### Get by ID
```csharp
// Gets user with ID=1 from the DB
var user = await Persistence.ForEntity<User, int>().GetAsync(1);

// Gets user with ID=1 from the DB, as well as the roles for this user (eager loading). For more info see: https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager
var user = await Persistence.ForEntity<User, int>().GetAsync(1,
    includes: x => x.Include(y => y.Roles)
);

// Gets user with ID=1 from the DB, with trackable option set to false (default is "true"). For more info see: https://learn.microsoft.com/en-us/ef/core/querying/tracking
var user = await Persistence.ForEntity<User, int>().GetAsync(1, trackable: false);

// Reads only the ID and the status of user with ID=1 from the DB
var user = await Persistence.ForEntity<User, int>().GetAsync(1, projection: x => new UserProjection() { Id = x.Id, Status = x.Status });

// Generates a query that is about to get user with ID=1 from the database, without executing it. Remark: all examples above, given for getting an entity by ID, are also applicable for this method
var userQuery = await Persistence.ForEntity<User, int>().GetQuery(1);

// Gets user with ID=1 from the DB and transforms it into a UserModel. Remark: all examples above, given for getting an entity by ID, are also applicable for this method
var userModel = await Persistence.ForModel<UserModel, User, int>().GetAsync(1);
```

#### Counting

```csharp
// Counts all users in the DB. A filter may be applied. The same functionality exist as a shortcut when working with models (Persistence.ForModel<>())
int usersCount = await Persistence.ForEntity<User>().CountAsync<UserFilter>();
```

#### Checking for existence

```csharp
// Checks if users exist in the DB. A filter may be applied. The same functionality exist as a shortcut when working with models (Persistence.ForModel<>())
bool usersExist = await Persistence.ForEntity<User>().ExistAsync<UserFilter>();

// Checks if a user exists in the DB by ID. The same functionality exist as a shortcut when working with models (Persistence.ForModel<>())
bool userExists = await Persistence.ForEntity<User, int>().ExistsAsync(1);
```

#### Saving (creating/updating)

```csharp
// Create example
// Creates a user entity and saves it into the DB
var user = new User() { Name = "John" };
await Persistence.ForEntity<User>().SaveAsync(user);

// Create example
// Creates a user model (such may come from a HTTP request), transforms it into a user entity (internally) and saves it into the DB. Returns the same model (unless there is a custom logic that modifies it upon the transformations), which contains the assigned user's ID
var userModel = new UserModel() { Name = "John" };
userModel = await Persistence.ForModel<User>().CreateAsync(userModel);

// Update example
// Gets a user entity with ID=1 from the DB, modifies it and saves it into the DB
var user = await Persistence.ForEntity<User, int>().GetAsync(1);
user.Name = "John Smith";
await Persistence.ForEntity<User>().SaveAsync(user);

// Update example
// Creates a user model (such may come from a HTTP request), gets a user entity with ID=userModel.Id (internally), transforms it into a user entity (internally) and saves it into the DB. Returns the same model (unless there is a custom logic that modifies it upon the transformations)
var userModel = new UserModel() 
{
    Id = 1,
    Name = "John Smith"
};
userModel = await Persistence.ForModel<UserModel, UserEntity, int>().UpdateAsync(userModel);
```

- Upon saving an entity you may plug your custom business logic by creating services that implements the following interfaces:
    - `IGetOriginalEntity<TEntity>` - Retrieve the original entity before modifications
    - `IEntityFilling<TEntity>` - Fill entity properties before validation
    - `IEntityValidating<TEntity>` - Validate entity before saving (pre-validation)
    - `IEntityValidated<TEntity>` - Validate entity after filling (post-validation)
    - `IEntitySaving<TEntity>` - Execute logic before saving to database
    - `IEntitySaved<TEntity>` - Execute logic after successful save
    
  Example:
    ```csharp
    public class UserValidated : IEntityValidated<User>
    {
        public async Task ValidatedAsync(User entity, User originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            var userFilter = new UserFilter()
            {
                EmailEquals = entity.Email,
                ExcludeIds = new[] { entity.Id }
            };
            bool userExists = await Persistence.ForEntity<User>().ExistAsync<UserFilter>(userFilter);

            if (userExists)
            {
                throw new EntityNotUniqueException("A user with this email already exists.");
            }
        }
    }
    ```

- If you need to check if you are about to create or update an entity, you may use method `IsNew()` of the entity itself. In a service that implements interface `IEntitySaved` you may get this information from the input parameter `entityWasNew`.
- Most of the above mentioned interfaces have a parameter `additionalParameters` of type `IDictionary<string, object>`. Adding data to this dictionary allows you to pass additional data to the next handlers in the chain. Normally you should not use this parameter, but it is left just in case you need it. If you think there is a missing functionallity in the chain, consider opening an issue or start a discussion in the repository.

#### Deleting

```csharp
// Gets a user entity with ID=1 from the DB and deletes it
var user = await Persistence.ForEntity<User, int>().GetAsync(1);
await Persistence.ForEntity<User>().DeleteAsync(user);

// Finds a user entity with ID=1 in the DB (internally) and if it exists, deletes it
await Persistence.ForEntity<User, int>().DeleteAsync(1);

// Finds a user entity with ID=1 in the DB (internally) and if it exists, deletes it and returns a user model in a way that it would look alike right before the deletion
var userModel = await Persistence.ForModel<UserModel, User, int>().DeleteAsync(1);
```
- Upon deleting an entity you may plug your custom business logic by creating services that implements the following interfaces:
    - `IEntityDeleting<TEntity>` - Execute logic before deletion
    - `IEntityDeleted<TEntity>` - Execute logic after successful deletion
- Both interfaces have a parameter `additionalParameters` of type `IDictionary<string, object>`. Adding data to this dictionary allows you to pass additional data to the next handlers in the chain. Normally you should not use this parameter, but it is left just in case you need it. If you think there is a missing functionallity in the chain, consider opening an issue or start a discussion in the repository.

#### Entity-to-model and model-to-entity transformations
These transformations are triggered when you use `Persistence.ForModel<>()` methods or `EntityRestController` that calls it internally. To specify how an entity should be converted (transformed) to a model (and vice-versa), create a service that implements interface `IModelsMappingService`. There is an abstract class - `ModelsMappingService`, that contains default logics, so it is recommended to be inherited as a starting point. A such service should be registered in the DI container.

- Entity-to-model transformations occur when you are about to read data from the DB. To specify how this transformation should happen, there are several options - some required and some not:
  - (Required) Override method `Project()`. This method specifies which fields of an entity should be read from the DB and to which class they should be stored. The class should implement either `IModelBase` or `IProjectionBase`:
    ```csharp
    // Here only user's ID and user's name will be read from the DB. This is equivalent to SQL query "SELECT Id, Name FROM User ..."
    public class UserModelMappingService : ModelsMappingService<UserModel, User>
    {
        public override Expression<Func<User, IProjectionBase>> Project()
        {
            return entity => new UserModel()
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
    ```
  - (Optional) Override method `Project()` and method `MapAsync()`. Method `MapAsync()` extends the functionality of `Project()` method by allowing filling other properties of a single model with data that didn't come with the entity and comes from a different place, like a C# code, *a different query to the DB, *external web API and so on:
    ```csharp
    public class UserModelMappingService : ModelsMappingService<UserModel, User>
    {
        public override Expression<Func<User, IProjectionBase>> Project()
        {
            return entity => new UserProjection()
            {
                Id = entity.Id,         // int
                Name = entity.Name,     // string
                Status = entity.Status  // enum of type UserStatus
            };
        }

        public override async Task<UserModel> MapAsync(IProjectionBase projection)
        {
            // The actual type is UserProjection, but we need to cast it. The type comes from the type that is returned in method "Project()"
            var proj = (UserProjection)projection;

            return await Task.FromResult(new UserModel()
            {
                Id = proj.Id,                                       // int
                Name = proj.Name,                                   // string
                Status = proj.Status,                               // enum of type UserStatus
                StatusText = proj.Status.Translate(typeof(Strings)) // string. Enum's value is being translated
            });
        }
    }
    ```
    *Keep in mind that this method is called for every entity that was returned from the DB. So if you need to call the DB for a different query, an external API or other extensive method to get the additional data, it will be executed N times, where N is the number of returned entities from the DB. This might be crucial for the performance, so in this case it is better to override method `ToModels()` (see next point).
  - (Optional) Override method `Project()` and method `ToModels()`. Method `ToModels()` extends the functionality of `Project()` method by allowing filling other properties of multiple models with data that didn't come with the entity and comes from a different place, like a C# code, a different query to the DB, external web API and so on:
    ```csharp
    public class UserModelMappingService : ModelsMappingService<UserModel, User>
    {
        public override Expression<Func<User, IProjectionBase>> Project()
        {
            return entity => new UserProjection()
            {
                Id = entity.Id,
                ExternalApiReference = entity.ExternalApiReference
            };
        }

        public Task<List<ProjectionModelPair<IProjectionBase, UserModel>>> ToModels(List<ProjectionModelPair<IProjectionBase, UserModel>> pairs)
        {
            var usersDataFromAPI = await FillUsersDataFromExternalAPI();

            foreach (var pair in pairs)
            {
                var proj = (UserProjection)pair.Projection;

                pair.Model = new UserModel()
                {
                    Id = proj.Id,
                    PropertyFromExternalAPI = usersDataFromAPI[proj.ExternalApiReference]
                };
            }

            return pairs;
        }

        private async Task<Dictionary<Guid, string>> FillUsersDataFromExternalAPI()
        {
            // Call to an external web API...
        }
    }
    ```
-  Model-to-entity transformations occur when you are about to save data in the DB. To specify how this transformation should happen, override (or implement) method `ToEntities()`:
    ```csharp
    public class UserModelMappingService : ModelsMappingService<UserModel, User>
    {
        public override async Task<List<EntityModelPair<User, UserModel>>> ToEntities(List<EntityModelPair<User, UserModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Name = pair.Model.Name;
            }

            return pairs;
        }
    }
    ```
    Here, if you need to check if you are about to create or update a model, you may use method `IsNew()` of the model itself.

    Additionally, if you have a business logic upon updating or deleting an entity that should access navigation properties or collections of an entity (and you do not use EF core's lazy loading), you should specify them in method `GetIncludes()`:
    ```csharp
    // File: UserModelMappingService.cs
    public class UserModelMappingService : ModelsMappingService<UserModel, User>
    {
        public override IQueryable<User> GetIncludes(IQueryable<User> query)
        {
            return base.GetIncludes(query).Include(x => x.Roles);
        }
    }

    // File: UserSaving.cs
    public class UserSaving : IEntitySaving<User>
    {
        public async Task SavingAsync(User entity, User originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            if (!entity.Roles.Any())
            {
                throw new BusinessException("The user should have at least one role assigned.");
            }
        }
    }
    ```

#### Database transactions

Moravian Star has an extended way to handle DB transactions compared to EF Core's DB transaction. Internally, it relies on EF Core's Db transaction, but adds some additional functionalities.

To execute a code in a DB transaction, do the following:

1. Register `IDbTransaction` service like this for your `DbContext`:
      ```csharp
      services.AddScoped<IDbTransaction<MyDbContext>, DbTransaction<MyDbContext>>();
      ```
2. Get an instance of `IDbTransaction` service via DI or `ServiceLocator`
3. Use the provided methods in this service to `Begin`, `Commit` or `Rollback` a transaction

Example:
```csharp
var dbTransaction = (IDbTransaction)ServiceLocator.Container.GetRequiredService(serviceType);
await dbTransaction.BeginAsync();

try
{
    var user1 = new User() { Name = "John Smith" };
    var user2 = new User() { Name = "John Doe" };
    await Persistence.ForEntity<User>().SaveAsync(user1);
    await Persistence.ForEntity<User>().SaveAsync(user2);
}
catch
{
    await dbTransaction.RollbackAsync();
    throw;
}

await dbTransaction.CommitAsync();
```

This way:
1. You won't be able to begin a new DB transaction, before you either commit or rollback the current one. This is done intentionally.
2. Upon commit a transaction you may plug your custom business logic by creating event handlers for the following events:
    - `IDbTransaction.Committing` event - Execute before transaction commit
    - `IDbTransaction.Committed` event - Execute after successful commit

    Example:
    ```csharp
    // Here we have implemented service IEntitySaved. Method SavedAsync triggers when a user entity was saved successfully via Persistence.ForEntity<User>().SaveAsync(user);. This means that every time a user is saved, a new committed event handler will be added and once the transaction is successfully committed, all stored committed event handlers will be executed
    public async Task SavedAsync(UserEntity entity, UserEntity originalEntity, bool entityWasNew, IDictionary<string, object> additionalParameters = null)
    {
        Persistence.ForDbContext<MyDbContext>().DbTransaction.Committed += (sender, eventArgs) =>
        {
            // Write your custom logic that will be executed only when the DB transaction was committed successfully. An example for such logic could be sending an email, uploading a picture, start a job, etc.
        };

        await Task.CompletedTask;
    }
    ```

Recommendation:
Always use Moravian Star's way to handle DB transactions, because of the latter functionalities. Also, it is very important to prevent saving without a transaction, by overriding `SaveChanges()` and `SaveChangesAsync()` methods in your `DbContext`, like this:
```csharp
public class SystemContext : DbContext
{
    //...

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnBeforeSaving()
    {
        if (Database.CurrentTransaction == null)
        {
            throw new InvalidOperationException("Saving data to a database without a transaction is not allowed. First open a database transaction and then save&commit the changes.");
        }
    }
}
```

#### Working with a different DbContext

All examples above required setting a default `DbContext` upon app's initialization like this: 

```csharp
// Sets the default DbContext type upon app's initialization.
app.UseMoravianStar(env, () =>
{
    Settings.DefaultDbContextType = typeof(MyDbContext);
});
```

If you have to use a different `DbContext` or if you prefer not to set a default `DbContext`, then you can access all of the above methods like this:
```csharp
var users = await Persistence.ForDbContext<MyDbContext>.ForEntity<User>().ReadAsync<UserFilter>();
```

### WebAPI features

#### EntityRestController

The `EntityRestController` is an abstract base api controller that contains the most common operations over an entity and model (like CRUD, count, exist, etc.), defined in the REST standard. Internally, each of its methods for these operations call the model repository service.

Available Endpoints:
- `GET /api/{controller}` - Read with filtering, sorting, pagination
- `GET /api/{controller}/{id}` - Get by ID
- `GET /api/{controller}/count` - Count with optional filter
- `GET /api/{controller}/exist` - Check existence with optional filter
- `GET /api/{controller}/exists/{id}` - Check existence by ID
- `POST /api/{controller}` - Create
- `PUT /api/{controller}/{id}` - Update
- `DELETE /api/{controller}/{id}` - Delete

To start using the functionalities of this controller first you need to create your own controller for your entity, model, filter and dbContext, and derive the `EntityRestController`:

```csharp
public class UserController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
{
}
```

At this stage and despite that you derived the controller, there are still no endpoints that are exposed. This is done intentionally as a security measure and with the idea that you may want for a given entity and model to expose just some of its operations (for example: only read, but not create, update, delete and the rest). It is achieved by filter attribute: `[NonInvokable]` that will throw `MethodAccessException` if someone decides to try to access an action in the controller that is not overridden explicitly. So, in order an endpoint to be visible to the audience, you should explicitly override the desired action (and call the base logic and/or write your own):

- Read:
    ```csharp
    public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
    {
        public override Task<ActionResult<PageResult<UserModel>>> Read([FromQuery] UserFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }
    }
    ```
    ```
    // Example "read" request:
    Request method: GET
    Request URL: https://localhost:80/api/users?
        NameContainsInsensitive=John
        ExcludeIds[0]=1
        sorts[0].field=Name&
        sorts[0].dir=0&
        PageNumber=1&
        PageSize=10
    ```
- Get by ID
    ```csharp
    public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
    {
        public override async Task<ActionResult<UserModel>> Get([FromRoute] int id)
        {
            return await base.Get(id);
        }
    }
    ```
    ```
    // Example "get by ID" request:
    Request method: GET
    Request URL: https://localhost:80/api/users/1
    ```
- Count
    ```csharp
    public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
    {
        public override async Task<ActionResult<int>> Count([FromQuery] UserFilter filter)
        {
            return await base.Count(filter);
        }
    }
    ```
    ```
    // Example "count" request:
    Request method: GET
    Request URL: https://localhost:80/api/users/count?
        NameContainsInsensitive=John
        ExcludeIds[0]=1
    ```

- Exist
    ```csharp
    public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
    {
        public override async Task<ActionResult<bool>> Exist([FromQuery] UserFilter filter)
        {
            return await base.Exist(filter);
        }
    }
    ```
    ```
    // Example "exist" request:
    Request method: GET
    Request URL: https://localhost:80/api/users/exist?
        NameContainsInsensitive=John
        ExcludeIds[0]=1
    ```

- Exists
    ```csharp
    public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
    {
        public override async Task<ActionResult<bool>> Exists([FromRoute] TId id)
        {
            return await base.Exists(id);
        }
    }
    ```
    ```
    // Example "exists" request:
    Request method: GET
    Request URL: https://localhost:80/api/users/exists/1
    ```

- Create
    ```csharp
    public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
    {
        public override async Task<ActionResult<UserModel>> Post([FromBody] UserModel model)
        {
            return await base.Post(model);
        }
    }
    ```
    ```
    // Example "create" request:
    Request method: POST
    Request URL: https://localhost:80/api/users
    Request body:
    {
        // All properties from UserModel
        "id": 0,
        "name": "John Smith"
    }
    ```

- Update
    ```csharp
    public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
    {
        public override async Task<ActionResult<UserModel>> Put([FromRoute] int id, [FromBody] UserModel model)
        {
            return await base.Put(id, model);
        }
    }
    ```
    ```
    // Example "update" request:
    Request method: PUT
    Request URL: https://localhost:80/api/users/1
    Request body:
    {
        // All properties from UserModel
        "id": 1,
        "name": "John Doe"
    }
    ```

- Delete
    ```csharp
    public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
    {
        public override async Task<ActionResult<UserModel>> Delete([FromRoute] int id)
        {
            return await base.Delete(id);
        }
    }
    ```
    ```
    // Example "delete" request:
    Request method: DELETE
    Request URL: https://localhost:80/api/users/1
    ```

Additional remarks about `EntityRestController`:
- The endpoints for `Read`, `Create`, `Update` and `Delete` execute in a SQL transaction by default, so there is no need to write any additional code and the whole HTTP request will be executed in a single SQL transaction. This is achieved by marking these endpoints with `[ExecuteInTransactionAsync]` attribute in `EntityRestController`. By default, the SQL transaction will be for the default `DbContext` that you have specified as such (see: [Working with a different DbContext](#working-with-a-different-dbcontext)). If you want the SQL transaction to be for a different `DbContext`, you should override the desired action (endpoint) and mark it with: `[ExecuteInTransactionAsync(typeof(OtherDbContext))]`. If you want to execute the action (endpoint) not in a SQL transaction, then you should override the desired action and mark it with: `[ExecuteInTransactionAsync(false)]`.
- The endpoints in this controller return the data in a specific format. To change the format you may create your own base controller (without deriving `EntityRestController`) and reuse the logics from `EntityRestControllerHelper`.

#### EnumsController

The `EnumsController` is an abstract base api controller that provides endpoints for retrieving enumerations in different ways.

To start using the functionalities of this controller first you need to create your own controller and derive the `EnumsController`:

```csharp
public class EnumsController : MoravianStar.WebAPI.Controllers.EnumsController
{
}
```

At this stage and despite that you derived the controller, there are still no endpoints that are exposed. This is done intentionally, because you might not need all of these functionalities exposed at all. It is achieved by filter attribute: `[NonInvokable]` that will throw `MethodAccessException` if someone decides to try to access an action in the controller that is not overridden explicitly. So, in order an endpoint to be visible to the audience, you should explicitly override the desired action (and call the base logic and/or write your own):

- Endpoint: **[GET] "/api/enums"** returns all enums defined in Moravian Star (like: `SortDirection` that is used to specify the sorting direction when using the read functionality) together with enums from your project. To specify which enums to be exposed from your project, set setting: "AssemblyForEnums" upon app's initialization:
    ```csharp
    // Sets the assembly for the enums upon app's initialization.
    app.UseMoravianStar(env, () =>
    {
        Settings.AssemblyForEnums = typeof(UserStatus).Assembly;
    });
    ```

    Then derive the base controller and override this action:
    ```csharp
    public class EnumsController : MoravianStar.WebAPI.Controllers.EnumsController
    {
        public override ActionResult<List<EnumNameValue>> Get()
        {
            return base.Get();
        }
    }
    ```

    This endpoint will return a JSON array like this:
    ```json
    [
        {
            "name": "SortDirection", // Exists in MoravianStar
            "values": {
                "Asc": 0,
                "Desc": 1
            }
        },
        {
            "name": "UserStatus", // Exists in your project
            "values": {
                "Inactive": 0,
                "Active": 1
            }
        }
    ]
    ```
    The purpose of this endpoint can be to help a front-end app to store all these enums in a dictionary and later use this dictionary to check some values like this: `model.status === enums.UserStatus.Active ? ...`, for example.
- Endpoint: **[GET] "/api/enums/{enumName}"** returns the values of a single enum in multiple formats like its integer value, its string value and its translated string value. Similarly to the latter endpoint (described above), this endpoint can return enums from Moravian Star, as well as enums from your project. Again you need to specify which enums to be exposed from your project by set setting: "AssemblyForEnums" as well as "StringResourceTypeForEnums" upon app's initialization:
    ```csharp
    // Sets the assembly for the enums upon app's initialization.
    app.UseMoravianStar(env, () =>
    {
        Settings.AssemblyForEnums = typeof(UserStatus).Assembly;
        Settings.StringResourceTypeForEnums = typeof(Strings);
    });
    ```

    Then derive the base controller and override this action:
    ```csharp
    public class EnumsController : MoravianStar.WebAPI.Controllers.EnumsController
    {
        public override ActionResult<List<EnumTextValue>> Get([FromRoute] string enumName, [FromQuery] List<int> exactEnumValues, [FromQuery] bool sortByText = false)
        {
            return base.Get(enumName, exactEnumValues, sortByText);
        }
    }
    ```
    This endpoint will return a JSON array like this:
    ```json
    // For enum "UserStatus" the result could be:
    [
        {
            "value": 0,
            "stringValue": "Inactive",
            "text": "Неактивен" // "Inactive" translated in Bulgarian
        },
        {
            "value": 1,
            "stringValue": "Active",
            "text": "Активен" // "Active" translated in Bulgarian
        }
    ]
    ```
    This endpoint has two optional parameters:
    - `exactEnumValues` (default: []) - a list of int values. When set, only those values will be returned. For example:
        ```json
        // [GET] "/api/enums/UserStatus?exactEnumValue[0]=1" will return only one element:
        [
            {
                "value": 1,
                "stringValue": "Active",
                "text": "Активен" // "Active" translated in Bulgarian
            }
        ]
        ```
    - `sortByText` (default: false) - boolean parameter that specifies if the values in the result should be sorted by property "text" (when passed "true") or by property "value" (when passed "false").

    The purpose for this endpoint can be to help a front-end app to populate enum's values in a dropdown, for example.

Additional remarks about `EnumsController`:
- The endpoints in this controller return the data in a specific format. To change the format you may create your own base controller (without deriving `EnumsController`) and reuse the logics from `EnumsControllerHelper`.

#### Exception handling
There is a custom exception middleware called: `ExceptionMiddleware`, that catches any exception, logs the exception, creates a generic error model, generates a correct HTTP status code related to the exception, puts it into the error model and writes the error model to the response. The middleware is registered together with MoravianStar's initialization (`app.UseMoravianStar(env);`), but you may register it individually, too.

```
// An example HTTP request and response for EntityNotFoundException (an exception in MoravianStar that is thrown when an entity could not be found by ID):
Request method: GET
Request URL: https://localhost:80/api/users/1
Response status code: 404
Response body:
{
    "message": "An entity of type: 'User' with ID: '1' was not found.", // Exception's message
    "exceptionType": "MoravianStar.Exceptions.EntityNotFoundException", // Exception's full name. Filled-in only for development environments
    "stackTrace": "..." // The whole stack trace. Filled-in only for development environments
}
```

The HTTP status code is set based on the exception thrown. You can find this logic in: `ExceptionExtensions.GetHttpStatusCode(ex)`.

If you use [ElmahCore](https://github.com/ElmahCore/ElmahCore) library to log the exceptions, you may use this exception extension method (`ExceptionExtensions.GetHttpStatusCode(ex)`) to set the correct HTTP status code in the ElmahCore's log like this:
```csharp
services.AddElmah<SqlErrorLog>(options =>
{
    options.OnError = async (httpContext, error) =>
    {
        if (error.Exception != null)
        {
            error.StatusCode = error.Exception.GetHttpStatusCode();
        }
        await Task.CompletedTask;
    };
});
```

Also, there is an attribute called: `[ValidateModelStateAttribute]`, which collects all model state errors, creates a message from them and throws an `InvalidModelStateException` with that message. If you want to use it, you should register it globally and also set option "SuppressModelStateInvalidFilter" to "true":
```csharp
services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelStateAttribute>();
});

services.Configure<ApiBehaviorOptions>(options =>
{
    // This options is set to "true", because the logic in ValidateModelStateAttribute 
    // won't be executed for controllers marked with ApiControllerAttribute
    options.SuppressModelStateInvalidFilter = true;
});
```

If you want to modify the logic in the `ExceptionMiddleware`, first disable it and second derive from it and register your custom middleware (or don't derive from it at all).

```csharp
// Disables the default exception middleware upon app's initialization.
app.UseMoravianStar(env, () =>
{
    Settings.RegisterDefaultExceptionMiddleware = false;
});
```

#### Logging of HTTP requests
Moravian Star WebAPI uses [ElmahCore](https://github.com/ElmahCore/ElmahCore) library to provide a logging of not only failed HTTP requests, but also of successful such. This is useful mostly for debugging/tracing cases. To use this functionality:
1. Add and setup [ElmahCore](https://github.com/ElmahCore/ElmahCore) library in your project
2. Mark any of your actions with attribute: `[ElmahRaiseLog]`.

#### Other WebAPI functionalities
MoravianStar.WebAPI contains various model binders, json converters and transformers like such that trim white-space characters from any string property, unify the naming of the endpoints and others.

### GraphQL features
All GraphQL features in Moravian Star are based on [Hot Chocolate](https://chillicream.com/docs/hotchocolate) library. They can be found in MoravianStar.GraphQL project and namespace.

#### UseServiceLocator attribute
This attribute should be applied on any query, mutation or subscription that requires access to MoravianStar's SQL features. Internally, it initializes MoravianStar's `ServiceLocator`, similarly to what does `app.UseMoravianStar(env);` in a WebAPI startup.

```csharp
// A sample GraphQL query that reads users.
[UseServiceLocator]
[UseOffsetPaging]
[UseProjection]
[GraphQLDescription("Gets the queryable users.")]
public IQueryable<User> GetUsers(UserFilter filter, List<Sort> sorts)
{
    return Persistence.ForDbContext<MyDbContext>().ForEntity<User>().ReadQuery(filter, sorts, trackable: false);
}
```

#### UseTransaction attribute
This attribute should be applied on any query, mutation or subscription that requires the SQL related code inside them to be executed in a single SQL transaction.

```csharp
// A sample GraphQL mutation that deletes a user (found by its ID) and its main address.
// All statements will be executed in a single SQL transaction.
[UseServiceLocator]
[UseTransaction]
[GraphQLDescription("Deletes a user.")]
public async Task<bool> DeleteUserAsync([ID] int id)
{
    var user = await Persistence.ForEntity<User, int>().GetAsync(id);

    if (user.MainAddressId.HasValue)
    {
        await Persistence.ForEntity<Address, Guid>().DeleteAsync(user.MainAddressId.Value);
    }

    await Persistence.ForEntity<User>().DeleteAsync(user);

    return true;
}
```

#### Error filters
- `ErrorImproverErrorFilter` - This error filter is similar to the `ExceptionMiddlleware` for WebAPI. It additionally modifies the error model, returned from Hot Chocolate upon exception, by setting the correct message and HTTP status code. To use it, just include it as a regular error filter:
  ```csharp
  services
    .AddGraphQLServer()
    .AddErrorFilter<ErrorImproverErrorFilter>()
  ```
- `ElmahErrorFilter` - This is an error filter that logs an exception with the help of ElmahCore. To use it, just include it as a regular error filter. If you also use `ErrorImproverErrorFilter`, it should be added after:
  ```csharp
  services
    .AddGraphQLServer()
    .AddErrorFilter<ErrorImproverErrorFilter>()
    .AddErrorFilter<ElmahErrorFilter>()
  ```

### Additional features

#### Translation of enum and boolean types
Moravian Star has a functionality to translate enumerations. It exists as method: `EnumExtensions.Translate()`. In order to work properly, you should put enum's values in a string resource file (.resx file) in the following pattern: `[EnumName]_[EnumValue]`. Example:

```
// In a .resx file:
Key                     Value
UserStatus_Active       Active
UserStatus_Inactive     Inactive
```

```csharp
// Getting a translated enum value:
string translatedEnumValue = user.Status.Translate(typeof(Strings));
```

It is recommended to put all enums in a single project as well as they to have unique names.

Translating a boolean value in pretty much the same. The two keys for the boolean values in the string resource file are: "Boolean_True" and "Boolean_False".

#### Exceptions

In certain cases, Moravian Star (or you) may throw the following exceptions:
- `BusinessException` - The exception that is thrown when a business logic error occurs.
- `EntityNotFoundException` - The exception that is thrown when an entity could not be found.
- `EntityNotUniqueException` - The exception that is thrown when trying to create or update an entity which conflicts with another entity by uniqueness.
- `InvalidModelStateException` - The exception that is thrown when some property or collection in the model state is not valid.

#### Extensions
There are some extension methods for working with strings, lists, DateTime on so. You may find them in namespace: `MoravianStar.Extensions`.

## The Big Picture

### The Problem

In typical data-driven applications, developers write repetitive code for each entity:
- CRUD operations
- Filtering and sorting logic
- Pagination
- Validation
- Transaction management
- Entity-to-DTO transformations

This leads to:
- ❌ Code duplication
- ❌ Inconsistent patterns
- ❌ Difficult maintenance
- ❌ More time on technical code than business logic

### The Solution

Moravian Star extracts all common patterns into reusable components while providing extension points for custom business logic.

The major concept is presented in the following diagram:

<img src="the-big-picture.jpg">

On the left side we have the WebAPI controllers (or could be any other technology which connects the UI with the business logic) and to the right we have the SQL database. Usually between them is the business logic, written in some services, that uses the chosen ORM to communicate with the database. Here, again we have this ORM and for Moravian Star this is Entity Framework Core (EF Core). In theory, it could be any other like: nHibernate, Dapper or just ADO.NET, but EF Core was chosen as most modern, fast enough, well documented and continuously maintained and developed.

Between the Controllers and the ORM, sit the `ModelRepository` and `EntityRepository` as services. They contain all the repetitive logics for each individual entity like the CRUD operations, and operations like counting and checking for existence, but they also provide points where you may plug your own business logic. They are accessible through the static class: `Persistence.ForEntity<>()` and `Persistence.ForModel<>()`.

`EntityRepository` service consists of CRUD methods related to a given entity. In the implementations of this service, additional business logics might be added in certain points of the operations (for example executing specific logics before or after saving/deleting of an entity) by creating and registering services that implement specific interfaces. In the latter services you may call the `EntityRepository` service again. For example, if you need to save a `User`, who has a `MainAddress` property of type `Address` and the type `Address` has a property `Country` which is mandatory and should be one of 5 predefined countries, then you may create `IEntityValidated` service for `Address` entity, put the logic for the 5 countries there and create `IEntitySaving` service for the `User`, where you may put a call for saving the `MainAddress`, which will automatically trigger the checking logic for the 5 countries. This way, you can chain and reuse existing logics with just one line of code.

`ModelRepository` service consists of CRUD methods related to a given entity and model. This means that for a given entity there might be one or more models. Imagine that you have a front-end app where you need to show all users in the system in a table (like an overview), but you show just few user's properties like: email, first name, last name and status. There is a button Edit next to each user in the table, which once clicked leads to a detailed page for the selected user, where you may edit all user's properties - not only first name, last name, but also password, main address, additional addresses and so on. So, in this case you present the user in two ways and you may create two models for each need. This leads to better performance, security and reusability of the code.

The `ModelRepository` service depends on other service called `IModelsMappingService`, which contains generic logics that transfers the entities to a model and vice versa, without to specify "how" they will be transferred. In order to specify this, you should create and register services that implements this interface. There is an abstract base class called ModelsMappingService that implements all methods with default logics and can be used as a starting point.

In other words, imagine an SQL query:
```sql
SELECT COLUMN1, COLUMN2
FROM TABLE1
WHERE COLUMN1 = SomeValue
ORDER BY COLUMN1 ASC
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
```

, then the `ModelRepository` service handles the `"SELECT"` clause and `EntityRepository` service handles the rest clauses as well as the `"SELECT"` clause, but retrieving all columns, unless something else is specified. At this stage, only these features are supported. Features like grouping and aggregation of data are not directly supported, but you may reuse the build query and extend it manually for your need.

`EntityRepository` service gives the ability for dynamic filtering and dynamic sorting, which means that the rules for these operations can be defined at one central place and whoever decides to filter or sort an entity (or a model) may choose any desired combination. This leads to better security and reusability of the code.

### Key Benefits

1. **Write Less Code**: Define your entity, model, filter, and mapping service once. Get complete CRUD operations automatically.

2. **Consistent Patterns**: All entities follow the same patterns, making the codebase predictable and maintainable.

3. **Reusable Business Logic**: Implement validation or business rules once in lifecycle hooks, reuse across different operations.

4. **Type-Safe**: Leverage C# type system for compile-time safety in filters, sorts, and projections.

5. **Performance Optimized**:
   - Projection support for selecting only needed fields
   - No-tracking queries for read-only operations
   - Batch operations for model enrichment
   - Efficient pagination with total count

6. **Flexible**: Override any behavior, add custom logic at any point, or bypass the framework when needed.

### Example: Complete User Management

```csharp
// 1. Install and setup Moravian Star

// 2. Define Entity and add it to your DbContext
public class User : EntityBase<int>
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public UserStatus Status { get; set; }
}

// 3. Define Model
public class UserModel : ModelBase<int>
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public UserStatus Status { get; set; }

    public string StatusText { get; set; }
}

// 4. Define Filter
public class UserFilter : FilterSorterBase<User>
{
    public string NameContains { get; set; }

    public override IQueryable<User> Filter<TDbContext>(IQueryable<User> query, IEntityRepository<User, TDbContext> repository)
    {
        if (!string.IsNullOrEmpty(NameContains))
        {
            query = query.Where(x => x.Name.Contains(NameContains));
        }

        return query;
    }

    public override List<(Expression<Func<User, object>> expression, SortDirection direction)> Sort<TDbContext>(IEnumerable<Sort> sorts, IEntityRepository<User, TDbContext> repository)
        {
            var result = base.Sort(sorts, repository);

            foreach (var sort in sorts)
            {
                if (sort.Field.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.Name, sort.Dir));
                }
            }

            return result;
        }
}

// 5. Define Mapping Service
public class UserModelMappingService : ModelsMappingService<UserModel, User>
{
    public override Expression<Func<User, IProjectionBase>> Project()
    {
        return entity => new UserModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Status = entity.Status
        };
    }

    public override async Task<UserModel> MapAsync(IProjectionBase projection)
    {
        var model = (UserModel)projection;
        model.StatusText = model.Status.Translate(typeof(Strings));
        return await Task.FromResult(model);
    }

    public override async Task<List<EntityModelPair<User, UserModel>>> ToEntities(List<EntityModelPair<User, UserModel>> pairs)
    {
        pairs = await base.ToEntities(pairs);

        foreach (var pair in pairs)
        {
            pair.Entity.Name = pair.Model.Name;
            pair.Entity.Email = pair.Model.Email;
            pair.Entity.Status = pair.Model.Status;
        }

        return pairs;
    }
}

// 6. Register UserModelMappingService in DI container
services.AddTransient<IModelsMappingService<UserModel, User>, UserModelMappingService>();

// 7. (Optional for WebAPI) Create UsersController and override the desired actions
public class UsersController : EntityRestController<User, int, UserModel, UserFilter, MyDbContext>
{
    public override Task<ActionResult<PageResult<UserModel>>> Read([FromQuery] UserFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
    {
        return base.Read(filter, sorts, page);
    }

    public override Task<ActionResult<UserModel>> Post([FromBody] UserModel model)
    {
        return base.Post(model);
    }

    public override Task<ActionResult<ApiResponse<UserModel>>> Put([FromRoute] int id, [FromBody] UserModel model)
    {
        return base.Put(id, model);
    }

    public override Task<ActionResult<ApiResponse<UserModel>>> Delete([FromRoute] int id)
    {
        return base.Delete(id);
    }
}

// That's it! You now have:
// ✅ Complete CRUD API
// ✅ Filtering and sorting
// ✅ Pagination
// ✅ Entity-to-Model and Model-to-Entity transformations
// ✅ Transaction support
// ✅ Basic validation support
// ✅ Lifecycle hooks ready to use
// ✅ Exception handling
// ✅ And many more...
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/lneshev/MoravianStar/blob/main/LICENSE) file for details.

## Contributing

Contributions, issues and feedbacks are welcome! Please feel free to submit an issue, pull request or start a discussion in the [repository](https://github.com/lneshev/MoravianStar).

## Support

- 📖 [Documentation](https://github.com/lneshev/MoravianStar)
- 💡 [Demo Repository](https://github.com/lneshev/MoravianStar-Demo)
- 🐛 [Issue Tracker](https://github.com/lneshev/MoravianStar/issues)