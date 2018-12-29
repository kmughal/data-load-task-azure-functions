namespace LoadBikePoints
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public static class StreamHelpers
    {
        public static Task<string> ReadXmlContentsFromStreamAsync(Stream stream)
        {  
            return Task.Factory.StartNew(() =>
            {
                var ms = new MemoryStream();
               stream.CopyTo(ms);
                return ms.ToArray();
            }).ContinueWith(task =>
            {
                var streamArray = task.Result;
                return Encoding.UTF8.GetString(streamArray);
            });
        }
    }
}
