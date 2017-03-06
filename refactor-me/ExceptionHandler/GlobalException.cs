using System;

namespace ProductAPI.ExceptionHandler
{
	public class GlobalException : Exception
	{
		public GlobalException()
		{
		}

		public GlobalException(string message): base(message)
		{
		}

		public GlobalException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}