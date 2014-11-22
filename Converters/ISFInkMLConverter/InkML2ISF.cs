/*
 * @(#) InkML2ISF.cs 1.0 06-08-2007 author: Manoj Prasad
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
using System.IO;
using Microsoft.Ink;
using InkML;

namespace InkMLConverters
{
	/// <summary>
	///
	/// </summary>

    public class InkML2ISF
    {
        #region Fields

        private InkInterpreter inkmlInterpreter;
        private Dictionary<string,Guid> mapChannelGuid;
        private Microsoft.Ink.Ink ISFink;

        /// <summary>
        /// Gets the Microsoft.Ink object
        /// </summary>
        public Microsoft.Ink.Ink ISFInk
        {
            get { return ISFink; }
        }
        #endregion Fields

        #region Constructor
        public InkML2ISF()
        {
            try
            {
                ReInitialize();
                Initialize();
            }
            catch(FileNotFoundException)
            {
                
            }
        }
        
        /// <summary>
        /// Function to re initialise the object
        /// </summary>
        public void ReInitialize()
        {
            inkmlInterpreter = new InkInterpreter();
            ISFink = new Microsoft.Ink.Ink();
        }

        /// <summary>
        /// Function to Initialise the Map between various channels in InkML to Guid if channels supported by the ISF
        /// </summary>
        private void Initialize()
        {
            mapChannelGuid = new Dictionary<string,Guid>();
            mapChannelGuid.Add("X",PacketProperty.X);
            mapChannelGuid.Add("Y",PacketProperty.Y);
            mapChannelGuid.Add("Z",PacketProperty.Z);
            mapChannelGuid.Add("T",PacketProperty.TimerTick);
            mapChannelGuid.Add("F",PacketProperty.NormalPressure);
            mapChannelGuid.Add("OTX",PacketProperty.XTiltOrientation);
            mapChannelGuid.Add("OTY",PacketProperty.YTiltOrientation);
            mapChannelGuid.Add("OR",PacketProperty.RollRotation);
            mapChannelGuid.Add("OA",PacketProperty.YawRotation);
            mapChannelGuid.Add("OE",PacketProperty.PitchRotation);
            mapChannelGuid.Add("E",PacketProperty.AltitudeOrientation);
            mapChannelGuid.Add("A",PacketProperty.AzimuthOrientation);
            mapChannelGuid.Add("B",PacketProperty.ButtonPressure);
            mapChannelGuid.Add("PacketStatus",PacketProperty.PacketStatus);
            mapChannelGuid.Add("S.NO",PacketProperty.SerialNumber);
            mapChannelGuid.Add("TP",PacketProperty.TangentPressure);
            mapChannelGuid.Add("TO",PacketProperty.TwistOrientation);

        }

        #endregion Constructor

        #region Converter and Save Functions

        /// <summary>
        /// Function to convert Input InkML File to MS.Ink Object.
        /// This function can be used to render the InkML file with TabletPC SDK
        /// </summary>
        /// <param name="inputFileName">InkML file to be converted to the MS.ink Object</param>
        public void ConvertToInk(string inputFileName)
        {
            try
            {
                inkmlInterpreter.LoadInkFile(inputFileName);
                ISFink = new Microsoft.Ink.Ink();
                Conversion(inkmlInterpreter.Ink);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Function to convert InkML file to ISF file.
        /// </summary>
        /// <param name="inputFileName">InkML File to be Converted To ISF</param>
        /// <param name="outputFileName">Output ISF file</param>
        public void ConvertToISF(string inputFileName, string outputFileName)
        {
            try
            {
                inkmlInterpreter.LoadInkFile(inputFileName);
                ConvertToISF(inkmlInterpreter.Ink,outputFileName);
            }
            catch (Exception)
            {
            }
        }
        public string ConvertToISF(string inputFileName)
        {
            try
            {
                inkmlInterpreter.LoadInkFile(inputFileName);
                return ConvertToISF(inkmlInterpreter.Ink);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Function to Convert the InkML.Ink object to ISF file.
        /// Calls the Conversion function to the MS.Ink Object.
        /// Then Saves the MS.Ink object to the ISF file.
        /// </summary>
        /// <param name="inputInk">InkML Ink Object to be converted to ISF</param>
        /// <param name="outputFileName">output ISF FILE</param>
        public void ConvertToISF(InkML.Ink inputInk, string outputFileName)
        {
            try
            {
                ISFink = new Microsoft.Ink.Ink();
                Conversion(inputInk);
                SaveISF(outputFileName);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Function to Convert the InkML.Ink object to ISF file.
        /// Calls the Conversion function to the MS.Ink Object.
        /// Then Saves the MS.Ink object to the ISF file.
        /// </summary>
        /// <param name="inputInk">InkML Ink Object to be converted to ISF</param>
        /// <param name="outputFileName">output ISF FILE</param>
        public string ConvertToISF(InkML.Ink inputInk)
        {
            try
            {
                ISFink = new Microsoft.Ink.Ink();
                Conversion(inputInk);
                return Convert.ToBase64String(ISFink.Save(PersistenceFormat.InkSerializedFormat));
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Function to save the MS.Ink object to ISF file.
        /// </summary>
        /// <param name="outputFileName"></param>
        private void SaveISF(string outputFileName)
        {
            FileStream fsTemp = new FileStream(outputFileName, FileMode.Create);
            fsTemp.Write(ISFink.Save(PersistenceFormat.InkSerializedFormat), 0, ISFink.Save(PersistenceFormat.InkSerializedFormat).Length);
            fsTemp.Close();
        }

        #endregion Load and Save Functions

        #region Convert/Add Functions

       
        /// <summary>
        /// Main Conversion Function to convert InkML Ink object to Microsoft.Ink object.
        /// Converts Trace to Stroke Objects and adds it to MS.Ink object
        /// Converts Annotation/AnnotationXML to MS.Ink.ExtendedProperties.
        /// TODO : Converting TraceGroup to the Strokes object.
        /// Add the Recognition Result from the Annotation/ AnnotationXML element in the TraceGroup
        /// </summary>
        /// <param name="inkmlinput">InkML Ink object to be converted to the MS.Ink object</param>
        private void Conversion(InkML.Ink inkmlinput)
        {
            List<InkElement>.Enumerator inkmlEnumerator = inkmlinput.GetInkElements();
            while (inkmlEnumerator.MoveNext())
            {
                InkElement inkmlElement = inkmlEnumerator.Current;
                if (inkmlElement.TagName.Equals("trace"))
                {
                    AddStroke(inkmlElement as Trace);
                }
                if (inkmlElement.TagName.Equals("traceGroup"))
                {
                    //TODO :Add a function to convert to convert TraceGroup to Strokes object
                    //      Add the Strokes object to the ISFink object.
                    //      Add the Strokes object to the ISFink.customstrokes.

                }
                if(inkmlElement.TagName.Equals("annotation"))
                {
                    AddExtendedProperty(inkmlElement as Annotation);
                }
                if(inkmlElement.TagName.Equals("annotationXML"))
                {
                    AddExtendedProperty(inkmlElement as AnnotationXML);
                }
            }
        }

        /// <summary>
        /// Function to Add a Stroke corresponding to a Trace in the InkML.
        /// This function adds the TraceData of the TraceObject to the Stroke object.
        /// Calls the GetDrawingAttributes function to convert the AssociatedBrush Element 
        /// to the drawingAttributes and assigns it to the drawing attribute of the stroke.        /// 
        /// </summary>
        /// <param name="inkmlTrace"></param>
        private void AddStroke(Trace inkmlTrace)
        {

            System.Drawing.Point[] strokePoints = new System.Drawing.Point[inkmlTrace.GetTraceData("X").Count];   
            List<string> channels = inkmlTrace.GetChannelNames();
            int x, y;
            TraceFormat traceFormat;
            if (inkmlTrace.AssociatedContext.InksourceElement.TraceFormat != null)
                traceFormat = inkmlTrace.AssociatedContext.InksourceElement.TraceFormat;
            else
                traceFormat = inkmlTrace.AssociatedContext.TraceFormatElement;
            string X_unit = traceFormat.GetChannel("X").Units;
            string Y_unit = traceFormat.GetChannel("Y").Units;
            for (int i = 0; i < inkmlTrace.GetTraceData("X").Count; i++)
            {
                double xTransformFactor = 1.00, yTransformFactor = 1.00;
                // convert the trace data to the device ("dev") unit which is -
                // HIMETRIC unit for TabletPC SDK and hence for the ISF format.
                // 1 HIMETRIC = 0.01 mm = 25.4/0.01 inches.
                // Finaly the data is divided by 96 PPI which is the typical screen resolution,

                if (X_unit.Equals("inch"))
                    xTransformFactor = (25.4 / 0.01);
                else if (X_unit.Equals("dev"))
                    xTransformFactor = 1.00;
                else if (X_unit == "cm")
                    xTransformFactor = (10.0 / 0.01);
                x = Convert.ToInt32(Convert.ToDouble(inkmlTrace.GetTraceData("X")[i]) * xTransformFactor);

                if (Y_unit.Equals("inch"))
                    yTransformFactor = (25.4 / 0.01);
                else if (Y_unit.Equals("dev"))
                    yTransformFactor = 1.00;
                else if (Y_unit == "cm")
                    yTransformFactor = (10 / .01);
                y = Convert.ToInt32(Convert.ToDouble(inkmlTrace.GetTraceData("Y")[i]) * yTransformFactor);

                strokePoints[i] = new System.Drawing.Point(x, y);
            }

            Microsoft.Ink.Stroke isfStroke = ISFink.CreateStroke(strokePoints);
            
            foreach (string channel in channels)
            {
                if (!channel.Equals("X") && !channel.Equals("Y"))
                {
                    var ret = inkmlTrace.GetTraceData(channel);
                    int[] array = (int[])ret.ToArray(typeof(int));
                    try
                    {
                        isfStroke.SetPacketValuesByProperty(mapChannelGuid[channel], 0, array.Length, array);
                    }
                    catch (Exception e) { Console.WriteLine("Could not set pressure"); }
                }
            }
            if (inkmlTrace.AssociatedBrush != null)
            {
                isfStroke.DrawingAttributes = GetDrawingAttributes(inkmlTrace.AssociatedBrush);
            }
            else
            {
                isfStroke.DrawingAttributes = new DrawingAttributes();
            }
            ISFink.Strokes.Add(isfStroke);
        }

        /// <summary>
        ///  Function to convert InkML AnnotationXML to ExtendedProperty element in MicroSoft.Ink
        /// This function adds the XmlElement corresponding to the AnnotationXML to the 
        /// ExtendedProperties element of the Ink Object.
        /// </summary>
        /// <param name="inkmlAnnotationXML">AnnotationXML Element to be added to the Ink Object</param>
        private void AddExtendedProperty(AnnotationXML inkmlAnnotationXML)
        {
            Guid temp;
            if (!inkmlAnnotationXML.GetAttribute("id").Equals(""))
            {
                temp = new Guid(inkmlAnnotationXML.GetAttribute("id"));
                ISFink.ExtendedProperties.Add(temp, inkmlAnnotationXML.AnnotationElement);
            }
        }

        /// <summary>
        /// Function to convert InkML Annotation to ExtendedProperty element in MicroSoft.Ink
        /// This function adds the XmlElement corresponding to the Annotation to the 
        /// ExtendedProperties element of the Ink Object.
        /// 
        /// </summary>
        /// <param name="inkmlAnnotation">Annotation Element to be added to the Ink Object</param>
        private void AddExtendedProperty(Annotation inkmlAnnotation)
        {
            Guid temp;
            if (!inkmlAnnotation.GetAttribute("id").Equals(""))
            {
                temp = new Guid(inkmlAnnotation.GetAttribute("id"));
                ISFink.ExtendedProperties.Add(temp, inkmlAnnotation.AnnotationElement);
            }
        }


        /// <summary>
        /// Function to convert the Brush element of InkML to DrawingAttributes of a stroke.
        /// * Maps the Elements Value to the various Drawing Attributes.
        /// * Fixed elements of the ISF are supported.Need to extend it for the
        ///  Extended property of the Drawing Attribute.
        /// - Run a Loop to know all the elements and attribute in the Annotation/AnnotationXML
        /// - Add each attribute and element to the Extended Attribute as the Data object with
        ///   a unique Guid.
        /// </summary>
        /// <param name="inkmlBrush">InkML Brush Element to be converted to the DrawingAttribute</param>
        /// <returns>Drawing Attributes Object</returns>
        private DrawingAttributes GetDrawingAttributes(Brush inkmlBrush)
        {
            DrawingAttributes result = new DrawingAttributes();
            AnnotationXML bAnnotation = inkmlBrush.BrushAnnotationXML;
            if (bAnnotation == null)
                return result;
            if (bAnnotation.GetProperty("color")!=null)
            {
                result.Color = System.Drawing.Color.FromName(bAnnotation.GetProperty("color"));
            }
            if (bAnnotation.GetProperty("width") != null)
            {
                result.Width = System.Convert.ToInt32(bAnnotation.GetProperty("width"));
            }
            if (bAnnotation.GetProperty("transparency") != null)
            {
                result.Transparency = System.Convert.ToByte(bAnnotation.GetProperty("transparency"));
            }
            if (bAnnotation.GetProperty("antialiased") != null)
            {
                result.AntiAliased = System.Convert.ToBoolean(bAnnotation.GetProperty("antialiased"));
            }
            if (bAnnotation.GetProperty("fittocurve") != null)
            {
                result.FitToCurve = System.Convert.ToBoolean(bAnnotation.GetProperty("fittocurve"));
            }
            if (bAnnotation.GetProperty("height") != null)
            {
                result.Height = System.Convert.ToInt32(bAnnotation.GetProperty("height"));
            }
            if (bAnnotation.GetProperty("ignorePressure") != null)
            {
                result.IgnorePressure = System.Convert.ToBoolean(bAnnotation.GetProperty("ignorePressure"));
            }
            if (bAnnotation.GetProperty("pentip") != null)
            {
                result.PenTip = (PenTip)System.Enum.Parse(typeof(PenTip), bAnnotation.GetProperty("pentip"));
            }
            return result;
        }

        #endregion Convert/Add Functions

    }
}
