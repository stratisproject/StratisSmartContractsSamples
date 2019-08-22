using Stratis.SmartContracts;

public class Starter : SmartContract
{
    public Starter(ISmartContractState state, string gameName)
        : base(state)
    {
        PingPongGameName = gameName;
        GameStarter = Message.Sender;

        var createResult = Create<Player>(0, new object[] { gameName });

        Assert(createResult.Success);

        GamePlayer = createResult.NewContractAddress;

        State = (uint)StateType.GameProvisioned;
    }

    public enum StateType : uint
    {
        GameProvisioned = 0,
        PingPonging = 1,
        GameFinished = 2
    }

    public uint State
    {
        get => PersistentState.GetUInt32(nameof(State));
        private set => PersistentState.SetUInt32(nameof(State), value);
    }

    public string PingPongGameName
    {
        get => PersistentState.GetString(nameof(PingPongGameName));
        private set => PersistentState.SetString(nameof(PingPongGameName), value);
    }

    public Address GameStarter
    {
        get => PersistentState.GetAddress(nameof(GameStarter));
        private set => PersistentState.SetAddress(nameof(GameStarter), value);
    }

    public Address GamePlayer
    {
        get => PersistentState.GetAddress(nameof(GamePlayer));
        private set => PersistentState.SetAddress(nameof(GamePlayer), value);
    }

    public int PingPongTimes
    {
        get => PersistentState.GetInt32(nameof(PingPongTimes));
        private set => PersistentState.SetInt32(nameof(PingPongTimes), value);
    }

    public void StartPingPong(int pingPongTimes)
    {
        Assert(Message.Sender == GameStarter);

        PingPongTimes = pingPongTimes;
        State = (uint)StateType.PingPonging;

        var callResult = Call(GamePlayer, 0, nameof(Player.Ping), new object[] { pingPongTimes });

        Assert(callResult.Success);
    }

    public void Pong(int currentPingPongTimes)
    {
        Assert(Message.Sender == GamePlayer);

        currentPingPongTimes = currentPingPongTimes - 1;

        if (currentPingPongTimes > 0)
        {
            State = (uint)StateType.PingPonging;
            var callResult = Call(GamePlayer, 0, nameof(Player.Ping), new object[] { currentPingPongTimes });
            Assert(callResult.Success);
        }
        else
        {
            State = (uint)StateType.GameFinished;
            var callResult = Call(GamePlayer, 0, nameof(Player.FinishGame));
            Assert(callResult.Success);
        }
    }

    public void FinishGame()
    {
        Assert(Message.Sender == GamePlayer);
        State = (uint)StateType.GameFinished;
    }
}