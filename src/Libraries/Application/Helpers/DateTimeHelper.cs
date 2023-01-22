using System;
using Application.Exceptions;

namespace Application.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetDateTimeFromTimeStamp(string timeStamp)
        {
            if (string.IsNullOrEmpty(timeStamp) || !long.TryParse(timeStamp, out long timeStampValue))
                throw new ProcessCancellationException("Message Time stamp is empty or invalid");

            DateTime origin = new(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timeStampValue).AddHours(1);
        }
    }
}

