Enum.GetNames(typeof(IID)).Select(x => x.Replace("_", ".")).Select(x => $".{x}").ToArray().Where(x => !Keys().Contains(x)).ToArray()
List<string> Keys()
{
     var list = new List<string>();
     foreach (KeyValuePair<string, JToken> i in jo) list.Add(i.Key);
     return list;
}