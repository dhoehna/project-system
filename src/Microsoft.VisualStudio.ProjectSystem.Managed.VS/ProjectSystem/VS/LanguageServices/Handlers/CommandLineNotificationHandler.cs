﻿// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using Microsoft.VisualStudio.Composition;
using Microsoft.VisualStudio.LanguageServices.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.LanguageServices.FSharp;

namespace Microsoft.VisualStudio.ProjectSystem.LanguageServices.Handlers;

/// <summary>
///     An indirection that sends design-time build results in the form of command-line arguments to the F# language-service.
/// </summary>
/// <remarks>
///     This indirection is needed because Microsoft.VisualStudio.ProjectSystem.FSharp does not have InternalsVisibleTo access to Roslyn.
/// </remarks>
[Export(typeof(IWorkspaceUpdateHandler))]
[method: ImportingConstructor]
internal class CommandLineNotificationHandler(UnconfiguredProject project) : IWorkspaceUpdateHandler, ICommandLineHandler
{
    /// <remarks>
    /// See <see cref="FSharpCommandLineParserService.HandleCommandLineNotifications"/> for an example of this export.
    /// </remarks>
    [ImportMany]
    public OrderPrecedenceImportCollection<Action<string?, BuildOptions, BuildOptions>> CommandLineNotifications { get; } = new(projectCapabilityCheckProvider: project);

    public void Handle(IWorkspaceProjectContext context, IComparable version, BuildOptions added, BuildOptions removed, ContextState state, IManagedProjectDiagnosticOutputService logger)
    {
        foreach (Action<string?, BuildOptions, BuildOptions> value in CommandLineNotifications.ExtensionValues())
        {
            value(context.BinOutputPath, added, removed);
        }
    }
}
