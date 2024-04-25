using System.Collections.Generic;

namespace EbMasterData.Editor
{
    public class ConvertEnums
    {
        private readonly Settings settings;

        public ConvertEnums(Settings settings)
        {
            this.settings = settings;
        }

        public List<string> CreateMasterDataEnums(List<ReaderForEditor.KeysData2> data)
        {
            var res = new List<string>
            {
                $"// Auto create by EbMasterData.ConvertEnums",
                $"namespace {settings.NamespaceName}",
                $"{{",
            };

            foreach (var v in settings.Enums)
            {
                res.AddRange(new List<string>
                {
                    $"    public enum {v}",
                    $"    {{",
                    $"    }}",
                });
            }

            res.AddRange(new List<string>
            {
                $"}}",
                $"",
            });

            return res;
        }
    }
}
