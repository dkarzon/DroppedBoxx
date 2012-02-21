using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DroppedBoxx.Properties;
using System.IO;
using System.Text.RegularExpressions;

namespace DroppedBoxx.Code
{
    public static class FileHelpers
    {
        public static Image GetIcon(string extension)
        {
            switch (extension.ToLower())
            {
                case ".exe":
                    return Resources.application_xp;
                case ".cab":
                    return Resources.application_lightning;
                case ".doc":
                case ".docx":
                case ".rtf":
                    return Resources.page_white_word;
                case ".pwi":
                    return Resources.note;
                case ".pdf":
                    return Resources.page_white_acrobat;
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                case ".png":
                    return Resources.picture;
                case ".txt":
                    return Resources.page_white_text;
                case ".zip":
                    return Resources.page_white_zip;
                case ".sln":
                case ".suo":
                case ".csproj":
                case ".vbproj":
                    return Resources.page_white_visualstudio;
                case ".xls":
                case ".xlsx":
                    return Resources.page_white_excel;
                case ".swf":
                case ".fla":
                case ".flv":
                    return Resources.page_white_flash;
                case ".ppt":
                case ".pptx":
                    return Resources.page_white_powerpoint;
                case ".c":
                    return Resources.page_white_c;
                case ".htm":
                case ".html":
                case ".xml":
                case ".aspx":
                case ".ascx":
                    return Resources.page_white_code;
                case ".cpp":
                    return Resources.page_white_cplusplus;
                case ".cs":
                    return Resources.page_white_csharp;
                case ".mdf":
                    return Resources.page_white_database;
                case ".dll":
                    return Resources.page_white_error;
                case ".h":
                    return Resources.page_white_h;
                case ".php":
                    return Resources.page_white_php;
                case ".vbs":
                    return Resources.script;
                case ".mp3":
                case ".wma":
                case ".m3a":
                case ".ape":
                case ".flac":
                case ".aac":
                case ".ogg":
                    return Resources.music;
                case ".avi":
                case ".mpg":
                case ".wmv":
                case ".mpeg":
                case ".mov":
                case ".3gp":
                case ".asf":
                case ".dvr-ms":
                    return Resources.television;
                default:
                    return Resources.page_white;
            }
        }

        public static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = Convert.ToInt32(readStream.Length);
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        public static string ToReadableSize(long input)
        {
            var newInput = input;

            if (newInput < 1100)
            {
                return string.Format("{0:0.00} B", newInput);
            }

            //convert to KB
            newInput = (newInput / 1024);
            if (newInput < 1100)
            {
                return string.Format("{0:0.00} KB", newInput);
            }

            //convert to MB
            newInput = (newInput / 1024);
            if (newInput < 1100)
            {
                return string.Format("{0:0.00} MB", newInput);
            }

            //convert to GB
            newInput = (newInput / 1024);
            if (newInput < 1100)
            {
                return string.Format("{0:0.00} GB", newInput);
            }

            return string.Format("{0:0.00 KB", (input / 1024));
        }


        public static string GetPrettyDate(DateTime d)
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.UtcNow.Subtract(d);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0)
            {
                return null;
            }

            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 minute ago";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} minutes ago",
                        Math.Floor((double)secDiff / 60));
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} hours ago",
                        Math.Floor((double)secDiff / 3600));
                }
            }
            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return string.Format("{0} days ago",
                    dayDiff);
            }
            if (dayDiff < 9)
            {
                return "1 week ago";
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} weeks ago", 
                    Math.Ceiling((double)dayDiff / 7));
            }
            if (dayDiff < 50)
            {
                return "1 Month ago";
            }
            return string.Format("{0} Months ago",
                    Math.Ceiling((double)dayDiff / 30));
        }

    }
}
