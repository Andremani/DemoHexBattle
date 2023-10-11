using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wunderwunsch.HexMapLibrary;

namespace Andremani.DemoHexBattle
{
    public class Formation
    {
        private MapSizeData mapSizeData;

        public Formation(MapSizeData mapSizeData)
        {
            this.mapSizeData = mapSizeData;
        }

        public Vector2Int GetPositionForLeftStack(int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector2Int(mapSizeData.offsetTileMaxValX - 1, GetOffsetTileMiddleValZ());
                case 1:
                    return new Vector2Int(mapSizeData.offsetTileMaxValX, GetOffsetTileMiddleValZ() - 1);
                case 2:
                    return new Vector2Int(mapSizeData.offsetTileMaxValX, GetOffsetTileMiddleValZ() + 1);
                case 3:
                    return new Vector2Int(mapSizeData.offsetTileMaxValX, GetOffsetTileMiddleValZ());
                case 4:
                    return new Vector2Int(mapSizeData.offsetTileMaxValX, mapSizeData.offsetTileMinValZ + 1);
                case 5:
                    return new Vector2Int(mapSizeData.offsetTileMaxValX, mapSizeData.offsetTileMaxValZ - 1);
                default: return new Vector2Int(mapSizeData.offsetTileMaxValX, mapSizeData.offsetTileMinValZ);
            }
        }

        public Vector2Int GetPositionForRightStack(int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector2Int(mapSizeData.offsetTileMinValX, 1);
                case 1:
                    return new Vector2Int(mapSizeData.offsetTileMinValX, 3);
                case 2:
                    return new Vector2Int(mapSizeData.offsetTileMinValX, 5);
                case 3:
                    return new Vector2Int(mapSizeData.offsetTileMinValX, 7);
                case 4:
                    return new Vector2Int(mapSizeData.offsetTileMinValX, 9);
                default: return new Vector2Int(mapSizeData.offsetTileMinValX, mapSizeData.offsetTileMinValZ);
            }
        }

        private int GetOffsetTileMiddleValZ()
        {
            return (mapSizeData.offsetTileMinValZ + mapSizeData.offsetTileMaxValZ) / 2;
        }
    }
}