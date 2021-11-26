import React from 'react';
import ReactDOM from 'react-dom';
import { Route, Link, Switch, BrowserRouter as Router } from 'react-router-dom';

import './index.css';
import HeaderLogo from './assets/headerlogo.png';

/* Not used because I can't solve the CORS problem in time */
// import Config from './Config';
// import Manage from './Manage';
import Generator from './Generator'

import * as serviceWorker from './serviceWorker';

const routing = (
  <Router>
    <header>
      <div className="header">
        <Link to="/">
          <img 
            className="logo"
            src={HeaderLogo} 
            alt="logo"
          />
        </Link>
        <div className="header-right">
          {/* <Link to="/">Config</Link> */}
          {/* <Link to="/manage">Manage</Link> */}
        </div>
      </div>
    </header>
    <div>
      <Switch>
        {/* <Route exact path="/" component={Config} /> */}
        {/* <Route path="/" component={Manage} /> */}
        <Route exact path="/" component={Generator} />
      </Switch>
    </div>
  </Router>
)

ReactDOM.render(routing, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
