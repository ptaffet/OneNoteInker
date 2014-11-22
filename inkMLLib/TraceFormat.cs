/*
 * @(#) TraceFormat.cs 1.0 06-08-2007 author: Manoj Prasad
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

    public class TraceFormat:InkElement
    {

        #region Fields and Attributes
        private string id="";
        private List<Channel> RegularChannel;
        private List<Channel> IntermittentChannel;
        private Definitions definitions;
        private static TraceFormat defaultTF = null;

        /// <summary>
        /// Get/Set the id attribute of the traceFormat Element
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                if (!definitions.ContainsID(value))
                {
                    id = value;
                    definitions.AddTraceFormat(this);
                }
                else
                {
                    throw new Exception("ID Exists.");
                }
            }
        }
        #endregion Fields and Attributes

        #region Constructors
        public TraceFormat(Definitions defs)
        {
            base.TagName = "traceFormat";
            RegularChannel = new List<Channel>();
            IntermittentChannel = new List<Channel>();
            this.definitions = defs;
        }

        public TraceFormat(Definitions defs,string id)
        {
            base.TagName = "traceFormat";
            RegularChannel = new List<Channel>();
            IntermittentChannel = new List<Channel>();
            this.definitions = defs;
            
            if (!definitions.ContainsID(id))
            {
                this.id = id;
            }
            else
            {
                throw new Exception("Unique Id Required. Id exists in Definitions.");
            }
            definitions.AddTraceFormat(this);
        }

        public TraceFormat(Definitions defs, XmlElement element)
        {
            base.TagName = "traceFormat";
            RegularChannel = new List<Channel>();
            IntermittentChannel = new List<Channel>();
            this.definitions = defs;
            ParseElement(element);
        }
        #endregion Constructors

        #region Override Functions ParseElement and ToInkML
        /// <summary>
        /// Function to Parse the xmlElement and fill details of TraceFormat object
        /// </summary>
        /// <param name="element"> Element to be Parsed</param>
        public override void ParseElement(XmlElement element)
        {
            Channel rc;
            if (element.LocalName.Equals("traceFormat"))
            {
                if (!element.GetAttribute("id").Equals(""))
                {
                    Id = element.GetAttribute("id");
                }
                int count = 0;
                foreach (XmlElement item in element)
                {
                    if (item.LocalName == "channel")
                    {
                        count++;
                        rc = new Channel(item);
                        RegularChannel.Add(rc);
                    }
                }
               if (count == 0)
                {
                    throw new Exception(" TraceFormat with no Regular Channels. Invalid.");
                }
                XmlNodeList icList = element.SelectNodes("./intermittentChannels/channel");
                if (icList.Count != 0)
                {
                    foreach (XmlNode c in icList)
                    {
                        Channel ic = new Channel((XmlElement)c);
                        IntermittentChannel.Add(ic);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid Element Name.");
            }
        }

        /// <summary>
        /// Function to Convert the Traceformat object to the xmlelement
        /// </summary>
        /// <param name="inkDocument"> Ink Document </param>
        /// <returns>TraceFormat xml Element</returns>
        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("traceFormat");
            if (!id.Equals(""))
            {
                result.SetAttribute("id", id);
            }
            foreach (Channel c in RegularChannel)
            {
                result.AppendChild(c.ToInkML(inkDocument));
            }
            XmlElement ic = inkDocument.CreateElement("intermittentChannels");
            foreach (Channel c in IntermittentChannel)
            {
                ic.AppendChild(c.ToInkML(inkDocument));
            }
            if (IntermittentChannel.Count != 0)
            {
                result.AppendChild(ic);
            }

            return result;
        }
        #endregion Override Functions ParseElement and ToInkML

        #region Get Channel Methods
        /// <summary>
        /// Function to find the number of channels in the traceFormat
        /// </summary>
        /// <returns></returns>
        public int GetChannelCount()
        {
            return RegularChannel.Count + IntermittentChannel.Count;
        }

        public int GetRegularChannelCount()
        {
            return RegularChannel.Count;
        }

        public int GetIntermittentChannelCount()
        {
            return IntermittentChannel.Count;
        }

        /// <summary>
        /// Function to find the index of the channel in the channel list
        /// </summary>
        /// <param name="channelName">Name of the channel</param>
        /// <returns>Index of the channel. return -1 if not found in the list</returns>
        public int GetChannelIndex(string channelName)
        {
            int i;
            for (i = 0; i < RegularChannel.Count; i++)
            {
                if (RegularChannel[i].Name.Equals(channelName))
                {
                    return i;
                }
            }
            for (i = 0; i < IntermittentChannel.Count; i++)
            {
                if (IntermittentChannel[i].Name.Equals(channelName))
                {
                    return i + RegularChannel.Count;
                }
            }

            return -1;
        }

        /// <summary>
        /// Function to find the channel with the channel Name from the traceFormat
        /// </summary>
        /// <param name="ChannelName">Name of the Channel</param>
        /// <returns>Channel object / Null if not found</returns>
        public Channel GetChannel(string channelName)
        {
            int i;
            for (i = 0; i < RegularChannel.Count; i++)
            {
                if (RegularChannel[i].Name.Equals(channelName))
                {
                    return RegularChannel[i];
                }
            }
            for (i = 0; i < IntermittentChannel.Count; i++)
            {
                if (IntermittentChannel[i].Name.Equals(channelName))
                {
                    return IntermittentChannel[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Function to find the channel element with the Index in the TraceFormat
        /// </summary>
        /// <param name="Index">Index of the Channel</param>
        /// <returns>Channel Object / Null</returns>
        public Channel GetChannel(int Index)
        {
            if (Index >= 0)
            {
                if (Index < RegularChannel.Count)
                {
                    return RegularChannel[Index];
                }
                Index -= RegularChannel.Count;
                if (Index < IntermittentChannel.Count)
                {
                    return IntermittentChannel[Index];
                }
            }
            return null;
        }


        /// <summary>
        /// Function to return the list of channel names in the TraceFormat
        /// </summary>
        /// <returns>List of strings containing channel Names</returns>
        public List<string> GetChannelNames()
        {
            List<string> result = new List<string>();
            foreach (Channel c in RegularChannel)
            {
                result.Add(c.Name);
            }
            foreach (Channel c in IntermittentChannel)
            {
                result.Add(c.Name);
            }
            return result;
        }

        public static TraceFormat GetDefaultTraceFormat(Definitions defs)
        {
            if (defaultTF == null)
            {
                defaultTF = new TraceFormat(defs, "DefaultTraceFormat");
                Channel xChannel = new Channel("X", ChannelType.INTEGER);
                Channel yChannel = new Channel("Y", ChannelType.INTEGER);
                Channel fChannel = new Channel("F", ChannelType.INTEGER);
                defaultTF.AddChannel(xChannel, true);
                defaultTF.AddChannel(yChannel, true);
                defaultTF.AddChannel(fChannel, true);
            }
            return defaultTF;
        }
        #endregion Get Channel  Methods

        #region Add and Remove Channel Methods
        /// <summary>
        /// Function to add a channel to the TraceFormat
        /// </summary>
        /// <param name="channel">Channel object to be added</param>
        /// <param name="regular">Regular channel (true)/ intermittent channel (false) </param>
        public bool AddChannel(Channel channel, bool regular)
        {
            if (GetChannelIndex(channel.Name) == -1)
            {
                if (regular)
                {
                    RegularChannel.Add(channel);
                }
                else
                {
                    IntermittentChannel.Add(channel);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function to remove a channel from the Traceformat
        /// </summary>
        /// <param name="ChannelName">Name of the Channel</param>
        /// <returns>boolean value</returns>
        public bool RemoveChannel(string ChannelName)
        {
            int index;
            if ((index=GetChannelIndex(ChannelName))!= -1)
            {
                if(index <RegularChannel.Count)
                {
                    RegularChannel.RemoveAt(index);
                    return true;
                }                
                index-=RegularChannel.Count;
                IntermittentChannel.RemoveAt(index);

            }
            return false;
        }
        #endregion Add and Remove Channel Methods

    }
}
