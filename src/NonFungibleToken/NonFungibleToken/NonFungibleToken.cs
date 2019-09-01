namespace NonFungibleToken
{
    using Stratis.SmartContracts;
    using System;

    public class NonFungibleToken : SmartContract, INonFungibleToken
    {
        /**
         * @dev Magic value of a smart contract that can recieve NFT.
         * Equal to: bytes4(keccak256("onERC721Received(address,address,uint256,bytes)")).
         */
        // todo: how do we calculate if this is still correct after we've implemented the NonFungibleTokenReceiver?
        internal static readonly byte[] MAGIC_ON_ERC721_RECEIVED = new byte[] { 0x15, 0x0b, 0x7a, 0x02 };

        private string GetIdToOwnerKey(ulong id)
        {
            return $"IdToOwner:{id}";
        }

        /**
         * @dev A mapping from NFT ID to the address that owns it.
         */
        private Address GetIdToOwner(ulong id)
        {
            return this.PersistentState.GetAddress(GetIdToOwnerKey(id));
        }

        private void SetIdToOwner(ulong id, Address value)
        {
            this.PersistentState.SetAddress(GetIdToOwnerKey(id), value);
        }

        /**
         * @dev Mapping from NFT ID to approved address.
         */
        private string GetIdToApprovalKey(ulong id)
        {
            return $"IdToApproval:{id}";
        }

        private Address GetIdToApproval(ulong id)
        {
            return this.PersistentState.GetAddress(GetIdToApprovalKey(id));
        }

        private void SetIdToApproval(ulong id, Address value)
        {
            this.PersistentState.SetAddress(GetIdToApprovalKey(id), value);
        }

        /**
        * @dev Mapping from owner address to count of his tokens.
        */
        private ulong GetOwnerToNFTokenCount(Address address)
        {
            return this.PersistentState.GetUInt64($"OwnerToNFTokenCount:{address}");
        }

        private void SetOwnerToNFTokenCount(Address address, ulong value)
        {
            this.PersistentState.SetUInt64($"OwnerToNFTokenCount:{address}", value);
        }

        /**
         * @dev Mapping from owner address to mapping of operator addresses.
         */
        // todo: review if this is the best way to solve it.
        private bool GetOwnerToOperators(Address owner, Address operatorAddress)
        {
            return this.PersistentState.GetBool($"OwnerToOperator:{owner}:{operatorAddress}");
        }

        private void SetOwnerToOperators(Address owner, Address operatorAddress, bool value)
        {
            this.PersistentState.SetBool($"OwnerToOperator:{owner}:{operatorAddress}", value);
        }

        public struct TransferLog
        {
            [Index]
            public Address From;
            [Index]
            public Address To;
            [Index]
            public ulong TokenId;
        }

        public struct ApprovalLog
        {
            [Index]
            public Address Owner;
            [Index]
            public Address Approved;
            [Index]
            public ulong TokenId;
        }

        public struct ApprovalForAllLog
        {
            [Index]
            public Address Owner;
            [Index]
            public Address Operator;

            public bool Approved;
        }

        /**
         * @dev Guarantees that the msg.sender is an owner or operator of the given NFT.
         * @param _tokenId ID of the NFT to validate.
         */
        private void CanOperate(ulong tokenId)
        {
            Address tokenOwner = GetIdToOwner(tokenId);
            Assert(tokenOwner == this.Message.Sender || GetOwnerToOperators(tokenOwner, this.Message.Sender));
        }

        /**
         * @dev Guarantees that the msg.sender is allowed to transfer NFT.
         * @param _tokenId ID of the NFT to transfer.
         */
        private void CanTransfer(ulong tokenId)
        {
            Address tokenOwner = GetIdToOwner(tokenId);
            Assert(
              tokenOwner == this.Message.Sender
              || GetIdToApproval(tokenId) == Message.Sender
              || GetOwnerToOperators(tokenOwner, Message.Sender)
            );
        }

        /**
         * @dev Guarantees that _tokenId is a valid Token.
         * @param _tokenId ID of the NFT to validate.
         */
        private void ValidNFToken(ulong tokenId)
        {
            Assert(GetIdToOwner(tokenId) != Address.Zero);
        }

        /**
         * @dev Contract constructor.
         */
        public NonFungibleToken(ISmartContractState state) : base(state)
        {
            this.SetSupportedInterfaces(0x01ffc9a7, true); // ERC165
            this.SetSupportedInterfaces(0x80ac58cd, true); // ERC721
        }

        /**
         * @dev Transfers the ownership of an NFT from one address to another address. This function can
         * be changed to payable.
         * @notice Throws unless `msg.sender` is the current owner, an authorized operator, or the
         * approved address for this NFT. Throws if `_from` is not the current owner. Throws if `_to` is
         * the zero address. Throws if `_tokenId` is not a valid NFT. When transfer is complete, this
         * function checks if `_to` is a smart contract (code size > 0). If so, it calls 
         * `onERC721Received` on `_to` and throws if the return value is not 
         * `bytes4(keccak256("onERC721Received(address,uint256,bytes)"))`.
         * @param _from The current owner of the NFT.
         * @param _to The new owner.
         * @param _tokenId The NFT to transfer.
         * @param _data Additional data with no specified format, sent in call to `_to`.
         */
        public void SafeTransferFrom(
          Address from,
          Address to,
          ulong tokenId,
          byte[] data)
        {
            SafeTransferFromInternal(from, to, tokenId, data);
        }

        /**
         * @dev Transfers the ownership of an NFT from one address to another address. This function can
         * be changed to payable.
         * @notice This works identically to the other function with an extra data parameter, except this
         * function just sets data to ""
         * @param _from The current owner of the NFT.
         * @param _to The new owner.
         * @param _tokenId The NFT to transfer.
         */
        public void SafeTransferFrom(
          Address from,
          Address to,
          ulong tokenId
        )
        {
            SafeTransferFromInternal(from, to, tokenId, new byte[0]);
        }

        /**
         * @dev Throws unless `msg.sender` is the current owner, an authorized operator, or the approved
         * address for this NFT. Throws if `_from` is not the current owner. Throws if `_to` is the zero
         * address. Throws if `_tokenId` is not a valid NFT. This function can be changed to payable.
         * @notice The caller is responsible to confirm that `_to` is capable of receiving NFTs or else
         * they maybe be permanently lost.
         * @param _from The current owner of the NFT.
         * @param _to The new owner.
         * @param _tokenId The NFT to transfer.
         */
        public void TransferFrom(
          Address from,
          Address to,
          ulong tokenId
        )
        {
            CanTransfer(tokenId);
            ValidNFToken(tokenId);

            Address tokenOwner = GetIdToOwner(tokenId);
            Assert(tokenOwner == from);
            Assert(to != Address.Zero);

            TransferInternal(to, tokenId);
        }

        /**
         * @dev Set or reaffirm the approved address for an NFT. This function can be changed to payable.
         * @notice The zero address indicates there is no approved address. Throws unless `msg.sender` is
         * the current NFT owner, or an authorized operator of the current owner.
         * @param _approved Address to be approved for the given NFT ID.
         * @param _tokenId ID of the token to be approved.
         */
        public void Approve(
          Address approved,
          ulong tokenId
        )
        {
            CanOperate(tokenId);
            ValidNFToken(tokenId);

            Address tokenOwner = GetIdToOwner(tokenId);
            Assert(approved != tokenOwner);

            SetIdToApproval(tokenId, approved);
            LogApproval(tokenOwner, approved, tokenId);
        }

        /**
         * @dev Enables or disables approval for a third party ("operator") to manage all of
         * `msg.sender`'s assets. It also emits the ApprovalForAll event.
         * @notice This works even if sender doesn't own any tokens at the time.
         * @param _operator Address to add to the set of authorized operators.
         * @param _approved True if the operators is approved, false to revoke approval.
         */
        public void SetApprovalForAll(
          Address operatorAddress,
          bool approved
        )
        {
            SetOwnerToOperators(this.Message.Sender, operatorAddress, approved);
            LogApprovalForAll(this.Message.Sender, operatorAddress, approved);
        }

        /**
         * @dev Returns the number of NFTs owned by `_owner`. NFTs assigned to the zero address are
         * considered invalid, and this function throws for queries about the zero address.
         * @param _owner Address for whom to query the balance.
         * @return Balance of _owner.
         */
        public ulong BalanceOf(
          Address owner
        )
        {
            Assert(owner != Address.Zero);
            return GetOwnerToNFTokenCount(owner);
        }

        /**
         * @dev Returns the address of the owner of the NFT. NFTs assigned to zero address are considered
         * invalid, and queries about them do throw.
         * @param _tokenId The identifier for an NFT.
         * @return Address of _tokenId owner.
         */
        public Address OwnerOf(
          ulong tokenId
        )
        {
            Address owner = GetIdToOwner(tokenId);
            Assert(owner != Address.Zero);
            return owner;
        }

        /**
         * @dev Get the approved address for a single NFT.
         * @notice Throws if `_tokenId` is not a valid NFT.
         * @param _tokenId ID of the NFT to query the approval of.
         * @return Address that _tokenId is approved for. 
         */
        public Address GetApproved(
          ulong tokenId
        )
        {
            ValidNFToken(tokenId);

            return GetIdToApproval(tokenId);
        }

        /**
         * @dev Checks if `_operator` is an approved operator for `_owner`.
         * @param _owner The address that owns the NFTs.
         * @param _operator The address that acts on behalf of the owner.
         * @return True if approved for all, false otherwise.
         */
        public bool IsApprovedForAll(
          Address owner,
          Address operatorAddress
        )
        {
            return GetOwnerToOperators(owner, operatorAddress);
        }

        /**
         * @dev Actually preforms the transfer.
         * @notice Does NO checks.
         * @param _to Address of a new owner.
         * @param _tokenId The NFT that is being transferred.
         */
        private void TransferInternal(
          Address to,
          ulong tokenId
        )
        {
            Address from = GetIdToOwner(tokenId);
            ClearApproval(tokenId);

            RemoveNFToken(from, tokenId);
            AddNFToken(to, tokenId);

            LogTransfer(from, to, tokenId);
        }

        /**
         * @dev Mints a new NFT.
         * @notice This is an internal function which should be called from user-implemented external
         * mint function. Its purpose is to show and properly initialize data structures when using this
         * implementation.
         * @param _to The address that will own the minted NFT.
         * @param _tokenId of the NFT to be minted by the msg.sender.
         */
        private void Mint(
          Address to,
          ulong tokenId
        )
        {
            // todo: no references and not on interface? what to do?
            Assert(to != Address.Zero);
            Assert(GetIdToOwner(tokenId) == Address.Zero);

            AddNFToken(to, tokenId);

            LogTransfer(Address.Zero, to, tokenId);
        }

        /**
         * @dev Burns a NFT.
         * @notice This is an internal function which should be called from user-implemented external burn
         * function. Its purpose is to show and properly initialize data structures when using this
         * implementation. Also, note that this burn implementation allows the minter to re-mint a burned
         * NFT.
         * @param _tokenId ID of the NFT to be burned.
         */
        private void Burn(
          ulong tokenId
        )
        {
            // todo: no references and not on interface? what to do?
            ValidNFToken(tokenId);

            Address tokenOwner = GetIdToOwner(tokenId);
            ClearApproval(tokenId);
            RemoveNFToken(tokenOwner, tokenId);
            LogTransfer(tokenOwner, Address.Zero, tokenId);
        }

        /**
         * @dev Removes a NFT from owner.
         * @notice Use and override this function with caution. Wrong usage can have serious consequences.
         * @param _from Address from wich we want to remove the NFT.
         * @param _tokenId Which NFT we want to remove.
         */
        private void RemoveNFToken(
          Address from,
          ulong tokenId
        )
        {
            Assert(GetIdToOwner(tokenId) == from);
            SetOwnerToNFTokenCount(from, GetOwnerToNFTokenCount(from) - 1);
            this.PersistentState.Clear(GetIdToOwnerKey(tokenId));
        }

        /**
         * @dev Assignes a new NFT to owner.
         * @notice Use and override this function with caution. Wrong usage can have serious consequences.
         * @param _to Address to wich we want to add the NFT.
         * @param _tokenId Which NFT we want to add.
         */
        public void AddNFToken(
          Address to,
          ulong tokenId
        )
        {
            Assert(GetIdToOwner(tokenId) == Address.Zero);

            SetIdToOwner(tokenId, to);
            ulong currentTokenAmount = GetOwnerToNFTokenCount(to);
            SetOwnerToNFTokenCount(to, checked(currentTokenAmount + 1));
        }

        /**
         * @dev Helper function that gets NFT count of owner. This is needed for overriding in enumerable
         * extension to remove double storage (gas optimization) of owner nft count.
         * @param _owner Address for whom to query the count.
         * @return Number of _owner NFTs.
         */
        /*private ulong GetOwnerNFTCount(
          Address owner
        )
        {            
            return GetOwnerToNFTokenCount(owner);
        }*/

        /**
         * @dev Actually perform the safeTransferFrom.
         * @param _from The current owner of the NFT.
         * @param _to The new owner.
         * @param _tokenId The NFT to transfer.
         * @param _data Additional data with no specified format, sent in call to `_to`.
         */
        private void SafeTransferFromInternal(
          Address from,
          Address to,
          ulong tokenId,
          byte[] data
        )
        {
            CanTransfer(tokenId);
            ValidNFToken(tokenId);

            Address tokenOwner = GetIdToOwner(tokenId);
            Assert(tokenOwner == from);
            Assert(to != Address.Zero);

            TransferInternal(to, tokenId);

            if (this.PersistentState.IsContract(to))
            {
                ITransferResult result = this.Call(to, 0, "onERC721Received", new object[] { this.Message.Sender, from, tokenId, data }, 0);
                var retval = result.ReturnValue as byte[];
                Assert(MagicBytesEqual(retval, MAGIC_ON_ERC721_RECEIVED));
            }
        }

        /** 
         * @dev Clears the current approval of a given NFT ID.
         * @param _tokenId ID of the NFT to be transferred.
         */
        private void ClearApproval(
          ulong tokenId
        )
        {
            if (GetIdToApproval(tokenId) != Address.Zero)
            {
                this.PersistentState.Clear(GetIdToApprovalKey(tokenId));
            }
        }

        /**
     * @dev Emits when ownership of any NFT changes by any mechanism. This event emits when NFTs are
     * created (`from` == 0) and destroyed (`to` == 0). Exception: during contract creation, any
     * number of NFTs may be created and assigned without emitting Transfer. At the time of any
     * transfer, the approved Address for that NFT (if any) is reset to none.
     */
        private void LogTransfer(
          Address from,
          Address to,
          ulong tokenId
        )
        {
            Log(new TransferLog() { From = from, To = to, TokenId = tokenId });
        }

        /**
         * @dev This emits when the approved Address for an NFT is changed or reaffirmed. The zero
         * Address indicates there is no approved Address. When a Transfer event emits, this also
         * indicates that the approved Address for that NFT (if any) is reset to none.
         */
        private void LogApproval(
          Address owner,
          Address approved,
          ulong tokenId
        )
        {
            Log(new ApprovalLog() { Owner = owner, Approved = approved, TokenId = tokenId });
        }

        /**
         * @dev This emits when an operator is enabled or disabled for an owner. The operator can manage
         * all NFTs of the owner.
         */
        private void LogApprovalForAll(
          Address owner,
          Address operatorAddress,
          bool approved
        )
        {
            Log(new ApprovalForAllLog() { Owner = owner, Operator = operatorAddress, Approved = approved });
        }

        private bool GetSupportedInterfaces(uint interfaceId)
        {
            return this.PersistentState.GetBool($"SupportedInterface:{interfaceId}");
        }

        private void SetSupportedInterfaces(uint interfaceId, bool value)
        {
            this.PersistentState.SetBool($"SupportedInterface:{interfaceId}", value);
        }

        /**
         * @dev Function to check which interfaces are suported by this contract.
         * @param _interfaceID Id of the interface.
         * @return True if _interfaceID is supported, false otherwise.
         */
        public bool SupportsInterface(uint interfaceID)
        {
            // todo: go back to byte4?
            return GetSupportedInterfaces(interfaceID);
        }

        // todo: rename to onNonFungibleTokenReceived?
        public byte[] onERC721Received(Address operatorAddress, Address fromAddress, ulong tokenId, byte[] data)
        {
            // todo: update documentation.
            string stringToHash = $"onERC721Received{operatorAddress},{fromAddress},{tokenId},{BitConverter.ToString(data)}";
            byte[] bytesToHash = Array.ConvertAll(stringToHash.ToCharArray(), s => Convert.ToByte(s));
            var keccakHash = this.Keccak256(bytesToHash);
            var magicBytes = new byte[] { keccakHash[0], keccakHash[1], keccakHash[2], keccakHash[3] };
            return magicBytes;
        }

        private bool MagicBytesEqual(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null)
            {
                return false;
            }

            if (b1.Length != 4 || b2.Length != 4)
            {
                return false;
            }

            for (var i = 0; i < 4; i++)
            {
                if (!b1[i].Equals(b2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
