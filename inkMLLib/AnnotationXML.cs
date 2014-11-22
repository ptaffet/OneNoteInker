/*
 * @(#) AnnotationXML.cs 1.0 06-08-2007 author: Manoj Prasad
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
using System.Collections;

/** This class models the <annotationXML> element in InkML. 
 * It is a child element to <brush>, <traceGroup> and <traceView> InML elements.
 * It maintains a HashMap with entry for each child element of <annotationXML> element.
 * An entry in the map has the tagName of the child element of <annotationXML> 
 * as the key and the text node value as the Value.
 */
namespace InkML
{
	/// <summary>
	///
	/// </summary>

    public class AnnotationXML:InkElement
    {
        //private Dictionary<String, String> otherAttributesMap;
        //private Dictionary<String, XmlElement> propertyElementsMap;
        private XmlElement annotationXML;
        private XmlDocument tempDocument;

        /// <summary>
        /// Gets/Sets the Type of the AnnotationXML
        /// </summary>
        public string Type
        {
            get { return annotationXML.GetAttribute("type"); }
            set { annotationXML.SetAttribute("type", value); }
        }

        /// <summary>
        /// Gets/Sets the Encoding Attribute of the AnnotationXML
        /// </summary>
        public string Encoding
        {
            get { return annotationXML.GetAttribute("encoding"); }
            set { annotationXML.SetAttribute("encoding", value); }
        }        

        /// <summary>
        /// Gets/Sets the hRef attribute of the AnnotationXML
        /// </summary>
        public string HRef
        {
            get { return annotationXML.GetAttribute("hRef"); }
            set { annotationXML.SetAttribute("hRef", value); }
        }

        /// <summary>
        /// Gets/Sets the AnnotationXML xmlElement
        /// </summary>
        public XmlElement AnnotationElement
        {
            get { return annotationXML; }
            set { annotationXML = value; }
        }
        /// <summary>
        /// Constructor to create an empty AnnotationXML object.
        /// 
        /// </summary>
        public AnnotationXML()
        {
            base.TagName = "annotationXML";
            tempDocument= new XmlDocument();
            this.annotationXML = tempDocument.CreateElement("annotationXML");
        }


        /// <summary>
        /// Constructor to Parse XmlElement and construct the AnnotationXML object
        /// </summary>
        /// <param name="element">Element to be parsed</param>
        public AnnotationXML(XmlElement element)
        {
            base.TagName = "annotationXML";
            this.annotationXML = element;            
        }

        /// <summary>
        /// Function to parse the Xmlelement.
        /// </summary>
        /// <param name="Element">Annotation XMLElement to be parsed</param>
        public override void ParseElement(XmlElement Element)
        {
                    
           
        }

        

        /// <summary>
        /// Function to find and return the Attribute Value
        /// </summary>
        /// <param name="attributeName">Name of the Attribute to find</param>
        /// <returns>Value of the Attribute</returns>
        public string GetAttribute(string attributeName)
        {
            return annotationXML.GetAttribute(attributeName);
        }

        /// <summary>
        /// Function to Set/Add attribute to the Annotation element
        /// </summary>
        /// <param name="attributeName">Name of the Attribute</param>
        /// <param name="value">Value of the Attribute</param>
        public void SetAttribute(string attributeName, string value)
        {
            annotationXML.SetAttribute(attributeName, value);            
        }


        /// <summary>
        /// Function to convert AnnotationXML object to the corresponding xmlElement
        /// </summary>
        /// <param name="inkDocument"></param>
        /// <returns></returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            //XmlElement result = inkDocument.CreateElement("annotationXML");
            return inkDocument.ImportNode(annotationXML, true) as XmlElement;
        }

        /// <summary>
        /// Adds a Property subelement to the AnnotationXML element
        /// </summary>
        /// <param name="PropertyName">Property to be added as the Subelement</param>
        /// <param name="value">Value of the Property</param>

        public void AddProperty(string PropertyName, string value)
        {
            XmlElement property = tempDocument.CreateElement(PropertyName);
            XmlText xmlvalue = tempDocument.CreateTextNode(value);
            property.AppendChild(xmlvalue);
            annotationXML.AppendChild(property);
        }

        /// <summary>
        /// Gets the Property of the Elements added to the AnnotationXML
        /// </summary>
        /// <param name="PropertyName">Name of the Property to be searched</param>
        /// <returns>Value of the Property</returns>
        public string GetProperty(string PropertyName)
        {
            try
            {
                XmlNode result = annotationXML.SelectSingleNode("./" + PropertyName);
                return result.InnerText;
            }
            catch (Exception )
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an Enumerator to access the Elements of the AnnotationXML element
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetAllProperties()
        {
            XmlNodeList result = annotationXML.SelectNodes("./*");
            return result.GetEnumerator();
        }

        /// <summary>
        /// Gets the Enumerator to access all the attributes.
        /// </summary>
        /// <returns>IEnumerator</returns>
        public IEnumerator GetAllAttributes()
        {
            return annotationXML.Attributes.GetEnumerator();
        }
    }
}
