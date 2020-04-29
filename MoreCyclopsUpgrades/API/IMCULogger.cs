using System;

namespace MoreCyclopsUpgrades.API
{
    /// <summary>
    /// Defines a set of logging APIs that other mods can use.<para/>
    /// Debug level logs will only be printed of MCU's debug logging is enabled.
    /// </summary>
    public interface IMCULogger
    {
        /// <summary>
        /// Gets a value indicating whether calls into <see cref="Debug"/> are handled or ignored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if debug level logs enabled; otherwise, <c>false</c>.
        /// </value>
        bool DebugLogsEnabled { get; }

        /// <summary>
        /// Writes an INFO level log to the log file. Can be optionally printed to screen.
        /// </summary>
        /// <param name="logmessage">The log message to write.</param>
        /// <param name="showOnScreen">if set to <c>true</c> the log message will show on screen.</param>
        void Info(string logmessage, bool showOnScreen = false);

        /// <summary>
        /// Writes a WARN level log to the log file. Can be optionally printed to screen.
        /// </summary>
        /// <param name="logmessage">The log message to write.</param>
        /// <param name="showOnScreen">if set to <c>true</c> the log message will show on screen.</param>
        void Warning(string logmessage, bool showOnScreen = false);

        /// <summary>
        /// Writes an ERROR level log to the log file. Can be optionally printed to screen.
        /// </summary>
        /// <param name="logmessage">The log message to write.</param>
        /// <param name="showOnScreen">if set to <c>true</c> the log message will show on screen.</param>
        void Error(string logmessage, bool showOnScreen = false);

        /// <summary>
        /// Writes <see cref="Exception" /> to an ERROR level log to file.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="logmessage">The optional additional message.</param>
        void Error(Exception ex, string logmessage = null);

        /// <summary>
        /// Writes an DEBUG level log to the log file if <see cref="DebugLogsEnabled"/> is enabled. Can be optionally printed to screen.<para/>
        /// No action taken when <see cref="DebugLogsEnabled"/> is set to <c>false</c>;
        /// </summary>
        /// <param name="logmessage">The log message to write.</param>
        /// <param name="showOnScreen">if set to <c>true</c> the log message will show on screen.</param>
        void Debug(string logmessage, bool showOnScreen = false);
    }
}
