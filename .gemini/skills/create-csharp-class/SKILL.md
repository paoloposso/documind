---
name: create-csharp-class
description: Creates a new C# class file from a template. Use when asked to create a new C# class, generate a class file, or create boilerplate C# code.
---

# Create C# Class

## Workflow

1.  **Identify Class Name:** If the user has not provided a name for the class, ask for one. The file name will be `<ClassName>.cs`.

2.  **Determine Namespace:** Determine the namespace for the class. A good way to do this is to find the project's root namespace from the `.csproj` file in the current directory or parent directories. Look for a `<RootNamespace>` element. If not found, use the name of the directory containing the new class file.

3.  **Read Template:** Read the content of the `assets/class.cs.template` file included in this skill.

4.  **Replace Placeholders:** In the template content, perform the following replacements:
    *   Replace `{{NAMESPACE}}` with the namespace determined in Step 2.
    *   Replace `{{CLASS_NAME}}` with the class name from Step 1.

5.  **Write File:** Write the final content to a new file named `<ClassName>.cs` in the appropriate directory.