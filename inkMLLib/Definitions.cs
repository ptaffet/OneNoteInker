/*
 * @(#) Definitions.cs 1.0 06-08-2007 author: Manoj Prasad
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
using System.Xml;


namespace InkML
{
	/// <summary>
	///
	/// </summary>

    public class Definitions :InkElement
    {
        #region Fields

        Dictionary<string, InkElement> IdMap;

        #endregion Fields

        #region Constructors
        public Definitions(XmlElement defs)
        {
            base.TagName = "Definitions";
            IdMap = new Dictionary<string, InkElement>();
            ParseElement(defs);

        }

        public Definitions()
        {
            base.TagName = "Definitions";
            IdMap = new Dictionary<string, InkElement>();

        }
        #endregion Constructors

        #region Add Ink Elements

        // If 2 elements have same key, one which is added last replaces the one which
        // been added earlier.

        /// <summary>
        /// Function to Add a InkELement to the Definitions
        /// </summary>
        /// <param name="id">Id of the element to be added</param>
        /// <param name="element">Ink Element to be added</param>
        public void AddInkElement(string id, InkElement element)
        {
            try
            {
                if (IdMap.ContainsKey(id))
                {
                    IdMap.Remove(id);
                }
                IdMap.Add(id, element);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Function to add a inkML.brush Element to the Definitions 
        /// </summary>
        /// <param name="brush">inkML.Brush element to be added</param>
        public void AddBrush(Brush brush)
        {
            try
            {
                if (IdMap.ContainsKey(brush.Id))
                {
                    IdMap.Remove(brush.Id);
                }
                IdMap.Add(brush.Id, brush);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// Function to add InkSource Element to the Definitions
        /// </summary>
        /// <param name="inkSource">InkSource Element to be added</param>
        public void AddInkSource(InkSource inkSource)
        {
            try
            {
                if (IdMap.ContainsKey(inkSource.Id))
                {
                    IdMap.Remove(inkSource.Id);
                }
                IdMap.Add(inkSource.Id, inkSource);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// Function to add traceFormat Element to the Definitions
        /// </summary>
        /// <param name="traceFormat">TraceFormat Element to be added</param>
        public void AddTraceFormat(TraceFormat traceFormat)
        {
            try
            {
                if (IdMap.ContainsKey(traceFormat.Id))
                {
                    IdMap.Remove(traceFormat.Id);
                }
                IdMap.Add(traceFormat.Id, traceFormat);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// Function to add Canvas Element to the Definitions
        /// </summary>
        /// <param name="canvas">Canvas Element to be added</param>
        public void AddCanvas(Canvas canvas)
        {
            try
            {
                if (IdMap.ContainsKey(canvas.Id))
                {
                    IdMap.Remove(canvas.Id);
                }
                IdMap.Add(canvas.Id, canvas);
            }
            catch (Exception)
            {
                
                throw;
            }
        }


        /// <summary>
        /// Function to add Context Element to the Definitions
        /// </summary>
        /// <param name="context">Context Element to be added</param>
        public void AddContext(Context context)
        {
            try
            {
                if (IdMap.ContainsKey(context.Id))
                {
                    IdMap.Remove(context.Id);
                }
                IdMap.Add(context.Id, context);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// Function to add traceGroup Element to the Definitions
        /// </summary>
        /// <param name="traceGroup">TraceGroup Element to be added</param>
        public void AddTraceGroup(TraceGroup traceGroup)
        {
            try
            {
                if (IdMap.ContainsKey(traceGroup.Id))
                {
                    IdMap.Remove(traceGroup.Id);
                }
                IdMap.Add(traceGroup.Id, traceGroup);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// Function to add trace Element to the Definitions
        /// </summary>
        /// <param name="trace">Trace Element to be added</param>
        public void AddTrace(Trace trace)
        {
            try
            {
                if (IdMap.ContainsKey(trace.Id))
                {
                    IdMap.Remove(trace.Id);
                }
                IdMap.Add(trace.Id, trace);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        #endregion Add Ink Elements

        #region Get Ink Elements

        /// <summary>
        /// Gets the Brush Element with given 'id'
        /// </summary>
        /// <param name="Id">Id of the Brush element</param>
        /// <returns>Brush element with id value = Id</returns>
        public Brush GetBrush(string Id)
        {
            InkElement result;
            if (IdMap.TryGetValue(Id.Substring(Id.IndexOf('#')+1), out result))
            {
                if (result.TagName.Equals("brush"))
                {
                    return result as Brush;
                }
                else
                {
                    throw new Exception("Invalid ID.");
                }
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// Gets the InkSource Element with the given Id.
        /// </summary>
        /// <param name="Id">Id value to be Searched</param>
        /// <returns>InkSource Element </returns>
        public InkSource GetInkSource(string Id)
        {
            InkElement result;
            if (IdMap.TryGetValue(Id.Substring(Id.IndexOf('#')+1), out result))
            {
                if (result.TagName.Equals("inkSource"))
                {
                    return result as InkSource;
                }
                else
                {
                    throw new Exception("Invalid ID.");
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the Canvas Element with the given Id
        /// </summary>
        /// <param name="Id">Id value to be Searched</param>
        /// <returns>Canvas Element</returns>
        public Canvas GetCanvas(string Id)
        {
            InkElement result;
            if (IdMap.TryGetValue(Id.Substring(Id.IndexOf('#') + 1), out result))
            {
                if (result.TagName.Equals("canvas"))
                {
                    return result as Canvas;
                }
                else
                {
                    throw new Exception("Invalid ID.");
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the TraceFormat Element with the given Id
        /// </summary>
        /// <param name="Id">Id value to be Searched</param>
        /// <returns>TraceFormat Element</returns>
        public TraceFormat GetTraceFormat(string Id)
        {
            InkElement result;
            if (IdMap.TryGetValue(Id.Substring(Id.IndexOf('#') + 1), out result))
            {
                if (result.TagName.Equals("traceFormat"))
                {
                    return result as TraceFormat;
                }
                else
                {
                    
                    throw new Exception("Invalid ID.");
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the Context Element with the given Id
        /// </summary>
        /// <param name="Id">Id value to be Searched</param>
        /// <returns>Context Element</returns>
        public Context GetContext(string Id)
        {
            InkElement result;
            if (IdMap.TryGetValue(Id.Substring(Id.IndexOf('#') + 1), out result))
            {
                if (result.TagName.Equals("context"))
                {
                    return result as Context;
                }
                else
                {
                    throw new Exception("Invalid ID.");
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the Trace Element with the given Id
        /// </summary>
        /// <param name="Id">Id value to be Searched</param>
        /// <returns>Trace Element</returns>
        public Trace GetTrace(string Id)
        {
            InkElement result;
            if (IdMap.TryGetValue(Id.Substring(Id.IndexOf('#') + 1), out result))
            {
                if (result.TagName.Equals("trace"))
                {
                    return result as Trace;
                }
                else
                {
                    throw new Exception("Invalid ID.");
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the TraceGroup Element with the given Id
        /// </summary>
        /// <param name="Id">Id value to be Searched</param>
        /// <returns>Tracegroup Element</returns>
        public TraceGroup GetTraceGroup(string Id)
        {
            InkElement result;
            if (IdMap.TryGetValue(Id.Substring(Id.IndexOf('#') + 1), out result))
            {
                if (result.TagName.Equals("traceGroup"))
                {
                    return result as TraceGroup;
                }
                else
                {
                    throw new Exception("Invalid ID.");
                }
            }
            else
            {
                return null;
            }
        }
 
        #endregion Get Ink Elements        

        #region Override Function Parse and ToInkML functions

        /// <summary>
        /// Function to parse the Definition Block 
        /// </summary>
        /// <param name="element">Element to be parsed</param>
        public override void ParseElement(XmlElement element)
        {
            inkml = new XmlNamespaceManager(element.OwnerDocument.NameTable);
            inkml.AddNamespace("inkml", "http://www.w3.org/2003/InkML");
            if (element.LocalName.Equals("definitions"))
            {
                foreach (XmlNode Node in element.ChildNodes)
                {
                    if (Node.LocalName.Equals("brush"))
                    {
                        Brush t = new Brush(this, (XmlElement)Node);
                        if (t.Id.Equals(""))
                        {
                            throw new Exception(" Invalid Input. No Id present in the Brush element of the Definition block.");
                        }
                    }
                    else if (Node.LocalName.Equals("inkSource"))
                    {
                        InkSource t = new InkSource(this, (XmlElement)Node);
                        if (t.Id.Equals(""))
                        {
                            throw new Exception(" Invalid Input. No Id present in the InkSource element of the Definition block.");
                        }
                    }
                    else if (Node.LocalName.Equals("canvas"))
                    {
                        Canvas t = new Canvas(this, (XmlElement)Node);
                        if (t.Id.Equals(""))
                        {
                            throw new Exception(" Invalid Input. No Id present in the Canvas element of the Definition block.");
                        }
                    }
                    else if (Node.LocalName.Equals("traceFormat"))
                    {
                        TraceFormat t = new TraceFormat(this, (XmlElement)Node);
                        if (t.Id.Equals(""))
                        {
                            throw new Exception(" Invalid Input. No Id present in the TraceFormat element of the Definition block.");
                        }
                    }
                    else if (Node.LocalName.Equals("context"))
                    {
                        Context t = new Context(this, (XmlElement)Node);
                        if (t.Id.Equals(""))
                        {
                            throw new Exception(" Invalid Input. No Id present in the Context element of the Definition block.");
                        }
                    }
                    if (Node.LocalName.Equals("trace"))
                    {
                        Trace t = new Trace(this, (XmlElement)Node);
                        if (t.Id.Equals(""))
                        {
                            throw new Exception(" Invalid Input. No Id present in the Trace element of the Definition block.");
                        }
                    }
                    if (Node.LocalName.Equals("traceGroup"))
                    {
                        TraceGroup t = new TraceGroup(this, (XmlElement)Node);
                        if (t.Id.Equals(""))
                        {
                            throw new Exception(" Invalid Input. No Id present in the Tracegroup element of the Definition block.");
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XmlElement.");
            }
        }


        /// <summary>
        /// Function to create a xmlElement corresponding to this Definitions object.
        /// </summary>
        /// <param name="inkDocument">ink Document</param>
        /// <returns>Definitions xmlElement</returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("definitions");
            Dictionary<string, InkElement>.Enumerator enummap = IdMap.GetEnumerator();
            while (enummap.MoveNext())
            {
                InkElement t = enummap.Current.Value;
                result.AppendChild(t.ToInkML(inkDocument));
            }
            
            return result;
        }
        #endregion Override Function Parse and ToInkML functions

        /// <summary>
        /// Function to remove a inkelement from the IdMap
        /// </summary>
        /// <param name="id">Id of the Element to be removed</param>
        public void Remove(string id)
        {
            IdMap.Remove(id);
        }

        /// <summary>
        /// Function to find id existing in the list.
        /// </summary>
        /// <param name="Id">Id to be checked in the list</param>
        /// <returns>Boolean value</returns>
        public bool ContainsID(string Id)
        {
            if (Id.IndexOf("#") >= 0)
            {
                return IdMap.ContainsKey(Id.Substring(Id.IndexOf("#") + 1));
            }
            else
            {
                return IdMap.ContainsKey(Id);
            }
        }

        /// <summary>
        /// Gets the Enumerator over the Definitions to access the Elements stored
        /// </summary>
        /// <returns>Dictionary<string,InkElement>.Enumerator</returns>
        public Dictionary<string,InkElement>.Enumerator GetDefinitions()
        {
            return IdMap.GetEnumerator();
        }
    
    }
}
