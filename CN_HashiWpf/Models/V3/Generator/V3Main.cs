namespace CNHashiWpf.Models.V3.Generator
{
    public class V3Main
    {
        public V3Main(int difficulty = -1, int amountNodes = 10, int width = 0, int length = 0, int alpha = 0, int beta = 0)
        {
            var rdm = new Random();
            if (difficulty >= 0)
            {
                if (difficulty == 0)
                {
                    var sizeLength = rdm.Next(5, 10);
                    var sizeWidth = rdm.Next(5, 10);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 4.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 0, 0, false);
                }
                if (difficulty == 1)
                {
                    var sizeLength = rdm.Next(14, 16);
                    var sizeWidth = rdm.Next(14, 16);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 4.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 1, 0, false);
                }
                if (difficulty == 2)
                {
                    var sizeLength = rdm.Next(10, 16);
                    var sizeWidth = rdm.Next(10, 16);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 2, 0, false);
                }
                if (difficulty == 3)
                {
                    var sizeLength = rdm.Next(11, 18);
                    var sizeWidth = rdm.Next(11, 18);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 3, 0, false);
                }
                if (difficulty == 4)
                {
                    var sizeLength = rdm.Next(10, 18);
                    var sizeWidth = rdm.Next(10, 18);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 4, 0, false);
                }
                if (difficulty == 5)
                {
                    var sizeLength = rdm.Next(13, 18);
                    var sizeWidth = rdm.Next(13, 18);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 5, 0, false);
                }
                if (difficulty == 6)
                {
                    var sizeLength = rdm.Next(15, 20);
                    var sizeWidth = rdm.Next(15, 20);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 6, 0, false);
                }
                if (difficulty == 7)
                {
                    var sizeLength = rdm.Next(14, 20);
                    var sizeWidth = rdm.Next(14, 20);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 7, 0, false);
                }
                if (difficulty == 8)
                {
                    var sizeLength = rdm.Next(16, 31);
                    var sizeWidth = rdm.Next(16, 31);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 8, 0, false);
                }
                if (difficulty == 9)
                {
                    var sizeLength = rdm.Next(20, 31);
                    var sizeWidth = rdm.Next(20, 31);
                    var n = (int)Math.Round(sizeWidth * sizeLength / 3.0);
                    var gen = new V3Generator(n, sizeLength, sizeWidth, 9, 0, false);
                }
            }
            else
            {
                var gen = new V3Generator(amountNodes, length, width, alpha, beta, true);
            }
        }
    }
}
