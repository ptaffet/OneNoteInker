/*
 * @(#) ISF2InkMLConverter.cs 1.0 06-08-2007 author: Manoj Prasad
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
 * $Revision: 259 $
 * $Author: selvarmu $
 * $LastChangedDate: 2008-07-06 14:36:54 +0530 (Sun, 06 Jul 2008) $
 ************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using InkML;
using InkMLConverters;
using Microsoft.Ink;

namespace ISF2InkMLConverter
{
	/// <summary>
	///
	/// </summary>

    class ISF2InkMLConverter
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length <= 0 || args.Length > 2)
                {
                    Console.WriteLine("Usage: isf2inkml <filename.isf> [<filename.inkml>]");
                    return;
                }

                if (args[0].ToLower().Contains(".isf"))
                {
                    string ConversionFileName="";
                    if (args.Length == 2) 
                    {
                        ConversionFileName = args[1];
                    }
                    else if (1 == args.Length)
                    {
                        ConversionFileName = args[0];
                        if (ConversionFileName.Contains("\\"))
                        {
                            int index = ConversionFileName.LastIndexOf("\\");
                            if(index>=0)
                            {
                                ConversionFileName = ConversionFileName.Substring(index + 1);                                
                            }
                        } 
                       ConversionFileName = ConversionFileName.Substring(0, ConversionFileName.Length - 4) + ".inkml";
                    }

                    if (ConversionFileName.ToLower().Contains(".inkml"))
                    {
                        ISF2InkML converter = new ISF2InkML();
                        converter.ConvertToInkML(args[0], ConversionFileName);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect output file extension. It should be '.inkml'.");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input file extension. It should be '.isf'.");                    
                }

            }
            catch (System.IO.FileNotFoundException e)
            {
                string errorMsg;
                if (e.Message.Contains("Microsoft.Ink"))
                {
                    errorMsg = "Could not load assembly 'Microsoft.Ink'.\n";
                    errorMsg += "Please Install the TabletPC SDK and copy the dll (Microsoft.Ink.dll) to the \n";
                    errorMsg += "ISF2InkMLConverter installation folder where you have the ISF2InkMLConverter.exe.";

                    Console.WriteLine(errorMsg, "Error");
                }

                if (e.Message.Contains("InkML") || e.Message.Contains("ISFInkMLConverter"))
                {
                    errorMsg = "Could not load assembly 'InkMLLibcs' or 'ISFInkMLConverter'.\n";
                    errorMsg += "Please download assembly from sourceforge.net and copy the dlls to the \n";
                    errorMsg += "ISF2InkMLConverter installation folder where you have the ISF2InkMLConverter.exe.";

                    Console.WriteLine(errorMsg, "Error");
                }
            }
        }
    }
}
