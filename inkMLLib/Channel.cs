/*
 * @(#) Channel.cs 1.0 06-08-2007 author: Manoj Prasad
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

    public class Channel:InkElement
    {
        #region Fields
        private string name = "";
        private string id = "";
        private ChannelType channelType = ChannelType.INTEGER;
        private string min = "";
        private string max = "";
        private string defaultValue="";
        private string units = "";          // Change it to  Enumerations
        private OrientationType orientation = OrientationType.POSITIVE;
        private string respectTo="";
        private TraceQualifiers qualifier;


        public TraceQualifiers Qualifier
        {
            get { return qualifier; }
            set { this.qualifier = value; }
        }

        /// <summary>
        /// Gets/Sets the Name attribute of the Channel Element
        /// </summary>
        public string Name
        {
            get { return name; }            
        }

        /// <summary>
        /// Gets/Sets the 'id' attribute of the Channel Element
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets/Sets the 'type' attribute of the Channel Element
        /// </summary>
        public ChannelType Type
        {
            get { return channelType; }
            set { channelType = value; }
        }

        /// <summary>
        /// Gets/Sets the 'max' attribute of the Channel Element
        /// </summary>
        public string Max
        {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        /// Gets/Sets the 'min' attribute of the Channel Element
        /// </summary>
        public string Min
        {
            get { return min; }
            set { min = value; }
        }

        /// <summary>
        /// Gets/Sets the 'default' attribute of the Channel Element
        /// </summary>
        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        /// <summary>
        /// Gets/Sets the 'units' attribute of the Channel Element
        /// </summary>
        public string Units
        {
            get { return units; }
            set { units = value; }
        }

        /// <summary>
        /// Gets/Sets the 'orientation' attribute of the Channel Element
        /// </summary>
        public OrientationType Orientation
        {
            get { return orientation; }
            set { orientation=value;}
        }

        /// <summary>
        /// Gets/Sets the 'respectTo' attribute of the Channel Element
        /// </summary>
        public string RespectTo
        {
            get { return respectTo; }
            set { respectTo = value; }
        }

        #endregion Fields

        #region Constructor
        public Channel(string name, string id, ChannelType type, string min,
            string max, string units, OrientationType orientation,
            string respectTo)
        {
            base.TagName = "channel";
            this.name = name;
            this.id = id;
            this.channelType = type;
            //if (ChannelType.BOOLEAN == this.channelType)
            //    this.defaultValue = "false";
            //else if ((ChannelType.DECIMAL == this.channelType) || (ChannelType.INTEGER == this.channelType))
            //    this.defaultValue = "0";
            this.min = min;
            this.max = max;
            this.orientation = orientation;
            this.respectTo = respectTo;
            this.units = units;
            qualifier = TraceQualifiers.Velocity;
        }

        public Channel(string name, ChannelType type)
        {
            base.TagName = "channel";
            this.name = name;            
            this.channelType = type;
            this.orientation = OrientationType.POSITIVE;
            qualifier = TraceQualifiers.Velocity;
        }

        public Channel(XmlElement element)
        {
            this.TagName = "channel";
            ParseElement(element);
            qualifier = TraceQualifiers.Velocity;
        }

        #endregion Constructor

        #region Override MEthods
        /// <summary>
        /// Function to Parse the Channel Element
        /// </summary>
        /// <param name="element">xml Element to be parsed</param>
        public override void ParseElement(XmlElement element)
        {
            if (element.LocalName.Equals("channel"))
            {
                name = element.GetAttribute("name");
                if (element.HasAttribute("id"))
                {
                    id = element.GetAttribute("id");
                }
                if (element.HasAttribute("type"))
                {
                    this.channelType = (ChannelType)Enum.Parse(typeof(ChannelType), element.GetAttribute("type").ToUpper());
                }
                if (element.HasAttribute("default"))
                {
                    this.defaultValue = element.GetAttribute("default");
                }
                if (element.HasAttribute("max"))
                {
                    this.max = element.GetAttribute("max");
                }
                if (element.HasAttribute("min"))
                {
                    this.min = element.GetAttribute("min");
                }
                if (element.HasAttribute("respectTo"))
                {
                    this.respectTo = element.GetAttribute("respectTo");
                }
                if (element.HasAttribute("units"))
                {
                    this.units = element.GetAttribute("units");
                }
                if (element.HasAttribute("orientation"))
                {
                    String attrValue = element.GetAttribute("orientation").ToUpper();
                    if(attrValue.Equals("+VE"))
                        this.orientation = OrientationType.POSITIVE;
                    if(attrValue.Equals("-VE"))
                        this.orientation = OrientationType.NEGATIVE;            
                }
            }
            else
            {
                throw new Exception(" Invalid Element Name.");
            }

        }

        /// <summary>
        /// Function to convert Channel object to Xml ELement
        /// </summary>
        /// <param name="inkDocument"></param>
        /// <returns></returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("channel");
            if (!name.Equals(""))
            {
                result.SetAttribute("name", name);
            }
            if (!id.Equals(""))
            {
                result.SetAttribute("id", id);
            }            
            result.SetAttribute("type", channelType.ToString());
            if (!DefaultValue.Equals(""))
            {
                result.SetAttribute("default", defaultValue);
            }
            if (!max.Equals(""))
            {
                result.SetAttribute("max", max);
            }
            if (!min.Equals(""))
            {
                result.SetAttribute("min", min);
            }
            if (orientation.Equals(OrientationType.POSITIVE))
            {
                result.SetAttribute("orientation", "+ve");
            }
            else
            {
                result.SetAttribute("orientation", "-ve");
            }

            if (!respectTo.Equals(""))
            {
                result.SetAttribute("respectTo", respectTo);
            }
            if (!units.Equals(""))
            {
                result.SetAttribute("units", units);
            }
            return result;
        }

        #endregion Override MEthods

    }
}
