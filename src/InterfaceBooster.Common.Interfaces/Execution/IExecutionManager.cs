using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.Execution.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Execution
{
    /// <summary>
    /// Initializes all needed components and handels the execution of an interface definition or parts of it like Jobs or single Synery code files.
    /// </summary>
    public interface IExecutionManager : IDisposable
    {
        /// <summary>
        /// Gets a flag that indicates whether Initialize() already has been called.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the current Interface Definition.
        /// </summary>
        InterfaceDefinitionData InterfaceDefinitionData { get; }

        /// <summary>
        /// Gets or sets the current runtime environment variables.
        /// </summary>
        ExecutionVariables EnvironmentVariables { get; set; }

        /// <summary>
        /// Runs all Jobs from the current Interface Definition.
        /// </summary>
        /// <returns></returns>
        ExecutionResult RunAllJobs();

        /// <summary>
        /// Runs the Job with the given name.
        /// </summary>
        /// <param name="name">The full name (case insensitive) of an existing Job from the current Interface Definition.</param>
        /// <returns></returns>
        ExecutionResult RunJob(string name);

        /// <summary>
        /// Runs the Job with the given GUID.
        /// </summary>
        /// <param name="guid">The GUID of an existing Job from the current Interface Definition.</param>
        /// <returns></returns>
        ExecutionResult RunJob(Guid guid);

        /// <summary>
        /// Runs a single code file without include files.
        /// </summary>
        /// <param name="relativeFilePath">The relative file path starting from the code directory.</param>
        /// <returns></returns>
        ExecutionResult RunSingleCodeFile(string relativeFilePath);

        /// <summary>
        /// Initialize the interface definition, the database, the plugin managers, the SyneryMemory and the SyneryInterpreter
        /// This method must be called before running a job.
        /// </summary>
        /// <param name="environmentVariables">At least Broadcaster and InterfaceDefinitionDirectoryPath are required!</param>
        /// <returns>true = success / false = error (see broadcasted messages for details)</returns>
        bool Initialize(ExecutionVariables environmentVariables);
    }
}
