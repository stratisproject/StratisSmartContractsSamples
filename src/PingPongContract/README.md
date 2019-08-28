Ping Pong Sample Smart Contract
====================================================
	
Overview 
---------
The Ping Pong contract demonstrates how one contract can be used to create other contracts. It also demonstrates how two contracts can interact with each other.
	
Contracts 
------------------
| Name       | Description                                                                                         |
|------------|-----------------------------------------------------------------------------------------------------|
| Starter | A factory which creates two Player contracts.                                        |
| Player | Able to send and receive pings from another contract.                                        |

Player Workflow States 
-------
| Name                 | Description                                                                                                 |
|----------------------|-------------------------------------------------------------------------------------------------------------|
| Provisioned | The state that is reached after the contract is first created.|
| SentPing | The state that is reached after the player has sent a ping, but has not received a response.  |
| ReceivedPing |  The state that is reached after the player has sent a ping, and has not received a response. |
| Finished | The state that is reached when the ping pong game is finished. |
	
	
Workflow Details
---------------
	
The ping pong game starts when the Starter contract is created. The Starter contract has a single method which can be used to create two Player contracts. When the Starter creates the Player contracts, each contract is assigned a 'Player'. Only the player is able to interact with the contract. A Player contract has an opponent Player. The opponent Player contract can engage in a back and forth game of sending pings, until one player ends the game.
	
	
Application Files
-----------------

[Player.sol](./PingPongContract/Player.sol)

[Starter.sol](./PingPongContract/Starter.sol)