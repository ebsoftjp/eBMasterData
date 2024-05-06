using System.Threading.Tasks;

namespace eBMasterData
{
    public static class Utility
    {
        public static async Task Reload(object obj, System.Func<int, int, string, bool> indicatorFunc)
        {
            var reader = new ReaderForRuntime(indicatorFunc);
            await reader.CreateFileList();
            await reader.ReadText();
            reader.ParseData();
            obj.GetType().GetMethod("Convert2").Invoke(obj, new object[] { reader.ParsedTables, reader.ParsedValues });
        }
    }
}
