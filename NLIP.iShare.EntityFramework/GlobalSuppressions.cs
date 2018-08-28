
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Critical Code Smell",
    "S2339:Public constant members should not be used",
    Justification = "Used in switch statements",
    Scope = "member", Target = "~F:NLIP.iShare.EntityFramework.Environments.Acc")]

[assembly: SuppressMessage("Critical Code Smell",
    "S2339:Public constant members should not be used",
    Justification = "Used in switch statements",
    Scope = "member", Target = "~F:NLIP.iShare.EntityFramework.Environments.Qa")]

[assembly: SuppressMessage("Critical Code Smell",
    "S2339:Public constant members should not be used",
    Justification = "Used in switch statements",
    Scope = "member", Target = "~F:NLIP.iShare.EntityFramework.Environments.Prod")]

[assembly: SuppressMessage("Critical Code Smell",
    "S2339:Public constant members should not be used",
    Justification = "Used in switch statements",
    Scope = "member", Target = "~F:NLIP.iShare.EntityFramework.Environments.Dev")]

