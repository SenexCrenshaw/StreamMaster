import { Logger } from '@lib/common/logger';
import { useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';

const RequireAuth = ({ children }: { children: JSX.Element }): JSX.Element => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const location = useLocation();

  console.log('RequireAuth Mounted');

  useEffect(() => {
    Logger.error('Check Auth Started');
    const checkAuth = async () => {
      try {
        const response = await fetch('/needAuth', {
          credentials: 'include',
          headers: {
            'Content-Type': 'application/json'
          },
          method: 'GET'
        });
        const needAuth = await response.json();
        setIsAuthenticated(needAuth === false);
      } catch (error) {
        Logger.error('Error checking authentication status:', error);
        setIsAuthenticated(false);
      }
    };
    checkAuth();
  }, []);

  Logger.error('RequireAuth', 'isAuthenticated', isAuthenticated);

  if (isAuthenticated === null) {
    // Still loading, optionally show a loading indicator here
    return <div>Loading...</div>;
  }

  if (!isAuthenticated) {
    // If not authenticated, redirect to login with the current path as ReturnUrl
    return <Navigate to={`/login?ReturnUrl=${encodeURIComponent(location.pathname + location.search)}`} />;
  }

  // User is authenticated, render the child component
  return children;
};

export default RequireAuth;
