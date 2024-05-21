import { Outlet } from 'react-router-dom';
import { RootSideBar } from './RootSideBar';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import useGetStreamGroup from '@lib/smAPI/StreamGroups/useGetStreamGroup';
import { GetStreamGroupRequest } from '@lib/smAPI/smapiTypes';

export const RootLayout = (): JSX.Element => {
  const sgquery = useGetStreamGroup({ SGName: 'ALL' } as GetStreamGroupRequest);
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  if (selectedStreamGroup === undefined && sgquery.data !== undefined) {
    setSelectedStreamGroup(sgquery.data);
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
