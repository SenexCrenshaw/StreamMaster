import { useLocation } from "react-router-dom";
import React, { useEffect } from "react";
import { useSessionStorage } from "primereact/hooks";
import { Navigate } from "react-router-dom";

const ProtectedRoute = (props: ProtectedRouteProps) => {
  const [navigateTo, setNavigateTo] = useSessionStorage<string>('/', 'navigateTo');
  const location = useLocation();

  useEffect(() => {
    if (navigateTo !== location.pathname) {
      setNavigateTo(location.pathname);
    }
  }, [location.pathname, navigateTo, setNavigateTo])

  if (!props.isAuthenticated) {
    return <Navigate replace to='/login' />;
  }

  return props.children;

}

type ProtectedRouteProps = {
  children: JSX.Element;
  isAuthenticated: boolean;

}


export default React.memo(ProtectedRoute);
