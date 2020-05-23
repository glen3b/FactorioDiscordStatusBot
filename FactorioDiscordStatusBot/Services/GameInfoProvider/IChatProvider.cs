using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FactorioDiscordStatusBot.Services.GameInfoProvider
{
    /// <summary>
    /// Provides chat or other entries from game server logs.
    /// This class is task- and thread-safe.
    /// </summary>
    public interface IChatProvider
    {
        public enum LineRetrieveResult
        {
            /// <summary>
            /// A line was successfully obtained.
            /// </summary>
            Success,
            /// <summary>
            /// No lines were available, but the underlying data stream may provide more data.
            /// </summary>
            NotAvailable,
            /// <summary>
            /// The underlying data stream will not provide more data.
            /// </summary>
            Closed
        }

        /// <summary>
        /// Gets a line from the server chat log, blocking if none are available.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown, or marked as an error in the returned task, if the underlying data store
        /// ceases to provide data, for instance due to a connection loss.
        /// </exception>
        /// <returns>A task which resolves to a server chat log.</returns>
        Task<string> GetLine();

        /// <summary>
        /// Attempts to get a chat line from the server chat log without blocking.
        /// </summary>
        /// <remarks>
        /// This method reserves the right to acquire locks synchronously, provided such locks are not expected to be held for
        /// long periods of time. This may 
        /// </remarks>
        /// <param name="chatLine">The parameter through which the returned line is retrieved, or null in event of failure.</param>
        /// <returns>A <see cref="LineRetrieveResult"/> indicating the result of this attempt to retrieve that line.</returns>
        LineRetrieveResult TryGetLine(out string chatLine);
    }
}
