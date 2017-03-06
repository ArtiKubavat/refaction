using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

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