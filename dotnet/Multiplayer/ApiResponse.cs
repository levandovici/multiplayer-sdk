using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer
{
    /// <summary>
    /// Base class for all API responses. Provides success status and error information.
    /// </summary>
    public abstract class ApiResponse
    {
        /// <summary>
        /// Indicates whether the API request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if the request failed, null otherwise.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// True when the authenticated player's account is inside the deletion
        /// grace window — every player-scoped endpoint then fails with this
        /// flag set. Restore via Players.RestoreAccountAsync before Purge_after.
        /// </summary>
        public bool Deletion_pending { get; set; }

        /// <summary>
        /// UTC timestamp (ISO 8601) after which an account pending deletion is
        /// permanently purged unless restored. Empty when not pending.
        /// </summary>
        public string Purge_after { get; set; } = string.Empty;
    }

    /// <summary>
    /// Generic base class for API responses with typed error handling.
    /// Converts error strings to typed enums for better error management.
    /// </summary>
    /// <typeparam name="TError">The error enum type, must be an Enum and IConvertible.</typeparam>
    public abstract class ApiResponse<TError> : ApiResponse where TError : Enum, IConvertible
    {
        /// <summary>
        /// Gets the error type as an enum by converting the Error string.
        /// </summary>
        public TError ErrorType => ErrorConverter.ConvertToEnum<TError>(Error ?? string.Empty);

        /// <summary>
        /// Gets a user-friendly error message based on the error type.
        /// </summary>
        public string ErrorMessage => ErrorConverter.GetErrorMessage(ErrorType);
    }
}
