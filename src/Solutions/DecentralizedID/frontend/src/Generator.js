import React, { useEffect, useState } from 'react';
import uuid from "uuid";

function Config() {
  const [context, setContext] = useState('https://www.w3.org/ns/did/v1');
  const [id, setId] = useState(`did:stratis:${uuid.v4()}`)
  const [authType, setAuthType] = useState('RsaVerification2018');
  const [publicKey, setPublicKey] = useState('');
  const [serviceType, setServiceType] = useState('VerifiableCredentialService');
  const [serviceEndpoint, setServiceEndpoint] = useState('VerifiableCredentialService');


  const [jsonData, setJsonData] = useState('');

  const handleSubmit = () => {
    let data = new Object();
    data['@context'] = context;
    data['id'] = id;
    data['authentication'] = [{
      "id": id + "#keys-1",
      "type": authType,
      "controller": id,
      "publicKey": publicKey
    }];
    data['service'] = [{
      "id": id + "#vcs",
      "type": serviceType,
      "serviceEndpoint": serviceEndpoint,
    }]

    let jsonData = JSON.stringify(data);

    setJsonData(jsonData);

    let copyText = document.getElementById('generate');
    copyText.select();
    copyText.setSelectionRange(0, 99999); /*For mobile devices*/
    document.execCommand("copy");

    window.alert('Copied to Clipboard!');
  }

  const toggleDisplay = () => {
    var x = document.getElementById("advanced");
    if (x.style.display === "none") {
      x.style.display = "block";
    } else {
      x.style.display = "none";
    }
  }

  return (
    <div className="center-container">
      <div className="card">
        <div className="card-header">
          <h1>Generate DID Document</h1>
        </div>
        <div className="card-content">
          <form>
              <div className="card">
                <div className="card-header">
                  <h3>Authentication</h3>
                </div>
                <div className="card-content">
                  <label htmlFor="auth-type">
                    <span>Authentication Type</span>
                    <input 
                      type="text" 
                      name="auth-type" 
                      id="auth-type" 
                      placeholder="Authentication Type"
                      value={authType}
                      onChange={(e) => {
                        setAuthType(e.target.value);
                      }} 
                    />
                  </label>
                  <label htmlFor="public-key">
                    <span>Public Key</span>
                    <input 
                      type="text" 
                      name="public-key" 
                      id="public-key" 
                      placeholder="Public Key"
                      value={publicKey}
                      onChange={(e) => {
                        setPublicKey(e.target.value);
                      }} 
                    />
                  </label>
                </div>
              </div>

              <div className="card">
                <div className="card-header">
                  <h3>Authentication Service</h3>
                </div>
                <div className="card-content">
                  <label htmlFor="service-type">
                    <span>Service Type</span>
                    <input 
                      type="text" 
                      name="service-type" 
                      id="service-type" 
                      placeholder="Service Type"
                      value={serviceType}
                      onChange={(e) => {
                        setServiceType(e.target.value);
                      }} 
                    />
                  </label>
                  <label htmlFor="service-endpoint">
                    <span>Service Endpoint</span>
                    <input 
                      type="text" 
                      name="service-endpoint" 
                      id="service-endpoint" 
                      placeholder="Service Endpoint"
                      value={serviceEndpoint}
                      onChange={(e) => {
                        setServiceEndpoint(e.target.value);
                      }} 
                    />
                  </label>
                </div>
              </div>

              <div className="card" onClick={toggleDisplay}>
                <div className="card-header">
                  <h3>Advanced Document Fields</h3>
                </div>
                <div id="advanced" className="card-content">
                  <label htmlFor="context">
                    <span>Context</span>
                    <input 
                      type="text" 
                      name="context" 
                      id="context" 
                      placeholder="DID Context"
                      value={context}
                      onChange={(e) => {
                        setContext(e.target.value);
                      }} 
                    />
                  </label>

                  <label htmlFor="document-id">
                    <span>Document ID</span>
                    <input 
                      type="text" 
                      name="document-id" 
                      id="document-id" 
                      placeholder="Document ID"
                      value={id}
                      onChange={(e) => {
                        setId(e.target.value);
                      }} 
                    />
                  </label>
                </div>
              </div>
            <button 
              type="button" 
              onClick={handleSubmit}
            >
              GENERATE
            </button>
          </form>
          <textarea
            id='generate'
            value={jsonData}
          />
        </div>
      </div>
    </div>
  );
}

export default Config;
