using System;
using System.Collections.Generic;
using System.IO;

namespace DrDoc.Generation
{
    public class PatternTemplateResolver : IPatternTemplateResolver
    {
        private bool HACK_inNamespace;

        public IList<TemplateMatch> Resolve(string path, IList<DocNamespace> namespaces)
        {
            var current = path;

            if (current.Contains("\\"))
            {
                current = current.Substring(0, current.IndexOf('\\'));
            }

            return ResolveRecursive(current, path, path, namespaces);
        }

        private IList<TemplateMatch> ResolveRecursive(string current, string outputPath, string templatePath, IList<DocNamespace> namespaces)
        {
            var matches = new List<TemplateMatch>();

            if (Path.GetExtension(current) == ".spark")
            {
                if (current == "!namespace.spark")
                {
                    foreach (var ns in namespaces)
                    {
                        matches.Add(
                            new TemplateMatch(
                                outputPath.Replace(".spark", ".htm").Replace("!namespace", ns.Name),
                                templatePath,
                                new OutputData { Namespaces = namespaces, Namespace = ns }
                            ));
                    }
                }
                else if (current == "!type.spark")
                {
                    foreach (var ns in namespaces)
                    {
                        foreach (var type in ns.Types)
                        {
                            var name = HACK_inNamespace ? type.Name : ns.Name + "." + type.Name;

                            matches.Add(
                                new TemplateMatch(
                                    outputPath.Replace(".spark", ".htm").Replace("!type", name),
                                    templatePath,
                                    new OutputData { Namespaces = namespaces, Namespace = ns, Type = type }
                                ));
                        }
                    }
                }
                else
                    matches.Add(
                        new TemplateMatch(
                            outputPath.Replace(".spark", ".htm"),
                            templatePath,
                            new OutputData { Namespaces = namespaces }
                        ));
            }
            else
            {
                if (!IsSpecial(current))
                {
                    var parts = templatePath.Replace(current, "").Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                    matches.AddRange(ResolveRecursive(parts[0], templatePath, templatePath, namespaces));
                }
                else if (current == "!namespace")
                {
                    var parts = templatePath.Substring(templatePath.IndexOf(current) + current.Length).Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                    HACK_inNamespace = false;

                    foreach (var ns in namespaces)
                    {
                        var nsPath = templatePath.Replace(current, ns.Name);

                        HACK_inNamespace = true;

                        matches.AddRange(ResolveRecursive(parts[0], nsPath, templatePath, new List<DocNamespace> { ns }));
                    }

                    HACK_inNamespace = false;
                }
                else if (current == "!type")
                {
                    var parts = templatePath.Replace(current, "").Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var ns in namespaces)
                    {
                        foreach (var type in ns.Types)
                        {
                            var typePath = templatePath.Replace(current, ns.Name + "." + type.Name);

                            matches.AddRange(ResolveRecursive(parts[0], typePath, templatePath, new List<DocNamespace> { ns }));    
                        }
                    }
                }
            }

            return matches;
        }

        private bool IsSpecial(string name)
        {
            var path = Path.GetFileNameWithoutExtension(name);

            return path == "!namespace" || path == "!type";
        }
    }
}