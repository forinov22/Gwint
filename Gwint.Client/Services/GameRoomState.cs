using Gwint.Contracts;

namespace Gwint.Client.Services
{
    public class GameRoomState
    {
        public GameRoomDto? CurrentRoom { get; private set; }

        public void SetRoom(GameRoomDto room)
        {
            CurrentRoom = room;
        }

        public void ClearRoom()
        {
            CurrentRoom = null;
        }
    }
}
