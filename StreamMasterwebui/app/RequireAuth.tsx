import { Logger } from '@lib/common/logger';
import { baseHostURL } from '@lib/settings';
import { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';

const RequireAuth = ({ children }: { children: JSX.Element }): JSX.Element => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const location = useLocation();

  function getQueryParam(param) {
    const params = new URLSearchParams(window.location.search);
    return params.get(param);
  }

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const response = await fetch(baseHostURL + '/needAuth', {
          credentials: 'include',
          headers: {
            'Content-Type': 'application/json'
          },
          method: 'GET'
        });
        const needAuth = await response.json();
        setIsAuthenticated(needAuth === false);
      } catch (error: any) {
        if (error?.message === 'Failed to fetch') {
          Logger.error('Failed to fetch, check if the server is running');
          setIsAuthenticated(null);
        } else {
          Logger.error('Error checking authentication status:', error);
          setIsAuthenticated(false);
        }
      }
    };
    checkAuth();
  }, []);

  if (isAuthenticated === null) {
    // Still loading, optionally show a loading indicator here
    return <div>Loading...</div>;
  }

  let returnUrl = getQueryParam('ReturnUrl');
  if (returnUrl === null || returnUrl === '' || returnUrl === '/') {
    returnUrl = '/editor/streams';
  }
  if (!isAuthenticated) {
    // const a = encodeURIComponent(location.pathname + location.search);

    // // If not authenticated, redirect to login with the current path as ReturnUrl
    // return <Navigate to={`/login?ReturnUrl=${encodeURIComponent(location.pathname + location.search)}`} />;
    if (!location.pathname.startsWith('/login')) {
      window.location.href = `/login?ReturnUrl=${encodeURIComponent(returnUrl)}`;
    }
    return <div />; // return null to avoid rendering further
  }

  // User is authenticated, render the child component
  return children;
};

export default RequireAuth;
