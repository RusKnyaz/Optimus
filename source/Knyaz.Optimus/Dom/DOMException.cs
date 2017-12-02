using System;

namespace Knyaz.Optimus.Dom
{
	/// <summary>
	/// Represents DOM exception which occurs as a result of calling a method or accessing a property of a web API
	/// </summary>
	public class DOMException : Exception
	{
		private readonly Codes _code;

		public enum Codes
		{
			/// <summary>
			/// The index is not in the allowed range. For example, this can be thrown by Range object. (Legacy code value: 1 and legacy constant name: INDEX_SIZE_ERR) 
			/// </summary>
			IndexSizeError = 1,

			/// <summary>
			/// The node tree hierarchy is not correct. (Legacy code value: 3 and legacy constant name: HIERARCHY_REQUEST_ERR)
			/// </summary>
			HierarchyRequestError = 3,

			/// <summary>
			///The object is in the wrong Document. (Legacy code value: 4 and legacy constant name: WRONG_DOCUMENT_ERR) 
			/// </summary>
			WrongDocumentError = 4,

			/// <summary>
			/// The string contains invalid characters. (Legacy code value: 5 and legacy constant name: INVALID_CHARACTER_ERR)
			/// </summary>
			InvalidCharacterError = 5,

			/// <summary>
			/// The object can not be modified. (Legacy code value: 7 and legacy constant name: NO_MODIFICATION_ALLOWED_ERR)
			/// </summary>
			NoModificationAllowedError = 7,

			/// <summary>
			/// The object can not be found here. (Legacy code value: 8 and legacy constant name: NOT_FOUND_ERR)
			/// </summary>
			NotFoundError = 8,

			/// <summary>
			/// The operation is not supported. (Legacy code value: 9 and legacy constant name: NOT_SUPPORTED_ERR)
			/// </summary>
			NotSupportedError = 9,

			/// <summary>
			/// The object is in an invalid state. (Legacy code value: 11 and legacy constant name: INVALID_STATE_ERR)
			/// </summary>
			InvalidStateError = 11,

			/// <summary>
			/// The string did not match the expected pattern. (Legacy code value: 12 and legacy constant name: SYNTAX_ERR)
			/// </summary>
			SyntaxError = 12,

			/// <summary>
			/// The object can not be modified in this way. (Legacy code value: 13 and legacy constant name: INVALID_MODIFICATION_ERR)
			/// </summary>
			InvalidModificationError = 13,

			/// <summary>
			/// The operation is not allowed by Namespaces in XML. (Legacy code value: 14 and legacy constant name: NAMESPACE_ERR)
			/// </summary>
			NamespaceError = 14,

			/// <summary>
			/// The object does not support the operation or argument. (Legacy code value: 15 and legacy constant name: INVALID_ACCESS_ERR)
			/// </summary>
			InvalidAccessError = 15,

			/// <summary>
			/// The type of the object does not match the expected type. (Legacy code value: 17 and legacy constant name: TYPE_MISMATCH_ERR) This value is deprecated, the JavaScript TypeError exception is now raised instead of a DOMException with this value.
			/// </summary>
			TypeMismatchError = 17,

			/// <summary>
			/// The operation is insecure. (Legacy code value: 18 and legacy constant name: SECURITY_ERR)
			/// </summary>
			SecurityError = 18,

			/// <summary>
			/// A network error occurred. (Legacy code value: 19 and legacy constant name: NETWORK_ERR)
			/// </summary>
			NetworkError = 19,

			/// <summary>
			/// The operation was aborted. (Legacy code value: 20 and legacy constant name: ABORT_ERR)
			/// </summary>
			AbortError = 20,

			/// <summary>
			/// The given URL does not match another URL. (Legacy code value: 21 and legacy constant name: URL_MISMATCH_ERR)
			/// </summary>
			URLMismatchError = 21,

			/// <summary>
			/// The quota has been exceeded. (Legacy code value: 22 and legacy constant name: QUOTA_EXCEEDED_ERR)
			/// </summary>
			QuotaExceededError = 22,

			/// <summary>
			/// The operation timed out. (Legacy code value: 23 and legacy constant name: TIMEOUT_ERR)
			/// </summary>
			TimeoutError = 23,

			/// <summary>
			/// The node is incorrect or has an incorrect ancestor for this operation. (Legacy code value: 24 and legacy constant name: INVALID_NODE_TYPE_ERR)
			/// </summary>
			InvalidNodeTypeError = 24,

			/// <summary>
			/// The object can not be cloned. (Legacy code value: 25 and legacy constant name: DATA_CLONE_ERR)
			/// </summary>
			DataCloneError = 25,

			/// <summary>
			/// The encoding or decoding operation failed.
			/// </summary>
			EncodingError,

			/// <summary>
			/// The input/output read operation failed.
			/// </summary>
			NotReadableError,

			/// <summary>
			/// The operation failed for an unknown transient reason (e.g. out of memory).
			/// </summary>
			UnknownError,

			/// <summary>
			/// A mutation operation in a transaction failed because a constraint was not satisfied.
			/// </summary>
			ConstraintError,

			/// <summary>
			/// Provided data is inadequate.
			/// </summary>
			DataError,

			/// <summary>
			/// A request was placed against a transaction which is currently not active, or which is finished.
			/// </summary>
			TransactionInactiveError,

			/// <summary>
			/// The mutating operation was attempted in a "readonly" transaction.
			/// </summary>
			ReadOnlyError,

			/// <summary>
			/// An attempt was made to open a database using a lower version than the existing version (No legacy code value and constant name).
			/// </summary>
			VersionError,

			/// <summary>
			/// The operation failed for an operation-specific reason.
			/// </summary>
			OperationError,

			/// <summary>
			/// The request is not allowed by the user agent or the platform in the current context, possibly because the user denied permission.
			/// </summary>
			NotAllowedError
		}

		internal DOMException(Codes code)
		{
			_code = code;
			Name = Enum.GetName(typeof(Codes), code);
		}

		public int Code => (int)_code;
		public string Name { get; }
	}
}
