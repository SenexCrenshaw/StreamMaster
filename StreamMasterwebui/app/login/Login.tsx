import { baseHostURL } from '@lib/settings';
import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

const Login: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const [loginFailed, setLoginFailed] = useState(false);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  // Get `ReturnUrl` from query params
  const getQueryParam = (param: string) => {
    const params = new URLSearchParams(location.search);
    return params.get(param);
  };

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const response = await fetch(baseHostURL + '/needAuth', {
          method: 'GET',
          credentials: 'include',
          headers: {
            'Content-Type': 'application/json'
          }
        });

        const data = await response.json();
        if (data === false) {
          // Redirect to ReturnUrl if authenticated
          const returnUrl = getQueryParam('ReturnUrl');
          navigate(returnUrl ? decodeURIComponent(returnUrl) : '/editor/streams');
        }
      } catch (error) {
        console.error('Error checking authentication status:', error);
      }
    };

    checkAuth();

    // Set the year for the footer
    document.getElementById('year')!.textContent = new Date().getFullYear().toString();

    // Check for login failure in query params
    if (location.search.includes('loginFailed=true')) {
      setLoginFailed(true);
    }
  }, [location, navigate]);

  const handleLogin = async (event: React.FormEvent) => {
    event.preventDefault();
    // Add login logic here with your API for authenticating the user
    // For example:
    // const result = await yourAuthService.login(username, password);
    // if (!result) setLoginFailed(true);
  };

  return (
    <div className="center">
      <div className="content">
        <div className="panel">
          <div className="panel-header" style={{ backgroundColor: '#263238' }}>
            <img src="/images/streammaster_logo.png" alt="Stream Master Logo" className="logo" />
          </div>

          <div className="panel-body" style={{ backgroundColor: '#21282c' }}>
            <div className="sign-in">Log In</div>

            <form onSubmit={handleLogin} className="mb-lg" noValidate>
              <div className="form-group">
                <input
                  type="email"
                  name="username"
                  className="form-input"
                  placeholder="Username"
                  autoComplete="off"
                  required
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                />
              </div>

              <div className="form-group">
                <input
                  type="password"
                  name="password"
                  className="form-input"
                  placeholder="Password"
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
              </div>
              <button type="submit" className="button" style={{ backgroundColor: '#6dc831ee', color: 'white' }}>
                Log In
              </button>

              {loginFailed && <div className="login-failed">Incorrect Username or Password</div>}
            </form>
          </div>
        </div>

        <div id="copy" className="copy">
          <span>&copy;</span>
          <span id="year"></span>
          <span>- Stream Master</span>
        </div>
      </div>
    </div>
  );
};

export default Login;
