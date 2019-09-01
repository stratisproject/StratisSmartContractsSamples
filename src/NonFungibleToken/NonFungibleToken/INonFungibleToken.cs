namespace NonFungibleToken
{
    using Stratis.SmartContracts;

    public interface INonFungibleToken : IERC165 , INonFungibleTokenReceiver
    {
        /**
      * @dev Emits when ownership of any NFT changes by any mechanism. This event emits when NFTs are
      * created (`from` == 0) and destroyed (`to` == 0). Exception: during contract creation, any
      * number of NFTs may be created and assigned without emitting Transfer. At the time of any
      * transfer, the approved Address for that NFT (if any) is reset to none.
      */
        /*LogTransfer(
          Address from,
          Address to,
          ulong tokenId
        );*/

        /**
         * @dev This emits when the approved Address for an NFT is changed or reaffirmed. The zero
         * Address indicates there is no approved Address. When a Transfer event emits, this also
         * indicates that the approved Address for that NFT (if any) is reset to none.
         */
        /*LogApproval(
          Address owner,
          Address approved,
          ulong tokenId
        );*/

        /**
         * @dev This emits when an operator is enabled or disabled for an owner. The operator can manage
         * all NFTs of the owner.
         */
        /*LogApprovalForAll(
          Address owner,
          Address operatorAddress,
          bool approved
        );*/

        /**
         * @dev Transfers the ownership of an NFT from one Address to another Address.
         * @notice Throws unless `msg.sender` is the current owner, an authorized operator, or the
         * approved Address for this NFT. Throws if `from` is not the current owner. Throws if `to` is
         * the zero Address. Throws if `tokenId` is not a valid NFT. When transfer is complete, this
         * function checks if `to` is a smart contract (code size > 0). If so, it calls
         * `onERC721Received` on `to` and throws if the return value is not 
         * `bytes4(keccak256("onERC721Received(Address,ulong,bytes)"))`.
         * @param from The current owner of the NFT.
         * @param to The new owner.
         * @param tokenId The NFT to transfer.
         * @param data Additional data with no specified format, sent in call to `to`.
         */
        void SafeTransferFrom(
          Address from,
          Address to,
          ulong tokenId,
          byte[] data);


        /**
         * @dev Transfers the ownership of an NFT from one Address to another Address.
         * @notice This works identically to the other function with an extra data parameter, except this
         * function just sets data to ""
         * @param from The current owner of the NFT.
         * @param to The new owner.
         * @param tokenId The NFT to transfer.
         */
        void SafeTransferFrom(
          Address from,
          Address to,
          ulong tokenId
        );


        /**
         * @dev Throws unless `msg.sender` is the current owner, an authorized operator, or the approved
         * Address for this NFT. Throws if `from` is not the current owner. Throws if `to` is the zero
         * Address. Throws if `tokenId` is not a valid NFT.
         * @notice The caller is responsible to confirm that `to` is capable of receiving NFTs or else
         * they mayb be permanently lost.
         * @param from The current owner of the NFT.
         * @param to The new owner.
         * @param tokenId The NFT to transfer.
         */
        void TransferFrom(
          Address from,
          Address to,
          ulong tokenId
        );


        /**
         * @dev Set or reaffirm the approved Address for an NFT.
         * @notice The zero Address indicates there is no approved Address. Throws unless `msg.sender` is
         * the current NFT owner, or an authorized operator of the current owner.
         * @param approved The new approved NFT controller.
         * @param tokenId The NFT to approve.
         */
        void Approve(
          Address approved,
          ulong tokenId
        );


        /**
         * @dev Enables or disables approval for a third party ("operator") to manage all of
         * `msg.sender`'s assets. It also emits the ApprovalForAll event.
         * @notice The contract MUST allow multiple operators per owner.
         * @param operator Address to add to the set of authorized operators.
         * @param approved True if the operators is approved, false to revoke approval.
         */
        void SetApprovalForAll(
          Address operatorAddress,
          bool approved
        );


        /**
         * @dev Returns the number of NFTs owned by `owner`. NFTs assigned to the zero Address are
         * considered invalid, and this function throws for queries about the zero Address.
         * @param owner Address for whom to query the balance.
         * @return Balance of owner.
         */
        ulong BalanceOf(
          Address owner
        );

        /**
         * @dev Returns the Address of the owner of the NFT. NFTs assigned to zero Address are considered
         * invalid, and queries about them do throw.
         * @param tokenId The identifier for an NFT.
         * @return Address of tokenId owner.
         */
        Address OwnerOf(
          ulong tokenId
        );

        /**
         * @dev Get the approved Address for a single NFT.
         * @notice Throws if `tokenId` is not a valid NFT.
         * @param tokenId The NFT to find the approved Address for.
         * @return Address that tokenId is approved for. 
         */
        Address GetApproved(
          ulong tokenId
        );

        /**
         * @dev Returns true if `operator` is an approved operator for `owner`, false otherwise.
         * @param owner The Address that owns the NFTs.
         * @param operator The Address that acts on behalf of the owner.
         * @return True if approved for all, false otherwise.
         */
        bool IsApprovedForAll(
          Address owner,
          Address operatorAddress
        );
    }
}
