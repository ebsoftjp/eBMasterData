using System.Linq;

namespace eBMasterData
{
    public class LoadedText
    {
        private readonly string name;
        private readonly string text;
        private readonly int format;

        public string Name => name;
        public string Text => text;
        public int Format => format;

        public LoadedText(string name, string text, Settings settings)
        {
            this.name = name;
            this.text = text;
            format = settings.Configs.Contains(name) ? 1 : 0;
        }

        public string[] ToLines()
        {
            return null;
        }
    }
}
