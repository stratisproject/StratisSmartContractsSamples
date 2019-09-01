namespace NonFungibleToken
{
    using Stratis.SmartContracts;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface INonFungibleTokenReceiver
    {
        /**
         * @dev Handle the receipt of a NFT. The ERC721 smart contract calls this function on the
         * recipient after a `transfer`. This function MAY throw to revert and reject the transfer. Return
         * of other than the magic value MUST result in the transaction being reverted.
         * Returns `bytes4(keccak256("onERC721Received(address,address,uint256,bytes)"))` unless throwing.
         * @notice The contract address is always the message sender. A wallet/broker/auction application
         * MUST implement the wallet interface if it will accept safe transfers.
         * @param operator The address which called `safeTransferFrom` function.
         * @param from The address which previously owned the token.
         * @param tokenId The NFT identifier which is being transferred.
         * @param data Additional data with no specified format.
         * @return Returns `bytes4(keccak256("onERC721Received(address,address,uint256,bytes)"))`.
         */        
        byte[] onERC721Received(
          Address operatorAddress,
          Address fromAddress,
          ulong tokenId,
          byte[] data
        );
    }
}
