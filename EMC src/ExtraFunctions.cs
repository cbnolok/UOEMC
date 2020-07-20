using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Threading;
using Ultima;

//Function that populates the landtiles dictionary.
// It contains the pre-KR landtiles ID and the correspective SA landtiles ID. IDs has changed from the KR to SA, so old dictionary is unusable.
// Since SA, pre-KR IDs and post-KR (SA+) IDs are the same, so it won't need a dictionary file. Though, having a dictionary would be useful for converting a landtile to another,
// this can be an interesting way to make for example a fully snowy or a "winter edition" of the map.


namespace EMC
{
    class ExtraFunctions
    {
        delegate void DelegateString(string msg);
        delegate void StatusBarDelegate();
        delegate void SuccessDelegate();

        //-----------------//
        //--PATCH MUL MAP--//
        //-----------------//

        


        //-------------------------//
        //--GENERATE DICTIONARIES--//
        //-------------------------//

        public static void GenerateDictionary_CC_EC(MainForm form)
        {
            //Dictionary population is achieved by reading ALL the standard map tiles from SA Classic Client (MAPx.MUL, not facetx.uop), finding the land graphic and searching the correspective
            // at the same coordinates in the SA Enhanced Client uncompressed map block files, obtained by unpacking facetX.uop (here i will use only facet0)
            //This obviously works only if both maps are identical (and this is the case of the two CC and EC map files, at the same patch version), or differs only for the tiles you want
            // to convert (read above), in the case we have a snowy CC map and a normal EC map.
            //
            //Only differing tiles will be saved in the dictionary.
            //
            //It doesn't use map diffs, for now! 

            form.Invoke( new DelegateString( form.AppendToLog ), "Setting up for dictionary generation..\n" );

            SortedDictionary<short, short> LandtileDictionary = new SortedDictionary<short, short>();

            //directory of the unpacked facet0.uop (use Mythic Package Editor to do that), where the binaries are, not where the directory tree is (build/sectors/facet0/)
            string sa_dir = "C:\\Users\\claudio\\ultima\\unpack\\build\\sectors\\facet_00\\";
            FileStream sa_stream = null;
            BinaryReader sa = null;

            int prog = 0;
            short buffer, count;
            string sa_filename;
            TileMatrix tileMatrix = new TileMatrix("C:\\UO_cc\\", 0, 0, 7168, 4096);   //pre-KR map, ML dimensions //0,0 = fileindex, mapid
            Tile tile = tileMatrix.GetLandTile(0, 0);

            try
            {
                form.Invoke(new DelegateString(form.AppendToLog), "Dictionary population started!\n");
                //start of blocks iteration - cycle between SA map blocks
                for (short blocksX = 0; blocksX < (tileMatrix.Width / 64); blocksX++)
                {
                    for (short blocksY = 0; blocksY < (tileMatrix.Height / 64); blocksY++)
                    {

                        sa_filename = string.Format("{0:D8}.bin", prog);
                        form.Invoke(new DelegateString(form.AppendToLog), String.Format("Reading SA block {0}\n", sa_filename));
                        form.Invoke(new StatusBarDelegate(form.ProgressbarIncrease));
                        sa_stream = new FileStream(sa_dir + sa_filename, FileMode.Open);
                        sa = new BinaryReader(sa_stream);
                        sa.BaseStream.Seek(1 + 2, SeekOrigin.Begin);                                                //skip file header (BYTE facetid, WORD fileid)

                        for (int x = 0 + blocksX * 64; x < blocksX * 64 + 64; x++)                                  //coordinates of the tile in the map, not in the block
                        {
                            for (int y = 0 + blocksY * 64; y < blocksY * 64 + 64; y++)
                            {
                                tile = tileMatrix.GetLandTile(x, y);

                                // start of the tile definition
                                sa.BaseStream.Seek(1, SeekOrigin.Current);                                          //skip the z BYTE
                                buffer = (short)sa.ReadInt16();                                                     //read landtilegraphic WORD
                                if ((short)tile.ID != buffer)
                                {
                                    if (!LandtileDictionary.ContainsKey((short)tile.ID))
                                    {
                                        LandtileDictionary.Add((short)tile.ID, buffer);                                 //add a voice to the dictionary, if not present
                                        form.Invoke(new DelegateString(form.AppendToLog), string.Format("At {0},{1} found 0x{2:X} to convert in 0x{3:X}\n", x, y, tile.ID, buffer));
                                    }
                                }
                                        
                                buffer = (short)sa.ReadByte();                                                      //read delimitercount (BYTE)
                                if (buffer > 0)                                                                     // if there are delimiters, skip some things..
                                {
                                    for (count = 0; count < buffer; count++)
                                    {
                                        if ((short)sa.ReadByte() <= 7)                                              //ensures there is a valid direction (BYTE)
                                        {
                                            sa.BaseStream.Seek(1 + 4, SeekOrigin.Current);                          //skip z (BYTE) and landtilegraphic (DWORD)
                                        }
                                    }
                                }
                                buffer = (short)sa.ReadByte();                                                      //read staticscount (BYTE)
                                if (buffer > 0)
                                {
                                    for (count = 0; count < buffer; count++)
                                    {
                                        sa.BaseStream.Seek(4 + 1 + 4, SeekOrigin.Current);                          //skip static graphic (DWORD), z (BYTE), color (DWORD)
                                    }
                                }
                                //end of the tile definition
                            }
                        }

                        sa.Close();
                        prog++;
                    }
                }
                //end of blocks iteration
            }
            catch (ObjectDisposedException)
            {
                form.Invoke(new DelegateString(form.AppendToLog), "\nError reading the file, maybe it's corrupted or the format is not valid..");
            }


            // Now that the dictionary is populated, we can write it into a file
            //
            //DICTIONARY FILE STRUCTURE:
            //(from the head until EOF, for each id)
            //- WORD pre-KR landtilegraphic id
            //- WORD stygian abyss enhanced client landtilegraphic id
            form.Invoke(new DelegateString(form.AppendToLog), "Dictionary generated in local memory, writing into file..\n");

            FileStream dict_stream = new FileStream("c:\\out\\landtileDict.bin", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(dict_stream);

            foreach (var p in LandtileDictionary)
            {
                writer.Write(p.Key);
                writer.Write(p.Value);
            }

            writer.Close();
            form.Invoke(new DelegateString(form.AppendToLog), "Dictionary generation completed!\n");
            form.Invoke(new SuccessDelegate(form.Success));
        }


        public static void GenerateDictionary_CC_CC(MainForm form)
        {
            // Dictionary population is achieved by reading ALL the map tiles from a MAPx.MUL, finding the land graphic and searching the correspective
            // at the same coordinates in the second MAPx.MUL.
            //This obviously works only if both maps are identical, or differs only for the tiles you want
            // to convert (read above), in the case we have a snowy map and a normal map.
            //
            //Only differing tiles will be saved in the dictionary.

            form.Invoke(new DelegateString(form.AppendToLog), "Setting up for dictionary generation..\n");

            SortedDictionary<short, short> LandtileDictionary = new SortedDictionary<short, short>();

            //tiles from file 1 (e.g. normal map) will be substituted
            TileMatrix tileMatrix_1 = new TileMatrix("C:\\out\\1\\", 0, 0, 7168, 4096);   //pre-KR map, ML dimensions //0,0 = fileindex, mapid
            Tile tile_1 = tileMatrix_1.GetLandTile(0, 0);
            // with the same tile (if different) from file 2 (e.g. snowy map)
            TileMatrix tileMatrix_2 = new TileMatrix("C:\\out\\2\\", 0, 0, 7168, 4096);   //pre-KR map, ML dimensions //0,0 = fileindex, mapid
            Tile tile_2 = tileMatrix_2.GetLandTile(0, 0);

            try
            {
                form.Invoke(new DelegateString(form.AppendToLog), "Dictionary population started! Reading...\n");

                for (int x = 0; x < tileMatrix_1.Width; x++)
                {
                    for (int y = 0; y < tileMatrix_1.Height; y++)
                    {
                        tile_1 = tileMatrix_1.GetLandTile(x, y);
                        tile_2 = tileMatrix_2.GetLandTile(x, y);

                        if (tile_1.ID != tile_2.ID)
                        {
                            if (!LandtileDictionary.ContainsKey((short)tile_1.ID))
                            {
                                LandtileDictionary.Add((short)tile_1.ID, (short)tile_2.ID); //add a voice to the dictionary, if not present
                                form.Invoke(new DelegateString(form.AppendToLog), string.Format("At {0},{1} found 0x{2:X} to convert in 0x{3:X}\n", x, y, tile_1.ID, tile_2.ID) );
                            }
                        }
                    }
                    form.Invoke(new StatusBarDelegate(form.ProgressbarIncrease));
                }
            }
            catch (ObjectDisposedException)
            {
                form.Invoke(new DelegateString(form.AppendToLog), "\nError reading the file, maybe it's corrupted or the format is not valid..");
            }


            // Now that the dictionary is populated, we can write it into a file
            //
            //DICTIONARY FILE STRUCTURE:
            //(from the head until EOF, for each id)
            //- WORD id to be substituted
            //- WORD substitute previous id with this one
            form.Invoke(new DelegateString(form.AppendToLog), "Dictionary generated in local memory, writing into file..\n");

            FileStream dict_stream = new FileStream("c:\\out\\landtileDict.bin", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(dict_stream);

            foreach (var p in LandtileDictionary)
            {
                writer.Write(p.Key);
                writer.Write(p.Value);
                //form.Invoke(new DelegateString(form.AppendToLog), string.Format("Writing 0x{0:X} to 0x{1:X}\n", p.Key, p.Value));
            }

            writer.Close();
            form.Invoke(new DelegateString(form.AppendToLog), "Dictionary generation completed!\n");
            form.Invoke(new SuccessDelegate(form.Success));
        }
    
    }
}
