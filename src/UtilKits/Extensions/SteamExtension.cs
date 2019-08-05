using System.IO;
using System.Text;

namespace UtilKits.Extensions
{
    public static class SteamExtension
    {
        /// <summary>
        /// Reads all bytes.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
		/// 取得編碼的方式
		/// 使用前 Application 需先執行下列程式，註冊編碼分頁
		/// Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		/// </summary>
		/// <param name="bom">file bytes</param>
		/// <returns></returns>
		public static Encoding GetEncoding(this byte[] bom)
        {
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            if (IsBig5Encoding(bom)) return Encoding.GetEncoding("big5");
            return Encoding.ASCII;
        }

        /// <summary>
        /// 取得是否為BIG5編碼的方式
        /// </summary>
        /// <param name="bytes">file bytes.</param>
        /// <returns>
        ///   <c>true</c> 如果編碼為BIG5, 此外為 <c>false</c>.
        /// </returns>
        public static bool IsBig5Encoding(this byte[] bytes)
        {
            Encoding big5 = Encoding.GetEncoding("big5");

            return bytes.Length == big5.GetByteCount(big5.GetString(bytes));
        }
    }

}
