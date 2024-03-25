using System;
using NLog;
using Extype;

namespace Parallops
{
    /// <summary>
    /// Provides the baseline for operations with execution switch.
    /// </summary>
    public abstract class OperationBase : IOperation
    {
        private readonly object _sync = new object();
        
        /// <summary>
        /// Gets the flag indicating whether the instance of this class is disposed or not.
        /// </summary>
        protected bool Disposed { get; private set; }
        
        /// <summary>
        /// Gets the instance of the logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <inheritdoc/>
        public bool Running { get; private set; }

        /// <summary>
        /// Baseline initialization of the instance.
        /// </summary>
        protected OperationBase()
        {
            Logger = LogManager.CreateNullLogger();
        }

        /// <summary>
        /// Baseline initialization of the instance.
        /// </summary>
        /// <param name="logger">The instance of logger.</param>
        protected OperationBase(ILogger logger)
        {
            Logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public void Start()
        {
            this.ThrowIfDisposed(Disposed);

            lock (_sync)
            {
                if (Running) return;

                try
                {
                    PerformStart();
                    Running = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// Provides the place for the polymorphic operation start.
        /// </summary>
        protected abstract void PerformStart();

        /// <inheritdoc/>
        public void Stop()
        {
            this.ThrowIfDisposed(Disposed);

            lock (_sync)
            {
                if (!Running) return;

                try
                {
                    PerformStop();
                    Running = false;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// Provides the place for the polymorphic operation stop.
        /// </summary>
        protected abstract void PerformStop();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (Disposed) return;

            PerformDisposal();
            GC.SuppressFinalize(this);

            Disposed = true;
        }

        /// <summary>
        /// Provides the place for the polymorphic object disposal.
        /// </summary>
        protected abstract void PerformDisposal();
    }
}