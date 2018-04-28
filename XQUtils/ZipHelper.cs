using SevenZip;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using XiangQiu.Foundation.Core.XQLogger;

namespace XiangQiu.Foundation.Core.XQUtils
{
    public class ZipHelper
    {
        #region 压缩，解压缩
        /// <summary>
        /// 使用Lzma算法压缩至字节流
        /// </summary>
        /// <typeparam name="Data"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Encode<Data>(Data data)
        {
            if (data == null)
                return null;
            return Encodes<Data>(new List<Data> { data });
        }
        /// <summary>
        /// 使用Lzma算法压缩集合至字节流
        /// </summary>
        /// <typeparam name="Data"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static byte[] Encodes<Data>(List<Data> datas)
        {
            if (datas == null || datas.Count == 0)
                return null;
            byte[] buffers = null;
            using (MemoryStream output = new MemoryStream())
            {
                var encoder = new LzmaEncodeStream(output);
                using (var inputSample = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    datas.ForEach(D =>
                    {
                        try
                        {
                            formatter.Serialize(inputSample, D);
                        }
                        catch (SerializationException ex)
                        {
                            Logger.GetInstance().Write("对象" + D + "未能序列化！");
                        }
                    }
                    );
                    inputSample.Position = 0;
                    int bufSize = 24576, count;
                    byte[] buf = new byte[bufSize];
                    while ((count = inputSample.Read(buf, 0, bufSize)) > 0)
                    {
                        encoder.Write(buf, 0, count);
                    }
                }
                encoder.Close();
                buffers = new byte[output.Length];
                output.Position = 0;
                output.Read(buffers, 0, buffers.Length);
            }
            return buffers;
        }
        /// <summary>
        /// 使用Lzma算法从字节流解压缩为数据对象
        /// </summary>
        /// <typeparam name="Data"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Data Decode<Data>(byte[] buffer) where Data : class
        {
            if (buffer == null || buffer.Length == 0)
                return null;
            List<Data> lst = Decodes<Data>(buffer);
            if (lst.Count != 0)
                return lst[0];
            return null;
        }
        /// <summary>
        /// 使用Lzma算法从字节流解压缩为集合
        /// </summary>
        /// <typeparam name="Data"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static List<Data> Decodes<Data>(byte[] buffer) where Data : class
        {
            if (buffer == null || buffer.Length == 0)
                return null;
            List<Data> lstData = new List<Data>();
            using (var input = new MemoryStream())
            {
                input.Write(buffer, 0, buffer.Length);
                input.Position = 0;
                var decoder = new LzmaDecodeStream(input);
                using (MemoryStream output = new MemoryStream())
                {
                    int bufSize = 24576, count;
                    byte[] buf = new byte[bufSize];
                    while ((count = decoder.Read(buf, 0, bufSize)) > 0)
                    {
                        output.Write(buf, 0, count);
                    }
                    output.Position = 0;
                    BinaryFormatter formatter = new BinaryFormatter();
                    while (true)
                    {
                        if (output.Position == output.Length)
                            break;
                        Data data = null;
                        try
                        {
                            data = formatter.Deserialize(output) as Data;
                        }
                        catch (SerializationException ex)
                        {
                            Logger.GetInstance().Write("反序列化失败！");
                        }
                        if (data == null)
                            break;
                        lstData.Add(data);
                    }
                }
                decoder.Close();
            }
            return lstData;
        }
        #endregion
    }
}
