using Stratis.Bitcoin.P2P.Peer;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Ticketbooth.Api.Tests.Fakes
{
    public class FakeNetworkPeerCollection : IReadOnlyNetworkPeerCollection
    {
        private readonly IList<INetworkPeer> _networkPeers;

        public FakeNetworkPeerCollection()
        {
            _networkPeers = new List<INetworkPeer>();
        }

        public void Add(INetworkPeer networkPeer)
        {
            _networkPeers.Add(networkPeer);
        }

        public INetworkPeer FindByEndpoint(IPEndPoint endpoint)
        {
            return null;
        }

        public List<INetworkPeer> FindByIp(IPAddress ip)
        {
            return null;
        }

        public INetworkPeer FindLocal()
        {
            return null;
        }

        public IEnumerator<INetworkPeer> GetEnumerator()
        {
            return _networkPeers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _networkPeers.GetEnumerator();
        }
    }
}
