namespace CNHashiWpf.Models.V2
{
    public class V2Island
    {
        public int X { get; }
        public int Y { get; }
        public int NType { get; set; }
        public int ICount { get; set; }

        public V2Island(int x, int y)
        {
            X = x;
            Y = y;
            NType = 0;
            ICount = 0;
        }

        public void MakeIsland(int thickness)
        {
            NType = 1;
            ICount = thickness;
        }

        public void MakeBridge(int thickness, bool isHorizontal)
        {
            NType = isHorizontal ? -thickness : -thickness - 2;
        }
    }
}
