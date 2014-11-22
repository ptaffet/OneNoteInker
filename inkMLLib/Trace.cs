/*
 * @(#) Trace.cs 1.0 06-08-2007 author: Manoj Prasad
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
using System.Collections;
using System.Drawing;

namespace InkML
{
	/// <summary>
	///
	/// </summary>

    public class Trace:Stroke
    {

        #region Attributes
        private TraceType traceType = TraceType.PenDown;
        private TraceContinuation continuation = TraceContinuation.None;
        private string id="";
        private string priorRef="";
        private int duration=0;
        private int timeOffset=0;
        private Definitions definitions;
        private string brushRef="";
        private string contextRef="";
        private bool ContainsBrush = false;

        private Brush associatedBrush;
        private Context associatedContext;
        private Context associatedCurrentContext;

        /// <summary>
        /// Gets/Sets the 'id' attribute of the Trace Element
        /// </summary>
        public string Id
        {
            get { return id; }           
        }

        /// <summary>
        /// Gets/Sets the 'type' attribute of the Trace Element
        /// </summary>
        public TraceType Type
        {
            get { return traceType; }
            set
            {               
                traceType = value;
            }
        }

        /// <summary>
        /// Gets/Sets the 'continuation' attribute of the Trace Element
        /// </summary>
        public TraceContinuation Continuation
        {
            get { return continuation; }
            set
            {               
                continuation = value;
            }
        }

        /// <summary>
        /// Gets/Sets the 'priorRef' attribute of the Trace Element
        /// </summary>
        public string PriorRef
        {
            get { return priorRef; }
            set { priorRef = value; }
        }

        /// <summary>
        /// Gets/Sets the 'contextRef' attribute of the Trace Element
        /// </summary>
        public string ContextRef
        {
            get { return contextRef; }
            set
            { 
                contextRef=value;
                ResolveContext();
            }
        }

        /// <summary>
        /// Gets/Sets the 'brushRef' attribute of the Trace Element
        /// </summary>
        public string BrushRef
        {
            get { return brushRef; }
            set 
            {
                brushRef=value;
                ResolveBrush();               
            }
        }

        /// <summary>
        /// Gets/Sets the 'duration' attribute of the Trace Element
        /// </summary>
        public int Duration
        {
            get { return duration; }
            set { duration=value; }
        }

        /// <summary>
        /// Gets/Sets the 'timeOffset' attribute of the Trace Element
        /// </summary>
        public int TimeOffset
        {
            get { return timeOffset; }
            set { timeOffset = value; }
        }

        /// <summary>
        /// Gets the associated Brush Element of the Trace Element
        /// </summary>
        public Brush AssociatedBrush
        {
            get { return associatedBrush; }
        }

        /// <summary>
        /// Gets the associated Context attribute of the Trace Element
        /// </summary>
        public Context AssociatedContext
        {
            get { return associatedContext; }
        }
        #endregion Attributes

        #region Fields
        private Dictionary<string, ArrayList> traceData;
        private Dictionary<string,ChannelType> traceChannelType;
        private List<string> channelNames;
        private ArrayList pointList;
        private Point maxpoint;
        private Point minpoint;
        private Point meanpoint;
        int space;

        public Point Maxpoint
        {
            get { return maxpoint; }
        }

        public Point Minpoint
        {
            get { return minpoint; }
        }

        public Point Meanpoint
        {
            get { return meanpoint; }
        }

        public int Space
        {
            get { return space; }
            set { space = value; }
        }

        public ArrayList PointList
        {
            get { return pointList; }
        }
        #endregion Fields

        #region Constructors
        public Trace(Definitions defs, Context currentContext)
        {
            maxpoint.X =-1;
            maxpoint.Y = -1;
            minpoint.X = -1;
            minpoint.Y = -1;
            meanpoint.X = 0;
            meanpoint.Y = 0;

            this.TagName = "trace";
            this.definitions = defs;
            associatedBrush = currentContext.BrushElement;
            associatedCurrentContext = new Context(defs);
            associatedCurrentContext.BrushElement = currentContext.BrushElement;
            associatedCurrentContext.TraceFormatElement = currentContext.TraceFormatElement;
            associatedCurrentContext.CanvasElement = currentContext.CanvasElement;
            associatedCurrentContext.InksourceElement = currentContext.InksourceElement;
            associatedContext = associatedCurrentContext;
            traceData = new Dictionary<string, ArrayList>();
            traceChannelType = new Dictionary<string, ChannelType>();
            channelNames = associatedContext.TraceFormatElement.GetChannelNames();
            for (int i = 0; i < associatedContext.TraceFormatElement.GetChannelCount(); i++)
            {
                traceChannelType.Add(associatedContext.TraceFormatElement.GetChannel(i).Name, associatedContext.TraceFormatElement.GetChannel(i).Type);
                traceData.Add(associatedContext.TraceFormatElement.GetChannel(i).Name,new ArrayList());
            }
        }

        public Trace(Definitions defs, Context currentContext, string id)
            :this(defs, currentContext)
        {            
            if (!id.Equals(""))
            {
                this.id = id;
                definitions.AddTrace(this);
            }

        }        

        public Trace(Definitions defs, Context currentContext, string id,string brushRef)
            : this(defs, currentContext)
        {
            if (!id.Equals(""))
            {
                this.id = id;
                definitions.AddTrace(this);
            }

            if (brushRef != null)
            {
                this.brushRef = brushRef;
                ResolveBrush();
            }
        }

        public Trace(Definitions defs, Context currentContext, XmlElement element)
            : this(defs, currentContext)
        {           
            ParseElement(element);
        }

        public Trace(Definitions defs, XmlElement element)
            :this(defs)
        {           
            if (element != null)
            {
                ParseElement(element,true);
            }
            else
            {
                throw new Exception("Null xml Element.");
            }
           
        }

        public Trace(Definitions defs)
        {
            traceData = new Dictionary<string, ArrayList>();
            traceChannelType = new Dictionary<string, ChannelType>();
            channelNames = new List<string>();
            this.TagName = "trace";
            if (defs != null)
            {
                definitions = defs;
            }
            else
            {
                throw new Exception("Null Definitions block");
            }
            associatedCurrentContext = new Context(defs);
            associatedContext = new Context(defs);
        }

        public Trace(Definitions defs, string id)
            :this(defs)
        {
           
            if (!id.Equals(""))
            {
                this.id = id;
                definitions.AddTrace(this);
            }
            
        }
        #endregion Constructors

        #region override function Parse and ToInkML
        private void ParseElement(XmlElement element, bool requireContext)
        {
            if (requireContext)
            {
                if (element.GetAttribute("contextRef").Equals(""))
                {
                    associatedContext.TraceFormatElement = TraceFormat.GetDefaultTraceFormat(definitions);
                    channelNames = associatedContext.TraceFormatElement.GetChannelNames();
                    traceChannelType.Clear();
                    for (int i = 0; i < associatedContext.TraceFormatElement.GetChannelCount(); i++)
                    {
                        traceChannelType.Add(associatedContext.TraceFormatElement.GetChannel(i).Name, associatedContext.TraceFormatElement.GetChannel(i).Type);                        
                    }
                }
            }
            ParseElement(element);
        }
        public override void ParseElement(XmlElement element)
        {
            if (element.LocalName.Equals("trace"))
            {
                id = element.GetAttribute("id");
                if (!id.Equals(""))
                {
                    definitions.AddTrace(this);
                }
                brushRef = element.GetAttribute("brushRef");
                if (!brushRef.Equals(""))
                {
                    ResolveBrush();
                }

                contextRef = element.GetAttribute("contextRef");
                if (!contextRef.Equals(""))
                {
                    ResolveContext();
                }

                priorRef = element.GetAttribute("priorRef");
                
                if (element.GetAttribute("duration")!=null && !element.GetAttribute("duration").Equals(""))
                {
                    duration = System.Convert.ToInt32(element.GetAttribute("duration"));
                }
                if (element.GetAttribute("timeOffset") != null && !element.GetAttribute("timeOffset").Equals(""))
                {
                    timeOffset = System.Convert.ToInt32(element.GetAttribute("timeOffset"));
                }
                string stroke = element.InnerText;
                if (!stroke.Equals(""))
                {
                    TranslateTraceToList(associatedContext.TraceFormatElement, stroke);
                }
                else
                {
                    throw new Exception("EMPTY TRACE ELEMENT.");
                }
                
            }
            else
            {
                throw new Exception("Invalid Element Name.");
            }
        }

        public override XmlElement ToInkML(XmlDocument inkDocument)
        {
            XmlElement result = inkDocument.CreateElement("trace");
            if (!id.Equals(""))
            {
                result.SetAttribute("id", id);
            }
            if (!brushRef.Equals(""))
            {
                result.SetAttribute("brushRef", brushRef);
            }
            if (!contextRef.Equals(""))
            {
                result.SetAttribute("contextRef", contextRef);
            }
            if (!priorRef.Equals(""))
            {
                result.SetAttribute("priorRef", priorRef);
            }
            if (duration != 0)
            {
                result.SetAttribute("duration", System.Convert.ToString(duration));
            }
            if (timeOffset != 0)
            {
                result.SetAttribute("timeOffset", System.Convert.ToString(timeOffset));
            }
            XmlText tracevalue = inkDocument.CreateTextNode(TranslateListToTrace(associatedContext.TraceFormatElement));
            result.AppendChild(tracevalue);
            return result;
        }

        #endregion override function Parse and ToInkML

        #region Resolve Function Context and Brush

        /// <summary>
        /// Function to Resolve the Context Associated with the Trace.
        /// Resolves the Context by giving priority to ContextRef over Current Context Values.
        /// </summary>
        public void ResolveContext()
        {
            Context tctx;
            if (definitions.ContainsID(contextRef))
            {
                tctx = definitions.GetContext(contextRef);
                if (tctx != null)
                {                    
                    if (tctx.CanvasElement != null)
                    {
                        associatedContext.CanvasElement = tctx.CanvasElement;
                    }
                    if ( tctx.InksourceElement != null)
                    {
                        associatedContext.InksourceElement = tctx.InksourceElement;
                    }
                    if (tctx.TraceFormatElement != null)
                    {
                        associatedContext.TraceFormatElement = tctx.TraceFormatElement;
                        channelNames = associatedContext.TraceFormatElement.GetChannelNames();
                        traceChannelType.Clear();
                        for (int i = 0; i < associatedContext.TraceFormatElement.GetChannelCount(); i++)
                        {
                            traceChannelType.Add(associatedContext.TraceFormatElement.GetChannel(i).Name, associatedContext.TraceFormatElement.GetChannel(i).Type);                            
                        }
                    }
                    if (!ContainsBrush && tctx.BrushElement!=null)
                    {
                        associatedBrush = tctx.BrushElement;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid Reference.");
            }
        }

        /// <summary>
        /// Function To Resolve the Brush Associated with the Trace Element
        /// Gives Priority to the BrushRef over the Current Context Brush Element
        /// </summary>
        public void ResolveBrush()
        {
            Brush tb;
            if (definitions.ContainsID(brushRef))
            {
                tb = definitions.GetBrush(brushRef);
                if (tb != null)
                {
                    associatedBrush = tb;
                    ContainsBrush = true;
                }
            }
            else
            {
                throw new Exception("Invalid Reference.");
            }
        }

        #endregion Resolve Function Context and Brush

        #region translate functions

        /// <summary>
        /// Function to translate the List of values to the string value
        /// -- Reads the channel values from TraceData list
        /// -- Orders it based on the order in which in each element value is added
        /// 
        /// TODO: use the traceFormat information to order the values of each packet
        /// text
        /// 
        /// use tf.getChannelNames() to get the values of the channelName[].
        /// use it for ordering the elements
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        private string TranslateListToTrace(TraceFormat tf) 
        {
            int i;
            bool[] first_sample = new bool[traceData.Count];
            bool[] first_velocity = new bool[traceData.Count];
            bool[] first_acceleration = new bool[traceData.Count];

            ArrayList Velocity_lastValue;
            ArrayList Sample_lastValue;
            ArrayList _sample = new ArrayList();
            string result = "";
            Velocity_lastValue = new ArrayList(traceData.Count);
            Sample_lastValue = new ArrayList(traceData.Count);
            
            ArrayList tempDataList;
            int interations =0;
           
            if (traceData.TryGetValue("X", out tempDataList))
            {
                interations = tempDataList.Count;
                for (i = 0; i < traceData.Count; i++)
                {
                    Velocity_lastValue.Add(new object());
                    Sample_lastValue.Add(new object());
                    first_sample[i] = true;
                    first_velocity[i] = true;
                    first_acceleration[i] = true;
                }
                
                try
                {
                    for (int j = 0; j < interations;j++ )
                    {
                       
                        for(i=0;i<traceData.Count;i++)
                        {
                            // implementation of check for channel type
                            // to be done.....

                            traceData.TryGetValue(channelNames[i], out _sample);

                            if (_sample != null)
                            {
                                if (first_sample[i] == true)
                                {
                                    first_sample[i] = false;
                                    result += Convert.ToString(_sample[j]) + " ";
                                }
                                else if (first_velocity[i] == true)
                                {
                                    first_velocity[i] = false;
                                    result += "'" + Convert.ToString(((double)_sample[j] - (double)Sample_lastValue[i])) + " ";
                                }
                                else
                                {
                                    result += Convert.ToString(((double)_sample[j] - (double)Sample_lastValue[i])) + " ";
                                }

                                Sample_lastValue[i] = _sample[j];
                                
                            }
                            
                        } 
                        result += ",";
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            return result.Substring(0,result.Length-1);
        }

        /// <summary>
        /// Function to Translate the string value of the Trace element to the List of values
        /// -- Converts the String into a intermediate representation with Parse function
        /// -- Translates the Intermediate values by following the semantics in InkML into a list of Values
        /// -- Creates a list for each channel and adds it to the TraceData mapping the channelName to the Arraylist of values
        ///
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="stroke"></param>
        private void TranslateTraceToList(TraceFormat tf,string stroke)
        {
            ArrayList Velocity_bool;
            ArrayList Acceleration_bool;
            ArrayList Velocity_Value;
            ArrayList Velocity_lastValue;
            ArrayList Sample_lastValue;
            int i, j;
            Velocity_bool = new ArrayList();
            Acceleration_bool = new ArrayList();
            Velocity_Value = new ArrayList();
            Velocity_lastValue = new ArrayList();
            Sample_lastValue = new ArrayList();
            ArrayList Sample = new ArrayList();
            List<TraceQualifiers> currentqualifiers = new List<TraceQualifiers>();
            for (i = 0; i < tf.GetChannelCount(); i++)
            {
                Velocity_bool.Add(false);
                Acceleration_bool.Add(false);
                Velocity_lastValue.Add(new object());
                Velocity_Value.Add(new object());
                Sample_lastValue.Add(i);
                currentqualifiers.Add(TraceQualifiers.Explicit);
            }
            
            ArrayList trace_line = ParseTrace(stroke);
            pointList = new ArrayList();
            pointList.Clear();
            foreach (unit[] units in trace_line)
            {
                Sample.Clear();
                for (j = 0; j < units.Length; j++)
                {
                    Sample.Add(j);
                }

                for (i = 0; i < units.Length; i++)
                {

                    if (tf.GetChannel(i) == null || tf.GetChannel(i).Type == ChannelType.INTEGER || tf.GetChannel(i).Type == ChannelType.DECIMAL)
                    {
                        if (units[i].Qualifier.Equals(TraceQualifiers.Velocity)||(units[i].Qualifier.Equals(TraceQualifiers.None)&&currentqualifiers[i].Equals(TraceQualifiers.Velocity)))
                        {
                            if(tf.GetChannel(i).Type == ChannelType.INTEGER)
                                Sample[i] = System.Convert.ToInt32(Sample_lastValue[i]) + System.Convert.ToInt32(units[i].Value);
                            else if (tf.GetChannel(i).Type == ChannelType.DECIMAL)
                                Sample[i] = System.Convert.ToDouble(Sample_lastValue[i]) + System.Convert.ToDouble(units[i].Value);
                            Sample_lastValue[i] = Sample[i];
                            Velocity_lastValue[i] = units[i].Value;
                            currentqualifiers[i] = TraceQualifiers.Velocity;
                        }
                        else if (units[i].Qualifier.Equals(TraceQualifiers.Acceleration) || (units[i].Qualifier.Equals(TraceQualifiers.None) && currentqualifiers[i].Equals(TraceQualifiers.Acceleration)))
                        {
                            if (tf.GetChannel(i).Type == ChannelType.INTEGER)
                                Velocity_Value[i] = System.Convert.ToInt32(Velocity_lastValue[i]) + System.Convert.ToInt32(units[i].Value);
                            else if (tf.GetChannel(i).Type == ChannelType.DECIMAL)
                                Velocity_Value[i] = System.Convert.ToDouble(Velocity_lastValue[i]) + System.Convert.ToDouble(units[i].Value);

                            Velocity_lastValue[i] = Velocity_Value[i];
                            Sample[i] = System.Convert.ToInt32(Sample_lastValue[i]) + System.Convert.ToInt32(Velocity_Value[i]);
                            Sample_lastValue[i] = Sample[i];
                            currentqualifiers[i] = TraceQualifiers.Acceleration;
                        }
                        else if (units[i].Qualifier.Equals(TraceQualifiers.Explicit) || (units[i].Qualifier.Equals(TraceQualifiers.None) && currentqualifiers[i].Equals(TraceQualifiers.Explicit)))
                        {
                            currentqualifiers[i] = TraceQualifiers.Explicit;
                            if (tf.GetChannel(i).Type == ChannelType.INTEGER)
                                Sample_lastValue[i] = Sample[i] = System.Convert.ToInt32(units[i].Value);
                            else if (tf.GetChannel(i).Type == ChannelType.DECIMAL)
                                Sample_lastValue[i] = Sample[i] = System.Convert.ToDouble(units[i].Value);
                        }
                        else if (units[i].Qualifier.Equals(TraceQualifiers.Prevvalue))
                            Sample[i] = Sample_lastValue[i];
                        
                    }
                    else if (tf.GetChannel(i).Type == ChannelType.BOOLEAN)
                    {
                        Sample[i] = System.Convert.ToBoolean(units[i].Value);
                    }

                }

                ArrayList temp;
                for (j = 0; j < traceData.Count; j++)
                {
                    if (j < units.Length)
                    {
                        if (traceData.TryGetValue(tf.GetChannelNames()[j], out temp))
                        {
                            temp.Add(Sample[j]);
                        }
                    }
                }
                pointList.Add(Sample.Clone());
            }
        }

        /// <summary>
        /// Function to Convert the String value of the Trace text into a intermediate representation
        /// 
        /// This function splits the text of the TraceElement
        /// Converts into intermediate representation (units object) which contains 
        /// the value and meaning associated with value.
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns>Arraylist of array of unit (representing each packet of data </returns>
        private ArrayList ParseTrace(string stroke)
        {
            string[] intermediate;
           
            // to split the string into sample substrings 
            intermediate = stroke.Split(',');            
            int i, j;
            string[] sample;
            ArrayList result = new ArrayList();
            for (i = 0; i <= intermediate.Length - 1; i++)
            {
                string[] c = {" ","\r\n","\t"};
                sample = intermediate[i].Replace("'", " '").Replace("\"", " \"").Replace("-", " -").Replace("' ", "'").Replace("\" ", "\"").Split(c, StringSplitOptions.RemoveEmptyEntries);
                
                unit[] unit_array = new unit[sample.Length];
                for (j = 0; j < sample.Length; j++)
                {
                    unit_array[j] = new unit();
                    if (sample[j].Equals("T", StringComparison.OrdinalIgnoreCase) || sample[j].Equals("F", StringComparison.OrdinalIgnoreCase))
                        unit_array[j].Value = sample[j];
                    else if (sample[j].Contains("#"))
                        unit_array[j].Value = sample[j].Substring(sample[j].LastIndexOf("#") + 1);
                    else if (sample[j].Contains("."))
                        unit_array[j].Value = sample[j];
                    else
                        unit_array[j].Value = sample[j];

                    if (sample[j].StartsWith("'"))
                    {
                        unit_array[j].Value = sample[j].Substring(1);
                        unit_array[j].Qualifier = TraceQualifiers.Velocity;
                    }
                    else if (sample[j].StartsWith("\""))
                    {
                        unit_array[j].Value = sample[j].Substring(1);
                        unit_array[j].Qualifier = TraceQualifiers.Acceleration;
                    }
                    else if (sample[j].StartsWith("!"))
                    {
                        unit_array[j].Value = sample[j].Substring(1);
                        unit_array[j].Qualifier = TraceQualifiers.Explicit;
                    }
                    else if (sample[j].Equals("*"))
                    {
                        unit_array[j].Value = sample[j];
                        unit_array[j].Qualifier = TraceQualifiers.Prevvalue;
                    }
                    else if (sample[j].Equals("?"))
                    {
                        unit_array[j].Value = sample[j];
                        unit_array[j].Qualifier = TraceQualifiers.Novalue;
                    }

                }
                result.Add(unit_array);
            }
            return result;
        }
        #endregion translate function 

        #region Channel Functions

        /// <summary>
        /// Finds whether the Channel exists in the Trace Element
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public bool ContainsChannel(string channelName)
        {
            return traceChannelType.ContainsKey(channelName);
        }

        /// <summary>
        /// Gets the Channel Type for the Given Channel name 
        /// </summary>
        /// <param name="channelName">Name of the Channel</param>
        /// <returns>ChannelType of the Channel</returns>
        public ChannelType GetChannelType(string channelName)
        {
            ChannelType value;
            traceChannelType.TryGetValue(channelName, out value);
            return value;
        }

        /// <summary>
        /// Gets the Enumerator to access the Channel type and Name supported in the Trace
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,ChannelType>.Enumerator GetChannelListEnumerator()
        {
            return traceChannelType.GetEnumerator();
        }

        /// <summary>
        /// Gets the list of Channel Names that each packet contains
        /// </summary>
        /// <returns>List<string> channel names</string></returns>
        public List<string> GetChannelNames()
        {
            return channelNames;
        }
        #endregion Channel Functions

        #region Set/Get TraceData
        public void SetTraceDataInteger(string channelName, ArrayList data)
        {
            traceChannelType.Remove(channelName.ToUpper());
            traceChannelType.Add(channelName.ToUpper(), ChannelType.INTEGER);
            traceData.Remove(channelName.ToUpper());
            // Converting the data in HIMETRIC unit(dev) to Pixels (PPI) unit.
            ArrayList dataCopy = (ArrayList)data.Clone();
            traceData.Add(channelName, (ArrayList)dataCopy);
            channelNames.Add(channelName);
        }
       
        public void SetTraceDataDecimal(string channelName, ArrayList data)
        {
            traceChannelType.Remove(channelName.ToUpper());
            traceChannelType.Add(channelName.ToUpper(), ChannelType.DECIMAL);
            traceData.Remove(channelName.ToUpper());
            ArrayList dataCopy = (ArrayList)data.Clone();
            traceData.Add(channelName, (ArrayList)dataCopy);
            channelNames.Add(channelName);
        }

        public void SetTraceDataBoolean(string channelName, ArrayList data)
        {
            traceChannelType.Remove(channelName.ToUpper());
            traceChannelType.Add(channelName.ToUpper(), ChannelType.BOOLEAN);
            traceData.Remove(channelName.ToUpper());
            traceData.Add(channelName, data);
            channelNames.Add(channelName);
        }

        public ArrayList GetTraceData(string channelName)
        {
            ArrayList result;
            traceData.TryGetValue(channelName, out result);
            return result;
        }
        #endregion Set/Get TraceData



    }

    public class unit
    {
        string value;
        TraceDatatype _datatype;         /// change it to enum later ....
        TraceQualifiers _qualifier;
        public unit()
        {
            _qualifier = TraceQualifiers.None;
        }

        public unit(string value)
        {
            this.value = value;
            _qualifier = TraceQualifiers.None;
        }

        public unit(string value, TraceDatatype _datatype, TraceQualifiers _qualifier)
        {
            this.value = value;
            this._qualifier = _qualifier;
            this._datatype = _datatype;
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public TraceDatatype Datatype
        {
            get { return _datatype; }
            set { _datatype = value; }
        }

        public TraceQualifiers Qualifier
        {
            get { return _qualifier; }
            set { _qualifier = value; }
        }
    }
}
