import { Outlet } from 'react-router-dom';
import { RootSideBar } from './RootSideBar';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import useGetStreamGroup from '@lib/smAPI/StreamGroups/useGetStreamGroup';
import { GetStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import { useIsTrue } from '@lib/redux/hooks/isTrue';

export const RootLayout = (): JSX.Element => {
  const { setIsTrue } = useIsTrue('streameditor-SMStreamDataSelector');
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const sgquery = useGetStreamGroup({ SGName: 'ALL' } as GetStreamGroupRequest);

  if (selectedStreamGroup === undefined && sgquery.data !== undefined) {
    setSelectedStreamGroup(sgquery.data);
  }

  const persistKey = 'persist:isTrue';
  const persistData = localStorage.getItem(persistKey);

  if (persistData === null) {
    setIsTrue(true);
  } else {
    try {
      const parsedData = JSON.parse(persistData);
      if (!parsedData.hasOwnProperty('streameditor-SMStreamDataSelector')) {
        setIsTrue(true);
      }
    } catch (error) {
      console.error('Error parsing JSON from localStorage:', error);
      setIsTrue(true); // In case of parsing error, assume it's not set correctly
    }
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
