/*
 * @(#) Annotation.cs 1.0 06-08-2007 author: Manoj Prasad
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

using System.Collections;
using System.Xml;

namespace InkML
{
	/// <summary>
	///
	/// </summary>

    public class Annotation :InkElement
    {
        private XmlElement annotation;
        
        /// <summary>
        /// Gets/Sets the Type of the Annotations
        /// </summary>
        public string Type
        {
            get { return annotation.GetAttribute("type"); }
            set { annotation.SetAttribute("type", value); }
        }

        /// <summary>
        /// Gets/Sets the Encoding Attribute of the Annotation
        /// </summary>
        public string Encoding
        {
            get { return annotation.GetAttribute("encoding"); }
            set { annotation.SetAttribute("encoding", value); }
        }

        /// <summary>
        /// Gets/Sets the Value of the Annotation Tag
        /// </summary>
        public string AnnotationTextValue
        {
            get { return annotation.Value; }
            set { annotation.Value = value; }
        }

        /// <summary>
        /// Gets/Sets the Whole Annotation xmlElement
        /// </summary>
        public XmlElement AnnotationElement
        {
            get { return annotation; }
            set { annotation = value; }
        }

        public Annotation(XmlElement annotation)
        {
            base.TagName = "annotation";
            this.annotation = annotation;
        }

        public Annotation()
        {
            base.TagName = "annotation";
            XmlDocument xd = new XmlDocument();
            this.annotation=xd.CreateElement("annotation");
        }

        public override void ParseElement(XmlElement element)
        {
        }

        /// <summary>
        /// Function to return the equivalent xmlElement for the Annotation object
        /// </summary>
        /// <param name="inkDocument"></param>
        /// <returns></returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            return inkDocument.ImportNode(annotation,true) as XmlElement;
        }

        /// <summary>
        /// Function to get the Attribute value from the annotation Element
        /// </summary>
        /// <param name="attributeName"> Name of the Attribute </param>
        /// <returns>Value of the Attribute </returns>
        public string GetAttribute(string attributeName)
        {
            return annotation.GetAttribute(attributeName);
        }

        /// <summary>
        /// Function to set a attribute value in the annotation element
        /// </summary>
        /// <param name="attributeName">Name of the Attribute</param>
        /// <param name="value">Value of the Attribute</param>
        public void SetAttribute(string attributeName, string value)
        {
            annotation.SetAttribute(attributeName, value);
        }

        /// <summary>
        /// Gets the Enumerator to access all the attributes.
        /// </summary>
        /// <returns>IEnumerator</returns>
        public IEnumerator GetAllAttributes()
        {
            return annotation.Attributes.GetEnumerator();
        }
    }
}
