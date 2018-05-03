# Tabula

An integration testing framework being built as a replacement for an
in-house testing system which has served faithfully and is showing its age.

### Mechanics

Intended to integrate with Visual Studio 2017,
so that when a scenario (a Tabula test file)
is saved with a .tab extension,
Studio will send the scenario to the transpiler,
which will generate a C# class,
which will be returned to Studio,
which will install it as a dependent item under the .tab scenario,
in much the way .aspx.designer.cs files are automatically generated.
