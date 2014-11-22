/*
 * @(#) ISF2InkML.cs 1.0 06-08-2007 author: Manoj Prasad
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
using System.IO;
using Microsoft.Ink;
using InkMLConverters;
using InkML;

namespace InkMLConverters
{
	/// <summary>
	///
	/// </summary>

    public class ISF2InkML
    {
        #region Fields 

        private InkInterpreter inkmlInterpreter;
        private Dictionary<Guid, string> mapGuidChannel;
        private int id;
        private DrawingAttributes currentBrush;
        private Guid[] currentTraceFormat;
        #endregion Fields

        #region Constructor

        public ISF2InkML()
        {
            currentBrush = new DrawingAttributes();
            currentTraceFormat = new Guid[5];
            ReInitialize();
            Initialize();
        }
        /// <summary>
        /// Function to re initialise the object
        /// </summary>
        public void ReInitialize()
        {
            id = 0;
            inkmlInterpreter = new InkInterpreter();
            currentTraceFormat.Initialize();
        }

        /// <summary>
        /// Function to initialise the Map between
        /// the Guids of the Channel supported in ISF to InkML Channel Names
        /// </summary>
        private void Initialize()
        {
            mapGuidChannel = new Dictionary<Guid,string>();
            mapGuidChannel.Add(PacketProperty.X, "X");
            mapGuidChannel.Add(PacketProperty.Y, "Y");
            mapGuidChannel.Add(PacketProperty.Z, "Z");
            mapGuidChannel.Add(PacketProperty.TimerTick, "T");
            mapGuidChannel.Add(PacketProperty.NormalPressure, "F");
            mapGuidChannel.Add(PacketProperty.XTiltOrientation, "OTX");
            mapGuidChannel.Add(PacketProperty.YTiltOrientation, "OTY");
            mapGuidChannel.Add(PacketProperty.RollRotation, "OR");
            mapGuidChannel.Add(PacketProperty.YawRotation, "OA");
            mapGuidChannel.Add(PacketProperty.PitchRotation, "OE");
            mapGuidChannel.Add(PacketProperty.AltitudeOrientation, "E");
            mapGuidChannel.Add(PacketProperty.AzimuthOrientation, "A");
            mapGuidChannel.Add(PacketProperty.ButtonPressure, "B");            
            mapGuidChannel.Add(PacketProperty.PacketStatus, "PacketStatus");          
            mapGuidChannel.Add(PacketProperty.SerialNumber, "S.NO");
            mapGuidChannel.Add(PacketProperty.TangentPressure, "TP");           
            mapGuidChannel.Add(PacketProperty.TwistOrientation, "TO");   
            
        }

        #endregion Constructor

        #region Convert Functions

        /// <summary>
        /// Function to Convert ISF file to InkML file
        /// </summary>
        /// <param name="InputFileName">Input ISF file</param>
        /// <param name="OutputFileName">Output InkML File</param>
        public void ConvertToInkML(string inputFileName, string outputFileName)
        {
            Microsoft.Ink.Ink inkobject = new Microsoft.Ink.Ink();
            
            using (FileStream inkStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            {
                byte[] inkBytes = new byte[inkStream.Length];
                inkStream.Read(inkBytes, 0, System.Convert.ToInt16(inkStream.Length));
                inkobject.Load(inkBytes);
                ConvertToInkML(inkobject, outputFileName);
                inkStream.Close();
            }

        }

        /// <summary>
        /// Function to convert Ink Object to InkML and Save it in the OutputFile
        /// </summary>
        /// <param name="InkObject">Ink Object to be converted to InkML</param>
        /// <param name="OutputFileName">Output InkML File</param>
        public void ConvertToInkML(Microsoft.Ink.Ink inkObject, string outputFileName)
        {
            if (outputFileName.Equals(""))
                return;

            currentTraceFormat = inkObject.Strokes[0].PacketDescription;            
            AddInk(inkObject);
            SaveInkML(outputFileName);
        }
        #endregion Convert Functions

        #region Add and Save Functions

        /// <summary>
        /// Function to convert Ink Object to inkML. 
        /// Converts Ink Objects to InkML objects and adds to InkML Interpreter.
        /// </summary>
        /// <param name="InkObject"> Ink Object to be converted and added to InkML</param>
        public void AddInk(Microsoft.Ink.Ink inkObject)
        {
            try
            {
                currentTraceFormat = inkObject.Strokes[0].PacketDescription;
                AddTraceFormat(inkObject.Strokes[0]);
                AddBrush(currentBrush);
                AddAnnotation(inkObject.ExtendedProperties);
                AddTraces(inkObject.Strokes);
            }
            catch(Exception )
            {
            }
        }

        /// <summary>
        /// Function to convert Tablet Object to InkML.
        /// Used to capture inkSource information.
        /// </summary>
        /// <param name="tabletObject"> Tablet Object</param>
        public void AddInk(Microsoft.Ink.Tablet tabletObject)
        {
            // Creating inkSource object to store the Tablet information
            InkSource resultIS = new InkSource(inkmlInterpreter.Defs);
            TraceFormat resultTF = new TraceFormat(inkmlInterpreter.Defs);
            try
            {
                resultIS.TraceFormat = GetTraceFormat(tabletObject);
            }
            catch (Exception)
            {
            }

            inkmlInterpreter.ParseInk(resultIS);
        }

      

        /// <summary>
        /// Function to Save the InkML objects created.
        /// </summary>
        /// <param name="FileName"> Output FileName</param>
        public void SaveInkML(string FileName)
        {
            inkmlInterpreter.SaveInk(FileName);
        }
        public string SaveInkML()
        {
            return inkmlInterpreter.SaveInk();
        }
        #endregion Add and Save Functions

        #region Converting Functions 
        /// <summary>
        /// Function to find the traceFormat of the Tablet.
        /// </summary>
        /// <param name="theTablet">Tablet Object</param>
        /// <returns>TraceFormat of the Tablet Object</returns>
        private TraceFormat GetTraceFormat(Tablet theTablet)
        {
            try
            {
                TraceFormat resultTF = new TraceFormat(inkmlInterpreter.Defs);
                // If this particular property is supported,
                // report the name and property metrics information.
                Channel tempChannel;
                Dictionary<Guid, string>.Enumerator guidenum = mapGuidChannel.GetEnumerator();
                // Checking the channel support for every Channel possible
                while (guidenum.MoveNext())
                {
                    // If the Channel is supported, 
                    // create a Channel object and fill the properties from the Metrics of the Tablet.
                    if (theTablet.IsPacketPropertySupported(guidenum.Current.Key))
                    {
                        TabletPropertyMetrics theMetrics = theTablet.GetPropertyMetrics(guidenum.Current.Key);
                        tempChannel = new Channel(guidenum.Current.Value, ChannelType.DECIMAL);
                        tempChannel.Units = "inch";
                        tempChannel.Max = System.Convert.ToString(theMetrics.Maximum);
                        tempChannel.Min = System.Convert.ToString(theMetrics.Minimum);

                        resultTF.AddChannel(tempChannel, true);
                    }
                }
                return resultTF;
            }
            catch (Exception)
            {
            }
            return null;
        }

        private void AddTraceFormat(Microsoft.Ink.Stroke stroke)
        {
            int i;
            Guid[] packetDescription = stroke.PacketDescription;
            TraceFormat resultTF = new TraceFormat(inkmlInterpreter.Defs);
            Channel tempChannel;
            
            for (i = 0; i < packetDescription.Length; i++)
            {
                
                // create a Channel object and fill the properties from the Metrics of stroke
                TabletPropertyMetrics theMetrics =stroke.GetPacketDescriptionPropertyMetrics((Guid)packetDescription.GetValue(i));
                tempChannel = new Channel(mapGuidChannel[((Guid)packetDescription.GetValue(i))], ChannelType.DECIMAL);
                tempChannel.Units = "inch";
                tempChannel.Max = System.Convert.ToString(theMetrics.Maximum);
                tempChannel.Min = System.Convert.ToString(theMetrics.Minimum);
                resultTF.AddChannel(tempChannel, true);

            }
            inkmlInterpreter.ParseInk(resultTF);
        }

        /// <summary>
        /// Function to Add Extended Properties of the Ink to InkML.
        /// </summary>
        /// <param name="inkAnnotation"> Extended Properties of Ink</param>
        private void AddAnnotation(ExtendedProperties inkAnnotation)
        {
            try
            {
                // Using Enumerator, Get the Data and Id
                // Initialise a annotation element for every Data and id and save the data into it.
                // Add the annotation element to the inkml interpreter
                Annotation result;
                ExtendedProperties.ExtendedPropertiesEnumerator annotationEnum = inkAnnotation.GetEnumerator();
                while (annotationEnum.MoveNext())
                {
                    result = new Annotation();
                    result.AnnotationTextValue = System.Convert.ToString(annotationEnum.Current.Data);
                    result.SetAttribute("id", annotationEnum.Current.Id.ToString());
                    inkmlInterpreter.ParseInk(result);
                }
            }
            catch (Exception)
            { }
        }

        /* This function Captures the Brush and traceFormat changes in a Stroke.
         * Other Properties like TransForm, Shear and Scale are not Captured.
         * This can be done after including the CanvasTransform and Mapping element
         * in the Parser.
         * 
         * **** TransForm property of the Strokes/Stroke sets a Matrix Mapping
         * **** Scale property of the Strokes/Stroke sets XY scaling values.
         * **** Shear and rotate are not supported in inkML. But can be added as annotations
         * 
         * This Function First checks for change in Brush. 
         * Adds a brush element to the inkML if there is a change.
         * 
         * Next it checks for change in TraceFormat.
         * Adds a traceFormat element to the inkMl if there is a change
         * 
         * It also has Extended Properties. This function doesnot convert this to 
         * annotations. Inserting AddAnnotation(extendedProperties) would solve this.
         * 
         * Next it adds a stroke object.
         */
        private void AddTraces(Strokes traces)
        {
            Trace tempTrace;
            Microsoft.Ink.Stroke temp;
            try
            {               
                for(int j=0;j<traces.Count;j++)
                {
                    temp = traces[j];
                    if (!temp.Deleted)
                    {
                        
                        if (BrushChanged(temp.DrawingAttributes))
                        {
                            AddBrush(temp.DrawingAttributes);
                            currentBrush = temp.DrawingAttributes;
                        }
                        if (TraceFormatChanged(temp.PacketDescription))
                        {
                            currentTraceFormat = temp.PacketDescription;
                            AddTraceFormat(temp);
                        }

                        tempTrace = new Trace(inkmlInterpreter.Defs);
                       
                        for (int i = 0; i < currentTraceFormat.Length; i++)
                        {
                            //Get stroke data.
                            ArrayList strokeData = new ArrayList(temp.GetPacketValuesByProperty((Guid)currentTraceFormat.GetValue(i)));
                            //Convert from HIMETRIC to inch 
                            // Converting the data in HIMETRIC unit(dev) to Pixels (PPI) unit.
                            for (int k = 0; k < strokeData.Count; k++)
                            {
                                // 1 HIMETRIC = .01 mm and 1 inch = 25.4 mm
                                // typical screenResolution = 96 pixels/inch
                                //value in pixels = himetric * .01 * screenResolution / 25.4
                                double value = (double)(int)strokeData[k];
                                value = (value * .01)/ 25.4;
                                strokeData[k] = (double)value;
                            }
                            //set trace data in inches to inkML trace
                            tempTrace.SetTraceDataDecimal(mapGuidChannel[((Guid)currentTraceFormat.GetValue(i))], strokeData);
                        }
                        inkmlInterpreter.ParseInk(tempTrace);
                    }
                }
            }
            catch (Exception )
            {
            }
        }


        /// <summary>
        /// Function to Add brush element to the InkML interpreter.
        /// Converts the Drawing Attribute of a stroke to AnnotationXML
        /// Extended Property is left for future implementation.
        /// Brush now supports only one annotation element within in it.
        /// </summary>
        /// <param name="brush">Drawing Attributes to be Converted to Brush element</param>
        private void AddBrush(DrawingAttributes brush)
        {
            Brush inkMLBrush = new Brush(inkmlInterpreter.Defs);
            AnnotationXML bAnnotation = new AnnotationXML();
            bAnnotation.AddProperty("color", brush.Color.Name);
            bAnnotation.AddProperty("width", System.Convert.ToString(brush.Width));
            bAnnotation.AddProperty("transparency", System.Convert.ToString(brush.Transparency));
            bAnnotation.AddProperty("antialiased", System.Convert.ToString(brush.AntiAliased));
            bAnnotation.AddProperty("fittocurve", System.Convert.ToString(brush.FitToCurve));
            bAnnotation.AddProperty("height", System.Convert.ToString(brush.Height));
            bAnnotation.AddProperty("ignorePressure", System.Convert.ToString(brush.IgnorePressure));
            bAnnotation.AddProperty("pentip", System.Convert.ToString(brush.PenTip));
            inkMLBrush.BrushAnnotationXML = bAnnotation;
            

            inkmlInterpreter.ParseInk(inkMLBrush);
        }

        /// <summary>
        /// Function to check whether the traceFormat list is the same as the 
        /// current TraceFormat List
        /// </summary>
        /// <param name="traceFormat"> list of channels in traceFormat</param>
        /// <returns> boolean value </returns>
        private bool TraceFormatChanged(Guid[] traceFormat)
        {
            if (currentTraceFormat.Length != traceFormat.Length)
            { return true; }

            IEnumerator tfEnum = traceFormat.GetEnumerator();
            IEnumerator ctfEnum = currentTraceFormat.GetEnumerator();
            while(tfEnum.MoveNext() && ctfEnum.MoveNext())
            {
                if (!((Guid)tfEnum.Current).Equals(ctfEnum.Current))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Function to find Change in the Brush properties.
        /// </summary>
        /// <param name="brush">Brush to be compared with the current brush</param>
        /// <returns>boolean value </returns>
        private bool BrushChanged(DrawingAttributes brush)
        {
            if (currentBrush.AntiAliased != brush.AntiAliased)
            {
                return true;
            }
            if (!currentBrush.Color.Equals(brush.Color))
            {
                return true;
            }
            if (currentBrush.FitToCurve != brush.FitToCurve)
            {
                return true;
            }
            if (currentBrush.Height != brush.Height)
            {
                return true;
            }
            if (currentBrush.IgnorePressure != brush.IgnorePressure)
            { return true; }
            if (currentBrush.PenTip != brush.PenTip)
            {
                return true;
            }
            if (currentBrush.Transparency != brush.Transparency)
            {
                return true;
            }
            if (currentBrush.Width != brush.Width)
            {
                return true;
            }
            return false;
        }

        private int IncrementId()
        {
            return id++;
        }
        #endregion Converting Functions
    }
}
