using System;
using System.IO;

/* This file (and other ones) was modified by Nolok.
 * I did two main fixes: changed the file loading code, in order to set a custom folder for finding the files;
 *  since with Stygian Abyss the art.mul has more "slots", i expanded the old ID limit (0x3FFF) in order to support
 *  higher IDs, which contain new items. Though not all the slots are usable with my fix, my suggestion is to
 *
 *  !!! use the lower free slots of the art.mul for custom graphics !!!
 *
 *  higher supported ID is now 0x7FFE (which is 0x3FFF*2, since i changed short to unsigned short).
*/

namespace Ultima
{
    public class TileMatrix
    {
        private HuedTile[][][][][] m_StaticTiles;
        private Tile[][][] m_LandTiles;

        private Tile[] m_InvalidLandBlock;
        private HuedTile[][][] m_EmptyStaticBlock;

        private FileStream m_Map;

        private FileStream m_Index;
        private BinaryReader m_IndexReader;

        private FileStream m_Statics;

        private int m_BlockWidth, m_BlockHeight;
        private int m_Width, m_Height;

        // Nolok
        public delegate void LogDelegate(string msg, bool alwaysshow = false);
        public delegate void ProgressBarValDelegate(int max);

        //private TileMatrixPatch m_Patch;

        /*public TileMatrixPatch Patch
		{
			get
			{
				return m_Patch;
			}
		}*/

        public int BlockWidth
        {
            get
            {
                return m_BlockWidth;
            }
        }

        public int BlockHeight
        {
            get
            {
                return m_BlockHeight;
            }
        }

        public int Width
        {
            get
            {
                return m_Width;
            }
        }

        public int Height
        {
            get
            {
                return m_Height;
            }
        }

        //manual input dir selection FIX
        public TileMatrix(string inpath, int fileIndex, int mapID, int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_BlockWidth = width >> 3;
            m_BlockHeight = height >> 3;

            if (fileIndex != 0x7F)
            {
                string mapPath = inpath + "map" + mapID + ".mul";
                m_Map = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                string indexPath = inpath + "staidx" + mapID + ".mul";
                m_Index = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                m_IndexReader = new BinaryReader(m_Index);

                string staticsPath = inpath + "statics" + mapID + ".mul";
                m_Statics = new FileStream(staticsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            m_EmptyStaticBlock = new HuedTile[8][][];

            for (int i = 0; i < 8; ++i)
            {
                m_EmptyStaticBlock[i] = new HuedTile[8][];

                for (int j = 0; j < 8; ++j)
                {
                    m_EmptyStaticBlock[i][j] = new HuedTile[0];
                }
            }

            m_InvalidLandBlock = new Tile[196];

            //m_LandTiles = new Tile[m_BlockWidth][][];
            //m_StaticTiles = new HuedTile[m_BlockWidth][][][][];

            //m_Patch = new TileMatrixPatch( this, mapID );

            /*for ( int i = 0; i < m_BlockWidth; ++i )
			{
				m_LandTiles[i] = new Tile[m_BlockHeight][];
				m_StaticTiles[i] = new Tile[m_BlockHeight][][][];
			}*/
        }

        public HuedTile[][][] EmptyStaticBlock
        {
            get
            {
                return m_EmptyStaticBlock;
            }
        }

        public void SetStaticBlock(int x, int y, HuedTile[][][] value)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight)
                return;

            if (m_StaticTiles[x] == null)
                m_StaticTiles[x] = new HuedTile[m_BlockHeight][][][];

            m_StaticTiles[x][y] = value;
        }

        public HuedTile[][][] GetStaticBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight || m_Statics == null || m_Index == null)
                return m_EmptyStaticBlock;

            //if (m_StaticTiles[x] == null)
            //    m_StaticTiles[x] = new HuedTile[m_BlockHeight][][][];

            HuedTile[][][] tiles = m_StaticTiles[x][y];

            //if (tiles == null)
            //    tiles = m_StaticTiles[x][y] = ReadStaticBlock(x, y);

            return tiles;
        }

        public HuedTile[] GetStaticTiles(int x, int y)
        {
            HuedTile[][][] tiles = GetStaticBlock(x >> 3, y >> 3);
            HuedTile[] ret = tiles[x & 0x7][y & 0x7];
            tiles = null;
            return ret;
        }

        public void SetLandBlock(int x, int y, Tile[] value)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight)
                return;

            if (m_LandTiles[x] == null)
                m_LandTiles[x] = new Tile[m_BlockHeight][];

            m_LandTiles[x][y] = value;
        }

        public Tile[] GetLandBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= m_BlockWidth || y >= m_BlockHeight || m_Map == null) return m_InvalidLandBlock;

            //if (m_LandTiles[x] == null)
            //    m_LandTiles[x] = new Tile[m_BlockHeight][];

            Tile[] tiles = m_LandTiles[x][y];

            //if (tiles == null)
            //    tiles = m_LandTiles[x][y] = ReadLandBlock(x, y);

            return tiles;
        }

        public Tile GetLandTile(int x, int y)
        {
            Tile[] tiles = GetLandBlock(x >> 3, y >> 3);
            Tile ret = tiles[((y & 0x7) << 3) + (x & 0x7)];
            //Saving memory (Nolok)
            tiles = null;
            return ret;
        }

        private static HuedTileList[][] m_Lists;

        private unsafe HuedTile[][][] ReadStaticBlock(int x, int y)
        {
            m_IndexReader.BaseStream.Seek(((x * m_BlockHeight) + y) * 12, SeekOrigin.Begin);

            int lookup = m_IndexReader.ReadInt32();
            int length = m_IndexReader.ReadInt32();

            if (lookup < 0 || length <= 0)
            {
                return m_EmptyStaticBlock;
            }
            else
            {
                int count = length / 7;

                m_Statics.Seek(lookup, SeekOrigin.Begin);

                StaticTile[] staTiles = new StaticTile[count];

                fixed (StaticTile* pTiles = staTiles)
                {
                    NativeMethods._lread(m_Statics.SafeFileHandle, pTiles, length);

                    if (m_Lists == null)
                    {
                        m_Lists = new HuedTileList[8][];

                        for (int i = 0; i < 8; ++i)
                        {
                            m_Lists[i] = new HuedTileList[8];

                            for (int j = 0; j < 8; ++j)
                                m_Lists[i][j] = new HuedTileList();
                        }
                    }

                    HuedTileList[][] lists = m_Lists;

                    StaticTile* pCur = pTiles, pEnd = pTiles + count;

                    while (pCur < pEnd)
                    {
                        ushort tmpID = 0x4000;
                        if ((pCur->m_ID + 0x4000) < 0xFFFF)
                            tmpID = (ushort)(pCur->m_ID + 0x4000);

                        //lists[pCur->m_X & 0x7][pCur->m_Y & 0x7].Add( (ushort)((pCur->m_ID & 0x3FFF) + 0x4000), pCur->m_Hue, pCur->m_Z );
                        lists[pCur->m_X & 0x7][pCur->m_Y & 0x7].Add(tmpID, pCur->m_Hue, pCur->m_Z);
                        ++pCur;
                    }

                    HuedTile[][][] tiles = new HuedTile[8][][];

                    for (int i = 0; i < 8; ++i)
                    {
                        tiles[i] = new HuedTile[8][];

                        for (int j = 0; j < 8; ++j)
                            tiles[i][j] = lists[i][j].ToArray();
                    }
                    //Saving memory (Nolok)
                    lists = null;
                    staTiles = null;
                    return tiles;
                }
            }
        }

        private unsafe Tile[] ReadLandBlock(int x, int y)
        {
            m_Map.Seek(((x * m_BlockHeight) + y) * 196 + 4, SeekOrigin.Begin);

            Tile[] tiles = new Tile[64];

            fixed (Tile* pTiles = tiles)
            {
                NativeMethods._lread(m_Map.SafeFileHandle, pTiles, 192);
            }

            return tiles;
        }

        public void Dispose()
        {
            if (m_Map != null)
                m_Map.Close();

            if (m_Statics != null)
                m_Statics.Close();

            if (m_IndexReader != null)
                m_IndexReader.Close();

            //Optimize memory usage
            m_LandTiles = null;
            m_StaticTiles = null;
        }

        //by Nolok, i don't really know if that's useful in term of memory usage
        public void DisposeMapBlock(int blockX, int blockY)
        {
            if (m_LandTiles != null)
            {
                if (m_LandTiles[blockX] != null)
                    m_LandTiles[blockX][blockY] = null;
            }

            if (m_StaticTiles != null)
            {
                if (m_StaticTiles[blockX] != null)
                    m_StaticTiles[blockX][blockY] = null;
            }
        }

        // Nolok: To prevent race conditions when doing the conversion in parallel mode, i need to pre-load the map.
        //  This because UltimaSDK loads the map block only when you are accessing a tile located into it.
        public void PreLoadMap( byte task_mapstatics, ProgressBarValDelegate ProgressSetMax, ProgressBarValDelegate ProgressSetVal, LogDelegate SendLog )
        {
            m_LandTiles = null;
            m_StaticTiles = null;
            m_LandTiles = new Tile[m_BlockWidth][][];
            m_StaticTiles = new HuedTile[m_BlockWidth][][][][];

            ProgressSetMax(400);
            int prog = 0, progMax = (m_BlockWidth * m_BlockHeight);
            int progVal = 0, progValOld = 0;

            for (int blockX = 0; blockX < m_BlockWidth; blockX++)
            {
                //if (m_LandTiles[blockX] == null)
                    m_LandTiles[blockX] = new Tile[m_BlockHeight][];
                if (task_mapstatics == 0) // && (m_StaticTiles[blocksX] == null)
                    m_StaticTiles[blockX] = new HuedTile[m_BlockHeight][][][];

                for (int blockY = 0; blockY < m_BlockHeight; blockY++)
                {
                    //if (m_LandTiles[blockX][blockY] == null)
                        m_LandTiles[blockX][blockY] = ReadLandBlock(blockX, blockY);
                    if (task_mapstatics == 0) // && (m_StaticTiles[blockX][blockY] == null)
                        m_StaticTiles[blockX][blockY] = ReadStaticBlock(blockX, blockY);

                    SendLog(string.Format("\nReading 8x8 block {0} (blockX {1}, blockY {2}). Ingame coords X[{3}-{4}] Y[{5}-{6}]",
                            prog, blockX, blockY, blockX * 64, (blockX * 64) + 63, blockY * 64, (blockY * 64) + 63));
                    prog++;
                    progVal = (prog*400)/progMax;
                    if (progVal > progValOld)
                        ProgressSetVal(progVal);
                    progValOld = progVal;
                }
            }
        }
    }

    [System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential, Pack=1 )]
	public struct StaticTile
	{
		public ushort m_ID;
		public byte m_X;
		public byte m_Y;
		public sbyte m_Z;
		public short m_Hue;
	}

	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential, Pack=1 )]
	public struct HuedTile
	{
		internal ushort m_ID;
		internal short m_Hue;
		internal sbyte m_Z;

		public int ID
		{
			get
			{
				return m_ID;
			}
		}

		public int Hue
		{
			get
			{
				return m_Hue;
			}
		}

		public int Z
		{
			get
			{
				return m_Z;
			}
			set
			{
				m_Z = (sbyte)value;
			}
		}

		public HuedTile( ushort id, short hue, sbyte z )
		{
			m_ID = id;
			m_Hue = hue;
			m_Z = z;
		}

		public void Set( ushort id, short hue, sbyte z )
		{
			m_ID = id;
			m_Hue = hue;
			m_Z = z;
		}
	}

	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential, Pack=1 )]
	public struct Tile : IComparable
	{
		internal ushort m_ID;
		internal sbyte m_Z;

		public int ID
		{
			get
			{
				return m_ID;
			}
		}

		public int Z
		{
			get
			{
				return m_Z;
			}
			set
			{
				m_Z = (sbyte)value;
			}
		}

		public bool Ignored
		{
			get
			{
				return ( m_ID == 2 || m_ID == 0x1DB || ( m_ID >= 0x1AE && m_ID <= 0x1B5 ) );
			}
		}

		public Tile( ushort id, sbyte z )
		{
			m_ID = id;
			m_Z = z;
		}

		public void Set( ushort id, sbyte z )
		{
			m_ID = id;
			m_Z = z;
		}

		public int CompareTo( object x )
		{
			if ( x == null )
				return 1;

			if ( !(x is Tile) )
				throw new ArgumentNullException();

			Tile a = (Tile)x;

			if ( m_Z > a.m_Z )
				return 1;
			else if ( a.m_Z > m_Z )
				return -1;

			//ItemData ourData = TileData.ItemTable[m_ID & 0x3FFF];
			//ItemData theirData = TileData.ItemTable[a.m_ID & 0x3FFF];
            ItemData ourData = TileData.ItemTable[m_ID & 0xFFDB];
            ItemData theirData = TileData.ItemTable[a.m_ID & 0xFFDB];

			if ( ourData.Height > theirData.Height )
				return 1;
			else if ( theirData.Height > ourData.Height )
				return -1;

			if ( ourData.Background && !theirData.Background )
				return -1;
			else if ( theirData.Background && !ourData.Background )
				return 1;

			return 0;
		}
	}
}