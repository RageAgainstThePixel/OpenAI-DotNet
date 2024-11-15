// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Extensions
{
    internal static class TaskExtensions
    {
        /// <summary>
        /// Runs <see cref="Task"/> with <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="task">The <see cref="Task"/> to run.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            await using (cancellationToken.Register(state => ((TaskCompletionSource<object>)state).TrySetResult(null), tcs))
            {
                var resultTask = await Task.WhenAny(task, tcs.Task);

                if (resultTask == tcs.Task)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                await task;
            }
        }

        /// <summary>
        /// Runs <see cref="Task{T}"/> with <see cref="CancellationToken"/>.
        /// </summary>
        /// <typeparam name="T">Task return type.</typeparam>
        /// <param name="task">The <see cref="Task{T}"/> to run.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns><see cref="Task{T}"/> result.</returns>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            await using (cancellationToken.Register(state => ((TaskCompletionSource<object>)state).TrySetResult(null), tcs))
            {
                var resultTask = await Task.WhenAny(task, tcs.Task);

                if (resultTask == tcs.Task)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                return await task;
            }
        }
    }
}
