/*
 * @(#) Canvas.cs 1.0 06-08-2007 author: Manoj Prasad
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

    public class Canvas:InkElement
    {
        #region Fields 
        private string id="";
        private string traceFormatRef="";
        private bool containstraceFormat = false;
        private Definitions definitions;
        //[Del]private List<Channel> channelList;
        private TraceFormat traceFormat;

        /// <summary>
        /// Get/Sets the Id of the Brush Element
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                if (!definitions.ContainsID(value))
                {
                    id = value;
                    definitions.AddCanvas(this);
                }
                else
                {
                    throw new Exception("ID Exists.");
                }
            }
        }

        /// <summary>
        /// Gets/Sets the TraceFormatRef Attribute of the Canvas Element
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
        /// Gets/Sets the TraceFormat of the Canvas Element
        /// </summary>
        public TraceFormat TraceFormat
        {
            get { return traceFormat; }
            set { traceFormat = value; }
        }

        #endregion Fields

        #region Constructor

        public Canvas(Definitions defs)
        {
            this.TagName = "canvas";
            this.definitions = defs;
        }

        public Canvas(Definitions defs, string id)
        {
            this.TagName = "canvas";
            this.definitions = defs;
            this.id = id;
            definitions.AddCanvas(this);
        }

        public Canvas(Definitions defs,XmlElement element)
        {
            this.TagName = "canvas";
            this.definitions = defs;
            ParseElement(element);
        }

        #endregion Constructor

        #region override Functions Parse and ToinkML functions
        /// <summary>
        /// Function to Parse element and create Canvas Object.
        /// </summary>
        /// <param name="element">Element to be parsed</param>
        public override void ParseElement(XmlElement element)
        {
            if (element.LocalName.Equals("canvas"))
            {
                this.id = element.GetAttribute("id");
                if (!id.Equals(""))
                {
                    if (!definitions.ContainsID(id))
                    {
                        definitions.AddCanvas(this);
                    }
                    else
                    {
                        throw new Exception("ID not Unique.");
                    }
                }
                this.traceFormatRef = element.GetAttribute("traceFormatRef");
                if (!traceFormatRef.Equals(""))
                {
                    ResolveTraceFormat();
                }
                if (element.SelectSingleNode("./traceFormat") != null)
                {
                    containstraceFormat = true;
                    traceFormat = new TraceFormat(definitions,element.SelectSingleNode("./traceFormat") as XmlElement);
                }
            }
            else
            {
                throw new Exception("Invalid Element Name.");
            }


        }

        /// <summary>
        /// Function to Convert the Canvas Object to xmlELement
        /// </summary>
        /// <param name="inkDocument"> Ink Document</param>
        /// <returns>Canvas Xml Element</returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("canvas");
            if (!id.Equals(""))
            {
                result.SetAttribute("id", id);
            }
            if (!traceFormatRef.Equals(""))
            {
                result.SetAttribute("traceFormatRef", traceFormatRef);
            }
            if (containstraceFormat)
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
            return result;
        }

        #endregion override Functions Parse and ToinkML functions

        #region Resolve Functions
        /// <summary>
        /// Function to resolve the TraceFormat
        /// </summary>
        public void ResolveTraceFormat()
        {

            if (!definitions.ContainsID(traceFormatRef))
            {
                if (definitions.GetTraceFormat(traceFormatRef) != null && !containstraceFormat)
                {
                    traceFormat = definitions.GetTraceFormat(traceFormatRef);
                }
            }
            else
            {
                throw new Exception(" Invalid TraceFormatRef");
            }
        }

        
        /// <summary>
        /// Function to get the Default Canvas.
        /// </summary>
        /// <param name="defs"></param>
        /// <returns></returns>
        public static Canvas GetDefaultCanvas(Definitions defs)
        {
            Canvas defaultcanvas = new Canvas(defs, "DefaultCanvas");
            defaultcanvas.TraceFormatRef = "#DefaultTraceformat";
            return defaultcanvas;
        }
        #endregion Resolve Functions
    }

}
