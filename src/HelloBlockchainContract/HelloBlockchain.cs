using Stratis.SmartContracts;

namespace HelloBlockchainContract
{
    public class HelloBlockchain : SmartContract
    {
        public HelloBlockchain(ISmartContractState state, string message)
            : base(state)
        {
            Requestor = Message.Sender;
            RequestMessage = message;
            State = (uint)StateType.Request;
        }

        public enum StateType : uint
        {
            Request = 0,
            Respond = 1
        }

        public uint State
        {
            get => PersistentState.GetUInt32(nameof(State));
            private set => PersistentState.SetUInt32(nameof(State), value);
        }

        public Address Requestor
        {
            get => PersistentState.GetAddress(nameof(Requestor));
            private set => PersistentState.SetAddress(nameof(Requestor), value);
        }

        public Address Responder
        {
            get => PersistentState.GetAddress(nameof(Responder));
            private set => PersistentState.SetAddress(nameof(Responder), value);
        }

        public string RequestMessage
        {
            get => PersistentState.GetString(nameof(RequestMessage));
            private set => PersistentState.SetString(nameof(RequestMessage), value);
        }

        public string ResponseMessage
        {
            get => PersistentState.GetString(nameof(ResponseMessage));
            private set => PersistentState.SetString(nameof(ResponseMessage), value);
        }

        public void SendRequest(string requestMessage)
        {
            Assert(Message.Sender == Requestor);

            RequestMessage = requestMessage;
            State = (uint)StateType.Request;
        }

        public void SendResponse(string responseMessage)
        {
            Responder = Message.Sender;

            ResponseMessage = responseMessage;
            State = (uint)StateType.Respond;
        }
    }
}
