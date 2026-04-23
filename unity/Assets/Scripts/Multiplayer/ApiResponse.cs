using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer
{
    public abstract class ApiResponse<TError> : ApiResponse where TError : Enum, IConvertible
    {
        /// <summary>
        /// Gets the error type as an enum by converting the Error string
        /// </summary>
        public TError ErrorType => ErrorConverter.ConvertToEnum<TError>(error ?? string.Empty);

        /// <summary>
        /// Gets a user-friendly error message based on the error type
        /// </summary>
        public string ErrorMessage => ErrorConverter.GetErrorMessage(ErrorType);
    }

    [System.Serializable]
    public abstract class ApiResponse
    {
        public bool success;
        public string error;
    }
}
