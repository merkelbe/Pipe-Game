﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PipeGame
{
    static class m
    {
        internal static Random Random = new Random();

        internal static int RandomIndex(List<int> _cutoffList)
        {
            int index = 0;
            int rando = Random.Next(0, _cutoffList[_cutoffList.Count - 1]);
            while (rando >= _cutoffList[index])
            {
                index++;
            }
            return index;
        }

        internal static List<T> GetListOfNElements<T>(T _element, int _count)
        {
            List<T> retList = new List<T>();
            for (int i = 0; i < _count; ++i)
            {
                retList.Add(_element);
            }
            return retList;
        }

        internal static List<T> GetIncList<T>(T _startValue, T _incrementor, int _count)
        {
            List<T> retList = new List<T>();
            T _currentValue = _startValue;
            for (int i = 0; i < _count; ++i)
            {
                retList.Add(_currentValue);
                _currentValue = (dynamic)_currentValue + (dynamic)_incrementor;
            }
            return retList;
        }

        internal static T RandomObject<T>(List<T> _objects)
        {
            return _objects[RandomIndex(GetIncList(1, 1, _objects.Count))];
        }

    }
}
