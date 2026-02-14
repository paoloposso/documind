using Documind.Application.Abstractions;

namespace Documind.Application;

public class KnowledgeSeeder(IIngestionService ingestionService) : IKnowledgeSeeder
{
    public async Task SeedAsync()
    {
        var facts = new List<(string Text, string Source)>
        {
            ("C# (C-sharp) is a modern, object-oriented, and type-safe programming language developed by Microsoft within the .NET framework. It is widely used for developing Windows desktop applications, web applications, and games.", "C# Programming"),
            ("Asynchronous programming in C# primarily uses the `async` and `await` keywords to write non-blocking code, improving application responsiveness. Methods marked `async` can use `await` to pause execution until an awaited task completes.", "C# Asynchronous Programming"),
            ("LINQ (Language Integrated Query) in C# provides a powerful, uniform query syntax for data from various sources like collections, databases, and XML. It simplifies data manipulation and retrieval through expressive query expressions.", "C# LINQ"),
            ("Dependency Injection (DI) is a design pattern used in C# to achieve Inversion of Control (IoC) between classes and their dependencies. It promotes loose coupling and makes code easier to test and maintain, often facilitated by frameworks like ASP.NET Core's built-in DI container.", "C# Dependency Injection"),
            ("Generics in C# allow you to define classes, interfaces, and methods with placeholders for the type of data they store or operate on. This enables type-safe code without limiting it to specific data types, enhancing reusability and performance.", "C# Generics"),
            ("Delegates in C# are type-safe function pointers that allow methods to be passed as arguments or returned from other methods. They are fundamental to event handling and callback mechanisms, enabling flexible and extensible code designs.", "C# Delegates"),
            ("ASP.NET Core is an open-source, cross-platform framework for building modern, cloud-based, internet-connected applications using C#. It supports various development models, including MVC, Razor Pages, and Web APIs.", "C# ASP.NET Core"),
            ("Garbage Collection in C# is handled by the .NET runtime's automatic memory management system. It automatically reclaims memory occupied by objects that are no longer referenced by the application, reducing memory leaks and simplifying development.", "C# Memory Management"),
            ("Structs in C# are value types, unlike classes which are reference types. They are typically used for small data structures that do not require inheritance, offering performance benefits by being allocated on the stack rather than the heap (unless boxed).", "C# Structs vs Classes"),
            ("Interfaces in C# define a contract that implementing classes or structs must adhere to. They specify a set of members (methods, properties, events, indexers) that the implementing type must provide, promoting polymorphism and abstract design.", "C# Interfaces"),
            ("Extension methods in C# allow you to add new methods to existing types without modifying the original type itself. They are static methods called as if they were instance methods on the extended type, improving code readability and reusability.", "C# Extension Methods"),
            ("Reflection in C# provides the ability to examine and manipulate metadata about types, methods, and properties at runtime. It's used for dynamic programming scenarios, such as loading assemblies, inspecting types, and invoking members dynamically.", "C# Reflection"),
            ("Exception handling in C# uses `try-catch-finally` blocks to manage errors that occur during program execution. `try` encloses code that might throw an exception, `catch` handles specific exceptions, and `finally` ensures cleanup code always runs.", "C# Exception Handling"),
            ("Nullable value types in C# allow value types (like `int`, `bool`, `DateTime`) to be assigned `null`. This is achieved by using the `?` suffix (e.g., `int?`) and is useful when a value might legitimately be absent.", "C# Nullable Types"),
            ("Records in C# are reference types that provide encapsulated data with value semantics. They are immutable by default and are ideal for modeling data transfer objects (DTOs) and other data-centric types.", "C# Records"),
            ("Pattern matching in C# allows for concise and expressive conditional logic based on the shape or properties of an object. It enhances readability and reduces boilerplate code for type checking and casting.", "C# Pattern Matching"),
            ("Top-level statements in C# simplify console applications by removing the need for explicit `Main` methods and class declarations. This makes C# programs more compact and easier to read for simple scenarios.", "C# Top-Level Statements"),
            ("Global using directives in C# allow you to declare `using` statements once, at the project level, making them applicable to all files within the project. This reduces repetitive `using` declarations in individual files.", "C# Global Using"),
            ("Collections in C# provide flexible ways to store and manage groups of objects, such as `List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`, and `Queue<T>`. They are essential for efficient data manipulation.", "C# Collections"),
            ("Polymorphism in C# allows objects of different classes to be treated as objects of a common base class or interface. It enables writing flexible and extensible code, primarily achieved through inheritance and interfaces.", "C# Polymorphism"),
            ("Encapsulation in C# is the bundling of data and methods that operate on the data within a single unit or class, restricting direct access to some of the object's components. It promotes data integrity and modularity.", "C# Encapsulation"),
            ("Sealed classes in C# prevent other classes from inheriting from them, often used to prevent unintended modifications to class behavior or to optimize performance. Structs are implicitly sealed.", "C# Sealed Classes"),
            ("Events in C# provide a mechanism for classes to notify other classes or objects when something of interest occurs. They are built on delegates and enable a publish-subscribe communication pattern.", "C# Events"),
            ("Attributes in C# are declarative tags that can be placed on code elements like classes, methods, and properties to associate metadata with them. This metadata can be queried at runtime using reflection.", "C# Attributes"),
            ("Tasks and Task Parallel Library (TPL) in C# offer a robust way to implement parallelism and concurrency. `Task` represents an asynchronous operation, while TPL simplifies parallel execution of computationally intensive operations.", "C# Concurrency"),
            ("Memory management in C# is primarily automatic through the Garbage Collector, but `IDisposable` and the `using` statement are crucial for deterministic release of unmanaged resources like file handles or database connections.", "C# Memory Management (Advanced)"),
            ("Immutable collections in C# provide thread-safe collections that cannot be modified after they are created. Any operation that would modify the collection returns a new collection, useful in concurrent programming.", "C# Immutable Collections"),
            ("Ahead-of-Time (AOT) Compilation in C# translates intermediate language (IL) code into native machine code before execution, typically during publishing. This can lead to faster startup times and improved runtime performance compared to Just-in-Time (JIT) compilation.", "C# AOT Compilation"),
            ("IEnumerable<T> in C# is an interface that defines a method for iterating over a collection of a specified type. It allows for deferred execution and is the foundation for LINQ queries, providing a lightweight way to work with sequences of data.", "C# IEnumerable"),
            ("IAsyncEnumerable<T> in C# is an interface that enables asynchronous iteration over sequences of data. It works with `async await` and `yield return` to produce elements asynchronously, which is particularly useful for streaming data or I/O-bound operations.", "C# IAsyncEnumerable"),
            ("Specialized collections in C# include `Stack<T>` (LIFO), `Queue<T>` (FIFO), `LinkedList<T>` (doubly linked list), and `SortedList<TKey, TValue>` (sorted key-value pairs). These provide optimized data structures for specific use cases beyond generic lists and dictionaries.", "C# Specialized Collections")
        };

        foreach (var (text, source) in facts)
        {
            await ingestionService.IngestAsync(text, source);
        }
    }
}