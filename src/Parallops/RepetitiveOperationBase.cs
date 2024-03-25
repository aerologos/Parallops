using System;
using System.Threading;

namespace Parallops
{
    /// <summary>
    /// Provides the baseline for operations repeatedly performing within an isolated thread.
    /// </summary>
    public abstract class RepetitiveOperationBase : OperationBase
    {
        private readonly int _downtime;

        /// <summary>
        /// The source of the <see cref="CancellationToken"/>.
        /// </summary>
        protected CancellationTokenSource TokenSource;

        /// <summary>
        /// Baseline initialization of the instance.
        /// </summary>
        /// <param name="downtime">The downtime of the operation in milliseconds between each iteration.</param>
        protected RepetitiveOperationBase(int downtime = 0)
        {
            _downtime = downtime;
        }

        /// <inheritdoc/>
        protected override void PerformStart()
        {
            var staleTokenSource = TokenSource;
            TokenSource = new CancellationTokenSource();
            staleTokenSource?.Dispose();

            ThreadPool.QueueUserWorkItem(RunUntilCancellation);
        }

        private void RunUntilCancellation(object stateInfo)
        {
            while (!Disposed && !TokenSource.IsCancellationRequested)
            {
                try
                {
                    RepeatOperation();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                if (Running) Thread.Sleep(_downtime);
            }
        }

        /// <summary>
        /// Provides the place for the polymorphic operation repeat.
        /// </summary>
        protected abstract void RepeatOperation();

        /// <inheritdoc/>
        protected override void PerformStop()
        {
            TokenSource?.Cancel();
        }

        /// <inheritdoc/>
        protected override void PerformDisposal()
        {
            TokenSource?.Dispose();
        }
    }
}