using System;
namespace Application.Helpers
{
	public static class StringExtension
	{
		public static string WordWrapContent(this String content)
		{
			if(!string.IsNullOrEmpty(content))
			{
			  content =	content.Replace("\n", Environment.NewLine);
              content =  content.Replace("/n", Environment.NewLine);
            }

            return content;
        }
	}
}

