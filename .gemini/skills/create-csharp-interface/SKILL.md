---
name: create-csharp-interface
description: Creates a new, syntactically correct C# interface file. Use when asked to create a C# interface, generate an interface file, or create boilerplate for an interface.
---

# Create C# Interface

## Workflow

1.  **Identify Interface Name:** If the user has not provided a name for the interface, ask for one. The file name will be `I<InterfaceName>.cs` or `<InterfaceName>.cs` if it already starts with 'I'.

2.  **Determine Namespace:** Determine the namespace for the interface. A good way to do this is to find the project's root namespace from the `.csproj` file in the current directory or parent directories. Look for a `<RootNamespace>` element. If not found, use the name of the directory containing the new interface file.

3.  **Identify Methods:**
    *   Ask the user for the methods to include in the interface. For each method, you need a name and a return type.
    *   Interface methods in C# do not have access modifiers (they are implicitly public) and do not have an implementation body.
    *   The user can provide one or many methods, or none.

4.  **Read Template:** Read the content of the `assets/interface.cs.template` file included in this skill.

5.  **Build Methods String:**
    *   Iterate through the methods identified in Step 3.
    *   For each method, create a signature string in the format `    {{RETURN_TYPE}} {{METHOD_NAME}}();` (e.g., `    string GetName();`). Note the indentation.
    *   Join these strings together with a newline character. If there are no methods, this will be an empty string.

6.  **Replace Placeholders:** In the template content, perform the following replacements:
    *   Replace `{{NAMESPACE}}` with the namespace determined in Step 2.
    *   Replace `{{INTERFACE_NAME}}` with the interface name from Step 1.
    *   Replace `{{METHODS}}` with the combined methods string you built in Step 5.

7.  **Write File:** Write the final content to a new file named `<InterfaceName>.cs` in the appropriate directory.