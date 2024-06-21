

namespace CaRo_Game
{
    class clsVideo
    {
        //Quay lại trò chơi
        public int length;
        public Node[] ToaDo;
        public Player[] player;
        public clsVideo(clsBanCo cls)
        {
            length = 0;
            ToaDo = new Node[cls.Row * cls.Column];
            player = new Player[cls.Row * cls.Column];
        }

        public void Add(Node n, Player p)
        {
            ToaDo[length] = n;
            player[length] = p;
            length++;
        }
        public void Reset(clsBanCo cls)
        {
            length = 0;
            ToaDo = new Node[cls.Row * cls.Column];
            player = new Player[cls.Row * cls.Column];
        }
    }
}
