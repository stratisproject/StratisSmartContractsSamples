import React, { useEffect, useState } from 'react';

function Config() {
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

 const handleSave = () => {
    localStorage.setItem('contractAddress', contractAddress);
    localStorage.setItem('walletName', walletName);
    localStorage.setItem('activeAddress', activeAddress);
    localStorage.setItem('walletPassword', walletPassword);

    window.alert('Saved!');
  }

  return (
    <div className="center-container">
      <div className="card">
        <div className="card-header">
          <h1>Configuration</h1>
        </div>
        <div className="card-content">
          <form>
            <label htmlFor="contract-address">
              <span>Contract Address</span>
              <input 
                type="text" 
                name="contract-address" 
                id="contract-address" 
                placeholder="Contract Address eg. CZtwKAMnz7UW9ADaFcC3im2YUBEN77TpU1"
                value={contractAddress}
                onChange={(e) => {
                  setContractAddress(e.target.value);
                }} 
              />
            </label>
            <label htmlFor="wallet-name">
              <span>Wallet Name</span>
              <input 
                type="text" 
                name="wallet-name" 
                id="wallet-name" 
                placeholder="Wallet Name eg. Hackathon_1"
                value={walletName}
                onChange={(e) => {
                  setWalletName(e.target.value);
                }} 
              />
            </label>
            <label htmlFor="active-address">
              <span>Active Address</span>
              <input 
                type="text" 
                name="active-address" 
                id="active-address" 
                placeholder="Active Address eg. CUtNvY1Jxpn4V4RD1tgphsUKpQdo4q5i54"
                value={activeAddress}
                onChange={(e) => {
                  setActiveAddress(e.target.value);
                }} 
              />
            </label>
            <label htmlFor="wallet-password">
              <span>Wallet Password</span>
              <input 
                type="password" 
                name="wallet-password" 
                id="wallet-password" 
                placeholder="Wallet Password"
                value={walletPassword}
                onChange={(e) => {
                  setWalletPassword(e.target.value);
                }} 
              />
            </label>

            <button 
              type="button" 
              onClick={handleSave}
            >
              SAVE
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Config;
