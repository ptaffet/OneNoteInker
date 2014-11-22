using Microsoft.Ink;
using OneNote = Microsoft.Office.Interop.OneNote;
using Word = Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace OneNoteInker
{
    class Program
    {
        static OneNote.Application app = new OneNote.Application();
        static XmlDocument xdoc;
        static XmlElement page;
        private static string DIRECTORY = @"C:\Users\Philip\Desktop\notes1\word";
        private static string BASE_DIRECTORY = @"C:\Users\Philip\Desktop\";
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                BASE_DIRECTORY = args[0];
                DIRECTORY = Path.Combine(BASE_DIRECTORY, "notes1", "word");
            }
            InitOneNote();
            TransferInk();
            CommitOneNote();
            Console.WriteLine("All done.");
            Console.ReadLine();
        }

        private static void CommitOneNote()
        {
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xdoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Close();
                app.UpdatePageContent(stringWriter.GetStringBuilder().ToString());
            }
        }

        private static void InitOneNote()
        {
            string hierarchXml;
            app.GetHierarchy(null, OneNote.HierarchyScope.hsPages, out hierarchXml);
            XmlDocument hierc = new XmlDocument();
            hierc.LoadXml(hierarchXml);
            XmlNamespaceManager nsman = new XmlNamespaceManager(hierc.NameTable);
            nsman.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");
            XmlNode unfiledPages = hierc.SelectSingleNode("//one:Section[@name = \"Unfiled Notes\"]/one:Page", nsman);
            string testPageID = unfiledPages.Attributes["ID"].Value;
            string pageXml;
            app.GetPageContent(testPageID, out pageXml, OneNote.PageInfo.piAll);

            xdoc = new XmlDocument();
            xdoc.LoadXml(pageXml);

        }

        private static void TransferInk()
        {
            Word.Application app = new Word.Application();
            var doc = app.Documents.Open(Path.Combine(BASE_DIRECTORY, "Notes1.docx"));
            var shapes = doc.Shapes;
            int a = shapes.Count;
            int i = 0;
            Dictionary<string, string> rIdToPath = new Dictionary<string, string>();
            XmlDocument xdoc = new XmlDocument();
            using (FileStream fs = new FileStream(Path.Combine(DIRECTORY, @"_rels\document.xml.rels"), FileMode.Open))
            using (XmlReader reader = XmlReader.Create(fs))
            {
                xdoc.Load(reader);
                var root = xdoc.ChildNodes[1];
                foreach (XmlNode relationship in root.ChildNodes)
                {
                    rIdToPath[relationship.Attributes["Id"].Value] = relationship.Attributes["Target"].Value;
                }

            }
            Dictionary<string, string> inkNumToPath = new Dictionary<string, string>();
            /*using (WordprocessingDocument wpdoc = WordprocessingDocument.Open(Path.Combine(BASE_DIRECTORY, "Notes2.docx"), false))
            {
                var maindoc = wpdoc.MainDocumentPart;
                var drawings = maindoc.RootElement.Descendants<Drawing>();
                foreach (var drawing in drawings)
                {
                    string name = drawing.Anchor.Descendants<DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties>().First().Name;
                    var contents = drawing.Anchor.Descendants<DocumentFormat.OpenXml.Office2010.Word.ContentPart>().First();
                    string rId = contents.RelationshipId.Value;
                    inkNumToPath[name] = rIdToPath[rId];
                }
            }*/
            xdoc = new XmlDocument();
            using (FileStream fs = new FileStream(Path.Combine(DIRECTORY, "document.xml"), FileMode.Open))
            using (XmlReader reader = XmlReader.Create(fs))
            {
                xdoc.Load(reader);
                XmlNamespaceManager nsman = new XmlNamespaceManager(xdoc.NameTable);
                nsman.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                nsman.AddNamespace("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
                nsman.AddNamespace("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
                nsman.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

                var nodes = xdoc.SelectNodes("//mc:AlternateContent/mc:Choice/w:drawing/wp:anchor", nsman);
                foreach (XmlNode item in nodes)
                {
                    string name = item.SelectSingleNode("./wp:docPr", nsman).Attributes["name"].Value;
                    string rId = item.SelectSingleNode("./a:graphic", nsman).ChildNodes[0].ChildNodes[0].Attributes["r:id"].Value;
                    inkNumToPath[name] = rIdToPath[rId];
                }

            }
           
            foreach (Word.Shape shape in shapes)
            {
                float oldLeft = shape.Left;
                shape.RelativeVerticalPosition = Word.WdRelativeVerticalPosition.wdRelativeVerticalPositionPage;
                shape.RelativeHorizontalPosition = Word.WdRelativeHorizontalPosition.wdRelativeHorizontalPositionPage;
                int waitcount = 20;
                while (shape.Left == oldLeft && waitcount-- > 0)
                    System.Threading.Thread.Sleep(5);
                if (waitcount <= 0)
                    Console.WriteLine("Reached wait limit.");
                AddInkToOneNote(inkNumToPath[shape.Name], shape.Left, shape.Top, 2*shape.Width, 2*shape.Height);
                Console.WriteLine(shape.Name + " (" + inkNumToPath[shape.Name] + ") placed at " + shape.Left +", " + shape.Top+ " and has size " + 2*shape.Width + " x " + 2*shape.Height);
                //if (i++ > 150)
                 //   break;
            }

            //doc.Close();

        }

        private static void AddInkToOneNote(string path, float left, float top, float width, float height)
        {

            InkMLConverters.InkML2ISF converter = new InkMLConverters.InkML2ISF();
            string base64 = converter.ConvertToISF(Path.Combine(DIRECTORY, path));
            string namesp = "http://schemas.microsoft.com/office/onenote/2013/onenote";

            XmlElement node = xdoc.CreateElement("one", "InkDrawing", namesp);

            XmlNode position = xdoc.CreateElement("one", "Position", namesp);
            position.Attributes.Append(CreateAttribute("x", left));
            position.Attributes.Append(CreateAttribute("y", top));
            position.Attributes.Append(CreateAttribute("z", 0));
            node.AppendChild(position);


            XmlNode size = xdoc.CreateElement("one", "Size", namesp);
            size.Attributes.Append(CreateAttribute("width", Math.Max(.05f, width)));
            size.Attributes.Append(CreateAttribute("height", Math.Max(.05f, height)));
            node.AppendChild(size);

            XmlNode data = xdoc.CreateElement("one", "Data", namesp);
            data.InnerText = base64;
            node.AppendChild(data);

            xdoc.DocumentElement.AppendChild(node);

        }

        private static XmlAttribute CreateAttribute(string name, float value)
        {
            var x = xdoc.CreateAttribute(name);
            x.Value = value.ToString();
            return x;
        }
    }
}
