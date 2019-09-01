using System;
using System.Collections.Generic;
using System.Text;

namespace NonFungibleToken
{
    public interface IERC165
    {
        /**
         * @dev Checks if the smart contract includes a specific interface.
         * @notice This function uses less than 30,000 gas.
         * @param interfaceID The interface identifier, as specified in ERC-165.
         * @return True if interfaceID is supported, false otherwise.
         */
        bool SupportsInterface(uint interfaceID);
    }
}
