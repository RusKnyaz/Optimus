using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Knyaz.Optimus.Tools
{
	/// <summary>
	/// Contains cookie parsing method.
	/// </summary>
	internal static class CookieParser
	{
		/// <summary>
		/// Converts string representation of cookie to <see cref="Cookie"/> instance.
		/// </summary>
		/// <param name="cookieText">Textual representation of cookie, like 'User=I; Path=/'</param>
		/// <returns></returns>
		public static Cookie FromString(string cookieText)
		{
			if (string.IsNullOrEmpty(cookieText))
				return null;
            
			var pairs = cookieText.Split(new[]{';'}, StringSplitOptions.RemoveEmptyEntries);

			var nameValue = pairs[0].Split('=');
			if (nameValue.Length != 2)
				return null;

			var name = nameValue[0].Trim();
			var value = nameValue[1].Trim();
            
			var cookie = new Cookie(name, value);
			foreach (var keyValueText in pairs.Skip(1))
			{
				var keyValuePair = keyValueText.Split('=');
				if(keyValuePair.Length == 0)
					continue;

				var key = keyValuePair[0].Trim();
				switch (key)
				{
					case "Path": cookie.Path = keyValuePair[1].Trim();
						break;
					case "Domain": cookie.Domain = keyValuePair[1].Trim();
						break;
					case "Expires": 
						cookie.Expires = DateTime.ParseExact(keyValuePair[1].Trim(),
							"ddd, dd MMM yyyy HH:mm:ss Z", CultureInfo.InvariantCulture)
							.ToUniversalTime();
						break;
					case "Secure" : cookie.Secure = true;
						break;
					case "HttpOnly" : cookie.HttpOnly = true;
						break;
				}
			}

			return cookie;
		}

		public static string ToString(CookieCollection cookies)
		{
			var sb = new StringBuilder();
			foreach (Cookie cookie in cookies)
			{
				if(cookie.Expired)
					continue;
				
				sb.Append(cookie.Name);
				sb.Append("=");
				sb.Append(cookie.Value);
				if (cookie.Secure)
				{
					sb.Append("; ");
					sb.Append("Secure");
				}

				if (cookie.HttpOnly)
				{
					sb.Append("; ");
					sb.Append("HttpOnly");
				}

				sb.Append("; ");
			}
			return sb.ToString().TrimEnd(';',' ');
		}
	}
}