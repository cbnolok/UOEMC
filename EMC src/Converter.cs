using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Ultima;
using ImageMagick;
using Mythic.Package;

//Enhanced Map Converter relies on UltimaSDK by Malganis (included in this solution) to handle old map muls, since
// it is well-written, working code and not re-inventing the wheel speeds up the writing of this tool. Thanks Malganis!

/*                  FACET bin files FORMAT (thanks to Kons)

-BYTE   FACETID
-WORD   FILEID
while( 2 ){
        do{
                -BYTE Z                                           Facet_SUB
                -WORD LANDTILEGraphic   
                -bBYTE  DELIMITERCount
                if(HIBYTE(bBYTE)>0){
                        for ( i = 0 ; i < bBYTE; ++i){          
                                -dBYTE  DIRECTION                 Facet_SUB_SUB
                                if(dBYTE <= 7){
                                        -BYTE   Z
                                        -DWORD  Graphic
                                }
                        }
                }
                -cBYTE  STATICSCount
                if(cBYTE>0){
                        for ( i = 0 ; i < cBYTE; ++i){
                                -DWORD  Graphic                   Facet_SUB_SUB_2
                                -BYTE   Z
                                -DWORD  Color
                        }
                }
                ++y
        }while( y < 64 )
        ++x
        if( x < 64 )
                continue
        break
*/

namespace EMC
{
    class Converter
    {
       private MainForm form;

        private int width, height;
        private byte mapIndex;
        private string inPath, outPath, binPath;
        private byte task_mapstatics;
        private byte task_season;
        private bool task_uop;
        private bool task_dds;

        private TileMatrix tileMatrix;
        private Dictionary<ushort, ushort> landTileDictionary = new Dictionary<ushort, ushort>();

        private short[] radarcol;
        private Bitmap bmp;
        private BitmapData bmpData;
        private unsafe byte* bmpPtr;

        delegate void DelegateString(string msg);
        delegate void DelegateBool(bool state);
        delegate void DelegateInt(int i);
        delegate void DelegateVoid();
        private DelegateString CDAppendToLog;   // cached delegate
        private DelegateBool CDItemsState;
        private DelegateBool CDProgressBarSetMarquee;
        private DelegateInt CDProgressBarSetMax;
        private DelegateInt CDProgressBarSetVal;
        private DelegateVoid CDProgressBarIncrease;
        private DelegateVoid CDSuccess;

        public Converter(MainForm f, int wid, int hei, byte mi, string inp, string outp, string binp, byte tsk_mapstatics, byte tsk_season, bool tsk_uop, bool tsk_dds)
        {
            form = f;
            width = wid;
            height = hei;
            mapIndex = mi;
            inPath = inp;
            outPath = outp;
            binPath = binp;
            tileMatrix = new TileMatrix(inPath, mapIndex, mapIndex, width, height);

            task_mapstatics = tsk_mapstatics;
            task_season = tsk_season;
            task_uop = tsk_uop;
            task_dds = tsk_dds;

            CDAppendToLog = new DelegateString(form.AppendToLog);
            CDItemsState = new DelegateBool(form.SetItemsState);
            CDProgressBarSetMarquee = new DelegateBool(form.ProgressbarSetMarquee);
            CDProgressBarIncrease = new DelegateVoid(form.ProgressbarIncrease);
            CDProgressBarSetMax = new DelegateInt(form.ProgressbarSetMax);
            CDProgressBarSetVal = new DelegateInt(form.ProgressbarSetVal);
            CDSuccess = new DelegateVoid(form.Success);

            Log("Loading hues.mul into memory... ", true);
            Hues.Init();
            Log("Done!\n", true);

            if (task_season != 0)
                LoadLTDictionary();

            if (tsk_dds)
            {
                loadRadarcol();
                bmp = new Bitmap(width, height, PixelFormat.Format16bppRgb555);
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
                unsafe
                {
                    bmpPtr = (byte*)bmpData.Scan0.ToPointer();
                }
            }
        }

        private unsafe void loadRadarcol()
        {
            Log("Loading radarcol.mul into memory... ", true);
            FileInfo radarinfo = new FileInfo("radarcol.mul");
            radarcol = new short[radarinfo.Length / 2];           //since short is 2 bytes and i receive radarcol length in bytes
            fixed (short* pColors = radarcol)
            {
                using (FileStream fs = new FileStream("radarcol.mul", FileMode.Open, FileAccess.Read, FileShare.Read))
                    NativeMethods._lread(fs.SafeFileHandle, pColors, (int)radarinfo.Length); //0x10000);
            }
            Log("Done!\n", true);
        }

        //----

        private void Log(string msg, bool alwaysshow = false)
        {
            if (form.hidelog && !alwaysshow)
                return;
            try
            {
                form.Invoke(CDAppendToLog, msg);
            }
            catch (ObjectDisposedException)
            { }
        }

        private void SetItemsState(bool state)
        {
            try
            {
                form.Invoke(CDItemsState, state);
            }
            catch (ObjectDisposedException)
            { }
        }

        public void ProgressSetMarquee(bool on)
        {
            try
            {
                form.Invoke(CDProgressBarSetMarquee, on);
            }
            catch (ObjectDisposedException)
            { }
        }

        public void ProgressSetMax(int max)
        {
            try
            {
                form.Invoke(CDProgressBarSetMax, max);
            }
            catch (ObjectDisposedException)
            { }
        }

        private void ProgressIncrease() // set values 0-100
        {
            try
            {
                form.Invoke(CDProgressBarIncrease);
            }
            catch (ObjectDisposedException)
            { }
        }

        public void ProgressSetVal(int val) // set it in percentage 0-100
        {
            try
            {
                form.Invoke(CDProgressBarSetVal, val);
            }
            catch (ObjectDisposedException)
            { }
        }

        private void Success()
        {
            try
            {
                form.Invoke(CDSuccess);
            }
            catch (ObjectDisposedException)
            { }
        }

        //-----

        public static byte GetZ(sbyte z)
        {
            // tile.Z is a sbyte, we need a byte. Positive and negative heights are calculated with the two's complement.
            // This is a rough method, but it works and i don't need to perform bitwise operations:
            // 0-127: positive heights
            // 128-255: negative heights (255 = -1, and so on)
            if (z < 0)
                return (byte)(256 + (int)z);
            else
                return (byte)(z);
        }

        //----

        public ushort GetLandTileID(int tileID)
        {
            //if (task_season != 0)
            //{
            if (!landTileDictionary.ContainsKey((ushort)tileID))
                return (ushort)tileID;
            else
                return landTileDictionary[(ushort)tileID];
            //}
            //else
            //    return (short)tileID;
        }

        //load a single Conversion Table into the dictionary
        private void LoadTableIntoDictionary(string file)
        {
            Log(String.Format("Loading {0}", file));

            string convtable;
            string[] convtable_rows;
            using (StreamReader reader = new StreamReader(file))
            {
                convtable = reader.ReadToEnd();
                convtable_rows = convtable.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                for (int i = 0; i < convtable_rows.Length; i++)
                {
                    string[] convtable_row_elements;
                    ushort original, sub;
                    convtable_row_elements = convtable_rows[i].Split(' ');
                    if (convtable_row_elements.Length != 2)
                    {
                        Log(String.Format("ERROR parsing Conversion Table {0} at line {1}: key not loaded\n", file, i + 1), true);
                        continue;
                    }

                    try
                    {
                        if (convtable_row_elements[0].ToLowerInvariant().StartsWith("0x"))
                            original = Convert.ToUInt16(convtable_row_elements[0], 16);
                        else
                            original = UInt16.Parse(convtable_row_elements[0]);

                        if (convtable_row_elements[1].ToLowerInvariant().StartsWith("0x"))
                            sub = Convert.ToUInt16(convtable_row_elements[1], 16);
                        else
                            sub = UInt16.Parse(convtable_row_elements[1]);

                        landTileDictionary.Add(original, sub);
                    }
                    catch (FormatException)
                    {
                        Log(String.Format("ERROR parsing Conversion Table {0} at line {1}: key not loaded\n", file, i + 1), true);
                        continue;
                    }
                    catch (ArgumentException)
                    {
                        Log(String.Format("ERROR parsing Conversion Table {0} at line {1}: duplicated key\n", file, i + 1), true);
                        continue;
                    }
                }
            }
        }

        //loads the landtile dictionary which contains which tile is to be substituted with another
        private void LoadLTDictionary()
        {
            Log("Loading conversion tables... ", true);
            try
            {
                LoadTableIntoDictionary("convtables/forest2dirtTOsnow2dirt.txt");
                LoadTableIntoDictionary("convtables/forest2rockTOsnow2rock.txt");
                LoadTableIntoDictionary("convtables/forest2waterTOsnow2water.txt");
                LoadTableIntoDictionary("convtables/forestTOsnow.txt");
                LoadTableIntoDictionary("convtables/grassTOsnow.txt");
                LoadTableIntoDictionary("convtables/grass2water-darkTOsnow2water-dark.txt");
                LoadTableIntoDictionary("convtables/grass2water-lightTOsnow2water.txt");
            }
            catch (System.IO.IOException)
            {
                Log("\nERROR opening transition tables: skipping season change\n", true);
                landTileDictionary.Clear();
            }
            finally
            {
                Log("Done!\n", true);
            }
        }


        //---------------------------------


        public void ConvertMap()
        {
            Log("Loading map into memory.", true);
            tileMatrix.PreLoadMap(task_mapstatics, ProgressSetMax, ProgressSetVal, Log);
            Log("\nMap loaded!\n", true);

            int totBlocks = (width / 64) * (height / 64);
            ProgressSetMax(totBlocks);


            Log("Conversion started.", true);
            try
            {
                //WriteMapResolution();
                //-From my experience, we don't need this file, even if we create a facet UOP without this it will work fine.
                //-Moreover, we don't have the real name of this file, so we can't repack it. When i'll find out this name i'll uncomment the line (and fix the output name).

                //for (int blocksX = 0; blocksX < (tileMatrix.Width / 64); blocksX++)
                Parallel.For(0, (tileMatrix.Width / 64), blocksX =>
               {
                   for (int blocksY = 0; blocksY < (tileMatrix.Height / 64); blocksY++)
                   {
                       WriteMapBlock(blocksX, blocksY, ((blocksX * 64) + blocksY));
                       //tileMatrix.DisposeMapBlock(blocksX, blocksY); 
                       //   I can't dispose the map block because i may need a different block from the current when writing delimiters
                   }
               });
            }
            catch (Exception e) // (ThreadAbortException e)
            {
                Log(String.Format("\nConversion ERROR: ABORTING. Message: {0}", e.Message), true);
                SetItemsState(true);
                ProgressSetVal(0);
                return;
            }
            finally
            {
                Log("\nConversion ended successifully!", true);
            }
            //Saving memory
            if (task_dds)
                radarcol = null;
            tileMatrix.Dispose();
            GC.Collect();       //needed, otherwise it won't readily free the memory
            GC.WaitForPendingFinalizers();


            if (task_uop)
            {
                ProgressSetVal(0);
                ProgressSetMax(totBlocks);
                MythicPackage package = new MythicPackage(5);

                Log("\nStarting to pack the files into the .uop package... ", true);
                try
                {
                    for (int i = 0; i < totBlocks; i++)
                    {
                        string binFileName = binPath + string.Format("{0:D8}.bin", i);
                        package.AddFile(binFileName, string.Format("build/sectors/facet_0{0}/", mapIndex), CompressionFlag.Zlib);
                        ProgressIncrease();
                    }
                    package.Save(outPath + string.Format("facet{0}.uop", mapIndex));
                }
                catch (System.IO.IOException e)
                {
                    Log(String.Format("\nUop packing ERROR: skipping. Message: {0}", e.Message), true);
                    File.Delete(outPath + string.Format("facet{0}.uop", mapIndex));
                }
                finally
                {
                    Log("Done!", true);
                }
                //Saving memory
                for (int i = 0; i < package.Blocks.Count; i++)
                    package.RemoveBlock(i);
                package = null;
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                Log("\nRemoving .bin temporary files... ", true);
                try
                {
                    Directory.Delete(outPath + "build", true);
                }
                catch (System.IO.IOException e)
                {
                    Log(String.Format("\nRemoving .bin temporary files ERROR: skipping. Message: {0}", e.Message), true);
                }
                finally
                {
                    Log("Done!", true);
                }
            }


            if (task_dds)
            {
                Log("\nStarting to render the dds image... ", true);
                ProgressSetMarquee(true);
                try
                {
                    // Unlock the bits.
                    unsafe { bmpPtr = null; }
                    bmp.UnlockBits(bmpData);

                    /*
                    MemoryStream bmpStream = new MemoryStream();
                    bmp.Save(bmpStream, ImageFormat.Bmp);
                    byte[] bmpArray = bmpStream.ToArray();
                    */

                    ImageConverter converter = new ImageConverter();
                    byte[] bmpArray = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                    bmp.Dispose();
                    using (MagickImage bmpMagick = new MagickImage(bmpArray))
                    {
                        bmpMagick.Format = MagickFormat.Dds;
                        bmpMagick.Settings.SetDefine(MagickFormat.Dds, "compression", "dxt1");  // convert to a DXT1 DDS
                        using (FileStream fileS = File.Create(outPath + string.Format("facet0{0}.dds", mapIndex)))
                        {
                            bmpMagick.Write(fileS);
                        }
                    }

                    Log("Done!", true);

                    //Saving memory
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception e) //(System.IO.IOException e)
                {
                    Log(String.Format("\nDDS rendering ERROR: skipping. Message: {0}", e.Message), true);
                    File.Delete(outPath + string.Format("facet0{0}.dds", mapIndex));
                }
                ProgressSetMarquee(false);
            }


            if (!task_uop)
            {
                Log(string.Format("\nSuccess!\n" +
                                  "Generated files are raw .bin binary files, " +
                                  "you need to repack them using Mythic Package Editor.\n" +
                                  "Remember, you should use zlib compression and add the files " +
                                  "with the directory tree (\"build/sectors/facet_0{0}/\".\n" +
                                  "Then, you can pack it all into facet{0}.uop\n\n", mapIndex), true);
            }
            else
            {
                Log("\nSuccess!\n", true);
            }
            Success();
        }


        //----------


        private void WriteMapBlock(int blockX, int blockY, int fileNo)
        {
            using (FileStream mapBlock = new FileStream(binPath + string.Format("{0:D8}.bin", fileNo), FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(mapBlock))
                {
                    Log(string.Format("\nWriting 64x64 block {0} (blockX {1}, blockY {2}). Ingame coords X[{3}-{4}] Y[{5}-{6}]",
                                Path.GetFileName(mapBlock.Name), blockX, blockY, blockX * 64, (blockX * 64) + 63, blockY * 64, (blockY * 64) + 63));
                    ProgressIncrease();

                    //  WRITING HEADER
                    writer.Write((byte)mapIndex);         //BYTE  ->  FACETID
                    writer.Write((short)fileNo);          //WORD  ->  FILEID

                    // WRITING TILE INFO
                    for (int x = 0 + blockX * 64; x < blockX * 64 + 64; x++)
                    {
                        for (int y = 0 + blockY * 64; y < blockY * 64 + 64; y++)
                        {
                            WriteLandTile(writer, x, y);
                            WriteDelimiters(writer, blockX, blockY, x, y);
                            if (task_mapstatics == 0)
                                WriteStatics(writer, x, y);
                            else //if (task_mapstatics == 1)
                                writer.Write((byte)0);      //no statics in this tile
                            //else
                            //    return;

                            if (task_dds)
                                WriteBmpTile(x, y);

                            //tileMatrix.DisposeMapBlock(blockX, blockY);
                        }
                    }
                }
            }
        }

        private void WriteLandTile(BinaryWriter writer, int x, int y)
        {
            Tile tile = tileMatrix.GetLandTile(x, y);

            writer.Write(GetZ((sbyte)tile.Z));                         //BYTE  -> tile Z           
            writer.Write(GetLandTileID(tile.ID));                      //WORD -> LANDTILEgraphic
        }


        enum Delimiter
        {
            /*
              1      2     7
                ----------
                |        |
                |        |
              0 |  Tile  | 3
                |        |
                |        |
                ----------
              6      5     4
        */
            Left = 0,
            TopLeft = 1,
            Top = 2,
            TopRight = 7,
            Right = 3,
            BottomRight = 4,
            Bottom = 5,
            BottomLeft = 6,
            None = 255
        }

        private void WriteDelimiters(BinaryWriter writer, int blocksX, int blocksY, int x, int y)
        {
            //We need to calculate and write those values
            /* 
                -bBYTE  DELIMITERCount
                if(HIBYTE(bBYTE)>0){
                        for ( i = 0 ; i < bBYTE; ++i){          
                                -dBYTE  DIRECTION           Facet_SUB_SUB   -> what does "facet_sub_sub" mean? nevertheless i made it work lol
                                if(dBYTE <= 7){
                                        -BYTE   Z
                                        -DWORD  Graphic     *******-> pay attention: not WORD, as before, but DWORD
                                }
                        }
             */
            // the graphic dword in the delimiter zone appears to be the landtilegraphic of the adjoining tile

            Tile tile = tileMatrix.GetLandTile(x, y);
            int y_local = y - (blocksY * 64);
            int x_local = x - (blocksX * 64);
            int blocksX_max = (tileMatrix.Width / 64) - 1;    //minus 1 because the numeration here starts from zero, not from one
            int blocksY_max = (tileMatrix.Height / 64) - 1;
            //blocksX_min and blocksY_min are 0, so it isn't necessary to make other 2 variables

            //First if/else blocks: determine tile position in the block
            //Second, nested if/else clauses: determine block position into the map and, from that, the direction of the near tile
            if (y_local == 0 && x_local == 0)       //------TopLeft tile of the block
            {
                if (blocksX == 0 && blocksY == 0)   //TopLeft block of the map
                {
                    writer.Write((byte)0);
                }
                else if (blocksX == 0)              //(extreme)Left - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x, y - 1);
                    writer.Write((byte)Delimiter.Top);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
                else if (blocksY == 0)              //(extreme)Top - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x - 1, y);
                    writer.Write((byte)Delimiter.Left);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
                else                                //Internal - Block (map block not at map borders, so it confines other with 8 blocks)
                {
                    writer.Write((byte)3);

                    tile = tileMatrix.GetLandTile(x - 1, y);
                    writer.Write((byte)Delimiter.Left);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));

                    tile = tileMatrix.GetLandTile(x - 1, y - 1);
                    writer.Write((byte)Delimiter.TopLeft);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));

                    tile = tileMatrix.GetLandTile(x, y - 1);
                    writer.Write((byte)Delimiter.Top);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
            }
            else if (y_local == 63 && x_local == 0)             //------BottomLeft - Tile
            {
                if (blocksX == 0 && blocksY == blocksY_max)     //BottomLeft - Block
                {
                    writer.Write((byte)0);
                }
                else if (blocksX == 0)                          //(extreme)Left - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x, y + 1);
                    writer.Write((byte)Delimiter.Bottom);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
                else if (blocksY == blocksY_max)                //(extreme)Bottom - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x - 1, y);
                    writer.Write((byte)Delimiter.Left);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
                else                                            //Internal - Block
                {
                    writer.Write((byte)3);

                    tile = tileMatrix.GetLandTile(x, y - 1);
                    writer.Write((byte)Delimiter.Bottom);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));

                    tile = tileMatrix.GetLandTile(x - 1, y + 1);
                    writer.Write((byte)Delimiter.BottomLeft);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));

                    tile = tileMatrix.GetLandTile(x - 1, y);
                    writer.Write((byte)Delimiter.Left);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
            }
            else if (y_local == 0 && x_local == 63)                 //------TopRight - Tile
            {
                if (blocksX == blocksX_max && blocksY == 0)         //TopRight - Block
                {
                    writer.Write((byte)0);
                }
                else if (blocksX == blocksX_max)                    //(extreme)Top - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x, y - 1);
                    writer.Write((byte)Delimiter.Top);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
                else if (blocksY == 0)                              //(extreme)Right - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x + 1, y);
                    writer.Write((byte)Delimiter.Right);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
                else                                                //Internal - Block
                {
                    writer.Write((byte)3);

                    tile = tileMatrix.GetLandTile(x, y - 1);
                    writer.Write((byte)Delimiter.Top);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));

                    tile = tileMatrix.GetLandTile(x + 1, y - 1);
                    writer.Write((byte)Delimiter.TopRight);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));

                    tile = tileMatrix.GetLandTile(x + 1, y);
                    writer.Write((byte)Delimiter.Right);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
            }
            else if (y_local == 63 && x_local == 63)                    //------BottomRight - Tile
            {
                if (blocksX == blocksX_max && blocksY == blocksY_max)   //BottomRight - Block
                {
                    writer.Write((byte)0);
                }
                else if (blocksX == blocksX_max)                        //(extreme)Right - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x, y + 1);
                    writer.Write((byte)Delimiter.Bottom);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
                else if (blocksY == blocksY_max)                        //(extreme)Bottom - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x + 1, y);
                    writer.Write((byte)Delimiter.Right);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
                else                                                    //Internal - Block
                {
                    writer.Write((byte)3);

                    tile = tileMatrix.GetLandTile(x + 1, y);
                    writer.Write((byte)Delimiter.Right);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));

                    tile = tileMatrix.GetLandTile(x + 1, y + 1);
                    writer.Write((byte)Delimiter.BottomRight);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));

                    tile = tileMatrix.GetLandTile(x, y + 1);
                    writer.Write((byte)Delimiter.Bottom);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
            }
            else if (x_local == 0)                                      //------(extreme)Left - Tile
            {
                if (blocksX == 0)                                       //(extreme)Left - Block
                {
                    writer.Write((byte)0);
                }
                else                                                    //Internal - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x - 1, y);
                    writer.Write((byte)Delimiter.Left);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
            }
            else if (x_local == 63)                                     //------(extreme)Right - Tile
            {
                if (blocksX == blocksX_max)                             //(extreme)Right - Block
                {
                    writer.Write((byte)0);
                }
                else                                                    //Internal - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x + 1, y);
                    writer.Write((byte)Delimiter.Right);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
            }
            else if (y_local == 0)                                  //------(extreme)Top - Tile
            {
                if (blocksY == 0)                                   //(extreme)Top - Block
                {
                    writer.Write((byte)0);
                }
                else                                                //Internal - Block 
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x, y - 1);
                    writer.Write((byte)Delimiter.Top);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
            }
            else if (y_local == 63)                                 //------(extreme)Bottom - Tile
            {
                if (blocksY == blocksY_max)                         //(extreme)Bottom - Block
                {
                    writer.Write((byte)0);
                }
                else                                                //Internal - Block
                {
                    writer.Write((byte)1);
                    tile = tileMatrix.GetLandTile(x, y + 1);
                    writer.Write((byte)Delimiter.Bottom);
                    writer.Write(GetZ((sbyte)tile.Z));
                    writer.Write((UInt32)GetLandTileID(tile.ID));
                }
            }
            else
            {
                //In this case it is an internal tile, that is it isn't located into the block perimeter
                writer.Write((byte)0);
            }
        }


        private void WriteStatics(BinaryWriter writer, int x, int y)
        {
            HuedTile[] statics = tileMatrix.GetStaticTiles(x, y);
            if (statics != null && statics.Length > 0)
            {
                writer.Write((byte)statics.Length);                   //BYTE -> STATICScount
                for (int i = 0; i < statics.Length; i++)
                {
                    HuedTile hued = statics[i];
                    writer.Write((UInt32)(hued.ID - 16384));        //DWORD -> STATICgraphic (16384 is the offet dued to the presence in the art.mul of the map art tiles?)
                    writer.Write(GetZ((sbyte)hued.Z));              //BYTE  -> STATIC_z
                    writer.Write((UInt32)hued.Hue);                   //DWORD -> STATICcolor (hue)
                }
            }
            else
                writer.Write((byte)0);                                //no statics in this tile
            statics = null;
        }


        //------


        private void WriteBmpTile(int x, int y)
        {
            //I did a LOT of copy-pasting (and changing the syntax for my purpose) from the BMP and DDS generating code from Nibbio's MapConverter

            Tile land = tileMatrix.GetLandTile(x, y);
            HuedTile[] statics = tileMatrix.GetStaticTiles(x, y);
            HuedTile highestStatic;
            ushort color;

            if (task_mapstatics == 0 && statics.Length > 0)
            {
                highestStatic = statics[0];
                for (short i = 1; i < statics.Length; i++)
                {
                    if (statics[i].Z > highestStatic.Z)
                    {
                        highestStatic = statics[i];
                    }
                }

                if (land.Z > highestStatic.Z)
                    color = (ushort)radarcol[GetLandTileID(land.ID)];
                else if (highestStatic.Hue > 0)
                    color = (ushort)(Hues.GetHue(highestStatic.Hue).Colors[(radarcol[highestStatic.ID + 0x4000] >> 10) & 0x1F]);
                else
                    color = (ushort)radarcol[highestStatic.ID];
            }
            else
                color = (ushort)radarcol[GetLandTileID(land.ID)];

            // Format16bppRgb555 = 5 Pixel per channel -> 15 bit + 1 -> short, little endian format
            int position = (y * bmpData.Width + x) * 2;
            unsafe
            {
                bmpPtr[position] = (byte)(color & 0xFF);
                bmpPtr[position + 1] = (byte)(color >> 8);
            }
            statics.Initialize();
            statics = null;
        }


        //--------


        //probably the resolution file generated will have wrong format, but i don't care since this file doesn't appear to be needed, in fact i don't call this funcion
        private void WriteMapResolution()   // is it "data.map"?
        {
            FileStream mapBlock = new FileStream(outPath + "facet" + mapIndex.ToString() + "-MAPresolution.bin", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(mapBlock);
            writer.Write(tileMatrix.Height);
            writer.Write(tileMatrix.Width);
            writer.Close();
        }
    }
}

