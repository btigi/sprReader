/*
4 char  RSPR
4 unknown (always 0x10020000?)
4 frame count
4 rotation count (first frame should always point East, orientation moves counter clockwise)
4 width
4 height
4 frame count (total?)
4 section count
frame count (in this section?) * 4 - frame ordering (-indexed)
4 first frame number
4 last frame number
??? unknown
[4 frame number (increments by 2 instead of 1...?)
 4 bytes specified in file to create frame]

29x 1E (fill x pixels with the default colour, i.e. fill 29 x 1E pixels with the default colour)
01x 0B (fill x pixels with the default colour, i.e. fill 0B x 1E pixels with the default colour)
1   number of explicit bytes
x   explit bytes
01x 11 (fill x pixels with the default colour, i.e. fill 01 x 11 pixels with the default colour)
01x 09 (fill x pixels with the default colour, i.e. fill 01 x 09 pixels with the default colour)
...
27x 1E

once the default colour is specified, any remaining unspecified pixels are filled with it


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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace sprReader
{
    public class sprConverter
    {
        public string filename;
        private FileStream sprFileStream;
        private BinaryReader sprBinaryReader;
        private const int DEFAULTCOLOUR = 0; // TEMP - should come from palette
        public Bitmap Palette { get; set; }

        public void Open(string ftgFilename)
        {
            sprFileStream = new FileStream(ftgFilename, FileMode.Open, FileAccess.Read);
            sprBinaryReader = new BinaryReader(sprFileStream);
            filename = ftgFilename;
        }

        public void Parse()
        {
            var identifier = sprBinaryReader.ReadChars(4);
            var version = sprBinaryReader.ReadBytes(4); // This can't be a literal version, unless its version 528(!) but it always seems constant
            var framesPerSectionCount = sprBinaryReader.ReadBytes(4);
            var rotationCount = sprBinaryReader.ReadInt32();
            var width = sprBinaryReader.ReadInt32();
            var height = sprBinaryReader.ReadInt32();
            var frameCountTotal = sprBinaryReader.ReadInt32();
            var sectionCount = sprBinaryReader.ReadInt32();
            for (int i = 0; i < frameCountTotal; i++)
            {
                var frameOrder = sprBinaryReader.ReadInt32();
            }
            var firstFrame = sprBinaryReader.ReadInt32(); // of each sequence
            var lastFrame = sprBinaryReader.ReadInt32(); // of each sequence

            // There are a bunch of unknown bytes now, from 16 up to around 120. No idea how to to calculate how many there are in a given file
            // As the start of the frame data is not located by offset, but just by parsing the file up to that point, this means we can't 
            // effectively read files.

            // The next bytes after the unknown ones are the frame size ones, which will be a dword 0 and a frame size.
            // We can try reading the file until we hit something that looks like a frame size (e.g. greater than 50, which
            // seems reasonable for a size) but this is not a terribly reliable method
            //var pos = sprBinaryReader.BaseStream.Position;
            //var tmp = 0;
            //while (tmp < 50)
            //{
            //    var data = sprBinaryReader.ReadBytes(2);
            //    tmp = BitConverter.ToInt16(data, 0);
            //}
            //pos = sprBinaryReader.BaseStream.Position - pos;


            // Output some debug information to try and figure out the unknown bytes
            //System.Diagnostics.Trace.WriteLine(String.Format("File|{0, 40}|frames|{1,3}|rotations|{2,2}|length|{3,3}", filename, frameCountTotal, rotationCount, pos));


            // So that we can actually we the data out we'll hardcode the amount of bytes to skip for some test files
            if (filename == "D:\\tmp\\Dark Reign\\ecsmosm1.spr") // 10 frames, 1 rotations, 2 hotspots
            {
                sprBinaryReader.ReadBytes(0x34); // 52
            }

            if (filename == "D:\\tmp\\Dark Reign\\uowtrst0.spr") // 3 frames, 16 rotations, 0 hotspots
            {
                sprBinaryReader.ReadBytes(0x18); // 24
            }

            if (filename == "D:\\tmp\\Dark Reign\\bfagtmn0.spr") // 1 frame, 1 rotations, 0 hotspots
            {
                sprBinaryReader.ReadBytes(0x10); // 16
            }

            if (filename == "D:\\tmp\\Dark Reign\\aoctr007.spr") // 1 frame, 1 rotations, 1 hotspot
            {
                sprBinaryReader.ReadBytes(0x10); // 16
            }

            if (filename == "D:\\tmp\\Dark Reign\\aoclf000.spr") // 1 frame, 1 rotations, 1 hotspot
            {
                sprBinaryReader.ReadBytes(0x10); // 16
            }


            for (int i = 0; i < frameCountTotal; i++)
            {
                var unknown5 = sprBinaryReader.ReadBytes(4);
                var frameDataSize = BitConverter.ToInt32(sprBinaryReader.ReadBytes(4), 0);
            }

            var dataByte = 0;
            for (int frame = 0; frame < frameCountTotal; frame++)
            {
                var data = new byte[(width * height) * 3];

                var currentRow = 0; // how far down the image we've progressed
                var widthProgress = 0; // how far across the current row we've progressed - used to increment currentRow when we've completed a row
                var doneDefault = false; // whether we've completed the initial default byte reading
                var doneData = false; // whether we've completed the data reading
                var dataPosition = 0; // position in the data array, where we write the actual image bytes

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

                var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

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
                        dest = dest + 3;
                    }
                }

                Marshal.Copy(pixelData, 0, bData.Scan0, pixelData.Length);
                bitmap.UnlockBits(bData);

                filename = Path.GetFileName(filename);
                bitmap.Save(String.Format(@"D:\tmp\dark reign\bmp\{0}_{1}.bmp", filename, frame));
            }

            var hotSpotCount = sprBinaryReader.ReadInt32();
            for (int i = 0; i < hotSpotCount; i++)
            {
                var hotSpotX = sprBinaryReader.ReadByte();
                var hotSpotY = sprBinaryReader.ReadByte();
                var unknown = sprBinaryReader.ReadByte(); // should be 0x01
            }
        }

        private Color GetColour(byte paleteIndex)
        {
            return Palette.Palette.Entries[paleteIndex];
        }

        public void Close()
        {
            if (sprBinaryReader != null)
            {
                sprBinaryReader.Close();
            }
            if (sprFileStream != null)
            {
                sprFileStream.Close();
            }
        }
    }
}