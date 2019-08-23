using Stratis.SmartContracts;

public class Starter : SmartContract
{
    public Starter(ISmartContractState state)
        : base(state)
    {
    }

    /// <summary>
    /// Creates two contracts that can ping/pong back and forth between each other up to <see cref="maxPingPongTimes"/>.
    /// </summary>
    /// <param name="player1"></param>
    /// <param name="player2"></param>
    /// <param name="gameName"></param>
    /// <param name="maxPingPongTimes"></param>
    public void StartGame(Address player1, Address player2, string gameName)
    {
        var player1CreateResult = Create<Player>(0, new object[] { player1, player2, gameName });

        Assert(player1CreateResult.Success);

        var player2CreateResult = Create<Player>(0, new object[] { player2, player1, gameName });

        Assert(player2CreateResult.Success);

        Log(
            new GameCreated
            {
                Player1Contract = player1CreateResult.NewContractAddress,
                Player2Contract = player2CreateResult.NewContractAddress,
                GameName = gameName
            }
        );

    }

    public struct GameCreated
    {
        [Index]
        public Address Player1Contract;

        [Index]
        public Address Player2Contract;

        public string GameName;
    }
}