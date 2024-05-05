import { Outlet } from 'react-router-dom';
import { RootSideBar } from './RootSideBar';
import Loader from '@components/loader/Loader';
import { GetIsSystemReady } from '@lib/smAPI/Settings/SettingsCommands';
import { useEffect, useState } from 'react';

export const RootLayout = (): JSX.Element => {
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const intervalId = setInterval(() => {
      GetIsSystemReady()
        .then((result) => {
          setIsReady(result ?? false);
        })
        .catch(() => {
          setIsReady(false);
        });
    }, 5000);

    return () => clearInterval(intervalId);
  }, []);

  if (!isReady || 1 === 1) {
    return (
      <div className="main-outlet flex flex-column w-full align-items-center justify-content-center border-1">
        <Loader />
      </div>
    );
  }
  return (
    <div className="flex max-h-screen p-fluid">
      <RootSideBar />

      <div className="main-outlet flex flex-column w-full">
        <Outlet />
      </div>
    </div>
  );
};
