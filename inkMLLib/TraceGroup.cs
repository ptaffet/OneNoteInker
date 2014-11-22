/*
 * @(#) TraceGroup.cs 1.0 06-08-2007 author: Manoj Prasad
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
using System.Xml;
using System.Collections;

namespace InkML
{
	/// <summary>
	///
	/// </summary>

    public class TraceGroup:Stroke
    {
        #region attributes
        private List<InkElement> groupList;
        private string id="";
        private string brushRef="";
        private string contextRef="";
        private bool ContainsBrush = false;

        private Brush associatedBrush;
        private Context associatedContext;
        private Context associatedCurrentContext;
        private Definitions definitions;

       
        /// <summary>
        /// Gets/Sets the 'id' attribute of the TraceGroup Element
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets/Sets the 'contextRef' attribute of the TraceGroup Element
        /// </summary>
        public string ContextRef
        {
            get { return contextRef; }
            set
            {
                contextRef = value;
                ResolveContext();
            }
        }

        /// <summary>
        /// Gets/Sets the 'brushRef' attribute of the TraceGroup Element
        /// </summary>
       
        public string BrushRef
        {
            get { return brushRef; }
            set
            {
                brushRef = value;
                ResolveBrush();
            }
        }

        /// <summary>
        /// Gets the associated Brush of the TraceGroup Element
        /// </summary>
        public Brush AssociatedBrush
        {
            get { return associatedBrush; }
        }

        /// <summary>
        /// Gets the associated context of the TraceGroup Element
        /// </summary>
        public Context AssociatedContext
        {
            get { return associatedContext; }
        }
        #endregion Attributes

        #region Constructor

        public TraceGroup(Definitions defs, XmlElement element)
        {
            associatedContext = new Context(defs);
            associatedCurrentContext = new Context(defs);
            groupList = new List<InkElement>();
            this.TagName = "traceGroup";
            if (defs != null)
            {
                definitions = defs;
            }
            else
            {
                throw new Exception("Null Definitions Block.");
            }
            if (element != null)
            {
                ParseElement(element);
            }
            else
            {
                throw new Exception("Null xml element");
            }
           
        }

        public TraceGroup(Definitions defs, Context currentContext, XmlElement element)
            
        {
            associatedCurrentContext = new Context(defs);
            associatedBrush = currentContext.BrushElement;
            
            associatedCurrentContext.BrushElement = currentContext.BrushElement;
            associatedCurrentContext.TraceFormatElement = currentContext.TraceFormatElement;
            associatedCurrentContext.CanvasElement = currentContext.CanvasElement;
            associatedCurrentContext.InksourceElement = currentContext.InksourceElement;
            associatedContext = associatedCurrentContext;
            groupList = new List<InkElement>();
            this.TagName = "traceGroup";
            if (defs != null)
            {
                definitions = defs;
            }
            else
            {
                throw new Exception("Null Definitions Block.");
            }
            if (element != null)
            {
                ParseElement(element);
            }
            else
            {
                throw new Exception("Null xml element");
            }
            
        }

        public TraceGroup(Definitions defs)
        {
            groupList = new List<InkElement>();
            this.TagName = "traceGroup";
            if (defs != null)
            {
                definitions = defs;
            }
            else
            {
                throw new Exception("Null Definitions Block.");
            }
            associatedContext = new Context(defs);
            associatedCurrentContext = new Context(defs);
        }

        public TraceGroup(Definitions defs, string id):this(defs)
        {
            
            if (!id.Equals(""))
            {
                this.id = id;
                definitions.AddTraceGroup(this);
            }
            
           
        }
        #endregion Constructor

        #region Override methods Parse and toInkML
        public override void ParseElement(XmlElement element)
        {
            if (element != null && element.LocalName.Equals("traceGroup"))
            {
                id = element.GetAttribute("id");
                if (!id.Equals(""))
                {
                    definitions.AddTraceGroup(this);
                }
                brushRef = element.GetAttribute("brushRef");
                if (!brushRef.Equals(""))
                {
                    ResolveBrush();
                }

                contextRef = element.GetAttribute("contextRef");
                if (!contextRef.Equals(""))
                {
                    ResolveContext();
                }

                foreach (XmlNode Node in element.ChildNodes)
                {
                    if(Node.LocalName.Equals("traceGroup"))
                    {
                        if (associatedContext != null)
                        {
                            groupList.Add(new TraceGroup(definitions, associatedContext, Node as XmlElement));
                        }
                        else
                        {
                            groupList.Add(new TraceGroup(definitions, Node as XmlElement));
                        }
                    }
                    if (Node.LocalName.Equals("trace"))
                    {
                        if (associatedContext != null)
                        {
                            groupList.Add(new Trace(definitions, associatedContext, Node as XmlElement));
                        }
                        else
                        {
                            groupList.Add(new Trace(definitions, Node as XmlElement));
                        }
                    }
                    if (Node.LocalName.Equals("annotation"))
                    {
                        groupList.Add(new Annotation(Node as XmlElement));
                    }
                    if (Node.LocalName.Equals("annotationXML"))
                    {
                        groupList.Add(new AnnotationXML(Node as XmlElement));
                    }
                }

            }
            else
            {
                throw new Exception("Invalid Element .");
            }
        }

        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("traceGroup");
            if (!id.Equals(""))
            {
                result.SetAttribute("id", id);
            }
            if (!brushRef.Equals(""))
            {
                result.SetAttribute("brushRef", brushRef);
            }
            if (!contextRef.Equals(""))
            {
                result.SetAttribute("contextRef", contextRef);
            }
            foreach (InkElement element in groupList)
            {
                result.AppendChild(element.ToInkML(inkDocument));
            }
            return result;
        }

        #endregion Override methods Parse and toInkML

        #region Resolve context and brush
        public void ResolveContext()
        {
            Context tctx;
            if (definitions.ContainsID(contextRef))
            {
                tctx = definitions.GetContext(contextRef);
                if (tctx != null)
                {
                    if (tctx.CanvasElement != null)
                    {
                        associatedContext.CanvasElement = tctx.CanvasElement;
                    }
                    if (tctx.InksourceElement != null)
                    {
                        associatedContext.InksourceElement = tctx.InksourceElement;
                    }
                    if (tctx.TraceFormatElement != null)
                    {
                        associatedContext.TraceFormatElement = tctx.TraceFormatElement;
                    }
                    if (!ContainsBrush && tctx.BrushElement != null)
                    {
                        associatedContext.BrushElement = tctx.BrushElement;
                        associatedBrush = tctx.BrushElement;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid Reference.");
            }
        }

        public void ResolveBrush()
        {
            Brush tb;
            if (definitions.ContainsID(brushRef))
            {
                tb = definitions.GetBrush(brushRef);
                if (tb != null)
                {
                    associatedBrush = tb;
                    ContainsBrush = true;
                }
            }
            else
            {
                throw new Exception("Invalid Reference.");
            }
        }
        #endregion Resolve context and brush

        #region Add and Remove Methods 
        public void AddTrace(Trace trace)
        {
            if (trace != null)
            {
                groupList.Add(trace);
            }
        }

        public void AddTraceGroup(TraceGroup traceGroup)
        {
            if (traceGroup != null)
            {
                groupList.Add(traceGroup);
            }
        }

        public void AddAnnotation(Annotation annotation)
        {
            if (annotation != null)
            {
                groupList.Add(annotation);
            }
        }

        public void AddAnnotationXML(AnnotationXML annotationXML)
        {
            if (annotationXML != null)
            {
                groupList.Add(annotationXML);
            }
        }


        #endregion  Add and Remove Methods

        public List<InkElement>.Enumerator GetEnumerator()
        {
            return groupList.GetEnumerator();
        }
    }
}
