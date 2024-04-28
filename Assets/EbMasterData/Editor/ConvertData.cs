using System.Collections.Generic;

namespace EbMasterData.Editor
{
    public class ConvertData
    {
        private string masterClassName => settings.DataFileName;
        private string tablePrefix => settings.ClassNamePrefix;
        private string classBase0 => settings.ClassNameBase;

        private readonly Settings settings;

        public ConvertData(Settings settings)
        {
            this.settings = settings;
        }

        public List<string> CreateMasterDataData(List<ReaderForEditor.KeysData2> data)
        {
            var res = new List<string>
            {
                $"// Auto create by EbMasterData.ConvertData",
                $"using System.Linq;",
                $"using UnityEngine;",
                $"",
                $"namespace {settings.NamespaceName}",
                $"{{",
                $"    public class {masterClassName} : ScriptableObject",
                $"    {{",
            };

            foreach (var v in data)
            {
                res.Add($"        public {tablePrefix}{v.name}[] {v.name};");
            }

            res.AddRange(new List<string>
            {
                $"",
                $"        public {tablePrefix}{classBase0}[] GetItemsFromTableName(string tableName) => tableName switch",
                $"        {{",
            });

            foreach (var v in data)
            {
                res.Add($"            \"{v.name}\" => {v.name},");
            }

            res.AddRange(new List<string>
            {
                $"            _ => null,",
                $"        }};",
                $"",
                $"        public void Convert2(string[] names, string[][][] data)",
                $"        {{",
                $"            Debug.Assert(names.Length == data.Length, $\"Convert2: {{names.Length}} != {{data.Length}}\");",
            });

            foreach (var v in data)
            {
                res.Add($"            {v.name} = ConvertList<{tablePrefix}{v.name}>(\"{v.name}\", names, data);");
            }

            res.AddRange(new List<string>
            {
                $"        }}",
                $"",
                $"        private T[] ConvertList<T>(string title, string[] titles, string[][][] data) where T : {tablePrefix}{classBase0}",
                $"        {{",
                $"            return data?",
                $"                .ElementAtOrDefault(System.Array.IndexOf(titles, title))?",
                $"                .Select(v => System.Activator.CreateInstance(typeof(T), new object[] {{ v }}) as T)?",
                $"                .ToArray() ?? new T[0];",
                $"        }}",
                $"    }}",
                $"}}",
                $"",
            });

            return res;
        }
    }
}
