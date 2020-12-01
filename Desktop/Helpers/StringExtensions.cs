using System.Text.RegularExpressions;

namespace RemoteVideoPlayer.Helpers
{
	public static class StringExtensions
	{
		public static string RemoveHtmlTags(this string input)
		{
			const string TEMPLATE = @"<.*?>|</.*?>";

			return Regex.Replace(input, TEMPLATE, "");
		}
	}
}