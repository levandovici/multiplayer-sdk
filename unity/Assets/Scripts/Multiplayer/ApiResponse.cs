using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer
{
    /// <summary>
    /// Generic base class for API responses with typed error handling in Unity.
    /// Converts error strings to typed enums for better error management.
    /// Uses snake_case property naming for Unity serialization compatibility.
    /// </summary>
    /// <typeparam name="TError">The error enum type, must be an Enum and IConvertible.</typeparam>
    [Serializable]
    public abstract class ApiResponse<TError> : ApiResponse where TError : Enum, IConvertible
    {
        /// <summary>
        /// Gets the error type as an enum by converting the error string.
        /// </summary>
        public TError ErrorType => ErrorConverter.ConvertToEnum<TError>(error ?? string.Empty);

        /// <summary>
        /// Gets a user-friendly error message based on the error type.
        /// </summary>
        public string ErrorMessage => ErrorConverter.GetErrorMessage(ErrorType);
    }

    /// <summary>
    /// Base class for all API responses in Unity.
    /// Provides success status and error information with snake_case naming for Unity serialization.
    /// </summary>
    [System.Serializable]
    public abstract class ApiResponse
    {
        /// <summary>
        /// Indicates whether the API request was successful.
        /// </summary>
        public bool success;

        /// <summary>
        /// Error message if the request failed, null otherwise.
        /// </summary>
        public string error;
    }
}
