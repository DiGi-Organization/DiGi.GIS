using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Determines whether an orthophoto JPEG tile is blank, that is of one uniform colour, within <see cref="Constants.OrtoBlank.Tolerance"/>.
        /// <para>This is the second of the three classification steps (exception, blank, content hash) for hosts that carry no image library: see <see cref="IsOrtoJpegBlank(byte[], int)"/>.</para>
        /// </summary>
        /// <param name="bytes">The encoded JPEG tile.</param>
        /// <returns>True when the tile is uniform; false when it has content, is not a baseline JPEG, or cannot be parsed.</returns>
        public static bool IsOrtoJpegBlank(this byte[]? bytes)
        {
            return IsOrtoJpegBlank(bytes, Constants.OrtoBlank.Tolerance);
        }

        /// <summary>
        /// Determines whether an orthophoto JPEG tile is blank, that is of one uniform colour, without decoding pixels.
        /// <para>The GUGiK WMS answers a missing or flaky epoch with HTTP 200 and a valid uniform white JPEG, so <see cref="IsOrtoImage(byte[])"/> cannot tell it from imagery. This check reads only the entropy-coded coefficients of a baseline (SOF0) JPEG: a tile is blank when every AC coefficient of every 8 × 8 block is zero (each block is flat) and, per colour component, the dequantised block means (DC / 8) spread by no more than <paramref name="tolerance"/>. A tile that is only partly empty - a mosaic that includes uncovered ground, filled black by the service - is not blank.</para>
        /// <para>Real imagery is rejected at its first non-flat block, so the cost is a few blocks for content and one pass over the (small) scan for a blank. Progressive, lossless or hierarchical JPEG, PNG, and any malformed stream answer false: an undecidable payload is never classified as blank. The pixel-based <c>DiGi.GIS.Emgu.CV.Query.IsOrtoBlank</c> remains the reference where Emgu is available.</para>
        /// </summary>
        /// <param name="bytes">The encoded JPEG tile.</param>
        /// <param name="tolerance">The largest per-component spread of the block means, on 0–255, that still counts as uniform.</param>
        /// <returns>True when the tile is uniform; false when it has content, is not a baseline JPEG, or cannot be parsed.</returns>
        public static bool IsOrtoJpegBlank(this byte[]? bytes, int tolerance)
        {
            if (bytes is null || bytes.Length < 4 || tolerance < 0 || bytes[0] != 0xFF || bytes[1] != 0xD8)
            {
                return false;
            }

            Dictionary<int, int> quantizationDcs = [];
            Dictionary<int, Dictionary<int, byte>> huffmanTables = [];
            List<(int Id, int Horizontal, int Vertical, int QuantizationTable)> frameComponents = [];
            List<(int Horizontal, int Vertical, int QuantizationTable, int DcTable, int AcTable)> scanComponents = [];

            int width = 0;
            int height = 0;
            int restartInterval = 0;
            int position = 2;
            int scanStart = -1;

            while (position + 4 <= bytes.Length)
            {
                if (bytes[position] != 0xFF)
                {
                    return false;
                }

                byte marker = bytes[position + 1];
                if (marker == 0xFF)
                {
                    position++;
                    continue;
                }

                int length = (bytes[position + 2] << 8) | bytes[position + 3];
                int segmentStart = position + 4;
                int segmentEnd = position + 2 + length;
                if (length < 2 || segmentEnd > bytes.Length)
                {
                    return false;
                }

                if (marker == 0xDB)
                {
                    int index = segmentStart;
                    while (index < segmentEnd)
                    {
                        int precision = bytes[index] >> 4;
                        int table = bytes[index] & 0x0F;
                        index++;
                        int size = precision == 0 ? 64 : 128;
                        if (index + size > segmentEnd)
                        {
                            return false;
                        }

                        quantizationDcs[table] = precision == 0 ? bytes[index] : (bytes[index] << 8) | bytes[index + 1];
                        index += size;
                    }
                }
                else if (marker == 0xC0)
                {
                    if (length < 8)
                    {
                        return false;
                    }

                    height = (bytes[segmentStart + 1] << 8) | bytes[segmentStart + 2];
                    width = (bytes[segmentStart + 3] << 8) | bytes[segmentStart + 4];
                    int count = bytes[segmentStart + 5];
                    if (segmentStart + 6 + (3 * count) > segmentEnd)
                    {
                        return false;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        int offset = segmentStart + 6 + (3 * i);
                        frameComponents.Add((bytes[offset], bytes[offset + 1] >> 4, bytes[offset + 1] & 0x0F, bytes[offset + 2]));
                    }
                }
                else if (marker is >= 0xC1 and <= 0xCF && marker != 0xC4 && marker != 0xC8 && marker != 0xCC)
                {
                    // Progressive, lossless, hierarchical or arithmetic-coded: not decided here.
                    return false;
                }
                else if (marker == 0xC4)
                {
                    int index = segmentStart;
                    while (index + 17 <= segmentEnd)
                    {
                        int tableClass = bytes[index] >> 4;
                        int tableId = bytes[index] & 0x0F;
                        int valueIndex = index + 17;
                        Dictionary<int, byte> codes = [];
                        int code = 0;
                        for (int codeLength = 1; codeLength <= 16; codeLength++)
                        {
                            int countOfLength = bytes[index + codeLength];
                            for (int i = 0; i < countOfLength; i++)
                            {
                                if (valueIndex >= segmentEnd)
                                {
                                    return false;
                                }

                                codes[(codeLength << 16) | code] = bytes[valueIndex];
                                valueIndex++;
                                code++;
                            }

                            code <<= 1;
                        }

                        huffmanTables[(tableClass << 4) | tableId] = codes;
                        index = valueIndex;
                    }
                }
                else if (marker == 0xDD)
                {
                    if (length < 4)
                    {
                        return false;
                    }

                    restartInterval = (bytes[segmentStart] << 8) | bytes[segmentStart + 1];
                }
                else if (marker == 0xDA)
                {
                    int count = bytes[segmentStart];
                    if (segmentStart + 1 + (2 * count) > segmentEnd)
                    {
                        return false;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        int id = bytes[segmentStart + 1 + (2 * i)];
                        int tables = bytes[segmentStart + 2 + (2 * i)];
                        int frameIndex = frameComponents.FindIndex(x => x.Id == id);
                        if (frameIndex < 0)
                        {
                            return false;
                        }

                        (int _, int horizontal, int vertical, int quantizationTable) = frameComponents[frameIndex];
                        scanComponents.Add((horizontal, vertical, quantizationTable, tables >> 4, tables & 0x0F));
                    }

                    scanStart = segmentEnd;
                    break;
                }

                position = segmentEnd;
            }

            // Only a single interleaved scan carrying every frame component is decided.
            if (scanStart < 0 || width <= 0 || height <= 0 || frameComponents.Count == 0 || scanComponents.Count != frameComponents.Count)
            {
                return false;
            }

            int maximumHorizontal = 1;
            int maximumVertical = 1;
            foreach ((int horizontal, int vertical, int quantizationTable, int dcTable, int acTable) in scanComponents)
            {
                if (horizontal < 1 || vertical < 1 || !quantizationDcs.ContainsKey(quantizationTable) || !huffmanTables.ContainsKey(dcTable) || !huffmanTables.ContainsKey(0x10 | acTable))
                {
                    return false;
                }

                maximumHorizontal = Math.Max(maximumHorizontal, horizontal);
                maximumVertical = Math.Max(maximumVertical, vertical);
            }

            long mcuCount = (long)((width + (8 * maximumHorizontal) - 1) / (8 * maximumHorizontal)) * ((height + (8 * maximumVertical) - 1) / (8 * maximumVertical));

            int readPosition = scanStart;
            int bitBuffer = 0;
            int bitCount = 0;
            bool endOfData = false;

            int[] predictions = new int[scanComponents.Count];
            double[] minimums = new double[scanComponents.Count];
            double[] maximums = new double[scanComponents.Count];
            for (int i = 0; i < scanComponents.Count; i++)
            {
                minimums[i] = double.MaxValue;
                maximums[i] = double.MinValue;
            }

            for (long mcu = 0; mcu < mcuCount; mcu++)
            {
                if (restartInterval > 0 && mcu > 0 && mcu % restartInterval == 0)
                {
                    bitCount = 0;
                    if (readPosition + 1 >= bytes.Length || bytes[readPosition] != 0xFF || bytes[readPosition + 1] < 0xD0 || bytes[readPosition + 1] > 0xD7)
                    {
                        return false;
                    }

                    readPosition += 2;
                    endOfData = false;
                    Array.Clear(predictions, 0, predictions.Length);
                }

                for (int component = 0; component < scanComponents.Count; component++)
                {
                    (int horizontal, int vertical, int quantizationTable, int dcTable, int acTable) = scanComponents[component];
                    Dictionary<int, byte> dcCodes = huffmanTables[dcTable];
                    Dictionary<int, byte> acCodes = huffmanTables[0x10 | acTable];
                    int quantizationDc = quantizationDcs[quantizationTable];

                    for (int block = 0; block < horizontal * vertical; block++)
                    {
                        int category = Decode(dcCodes);
                        if (category < 0 || category > 15)
                        {
                            return false;
                        }

                        int difference = Receive(category);
                        if (endOfData)
                        {
                            return false;
                        }

                        predictions[component] += difference;
                        double mean = predictions[component] * quantizationDc / 8.0;
                        minimums[component] = Math.Min(minimums[component], mean);
                        maximums[component] = Math.Max(maximums[component], mean);
                        if (maximums[component] - minimums[component] > tolerance)
                        {
                            return false;
                        }

                        int coefficient = 1;
                        while (coefficient < 64)
                        {
                            int runSize = Decode(acCodes);
                            if (runSize < 0)
                            {
                                return false;
                            }

                            if ((runSize & 0x0F) != 0)
                            {
                                // A non-zero AC coefficient: the block is not flat.
                                return false;
                            }

                            if (runSize != 0xF0)
                            {
                                // End of block.
                                break;
                            }

                            coefficient += 16;
                        }
                    }
                }
            }

            return true;

            int ReadBit()
            {
                if (bitCount == 0)
                {
                    if (readPosition >= bytes.Length)
                    {
                        endOfData = true;
                        return 0;
                    }

                    int value = bytes[readPosition];
                    readPosition++;
                    if (value == 0xFF)
                    {
                        if (readPosition < bytes.Length && bytes[readPosition] == 0x00)
                        {
                            readPosition++;
                        }
                        else
                        {
                            // A marker inside the expected scan data: the stream ended early.
                            readPosition--;
                            endOfData = true;
                            return 0;
                        }
                    }

                    bitBuffer = value;
                    bitCount = 8;
                }

                bitCount--;
                return (bitBuffer >> bitCount) & 1;
            }

            int Decode(Dictionary<int, byte> codes)
            {
                int code = 0;
                for (int codeLength = 1; codeLength <= 16; codeLength++)
                {
                    code = (code << 1) | ReadBit();
                    if (endOfData)
                    {
                        return -1;
                    }

                    if (codes.TryGetValue((codeLength << 16) | code, out byte symbol))
                    {
                        return symbol;
                    }
                }

                return -1;
            }

            int Receive(int category)
            {
                int value = 0;
                for (int i = 0; i < category; i++)
                {
                    value = (value << 1) | ReadBit();
                }

                if (category > 0 && value < (1 << (category - 1)))
                {
                    value -= (1 << category) - 1;
                }

                return value;
            }
        }
    }
}
