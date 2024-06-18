﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace IDZBase.Core.GameTemplates.Coloring
{
    public class SplitTexture
    {
        private readonly int[] _textureArray;
        private readonly uint[] _visitedBitmask;
        private readonly int _textureWidth;
        private readonly int _textureHeight;
        private const uint MaskOne = 0x80000000;

        public SplitTexture(Texture2D texture2D, int threshold = 254)
        {
            _textureWidth = texture2D.width;
            _textureHeight = texture2D.height;

            var colors = texture2D.GetPixels32();
            
            _textureArray = new int[colors.Length];

            for (var i = 0; i < colors.Length; i++)
            {
                _textureArray[i] = colors[i].a > threshold ? 1 : 0;
            }

            _visitedBitmask = new uint[(_textureWidth * _textureHeight + 31) / 32];
        }
        
        public SplitTexture(Texture2D texture2D, out List<IEnumerable<int>> result, int threshold = 254)
            : this(texture2D, threshold)
        {
            result = AutoSplit();
        }
        
        private List<IEnumerable<int>> AutoSplit()
        {
            var index = 0;
            var result = new List<IEnumerable<int>>();

            while (index < _textureArray.Length)
            {
                if (!IsVisited(index) && IsValid(index))
                {
                    var whitePixels = Split(index % _textureWidth, index / _textureHeight).ToList();
                    if (whitePixels.Count > 1) result.Add(whitePixels);
                }

                index++;
            }

            return result;
        }
        
        public async Task<List<IEnumerable<int>>> AutoSplitAsync()
        {
            var index = 0;
            var result = new List<IEnumerable<int>>();

            while (index < _textureArray.Length)
            {
                if (!IsVisited(index) && IsValid(index))
                {
                    var whitePixels = Split(index % _textureWidth, index / _textureHeight).ToList();
                    if (whitePixels.Count > 1) result.Add(whitePixels);
                    await Task.Yield();
                }

                index++;
                
            }

            return result;
        }
        
        public IEnumerable<int> Split(int startX, int startY)
        {
            var pixels = new List<int>();
            var queue = new Queue<int>();
            _visitedBitmask[GetBitmaskIndex(startX, startY)] |= GetBitmaskMask(startX, startY);
            var startIndex = GetIndex(startX, startY);
            queue.Enqueue(startIndex);

            while (queue.Count > 0)
            {
                var currentIndex = queue.Dequeue();
                pixels.Add(currentIndex);

                var neighbors = GetNeighbors(currentIndex);
                foreach (var neighborIndex in neighbors)
                {
                    if (!IsValid(neighborIndex) || IsVisited(neighborIndex)) continue;
                    queue.Enqueue(neighborIndex);
                    SetVisited(neighborIndex);
                }
            }

            return pixels;
        }
        
        private int GetIndex(int x, int y)
        {
            return y * _textureWidth + x;
        }

        private int GetBitmaskIndex(int x, int y)
        {
            return (y * _textureWidth + x) / 32;
        }

        private uint GetBitmaskMask(int x, int y)
        {
            return MaskOne >> (y * _textureWidth + x) % 32;
        }

        private IEnumerable<int> GetNeighbors(int index)
        {
            var x = index % _textureWidth;
            var y = index / _textureHeight;
            
            var neighbors = new int[4];
            if (x > 0) neighbors[0] = index - 1;
            if (x < _textureWidth - 1) neighbors[1] = index + 1;
            if (y > 0) neighbors[2] = index - _textureWidth;
            if (y < _textureHeight - 1) neighbors[3] = index + _textureWidth;
            return neighbors;
        }

        private bool IsValid(int index)
        {
            return _textureArray[index] == 1;
        }

        private bool IsVisited(int index)
        {
            var bitmask = _visitedBitmask[GetBitmaskIndex(index % _textureWidth, index / _textureHeight)];
            var mask = GetBitmaskMask(index % _textureWidth, index / _textureHeight);
            return (bitmask & mask) != 0;
        }

        private void SetVisited(int index)
        {
            var bitmaskIndex = GetBitmaskIndex(index % _textureWidth, index / _textureHeight);
            var bitmaskMask = GetBitmaskMask(index % _textureWidth, index / _textureHeight);
            _visitedBitmask[bitmaskIndex] |= bitmaskMask;
        }
    }
}