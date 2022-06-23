/*
Terminology:
    section -> an animation purpose, e.g. walking, firing
      rotation -> the direction the sprite is facing
        frame -> the individual image of a sprite

HEADER
4 char  RSPR / SSPR
4 version (always 0x10020000)
4 frame count
4 rotation count (first frame should always point East, orientation moves counter clockwise)
4 width
4 height
4 frame count (total?)
4 section count

FRAME INDICES
frame count (in this section?) * 4 - frame ordering (-indexed)

SECTIONS
4 first frame number
4 last frame number
4 framerate
4 hotspot count

ANIMATION INFORMATION?

FRAME INFORMATION

FRAMES
29x 1E (fill x pixels with the default colour, i.e. fill 29 x 1E pixels with the default colour)
01x 0B (fill x pixels with the default colour, i.e. fill 0B x 1E pixels with the default colour)
1   number of explicit bytes
x   explit bytes
01x 11 (fill x pixels with the default colour, i.e. fill 01 x 11 pixels with the default colour)
01x 09 (fill x pixels with the default colour, i.e. fill 01 x 09 pixels with the default colour)
...
27x 1E

once the default colour is specified, any remaining unspecified pixels are filled with it

HOTSPOTS
hot spot information is stored after the frame data
4 length of hotspot data (0 = no hotspots)
  -- data below is option, dependent on above value
  foreach frame:
    1 hotspot 1 x coord
    1 hotspot 1 y coord
    1 unknown (always 0x01)
    1 hotspot 2 x coord
    1 hotspot 2 y coord
    1 unknown (always 0x01)
    1 hotspot 3 x coord
    1 hotspot 3 y coord
    ...


max 6 sections:
 Movement
 Firing Animation
 Second Firing Animation
 Special Action Sections
 Idle Animation Sections
 Standing Animation

for each section...

  .---> rotations
  |     walk_east   walk_north  walk_west   walk_south
  |     frame_e1    frame_n1    frame_w1    frame_s1
frames  frame_e2    frame_n2    frame_w2
        frame_e3                frame_w3
        frame_e4                frame_w4
        frame_e5                
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace sprReader
{
    public class sprConverter
    {
        private const int HeaderSize = 32;
        private const int FrameOrderSize = 4;
        private const int SectionSize = 16;
        private const string SignatureSprite = "RSPR";
        private const string SignatureShadow = "SSPR";
        private const int Version = 528;

        private const int DEFAULTCOLOUR = 0; // TEMP - should come from palette

        private string filename { get; set; }
        private FileStream sprFileStream { get; set; }
        private BinaryReader sprBinaryReader { get; set; }
        public Bitmap Palette { get; set; }

        public void Open(string ftgFilename)
        {
            sprFileStream = new FileStream(ftgFilename, FileMode.Open, FileAccess.Read);
            sprBinaryReader = new BinaryReader(sprFileStream);
            filename = ftgFilename;
        }

        public List<Bitmap> Parse()
        {
            var result = new List<Bitmap>();

            var identifier = new string(sprBinaryReader.ReadChars(4));
            if (identifier != SignatureSprite && identifier != SignatureShadow)
            {
                throw new InvalidOperationException($"Unhandled SPR type: {identifier}");
            }

            var version = sprBinaryReader.ReadInt32();
            if (version != Version)
            {
                throw new InvalidOperationException($"Unhandled SPR version: {version}");
            }

            var framesPerSectionCount = sprBinaryReader.ReadInt32();
            var rotationCount = sprBinaryReader.ReadInt32();
            var width = sprBinaryReader.ReadInt32();
            var height = sprBinaryReader.ReadInt32();
            var frameCountTotal = sprBinaryReader.ReadInt32();
            var sectionCount = sprBinaryReader.ReadInt32();

            var sectionOffset = HeaderSize + (FrameOrderSize * frameCountTotal);
            var frameOffset = sectionOffset + (SectionSize * sectionCount) + (4 * framesPerSectionCount) + 4;


            // An array of frame indices, e.g. if there are 3 frames this lists 0,1,2
            for (int i = 0; i < frameCountTotal; i++)
            {
                var frameOrder = sprBinaryReader.ReadInt32();
            }


            // Section information
            sprBinaryReader.BaseStream.Position = sectionOffset;
            for (int i = 0; i < sectionCount; i++)
            {
                var sectionFirst = sprBinaryReader.ReadInt32();
                var sectionLast = sprBinaryReader.ReadInt32();
                var sectionFrameRate = sprBinaryReader.ReadInt32();
                var sectionHotSpotCount = sprBinaryReader.ReadInt32();
            }


            // (4 * framesPerSectionCount) + 4 bytes of data


            // Frame information
            sprBinaryReader.BaseStream.Position = frameOffset;
            for (int i = 0; i < frameCountTotal; i++)
            {
                var unknown5 = sprBinaryReader.ReadBytes(4);
                var frameDataSize = BitConverter.ToInt32(sprBinaryReader.ReadBytes(4), 0);
            }


            // Frame data
            byte dataByte;
            for (int frame = 0; frame < frameCountTotal; frame++)
            {
                var data = new byte[(width * height) * 3];

                var currentRow = 0; // how far down the image we've progressed
                var widthProgress = 0; // how far across the current row we've progressed - used to increment currentRow when we've completed a row
                var doneDefault = false; // whether we've completed the initial default byte reading
                var doneData = false; // whether we've completed the data reading
                var dataPosition = 0; // position in the data array, where we write the actual image bytes


                if (identifier == "SSPR")
                {
                    bool writeColour;
                    var runningOffset = 0;

                    // Not sure how to calculate the length, so we'll use the fact that the only other thing in the file is
                    // a single hotspot, of a fixed length, so the size muut be here to the end of the file, minus the length
                    // of the hotspot.
                    var pos = sprBinaryReader.BaseStream.Position;
                    var len = sprBinaryReader.BaseStream.Length - 7;
                    var dataLength = len - pos;

                    //  D:\data\DarkReign\aopln2sh.spr

                    /*
                    The shadow sprite treats each pixel as on (coloured) or off (transparent)
                    Read through the data bytes, with the first byte being how many off pixels to 
                    draw, the second byte being how many on pixels to draw, the third being how
                    many off pixels to draw and so on. The pattern resets whenever the number of
                    pixels we have drawn matches the width of the output image.
                    Note: If we hit a zero, the processing for this line is inverted (i.e. the first
                    byte is how many on pixels, rather than how many off pixels)
                    */

                    var cnt = 0;
                    var invertLevel = 0;
                    for (int i = 0; i < dataLength; i++)
                    {
                        if (runningOffset % width == 0)
                        {
                            cnt = 0;

                            // the first time we hit this statement, don't do anything
                            // the second time we hit this statement, set invert to false
                            if (invertLevel == 2)
                            {
                                invertLevel = 0;
                            }
                            if (invertLevel == 1)
                            {
                                invertLevel++;
                            }
                        }

                        writeColour = cnt % 2 != 0;
                        if (invertLevel == 2)
                        {
                            writeColour = cnt % 2 == 0;
                        }

                        dataByte = sprBinaryReader.ReadByte();

                        if (dataByte == default)
                        {
                            invertLevel = 1;
                        }

                        for (int j = 0; j < dataByte; j++)
                        {
                            data[runningOffset + (j * 3) + 0] = writeColour ? (byte)255 : (byte)0;
                            data[runningOffset + (j * 3) + 1] = writeColour ? (byte)255 : (byte)0;
                            data[runningOffset + (j * 3) + 2] = writeColour ? (byte)255 : (byte)0;
                        }
                        runningOffset += (dataByte * 3);
                        cnt++;
                    }
                }


                if (identifier == "RSPR")
                {
                    while (currentRow < height)
                    {
                        dataByte = sprBinaryReader.ReadByte();
                        if (dataByte == width)
                        {
                            // fill this row with the default colour
                            for (int i = 0; i < width * 3; i += 3)
                            {
                                data[dataPosition + i] = DEFAULTCOLOUR;
                                data[dataPosition + i + 1] = DEFAULTCOLOUR;
                                data[dataPosition + i + 2] = DEFAULTCOLOUR;
                            }
                            dataPosition += width * 3;
                            currentRow++;
                        }
                        else
                        {
                            if (!doneDefault)
                            {
                                // fill dataByte pixels with the default colour
                                for (int i = 0; i < dataByte * 3; i += 3)
                                {
                                    data[dataPosition + i] = DEFAULTCOLOUR;
                                    data[dataPosition + i + 1] = DEFAULTCOLOUR;
                                    data[dataPosition + i + 2] = DEFAULTCOLOUR;
                                }
                                dataPosition += dataByte * 3;
                                doneDefault = true;
                                widthProgress = dataByte;
                            }
                            else
                            {
                                if (!doneData)
                                {
                                    // fill dataByte pixels with the colours specified by the next dataByte bytes
                                    for (int i = 0; i < dataByte; i++)
                                    {
                                        var colourByte = sprBinaryReader.ReadByte();
                                        var colour = GetColour(colourByte);
                                        data[dataPosition] = colour.B;
                                        data[dataPosition + 1] = colour.G;
                                        data[dataPosition + 2] = colour.R;
                                        dataPosition += 3;
                                        widthProgress += 1;
                                    }
                                    doneData = true;
                                }
                                else
                                {
                                    // fill dataByte pixels with the default colour
                                    for (int i = 0; i < dataByte * 3; i += 3)
                                    {
                                        data[dataPosition + i] = DEFAULTCOLOUR;
                                        data[dataPosition + i + 1] = DEFAULTCOLOUR;
                                        data[dataPosition + i + 2] = DEFAULTCOLOUR;
                                    }
                                    dataPosition += dataByte * 3;
                                    widthProgress += dataByte;

                                    // Either we're at the end of the line, so we'll reset this anyway, or we're 
                                    // going to hit more data data, so we want to be expecting it
                                    doneData = false;
                                }
                            }
                        }

                        if (widthProgress == width)
                        {
                            currentRow++;
                            widthProgress = 0;
                            doneDefault = false;
                            doneData = false;
                        }
                    }
                }

                //using (var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                {
                    var bData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                    var size = bData.Stride * bData.Height;
                    var pixelData = new byte[size];

                    var src = 0;
                    var dest = 0;
                    for (int y = 0; y < height; y++)
                    {
                        // We need to account for the bitmap stride being aligned to a 4 byte boundary
                        while (dest % 4 != 0)
                        {
                            dest++;
                        }

                        for (int x = 0; x < width; x++)
                        {
                            pixelData[dest] = data[src]; //b
                            pixelData[dest + 1] = data[src + 1]; //g
                            pixelData[dest + 2] = data[src + 2]; //r
                            src += 3;
                            dest += 3;
                        }
                    }

                    Marshal.Copy(pixelData, 0, bData.Scan0, pixelData.Length);
                    bitmap.UnlockBits(bData);

                    //filename = Path.GetFileName(filename);
                    //bitmap.Save(String.Format(@"D:\data\DarkReign\{0}_{1}.bmp", filename, frame));
                    result.Add(bitmap);
                }
            }

            var hotSpotCount = sprBinaryReader.ReadInt32();
            for (int i = 0; i < hotSpotCount; i++)
            {
                var hotSpotX = sprBinaryReader.ReadByte();
                var hotSpotY = sprBinaryReader.ReadByte();
                var unknown = sprBinaryReader.ReadByte(); // should be 0x01
            }

            return result;
        }

        private Color GetColour(byte paleteIndex)
        {
            return Palette.Palette.Entries[paleteIndex];
        }

        public void Close()
        {
            sprBinaryReader?.Close();
            sprFileStream?.Close();
        }
    }
}