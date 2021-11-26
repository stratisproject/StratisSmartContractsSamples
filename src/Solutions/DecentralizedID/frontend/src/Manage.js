import React, { useState, useEffect } from 'react';
import axios from 'axios';

function Manage() {
  const [didNumOwner, setDidNumOwner] = useState(0); 
  const [didNumData, setDidNumData] = useState(0); 
  const [contractAddress, setContractAddress] = useState(''); 
  const [walletName, setWalletName] = useState('');
  const [activeAddress, setActiveAddress] = useState('');
  const [walletPassword, setWalletPassword] = useState('');

  useEffect(() => {
    setContractAddress(localStorage.getItem('contractAddress'));
    setWalletName(localStorage.getItem('walletName'));
    setActiveAddress(localStorage.getItem('activeAddress'));
    setWalletPassword(localStorage.getItem('walletPassword'));
  }, [])

  const getOwner = () => {
    axios.post(`${process.env.REACT_APP_API_URL}/api/contract/${contractAddress}/method/GetOwnerOfDID`, 
    {
      "DIDIndex": didNumOwner,
    },
    {
      crossdomain: true,
      headers: {
        'Access-Control-Allow-Origin': '*',
        "GasPrice": 100,
        "GasLimit": 100000,
        "Amount": 0,
        "FeeAmount": 0.01,
        "WalletName": walletName,
        "WalletPassword": walletPassword,
        "Sender": activeAddress,
        "Content-Type": "application/json",
      },
    }).then((res) => {
      console.log(res);
    }).catch((err) => {
      console.log(err);
    });
  }
  
  const getData = () => {
    return;
  }

  return (
    <div>
      <div className="center-container">
        <div className="card">
          <div className="card-header">
            <h1>Add Document</h1>
          </div>
        </div>

        <div className="card">
          <div className="card-header" style={{ backgroundColor: "#f5075a"}}>
            <h1>Revoke Document</h1>
          </div>
        </div>

        <div className="card">
          <div className="card-header">
            <h1>Get Information</h1>
            
          </div>
          <div className="card-content">
            <div className="card">
              <form>
                <h2>Find Owner Of Document</h2>
                <label htmlFor="did-owner">
                  <span>Decentralized Document ID</span>
                  <input 
                    type="number" 
                    name="did-owner" 
                    id="did-owner" 
                    value={didNumOwner}
                    onChange={(e) => {
                      setDidNumOwner(e.target.value);
                    }} 
                  />
                </label>
                <button 
                  type="button" 
                  onClick={getOwner}
                >
                  Submit
                </button>
              </form>
            </div>

            <div className="card">
              <form>
                <h2>Get Document Data</h2>
                <label htmlFor="did-data">
                  <span>Decentralized Document ID</span>
                  <input 
                    type="number" 
                    name="did-data" 
                    id="did-data" 
                    value={didNumData}
                    onChange={(e) => {
                      setDidNumData(e.target.value);
                    }} 
                  />
                </label>
                <button 
                  type="button" 
                  onClick={getData}
                >
                  Submit
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Manage;
