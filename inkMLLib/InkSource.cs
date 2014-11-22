/*
 * @(#) InkSource.cs 1.0 06-08-2007 author: Manoj Prasad
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

    public class InkSource : InkElement
    {

        #region Fields
        private string id;
        private string manufacturer;
        private string model;
        private string serialNo;
        private string specificationRef;
        private string description;
        private TraceFormat traceFormat;
        private Definitions definitions;

        /// <summary>
        /// Gets/Sets the 'id' attribute of the InkSource Element
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                if (!definitions.ContainsID(value))
                {
                    id = value;
                    definitions.AddInkSource(this);
                }
                else
                {
                    throw new Exception("ID Exists.");
                }
            }
        }

        /// <summary>
        /// Gets/Sets the 'manufacturer' attribute of the InkSource Element
        /// </summary>
        public string Manufacturer
        {
            get { return manufacturer; }
            set { manufacturer = value; }
        }

        /// <summary>
        /// Gets/Sets the 'model' attribute of the InkSource Element
        /// </summary>      
        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        /// <summary>
        /// Gets/Sets the 'serialNo' attribute of the InkSource Element
        /// </summary>
        public string SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }

        /// <summary>
        /// Gets/Sets the 'specificationRef' attribute of the InkSource Element
        /// </summary>
        public string SpecificationRef
        {
            get { return specificationRef; }
            set { specificationRef = value; }
        }

        /// <summary>
        /// Gets/Sets the 'desc' attribute of the InkSource Element
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets/Sets the traceFormat element of the InkSource Element
        /// </summary>
        public TraceFormat TraceFormat
        {
            get { return traceFormat; }
            set
            {
                if (value != null)
                {
                    traceFormat = value;
                }
                else
                {
                    throw new Exception("Invalid Assignment Error.");
                }
            }
        }

        #endregion Fields

        #region Contructors
        public InkSource(Definitions defs)
        {
            base.TagName = "inkSource";
            this.definitions = defs;
        }

        public InkSource(Definitions defs, XmlElement element)
            : this(defs)
        {

            ParseElement(element);
        }

        public InkSource(Definitions defs, string Id, TraceFormat traceFormat)
            : this(defs)
        {

            this.id = Id;
            this.traceFormat = traceFormat;


        }

        #endregion Contructors

        #region override Functions ParseElement and ToInkML

        /// <summary>
        /// Function to Parse a xmkelement and fill inksource details
        /// </summary>
        /// <param name="element">Xml Element to be Parsed</param>
        public override void ParseElement(XmlElement element)
        {
            if (element.LocalName.Equals("inkSource"))
            {
                id = element.GetAttribute("id");
                if (id == "")
                    id = element.GetAttribute("id", "http://www.w3.org/XML/1998/namespace");
                if (id.Equals(""))
                {
                    throw new Exception("Required Field ID not present.");
                }
                else
                {
                    definitions.AddInkSource(this);
                }
                manufacturer = element.GetAttribute("manufacturer");
                model = element.GetAttribute("model");
                serialNo = element.GetAttribute("serialNo");
                specificationRef = element.GetAttribute("specificationRef");
                description = element.GetAttribute("description");

                bool found = false;
                foreach (XmlElement item in element)
                {
                    if (item.LocalName == "traceFormat")
                    {
                        this.traceFormat = new TraceFormat(definitions, item);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("TraceFormat element Required.");
                }

            }
            else
            {
                throw new Exception("Invalid Element Name.");
            }
        }

        /// <summary>
        /// Function to convert InkSource object to xml Element
        /// </summary>
        /// <param name="inkDocument">Ink Document</param>
        /// <returns>Ink Source Xml Element</returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("inkSource");
            result.SetAttribute("id", id);
            if (!manufacturer.Equals(""))
            {
                result.SetAttribute("manufacturer", manufacturer);
            }
            if (!model.Equals(""))
            {
                result.SetAttribute("model", model);
            }
            if (!serialNo.Equals(""))
            {
                result.SetAttribute("serialNo", serialNo);
            }
            if (!specificationRef.Equals(""))
            {
                result.SetAttribute("specificationRef", specificationRef);
            }
            if (!description.Equals(""))
            {
                result.SetAttribute("description", description);
            }
            if (traceFormat.Id.Equals(""))
            {
                result.AppendChild(traceFormat.ToInkML(inkDocument));
            }
            else
            {
                XmlElement temp = inkDocument.CreateElement("traceFormat");
                temp.SetAttribute("hRef", "#" + id);
                result.AppendChild(temp);
            }
            return result;
        }

        #endregion override Functions ParseElement and ToInkML
    }
}
