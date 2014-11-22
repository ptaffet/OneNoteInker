/*
 * @(#) Context.cs 1.0 06-08-2007 author: Manoj Prasad
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

namespace InkML
{
	/// <summary>
	///
	/// </summary>

    public class Context:InkElement
    {

        #region Fields
        private string id="";
        private string contextRef="";
        private string brushRef="";
        private string canvasRef="";
        private string traceFormatRef="";
        private string inkSourceRef="";

        private bool containsbrush = false;
        private bool containscanvas = false;
        private bool containstraceFormat = false;
        private bool containsinkSource = false;

        private Brush brush;
        private Canvas canvas;
        private TraceFormat traceFormat;
        private InkSource inkSource;
        private Definitions definitions;

        /// <summary>
        /// Gets/Sets the 'id' attribute of the Context Element
        /// </summary>
        public string Id
        {
            get { return id; }
            set 
            {
                if (!definitions.ContainsID(value))
                {
                    id = value;
                    definitions.AddContext(this);
                }
                else
                {
                    throw new Exception("ID Exists.");
                }
            }
        }

        /// <summary>
        /// Gets/Sets the 'contextRef' attribute of the Context Element
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
        /// Gets/Sets the 'traceFormatRef' attribute of the Context Element
        /// </summary>
        public string TraceFormatRef
        {
            get { return traceFormatRef; }
            set
            {
                traceFormatRef = value;
                ResolveTraceFormat();
            }
        }

        /// <summary>
        /// Gets/Sets the 'brushRef' attribute of the Context Element
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
        /// Gets/Sets the 'canvasRef' attribute of the Context Element
        /// </summary>
        public string CanvasRef
        {
            get { return canvasRef; }
            set
            {
                canvasRef = value;
                ResolveCanvas();
            }
        }

        /// <summary>
        /// Gets/Sets the 'inkSourceRef' attribute of the Context Element
        /// </summary>
        public string InkSourceRef
        {
            get { return inkSourceRef; }
            set 
            {
                inkSourceRef = value;
                ResolveInkSource();
            }
        }

        /// <summary>
        /// Gets/Sets the 'id' attribute of the Context Element
        /// </summary>
        public Brush BrushElement
        {
            get { return brush; }
            set
            {
                brush = value;
                containsbrush = true;
            }
        }

        /// <summary>
        /// Gets/Sets the traceFormat Element attribute of the Context Element
        /// </summary>
        public TraceFormat TraceFormatElement
        {
            get { return traceFormat; }
            set 
            {
                traceFormat = value;
                containstraceFormat = true;
            }
        }

        /// <summary>
        /// Gets/Sets the inkSource Element attribute of the Context Element
        /// </summary>
        public InkSource InksourceElement
        {
            get { return inkSource; }
            set
            { 
                inkSource = value;
                containsinkSource = true;
            }                
        }

        /// <summary>
        /// Gets/Sets the canvas Element attribute of the Context Element
        /// </summary>
        public Canvas CanvasElement
        {
            get { return canvas; }
            set
            { 
                canvas = value;
                containscanvas = true;
            }
        }
#endregion Fields

        #region Constructors
        public Context(Definitions defs)
        {
            base.TagName = "context";
            this.definitions = defs;
        }

        public Context(Definitions defs, string id):this(defs)
        {
            this.id = id;
            definitions.AddContext(this);
        }

        public Context(Definitions defs, XmlElement element):this(defs)
        {
            
            ParseElement(element);
        }
        #endregion Constructors

        #region override functions Parse and toinkML
        /// <summary>
        /// Function to parse a xml Element and fill in the Context Details
        /// </summary>
        /// <param name="element"></param>
        public override void ParseElement(XmlElement element)
        {
            inkml = new XmlNamespaceManager(element.OwnerDocument.NameTable);
            if (element.LocalName.Equals("context"))
            {
                foreach (XmlElement item in element)
                {

                    if (item.LocalName == "brush")
                    {
                        brush = new Brush(definitions, item);
                        containsbrush = true;
                    }
                    if (item.LocalName == "inkSource")
                    {
                        inkSource = new InkSource(definitions, item);
                        containsinkSource = true;
                    }
                    if (item.LocalName== "canvas")
                    {
                        canvas = new Canvas(definitions, item);
                        containscanvas = true;
                    }
                    if (item.LocalName == "traceFormat")
                    {
                        traceFormat = new TraceFormat(definitions, item);
                        containstraceFormat = true;
                    }

                }
                id = element.GetAttribute("id");
                if (id == "")
                    id = element.GetAttribute("id", "http://www.w3.org/XML/1998/namespace");
                if (id!=null && !id.Equals(""))
                {
                    definitions.AddContext(this);
                }
               
                contextRef = element.GetAttribute("contextRef");
                if (!contextRef.Equals(""))
                {
                    ResolveContext();
                }

                brushRef = element.GetAttribute("brushRef");
                if (!brushRef.Equals(""))
                {
                    ResolveBrush();
                }

                canvasRef = element.GetAttribute("canvasRef");
                if (!canvasRef.Equals(""))
                {
                    ResolveCanvas();
                }

                inkSourceRef = element.GetAttribute("inkSourceRef");
                if (!inkSourceRef.Equals(""))
                {
                    ResolveInkSource();
                }

                traceFormatRef = element.GetAttribute("traceFormatRef");
                if (!traceFormatRef.Equals(""))
                {
                    ResolveTraceFormat();
                }

            }
            else
            {
                throw new Exception("Invalid Element Name.");
            }
        }

        /// <summary>
        /// Function to convert the Context element to xml Element
        /// </summary>
        /// <param name="inkDocument"></param>
        /// <returns>Context xml Element</returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("context");
            if (!id.Equals(""))
            {
                result.SetAttribute("id", id);
            }
            if (!contextRef.Equals(""))
            {
                result.SetAttribute("contextRef", contextRef);
            }
            if (!brushRef.Equals(""))
            {
                result.SetAttribute("brushRef", brushRef);
            }
            if (!traceFormatRef.Equals(""))
            {
                result.SetAttribute("traceFormatRef", traceFormatRef);
            }
            if (!canvasRef.Equals(""))
            {
                result.SetAttribute("canvasRef", canvasRef);
            }
            if (!inkSourceRef.Equals(""))
            {
                result.SetAttribute("inkSourceRef", inkSourceRef);
            }

            if (containsbrush && brush!=null)
            {
                if (brush.Id.Equals(""))
                {
                    result.AppendChild(brush.ToInkML(inkDocument));
                }
                else
                {
                    result.SetAttribute("brushRef", "#" + brush.Id);
                }
            }
            if (containstraceFormat && traceFormat!=null)
            {
                if (traceFormat.Id.Equals(""))
                {
                    result.AppendChild(traceFormat.ToInkML(inkDocument));
                }
                else
                {
                    result.SetAttribute("traceFormatRef", "#" + traceFormat.Id);
                }
            }
            if (containscanvas && canvas!=null)
            {
                if (canvas.Id.Equals(""))
                {
                    result.AppendChild(canvas.ToInkML(inkDocument));
                }
                else
                {
                    result.SetAttribute("canvasRef", "#" + canvas.Id);
                }
            }
            if (containsinkSource && inkSource!=null)
            {
                if (inkSource.Id.Equals(""))
                {
                    result.AppendChild(inkSource.ToInkML(inkDocument));
                }
                else
                {
                    result.SetAttribute("inkSourceRef", "#" + inkSource.Id);
                }
            }
            return result;
        }
        #endregion override functions Parse and toinkML

        #region Resolve Functions
        /// <summary>
        /// Function to Resolve the Context.
        /// Resolves the Preference order and fixes the context subelements
        /// The subelements if mentioned have greater priority over the Reference attribute.
        /// </summary>
        public void ResolveContext()
        {
            Context tCtx;
            if (definitions.ContainsID(contextRef))
            {
                tCtx = definitions.GetContext(contextRef);
                if (tCtx != null)
                {
                    if (!containsbrush && tCtx.BrushElement!=null)
                    {
                        brush = tCtx.BrushElement;
                    }
                    if (!containscanvas && tCtx.CanvasElement!=null)
                    {
                        canvas = tCtx.CanvasElement;
                    }
                    if (!containsinkSource && tCtx.InksourceElement!=null)
                    {
                        inkSource = tCtx.InksourceElement;
                    }
                    if (!containstraceFormat && tCtx.TraceFormatElement!=null)
                    {
                        traceFormat = tCtx.TraceFormatElement;
                    }
                }
                else
                {
                    throw new Exception("Invalid Id.");
                }
            }
            else
            {
                throw new Exception("Invalid Id");
            }
        }

        /// <summary>
        /// Function to Resolve the Brush Element
        /// </summary>
        public void ResolveBrush()
        {
            Brush tBr;
            if (definitions.ContainsID(brushRef))
            {
                tBr= definitions.GetBrush(brushRef);
                if (tBr != null)
                {
                    if (!containsbrush)
                    {
                        brush = tBr;
                    }
                }
                else
                {
                    throw new Exception("Invalid Brush Id.");
                }
            }
            else
            {
                throw new Exception("Invalid Id.");
            }
        }

        /// <summary>
        /// Function to Resolve the Canvas Element
        /// </summary>
        public void ResolveCanvas()
        {
            Canvas tcanvas;
            if (definitions.ContainsID(canvasRef))
            {
                tcanvas = definitions.GetCanvas(canvasRef);
                if (tcanvas != null)
                {
                    if (!containscanvas)
                    {
                        canvas = tcanvas;
                    }
                }
                else
                {
                    throw new Exception("Invalid Canvas Id.");
                }
            }
            else
            {
                throw new Exception("Invalid Id.");
            }
        }

        /// <summary>
        /// Function to resolve the InkSource element from the Ref and subelement
        /// </summary>
        public void ResolveInkSource()
        {
            InkSource tIS;
            if (definitions.ContainsID(inkSourceRef))
            {
                tIS = definitions.GetInkSource(inkSourceRef);
                if (tIS != null)
                {
                    if (!containsinkSource)
                    {
                        inkSource = tIS;
                    }
                }
                else
                {
                    throw new Exception("Invalid InkSource Id.");
                }
            }
            else
            {
                throw new Exception("Invalid Id.");
            }
        }

        /// <summary>
        /// Function to resolve the TraceFormat of the Context from the subelement and Ref
        /// </summary>
        public void ResolveTraceFormat()
        {
            TraceFormat tTraceFormat;
            if (definitions.ContainsID(traceFormatRef))
            {
                tTraceFormat = definitions.GetTraceFormat(traceFormatRef);
                if (tTraceFormat != null)
                {
                    if (!containsbrush)
                    {
                        traceFormat= tTraceFormat;
                    }
                }
                else
                {
                    throw new Exception("Invalid TraceFormat Id.");
                }
            }
            else
            {
                throw new Exception("Invalid Id.");
            }
        }

        #endregion Resolve Functions
    }
}
