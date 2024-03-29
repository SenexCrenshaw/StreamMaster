import { Outlet } from 'react-router-dom';
import { RootSideBar } from './RootSideBar';

export const RootLayout = (): JSX.Element => {
  return (
    <div className="flex max-h-screen">
      <RootSideBar />

      <div className="flex flex-column w-full">
        <Outlet />
      </div>
    </div>
  );
};
