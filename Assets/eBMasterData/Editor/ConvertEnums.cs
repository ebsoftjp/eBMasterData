#if UNITY_EDITOR
using System.Collections.Generic;

namespace eBMasterData.Editor
{
    public class ConvertEnums
    {
        private readonly Settings settings;

        public ConvertEnums(Settings settings)
        {
            this.settings = settings;
        }

        public List<string> CreateMasterDataEnums(List<ReaderForEditor.EnumData> enumData)
        {
            var res = new List<string>
            {
                $"// Auto create by eBMasterData.ConvertEnums",
                $"namespace {settings.NamespaceName}",
                $"{{",
            };

            for (int i = 0; i < enumData.Count; i++)
            {
                if (i > 0) res.Add("");

                var v = enumData[i];
                res.AddRange(new List<string>
                {
                    $"    public enum {v.name}",
                    $"    {{",
                });

                for (int j = 0; j < v.values.Length; j++)
                {
                    res.Add($"        {v.values[j]} = {j + 1},");
                }

                res.AddRange(new List<string>
                {
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
#endif
