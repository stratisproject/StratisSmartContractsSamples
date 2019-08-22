using Stratis.SmartContracts;

public class Player : SmartContract
{
    public Player(ISmartContractState state, string pingPongGameName)
        : base(state)
    {
        GameStarter = Message.Sender;
        PingPongGameName = pingPongGameName;
        State = (uint)StateType.PingPongPlayerCreated;
    }

    public enum StateType : uint
    {
        PingPongPlayerCreated = 0,
        PingPonging = 1,
        GameFinished = 2
    }

    public uint State
    {
        get => PersistentState.GetUInt32(nameof(State));
        private set => PersistentState.SetUInt32(nameof(State), value);
    }

    public Address GameStarter
    {
        get => PersistentState.GetAddress(nameof(GameStarter));
        private set => PersistentState.SetAddress(nameof(GameStarter), value);
    }

    public string PingPongGameName
    {
        get => PersistentState.GetString(nameof(PingPongGameName));
        private set => PersistentState.SetString(nameof(PingPongGameName), value);
    }

    public void Ping(uint currentPingPongTimes)
    {
        Assert(Message.Sender == GameStarter);

        currentPingPongTimes = currentPingPongTimes - 1;

        if (currentPingPongTimes > 0)
        {
            State = (uint)StateType.PingPonging;
            var callResult = Call(GameStarter, 0, nameof(Starter.Pong), new object[] { currentPingPongTimes });
            Assert(callResult.Success);
        }
        else
        {
            State = (uint)StateType.GameFinished;
            var callResult = Call(GameStarter, 0, nameof(Starter.FinishGame));
            Assert(callResult.Success);
        }
    }

    public void FinishGame()
    {
        Assert(Message.Sender == GameStarter);
        State = (uint)StateType.GameFinished;
    }
}