﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleTest
{

    /// <summary>
    /// SSTMap  - основной класс программы, оперирующий данными
    /// </summary>
    public class SSTMap
    {
        /// <summary>
        /// Ширина сетки sst.
        /// </summary>
        public readonly int width = 32768;
        /// <summary>
        /// Высота сетки sst.
        /// </summary>
        public readonly int length = 16384;
        /// <summary>
        /// Пропорция, которая используется в методе GetData.
        /// </summary>
        public readonly int Proportion;
        /// <summary>
        /// Массив байтов, содержащий данные о температуре.
        /// </summary>
        public byte[,] Bytes { get; set; }
        /// <summary>
        /// Входное изображение карты.
        /// </summary>
        public Bitmap Map { get; set; }
        /// <summary>
        /// Путь для изображения, которые будет отображать результат.
        /// </summary>
        public string MapStt { get; set; }
        /// <summary>
        /// Путь к файлу sst с данными о температуре.
        /// </summary>
        public string sstFilePath { get; set; }
        public SSTMap(string mapImg, string sst, string resultImg)
        {
            Map = new Bitmap(mapImg);
            MapStt = resultImg;
            Proportion = width / Map.Width;
            Bytes = new byte[Map.Height, Map.Width];
            sstFilePath = sst;
            GetData();
        }
        /// <summary>
        /// GetData - метод, получающий данные о воде и преобразующий их для отображению на результирующем снимке.
        /// Преображение по принципу отношения( например ширина сетки(32768) деленная на ширину изображени карты, т.е. сколько
        /// байтов в сетке sst отвечают за один пиксель.
        /// </summary>
        private void GetData()
        {
            using (var reader = new BinaryReader(File.Open(sstFilePath, FileMode.Open)))
            {
                byte[,] tempStore = new byte[length, width];
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        tempStore[i, j] = reader.ReadByte();
                    }
                }
                
                for (int i = 0; i < Map.Height; i++)
                {
                    for (int j = 0; j < Map.Width; j++)
                    {
                        Bytes[i, j] = tempStore[i * Proportion, j * Proportion];
                    }
                }

            }

        }
    }
    

        class Program
        {
        /// <summary>
        /// Метод-иструкция по помощи и для ввода данных.
        /// </summary>
        public static void Help()
        {
            Console.WriteLine("Программа должна получать на вход три параметра:\n" +
    "1.Путь к файлу с базовой картой\n" + "2.Путь к файлу с данными о температуре\n" +
    "3.Путь к файлу с результатом\n" + "Пример:> sst - image.exe  map.jpg  sst.bin  sst - map.jpgч\n" +
    "При вызове программы с параметром --h или --help вы получите информацию о использовании.");
        }
        /// <summary>
        /// Метод, проверющий входящие параметры.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool ChechAgrs(string[] args)
        {
            if (args == null)
            {
                return false;
            }
            else if(args[0].Contains(".jpg") && args[1].Contains(".bin") && args[2].Contains(".jpg"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        static void Main(string[] args)
            {
            string mapFileImage = string.Empty;
            string sstFile = string.Empty;
            string mapResultFile = string.Empty;
            bool isChecked = ChechAgrs(args);

            if (args.Count() == 3 && isChecked == true)
            {
                try
                {
                    mapFileImage = args[0];
                    sstFile = args[1];
                    mapResultFile = args[2];
                }
                catch(Exception ex)
                {
                    Help();
                }
            }
            else if(args.Count() > 3 && isChecked == true)
            {
                mapFileImage = args[0];
                sstFile = args[1];
                mapResultFile = args[2];
                for (int i = 0; i < args.Count(); i++)
                {
                    if(args[i] == "--help" || args[i] == "--h")
                    {
                        Help();
                    } 
                }
            }

            if (isChecked == true)
            {
                SSTMap map = new SSTMap(mapFileImage, sstFile, mapResultFile);

                for (int i = 0; i < map.Map.Height; i++)
                {
                    for (int j = 0; j < map.Map.Width; j++)
                    {
                        if (map.Bytes[i, j] >= 30 && map.Bytes[i, j] <= 40)
                        {
                            map.Map.SetPixel(j, map.Map.Height - i, Color.Blue);
                        }
                        else if (map.Bytes[i, j] >= 41 && map.Bytes[i, j] <= 50)
                        {
                            map.Map.SetPixel(j, map.Map.Height - i, Color.LightBlue);
                        }
                        else if (map.Bytes[i, j] >= 51 && map.Bytes[i, j] <= 60)
                        {
                            map.Map.SetPixel(j, map.Map.Height - i, Color.Green);
                        }
                        else if (map.Bytes[i, j] >= 61 && map.Bytes[i, j] <= 70)
                        {
                            map.Map.SetPixel(j, map.Map.Height - i, Color.Yellow);
                        }
                        else if (map.Bytes[i, j] >= 71 && map.Bytes[i, j] <= 80)
                        {
                            map.Map.SetPixel(j, map.Map.Height - i, Color.Orange);
                        }
                    }
                }

                Image img = map.Map;
                img.Save(map.MapStt);
                Console.ReadKey();
            }
            else
            {
                Help();
                Console.ReadKey();
            }
            }
        }
    }
