using System;

namespace Parallops
{
    /// <summary>
    /// Describes the contract for operations with execution state.
    /// </summary>
    public interface IOperation : IDisposable
    {
        /// <summary>
        /// Gets the flag indicating whether the operation is started or not.
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// Starts the operation.
        /// </summary>
        void Start();
        
        /// <summary>
        /// Stops the operation.
        /// </summary>
        void Stop();
    }
}