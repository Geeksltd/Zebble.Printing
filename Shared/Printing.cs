namespace Zebble
{
    using System;
    using System.IO;

    public partial class Printing
    {
        static FileInfo GetFile(string path)
        {
            if (path.IsUrl())
            {
                var ext = "png";
                if (path.ToLower().EndsWith(".webp")) ext = "webp";
                if (path.ToLower().EndsWithAny(".jpg", "jpeg")) ext = "jpg";

                path = path.ToIOSafeHash();
                path = Device.IO.Cache.GetFile($"{path}.{ext}").FullName;
            }

            return Device.IO.File(path);
        }
    }
}
