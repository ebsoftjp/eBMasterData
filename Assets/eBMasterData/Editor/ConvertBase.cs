#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace eBMasterData.Editor
{
    public class ConvertBase
    {
        private readonly Settings settings;

        public ConvertBase(Settings settings)
        {
            this.settings = settings;
        }

        public List<string> Create(List<ReaderForEditor.KeysData2> data)
        {
            var mdClass = $"{settings.NamespaceName}.{settings.DataFileName}";
            var mdBaseClass = $"{settings.NamespaceName}.{settings.ClassNamePrefix}{settings.ClassNameBase}";
            var res = new List<string>
            {
                $"// Auto create by eBMasterData.ConvertBase",
                $"using System.Collections.Generic;",
                $"using System.Linq;",
                $"using UnityEngine;",
                $"",
                $"public static class {settings.BaseFileName}",
                $"{{",
                $"    private const string resourcePath = \"{Paths.DataPath}\";",
                $"",
                $"    public static {mdClass} _Tables;",
                $"    public static {mdClass} Tables {{ get {{ if (!_Tables) _Tables = Resources.Load<{mdClass}>(resourcePath); return _Tables; }} }}",
                $"",
            };

            foreach (var v in data)
            {
                if (!settings.Configs.Contains(v.name))
                {
                    res.Add($"    public static {settings.NamespaceName}.{settings.ClassNamePrefix}{v.name}[] {v.name} => Tables.{v.name};");
                }
                else
                {
                    res.AddRange(new List<string>
                    {
                        $"    public static class {v.name}",
                        $"    {{",
                    });

                    var dic = new Dictionary<string, List<string>>();
                    foreach (var v3 in v.keys)
                    {
                        var key = v3.key.Split("_")[0];
                        if (!dic.ContainsKey(key)) dic[key] = new();
                        var type = v3.type;
                        if (settings.Enums.Contains(type)) type = $"{settings.NamespaceName}.{type}";
                        dic[key].Add($"            public static {type} {v3.key.Replace($"{key}_", "")} => Tables.{v.name}[0].{v3.key};");
                    }

                    for (int i = 0; i < dic.Keys.Count; i++)
                    {
                        if (i > 0) res.Add("");

                        var key = dic.Keys.ElementAt(i);
                        res.AddRange(new List<string>
                        {
                            $"        public static class {key}",
                            $"        {{",
                        });

                        res.AddRange(dic[key]);

                        res.AddRange(new List<string>
                        {
                            $"        }}",
                        });
                    }
                    res.AddRange(new List<string>
                    {
                        $"    }}",
                    });
                }
            }

            res.AddRange(new List<string>
            {
                $"",
                $"    public static T At<T>(this IList<T> self, string key) where T : {mdBaseClass} => self.FirstOrDefault(v => v.Id == key);",
                $"    public static T[] ArrayAt<T>(this IList<T> self, string key) where T : {mdBaseClass} => self.Where(v => v.Id == key).ToArray();",
                $"}}",
                $"",
            });

            return res;
        }
    }
}
#endif
