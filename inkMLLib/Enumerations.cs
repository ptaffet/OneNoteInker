/*
 * @(#) Enumerations.cs 1.0 06-08-2007 author: Manoj Prasad
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


namespace InkML
{
    // Different channels type.
    public enum ChannelType
    {
        DECIMAL, INTEGER, BOOLEAN
    }

    // The orientation  of Channel.
    public enum OrientationType
    {
        POSITIVE, NEGATIVE
    }

    public enum TraceType
    {
        PenDown,
        PenUp,
        Indeterminate
    }

    public enum TraceContinuation
    {
        Begin,
        End,
        Middle,
        None
    }


    /// <summary>
    ///  Various datatypes in the trace data.
    /// </summary>
    public enum TraceDatatype
    {
        Boolean,
        Decimal,
        Integer,
        Hex,
        None
    }

    /// <summary>
    /// Various qualifiers in the trace data
    /// </summary>
    public enum TraceQualifiers
    {
        Velocity,       // ' denotes single difference
        Acceleration,   // " denotes double difference
        Explicit,       // ! denotes explicit difference
        Novalue,        // ? denotes No value for the channel
        Prevvalue,      // * denotes Previous value is unchanged for the channel.
        None
    }

    public enum TransformType
    {
        Resolution,
        Canvas
    }
}
