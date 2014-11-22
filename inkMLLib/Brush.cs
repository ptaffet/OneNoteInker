/*
 * @(#) Brush.cs 1.0 06-08-2007 author: Manoj Prasad
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

    public class Brush :InkElement
    {
        #region Fields
        private Annotation annotation;
        private AnnotationXML annotationXML;
        private string id="";
        private string brushRef="";
        private bool containsA;
        private bool containsAX;
        private Definitions definitions;

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
                    definitions.AddBrush(this);
                }
                else
                {
                    throw new Exception("ID Exists.");
                }
            }
        }

        /// <summary>
        /// Gets/Sets the BrushRef attribute of the Brush Element
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
        /// Gets/Sets the Annotation in the BrushELement
        /// </summary>
        public Annotation BrushAnnotation
        {
            get { return annotation; }
            set
            {
                annotation = value;
                containsA = true;
            }
        }

        /// <summary>
        /// Gets/Sets the AnnotationXML in the Brush ELement
        /// </summary>
        public AnnotationXML BrushAnnotationXML
        {
            get { return annotationXML; }
            set
            {
                annotationXML = value;
                containsAX = true;
            }
        }

        #endregion Fields

        #region Constructors
        public Brush(Definitions defs)
        {
            base.TagName = "brush";
            this.definitions = defs;
            id = "";
        }

        public Brush(Definitions defs, string id)
        {
            base.TagName = "brush";
            this.definitions = defs;
            this.id=id;
        }

        public Brush(Definitions defs, XmlElement element)
        {
            base.TagName = "brush";
            this.definitions = defs;
            ParseElement(element);

        }

        #endregion Constructors

        #region Override Function Parse and ToinkML
        /// <summary>
        /// Function to parse the brush element and resolve the references.
        /// </summary>
        /// <param name="element">XmlElement to be Parsed</param>
        public override void ParseElement(XmlElement element)
        {
            if (element.LocalName.Equals("brush"))
            {
                this.id = element.GetAttribute("id");
                if (id == "")
                    id = element.GetAttribute("id", "http://www.w3.org/XML/1998/namespace");
                if (!id.Equals(""))
                {
                    definitions.AddBrush(this);
                }
                this.brushRef = element.GetAttribute("brushRef");
                if (!brushRef.Equals(""))
                {
                    ResolveBrush();
                }
                if (element.SelectSingleNode("./annotation") != null)
                {
                    annotation = new Annotation(element.SelectSingleNode("./annotation") as XmlElement);
                    containsA = true;
                }
                else
                {
                    containsA = false;
                }
                if (element.SelectSingleNode("./annotationXML") != null)
                {
                    annotationXML = new AnnotationXML(element.SelectSingleNode("./annotationXML") as XmlElement);
                    containsAX = true;
                }
                else
                {
                    containsAX = false;
                }
            }
            else
            {
                throw new Exception("Invalid Element Name");
            }
        }

        /// <summary>
        /// Function to create equivalent xmlelement from the Brush object
        /// </summary>
        /// <param name="inkDocument">xmlDocument element</param>
        /// <returns>Brush XmlElement</returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("brush");
            if (!"".Equals(id))
            {
                result.SetAttribute("id", id);
            }
            if (!"".Equals(brushRef))
            {
                result.SetAttribute("brushRef", brushRef);
            }
            if (containsA)
            {
                result.AppendChild(annotation.ToInkML(inkDocument));
            }
            if (containsAX)
            {
                result.AppendChild(annotationXML.ToInkML(inkDocument));
            }
            return result;
        }

        #endregion Override Function Parse and ToinkML

        #region Resolve Functions
        /// <summary>
        /// Function to resolve the brush references
        /// </summary>
        public void ResolveBrush()
        {
            Brush temp = definitions.GetBrush(brushRef);
            if (!containsAX)
            {
                this.annotationXML=temp.annotationXML;
            }
            if (!containsA)
            {
                this.annotation = temp.annotation;
            }
        }

        #endregion Resolve Functions

    }
}
