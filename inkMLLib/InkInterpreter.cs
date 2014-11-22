/*
 * @(#) InkInterpreter.cs 1.0 06-08-2007 author: Manoj Prasad
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
using System.IO;

namespace InkML
{
	/// <summary>
	///
	/// </summary>

    public class InkInterpreter
    {
        #region Fields
        //public event EventHandler<StrokeEventArgs> StrokeReceived;
        //public event EventHandler<TraceEventArgs> TraceReceived;
        //public event EventHandler<TraceGroupEventArgs> TraceGroupReceived;
        //public event EventHandler<ContextEventArgs> ContextReceived;
        private Ink ink;
        private int id;
        private Context currentContext;
        private string currentBrushRef;
        private string currentContextRef;
        private string currentTraceFormatRef;
        private string currentCanvasRef;
        private string currentInkSourceRef;
       
        private bool contextChange;
        private Definitions definitions;

        public Definitions Defs
        {
            get { return definitions; }
        }

        public Ink Ink
        {
            get { return ink; }
        }

        #endregion Fields

        #region Constructor
        public InkInterpreter()
        {
            id = 0;
            currentContext = new Context(definitions);
            definitions = new Definitions();
            ink = new Ink(definitions);
            currentContext.TraceFormatElement = TraceFormat.GetDefaultTraceFormat(definitions);
            currentContext.CanvasElement=Canvas.GetDefaultCanvas(definitions);

        }

        #endregion Constructor

        #region Load and Save Files
        public void LoadInkFile(string FileName)
        {
            XmlDocument inkDocument = new XmlDocument();
            inkDocument.Load(FileName);
            ParseInk(inkDocument.DocumentElement,true);
        }

        public void SaveInk(string FileName)
        {
            XmlDocument inkDocument = new XmlDocument();            
            inkDocument.AppendChild(ink.ToInkML(inkDocument));
            inkDocument.Save(FileName);
        }
        public string SaveInk()
        {
            XmlDocument inkDocument = new XmlDocument();
            inkDocument.AppendChild(ink.ToInkML(inkDocument));
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                inkDocument.WriteTo(xmlTextWriter);
                xmlTextWriter.Close();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        #endregion Load and Save Files

        #region Parsing Functions

        /// <summary>
        /// Function to add xmlElements to the Ink collection in the interpreter
        /// This function Parses the InkML fragments and creates a Ink object tree.
        /// </summary>
        /// <param name="element">Element to be added</param>
        /// <param name="root"> Value representing whether the Element added is a root element?</param>
        public void ParseInk(XmlElement element,bool root)
        {
            if (root)
            {
                foreach (XmlNode Node in element)
                {
                    ParseInk(Node as XmlElement);
                }
            }
            else
            {
                ParseInk(element);
            }
        }

        /// <summary>
        /// Function to add xmlelement to the Ink Collection
        /// </summary>
        /// <param name="element"></param>
        public void ParseInk(XmlElement element)
        {
            if (element.LocalName.Equals("trace"))
            {
                Trace trace = new Trace(definitions,currentContext, element);                
                ink.AddTrace(trace);                
            }
            else if (element.LocalName.Equals("traceGroup"))
            {
                TraceGroup traceGroup = new TraceGroup(definitions,currentContext, element);                
                ink.AddTraceGroup(traceGroup);                
            }
            else if (element.LocalName.Equals("context"))
            {
                Context context = new Context(definitions, element);

                UpdateCurrentContext(context);
                ink.AddContext(context);
            }            
            else if (element.LocalName.Equals("annotation"))
            {
                Annotation annotation = new Annotation(element);
                ink.AddAnnotation(annotation);
            }
            else if (element.LocalName.Equals("annotationXML"))
            {
                AnnotationXML annotationXML = new AnnotationXML(element);
                ink.AddAnnotationXML(annotationXML);
            }
            else if (element.LocalName.Equals("definitions"))
            {
                Definitions defs = new Definitions(element);
                Dictionary<string,InkElement>.Enumerator enumdef = defs.GetDefinitions();
                while (enumdef.MoveNext())
                {
                    definitions.AddInkElement(enumdef.Current.Key, enumdef.Current.Value);
                }
                ink.AddDefinitions(defs);
            }
            else
            {
                throw new Exception("Invalid Ink Element.");
            }


        }

        public void ParseInk(string ink, bool root)
        {
            XmlDocument inkdocument = new XmlDocument();
            inkdocument.LoadXml(ink);
            ParseInk(inkdocument.DocumentElement);
        }

        public void ParseInk(InkElement element)
        {
            Trace trace;
            if (element.TagName.Equals("trace"))
            {
                if (contextChange)
                {
                    
                    Context context = new Context(definitions, "ctx" + id.ToString());
                    IncrementID();
                    if (currentBrushRef != null)
                    {
                        context.BrushRef = currentBrushRef;
                    }
                    if (currentTraceFormatRef != null)
                    {
                        context.TraceFormatRef = currentTraceFormatRef;
                    }
                    if (currentCanvasRef != null)
                    {
                        context.CanvasRef = currentCanvasRef;
                    }
                    if (currentInkSourceRef != null)
                    {
                        context.InkSourceRef = currentInkSourceRef;
                    }
                    currentContextRef = "#" + context.Id;
                    ink.AddDefinitions(context);
                }
                trace = element as Trace;
                trace.BrushRef = currentBrushRef;
                trace.ContextRef = currentContextRef;
                ink.AddTrace(trace);
                contextChange = false;
            }
            else if (element.TagName.Equals("traceGroup"))
            {
                if (contextChange)
                {
                    Context context = new Context(definitions, "ctx" + System.Convert.ToString(id));
                    IncrementID();
                    context.BrushRef = currentBrushRef;
                    context.TraceFormatRef = currentTraceFormatRef;
                    context.CanvasRef = currentCanvasRef;
                    context.InkSourceRef = currentInkSourceRef;
                    currentContextRef = "#" + context.Id;
                    ink.AddDefinitions(context);
                }

                TraceGroup traceGroup = element as TraceGroup;
                traceGroup.BrushRef = currentBrushRef;
                traceGroup.ContextRef = currentContextRef;
                ink.AddTraceGroup(traceGroup);
                contextChange = false;
                //StrokeReceived(this, new StrokeEventArgs(traceGroup));
                //TraceGroupReceived(this, new TraceGroupEventArgs(traceGroup));
            }
            else if (element.TagName.Equals("brush"))
            {
                Brush tbr = (Brush)element;
               
                tbr.Id = "br" + System.Convert.ToString(id);
                IncrementID();
                
                currentBrushRef = "#" + tbr.Id;
                ink.AddDefinitions(tbr);
            }
            else if (element.TagName.Equals("traceFormat"))
            {
                TraceFormat ttf = (TraceFormat)element;
                
                ttf.Id = "tf" + System.Convert.ToString(id);
                IncrementID();
                
                contextChange = true;
                ink.AddDefinitions(ttf);
                currentTraceFormatRef = "#" + ttf.Id;
            }
            else if (element.TagName.Equals("inkSource"))
            {
                InkSource tis = (InkSource)element;
                
                tis.Id = "is" + System.Convert.ToString(id);
                IncrementID();
                
                contextChange = true;
                ink.AddDefinitions(tis);
                currentInkSourceRef = "#" + tis.Id;
            }
            else if (element.TagName.Equals("canvas"))
            {
                Canvas canvas = (Canvas)element;
                
                canvas.Id = "cs" + System.Convert.ToString(id);
                IncrementID();
                
                contextChange = true;
                ink.AddDefinitions(canvas);
                currentCanvasRef = "#" + canvas.Id;
            }
            else if (element.TagName.Equals("annotation"))
            {
                Annotation annotation = element as Annotation ;
                ink.AddAnnotation(annotation);
            }
            else if (element.TagName.Equals("annotationXML"))
            {
                AnnotationXML annotationXML = element as AnnotationXML;
                ink.AddAnnotationXML(annotationXML);
            }
            else
            {
                throw new Exception("Invalid Ink Element.");
            }
        }
        #endregion Parsing Functions


        private void UpdateCurrentContext(Context context)
        {
            if (context.BrushElement != null)
            {
                currentContext.BrushElement = context.BrushElement;
            }
            if (context.TraceFormatElement != null)
            {
                currentContext.TraceFormatElement = context.TraceFormatElement;
            }
            if (context.CanvasElement != null)
            {
                currentContext.CanvasElement = context.CanvasElement;
            }
            if (context.InksourceElement != null)
            {
                currentContext.InksourceElement = context.InksourceElement;
            }
        }

        private void IncrementID()
        {
            id++;
        }

    }

    #region Event Argument Classes 
    public class StrokeEventArgs : EventArgs
    {
        private Stroke stroke;

        public Stroke Stroke
        {
            get { return stroke; }
        }

        public StrokeEventArgs(Stroke stroke)
        {
            this.stroke = stroke;
        }
    }

    public class TraceEventArgs : EventArgs
    {
        private Trace trace;

        public Trace Trace
        {
            get { return trace; }
        }

        public TraceEventArgs(Trace trace)
        {
            this.trace = trace;
        }
    }

    public class TraceGroupEventArgs : EventArgs
    {
        private TraceGroup traceGroup;

        public TraceGroup TraceGroup
        {
            get { return traceGroup; }
        }

        public TraceGroupEventArgs(TraceGroup traceGroup)
        {
            this.traceGroup = traceGroup;
        }
    }

    public class ContextEventArgs : EventArgs
    {
        private Context context;

        public Context Context
        {
            get { return context; }
        }

        public ContextEventArgs(Context context)
        {
            this.context = context;
        }
    }
    #endregion Event Argument Classes
}
