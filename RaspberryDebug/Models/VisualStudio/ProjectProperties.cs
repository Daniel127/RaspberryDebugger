﻿//-----------------------------------------------------------------------------
// FILE:	    ProjectProperties.cs
// CONTRIBUTOR: Jeff Lill
// COPYRIGHT:   Open Source
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using EnvDTE;
using EnvDTE80;

using Task = System.Threading.Tasks.Task;
using System.Diagnostics.Contracts;
using Newtonsoft.Json.Linq;

namespace RaspberryDebug
{
    /// <summary>
    /// The Visual Studio <see cref="Project"/> class properties can only be
    /// accessed from the UI thread, so we'll use this class to capture the
    /// properties we need on a UI thread so we can use them later on other
    /// threads.
    /// </summary>
    internal class ProjectProperties
    {
        //--------------------------------------------------------------------
        // Static members

        /// <summary>
        /// Creates a <see cref="ProjectProperties"/> instance holding the
        /// necessary properties from a <see cref="Project"/>.  This must
        /// be called on a UI thread.
        /// </summary>
        /// <param name="project">The source project.</param>
        /// <returns>The cloned <see cref="ProjectProperties"/>.</returns>
        public static ProjectProperties CopyFrom(Project project)
        {
            Covenant.Requires<ArgumentNullException>(project != null, nameof(project));
            ThreadHelper.ThrowIfNotOnUIThread();

            //Read the project file to find out more about the project.

            var projectFolder = Path.GetDirectoryName(project.FullName);
            var projectFile   = File.ReadAllText(project.FullName);
            var isNetCore     = true;

            if (projectFile.Trim().StartsWith("<Project "))
            {
                isNetCore = true;
            }
            else
            {
                // This doesn't look like a .NET Core project so it not supported.

                isNetCore = false;
            }

            // Load [Properties/launchSettings.json] if present to obtain the command line
            // arguments and environment variables as well as the target connection.  Note
            // that we're going to use the profile named for the project and ignore any others.

            var launchSettingsPath   = Path.Combine(projectFolder, "Properties", "launchSettings.json");
            var debugHost            = (string)null;
            var commandLineArgs      = (string)null;
            var environmentVariables = new Dictionary<string, string>();

            if (File.Exists(launchSettingsPath))
            {
                var settings = JObject.Parse(File.ReadAllText(launchSettingsPath));
                var profiles = settings.Property("profiles");

                if (profiles != null)
                {
                    foreach (var profile in ((JObject)profiles.Value).Properties())
                    {
                        if (profile.Name == project.Name)
                        {
                            var profileObject              = (JObject)profile.Value;
                            var environmentVariablesObject = (JObject)profileObject.Property("environmentVariables")?.Value;

                            commandLineArgs = (string)profileObject.Property("commandLineArgs")?.Value;

                            if (environmentVariablesObject != null)
                            {
                                // NOTE: The [@RASPBERRY] variable (case insensitive) is reserved and specifies
                                // the connection host when present.  It is never passed to the target program.

                                foreach (var variable in environmentVariablesObject.Properties())
                                {
                                    if (variable.Name.Equals("@RASPBERRY", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        debugHost = (string)variable.Value;
                                    }
                                    else
                                    {
                                        environmentVariables[variable.Name] = (string)variable.Value;
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            // Return the properties.

            return new ProjectProperties()
            {
                Name                 = project.Name,
                FullPath             = project.FullName,
                Configuration        = project.ConfigurationManager.ActiveConfiguration.ConfigurationName,
                IsNetCore            = isNetCore,
                OutputFolder         = Path.Combine(projectFolder, project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString()),
                AssemblyName         = project.Properties.Item("AssemblyName").Value.ToString(),
                DebugHost            = debugHost,
                CommandLineArgs      = commandLineArgs,
                EnvironmentVariables = environmentVariables
            };
        }

        //--------------------------------------------------------------------
        // Instance members

        /// <summary>
        /// Returns the project's friendly name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns the fullly qualfied path to the project file.
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Indicates that the project targets .NET Core rather than the .NET Framework.
        /// </summary>
        public bool IsNetCore { get; private set; }

        /// <summary>
        /// Returns the project's build configuration.
        /// </summary>
        public string Configuration { get; private set; }

        /// <summary>
        /// Returns the fully qualified path to the project's output directory.
        /// </summary>
        public string OutputFolder { get; private set; }

        /// <summary>
        /// Returns the publish runtime.
        /// </summary>
        public string Runtime => "linux-arm";

        /// <summary>
        /// Returns the publication folder.
        /// </summary>
        public string PublishFolder => Path.Combine(OutputFolder, Runtime);

        /// <summary>
        /// Returns the name of the output assembly.
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Returns the host identifying the target Raspberry or <c>null</c> when
        /// the default Raspberry connection should be used.
        /// </summary>
        public string DebugHost { get; private set; }

        /// <summary>
        /// Returns the command line arguments to be passed to the debugged program.
        /// </summary>
        public string CommandLineArgs { get; private set; }

        /// <summary>
        /// Returns the environment variables to be passed to the debugged program.
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables { get; private set; }
    }
}
