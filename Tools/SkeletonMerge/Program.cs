using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var root = Path.GetFullPath(args[0]);
var mode = args.Length > 1 ? args[1] : "inventory";
var fieldStage=mode=="generate-fields";
var packages = new[] {("urp", "com.unity.render-pipelines.universal"), ("core", "com.unity.render-pipelines.core")};
var options = new CSharpParseOptions(LanguageVersion.CSharp9, preprocessorSymbols: new[] {"UNITY_2022_2_OR_NEWER", "UNITY_6000_0_OR_NEWER", "ENABLE_VR", "ENABLE_XR_MODULE", "ENABLE_UNITY_COLLECTIONS_CHECKS", "UNITY_2020_2_OR_NEWER", "UNITY_2021_1_OR_NEWER", "UNITY_2021_2_OR_NEWER", "UNITY_2022_1_OR_NEWER", "UNITY_2023_1_OR_NEWER", "UNITY_2023_2_OR_NEWER", "UNITY_EDITOR", "DEVELOPMENT_BUILD", "UNITY_ANDROID"});
if(mode=="repair") {
 foreach(var package in packages.Select(p=>p.Item2)) foreach(var path in Directory.GetFiles(Path.Combine(root,package,"Runtime"),"*.cs",SearchOption.AllDirectories)) {
  var source=File.ReadAllText(path); var unit=CSharpSyntaxTree.ParseText(source,options).GetRoot(); var edits=new List<(int pos,string text)>();
  foreach(var m in unit.DescendantNodes().OfType<MethodDeclarationSyntax>()) {
   if(m.Body==null || m.Body.Statements.Count>1)continue;
   if(m.Body.Statements.Any(st=>st is not ReturnStatementSyntax || ((ReturnStatementSyntax)st).Expression is InvocationExpressionSyntax))continue;
   var outs=m.ParameterList.Parameters.Where(p=>p.Modifiers.Any(SyntaxKind.OutKeyword)).ToArray();
   if(outs.Length>0)edits.Add((m.Body.OpenBraceToken.Span.End,"\n"+string.Join("\n",outs.Select(p=>"            "+p.Identifier.Text+" = default;"))+"\n"));
  }
  if(path.Contains("/_Skeleton/")) foreach(var c in unit.DescendantNodes().OfType<ConstructorDeclarationSyntax>()) {
   if(c.Parent is StructDeclarationSyntax && c.Initializer==null && c.Body?.Statements.Count==0)edits.Add((c.ParameterList.Span.End," : this()"));
  }
  foreach(var e in edits.OrderByDescending(e=>e.pos))source=source.Insert(e.pos,e.text);
  if(edits.Count>0)File.WriteAllText(path,source);
 }
 return;
}
string Name(BaseTypeDeclarationSyntax t) => string.Join(".", t.Ancestors().Reverse().Select(a => a switch { BaseNamespaceDeclarationSyntax n => n.Name.ToString(), BaseTypeDeclarationSyntax b => b.Identifier.ValueText, _ => "" }).Where(s => s != "").Append(t.Identifier.ValueText));
string Key(MemberDeclarationSyntax m) => m switch {
 FieldDeclarationSyntax f => "field:" + string.Join(",", f.Declaration.Variables.Select(v=>v.Identifier.ValueText)),
 EventFieldDeclarationSyntax f => "field:" + string.Join(",", f.Declaration.Variables.Select(v=>v.Identifier.ValueText)),
 PropertyDeclarationSyntax p => "property:"+p.Identifier.ValueText,
 EventDeclarationSyntax e => "field:"+e.Identifier.ValueText,
 BaseTypeDeclarationSyntax b => "type:"+b.Identifier.ValueText,
 MethodDeclarationSyntax x => "method:"+ x.Identifier.ValueText + "`" + (x.TypeParameterList?.Parameters.Count ?? 0)+"("+string.Join(",",x.ParameterList.Parameters.Select(p=>Regex.Replace(p.Type?.ToString()??"", @"\b(?:[A-Za-z_]\w*\.)+", "").Replace(" ", "")))+")",
 _ => ""
};
var log = new List<object>();
var patches = new Dictionary<string,List<(int pos,string text,string owner)>>();
var excluded = new HashSet<string>{
 "UnityEngine.Rendering.Universal.PlatformAutoDetect", "UnityEngine.Rendering.ProfilingSampler", "UnityEngine.Rendering.CoreUtils"
};

foreach (var (stage, package) in packages) {
 var sources = Directory.GetFiles(Path.Combine(root, package, "Runtime"), "*.cs", SearchOption.AllDirectories).Where(p=>!p.Contains("/_Skeleton/"));
 var existing = sources.SelectMany(p=>CSharpSyntaxTree.ParseText(File.ReadAllText(p), options).GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Select(t=>(path:p,type:t))).GroupBy(x=>Name(x.type)).ToDictionary(g=>g.Key,g=>g.ToList());
 foreach(var file in Directory.GetFiles(Path.Combine(root,".qa-work",stage+(fieldStage?"-fields":"")),"*.cs",SearchOption.AllDirectories).Where(p=>!p.Contains("/Properties/"))) {
  var text=File.ReadAllText(file);
  text=Regex.Replace(text,@"^using Il2CppDummyDll;\r?\n", "", RegexOptions.Multiline);
  text=Regex.Replace(text,@"^\s*\[(?:Token|Address|FieldOffset|MetadataOffset|Il2CppDummyDll\.[A-Za-z]+)\([^\r\n]*\)\]\r?\n", "\n", RegexOptions.Multiline);
  text=Regex.Replace(text,@"namespace ([\w.]+);", "namespace $1\n{");
  if (Regex.IsMatch(File.ReadAllText(file),@"namespace [\w.]+;")) text+="\n}\n";
  var tree=CSharpSyntaxTree.ParseText(text,options);
  var unit=(CompilationUnitSyntax)tree.GetRoot();
  var top=unit.DescendantNodes().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
  if(top==null) {
   if(fieldStage && unit.DescendantNodes().OfType<DelegateDeclarationSyntax>().Any() && file.Contains("/VL/")) {
    var dest=Path.Combine(root,package,"Runtime","QualiArts","_Skeleton",Path.GetRelativePath(Path.Combine(root,".qa-work",stage+"-fields"),file));
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);File.WriteAllText(dest,text);
   }
   continue;
  }
  if(file.Contains("/Microsoft.CodeAnalysis/")||file.Contains("/System.Runtime.CompilerServices/")||file.Contains("<PrivateImplementationDetails>"))continue;
  var name=Name(top);
  if(!existing.ContainsKey(name)) {
   var dest=Path.Combine(root,package,"Runtime","QualiArts","_Skeleton",Path.GetRelativePath(Path.Combine(root,".qa-work",stage+(fieldStage?"-fields":"")),file));
   log.Add(new{kind="type",name,destination=dest});
   if(mode=="generate" || fieldStage) {Directory.CreateDirectory(Path.GetDirectoryName(dest)!); File.WriteAllText(dest,"// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.\n"+text);}
   continue;
  }
  foreach(var gt in unit.DescendantNodes().OfType<TypeDeclarationSyntax>()) {
   var full=Name(gt);
   if(!existing.TryGetValue(full,out var parts))continue;
   if(args.Length>2 && full!=args[2] && !full.StartsWith(args[2]+"."))continue;
   var keys=parts.SelectMany(p=>p.type is TypeDeclarationSyntax td? td.Members.Select(Key):Enumerable.Empty<string>()).ToHashSet();
   var additions=gt.Members.Where(m=>Key(m)!=""&&!keys.Contains(Key(m))).ToList();
   // Multi-variable stock fields are compared by individual name.
   additions.RemoveAll(m=>m is FieldDeclarationSyntax f&&f.Declaration.Variables.All(v=>parts.Any(p=>p.type is TypeDeclarationSyntax td&&td.Members.OfType<FieldDeclarationSyntax>().Any(sf=>sf.Declaration.Variables.Any(sv=>sv.Identifier.ValueText==v.Identifier.ValueText)))));
   additions.RemoveAll(m => Key(m).Contains("_002E") || Key(m).Contains("_003C") || excluded.Contains(full));
   if(additions.Count==0)continue;
   log.Add(new{kind="members",name=full,destination=parts[0].path,members=additions.Select(Key).ToArray()});
   if(mode=="merge") {
    foreach(var member in additions) {
     var target=parts.OrderByDescending(p=>((TypeDeclarationSyntax)p.type).Members.Count).First();
     if(target.type==null)target=parts[0];
     int pos=((TypeDeclarationSyntax)target.type).CloseBraceToken.FullSpan.Start;
     if(member is FieldDeclarationSyntax) {
      var following = gt.Members.SkipWhile(m=>m!=member).Skip(1).OfType<FieldDeclarationSyntax>().ToArray();
      foreach(var f in following) {
       var anchor=parts.SelectMany(p=>((TypeDeclarationSyntax)p.type).Members.Select(m=>(p.path,m))).FirstOrDefault(x=>Key(x.m)==Key(f));
       if(anchor.m!=null){target=(anchor.path,(BaseTypeDeclarationSyntax)anchor.m.Parent!);pos=anchor.m.FullSpan.Start;break;}
      }
     }
     var code="\n        // TODO(qa-recon): game declaration; DummyDll body remains a placeholder.\n"+member.NormalizeWhitespace("    ").ToFullString()+"\n";
     if(!patches.ContainsKey(target.path))patches[target.path]=new();
     patches[target.path].Add((pos,code,full));
    }
    // Imported game declaration types must resolve in the original file as well.
    foreach(var filePath in patches.Where(p=>p.Value.Any(e=>e.owner==full)).Select(p=>p.Key).ToArray()) {
     var sourceUnit=CSharpSyntaxTree.ParseText(File.ReadAllText(filePath),options).GetCompilationUnitRoot();
     var imported=sourceUnit.Usings.Select(u=>u.ToString()).ToHashSet();
     var extra=unit.Usings.Where(u=>!imported.Contains(u.ToString())).Select(u=>u.ToString()).ToArray();
     if(extra.Length>0){if(!patches.ContainsKey(filePath))patches[filePath]=new();patches[filePath].Add((0,string.Join("\n",extra)+"\n",full));}
    }
   }
  }
 }
}
if(mode=="merge") {
 foreach(var (path,edits) in patches) {
  var source=File.ReadAllText(path);
  foreach(var edit in edits.Distinct().OrderByDescending(e=>e.pos).ThenByDescending(e=>edits.IndexOf(e))) source=source.Insert(edit.pos,edit.text);
  File.WriteAllText(path,source);
 }
}
File.WriteAllText(Path.Combine(root,".qa-work","inventory.json"),JsonSerializer.Serialize(log,new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"{log.Count} changes inventoried");
