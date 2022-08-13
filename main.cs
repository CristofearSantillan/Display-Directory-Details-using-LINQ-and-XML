using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

class Program
{

    static string FormatByteSize(long byteSize)
    {

        string[] units = { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        var i = (int)(Math.Log(byteSize) / Math.Log(1000));

        return $"{byteSize / Math.Pow(1000, i):F2} {units[i]}";

    }

    static IEnumerable<string> EnumerateFilesRecursively(string path)
    {

        foreach (string file in System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        {

            yield return file;

        }

    }

    static XDocument CreateReport(IEnumerable<string> files)
    {

        XDocument report = new XDocument(
          new XDocumentType("html", null, null, null),
          new XElement("html",
            new XElement("head",
              new XElement("title", "File Report"),
              new XElement("style", "table, th, td { border: 1px solid black; }")
            ),
            new XElement("body",
              new XElement("table",
                new XElement("thead",
                  new XElement("tr",
                    new XElement("th", "Type"),
                    new XElement("th", "Count"),
                    new XElement("th", "Total Size")
                  )
                ),
                new XElement("tbody",
                  files.GroupBy(f => Path.GetExtension(f).ToLower())
                  .Select(g => new
                  {
                      type = g.Key,
                      count = g.Count(),
                      total = g.Sum(x => new System.IO.FileInfo(x).Length)
                  })
                  .OrderByDescending(g => g.total)
                  .Select(g => new XElement("tr",
                    new XElement("td", g.type),
                    new XElement("td", g.count, new XAttribute("align", "right")),
                    new XElement("td", FormatByteSize(g.total), new XAttribute("align", "right"))
                  ))
                )
              )
            )
          )
        );

        return report;

    }

    static void Main(string[] args)
    {

        CreateReport(EnumerateFilesRecursively("./Test")).Save("./output.html");
        //CreateReport(EnumerateFilesRecursively(args[0])).Save(args[1]);

    }

}