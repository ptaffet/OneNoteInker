/*
 * @(#) Ink.cs 1.0 06-08-2007 author: Manoj Prasad
 *************************************************************************
 * Copyright (c) 2008 Hewlett-Packard Development Company, L.P.
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 **************************************************************************/
/************************************************************************
 * SVN MACROS
 *
 * $Revision: 244 $
 * $Author: mnab $
 * $LastChangedDate: 2008-07-04 13:57:50 +0530 (Fri, 04 Jul 2008) $
 ************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;

namespace InkML
{
	/// <summary>
	///
	/// </summary>

    public class Ink:InkElement
    {

        #region Fields
        private List<InkElement> inkList;
        
        private string documentId="";
        private Definitions definitions;
        private Definitions definitionsBlock;
        public string DocumentId
        {
            get { return documentId; }
        }
        #endregion Fields

        #region Constructors

        public Ink(Definitions defs)
        {
            definitionsBlock = new Definitions();
            definitions = defs;
            inkList = new List<InkElement>();
            inkList.Add(definitionsBlock);
        }

        public Ink(string DocumentId,Definitions defs)
            :this(defs)
        {
            this.documentId = DocumentId;
        }

        public Ink(XmlElement element,Definitions defs)
            :this(defs)
        {
            ParseElement(element);
        }

        #endregion Constructors

        #region Overriden Methods Parse & ToInkML

        public override void ParseElement(XmlElement element)
        {
            if (element != null && element.LocalName.Equals("ink"))
            {
                documentId = element.GetAttribute("documentID");
                foreach (XmlNode Node in element)
                {
                    if (Node.LocalName.Equals("trace"))
                    {
                        inkList.Add(new Trace(definitions,Node as XmlElement));
                    }
                    else if (Node.LocalName.Equals("traceGroup"))
                    {
                        inkList.Add(new TraceGroup(definitions,Node as XmlElement));
                    }
                    else if (Node.LocalName.Equals("annotation"))
                    {
                        inkList.Add(new Annotation( Node as XmlElement));
                    }
                    else if (Node.LocalName.Equals("annotationXML"))
                    {
                        inkList.Add(new AnnotationXML(Node as XmlElement));
                    }
                    else if (Node.LocalName.Equals("definition"))
                    {
                        inkList.Add(new Definitions(Node as XmlElement));
                        
                    }                
                }

            }
            else
            {
                throw new Exception("Invalid Element.");
            }
        }

        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("ink");
            if (!documentId.Equals(""))
            {
                result.SetAttribute("documentID", documentId);
            }
            foreach (InkElement element in inkList)
            {
                result.AppendChild(element.ToInkML(inkDocument));                
            }
            return result;
        }        

        #endregion Overriden Methods Parse & ToInkML

        #region Add functions
        public void AddAnnotation(Annotation annotation)
        {
            if (annotation != null)
            {
                inkList.Add(annotation);
            }
        }

        public void AddAnnotationXML(AnnotationXML annotationXML)
        {
            if (annotationXML != null)
            {
                inkList.Add(annotationXML);
            }
        }

        public void AddTrace(Trace trace)
        {
            if (trace != null)
            {
                inkList.Add(trace);                   
            }

        }

        public void AddTraceGroup(TraceGroup traceGroup)
        {
            if(traceGroup!=null)
            {
                inkList.Add(traceGroup);
            }
        }

        public void AddContext(Context context)
        {
            if (context != null)
            {
                inkList.Add(context);
            }
        }

        public void AddDefinitions(Definitions defs)
        {
            if (defs != null)
            {
                inkList.Add(defs);                
            }
        }

        public void AddDefinitions(TraceFormat traceFormat)
        {
            if (!traceFormat.Id.Equals(""))
            {
                definitionsBlock.AddTraceFormat(traceFormat);
            }
        }

        public void AddDefinitions(Canvas canvas)
        {
            if (!canvas.Id.Equals(""))
            {
                definitionsBlock.AddCanvas(canvas);
            }
        }

        public void AddDefinitions(InkSource inkSource)
        {
            if (!inkSource.Id.Equals(""))
            {
                definitionsBlock.AddInkSource(inkSource);
            }
        }

        public void AddDefinitions(Brush brush)
        {
            if (!brush.Id.Equals(""))
            {
                definitionsBlock.AddBrush(brush);
            }
        }

        public void AddDefinitions(Context context)
        {
            if (!context.Id.Equals(""))
            {
                definitionsBlock.AddContext(context);
            }
        }
        
        #endregion add functions

        #region Enumerators
        public List<InkElement>.Enumerator GetInkElements()
        {
            return inkList.GetEnumerator();            
        }

        #endregion Enumerators

        #region Remove Region
        public void RemoveElement(InkElement element)
        {
            inkList.Remove(element);           
        }
        #endregion Remove Region

    }
}
